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
    public class login_sessionController : Controller
    {
        static private string DataBaseName = ConfigurationManager.AppSettings["person_page_database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private SQLControl sQLControl_login_session = new SQLControl(IP, DataBaseName, "login_session", UserName, Password, Port, SSLMode);


        public int enum_login_session_Length = Enum.GetValues(typeof(enum_login_session)).Length;
        public enum enum_login_session
        {
            login_session,
        }
        public class Class_login_session
        {
            private string Login_session = "";
            public string login_session { get => Login_session; set => Login_session = value; }
        }
        [HttpGet]
        public string Get(string login_session)
        {
          
            List<object[]> list_value = sQLControl_login_session.GetAllRows(null);
            List<Class_login_session> list_out_value = new List<Class_login_session>();
            if (login_session .StringIsEmpty() == false)
            {
                list_value = list_value.GetRows((int)enum_login_session.login_session, login_session);
            }
            for (int i = 0; i < list_value.Count; i++)
            {
                Class_login_session class_Login_Session = new Class_login_session();
                class_Login_Session.login_session = list_value[i][(int)enum_login_session.login_session].ObjectToString();
                list_out_value.Add(class_Login_Session);
            }

            if (list_value.Count > 0)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<List<Class_login_session>>(list_out_value, options);


                return jsonString;
            }
            else
            {
                return "-2";
            }
        }
        [HttpPost]
        public string Post([FromBody] Class_login_session data)
        {

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<Class_login_session>(data, options);
            }
            catch
            {
                return "-1";
            }

            if (sQLControl_login_session.GetRowsByDefult(null, enum_login_session.login_session.GetEnumName(), data.login_session).Count > 0)
            {
                return "-2";
            }
            else
            {
                object[] value = new object[this.enum_login_session_Length];
                value[(int)enum_login_session.login_session] = data.login_session;
                sQLControl_login_session.AddRow(null, value);
                return "200";
            }

        }
        [HttpDelete]
        public string Delete([FromBody] Class_login_session data)
        {

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<Class_login_session>(data, options);
            }
            catch
            {
                return "-1";
            }
            if (!(sQLControl_login_session.GetRowsByDefult(null, enum_login_session.login_session.GetEnumName(), data.login_session).Count > 0))
            {
                return "-2";
            }
            else
            {
                sQLControl_login_session.DeleteByDefult(null, enum_login_session.login_session.GetEnumName(), data.login_session);
                return "200";
            }
        }
    }
}
