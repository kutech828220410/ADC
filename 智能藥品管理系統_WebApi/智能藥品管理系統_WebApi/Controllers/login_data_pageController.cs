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
using System.ComponentModel;
using System.Reflection;
using System.Configuration;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace 智能藥品管理系統_WebApi
{


    [Route("api/[controller]")]
    [ApiController]

    public class login_data_pageController : ControllerBase
    {
        static private string DataBaseName = ConfigurationManager.AppSettings["person_page_database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["person_page_IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private MyConvert myConvert = new MyConvert();
        private SQLControl sQLControl_login_data_index = new SQLControl(IP, DataBaseName, "login_data_index", UserName, Password, Port, SSLMode);
        private SQLControl sQLControl_login_data = new SQLControl(IP, DataBaseName, "login_data", UserName, Password, Port, SSLMode);

     
        [Route("data")]
        [HttpGet()]
        public string Get_logindata()
        {
            string jsonString = MySQL_Login.LoginDataWebAPI.Get_login_data_index_JSONString(sQLControl_login_data_index);
            return jsonString;
        }
        [HttpGet]
        public string Get(string level)
        {
            List<MySQL_Login.LoginDataWebAPI.Class_login_data> list_class_login_data = MySQL_Login.LoginDataWebAPI.Get_login_data(sQLControl_login_data);
            if (list_class_login_data.Count > 0)
            {
                if(level.StringIsEmpty() == false)
                {
                    list_class_login_data = (from value in list_class_login_data
                                             where value.level.StringToInt32().ToString() == level.StringToInt32().ToString()
                                             select value).ToList();
                }
        
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<List<MySQL_Login.LoginDataWebAPI.Class_login_data>>(list_class_login_data, options);

                return jsonString;

            }
            else
            {
                return null;
            }
        }


        // PUT api/<login_data_pageController>/5
        [HttpPut]
        public string Put([FromBody] MySQL_Login.LoginDataWebAPI.Class_login_data data)
        {


            List<object[]> list_value = sQLControl_login_data.GetAllRows(sQLControl_login_data.TableName);
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<MySQL_Login.LoginDataWebAPI.Class_login_data>(data, options);
            }
            catch
            {
                return "-1";
            }
            if (data.level.StringToInt32() >= 0 && data.level.StringToInt32() <= 19)
            {
                MySQL_Login.LoginDataWebAPI.Set_login_data(data, sQLControl_login_data);
                return "200";
            }
            else
            {
                return "-10";
            }
        }


    }
}
