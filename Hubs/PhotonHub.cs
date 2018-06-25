using EverydayBaseballVRGame.Models;
using EverydayBaseballVRGame.Photon;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EverydayBaseballVRGame.Hubs
{
    public class PhotonHub : Hub
    {
        private readonly IRoomServer _host;

        
        public PhotonHub(IRoomServer host)
        {
            _host = host;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine(string.Format("OnDisconnected: {0}", Context.ConnectionId));
            
            _host.OnDisconnected(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            Debug.WriteLine(string.Format("onConnected: {0}", Context.ConnectionId));

            HubConnector.Connector = Clients;
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Debug.WriteLine(string.Format("OnReconnected: {0}", Context.ConnectionId));
            
            return base.OnReconnected();
        }

        public void RequestCreateRoom(object args)
        {
            CS_CreateRoom cs = JsonConvert.DeserializeObject<CS_CreateRoom>(args.ToString());
                        
            _host.CreateRoom(cs, Context.ConnectionId);
        }

        public void RequestRoomList(object args)
        {
            CS_RoomList cs = JsonConvert.DeserializeObject<CS_RoomList>(args.ToString());

            _host.GetRoomList(cs, Context.ConnectionId);
        }

        public void EnterRoom(object args)
        {
            CS_EnterRoom cs = JsonConvert.DeserializeObject<CS_EnterRoom>(args.ToString());

            _host.EnterRoom(cs, Context.ConnectionId);
        }

        public void ExitRoom(object args)
        {
            CS_ExitRoom cs = JsonConvert.DeserializeObject<CS_ExitRoom>(args.ToString());
            
            _host.ExitRoom(cs, Context.ConnectionId);
        }

        public void StartGame(object args)
        {
            CS_StartGame cs = JsonConvert.DeserializeObject<CS_StartGame>(args.ToString());
            _host.StartGame(cs, Context.ConnectionId);
            
        }

        public void GetAddress()
        {
            _host.GetAddress(Context.ConnectionId);
        }
    }
}

