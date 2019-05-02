using System;
using System.Net;
using System.Net.Http;

namespace RestBuilder.Service
{
    public class RestResult<TResult, TSend> where TResult : class where TSend : class
    {
        public string TrackingIndicator { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public TSend SendObject { get; set; }
        public string SendContent { get; set; }
        public TResult ResultObject { get; set; }
        public string ResultContent { get; set; }
        public Exception ResultException { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }
        public bool AlertMessageDisplayed { get; set; }
        public long CallDuration { get; set; }
        public bool RestCallSucceeded { get; set; } = false;
    }
}
