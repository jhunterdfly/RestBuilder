using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RestBuilder.Service
{
#pragma warning disable RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    /// <summary>
    /// Rest builder. used for fluent style syntax for processing Http calls
    /// </summary>
    /// <remarks>The Restbuilder class is used to house <see langword="static"/> methods and properties.
    /// Since the class is inherited by RestBuilder<TResult,TSend> it should not be marked <see langword="static"/> </remarks>
    public partial class RestBuilder
#pragma warning restore RECS0014 // If all fields, properties and methods members are static, the class can be made static.
    {
        protected static readonly HttpClient httpClient = new HttpClient();
        protected static string AppVersion;

        static RestBuilder()
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            DefaultAuthorizationService = new NoneRbAuthorizationService();
            DefaultLoggingService = new ConsoleRbLoggingService();
        }

        //TODO this might be redundant
        public static void SetApplicationVersion(string appVersion)
        {
            AppVersion = appVersion;
        }

        protected static readonly IDictionary<string, DateTime> alertTracking = new Dictionary<string, DateTime>();
        protected static readonly IDictionary<string, int> trackingIndicatorCounts = new Dictionary<string, int>();
    }

    /// <summary>
    /// Rest builder. Generic class to support fluent style syntax for Http calls
    /// </summary>
    public partial class RestBuilder<TResult, TSend> : RestBuilder, IRestBuilder<TResult, TSend> where TResult : class where TSend : class
    {
        private readonly IDictionary<string, string> requestHeaders = new Dictionary<string, string>();
        private readonly IDictionary<HttpStatusCode, Func<RestResult<TResult, TSend>, Task>> statusResponseFuncs = new Dictionary<HttpStatusCode, Func<RestResult<TResult, TSend>, Task>>();

        public RestBuilder()
        {
            InitializeStatusReponseFunctions();

            GenerateSendContent = GenerateStringSendContent;
        }

        private void InitializeStatusReponseFunctions()
        {

            //statusResponseFuncs.Add(HttpStatusCode.OK, DefaultStatusResponseOk);
            //statusResponseFuncs.Add(HttpStatusCode.Created, DefaultStatusResponseOk);

            statusResponseFuncs.Add(HttpStatusCode.RequestTimeout, DefaultStatusResponsePoorConnectivity);
            statusResponseFuncs.Add(HttpStatusCode.GatewayTimeout, DefaultStatusResponsePoorConnectivity);
            statusResponseFuncs.Add(HttpStatusCode.Forbidden, DefaultStatusResponseForbidden);
            statusResponseFuncs.Add(HttpStatusCode.ServiceUnavailable, DefaultStatusResponseServerUnavailable);
        }


        /// <summary>
        /// Gets or sets the URI that Http call is intended for.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; internal set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public HttpMethod Method { get; internal set; }

        /// <summary>
        /// Gets or sets the send object.
        /// </summary>
        /// <value>The send object.</value>
        public TSend SendObject { get; internal set; }


        /// <summary>
        /// Allows the calling function to specify if internet connectivity should be checked prior to attempting the Http call.
        /// </summary>
        /// <remarks>Connectivity Precheck is not required by default. If included, the ExecuteAsync call will abort the call if connectivity precheck fails</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        public IRestBuilder<TResult, TSend> WithConnectivityPrecheck(bool value, Action<RestResult<TResult, TSend>> offlineAction = null)
        {
            ConnectivityOfflineAction = offlineAction;
            ConnectivityPrecheck = value;
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Ecolab.CustomerAuditApplication.RestServices.RestBuilder`2"/>
        /// connectivity precheck.
        /// </summary>
        /// <value><c>true</c> if connectivity precheck; otherwise, <c>false</c>.</value>
        public bool ConnectivityPrecheck { get; private set; } = false;

        /// <summary>
        /// Gets the Connectivity Offline action.
        /// </summary>
        /// <value>The offline action.</value>
        public Action<RestResult<TResult, TSend>> ConnectivityOfflineAction { get; private set; }


        private void DefaultConnectivityOfflineAction(RestResult<TResult, TSend> rest)
        {
            //var networkStateManager = SimpleIoc.Default.GetInstance<INetworkStateManager>();

            //if (networkStateManager.GetConnectivityState() == NetworkStates.Offline)
            //{
            //    restResult.ResultContent = "ConnectivityPrecheck showed status was offline.  Aborting Http call.";
            //    Task.Run(async () => await networkStateManager.DisplayNoConnectionAlert1(restResult.TrackingIndicator));
            //}

            //if (networkStateManager.GetConnectivityState() == NetworkStates.Unreachable)
            //{
            //    restResult.ResultContent = "ConnectivityPrecheck showed network status was Unreachable.  Aborting Http call.";
            //    Task.Run(async () => await networkStateManager.DisplayPoorConnectivityAlert(restResult.TrackingIndicator));
            //}

            restResult.AlertMessageDisplayed = true;
        }


        /// <summary>
        /// Specifies an awaitable function to execute in response to a specified Http Status Code
        /// </summary>
        /// <remarks>Setting a new func for a status code that has been added (e.g. default funcs) will REPLACE the default or prior func with the stated version.
        /// Setting the func to null will remove all functions for that status code (some are defaulted by RestBuilder).  Multiple occurrences of this method are allowed.</remarks>
        /// <returns>The status response.</returns>
        /// <param name="statusCode">Status code.</param>
        /// <param name="func">the awaitable function to execute.</param>
        public IRestBuilder<TResult, TSend> WithStatusResponse(HttpStatusCode statusCode, Func<RestResult<TResult, TSend>, Task> func)
        {
            if (func != null)
                statusResponseFuncs[statusCode] = func;
            else if (func == null && statusResponseFuncs.ContainsKey(statusCode))
                statusResponseFuncs.Remove(statusCode);

            return this;
        }

        private async Task DefaultStatusResponsePoorConnectivity(RestResult<TResult, TSend> result)
        {
            var now = DateTime.UtcNow;
            var key = "PoorConnectivity";
            int minMinutes = 5;

            if (alertTracking.ContainsKey(key) && now.Subtract(alertTracking[key]).TotalMinutes < minMinutes)
                return;

            alertTracking[key] = now;

            //var networkStateManager = SimpleIoc.Default.GetInstance<INetworkStateManager>();
            //await networkStateManager.DisplayPoorConnectivityAlert($"{restResult.TrackingIndicator}-{(int)restResult.ResponseMessage.StatusCode}");
            restResult.AlertMessageDisplayed = true;
        }

        private async Task DefaultStatusResponseServerUnavailable(RestResult<TResult, TSend> result)
        {
            var now = DateTime.UtcNow;
            var key = "ServerUnavailable";
            int minMinutes = 10;

            if (alertTracking.ContainsKey(key) && now.Subtract(alertTracking[key]).TotalMinutes < minMinutes)
                return;

            alertTracking[key] = now;

            //var networkStateManager = SimpleIoc.Default.GetInstance<INetworkStateManager>();
            //await networkStateManager.DisplayServerUnavailableAlert($"{restResult.TrackingIndicator}-{(int)restResult.ResponseMessage.StatusCode}");
            restResult.AlertMessageDisplayed = true;
        }

        private async Task DefaultStatusResponseForbidden(RestResult<TResult, TSend> result)
        {
            if (result.ResultContent.Contains("<!DOCTYPE html>"))
            {
                // this is a server is down error, other 403 errors should be handled by the calling function
                //var networkStateManager = SimpleIoc.Default.GetInstance<INetworkStateManager>();
                //await networkStateManager.DisplayResourceUnavailableAlert($"{restResult.TrackingIndicator}-{(int)restResult.ResponseMessage.StatusCode}");
                restResult.AlertMessageDisplayed = true;
            }
        }



        /// <summary>
        /// Specifies a Request Header to use with a specific call.
        /// </summary>
        /// <remarks>Multiple occurrences of this method are allowed.
        /// No action is performed if either the key or the value are null or empty.</remarks>
        /// <returns>The request header.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public IRestBuilder<TResult, TSend> WithRequestHeader(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                requestHeaders[key] = value;

            return this;
        }


        private SendContentType _sendContentType = SendContentType.String;
        private Func<RestResult<TResult, TSend>, HttpContent> GenerateSendContent = null;

        /// <summary>
        /// Specifies the content type to send (put, post, etc) to the API call.
        /// </summary>
        /// <remarks>Defaults to StringContent Type if not specified.  Custom Type requires a <paramref name="generateCustomSendContent"/> to be supplied, or an ArgumentNullException will be thrown.</remarks>
        /// <returns>The send content type.</returns>
        /// <param name="sendContentType">Send content type.</param>
        /// <param name="generateCustomSendContent">Optional Content Type function to use for custom content type generation</param>
        public IRestBuilder<TResult, TSend> WithSendContentType(SendContentType sendContentType, Func<RestResult<TResult, TSend>, HttpContent> generateCustomSendContent = null)
        {
            _sendContentType = sendContentType;

            switch (sendContentType)
            {
                case SendContentType.String:
                    GenerateSendContent = GenerateStringSendContent;
                    break;
                case SendContentType.FormUrlEncodedContent:
                    GenerateSendContent = GenerateFormUrlSendContent;
                    break;
                case SendContentType.Custom:
                    GenerateSendContent = generateCustomSendContent;
                    break;
            }

            return this;
        }

        private HttpContent GenerateStringSendContent(RestResult<TResult, TSend> res)
        {
            return new StringContent(res.SendContent, Encoding.UTF8, "application/json");
        }

        private HttpContent GenerateFormUrlSendContent(RestResult<TResult, TSend> res)
        {
            var send = res.SendObject as IEnumerable<KeyValuePair<string, string>>;

            if (send == null)
            {
                var ex = new ArgumentException("RestResult.SendObject is wrong type.  It must be of type 'IEnumerable<KeyValuePair<string, string>>'.");
                ex.Data.Add(nameof(TrackingIndicator), TrackingIndicator);
                ex.Data.Add(nameof(Uri), Uri);

                restResult.ResultException = ex;
                LoggingService?.Log(ex);

                ExceptionAction?.Invoke(restResult);

                return null;
            }

            return new FormUrlEncodedContent(send);
        }



        /// <summary>
        /// Sets the tracking indicator. a unique string representing a single REST call
        /// </summary>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">Value.</param>
        public IRestBuilder<TResult, TSend> WithTrackingIndicator(string value)
        {
            TrackingIndicator = value;
            return this;
        }

        /// <summary>
        /// Gets the tracking indicator.
        /// </summary>
        /// <remarks>Each Http Call must have it's own unique tracking identifier</remarks>
        /// <value>The tracking indicator.</value>
        public string TrackingIndicator { get; private set; }

        /// <summary>
        /// Gets the success action.
        /// </summary>
        /// <value>The success action.</value>
        public Action<RestResult<TResult, TSend>> SuccessAction { get; private set; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call is successful.
        /// </summary>
        /// <remarks>The SuccessAction is optional and if implmented will execuate after all the default handling has completed </remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        public IRestBuilder<TResult, TSend> WithSuccessAction(Action<RestResult<TResult, TSend>> action)
        {
            SuccessAction = action;
            return this;
        }

        /// <summary>
        /// Gets the failure action.
        /// </summary>
        /// <value>The failure action.</value>
        public Action<RestResult<TResult, TSend>> FailureAction { get; private set; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call is not successful.
        /// </summary>
        /// <remarks>The FailureAction is optional and if implmented will execuate after all the default handling has completed.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        public IRestBuilder<TResult, TSend> WithFailureAction(Action<RestResult<TResult, TSend>> action)
        {
            FailureAction = action;
            return this;
        }

        /// <summary>
        /// Gets the exception action.
        /// </summary>
        /// <value>The exception action.</value>
        public Action<RestResult<TResult, TSend>> ExceptionAction { get; private set; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call encounters an exception.
        /// </summary>
        /// <remarks>The ExceptionAction is optional and if implmented will execuate after all the default handling has completed.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        public IRestBuilder<TResult, TSend> WithExceptionAction(Action<RestResult<TResult, TSend>> action)
        {
            ExceptionAction = action;
            return this;
        }

        /// <summary>
        /// Gets the success criteria func.
        /// </summary>
        /// <remarks>The SuccessCriteriaFunc is optional and if implmented will replace the default method of determining success.</remarks>
        /// <value>The success criteria func.</value>
        public Func<HttpResponseMessage, bool> SuccessCriteriaFunc { get; private set; }

        /// <summary>
        /// Allows the calling function to optionally specify a Func to specify different criteria to determine if an Http call was successful.
        /// </summary>
        /// <remarks>The default criteria func just calls IsSuccessStatusCode()</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="func">Func.</param>
        public IRestBuilder<TResult, TSend> WithSuccessCriteria(Func<HttpResponseMessage, bool> func)
        {
            SuccessCriteriaFunc = func;
            return this;
        }

        /// <summary>
        /// Defaults the success criteria func.
        /// </summary>
        /// <returns><c>true</c>, if success criteria func was defaulted, <c>false</c> otherwise.</returns>
        /// <param name="response">Response.</param>
        private bool DefaultSuccessCriteriaFunc(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode;
        }


        /// <summary>
        /// Gets the process result content action.
        /// </summary>
        /// <value>The process result content action.</value>
        public Action<RestResult<TResult, TSend>> ResultContentAction { get; private set; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action to process the result content.
        /// </summary>
        /// <remarks>The ResultContentAction is optional and if implmented will execute upon successful call to convert the  </remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        public IRestBuilder<TResult, TSend> WithResultContentAction(Action<RestResult<TResult, TSend>> action)
        {
            ResultContentAction = action;
            return this;
        }
    }
}
