using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FrpcUI.Class
{
    public partial class Jiedianxiangqin
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class Data
    {
        /// <summary>
        /// 节点管理员端口，没啥用
        /// </summary>
        [JsonProperty("adminPort")]
        public long AdminPort { get; set; }

        /// <summary>
        /// apitoken，没啥用
        /// </summary>
        [JsonProperty("apitoken")]
        public string Apitoken { get; set; }

        /// <summary>
        /// 节点地区
        /// </summary>
        [JsonProperty("area")]
        public string Area { get; set; }

        /// <summary>
        /// 国内带宽，yes则为国内带宽，no则为国外带宽
        /// </summary>
        [JsonProperty("china")]
        public string China { get; set; }

        /// <summary>
        /// 节点经纬度
        /// </summary>
        [JsonProperty("coordinates")]
        public string Coordinates { get; set; }

        /// <summary>
        /// 节点是否有防御，这里的防御指＞5Gbps的DDOS防御
        /// </summary>
        [JsonProperty("fangyu")]
        public string Fangyu { get; set; }

        /// <summary>
        /// 节点id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 节点域名
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// ipv6，节点的ipv6地址，没有则为null
        /// </summary>
        [JsonProperty("ipv6")]
        public string Ipv6 { get; set; }

        /// <summary>
        /// 节点名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 权限组
        /// </summary>
        [JsonProperty("nodegroup")]
        public string Nodegroup { get; set; }

        /// <summary>
        /// 节点Token，没啥用
        /// </summary>
        [JsonProperty("nodetoken")]
        public string Nodetoken { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// frps连接端口，没啥用
        /// </summary>
        [JsonProperty("port")]
        public long Port { get; set; }

        /// <summary>
        /// 节点本地ipv4
        /// </summary>
        [JsonProperty("realIp")]
        public string RealIp { get; set; }

        /// <summary>
        /// 外网端口范围，允许的外网端口范围
        /// </summary>
        [JsonProperty("rport")]
        public string Rport { get; set; }

        /// <summary>
        /// 节点状态，online为在线，offline为离线
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// 过白，节点是否需要过白
        /// </summary>
        [JsonProperty("toowhite")]
        public bool Toowhite { get; set; }

        /// <summary>
        /// 是否允许UDP
        /// </summary>
        [JsonProperty("udp")]
        public string Udp { get; set; }

        /// <summary>
        /// 是否允许建站，这里的建站指的端口类型http和https，yes为允许，no为不允许
        /// </summary>
        [JsonProperty("web")]
        public string Web { get; set; }
    }


    public partial class Data
    {
        public ObservableCollection<Data> Datas { get; set; }
        public Data()
        {
            Datas = new ObservableCollection<Data>();
        }

    }

    public partial class Jiedianxiangqin
    {
        public static Jiedianxiangqin FromJson(string json) => JsonConvert.DeserializeObject<Jiedianxiangqin>(json, FrpcUI.Class.Converter.Settings);
    }

    public static class Serialize1
    {
        public static string ToJson(this Jiedianxiangqin self) => JsonConvert.SerializeObject(self, FrpcUI.Class.Converter.Settings);
    }

    internal static class Converter1
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
