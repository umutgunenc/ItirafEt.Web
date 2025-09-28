using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using ItirafEt.SharedComponents.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using static ItirafEt.Shared.Models.HubConstants;
using static ItirafEt.SharedComponents.Helpers.PageNameConstants;

namespace ItirafEt.SharedComponents.Services
{
    public interface ISignalRService
    {
        Task<HubConnection?> ConfigureHubConnectionAsync(HubConstants.HubType hubType ,PageNameConstants.PageType pageType);
        //Task<HubConnection?> GetConnectionAsync(PageType pageType, HubType hubType);
        //bool IsConnected(HubConnection? connection);
        Task DisposeAsync(PageType pageType, HubType hubType);
        Task DisposeAllAsync();
        Task StopAsync(PageType pageType, HubType hubType);
    }
}