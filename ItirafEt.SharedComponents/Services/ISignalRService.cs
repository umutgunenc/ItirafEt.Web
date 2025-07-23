using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItirafEt.Shared.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace ItirafEt.SharedComponents.Services
{
    public interface ISignalRService
    {
        HubConnection? ConfigureHubConnection(HubConstants.HubType hubType);
        //HubConnection? GetConnection(HubConstants.HubType hubType);
        //bool IsConnected(HubConstants.HubType hubType);
    }
}
