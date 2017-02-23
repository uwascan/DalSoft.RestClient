﻿using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DalSoft.RestClient.Handlers
{
    internal class DefaultJsonHandler : DelegatingHandler
    {
        private readonly Config _config;

        public DefaultJsonHandler(Config config)
        {
            _config = config ?? new Config();
        }        
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_config.UseDefaultHandlers)
            {
                request.Content = GetContent(request);
                
                if (!request.Headers.Accept.Any())
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Config.JsonMediaType));

                request.ExpectJsonResponse(true);
            }
            
            return await base.SendAsync(request, cancellationToken);
        }

        private static HttpContent GetContent(HttpRequestMessage request)
        {
            var content = request.GetContent();            

            if (content == null)
                return null;

            var httpContent = new StringContent(JsonConvert.SerializeObject(content));

            httpContent.Headers.Clear(); //Clear the defaults we want to control all the headers
            httpContent.Headers.Add("Content-Type", request.GetContentType() ?? Config.JsonMediaType);
            
            return httpContent;
        }
    }
}
