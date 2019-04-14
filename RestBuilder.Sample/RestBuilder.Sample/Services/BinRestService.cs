using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestBuilder.Service;

namespace RestBuilder.Sample
{
    public class BinRestService : IBinRestService
    {
        public async Task<PostBin> CreateBin()
        {
            var url = "https://postb.in/api/bin";

            var result = await Rest.Post<PostBin>(url)
                                   .WithTrackingIndicator("RB1001")
                                   .ExecuteAsync();

            return result.ResultObject;
        }

        public async Task<PostBin> GetBin(string binId)
        {
            var url = $"https://postb.in/api/bin/{binId}";

            var result = await Rest.Get<PostBin>(url)
                                   .WithTrackingIndicator("RB1002a")
                                   .ExecuteAsync();

            return result.ResultObject;
        }

        public async Task<PostBin> GetBin(string binId, PostBin defaultPostBin)
        {
            var url = $"https://postb.in/api/bin/{binId}";

            var result = await Rest.Get<PostBin>(url)
                                   .WithTrackingIndicator("RB1002b")
                                   .WithDefaultResultObject(defaultPostBin)
                                   .ExecuteAsync();

            return result.ResultObject;
        }

        public async Task<bool> DeleteBin(string binId)
        {
            bool isSuccessful = false;

            var url = $"https://postb.in/api/bin/{binId}";

            var result = await Rest.Delete(url)
                                   .WithTrackingIndicator("RB1003")
                                   .WithSuccessAction(res =>
                                   {
                                       isSuccessful = true;
                                   })
                                   .ExecuteAsync();

            return isSuccessful;
        }

        public async Task<string> PostUser(string binId, User user)
        {
            string userRequestId = null;

            var url = $"https://postb.in/{binId}";

            var result = await Rest.Post<string, User>(url, user)
                                   .WithTrackingIndicator("RB1004")
                                   .WithSuccessAction(res =>
                                   {
                                       userRequestId = res.ResultContent;
                                   })
                                   .WithFailureAction(res =>
                                   {
                                       userRequestId = "Failure";
                                   })
                                   .ExecuteAsync();

            return userRequestId;
        }

        public async Task<string> PostUserWithException(string binId, User user)
        {
            string userRequestId = null;

            var url = $"https://postb.in/{binId}";

            var result = await Rest.Post<string, User>(url, user)
                                   .WithTrackingIndicator("RB1004")
                                   .WithSuccessAction(res =>
                                   {
                                       throw new ArgumentException("I don't like this user.");
                                   })
                                   .WithFailureAction(res =>
                                   {
                                       userRequestId = "Failure";
                                   })
                                   .WithExceptionAction(res =>
                                   {
                                       userRequestId = "Exception";
                                   })
                                   .ExecuteAsync();

            return userRequestId;
        }


        public async Task<User> GetUser(string binId, string userRequestId)
        {
            var url = $"https://postb.in/api/bin/{binId}/req/{userRequestId}";

            User user = null;

            var result = await Rest.Get<string>(url)
                                   .WithTrackingIndicator("RB1005a")
                                   .WithSuccessAction(res =>
                                   { 
                                       var jObject = JObject.Parse(res.ResultContent);

                                       if (jObject.ContainsKey("body"))
                                           user = JsonConvert.DeserializeObject<User>(jObject["body"].ToString());
                                   })
                                   .ExecuteAsync();

            return user;
        }


        public async Task<User> GetUser2(string binId, string userRequestId)
        {
            var url = $"https://postb.in/api/bin/{binId}/req/{userRequestId}";

            var result = await Rest.Get<User>(url)
                                   .WithTrackingIndicator("RB1005b")
                                   .WithResultContentAction(res =>
                                   {
                                       var jObject = JObject.Parse(res.ResultContent);

                                       if (jObject.ContainsKey("body"))
                                           res.ResultObject = JsonConvert.DeserializeObject<User>(jObject["body"].ToString());
                                   })
                                   .ExecuteAsync();

            return result.ResultObject;
        }
    }
}
