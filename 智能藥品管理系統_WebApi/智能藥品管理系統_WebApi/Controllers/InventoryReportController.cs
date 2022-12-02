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
    public class InventoryReportController
    {
        public class class_InventoryReport_data
        {
            private List<dataClass> _code_all = new List<dataClass>();

            [JsonPropertyName("inventory_id")]
            public string inventory_id { get; set; }
            [JsonPropertyName("user_id")]
            public string user_id { get; set; }
            [JsonPropertyName("code_all")]
            public List<dataClass> code_all { get => _code_all; set => _code_all = value; }

            public class dataClass
            {

                public dataClass(string code, string inventory)
                {
                    this.code = code;
                    this.inventory = inventory;
                }

                private string _code;
                private string _inventory;

                [JsonPropertyName("code")]
                public string code { get => _code; set => _code = value; }
                [JsonPropertyName("inventory")]
                public string inventory { get => _inventory; set => _inventory = value; }
            }
        }

        [Route("test")]
        [HttpGet]
        public string Get()
        {
            class_InventoryReport_data class_InventoryReport_Data = new class_InventoryReport_data();
            class_InventoryReport_Data.user_id = "TEST";
            class_InventoryReport_Data.inventory_id = "001";
            class_InventoryReport_Data.code_all.Add(new class_InventoryReport_data.dataClass("02002", "100"));
            class_InventoryReport_Data.code_all.Add(new class_InventoryReport_data.dataClass("10050", "200"));
            return class_InventoryReport_Data.JsonSerializationt();
        }
        [HttpPost]
        public string Post([FromBody] class_InventoryReport_data data)
        {
            
            return "200";
        }
    }
}
