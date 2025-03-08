using Microsoft.AspNetCore.Http;
using TNG.Shared.Lib.Intefaces;
using RestSharp;

namespace TNG.Shared.Lib
{
    public class RestLayer : IRestLayer
    {
        public string MakePostCall<T>(T doc, string url, string pubToken)
        {
            try
            {
                var client = new RestClient();
                var request = new RestSharp.RestRequest(url, RestSharp.Method.POST)
                { RequestFormat = RestSharp.DataFormat.Json }
                                .AddJsonBody(doc);
                request.AddHeader("tngpubkey", pubToken);
                var response = client.Execute(request);
                return response.Content;
            }
            catch
            {
            }
            return string.Empty;

        }

        public string MakeGetCall(RestRequest req, string url, string pubToken = "")
        {
            try
            {
                var client = new RestClient();
                var request = new RestSharp.RestRequest(url, RestSharp.Method.POST)
                { RequestFormat = RestSharp.DataFormat.Json };
                foreach (var param in req.Parameters)
                {
                    request.AddParameter(param);
                }


                request.AddHeader("tngpubkey", pubToken);
                var response = client.Execute(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;

        }

        public string MakeBarterShopPostCall(RestRequest request, string url, string pubToken = "")
        {
            try
            {
                var client = new RestClient();
                request.AddHeader("tngpubkey", pubToken);
                var response = client.Execute(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return string.Empty;

        }




    }
}