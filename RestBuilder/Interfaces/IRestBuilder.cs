using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestBuilder.Service
{
    public interface IRestBuilder<TResult, TSend> where TResult : class where TSend : class
    {
        /// <summary>
        /// Gets or sets the URI that Http call is intended for.
        /// </summary>
        /// <value>The URI.</value>
        Uri Uri { get; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        HttpMethod Method { get; }

        /// <summary>
        /// Gets the tracking indicator.
        /// </summary>
        /// <remarks>Each Http Call must have it's own unique tracking identifier</remarks>
        /// <value>The tracking indicator.</value>
        string TrackingIndicator { get; }

        /// <summary>
        /// Sets the tracking indicator. a unique string representing a single REST call
        /// </summary>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">Value.</param>
        IRestBuilder<TResult, TSend> WithTrackingIndicator(string value);

        /// <summary>
        /// Allows the calling function to explicitly opt out of authorization for Http Calls..
        /// </summary>
        /// <remarks>Authorization is required by default, so calling function must opt out if authorization is not required.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        IRestBuilder<TResult, TSend> WithNoAuthorizationRequired();

        /// <summary>
        /// Sets the type of Authorization to be used in the call.
        /// </summary>
        /// <returns>The RestBuilder object used to build/execute the Http call.</returns>
        IRestBuilder<TResult, TSend> WithAuthorizationService(IRbAuthorizationService authorizationService);

        /// <summary>
        /// Sets the type of LoggingService  to be used in the call.
        /// </summary>
        /// <returns>The RestBuilder object used to build/execute the Http call.</returns>
        IRestBuilder<TResult, TSend> WithLoggingService(IRbLoggingService loggingService);

        /// <summary>
        /// Allows the calling function to optionally specify a Func to specify different criteria to determine if an Http call was successful.
        /// </summary>
        /// <remarks>The default criteria func just calls IsSuccessStatusCode()</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="func">Func.</param>
        IRestBuilder<TResult, TSend> WithSuccessCriteria(Func<HttpResponseMessage, bool> func);

        /// <summary>
        /// Gets the success criteria func.
        /// </summary>
        /// <remarks>The SuccessCriteriaFunc is optional and if implmented will replace the default method of determining success.</remarks>
        /// <value>The success criteria func.</value>
        Func<HttpResponseMessage, bool> SuccessCriteriaFunc { get; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call is successful.
        /// </summary>
        /// <remarks>The SuccessAction is optional and if implmented will execuate after all the default handling has completed </remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        IRestBuilder<TResult, TSend> WithSuccessAction(Action<RestResult<TResult, TSend>> action);

        /// <summary>
        /// Gets the success action.
        /// </summary>
        /// <value>The success action.</value>
        Action<RestResult<TResult, TSend>> SuccessAction { get; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call is not successful.
        /// </summary>
        /// <remarks>The FailureAction is optional and if implmented will execuate after all the default handling has completed.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        IRestBuilder<TResult, TSend> WithFailureAction(Action<RestResult<TResult, TSend>> action);

        /// <summary>
        /// Gets the failure action.
        /// </summary>
        /// <value>The failure action.</value>
        Action<RestResult<TResult, TSend>> FailureAction { get; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action when the Http call encounters an exception.
        /// </summary>
        /// <remarks>The ExceptionAction is optional and if implmented will execuate after all the default handling has completed.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        IRestBuilder<TResult, TSend> WithExceptionAction(Action<RestResult<TResult, TSend>> action);

        /// <summary>
        /// Gets the exception action.
        /// </summary>
        /// <value>The exception action.</value>
        Action<RestResult<TResult, TSend>> ExceptionAction { get; }

        /// <summary>
        /// Allows the calling function to specify a default object for use in the event that an Http call is not successful.
        /// </summary>
        /// <remarks>if a default object is not provided, a reasonable non-null default will be applied.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="defaultObject">Default object.</param>
        IRestBuilder<TResult, TSend> WithDefaultResultObject(TResult defaultObject);

        /// <summary>
        /// Gets the default result object.
        /// </summary>
        /// <value>The default result object.</value>
        TResult DefaultResultObject { get; }


        /// <summary>
        /// Gets the process result content action.
        /// </summary>
        /// <value>The process result content action.</value>
         Action<RestResult<TResult, TSend>> ResultContentAction { get; }

        /// <summary>
        /// Allows the calling function to optionally specify an Action to process the result content.
        /// </summary>
        /// <remarks>The ResultContentAction is optional and if implmented will execute upon successful call to convert the  </remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="action">Action.</param>
        IRestBuilder<TResult, TSend> WithResultContentAction(Action<RestResult<TResult, TSend>> action);

        /// <summary>
        /// Gets or sets the send object.
        /// </summary>
        /// <value>The send object.</value>
        TSend SendObject { get; }


        /// <summary>
        /// Allows the calling function to specify if internet connectivity should be checked prior to attempting the Http call.
        /// </summary>
        /// <remarks>Connectivity Precheck is not required by default. If included, the ExecuteAsync call will abort the call if connectivity precheck fails</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        IRestBuilder<TResult, TSend> WithConnectivityPrecheck(bool value, Action<RestResult<TResult, TSend>> offlineAction = null);

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:RestBuilder.Service.RestBuilder`2"/>
        /// connectivity precheck.
        /// </summary>
        /// <value><c>true</c> if connectivity precheck; otherwise, <c>false</c>.</value>
        bool ConnectivityPrecheck { get; }

        /// <summary>
        /// Gets the Connectivity Offline action.
        /// </summary>
        /// <value>The offline action.</value>
        Action<RestResult<TResult, TSend>> ConnectivityOfflineAction { get; }



        ///// <summary>
        ///// Gets the type of the authorization.
        ///// </summary>
        ///// <value>The type of the authorization.</value>
        //CallAuthorizationType AuthorizationType { get; }

        /// <summary>
        /// Specifies an awaitable function to execute in response to a specified Http Status Code
        /// </summary>
        /// <remarks>Setting a new func for a status code that has been added (e.g. default funcs) will REPLACE the default or prior func with the stated version.
        /// Setting the func to null will remove all functions for that status code (some are defaulted by RestBuilder).  Multiple occurrences of this method are allowed.</remarks>
        /// <returns>The status response.</returns>
        /// <param name="statusCode">Status code.</param>
        /// <param name="func">the awaitable function to execute.</param>
        IRestBuilder<TResult, TSend> WithStatusResponse(HttpStatusCode statusCode, Func<RestResult<TResult, TSend>, Task> func);


        /// <summary>
        /// Specifies a Request Header to use with a specific call.
        /// </summary>
        /// <remarks>Multiple occurrences of this method are allowed.</remarks>
        /// <returns>The request header.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        IRestBuilder<TResult, TSend> WithRequestHeader(string key, string value);

        /// <summary>
        /// Specifies the content type to send (put, post, etc) to the API call.
        /// </summary>
        /// <remarks>Defaults to StringContent Type if not specified.  Custom Type requires a <paramref name="generateCustomSendContent"/> to be supplied, or an ArgumentNullException will be thrown.</remarks>
        /// <returns>The send content type.</returns>
        /// <param name="sendContentType">Send content type.</param>
        /// <param name="generateCustomSendContent">Optional Content Type function to use for custom content type generation</param>
        IRestBuilder<TResult, TSend> WithSendContentType(SendContentType sendContentType, Func<RestResult<TResult, TSend>, HttpContent> generateCustomSendContent = null);

        /// <summary>
        /// Allows the calling function to explicitly opt in or out of logging Send and Result content..
        /// </summary>
        /// <remarks>Logging Send and Result content is active by default and the calls must opt out if it is not required.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        IRestBuilder<TResult, TSend> WithExcludeContentLogging(bool value);

        /// <summary>
        /// Executes the Http Call, based on RestBuilder properties
        /// </summary>
        /// <returns>The async.</returns>
        Task<RestResult<TResult, TSend>> ExecuteAsync();
    }
}
