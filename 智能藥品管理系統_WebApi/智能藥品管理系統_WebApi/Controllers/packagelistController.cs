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
    public enum enum_packagelist : int
    {
        GUID,
        套餐代碼,
        套餐名稱,
    }
    public enum enum_sub_packagelist : int
    {
        GUID,
        套餐代碼,
        藥品碼,
        藥品名稱,
        單位,
        數量,
    }

    public class class_packagelist_data
    {
        private List<dataClass> _code_all = new List<dataClass>();

        [JsonPropertyName("code")]
        public string 套餐代碼 { get; set; }
        [JsonPropertyName("name")]
        public string 套餐名稱 { get; set; }
        [JsonPropertyName("code_all")]
        public List<dataClass> code_all { get => _code_all; set => _code_all = value; }

        public class dataClass
        {

            public dataClass(string code, string name, string package, string inventory)
            {
                this.code = code;
                this.name = name;
                this.package = package;
                this.inventory = inventory;
            }

            private string _code;
            private string _name;
            private string _inventory;
            private string _package;

            [JsonPropertyName("code")]
            public string code { get => _code; set => _code = value; }
            [JsonPropertyName("name")]
            public string name { get => _name; set => _name = value; }
            [JsonPropertyName("inventory")]
            public string inventory { get => _inventory; set => _inventory = value; }
            [JsonPropertyName("package")]
            public string package { get => _package; set => _package = value; }
        }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class packagelistController : ControllerBase
    {
        static private string DataBaseName = ConfigurationManager.AppSettings["database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private SQLControl sQLControl_packagelist = new SQLControl(IP, DataBaseName, "pakagelist_page", UserName, Password, Port, SSLMode);
        private SQLControl sQLControl_sub_packagelist = new SQLControl(IP, DataBaseName, "sub_pakagelist_page", UserName, Password, Port, SSLMode);
      
        [HttpGet]
        public string Get()
        {
            List<object[]> list_packagelist = sQLControl_packagelist.GetAllRows(null);
            List<object[]> list_sub_packagelist = sQLControl_sub_packagelist.GetAllRows(null);
            List<object[]> list_sub_packagelist_buf = new List<object[]>();
            List<class_packagelist_data> list_out_value = new List<class_packagelist_data>();
            string jsonString = null;
            for(int i = 0; i < list_packagelist.Count; i++)
            {
                class_packagelist_data _class_packagelist_data = new class_packagelist_data();
                string 套餐代碼 = list_packagelist[i][(int)enum_packagelist.套餐代碼].ObjectToString();
                string 套餐名稱 = list_packagelist[i][(int)enum_packagelist.套餐名稱].ObjectToString();
                _class_packagelist_data.套餐代碼 = 套餐代碼;
                _class_packagelist_data.套餐名稱 = 套餐名稱;


                list_sub_packagelist_buf = list_sub_packagelist.GetRows((int)enum_sub_packagelist.套餐代碼, 套餐代碼);
                for (int k = 0; k < list_sub_packagelist_buf.Count; k++)
                {
                    string code = list_sub_packagelist_buf[k][(int)enum_sub_packagelist.藥品碼].ObjectToString();
                    string name = list_sub_packagelist_buf[k][(int)enum_sub_packagelist.藥品名稱].ObjectToString();
                    string package = list_sub_packagelist_buf[k][(int)enum_sub_packagelist.單位].ObjectToString();
                    string inventory = list_sub_packagelist_buf[k][(int)enum_sub_packagelist.數量].ObjectToString();

                    class_packagelist_data.dataClass dataClass = new class_packagelist_data.dataClass(code, name, package, inventory); ;
                    _class_packagelist_data.code_all.Add(dataClass);
                }
                list_out_value.Add(_class_packagelist_data);


            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            jsonString = JsonSerializer.Serialize<List<class_packagelist_data>>(list_out_value, options);
            return jsonString;
        }

    }
}
