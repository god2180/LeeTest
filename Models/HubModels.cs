using EverydayBaseballVRGame.Photon;
using System.Collections.Generic;

namespace EverydayBaseballVRGame.Models
{
    public class HubModel
    {
        public string Msg { get; set; }
        public int Code { get; set; }
    }

    public class Ip
    {
        public string PrivateIp {get; set;}
        public ushort Port { get; set; }
    }

    public class SC_Room : Ip
    {
        public int Player { get; set; }
    }

    public class CS_CreateRoom
    {
        public string PrivateIp { get; set; }
        public ushort Port { get; set; }
        public string GameName { get; set; }
    }

    public class SC_CreateRoom : HubModel
    {
    }

    public class CS_RoomList
    {
        public string GameName { get; set; }
    }

    public class SC_RoomList : HubModel
    {
        public List<SC_Room> RoomList { get; set; }
        //public string PrivateIP { get; set; }
        //public ushort Port { get; set; }
    }

    public class CS_EnterRoom
    {
        public string PrivateIp { get; set; }
        public ushort Port { get; set; }
        public string GameName { get; set; }
    }

    public class SC_EnterRoom : HubModel
    {

    }

    public class CS_ExitRoom
    {
        public string PrivateIp { get; set; }
        public ushort Port { get; set; }
        public string GameName { get; set; }
    }

    public class CS_StartGame
    {
        public string GameName { get; set; }
    }

    public class SC_StartGame : HubModel
    {
    }

}