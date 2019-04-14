using System;
using System.Threading.Tasks;

namespace RestBuilder.Sample
{
    public interface IBinRestService
    {
        Task<PostBin> CreateBin();
        Task<PostBin> GetBin(string binId);
        Task<PostBin> GetBin(string binId, PostBin defaultPostBin);
        Task<bool> DeleteBin(string binId);
        Task<string> PostUser(string binId, User user);
        Task<string> PostUserWithException(string binId, User user);
        Task<User> GetUser(string binId, string userRequestId);
        Task<User> GetUser2(string binId, string userRequestId);
    }
}
