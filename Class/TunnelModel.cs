using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpcUI.Class
{

    public class TunnelModel
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string Status { get; set; } // 在线或离线
        public string Region { get; set; }
        public string Address { get; set; }
        public string ConnectUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public long UpBytes { get; set; }
        public long DownBytes { get; set; }
        public int ConnectionCount { get; set; }
    }

    public class PeizhiwenjianModel
    {
        // 通用配置部分 (common)
        public string state { get; set; }
        public string server_addr { get; set; }
        public string server_port { get; set; }
        public string tls_enable { get; set; }
        public string user { get; set; }
        public string token { get; set; }

        // 隧道配置部分 (tunnel-specific, e.g., SMB-HanGuo)
        public string type { get; set; }
        public string local_ip { get; set; }
        public string local_port { get; set; }
        public string custom_domains { get; set; }
        public string use_encryption { get; set; }
        public string use_compression { get; set; }
        public string proxy_protocol_version { get; set; }

        // 对象字符串表示
        public override string ToString()
        {
            return
                $"[common]\n" +
                $"server_addr = {server_addr}\n" +
                $"server_port = {server_port}\n" +
                $"tls_enable = {tls_enable}\n" +
                $"user = {user}\n" +
                $"token = {token}\n\n" +
                $"[Tunnel Configuration]\n" +
                $"type = {type}\n" +
                $"local_ip = {local_ip}\n" +
                $"local_port = {local_port}\n" +
                $"custom_domains = {custom_domains}\n" +
                $"use_encryption = {use_encryption}\n" +
                $"use_compression = {use_compression}\n" +
                $"proxy_protocol_version = {proxy_protocol_version}";
        }
        // 从JSON字符串初始化对象
        public static PeizhiwenjianModel FromJson(string json)
        {
            var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
            var model = new PeizhiwenjianModel
            {
                state = jsonObject.state,
                server_addr = jsonObject.data.common.server_addr,
                server_port = jsonObject.data.common.server_port,
                tls_enable = jsonObject.data.common.tls_enable,
                user = jsonObject.data.common.user,
                token = jsonObject.data.common.token,
                type = jsonObject.data.tunnels[0].type,
                local_ip = jsonObject.data.tunnels[0].local_ip,
                local_port = jsonObject.data.tunnels[0].local_port,
                custom_domains = jsonObject.data.tunnels[0].custom_domains,
                use_encryption = jsonObject.data.tunnels[0].use_encryption,
                use_compression = jsonObject.data.tunnels[0].use_compression,
                proxy_protocol_version = jsonObject.data.tunnels[0].proxy_protocol_version
            };
            return model;
        }

        // 将对象转换为JSON字符串
        public string ToJson()
        {
            var jsonObject = new
            {
                state = this.state,
                data = new
                {
                    common = new
                    {
                        server_addr = this.server_addr,
                        server_port = this.server_port,
                        tls_enable = this.tls_enable,
                        user = this.user,
                        token = this.token
                    },
                    tunnels = new[]
                    {
                    new
                    {
                        type = this.type,
                        local_ip = this.local_ip,
                        local_port = this.local_port,
                        custom_domains = this.custom_domains,
                        use_encryption = this.use_encryption,
                        use_compression = this.use_compression,
                        proxy_protocol_version = this.proxy_protocol_version
                    }
                }
                }
            };
            return JsonConvert.SerializeObject(jsonObject);
        }
    }

    public class HuoqusuidaoModel
    {
        // 返回结果中的msg字段
        public string Msg { get; set; }

        // 返回结果中的code字段
        public int Code { get; set; }

        // 返回结果中的data字段
        public SuidaoDataModel Data { get; set; }

        // 返回结果中的state字段
        public string State { get; set; }
    }

    public class SuidaoDataModel
    {
        // 通用配置部分 (common)
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalIp { get; set; }
        public string Type { get; set; }
        public int NPort { get; set; }
        public string Dorp { get; set; } // 这里假设 "dorp" 是自定义域名的缩写
        public string Node { get; set; }
        public string State { get; set; }
        public int UserId { get; set; }
        public string Encryption { get; set; }
        public string Compression { get; set; }
        public string Ap { get; set; } // 这里假设 "ap" 是代理协议版本的缩写
        public DateTime Uptime { get; set; }
        public string ClientVersion { get; set; }
        public long TodayTrafficIn { get; set; }
        public long TodayTrafficOut { get; set; }
        public int CurConns { get; set; }
        public string NodeState { get; set; }
        public string Ip { get; set; }

        // 对象字符串表示
        public override string ToString()
        {
            return
                $"[common]\n" +
                $"id = {Id}\n" +
                $"name = {Name}\n" +
                $"local_ip = {LocalIp}\n" +
                $"type = {Type}\n" +
                $"nport = {NPort}\n" +
                $"custom_domains = {Dorp}\n" +
                $"node = {Node}\n" +
                $"state = {State}\n" +
                $"userid = {UserId}\n" +
                $"tls_enable = {Encryption}\n" +
                $"use_compression = {Compression}\n" +
                $"proxy_protocol_version = {Ap}\n" +
                $"uptime = {Uptime}\n" +
                $"client_version = {ClientVersion}\n" +
                $"today_traffic_in = {TodayTrafficIn}\n" +
                $"today_traffic_out = {TodayTrafficOut}\n" +
                $"cur_conns = {CurConns}\n" +
                $"node_state = {NodeState}\n" +
                $"server_addr = {Ip}\n";
        }
    }


}
