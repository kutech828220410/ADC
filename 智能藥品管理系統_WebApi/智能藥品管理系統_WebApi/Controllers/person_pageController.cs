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
    [Route("api/[controller]")]
    [ApiController]
    public class person_pageController : ControllerBase
    {
        static private string DataBaseName = ConfigurationManager.AppSettings["person_page_database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password = ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["person_page_IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;
        private SQLControl sQLControl_person_page = new SQLControl(IP, DataBaseName, "person_page", UserName, Password, Port, SSLMode);
        public class class_person_page_data
        {
            [JsonPropertyName("ID")]
            public string ID { get; set; }
            [JsonPropertyName("name")]
            public string 姓名 { get; set; }
            [JsonPropertyName("gender")]
            public string 性別 { get; set; }
            [JsonPropertyName("password")]
            public string 密碼 { get; set; }
            [JsonPropertyName("employer")]
            public string 單位 { get; set; }
            [JsonPropertyName("level")]
            public string 權限等級 { get; set; }
            [JsonPropertyName("color")]
            public string 顏色 { get; set; }
            [JsonPropertyName("UID")]
            public string 卡號 { get; set; }
        }
        public enum enum_person_page
        {
            GUID,
            ID,
            姓名,
            性別,
            密碼,
            單位,
            權限等級,
            顏色,
            卡號,
            一維條碼,
            識別圖案,
        }
        // GET: api/<person_pageController>
        [HttpGet]
        public string Get(string ID ,string? name, string? UID)
        {

            List<object[]> list_value = sQLControl_person_page.GetAllRows(null);

            if (!ID.StringIsEmpty())
            {
                list_value = list_value.Where(a => a[(int)enum_person_page.ID].ObjectToString() == ID).ToList();
            }
            if (name != null)
            {
                if (!name.StringIsEmpty())
                {
                    list_value = list_value.Where(a => a[(int)enum_person_page.姓名].ObjectToString() == name).ToList();
                }
            }
            if (UID != null)
            {
                if (!UID.StringIsEmpty())
                {
                    list_value = list_value.Where(a => a[(int)enum_person_page.卡號].ObjectToString() == UID).ToList();
                }
            }

            List<class_person_page_data> list_out_value = new List<class_person_page_data>();
            for (int i = 0; i < list_value.Count; i++)
            {
                class_person_page_data class_Person_Page_Data = new class_person_page_data();
                class_Person_Page_Data.ID = list_value[i][(int)enum_person_page.ID].ObjectToString();
                class_Person_Page_Data.姓名 = list_value[i][(int)enum_person_page.姓名].ObjectToString();
                class_Person_Page_Data.性別 = list_value[i][(int)enum_person_page.性別].ObjectToString();
                class_Person_Page_Data.密碼 = list_value[i][(int)enum_person_page.密碼].ObjectToString();
                class_Person_Page_Data.單位 = list_value[i][(int)enum_person_page.單位].ObjectToString();
                class_Person_Page_Data.權限等級 = list_value[i][(int)enum_person_page.權限等級].ObjectToString();
                class_Person_Page_Data.顏色 = list_value[i][(int)enum_person_page.顏色].ObjectToString();
                class_Person_Page_Data.卡號 = list_value[i][(int)enum_person_page.卡號].ObjectToString();

                list_out_value.Add(class_Person_Page_Data);
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            string jsonString = JsonSerializer.Serialize<List<class_person_page_data>>(list_out_value, options);

            return jsonString;
        }

        // POST api/<person_pageController>
        [HttpPost]
        public string Post([FromBody] class_person_page_data data)
        {
            List<object[]> list_value = sQLControl_person_page.GetAllRows(null);
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_person_page_data>(data, options);
            }
            catch
            {
                return "-1";
            }

            list_value = list_value.GetRows((int)enum_person_page.ID, data.ID);
            if (list_value.Count > 0) return "-2";
            object[] value = new object[new enum_person_page().GetLength()];
            value[(int)enum_person_page.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_person_page.ID] = data.ID;
            value[(int)enum_person_page.姓名] = data.姓名;
            value[(int)enum_person_page.性別] = data.性別;
            value[(int)enum_person_page.密碼] = data.密碼;
            value[(int)enum_person_page.單位] = data.單位;
            value[(int)enum_person_page.權限等級] = data.權限等級;
            value[(int)enum_person_page.顏色] = data.顏色;
            value[(int)enum_person_page.卡號] = data.卡號;
            sQLControl_person_page.AddRow(null, value);
            return "200";
        }

        // PUT api/<person_pageController>/5
        [HttpPut]
        public string Put([FromBody] class_person_page_data data)
        {

            List<object[]> list_value = sQLControl_person_page.GetAllRows(null);
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_person_page_data>(data, options);
            }
            catch
            {
                return "-1";
            }
            list_value = list_value.GetRows((int)enum_person_page.ID, data.ID);
            if (list_value.Count == 0)
            {
                return "-2";
            }
            else
            {
                object[] value = new object[new enum_person_page().GetLength()];
                value[(int)enum_person_page.GUID] = list_value[0][(int)enum_person_page.GUID].ObjectToString();
                value[(int)enum_person_page.ID] = data.ID;
                value[(int)enum_person_page.姓名] = data.姓名;
                value[(int)enum_person_page.性別] = data.性別;
                value[(int)enum_person_page.密碼] = data.密碼;
                value[(int)enum_person_page.單位] = data.單位;
                value[(int)enum_person_page.權限等級] = data.權限等級;
                value[(int)enum_person_page.顏色] = data.顏色;
                value[(int)enum_person_page.卡號] = data.卡號;
                value[(int)enum_person_page.一維條碼] = list_value[0][(int)enum_person_page.一維條碼].ObjectToString();
                value[(int)enum_person_page.識別圖案] = list_value[0][(int)enum_person_page.識別圖案].ObjectToString();
                sQLControl_person_page.UpdateByDefult(null, enum_person_page.ID.GetEnumName(), data.ID, value);

                return "200";
            }

        }

        // DELETE api/<person_pageController>/5
        [HttpDelete]
        public string Delete([FromBody] class_person_page_data data)
        {
            List<object[]> list_value = sQLControl_person_page.GetAllRows(null);
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_person_page_data>(data, options);
            }
            catch
            {
                return "-1";
            }
            if (!(sQLControl_person_page.GetRowsByDefult(null, enum_person_page.ID.GetEnumName(), data.ID).Count > 0))
            {
                return "-2";
            }
            else
            {
                sQLControl_person_page.DeleteByDefult(null, enum_person_page.ID.GetEnumName(), data.ID);
                return "200";
            }
        }


    }
}
