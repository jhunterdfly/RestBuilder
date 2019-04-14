using System;
namespace RestBuilder.Service
{
    public interface IRbAuthorizationService
    {
        CallAuthorizationType CallAuthorizationType { get; }
        string GetToken();
    }
}
