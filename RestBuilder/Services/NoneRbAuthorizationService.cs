using System;

namespace RestBuilder.Service
{
    public class NoneRbAuthorizationService : IRbAuthorizationService
    {
        public CallAuthorizationType CallAuthorizationType => CallAuthorizationType.None;

        public string GetToken()
        {
            return null;
        }
    }
}
