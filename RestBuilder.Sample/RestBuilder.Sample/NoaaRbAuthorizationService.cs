using System;
using RestBuilder.Service;

namespace RestBuilder.Sample
{
    public class NoaaRbAuthorizationService : IRbAuthorizationService
    {
        public CallAuthorizationType CallAuthorizationType => CallAuthorizationType.Bearer;

        public string GetToken()
        {
            return "mKFUPzCcCcsJqVGUNrOsUBAOuSSKlKuJ";
        }
    }
}
