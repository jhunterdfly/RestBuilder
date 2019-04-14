using System;
using System.Collections.Generic;
using System.Net.Http;

namespace RestBuilder.Service
{
    public partial class RestBuilder
    {
        protected static IRbLoggingService DefaultLoggingService { get; private set; }
        public static void SetDefaultLoggingService(IRbLoggingService loggingService)
        {
            DefaultLoggingService = loggingService;
        }
    }
    
    /// <summary>
    /// Rest builder. Generic class to support fluent style syntax for Http calls
    /// </summary>
    public partial class RestBuilder<TResult, TSend> : RestBuilder, IRestBuilder<TResult, TSend> where TResult : class where TSend : class
    {
        public IRbLoggingService LoggingService { get; private set; } = DefaultLoggingService;
        public IRestBuilder<TResult, TSend> WithLoggingService(IRbLoggingService loggingService)
        {
            LoggingService = loggingService ?? DefaultLoggingService;

            return this;
        }

        private bool ExcludeContentLogging = false;

        /// <summary>
        /// Allows the calling function to explicitly opt in or out of logging Send and Result content..
        /// </summary>
        /// <remarks>Logging Send and Result content is active by default and the calls must opt out if it is not required.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        public IRestBuilder<TResult, TSend> WithExcludeContentLogging(bool value)
        {
            ExcludeContentLogging = value;
            return this;
        }

        /// <summary>
        /// Prepares and sends a metric tracking event to App Insights.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        private void SendMetric(string eventName)
        {
            var properties = new Dictionary<string, string>
            {
                { nameof(TrackingIndicator), TrackingIndicator },
                { nameof(Uri), Uri.ToString() },
            };

            if (restResult.RequestMessage != null)
            {
                properties.Add("RequestMessage.RequestUri", $"{restResult.RequestMessage.Method} - {restResult.RequestMessage.RequestUri}");
                properties.Add($"Call Duration (milliseconds): {restResult.RequestMessage.Method} {Uri.AbsolutePath.Substring(1)}", restResult.CallDuration.ToString());

                if (restResult.RequestMessage.Method == HttpMethod.Put || restResult.RequestMessage.Method == HttpMethod.Post)
                {
                    properties.Add("SendContent", GetSendContentText());
                    properties.Add("SendContent.Length", (restResult.SendContent?.Length ?? 0).ToString());
                }
            }
            else
            {
                properties.Add("Call Duration (milliseconds)", restResult.CallDuration.ToString());
            }

            if (restResult.ResponseMessage != null)
            {
                properties.Add("ResponseMessage.StatusCode", ((int)restResult.ResponseMessage.StatusCode).ToString());
                properties.Add("ResponseMessage.StatusDesc", restResult.ResponseMessage.StatusCode.ToString());
                properties.Add("ResponseMessage.ReasonPhrase", restResult.ResponseMessage.ReasonPhrase);
            }

            properties.Add("ResultContent", GetResultContentText());
            properties.Add("ResultContent.Length", restResult.ResultContent.Length.ToString());

            properties.Add($"{restResult.TrackingIndicator}", $"Count: {trackingIndicatorCounts[restResult.TrackingIndicator]}");

            DefaultLoggingService.Log($"RestBuilder.SendMetric {eventName}", properties);

            // review properties and ensure they are less then 125 characters in length
            // needed to minimize AppCenter warnings for analytics tracking
            //var properties2 = new Dictionary<string, string>();

            //foreach ( var key in properties.Keys)
            //{
            //  properties2.Add(key, (properties[key].Length > 125) ? properties[key].Substring(0, 120) + " ..." : properties[key]);
            //}

            //Analytics.TrackEvent(eventName, properties2);
        }

        /// <summary>
        /// returns the result content, limited to a max length (1000 chars)
        /// </summary>
        /// <returns>The tracking content.</returns>
        private string GetSendContentText()
        {
            if (restResult == null)
                return "null restResult object";

            if (ExcludeContentLogging)
                return "Excluded";

            string contentText = null;
            int contentTrackingLength = 1000;

            if (string.IsNullOrEmpty(restResult.SendContent))
                contentText = (restResult.SendContent == null) ? "Null content sent." : "Empty Content sent";
            else if (restResult.SendContent.Length > contentTrackingLength)
                contentText = $"{Substring(restResult.SendContent, contentTrackingLength)} ...";
            else
                contentText = restResult.SendContent;

            return contentText;
        }

        /// <summary>
        /// returns the result content, limited to a max length (1000 chars)
        /// </summary>
        /// <returns>The tracking content.</returns>
        private string GetResultContentText()
        {
            if (restResult == null)
                return "null restResult object";

            if (ExcludeContentLogging)
                return "Excluded";

            string contentText = null;
            int contentTrackingLength = 1000;

            if (string.IsNullOrEmpty(restResult.ResultContent))
                contentText = (restResult.ResultContent == null) ? "Null content returned." : "Empty Content returned";
            else if (restResult.ResultContent.Length > contentTrackingLength)
                contentText = $"{Substring(restResult.ResultContent, contentTrackingLength)} ...";
            else
                contentText = restResult.ResultContent;

            return contentText;
        }

        /// <summary>
        /// Helper method for obtaining a safe substring.
        /// </summary>
        /// <returns>The substring.</returns>
        /// <param name="input">Input.</param>
        /// <param name="length">Length.</param>
        private string Substring(string input, int length)
        {
            return input.Length >= length ? input.Substring(0, length) : input;
        }
    }
}
