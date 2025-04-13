using System;
using System.Collections.ObjectModel;

namespace FrpcUI.Class
{
    public class LoginModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LocalIP { get; set; }
        public string Type { get; set; }
        public int NPort { get; set; }
        public string DORP { get; set; } // 这个字段名有些不明确，你可以根据实际需求调整
        public string Node { get; set; }
        public bool State { get; set; }
        public int UserID { get; set; }
        public bool UseEncryption { get; set; }
        public bool UseCompression { get; set; }
        public string AP { get; set; } // 这个字段名有些不明确，你可以根据实际需求调整
        //public DateTime Uptime { get; set; }
        public string ClientVersion { get; set; }
        public long TodayTrafficIn { get; set; }
        public long TodayTrafficOut { get; set; }
        public int CurConns { get; set; }
        public string NodeState { get; set; } // 在线或离线
        public string ConnectUrl { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string tunnelCount { get; set; }
        // 新增属性
        public string Msg { get; set; }
        public int Code { get; set; }
        public string RealName { get; set; }
        public string Token { get; set; }
        public bool QianDao { get; set; }
        public string UserGroup { get; set; }
        public int Integral { get; set; }
        public int AbroadBandwidth { get; set; }
        public int Bandwidth { get; set; }
        public long QQ { get; set; }
        public int Tunnel { get; set; }
        public int UsedTunnel { get; set; }
        public string Mail { get; set; }
        public string UserImg { get; set; }
        public string IdentityID { get; set; }
        public DateTime? DateOut { get; set; }
    }

    public class LogingModelViewModel
    {
        public ObservableCollection<LoginModel> LoginModels { get; set; }
        public LogingModelViewModel()
        {
            LoginModels = new ObservableCollection<LoginModel>();

        }


    }


}
