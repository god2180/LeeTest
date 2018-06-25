using EverydayBaseballVRGame.Models;
using Microsoft.AspNetCore.SignalR.Hubs;
using System.Collections.Generic;
using System.Linq;

namespace EverydayBaseballVRGame.Hubs
{
    public static class HubConnector
    {
        public static IHubConnectionContext<dynamic> Connector;

        public static dynamic Clients(IList<string> connectionIds) { return Connector.Clients(connectionIds); }
        public static dynamic Client(string connectionId) { return Connector.Client(connectionId); }

        //public static dynamic All<T> (List<T> users) where T : BaseUser
        //{
        //    var connectionIds = from user in users
        //                        select user.ConnectionId;

        //    return Clients(connectionIds.ToList());
        //}


        //public static dynamic AllExclude<T> (List<T> users, params string[] excludeConnectionIds) where T : BaseUser
        //{
        //    var connectionIds = from user in users
        //                        where !excludeConnectionIds.Contains(user.ConnectionId)
        //                        select user.ConnectionId;

        //    return Clients(connectionIds.ToList());                                
        //}
        
    }
}