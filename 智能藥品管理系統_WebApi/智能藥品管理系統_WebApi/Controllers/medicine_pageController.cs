using Basic;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SQLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using H_Pannel_lib;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace 智能藥品管理系統_WebApi
{
    public class class_medicine_page_firstclass_data
    {
        [JsonPropertyName("code")]
        public string 藥品碼 { get; set; }
        [JsonPropertyName("chinese_name")]
        public string 藥品中文名稱 { get; set; }
        [JsonPropertyName("name")]
        public string 藥品名稱 { get; set; }
        [JsonPropertyName("barcode")]
        public string 藥品條碼 { get; set; }
        [JsonPropertyName("package")]
        public string 包裝單位 { get; set; }
        [JsonPropertyName("inventory")]
        public string 庫存 { get; set; }
        [JsonPropertyName("safe_inventory")]
        public string 安全庫存 { get; set; }

    }
    public enum enum_medicine_page_firstclass
    {
        GUID,
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        藥品條碼1,
        包裝單位,
        庫存,
        安全庫存,
    }
    [Route("api/[controller]")]
    [ApiController]
    public class medicine_pageController : ControllerBase
    {

        static private string DataBaseName = ConfigurationManager.AppSettings["database"];
        static private string UserName = ConfigurationManager.AppSettings["user"];
        static private string Password =  ConfigurationManager.AppSettings["password"];
        static private string IP = ConfigurationManager.AppSettings["IP"];
        static private uint Port = (uint)ConfigurationManager.AppSettings["port"].StringToInt32();
        static private MySqlSslMode SSLMode = MySqlSslMode.None;

        private SQLControl sQLControl_WT32_serialize = new SQLControl(IP, DataBaseName, "wt32_storage_jsonstring", UserName, Password, Port, SSLMode);
        private SQLControl sQLControl_medicine_page = new SQLControl(IP, DataBaseName, "medicine_page", UserName, Password, Port, SSLMode);
        private SQLControl sQLControl_套餐列表 = new SQLControl(IP, DataBaseName, "pakagelist_page", UserName, Password, Port, SSLMode);
        private SQLControl sQLControl_套餐內容 = new SQLControl(IP, DataBaseName, "sub_pakagelist_page", UserName, Password, Port, SSLMode);


        #region storage_type
        [Route("storage_type")]
        [HttpGet()]
        public string Get_storage_type()
        {
            string jsonString = "";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            string[] enums = new DeviceType().GetEnumNames();
            List<string> list_str = new List<string>();
            for (int i = 0; i < enums.Length; i++)
            {
                list_str.Add(enums[i]);
            }
            jsonString = JsonSerializer.Serialize<List<string>>(list_str, options);
            return jsonString;
        }
        #endregion
        #region storage_list
        private class returnData
        {
            private string result = "";
            private int code = 0;
            private List<class_儲位總庫存表> data = new List<class_儲位總庫存表>();

            public string Result { get => result; set => result = value; }
            public int Code { get => code; set => code = value; }
            public List<class_儲位總庫存表> Data { get => data; set => data = value; }
        }


        public class class_儲位總庫存表
        {
            [JsonPropertyName("IP")]
            public string IP { get; set; }
            [JsonPropertyName("storage_name")]
            public string 儲位名稱 { get; set; }
            [JsonPropertyName("Code")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("Neme")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("min_package")]
            public string 最小包裝量 { get; set; }         
            [JsonPropertyName("package")]
            public string 單位 { get; set; }
            [JsonPropertyName("inventory")]
            public string 庫存 { get; set; }
            [JsonPropertyName("storage_type")]
            public string 儲位型式 { get; set; }
            [JsonPropertyName("max_inventory")]
            public string 可放置盒數 { get; set; }
            [JsonPropertyName("position")]
            public string 位置 { get; set; }

        }
        public enum enum_套餐列表
        {
            GUID,
            套餐代碼,
            套餐名稱,
        }
        public enum enum_套餐內容
        {
            GUID,
            套餐代碼,
            藥品碼,
            藥品名稱,
            單位,
            數量
        }

        public enum enum_儲位總庫存表 : int
        {
            儲位名稱,
            藥品碼,
            藥品名稱,
            單位,
            庫存,
            儲位型式,
            可放置盒數,
            IP,
        }
        public class ICP_儲位管理_儲位資料 : IComparer<class_儲位總庫存表>
        {
            public int Compare(class_儲位總庫存表 x, class_儲位總庫存表 y)
            {
                string IP_0 = x.IP;
                string IP_1 = y.IP;
       
                string[] IP_0_Array = IP_0.Split('.');
                string[] IP_1_Array = IP_1.Split('.');
                IP_0 = "";
                IP_1 = "";
                for (int i = 0; i < 4; i++)
                {
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];

                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];

                    IP_0 += IP_0_Array[i];
                    IP_1 += IP_1_Array[i];
                }
                int cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
        [Route("storage_list")]
        [HttpGet()]
        public string Get_storage_list(string? code, string? name)
        {
            List<object[]> list_藥檔資料 = sQLControl_medicine_page.GetAllRows(null);
            List<object[]> list_藥檔資料_buf = new List<object[]>();
            List<object[]> list_套餐列表 = this.sQLControl_套餐列表.GetAllRows(null);
            List<object[]> list_套餐內容 = this.sQLControl_套餐內容.GetAllRows(null);
            List<object[]> list_套餐列表_buf = new List<object[]>();
            string jsonString = "";
            this.Function_讀取儲位();
            returnData returnData = new returnData();
            returnData.Code = 200;
            returnData.Result = "取得儲位資料成功!";
            for (int i = 0; i < devices.Count; i++)
            {
                class_儲位總庫存表 Class_儲位總庫存表 = new class_儲位總庫存表();
                Class_儲位總庫存表.儲位名稱 = devices[i].StorageName;
                Class_儲位總庫存表.藥品碼 = devices[i].Code;
                Class_儲位總庫存表.藥品名稱 = devices[i].Name;
                Class_儲位總庫存表.單位 = devices[i].Package;
                Class_儲位總庫存表.庫存 = devices[i].Inventory;
                Class_儲位總庫存表.儲位型式 = devices[i].DeviceType.GetEnumName();
                Class_儲位總庫存表.IP = devices[i].IP;
                Class_儲位總庫存表.可放置盒數 = devices[i].Max_Inventory.ToString();
                Class_儲位總庫存表.最小包裝量 = devices[i].Min_Package_Num.ToString();

                list_套餐列表_buf = list_套餐列表.GetRows((int)enum_套餐列表.套餐代碼, Class_儲位總庫存表.藥品碼);
                if (list_套餐列表_buf.Count > 0)
                {
                    Class_儲位總庫存表.藥品名稱 = list_套餐列表_buf[0][(int)enum_套餐列表.套餐名稱].ObjectToString();
                    Class_儲位總庫存表.單位 = "Package";
                }
                else
                {
                    list_藥檔資料_buf = list_藥檔資料.GetRows((int)enum_medicine_page_firstclass.藥品碼, Class_儲位總庫存表.藥品碼);
                    if (list_藥檔資料_buf.Count > 0)
                    {
                        Class_儲位總庫存表.藥品名稱 = list_藥檔資料_buf[0][(int)enum_medicine_page_firstclass.藥品名稱].ObjectToString();
                        Class_儲位總庫存表.單位 = list_藥檔資料_buf[0][(int)enum_medicine_page_firstclass.包裝單位].ObjectToString();
                    }
                }
                returnData.Data.Add(Class_儲位總庫存表);

            }
            returnData.Data.Sort(new ICP_儲位管理_儲位資料());
            //$"{(i / 6) + 1}-{(i % 6) + 1}"
            for (int i = 0; i < returnData.Data.Count; i++)
            {
                returnData.Data[i].位置 = $"{(i / 6) + 1}-{(i % 6) + 1}";
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            jsonString = JsonSerializer.Serialize<returnData>(returnData, options);

            return jsonString;
        }
        #endregion
        #region storage_validity_date_list
        public class class_儲位效期表
        {
            [JsonPropertyName("storage_name")]
            public string 儲位名稱 { get; set; }
            [JsonPropertyName("Code")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("Neme")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("package")]
            public string 單位 { get; set; }
            [JsonPropertyName("validity_date")]
            public string 效期 { get; set; }
            [JsonPropertyName("inventory")]
            public string 庫存 { get; set; }
            [JsonPropertyName("storage_type")]
            public string 儲位型式 { get; set; }

        }


        public enum enum_儲位效期表 : int
        {
            儲位名稱,
            藥品碼,
            藥品名稱,
            單位,
            效期,
            庫存,
            儲位型式,
            IP,
        }
       
        [Route("storage_validity_date_list")]
        [HttpGet()]
        public string Get_storage_validity_date_list(string? code, string? name)
        {
 
            string jsonString = "";
            this.Function_讀取儲位();
            List<object[]> list_藥品資料_儲位效期表_value = new List<object[]>();
            List<object[]> list_藥品資料 = this.sQLControl_medicine_page.GetAllRows(null);
            List<object[]> list_藥品資料_buf = new List<object[]>();

            for (int i = 0; i < devices.Count; i++)
            {
                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_medicine_page_firstclass.藥品碼, devices[i].Code);
                if (list_藥品資料_buf.Count == 0) continue;

                for (int k = 0; k < devices[i].List_Validity_period.Count; k++)
                {
                    object[] value = new object[new enum_儲位效期表().GetLength()];

                    value[(int)enum_儲位效期表.儲位名稱] = devices[i].StorageName;
                    value[(int)enum_儲位效期表.藥品碼] = devices[i].Code;
                    value[(int)enum_儲位效期表.庫存] = devices[i].Inventory;
                    value[(int)enum_儲位效期表.儲位型式] = devices[i].DeviceType.GetEnumName();
                    value[(int)enum_儲位效期表.IP] = devices[i].IP;
                    value[(int)enum_儲位效期表.庫存] = devices[i].List_Inventory[k].ToString();
                    value[(int)enum_儲位效期表.效期] = devices[i].List_Validity_period[k];
                    value[(int)enum_儲位效期表.藥品名稱] = list_藥品資料_buf[0][(int)enum_medicine_page_firstclass.藥品名稱].ObjectToString();
                    value[(int)enum_儲位效期表.單位] = list_藥品資料_buf[0][(int)enum_medicine_page_firstclass.包裝單位].ObjectToString();

           
                    list_藥品資料_儲位效期表_value.Add(value);
                }
            }




            if (code != null)
            {
                list_藥品資料_儲位效期表_value = list_藥品資料_儲位效期表_value.GetRows((int)enum_儲位效期表.藥品碼, code);
            }
            if (name != null)
            {
                list_藥品資料_儲位效期表_value = list_藥品資料_儲位效期表_value.GetRowsByLike((int)enum_儲位效期表.藥品名稱, name);
            }

            list_藥品資料_儲位效期表_value = list_藥品資料_儲位效期表_value.OrderBy(r => r[(int)enum_儲位效期表.儲位名稱].ObjectToString()).ToList();
            List<class_儲位效期表> list_out_value = new List<class_儲位效期表>();

            for (int i = 0; i < list_藥品資料_儲位效期表_value.Count; i++)
            {
                class_儲位效期表 Class_儲位效期表 = new class_儲位效期表();
                Class_儲位效期表.儲位名稱 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.儲位名稱].ObjectToString();
                Class_儲位效期表.藥品碼 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.藥品碼].ObjectToString();
                Class_儲位效期表.藥品名稱 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.藥品名稱].ObjectToString();
                Class_儲位效期表.單位 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.單位].ObjectToString();
                Class_儲位效期表.庫存 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.庫存].ObjectToString();
                Class_儲位效期表.效期 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.效期].ObjectToString();
                Class_儲位效期表.儲位型式 = list_藥品資料_儲位效期表_value[i][(int)enum_儲位效期表.儲位型式].ObjectToString();

                list_out_value.Add(Class_儲位效期表);

            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };
            jsonString = JsonSerializer.Serialize<List<class_儲位效期表>>(list_out_value, options);

            return jsonString;
        }
        #endregion
   

        // GET: api/<medicine_page_firstclassController>
        [HttpGet]
        public string Get(string code, string? package, string? type)
        {
            List<object[]> list_value = sQLControl_medicine_page.GetAllRows(null);
            List<object[]> list_group_buf = new List<object[]>();
            List<class_medicine_page_firstclass_data> list_out_value = new List<class_medicine_page_firstclass_data>();
 

            if (list_value.Count > 0)
            {             
                this.Function_讀取儲位();

                if (!code.StringIsEmpty())
                {
                    list_value = list_value.GetRows((int)enum_medicine_page_firstclass.藥品碼, code);
                }
                if (!package.StringIsEmpty())
                {
                    list_value = list_value.GetRows((int)enum_medicine_page_firstclass.包裝單位, package);
                }
           
          
                for (int i = 0; i < list_value.Count; i++)
                {
                    class_medicine_page_firstclass_data class_medicine_page_firstclass_Data = new class_medicine_page_firstclass_data();
                    class_medicine_page_firstclass_Data.藥品碼 = list_value[i][(int)enum_medicine_page_firstclass.藥品碼].ObjectToString();
                    class_medicine_page_firstclass_Data.藥品中文名稱 = list_value[i][(int)enum_medicine_page_firstclass.藥品中文名稱].ObjectToString();
                    class_medicine_page_firstclass_Data.藥品名稱 = list_value[i][(int)enum_medicine_page_firstclass.藥品名稱].ObjectToString();
                    class_medicine_page_firstclass_Data.藥品條碼 = list_value[i][(int)enum_medicine_page_firstclass.藥品條碼1].ObjectToString();
                    class_medicine_page_firstclass_Data.包裝單位 = list_value[i][(int)enum_medicine_page_firstclass.包裝單位].ObjectToString();
                    class_medicine_page_firstclass_Data.庫存 = list_value[i][(int)enum_medicine_page_firstclass.庫存].ObjectToString();
                    class_medicine_page_firstclass_Data.安全庫存 = list_value[i][(int)enum_medicine_page_firstclass.安全庫存].ObjectToString();

                    
                    class_medicine_page_firstclass_Data.庫存 = this.Function_取得儲位庫存(class_medicine_page_firstclass_Data.藥品碼).ToString();
                    list_out_value.Add(class_medicine_page_firstclass_Data);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<List<class_medicine_page_firstclass_data>>(list_out_value, options);


                return jsonString;
            }
            return null;
        }
        // POST api/<medicine_page_firstclassController>
        [HttpPost]
        public string Post([FromBody] class_medicine_page_firstclass_data data)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_medicine_page_firstclass_data>(data, options);
            }
            catch
            {
                return "-1";
            }

            if (sQLControl_medicine_page.GetRowsByDefult(null, enum_medicine_page_firstclass.藥品碼.GetEnumName(), data.藥品碼).Count > 0)
            {
                return "-2";
            }
            else
            {
             

                object[] value = new object[new enum_medicine_page_firstclass().GetLength()];
                value[(int)enum_medicine_page_firstclass.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_medicine_page_firstclass.藥品碼] = data.藥品碼;
                value[(int)enum_medicine_page_firstclass.藥品中文名稱] = data.藥品中文名稱;
                value[(int)enum_medicine_page_firstclass.藥品名稱] = data.藥品名稱;
                value[(int)enum_medicine_page_firstclass.包裝單位] = data.包裝單位;
                value[(int)enum_medicine_page_firstclass.藥品條碼1] = data.藥品條碼;
                value[(int)enum_medicine_page_firstclass.庫存] = data.庫存;
                value[(int)enum_medicine_page_firstclass.安全庫存] = data.安全庫存;

                sQLControl_medicine_page.AddRow(null, value);
                return "200";
            }

        }
        // PUT api/<medicine_page_firstclassController>/5
        [HttpPut]
        public string Put([FromBody] class_medicine_page_firstclass_data data)
        {

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_medicine_page_firstclass_data>(data, options);
            }
            catch
            {
                return "-1";
            }

            List<object[]> list_value = sQLControl_medicine_page.GetRowsByDefult(null, enum_medicine_page_firstclass.藥品碼.GetEnumName(), data.藥品碼);
            if (!(list_value.Count > 0))
            {
                return "-3";
            }
            else
            {
                object[] value = list_value[0];

         
                value[(int)enum_medicine_page_firstclass.藥品碼] = data.藥品碼;
                value[(int)enum_medicine_page_firstclass.藥品中文名稱] = data.藥品中文名稱;
                value[(int)enum_medicine_page_firstclass.藥品名稱] = data.藥品名稱;
                value[(int)enum_medicine_page_firstclass.包裝單位] = data.包裝單位;
                value[(int)enum_medicine_page_firstclass.藥品條碼1] = data.藥品條碼;
                value[(int)enum_medicine_page_firstclass.庫存] = data.庫存;
                value[(int)enum_medicine_page_firstclass.安全庫存] = data.安全庫存;

                sQLControl_medicine_page.UpdateByDefult(null, enum_medicine_page_firstclass.藥品碼.GetEnumName(), data.藥品碼, value);

                return "200";
            }


        }
        // DELETE api/<medicine_page_firstclassController>/5
        [HttpDelete]
        public string Delete([FromBody] class_medicine_page_firstclass_data data)
        {

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                string jsonString = JsonSerializer.Serialize<class_medicine_page_firstclass_data>(data, options);
            }
            catch
            {
                return "-1";
            }
            if (!(sQLControl_medicine_page.GetRowsByDefult(null, enum_medicine_page_firstclass.藥品碼.GetEnumName(), data.藥品碼).Count > 0))
            {
                return "-2";
            }
            else
            {
                sQLControl_medicine_page.DeleteByDefult(null, enum_medicine_page_firstclass.藥品碼.GetEnumName(), data.藥品碼);
                return "200";
            }
        }


        #region Function
        List<DeviceBasic> devices = new List<DeviceBasic>();

        private void Function_讀取儲位()
        {

 
            List<Storage> pannel35s = StorageMethod.SQL_GetAllStorage(sQLControl_WT32_serialize);
  

            List<DeviceBasic> devices_pannel35s = pannel35s.GetAllDeviceBasic();


            for (int i = 0; i < devices_pannel35s.Count; i++)
            {
                this.devices.Add(devices_pannel35s[i]);
                continue;
                if (devices_pannel35s[i].Code.StringIsEmpty() != true)
                {
                    this.devices.Add(devices_pannel35s[i]);
                }
            }
        
        }
        private int Function_取得儲位庫存(string 藥品碼)
        {
            int 庫存 = 0;
            List<DeviceBasic> deviceBasics = (from value in devices
                                              where value.Code == 藥品碼
                                              select value).ToList();
            for (int k = 0; k < deviceBasics.Count; k++)
            {
                庫存 += deviceBasics[k].Inventory.StringToInt32();
            }
            return 庫存;
        }
        #endregion
    }
}
