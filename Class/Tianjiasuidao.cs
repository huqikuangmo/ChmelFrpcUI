using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FrpcUI.Class
{
    public partial class TianjiasuidaoModel
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class Datum
    {
        /// <summary>
        /// 节点地区
        /// </summary>
        [JsonProperty("area")]
        public string Area { get; set; }

        /// <summary>
        /// 带宽限速是否为中国，只会返回yes或no，yes为使用国内限速，no为使用国外限速
        /// </summary>
        [JsonProperty("china")]
        public string China { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 节点名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 节点权限组，只会返回user或vip，user为免费节点，vip为会员节点
        /// </summary>
        [JsonProperty("nodegroup")]
        public string Nodegroup { get; set; }

        /// <summary>
        /// 是否允许UDP协议，只会返回true或false，true为允许，false为不允许
        /// </summary>
        [JsonProperty("udp")]
        public string Udp { get; set; }

        /// <summary>
        /// 是否允许建站，只会返回yes或no，yes为允许，no为不允许
        /// </summary>
        [JsonProperty("web")]
        public string Web { get; set; }
    }
    public partial class Datum
    {
        public ObservableCollection<Datum> Datums { get; set; }
        public Datum()
        {
            Datums = new ObservableCollection<Datum>();
        }

    }

    public partial class TianjiasuidaoModel
    {
        public static TianjiasuidaoModel FromJson(string json) => JsonConvert.DeserializeObject<TianjiasuidaoModel>(json, FrpcUI.Class.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this TianjiasuidaoModel self) => JsonConvert.SerializeObject(self, FrpcUI.Class.Converter.Settings);
    }

    internal static class Converter
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
