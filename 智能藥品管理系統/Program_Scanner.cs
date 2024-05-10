using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ArcSoftFace.SDKModels;
using ArcSoftFace.SDKUtil;
using ArcSoftFace.Utils;
using ArcSoftFace.Entity;
using MyFaceID;
using MyUI;
using Basic;
namespace 智能藥品管理系統
{
   
    public partial class Form1 : Form
    {
        private enum enum_Scanner_陣列內容
        {
            藥袋序號 = 0,
            病人姓名 = 2,
            病歷號 = 1,
            藥品碼 = 11,
            使用數量 = 10,
            開方日期 = 12,
            開方時間 = 13,
            藥品名稱 = 5,
            中文名稱,
            包裝單位,
        }

        private string Scanner_Readline = "";
        private MySerialPort MySerialPort_Scanner = new MySerialPort();
        private void Program_Scanner_Init()
        {
            this.MySerialPort_Scanner.Init("COM2", 115200, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One,true);
            if(!this.MySerialPort_Scanner.IsConnected)
            {
                MyMessageBox.ShowDialog("掃碼器初始化失敗!");
            }
        }
        private void Program_Scanner()
        {
            sub_Program_Scanner_讀取藥單資料();
        }


        #region PLC_Scanner_讀取藥單資料
        
        PLC_Device PLC_Device_Scanner_讀取藥單資料 = new PLC_Device("S3005");
        PLC_Device PLC_Device_Scanner_讀取藥單資料_OK = new PLC_Device("S3006");
        string[] Scanner_讀取藥單資料_Array;
        MyTimer MyTimer_Scanner_讀取藥單資料 = new MyTimer();
        int cnt_Program_Scanner_讀取藥單資料 = 65534;
        void sub_Program_Scanner_讀取藥單資料()
        {
            if (cnt_Program_Scanner_讀取藥單資料 == 65534)
            {
                PLC_Device_Scanner_讀取藥單資料.SetComment("PLC_Scanner_讀取藥單資料");
                PLC_Device_Scanner_讀取藥單資料_OK.SetComment("PLC_Scanner_讀取藥單資料_OK");
                PLC_Device_Scanner_讀取藥單資料.Bool = false;
                cnt_Program_Scanner_讀取藥單資料 = 65535;
            }
            if (cnt_Program_Scanner_讀取藥單資料 == 65535) cnt_Program_Scanner_讀取藥單資料 = 1;
            if (cnt_Program_Scanner_讀取藥單資料 == 1) cnt_Program_Scanner_讀取藥單資料_檢查按下(ref cnt_Program_Scanner_讀取藥單資料);
            if (cnt_Program_Scanner_讀取藥單資料 == 2) cnt_Program_Scanner_讀取藥單資料_初始化(ref cnt_Program_Scanner_讀取藥單資料);
            if (cnt_Program_Scanner_讀取藥單資料 == 3) cnt_Program_Scanner_讀取藥單資料_等待接收延遲(ref cnt_Program_Scanner_讀取藥單資料);
            if (cnt_Program_Scanner_讀取藥單資料 == 4) cnt_Program_Scanner_讀取藥單資料_檢查接收結果(ref cnt_Program_Scanner_讀取藥單資料);
            if (cnt_Program_Scanner_讀取藥單資料 == 5) cnt_Program_Scanner_讀取藥單資料 = 65500;
            if (cnt_Program_Scanner_讀取藥單資料 > 1) cnt_Program_Scanner_讀取藥單資料_檢查放開(ref cnt_Program_Scanner_讀取藥單資料);

            if (cnt_Program_Scanner_讀取藥單資料 == 65500)
            {
                PLC_Device_Scanner_讀取藥單資料.Bool = false;
                cnt_Program_Scanner_讀取藥單資料 = 65535;
            }
        }
        void cnt_Program_Scanner_讀取藥單資料_檢查按下(ref int cnt)
        {
            if (PLC_Device_Scanner_讀取藥單資料.Bool) cnt++;
        }
        void cnt_Program_Scanner_讀取藥單資料_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Scanner_讀取藥單資料.Bool) cnt = 65500;
        }
        void cnt_Program_Scanner_讀取藥單資料_初始化(ref int cnt)
        {
            PLC_Device_Scanner_讀取藥單資料_OK.Bool = false;
            if (MySerialPort_Scanner.ReadByte() != null)
            {
                MyTimer_Scanner_讀取藥單資料.StartTickTime(100);
                cnt++;
            }

        }
        void cnt_Program_Scanner_讀取藥單資料_等待接收延遲(ref int cnt)
        {

            if (MyTimer_Scanner_讀取藥單資料.IsTimeOut())
            {
                cnt++;
            }

        }
        void cnt_Program_Scanner_讀取藥單資料_檢查接收結果(ref int cnt)
        {
            if (MySerialPort_Scanner.ReadByte() != null)
            {
               
                string text = this.MySerialPort_Scanner.ReadString();
                text = text.Replace("\0", "");
                Console.WriteLine($"接收資料長度 : {text.Length} ");
                this.MySerialPort_Scanner.ClearReadByte();
                if (text.Length <= 2 || text.Length > 300)
                {
                    Console.WriteLine($"接收資料長度異常");
                    cnt = 65500;
                    return;
                }
                if (text.Substring(text.Length - 2, 2) != "\r\n")
                {
                    Console.WriteLine($"接收資料結束碼異常");
                    cnt = 65500;
                    return;
                }
                text = text.Replace("\r\n", "");
                Console.WriteLine($"接收結尾碼!");
                string[] array = text.Split(';');
                if (array.Length <= 15)
                {
                    Console.WriteLine($"接收資料長度分析內容異常");
                    cnt = 65500;
                    return;
                }
                string 藥袋序號 = array[(int)enum_Scanner_陣列內容.藥袋序號];
                string 病人姓名 = array[(int)enum_Scanner_陣列內容.病人姓名];
                string 藥品代碼 = array[(int)enum_Scanner_陣列內容.藥品碼];
                string 使用數量 = array[(int)enum_Scanner_陣列內容.使用數量];
                string 病歷號 = array[(int)enum_Scanner_陣列內容.病歷號];
                string 開方日期 = array[(int)enum_Scanner_陣列內容.開方日期];
                string 開方時間 = array[(int)enum_Scanner_陣列內容.開方時間];

                string[] 開方日期_array = myConvert.分解分隔號字串(開方日期, "-");
                if(開方日期_array.Length == 2 && 開方時間.Length == 6)
                {
                    array[(int)enum_Scanner_陣列內容.開方日期] = $"{DateTime.Now.Year}/{開方日期_array[0]}/{開方日期_array[1]}";
                    string Hour = 開方時間.Substring(0, 2);
                    string Min = 開方時間.Substring(2, 2);
                    string Sec = 開方時間.Substring(4, 2);
                    array[(int)enum_Scanner_陣列內容.開方時間] = $"{ array[(int)enum_Scanner_陣列內容.開方日期]} {Hour}:{Min}:{Sec}";

                }
                else
                {
                    array[(int)enum_Scanner_陣列內容.開方日期] = DateTime.Now.ToDateString();
                    array[(int)enum_Scanner_陣列內容.開方時間] = DateTime.Now.ToTimeString();
                }

                array[(int)enum_Scanner_陣列內容.使用數量] = ((int)Math.Ceiling(使用數量.StringToDouble())).ToString();

              
                List<object[]> list_藥品資料 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetRows((int)enum_參數設定_藥檔資料.藥品碼, 藥品代碼, false);
                if(list_藥品資料.Count == 0)
                {
                    Console.WriteLine($"查無此藥品代碼({藥品代碼})");
                    cnt = 65500;
                    return;
                }
                array[(int)enum_Scanner_陣列內容.藥品名稱] = list_藥品資料[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
                array[(int)enum_Scanner_陣列內容.中文名稱] = list_藥品資料[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
                array[(int)enum_Scanner_陣列內容.包裝單位] = list_藥品資料[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();

                Console.WriteLine($"---------------------------------------------------------------");
                Console.WriteLine($"藥袋序號:{array[(int)enum_Scanner_陣列內容.藥袋序號]}");
                Console.WriteLine($"病人姓名:{array[(int)enum_Scanner_陣列內容.病人姓名]}");
                Console.WriteLine($"藥品碼:{array[(int)enum_Scanner_陣列內容.藥品碼]}");
                Console.WriteLine($"中文名稱:{array[(int)enum_Scanner_陣列內容.中文名稱]}");
                Console.WriteLine($"包裝單位:{array[(int)enum_Scanner_陣列內容.包裝單位]}");
                Console.WriteLine($"使用數量:{array[(int)enum_Scanner_陣列內容.使用數量]}");
                Console.WriteLine($"開方日期:{array[(int)enum_Scanner_陣列內容.開方日期]}");
                Console.WriteLine($"開方時間:{array[(int)enum_Scanner_陣列內容.開方時間]}");
                Console.WriteLine($"---------------------------------------------------------------");

                for (int i = 0; i < array.Length; i++)
                {
                    Console.WriteLine($"{((enum_Scanner_陣列內容)i).GetEnumName()} : {array[i]}");
                }
                PLC_Device_Scanner_讀取藥單資料_OK.Bool = true;
                Scanner_讀取藥單資料_Array = array.DeepClone();
                cnt++;
                return;
            }
        }










        #endregion
    }
}
