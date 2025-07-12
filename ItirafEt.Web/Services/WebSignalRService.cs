using ItirafEt.Shared.Models;
using ItirafEt.SharedComponents.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ItirafEt.Web.Services
{
    public class WebSignalRService : SignalRServiceBase
    {
        
        //private readonly NavigationManager _navigationManager;

        //public WebSignalRService(NavigationManager navigationManager)
        //{
        //    _navigationManager = navigationManager;
        //}

        public override HubConnection? ConfigureHubConnection(HubConstants.HubType hubType)
        {
            if (IsConnected(hubType)) return GetConnection(hubType);

            var baseUrl = ApiBaseUrl.BaseUrl;

            var hubUrl = $"{baseUrl}{HubConstants.GetHubUrl(hubType)}";

            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            //_connections[hubType] = connection;
            //await connection.StartAsync();
            return connection;
        }
    }
}
