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

namespace 智能藥品管理系統_WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class testController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            string str = System.Environment.CurrentDirectory;
            //string jsonstring = MyFileStream.LoadFileAllText(@"C:\Users\HS\Documents\智能藥品管理系統_WebApi\智能藥品管理系統_WebApi\bin\Release\net5.0\test.txt", "utf-8");
            string jsonstring = MyFileStream.LoadFileAllText(@$"{str}\test.txt", "utf-8");
            return "Api test sucess!";
        }
    }
}
