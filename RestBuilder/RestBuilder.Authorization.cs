using System;
namespace RestBuilder.Service
{
	public partial class RestBuilder
	{
        protected static IRbAuthorizationService DefaultAuthorizationService { get; private set; }
        public static void SetDefaultAuthorizationService(IRbAuthorizationService authorizationService)
        {
            DefaultAuthorizationService = authorizationService;
        }
    }
    
     /// <summary>
    /// Rest builder. Generic class to support fluent style syntax for Http calls
    /// </summary>
    public partial class RestBuilder<TResult, TSend> : RestBuilder, IRestBuilder<TResult, TSend> where TResult : class where TSend : class
    {
        private bool AuthorizationRequired = true;

        /// <summary>
        /// Allows the calling function to explicitly opt out of authorization for Http Calls..
        /// </summary>
        /// <remarks>Authorization is required by default, so calling function must opt out if authorization is not required.</remarks>
        /// <returns>The Restbuilder object used to build/execute the Http call.</returns>
        public IRestBuilder<TResult, TSend> WithNoAuthorizationRequired()
        {
            AuthorizationRequired = false;

            return this;
        }

        public IRbAuthorizationService AuthorizationService { get; private set; } = DefaultAuthorizationService;
        public IRestBuilder<TResult, TSend> WithAuthorizationService(IRbAuthorizationService authorizationService)
        {
            AuthorizationService = authorizationService ?? DefaultAuthorizationService;

            return this;
        }

        /// <summary>
        /// Helper method to apply authorization if required.
        /// </summary>
        private void ApplyAuthorization()
        {
            if (!AuthorizationRequired)
                return;

            var authorizationService = AuthorizationService ?? DefaultAuthorizationService;

            if (authorizationService == null)
                return;

            var token = authorizationService.GetToken();

            switch (authorizationService.CallAuthorizationType)
            {
                case CallAuthorizationType.Bearer:
                    if (string.IsNullOrEmpty(token))
                        throw new ArgumentException("Bearer Authorization Token must be supplied.");

                    restResult.RequestMessage.Headers.Add("Authorization", "Bearer " + token);
                    break;
                case CallAuthorizationType.Sas:
                    if (string.IsNullOrEmpty(token))
                        throw new ArgumentException("SAS Authorization Token must be supplied.");

                    restResult.RequestMessage.Headers.Add("Authorization", token);
                    break;
                case CallAuthorizationType.Refresh:
                    break;
            }
        }
    }
}
