using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using ItirafEt.SharedComponents.Helpers;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace ItirafEt.Mobile.Services
{
    public class MobileSignalRService : SignalRServiceBase
    {

        public override HubConnection? ConfigureHubConnection(HubConstants.HubType hubType)
        {
            if (IsConnected(hubType))
                return GetConnection(hubType);

            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android ?
                ApiBaseUrl.AndroidBaseUrlHttp :
                ApiBaseUrl.BaseUrl;

            var hubUrl = $"{baseUrl}{HubConstants.GetHubUrl(hubType)}";

            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    if (DeviceInfo.Platform == DevicePlatform.Android)
                    {
                        options.HttpMessageHandlerFactory = handler =>
                            new AndroidMessageHandler { InnerHandler = handler };
                    }
                })
                .WithAutomaticReconnect()
                .Build();

            //_connections[hubType] = connection;
            return connection;
            //await connection.StartAsync();
        }
    }

    public class AndroidMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Android için gerekli özel işlemler
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
