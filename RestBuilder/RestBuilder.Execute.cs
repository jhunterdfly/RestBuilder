using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RestBuilder.Service
{
    public partial class RestBuilder
    {
    }

    /// <summary>
    /// Rest builder. Generic class to support fluent style syntax for Http calls
    /// </summary>
    public partial class RestBuilder<TResult, TSend> : RestBuilder, IRestBuilder<TResult, TSend> where TResult : class where TSend : class
    {
        /// <summary>
        /// Gets the default result object.
        /// </summary>
        /// <value>The default result object.</value>
        public TResult DefaultResultObject { get; private set; } = null;

        /// <summary>
        /// Allows the calling function to specify a default object for use in the event that an Http call is not successful.
        /// </summary>
        /// <remarks>if a default object is not provided, a reasonable non-null default will be applied.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="defaultObject">Default object.</param>
        public IRestBuilder<TResult, TSend> WithDefaultResultObject(TResult defaultObject)
        {
            DefaultResultObject = defaultObject;
            return this;
        }

        /// <summary>
        /// Gets the default result object.
        /// </summary>
        /// <returns>The default result object.</returns>
        private TResult GetDefaultResultObject()
        {
            return DefaultResultObject ?? CreateDefaultObject();
        }

        /// <summary>
        /// Creates a reasonable non-null default object for use in return objects to calling function.
        /// </summary>
        /// <remarks>Currently any object defaulted must have a no-argument constructor available for the default.</remarks>
        /// <returns>The default object.</returns>
        private TResult CreateDefaultObject()
        {
            try
            {
                if (typeof(TResult).Equals(typeof(String)) || typeof(TResult).Equals(typeof(string)))
                    return string.Empty as TResult;

                if (typeof(TResult).Equals(typeof(bool)))
                    return false as TResult;

                if (typeof(TResult).Equals(typeof(StringContent)))
                    return new StringContent(string.Empty) as TResult;

                //TODO: expand to support multiple paramater constructors
                var defaultObject = Activator.CreateInstance<TResult>();

                return defaultObject;
            }
            catch (Exception exc)
            {
                exc.Data.Add("Note", $"Ensure that type {typeof(TResult).FullName} has a zero-argument constructor.");
                LoggingService?.Log(exc);
            }

            return null;
        }

        /// <summary>
        /// a RestResult property containing information about the results of a particular Htpp call.
        /// </summary>
        RestResult<TResult, TSend> restResult = null;


        /// <summary>
        /// Executes the Http Call, based on RestBuilder properties
        /// </summary>
        /// <returns>The async.</returns>
        public async Task<RestResult<TResult, TSend>> ExecuteAsync()
        {
            CreateRestResult();

            if (!ConfirmTrackingIndicator())
                return restResult;

            if (trackingIndicatorCounts.ContainsKey(restResult.TrackingIndicator))
                trackingIndicatorCounts[restResult.TrackingIndicator]++;
            else
                trackingIndicatorCounts[restResult.TrackingIndicator] = 1;

            if (ConnectivityPrecheck)
            {
                //var networkStateManager = SimpleIoc.Default.GetInstance<INetworkStateManager>();

                //if (networkStateManager != null && networkStateManager.GetConnectivityState() != NetworkStates.Online)
                {
                    ConnectivityOfflineAction = ConnectivityOfflineAction ?? DefaultConnectivityOfflineAction;

                    ConnectivityOfflineAction?.Invoke(restResult);

                    return restResult;
                }

            }

            SuccessCriteriaFunc = SuccessCriteriaFunc ?? DefaultSuccessCriteriaFunc;
            CreateRequestMethod();

            ApplySendContent();

            var stopwatch = new System.Diagnostics.Stopwatch();

            try
            {
                ApplyAuthorization();

                stopwatch.Start();
                restResult.ResponseMessage = await httpClient.SendAsync(restResult.RequestMessage).ConfigureAwait(false);
                stopwatch.Stop();
                restResult.CallDuration = stopwatch.ElapsedMilliseconds;

                restResult.StatusCode = restResult.ResponseMessage.StatusCode;
                restResult.ResultContent = await restResult.ResponseMessage.Content.ReadAsStringAsync();

                await ExecuteStatusResponse();

                if (SuccessCriteriaFunc(restResult.ResponseMessage))
                {
                    (ResultContentAction ?? DefaultResultContentAction)?.Invoke(restResult);

                    restResult.RestCallSucceeded = true;
                    SendMetric($"{TrackingIndicator} - Rest Call Succeeded");
                    SuccessAction?.Invoke(restResult);
                }
                else
                {
                    SendMetric($"{TrackingIndicator} - Rest Call Failed");
                    FailureAction?.Invoke(restResult);
                }
            }
            catch (Exception exc)
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                    restResult.CallDuration = stopwatch.ElapsedMilliseconds;
                }

                exc.Data.Add(nameof(TrackingIndicator), TrackingIndicator);
                exc.Data.Add(nameof(Uri), Uri);

                if (restResult.RequestMessage != null)
                {
                    exc.Data.Add("RequestMessage.RequestUri", $"{restResult.RequestMessage.Method} - {restResult.RequestMessage.RequestUri}");
                    exc.Data.Add($"Call Duration (milliseconds): {restResult.RequestMessage.Method} {Uri.AbsolutePath.Substring(1)}", restResult.CallDuration.ToString());

                    if (restResult.RequestMessage.Method == HttpMethod.Put || restResult.RequestMessage.Method == HttpMethod.Post)
                    {
                        exc.Data.Add("SendContent", GetSendContentText());
                        exc.Data.Add("SendContent.Length", (restResult.SendContent?.Length ?? 0).ToString());
                    }
                }
                else
                {
                    exc.Data.Add("Call Duration (milliseconds)", restResult.CallDuration.ToString());
                }

                if (restResult.ResponseMessage != null)
                {
                    restResult.StatusCode = restResult.ResponseMessage.StatusCode;
                    exc.Data.Add("ResponseMessage.StatusCode", (int)restResult.ResponseMessage.StatusCode);
                    exc.Data.Add("ResponseMessage.StatusDesc", restResult.ResponseMessage.StatusCode);
                    exc.Data.Add("ResponseMessage.ReasonPhrase", restResult.ResponseMessage.ReasonPhrase);
                }

                exc.Data.Add("ResultContent", GetResultContentText());
                exc.Data.Add("CallDuration", restResult.CallDuration.ToString());

                restResult.ResultException = exc;

                LoggingService?.Log(exc);

                ExceptionAction?.Invoke(restResult);
            }

            return restResult;
        }

        /// <summary>
        /// Defaults the result content action TO JSON deserialization
        /// </summary>
        /// <param name="result">Result.</param>
        private void DefaultResultContentAction(RestResult<TResult, TSend> result)
        {
            if (!string.IsNullOrEmpty(restResult.ResultContent)
                && !string.Equals("null", restResult.ResultContent)
                && !typeof(TResult).Equals(typeof(String))
                && !typeof(TResult).Equals(typeof(string))
               )
            {
                restResult.ResultObject = JsonConvert.DeserializeObject<TResult>(restResult.ResultContent);
            }
        }

        private bool ConfirmTrackingIndicator()
        {
            if (!string.IsNullOrWhiteSpace(TrackingIndicator))
                return true;

            var ex = new ArgumentException("TrackingIndicator is required.");
            ex.Data.Add(nameof(TrackingIndicator), TrackingIndicator);
            ex.Data.Add(nameof(Uri), Uri);

            restResult.ResultException = ex;
            LoggingService?.Log(ex);

            ExceptionAction?.Invoke(restResult);

            return false;
        }

        private void CreateRequestMethod()
        {
            restResult.RequestMessage = new HttpRequestMessage(Method, Uri);

            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    restResult.RequestMessage.Headers.Add(header.Key, header.Value);
                }
            }
        }

        private async Task ExecuteStatusResponse()
        {
            if (!statusResponseFuncs.ContainsKey(restResult.StatusCode))
                return;

            var func = statusResponseFuncs[restResult.StatusCode];

            await func(restResult);
        }


        /// <summary>
        /// Factory method to create the RestResult object.
        /// </summary>
        private void CreateRestResult()
        {
            restResult = new RestResult<TResult, TSend>
            {
                TrackingIndicator = this.TrackingIndicator,
                SendObject = this.SendObject,
                ResultObject = GetDefaultResultObject()
            };
        }

        /// <summary>
        /// Helper method to process the Send object for the Http Put and Post calls
        /// </summary>
        private void ApplySendContent()
        {
            if (SendObject == default(TSend))
                return;

            if (_sendContentType == SendContentType.Custom && GenerateSendContent == null)
            {
                var ex = new ArgumentNullException(nameof(GenerateSendContent), $"{nameof(GenerateSendContent)} must be supplied when using SendContentType.Custom");
                ex.Data.Add(nameof(TrackingIndicator), TrackingIndicator);
                ex.Data.Add(nameof(Uri), Uri);

                restResult.ResultException = ex;

                LoggingService?.Log(ex);

                ExceptionAction?.Invoke(restResult);

                return;
            }

            restResult.SendContent = JsonConvert.SerializeObject(SendObject, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-ddTHH:mm:ss" });

            restResult.RequestMessage.Content = GenerateSendContent(restResult);
        }

    }
}
