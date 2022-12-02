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
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace 智能藥品管理系統_WebApi
{

    public class Class_手術房名稱
    {
        [JsonPropertyName("Code")]
        public string 序號 { get; set; }
        [JsonPropertyName("Name")]
        public string 名稱 { get; set; }

    }
    public enum enum_手術房名稱 : int
    {
        GUID,
        序號,
        名稱,
    }

    [Route("api/[controller]")]
    [ApiController]
    public class room_pageController : ControllerBase
    {
        static private string DataBaseName = ConfigurationManager.AppSettings["database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private SQLControl sQLControl_room_page = new SQLControl(IP, DataBaseName, "operating_room_page", UserName, Password, Port, SSLMode);
        [HttpGet]
        public string Get()
        {
            List<object[]> list_value = sQLControl_room_page.GetAllRows(null);
            List<object[]> list_group_buf = new List<object[]>();
            List<Class_手術房名稱> list_out_value = new List<Class_手術房名稱>();


            if (list_value.Count > 0)
            {
                for (int i = 0; i < list_value.Count; i++)
                {
                    Class_手術房名稱 class_手術房名稱 = new Class_手術房名稱();
                    class_手術房名稱.序號 = list_value[i][(int)enum_手術房名稱.序號].ObjectToString();
                    class_手術房名稱.名稱 = list_value[i][(int)enum_手術房名稱.名稱].ObjectToString();

                    list_out_value.Add(class_手術房名稱);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<List<Class_手術房名稱>>(list_out_value, options);


                return jsonString;
            }
            return null;
        }
    }
}
