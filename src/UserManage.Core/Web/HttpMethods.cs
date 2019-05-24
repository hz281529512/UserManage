using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserManage.Web
{
    internal partial class HttpMethods
    {

        public static string RestGet(string url)
        {
            RestClient rest = new RestClient(url);
            var request = new RestRequest(Method.GET);
            var response = rest.Execute(request);
            return response.Content;
        }

        public static Task<string> RestGetAsync(string route, Dictionary<string, string> param = null)
        {
            RestClient client = new RestClient(route);
            var request = new RestRequest(Method.GET);
            var tcs = new TaskCompletionSource<string>();
            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            if (param != null)
            {
                foreach (var item in param)
                {
                    request.AddParameter(item.Key, item.Value);
                }
            }

            //IRestResponse response = client.Execute(request);
            client.ExecuteAsync(request, response => { tcs.SetResult(response.Content); });
            return tcs.Task;
        }

        public static Task<string> RestPostAsync(string url, string param = null)
        {
            RestClient rest = new RestClient(url);
            var request = new RestRequest(Method.POST);
            var tcs = new TaskCompletionSource<string>();
            request.AddParameter("application/x-www-form-urlencoded", param, ParameterType.RequestBody);
            rest.ExecuteAsync(request, response => { tcs.SetResult(response.Content); });
            return tcs.Task;
        }

        public static string RestJsonPost(string route, Dictionary<string, object> param = null)
        {
            RestClient client = new RestClient(route);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");


            if (param != null)
            {
                var json_param = JsonConvert.SerializeObject(param);
                request.AddParameter("undefined", json_param, ParameterType.RequestBody);
            }

            IRestResponse response = client.Execute(request);
            return response.Content;

        }
    }
}
