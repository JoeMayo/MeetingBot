﻿using MeetingSchedulerDialog.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MeetingSchedulerDialog.Controllers
{
    public class WebChatController : ApiController
    {
        public async Task<HttpResponseMessage> Get()
        {
            string webChatSecret = ConfigurationManager.AppSettings["WebChatSecret"];

            string result = await GetIFrameViaPostWithToken(webChatSecret);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(result, Encoding.UTF8, "text/html");
            return response;
        }

        async Task<string> GetIFrameViaPostWithToken(string webChatSecret)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://webchat.botframework.com/api/conversations");
            request.Headers.Add("Authorization", "BOTCONNECTOR " + webChatSecret);

            HttpResponseMessage response = await new HttpClient().SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            WebChatTokenResponse webChatResponse = JsonConvert.DeserializeObject<WebChatTokenResponse>(responseJson);

            return $"<iframe width='800px' height='600px' src='https://webchat.botframework.com/embed/MeetingSchedulerBot?t={webChatResponse.Token}'></iframe>";
        }
    }
}