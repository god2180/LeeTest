using EverydayBaseballVRGame.Hubs;
using EverydayBaseballVRGame.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;



namespace EverydayBaseballVRGame.Photon
{
    public enum RoomState
    {
        Wait,
        Play
    }

    public class Room
    {
        public string ConnectionId { get; set; }
        public string PrivateIp { get; set; }
        public ushort Port { get; set; }
        public RoomState State { get; set; }
        public string GameName { get; set; }
        public int Player { get; set; }
    }

    public class User
    {
        public string ConnectionId { get; set; }
        public int RoonNum { get; set; }
    }

    public interface IRoomServer
    {
        void CreateRoom(CS_CreateRoom cs, string connectionId);
        void GetRoomList(CS_RoomList cs, string connectionId);
        void EnterRoom(CS_EnterRoom cs, string connectionId);
        void ExitRoom(CS_ExitRoom cs, string connectionId);
        void StartGame(CS_StartGame cs, string connectionId);
        void GetAddress(string connectionId);
        void OnDisconnected(string connectionId);
    }

    public class RoomServer : IRoomServer
    {
        private List<Room> _host;
        private List<User> _user;
        private IHttpContextAccessor _accessor;

        public RoomServer(IHttpContextAccessor accessor)
        {
            _host = new List<Room>();
            _user = new List<User>();
            _accessor = accessor;
        }

        public void CreateRoom(CS_CreateRoom cs, string connectionId)
        {
            try
            {
                SC_CreateRoom sc = new SC_CreateRoom()
                {
                    Code = 1,
                };
                
                Room room = _host.Find(r => r.PrivateIp == cs.PrivateIp && r.Port == cs.Port);
                if (room != null)
                {
                    sc.Code = 101;
                    sc.Msg = "CreateRoom Fail. Ip & Port are already using";
                    HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(sc));
                }

                Room data = new Room()
                {
                    ConnectionId = connectionId,
                    PrivateIp = cs.PrivateIp,
                    Port = cs.Port,
                    State = RoomState.Wait,
                    GameName = cs.GameName,
                    Player = 1,
                };

                _host.Add(data);

                User user = new User()
                {
                    ConnectionId = connectionId,
                    RoonNum = _host.Count - 1,
                };

                _user.Add(user);

                
                HubConnector.Client(connectionId).ResponseCreateRoom(JsonConvert.SerializeObject(sc));
            }
            catch(Exception ex) {

                HubModel error = new HubModel()
                {
                    Code = 101,
                    Msg = "CreateRoom Exception Fail : " + ex.ToString(),
                };

                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }
        }

        public void GetRoomList(CS_RoomList cs, string connectionId)
        {
            try
            {
                SC_RoomList sc = new SC_RoomList()
                {
                    Code = 1,
                    RoomList = new List<SC_Room>(),
                };
                for (int i = 0; i < _host.Count; i++)
                {
                    if (_host[i].State == RoomState.Wait && _host[i].GameName == cs.GameName)                    
                    {
                        sc.RoomList.Add(new SC_Room()
                        {
                            PrivateIp = _host[i].PrivateIp,
                            Port = _host[i].Port,
                            Player = _host[i].Player,
                        });
                    }                    
                }
                HubConnector.Client(connectionId).GetRoomList(JsonConvert.SerializeObject(sc));
            }
            catch(Exception ex)
            {
                HubModel error = new HubModel()
                {
                    Code = 102,
                    Msg = "GetRoomList Exception Fail : " + ex.ToString(),
                };

                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }
        }

        public void EnterRoom(CS_EnterRoom cs, string connectionId)
        {
            try
            {
                SC_EnterRoom sc = new SC_EnterRoom()
                {
                    Code = 1,
                };

                var host = _host.Find(r => r.PrivateIp == cs.PrivateIp && r.Port == cs.Port && r.GameName == cs.GameName );

                if (host == null)
                {
                    sc.Code = 103;
                    sc.Msg = "EnterRoom Fail";
                    HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(sc));
                }
                
                host.Player++;

                HubConnector.Client(connectionId).EnterRoom(JsonConvert.SerializeObject(sc));
            }
            catch(Exception ex)
            {
                HubModel error = new HubModel()
                {
                    Code = 103,
                    Msg = "EnterRoom Exception Fail : " + ex.ToString(),
                };
                
                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }
        }

        public void ExitRoom(CS_ExitRoom cs, string connectionId)
        {
            try
            {
                var host = _host.Find(r => r.PrivateIp == cs.PrivateIp && r.Port == cs.Port && r.GameName == cs.GameName);

                if (host == null)
                {
                    HubModel sc = new HubModel()
                    {
                        Code = 104,
                        Msg = "ExitRoom Fail",
                    };

                    HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(sc));
                }

                host.Player--;
            }
            catch (Exception ex)
            {
                HubModel error = new HubModel()
                {
                    Code = 103,
                    Msg = "EnterRoom Exception Fail : " + ex.ToString(),
                };

                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }

        }

        public void StartGame(CS_StartGame cs, string connectionId)
        {
            try
            {
                SC_StartGame sc = new SC_StartGame()
                {
                    Code = 1
                };

                var host = _host.Find(r => r.GameName == cs.GameName && r.ConnectionId == connectionId);
                if (host == null)
                {
                    sc.Code = 105;
                    sc.Msg = "StartGame Fail";
                    HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(sc));
                }
                
                host.State = RoomState.Play;                

                HubConnector.Client(connectionId).StartGame(JsonConvert.SerializeObject(sc));
            }
            catch(Exception ex)
            {
                HubModel error = new HubModel()
                {
                    Code = 105,
                    Msg = "StartGame Exception Fail : " + ex.ToString(),
                };

                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }
        }

        public void OnDisconnected(string connectionId)
        {
            try
            {
                Room host = _host.Find(r => r.ConnectionId == connectionId);
                if(host != null)
                    _host.Remove(host);
            }

            catch(Exception ex)
            {
                HubModel error = new HubModel()
                {
                    Code = 2,
                    Msg = "Disconnect Exception Fail : " + ex.ToString(),
                };

                HubConnector.Client(connectionId).ErrorMsg(JsonConvert.SerializeObject(error));
            }
        }

        public void GetAddress(string connectionId)
        {

            try
            {
                string address = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

                HubConnector.Client(connectionId).GetAddress(JsonConvert.SerializeObject(address));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
