using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;

namespace 智能藥品管理系統_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tradingController : ControllerBase
    {

        static private string DataBaseName = ConfigurationManager.AppSettings["database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private MyConvert myConvert = new MyConvert();
        private SQLControl sQLControl_trading = new SQLControl(IP, DataBaseName, "trading", UserName, Password, Port, SSLMode);

        public class class_trading_data
        {
            [JsonPropertyName("Action")]
            public string 動作 { get; set; }
            [JsonPropertyName("code")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("name")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("SN")]
            public string 藥袋序號 { get; set; }
            [JsonPropertyName("inventory")]
            public string 庫存量 { get; set; }
            [JsonPropertyName("room")]
            public string 房名 { get; set; }
            [JsonPropertyName("value")]
            public string 交易量 { get; set; }
            [JsonPropertyName("balance")]
            public string 結存量 { get; set; }
            [JsonPropertyName("operator")]
            public string 操作人 { get; set; }
            [JsonPropertyName("patient_name")]
            public string 病人姓名 { get; set; }
            [JsonPropertyName("patient_code")]
            public string 病歷號 { get; set; }
            [JsonPropertyName("operating_time")]
            public string 操作時間 { get; set; }
            [JsonPropertyName("prescription_time")]
            public string 開方時間 { get; set; }
            [JsonPropertyName("remark")]
            public string 備註 { get; set; }
        }
        public int enum_trading_Data_Length = Enum.GetValues(typeof(enum_trading_page)).Length;
        public enum enum_trading_page
        {
            GUID,
            動作,
            藥品碼,
            藥品名稱,
            藥袋序號,
            房名,
            庫存量,
            交易量,
            結存量,
            操作人,
            病人姓名,
            病歷號,
            操作時間,
            開方時間,
            備註,
        }
        public int enum_action_Data_Length = Enum.GetValues(typeof(enum_action_page)).Length;
        public enum enum_action_page
        {
            掃碼領藥,
            手輸領藥,
            批次領藥,
            掃碼退藥,
            手輸退藥,
            重複領藥,
            人臉識別登入,
            RFID登入,
            密碼登入,
            登出,
            操作工程模式,
            效期庫存異動,
            入庫,
            實瓶繳回,
            空瓶繳回,
            退藥回收,
            None,
        }

        [Route("action")]
        [HttpGet()]
        public string Get_aciotn()
        {
            string jsonString = "";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            string[] enums = enum_action_page.None.GetEnumNames();
            List<string> list_str = new List<string>();
            for (int i = 0; i < enums.Length; i++)
            {
                list_str.Add(enums[i]);
            }
            jsonString = JsonSerializer.Serialize<List<string>>(list_str, options);
            return jsonString;
        }

        [HttpGet()]
        public string Get(string action, string? code, string? name, string? SN, string? Operator, string? patient_name, string? patient_code, DateTime? operating_time_start, DateTime? operating_time_end, DateTime? prescription_time_start, DateTime? prescription_time_end)
        {


            List<object[]> list_value = sQLControl_trading.GetAllRows(null);
            List<object[]> list_value_buf = new List<object[]>();
            List<class_trading_data> list_out_value = new List<class_trading_data>();
            
            List<List<object[]>> list_list_value_buf = new List<List<object[]>>();
            if (action != null)
            {
                string[] array_action = myConvert.分解分隔號字串(action, ',', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array_action.Length; i++)
                {
                    list_list_value_buf.Add(list_value.GetRows((int)enum_trading_page.動作, array_action[i]));              
                }
            }

            for (int i = 0; i<  list_list_value_buf.Count; i++)
            {
                for (int k = 0; k < list_list_value_buf[i].Count; k++)
                {
                    list_value_buf.Add(list_list_value_buf[i][k]);
                }
            }
            if (list_list_value_buf.Count == 0)
            {
                list_value_buf = list_value;
            }

            if (operating_time_start != null && operating_time_end != null)
            {
                DateTime dateTime_start = new DateTime(operating_time_start.Value.Year, operating_time_start.Value.Month, operating_time_start.Value.Day, 0, 0, 0);
                DateTime dateTime_end = new DateTime(operating_time_end.Value.Year, operating_time_end.Value.Month, operating_time_end.Value.Day, 23, 59, 59);

                dateTime_start = operating_time_start.Value;
                dateTime_end = operating_time_end.Value;

                list_value_buf = list_value_buf.Where(a => (TypeConvert.IsInDate(a[(int)enum_trading_page.操作時間].StringToDateTime(), dateTime_start, dateTime_end))).ToList();
            }
            if (prescription_time_start != null && prescription_time_end != null)
            {
                //DateTime dateTime_start = new DateTime(prescription_time_start.Value.Year, prescription_time_start.Value.Month, prescription_time_start.Value.Day, 0, 0, 0);
                //DateTime dateTime_end = new DateTime(prescription_time_end.Value.Year, prescription_time_end.Value.Month, prescription_time_end.Value.Day, 23, 59, 59);
                //list_value_buf = list_value_buf.Where(a => (TypeConvert.IsInDate(a[(int)enum_trading_page.開方時間].StringToDateTime(), dateTime_start, dateTime_end))).ToList();
            }
            if (code != null)
            {
                list_value_buf = list_value_buf.Where(a => a[(int)enum_trading_page.藥品碼].ObjectToString() == code).ToList();
            }
        
            if (Operator != null)
            {
                list_value_buf = list_value_buf.Where(a => a[(int)enum_trading_page.操作人].ObjectToString() == Operator).ToList();
            }
           
      
            if (name != null)
            {
                list_value_buf = list_value_buf.Where(a => a[(int)enum_trading_page.藥品名稱].ObjectToString().Contains(name)).ToList();
            }

            for (int i = 0; i < list_value_buf.Count; i++)
            {
                class_trading_data class_Trading_Data = new class_trading_data();
                class_Trading_Data.動作 = list_value_buf[i][(int)enum_trading_page.動作].ObjectToString();
                class_Trading_Data.藥品碼 = list_value_buf[i][(int)enum_trading_page.藥品碼].ObjectToString();
                class_Trading_Data.藥品名稱 = list_value_buf[i][(int)enum_trading_page.藥品名稱].ObjectToString();
                class_Trading_Data.房名 = list_value_buf[i][(int)enum_trading_page.房名].ObjectToString();
                class_Trading_Data.病人姓名 = list_value_buf[i][(int)enum_trading_page.病人姓名].ObjectToString();
                class_Trading_Data.病歷號 = list_value_buf[i][(int)enum_trading_page.病歷號].ObjectToString();
                class_Trading_Data.開方時間 = list_value_buf[i][(int)enum_trading_page.開方時間].ObjectToString();
                class_Trading_Data.藥袋序號 = list_value_buf[i][(int)enum_trading_page.藥袋序號].ObjectToString();
                class_Trading_Data.庫存量 = list_value_buf[i][(int)enum_trading_page.庫存量].ObjectToString();
                class_Trading_Data.交易量 = list_value_buf[i][(int)enum_trading_page.交易量].ObjectToString();
                class_Trading_Data.結存量 = list_value_buf[i][(int)enum_trading_page.結存量].ObjectToString();
                class_Trading_Data.操作人 = list_value_buf[i][(int)enum_trading_page.操作人].ObjectToString();
     
                class_Trading_Data.操作時間 = list_value_buf[i][(int)enum_trading_page.操作時間].ToDateTimeString();
                class_Trading_Data.備註 = list_value_buf[i][(int)enum_trading_page.備註].ObjectToString();
                list_out_value.Add(class_Trading_Data);

            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
         
            string jsonString = JsonSerializer.Serialize<List<class_trading_data>>(list_out_value, options);


            return jsonString;
        }
    }
}
