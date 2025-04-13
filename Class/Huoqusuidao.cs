using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FrpcUI.Class
{
    public class HuoqusuidaoModel
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
        public DateTime Uptime { get; set; }
        public string ClientVersion { get; set; }
        public long TodayTrafficIn { get; set; }
        public long TodayTrafficOut { get; set; }
        public int CurConns { get; set; }
        public string NodeState { get; set; } // 在线或离线
        public string ConnectUrl { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
    }

    public class HuoqusuidaoViewModel
    {
        public ObservableCollection<HuoqusuidaoModel> HuoqusuidaoModels { get; set; }

        public HuoqusuidaoViewModel()
        {
            // 模拟后台数据
            HuoqusuidaoModels = new ObservableCollection<HuoqusuidaoModel>
            {
                new HuoqusuidaoModel
                {
                    ID = 47299,
                    Name = "SMB-HuNan",
                    LocalIP = "127.0.0.1",
                    Type = "https",
                    NPort = 443,
                    DORP = "smb.fangaofeng.icu",
                    Node = "湖南娄底",
                    State = true,
                    UserID = 13918,
                    UseEncryption = true,
                    UseCompression = true,
                    AP = "proxy_protocol_version = v2",
                    Uptime = DateTime.Parse("2025-03-25T07:21:02.000+00:00"),
                    ClientVersion = null,
                    TodayTrafficIn = 0,
                    TodayTrafficOut = 0,
                    CurConns = 0,
                    NodeState = "online",
                    ConnectUrl = "smb.fangaofeng.icu",
                    Region = "湖南娄底",
                    Address = "127.0.0.1:443 - https"
                },
                // 如果你需要更多的模拟数据，可以在此继续添加
            };
        }
    }
}
