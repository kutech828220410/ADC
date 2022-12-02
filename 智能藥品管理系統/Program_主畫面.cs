using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyFaceID;
using ArcSoftFace.SDKModels;
using ArcSoftFace.SDKUtil;
using ArcSoftFace.Utils;
using ArcSoftFace.Entity;
using MyUI;
using Basic;
using RFID_FX600lib;
using MySQL_Login;
using H_Pannel_lib;


namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {
        public enum enum_主畫面_藥單列表_狀態
        {
            等待開始作業,
            等待作業,
            作業中,
            作業完成,
            庫存不足,
            未搜尋到儲位,
            無法找出組合,
        }
        public enum enum_主畫面_藥單列表
        {
            GUID,
            序號,
            動作,
            藥品碼,
            藥品名稱,
            單位,
            庫存量,
            交易量,
            結存量,
            藥袋序號,
            房號,
            病人姓名,
            病歷號,
            開方時間,
            操作時間,
            狀態,
        }
        private MyThread MyThread_取藥堆疊;
        private List<object[]> List_人員資料 = new List<object[]>();
        private string 主畫面_登入者姓名 = "";
        private string 主畫面_登入者ID = "";
        private void Program_主畫面_Init()
        {
            this.textBox_主畫面_密碼.PassWordChar = true;
            this.sqL_DataGridView_主畫面_領退藥作業列表.Init();
            if(!this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_CreateTable();
            }
            this.sqL_DataGridView_主畫面_領退藥作業列表.DataGridRefreshEvent += SqL_DataGridView_主畫面_領退藥作業列表_DataGridRefreshEvent;
            this.MyThread_取藥堆疊 = new MyThread();
            this.MyThread_取藥堆疊.Add_Method(sub_Program_主畫面_取藥堆疊檢查);
            this.MyThread_取藥堆疊.AutoRun(true);
            this.MyThread_取藥堆疊.SetSleepTime(10);
            this.MyThread_取藥堆疊.Trigger();
        } 

        private bool flag_Program_主畫面 = false;
        private void Program_主畫面()
        {
            if (this.plC_ScreenPage_Main.PageText == "主畫面")
            {
                if (!flag_Program_主畫面)
                {
                    this.MySerialPort_Scanner.ClearReadByte();

                    this.List_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                    this.List_人員資料 = (from value in List_人員資料
                                      where value[(int)enum_人員資料.識別圖案].ObjectToString().StringIsEmpty() == false
                                      select value).ToList();
                    myFaceIDUI_Main.ClearFaceList();
                    string json = "";
                    for (int i = 0; i < this.List_人員資料.Count; i++)
                    {
                        json = this.List_人員資料[i][(int)enum_人員資料.識別圖案].ObjectToString();
                        FaceFeature faceFeature = json.JsonDeserializet<FaceFeature>();
                        myFaceIDUI_Main.RegisterFaceList(faceFeature.ToASF_FaceFeature());
                    }
                    this.Function_登出();
                    flag_Program_主畫面 = true;
                }
            }
            else
            {
                flag_Program_主畫面 = false;
            }

            sub_Program_主畫面_領退藥();
        }

        #region PLC_主畫面_領退藥
        private object[] 主畫面_領退藥_手術房;
        private object[] 主畫面_領退藥_選擇套餐;
        private object[] 主畫面_領退藥_退藥藥品;
        private int 主畫面_領退藥_退藥數量;
        private string 主畫面_領退藥_病歷號 = "";
        private string 主畫面_領退藥_病人姓名 = "";
        private List<object[]> 主畫面_領退藥_藥品選擇 = new List<object[]>();
        private enum_功能選擇 主畫面_領退藥_功能選擇 = enum_功能選擇.None;
        private enum_空瓶實瓶選擇 enum_空瓶實瓶選擇 = new enum_空瓶實瓶選擇();
        private MyUI.MyTimer MyTimer_主畫面_領退藥_退藥鎖逾時 = new MyUI.MyTimer();
        private MyUI.MyTimer MyTimer_主畫面_領退藥_馬達出料延遲 = new MyUI.MyTimer();
        MyUI.MyTimer MyTimer_主畫面_領退藥_重複登入延遲 = new MyUI.MyTimer();
        PLC_Device PLC_Device_主畫面_領退藥 = new PLC_Device("S1005");
        PLC_Device PLC_Device_主畫面_領退藥_OK = new PLC_Device("S1006");
        PLC_Device PLC_Device_主畫面_領退藥_登入按下 = new PLC_Device("S1007");
        PLC_Device PLC_Device_主畫面_領退藥_識別登入 = new PLC_Device("S1008");
        PLC_Device PLC_Device_主畫面_領退藥_取藥防夾 = new PLC_Device("X74");
        MyUI.MyTimer MyTimer_主畫面_領退藥_取藥防夾 = new MyUI.MyTimer();


        PLC_Device PLC_Device_主畫面_領藥按鈕 = new PLC_Device("S1010");
        PLC_Device PLC_Device_主畫面_退藥按鈕 = new PLC_Device("S1011");
        PLC_Device PLC_Device_主畫面_領退藥按鈕致能 = new PLC_Device("S1015");
        PLC_Device PLC_Device_主畫面_麻醉部模式 = new PLC_Device("S4040");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_01 = new PLC_Device("M3000");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_02 = new PLC_Device("M3001");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_03 = new PLC_Device("M3002");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_04 = new PLC_Device("M3003");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_05 = new PLC_Device("M3004");
        PLC_Device PLC_Device_主畫面_領退藥_狀態顯示_06 = new PLC_Device("M3005");
        int cnt_Program_主畫面_領退藥 = 65534;
        void sub_Program_主畫面_領退藥()
        {

            if (this.plC_ScreenPage_Main.PageText == "主畫面")
            {
                PLC_Device_主畫面_領退藥.Bool = true;

            }
            else
            {
                MyTimer_主畫面_領退藥_重複登入延遲.TickStop();
                MyTimer_主畫面_領退藥_重複登入延遲.StartTickTime(2000);

                PLC_Device_主畫面_領退藥.Bool = false;
            }
            if (!PLC_Device_全軸復歸_OK.Bool) PLC_Device_主畫面_領退藥.Bool = false;
            if (cnt_Program_主畫面_領退藥 == 65534)
            {
                PLC_Device_主畫面_領退藥.SetComment("PLC_主畫面_領退藥");
                PLC_Device_主畫面_領退藥_OK.SetComment("PLC_主畫面_領退藥_OK");
                PLC_Device_主畫面_領退藥.Bool = false;

                PLC_Device_主畫面_領退藥_狀態顯示_01.Bool = false;
                cnt_Program_主畫面_領退藥 = 65535;
            }
            if (cnt_Program_主畫面_領退藥 == 65535) cnt_Program_主畫面_領退藥 = 1;
            if (cnt_Program_主畫面_領退藥 == 1) cnt_Program_主畫面_領退藥_檢查按下(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2) cnt_Program_主畫面_領退藥_初始化(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3) cnt_Program_主畫面_領退藥_取藥門關門開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 4) cnt_Program_主畫面_領退藥_取藥門關門結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5)
            {
                PLC_Device_主畫面_領退藥_狀態顯示_01.Bool = true;
                cnt_Program_主畫面_領退藥 = 100;
            }
            if (cnt_Program_主畫面_領退藥 == 100) cnt_Program_主畫面_領退藥_100_檢查登入(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 101)
            {
                PLC_Device_主畫面_領退藥_狀態顯示_02.Bool = true;
                PLC_Device_主畫面_領退藥按鈕致能.Bool = true;
                this.Invoke(new Action(delegate
                {
                    plC_RJ_Button_主畫面_登入.Texts = "登出";
                    textBox_主畫面_帳號.Texts = "";
                    textBox_主畫面_密碼.Texts = "";
                }));
                cnt_Program_主畫面_領退藥 = 200;
            }
            if (cnt_Program_主畫面_領退藥 == 200) cnt_Program_主畫面_領退藥_200_選擇領退藥(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 201) cnt_Program_主畫面_領退藥_200_檢查模式(ref cnt_Program_主畫面_領退藥);           
            if (cnt_Program_主畫面_領退藥 == 202)
            {
                cnt_Program_主畫面_領退藥 = 300;
            }

            if (cnt_Program_主畫面_領退藥 == 300) cnt_Program_主畫面_領退藥_300_選擇手術房(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 301)
            {
                cnt_Program_主畫面_領退藥 = 400;
            }

            if (cnt_Program_主畫面_領退藥 == 400) cnt_Program_主畫面_領退藥_400_輸入病歷號及病人姓名(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 401)
            {
                cnt_Program_主畫面_領退藥 = 500;
            }

            if (cnt_Program_主畫面_領退藥 == 500) cnt_Program_主畫面_領退藥_500_功能選擇(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 501)
            {
                cnt_Program_主畫面_領退藥 = 600;
            }

            if (cnt_Program_主畫面_領退藥 == 1000) cnt_Program_主畫面_領退藥_1000_領藥_選擇套餐(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1001) cnt_Program_主畫面_領退藥_1000_領藥_檢查重複領取(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1002) cnt_Program_主畫面_領退藥_1000_領藥_寫入取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1003) cnt_Program_主畫面_領退藥_1000_領藥_檢查取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1004) cnt_Program_主畫面_領退藥_1000_領藥_至出貨位置開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1005) cnt_Program_主畫面_領退藥_1000_領藥_至出貨位置結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1006) cnt_Program_主畫面_領退藥_1000_領藥_輸送帶開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1007) cnt_Program_主畫面_領退藥_1000_領藥_輸送帶結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1008) cnt_Program_主畫面_領退藥_1000_領藥_開取藥門開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1009) cnt_Program_主畫面_領退藥_1000_領藥_開取藥門結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 1010)
            {
                cnt_Program_主畫面_領退藥 = 65490;
            }

            if (cnt_Program_主畫面_領退藥 == 2000) cnt_Program_主畫面_領退藥_2000_領藥_藥品選擇(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2001) cnt_Program_主畫面_領退藥_2000_領藥_寫入取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2002) cnt_Program_主畫面_領退藥_2000_領藥_檢查取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2003) cnt_Program_主畫面_領退藥_2000_領藥_至出貨位置開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2004) cnt_Program_主畫面_領退藥_2000_領藥_至出貨位置結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2005) cnt_Program_主畫面_領退藥_2000_領藥_輸送帶開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2006) cnt_Program_主畫面_領退藥_2000_領藥_輸送帶結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2007) cnt_Program_主畫面_領退藥_2000_領藥_開取藥門開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2008) cnt_Program_主畫面_領退藥_2000_領藥_開取藥門結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 2009)
            {
                cnt_Program_主畫面_領退藥 = 65490;
            }

            if (cnt_Program_主畫面_領退藥 == 3000) cnt_Program_主畫面_領退藥_3000_退藥_藥品選擇(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3001) cnt_Program_主畫面_領退藥_3000_退藥_空瓶實瓶選擇(ref cnt_Program_主畫面_領退藥);            
            if (cnt_Program_主畫面_領退藥 == 3002) cnt_Program_主畫面_領退藥_3000_退藥_等待退藥開鎖打開(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3003) cnt_Program_主畫面_領退藥_3000_退藥_退藥開鎖打開結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3004) cnt_Program_主畫面_領退藥_3000_退藥_開啟退藥掃碼頁面(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3005) cnt_Program_主畫面_領退藥_3000_退藥_寫入交易紀錄(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 3006)
            {
                cnt_Program_主畫面_領退藥 = 65500;
            }


            if (cnt_Program_主畫面_領退藥 == 5000) cnt_Program_主畫面_領退藥_5000_領藥_等待掃碼(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5001) cnt_Program_主畫面_領退藥_5000_領藥_掃碼完成(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5002) cnt_Program_主畫面_領退藥_5000_領藥_檢查重複領藥(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5003) cnt_Program_主畫面_領退藥_5000_領藥_寫入取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5004)
            {
                cnt_Program_主畫面_領退藥 = 5000;
            }

            if (cnt_Program_主畫面_領退藥 == 5100) cnt_Program_主畫面_領退藥_5100_領藥_檢查取藥堆疊(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5101) cnt_Program_主畫面_領退藥_5100_領藥_至出貨位置開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5102) cnt_Program_主畫面_領退藥_5100_領藥_至出貨位置結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5103) cnt_Program_主畫面_領退藥_5100_領藥_輸送帶開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5104) cnt_Program_主畫面_領退藥_5100_領藥_輸送帶結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5105) cnt_Program_主畫面_領退藥_5100_領藥_開取藥門開始(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5106) cnt_Program_主畫面_領退藥_5100_領藥_開取藥門結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 5107)
            {
                cnt_Program_主畫面_領退藥 = 65490;
            }


            if (cnt_Program_主畫面_領退藥 == 6000) cnt_Program_主畫面_領退藥_6000_退藥_等待掃碼(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6001) cnt_Program_主畫面_領退藥_6000_退藥_掃碼完成(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6002) cnt_Program_主畫面_領退藥_6000_退藥_輸入退藥數量(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6003) cnt_Program_主畫面_領退藥_6000_退藥_等待退藥開鎖打開(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6004) cnt_Program_主畫面_領退藥_3000_退藥_退藥開鎖打開結束(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6005) cnt_Program_主畫面_領退藥_6000_退藥_寫入交易紀錄(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 6006)
            {
                cnt_Program_主畫面_領退藥 = 65500;
            }
            if (cnt_Program_主畫面_領退藥 == 3990) cnt_Program_主畫面_領退藥_3990_退藥_開鎖逾時(ref cnt_Program_主畫面_領退藥);


            if (cnt_Program_主畫面_領退藥 > 1) cnt_Program_主畫面_領退藥_檢查放開(ref cnt_Program_主畫面_領退藥);

            if (cnt_Program_主畫面_領退藥 == 65490) cnt_Program_主畫面_領退藥_65490_初始化(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 65491) cnt_Program_主畫面_領退藥_65490_取藥防夾ON(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 65492) cnt_Program_主畫面_領退藥_65490_取藥防夾OFF(ref cnt_Program_主畫面_領退藥);
            if (cnt_Program_主畫面_領退藥 == 65492) cnt_Program_主畫面_領退藥 = 65501;

            if (cnt_Program_主畫面_領退藥 == 65500)
            {
                //MyTimer_主畫面_領退藥_重複登入延遲.TickStop();
                //MyTimer_主畫面_領退藥_重複登入延遲.StartTickTime(2000);

                cnt_Program_主畫面_領退藥 = 65501;
            }
            if (cnt_Program_主畫面_領退藥 == 65501)
            {
                PLC_Device_主畫面_領退藥_識別登入.Bool = false;

                PLC_Device_主畫面_領退藥_狀態顯示_01.Bool = false;
                PLC_Device_主畫面_領退藥_狀態顯示_02.Bool = false;
                PLC_Device_主畫面_領退藥_狀態顯示_03.Bool = false;
                PLC_Device_主畫面_領退藥_狀態顯示_04.Bool = false;
                PLC_Device_主畫面_領退藥_狀態顯示_05.Bool = false;
                PLC_Device_主畫面_領退藥_狀態顯示_06.Bool = false;
                PLC_Device_主畫面_領藥按鈕.Bool = false;
                PLC_Device_主畫面_退藥按鈕.Bool = false;
                plC_RJ_Button_主畫面_開始作業.Bool = false;
                this.Invoke(new Action(delegate
                {
                    plC_RJ_Button_主畫面_登入.Texts = "登入";
                }));

                PLC_Device_主畫面_領退藥.Bool = false;
                PLC_Device_主畫面_領退藥_OK.Bool = false;


                cnt_Program_主畫面_領退藥 = 65535;
            }

            if (PLC_Device_主畫面_領退藥按鈕致能.Bool)
            {
                plC_RJ_Button_主畫面_領藥.BorderColor = Color.HotPink;
                plC_RJ_Button_主畫面_退藥.BorderColor = Color.HotPink;
                if (PLC_Device_M8013.Bool)
                {
                    plC_RJ_Button_主畫面_領藥.BorderSize = 5;
                    plC_RJ_Button_主畫面_退藥.BorderSize = 5;
                }
                else
                {
                    plC_RJ_Button_主畫面_領藥.BorderSize = 0;
                    plC_RJ_Button_主畫面_退藥.BorderSize = 0;
                }
            }
            else
            {
                plC_RJ_Button_主畫面_領藥.BorderColor = Color.Lime;
                plC_RJ_Button_主畫面_退藥.BorderColor = Color.Lime;
                if (PLC_Device_主畫面_領藥按鈕.Bool)
                {               
                    plC_RJ_Button_主畫面_領藥.BorderSize = 8;
                }
                else
                {           
                    plC_RJ_Button_主畫面_領藥.BorderSize = 0;
                }
                if (PLC_Device_主畫面_退藥按鈕.Bool)
                {
                    plC_RJ_Button_主畫面_退藥.BorderSize = 8;
                }
                else
                {
                    plC_RJ_Button_主畫面_退藥.BorderSize = 0;
                }
            }

      
        }
        void cnt_Program_主畫面_領退藥_65490_初始化(ref int cnt)
        {
            MyTimer_主畫面_領退藥_重複登入延遲.TickStop();
            MyTimer_主畫面_領退藥_重複登入延遲.StartTickTime(10000);
            MyTimer_主畫面_領退藥_取藥防夾.TickStop();
            MyTimer_主畫面_領退藥_取藥防夾.StartTickTime(200);
            cnt++;
        }
        void cnt_Program_主畫面_領退藥_65490_取藥防夾ON(ref int cnt)
        {
            if (MyTimer_主畫面_領退藥_重複登入延遲.IsTimeOut())
            {
                cnt = 65501;
                return;
            }
            if (PLC_Device_主畫面_領退藥_取藥防夾.Bool)
            {
                if (MyTimer_主畫面_領退藥_取藥防夾.IsTimeOut())
                {
                    MyTimer_主畫面_領退藥_取藥防夾.TickStop();
                    MyTimer_主畫面_領退藥_取藥防夾.StartTickTime(200);
                    cnt++;
                }
            }
            else
            {
                MyTimer_主畫面_領退藥_取藥防夾.TickStop();
                MyTimer_主畫面_領退藥_取藥防夾.StartTickTime(200);
            }
        }
        void cnt_Program_主畫面_領退藥_65490_取藥防夾OFF(ref int cnt)
        {
            if (MyTimer_主畫面_領退藥_重複登入延遲.IsTimeOut())
            {
                cnt = 65501;
                return;
            }
            if (!PLC_Device_主畫面_領退藥_取藥防夾.Bool)
            {
                if (MyTimer_主畫面_領退藥_取藥防夾.IsTimeOut())
                {
                    MyTimer_主畫面_領退藥_取藥防夾.TickStop();
                    MyTimer_主畫面_領退藥_取藥防夾.StartTickTime(200);
                    cnt++;
                }
            }
            else
            {
                MyTimer_主畫面_領退藥_取藥防夾.TickStop();
                MyTimer_主畫面_領退藥_取藥防夾.StartTickTime(200);
            }
        }

        void cnt_Program_主畫面_領退藥_檢查按下(ref int cnt)
        {
            if (PLC_Device_主畫面_領退藥.Bool) cnt++;
        }  
        void cnt_Program_主畫面_領退藥_檢查放開(ref int cnt)
        {
            if (!PLC_Device_主畫面_領退藥.Bool) cnt = 65500;
        }
        void cnt_Program_主畫面_領退藥_初始化(ref int cnt)
        {
            PLC_Device_主畫面_領退藥_狀態顯示_01.Bool = false;
            PLC_Device_主畫面_領退藥_狀態顯示_02.Bool = false;
            PLC_Device_主畫面_領退藥_狀態顯示_03.Bool = false;
            PLC_Device_主畫面_領退藥_狀態顯示_04.Bool = false;
            PLC_Device_主畫面_領退藥_狀態顯示_05.Bool = false;
            PLC_Device_主畫面_領退藥_狀態顯示_06.Bool = false;


            PLC_Device_主畫面_領退藥_識別登入.Bool = false;
            PLC_Device_主畫面_領退藥_登入按下.Bool = false;
            PLC_Device_主畫面_領藥按鈕.Bool = false;
            PLC_Device_主畫面_退藥按鈕.Bool = false;
            PLC_Device_主畫面_領退藥按鈕致能.Bool = false;
            plC_RJ_Button_主畫面_開始作業.Bool = false;
            while (true)
            {
                List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
                if (list_value.Count == 0) break;
                this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_DeleteExtra(list_value, true);
            }
            
            cnt++;
        }
        void cnt_Program_主畫面_領退藥_取藥門關門開始(ref int cnt)
        {
            if(!PLC_Device_取物門_移動到關門位置.Bool)
            {
                PLC_Device_取物門_移動到關門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_取藥門關門結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到關門位置.Bool)
            {
                cnt++;
            }
        }

        void cnt_Program_主畫面_領退藥_100_檢查登入(ref int cnt)
        {
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("Consolas", 16F, FontStyle.Bold), true);
            狀態顯示 += $"請登入(RFID.人臉識別.密碼)...";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_01.GetAdress(), 狀態顯示);

            List<RFID_FX600_UI.RFID_Device> rFID_Devices = this.rfiD_FX600_UI.Get_RFID();
            if (rFID_Devices.Count > 0)
            {
                string 卡號 = rFID_Devices[0].UID;
                List<object[]> list_人員資料 = this.List_人員資料;
                List<object[]> list_人員資料_buf = new List<object[]>();
                list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.卡號, 卡號);
                if (list_人員資料_buf.Count > 0)
                {
                    string ID = list_人員資料_buf[0][(int)enum_人員資料.ID].ObjectToString();
                    string 姓名 = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                    this.主畫面_登入者姓名 = 姓名;
                    this.主畫面_登入者ID = ID;
                    Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.RFID登入, 姓名, ID);

                    狀態顯示 = "";
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("Consolas", 16F, FontStyle.Bold), true);
                    狀態顯示 += $"--------------登入資訊---------------\n";
                    狀態顯示 += $"登入方式 ".StringLength(10) + $": RFID\n";
                    狀態顯示 += $"姓名 ".StringLength(10) + $": {姓名}\n";
                    狀態顯示 += $"ID ".StringLength(10) +$": {ID}\n";
                    狀態顯示 += $"-------------------------------------\n";
                    this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_01.GetAdress(), 狀態顯示);
                    cnt++;
                    return;
                }             
            }
            if (retryToSucess >= 2)
            {
                int index = FaceTest_index;
                if (FaceTest_index >= 0)
                {
                    string ID = this.List_人員資料[index][(int)enum_人員資料.ID].ObjectToString();
                    string 姓名 = this.List_人員資料[index][(int)enum_人員資料.姓名].ObjectToString();
                    this.主畫面_登入者姓名 = 姓名;
                    this.主畫面_登入者ID = ID;
                    Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.人臉識別登入, 姓名, ID);

                    狀態顯示 = "";
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                    狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("Consolas", 16F, FontStyle.Bold), true);
                    狀態顯示 += $"--------------登入資訊---------------\n";
                    狀態顯示 += $"登入方式 ".StringLength(10) + $": 人臉識別\n";
                    狀態顯示 += $"姓名 ".StringLength(10) + $": {姓名}\n";
                    狀態顯示 += $"ID ".StringLength(10) + $": {ID}\n";
                    狀態顯示 += $"-------------------------------------\n";
                    this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_01.GetAdress(), 狀態顯示);
                    PLC_Device_主畫面_領退藥_識別登入.Bool = false;
                    cnt++;
                    return;
                }
            }
            if(PLC_Device_主畫面_領退藥_登入按下.Bool)
            {
                PLC_Device_主畫面_領退藥_登入按下.Bool = false;
                string ID = textBox_主畫面_帳號.Texts;
                string 密碼 = textBox_主畫面_密碼.Texts;
                List<object[]> list_人員資料 = this.List_人員資料;
                List<object[]> list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.ID, ID);
                if (list_人員資料_buf.Count > 0)
                {
                    if (list_人員資料_buf[0][(int)enum_人員資料.密碼].ObjectToString() == 密碼)
                    {
                        string 姓名 = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                        Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.密碼登入, 姓名, ID);

                        狀態顯示 = "";
                        狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                        狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                        狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("Consolas", 16F, FontStyle.Bold), true);
                        狀態顯示 += $"--------------登入資訊---------------\n";
                        狀態顯示 += $"登入方式 ".StringLength(10) + $": 帳號密碼\n";
                        狀態顯示 += $"姓名 ".StringLength(10) + $": {姓名}\n";
                        狀態顯示 += $"ID ".StringLength(10) + $": {ID}\n";
                        狀態顯示 += $"-------------------------------------\n";
                        this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_01.GetAdress(), 狀態顯示);
                        cnt++;
                        return;
                    }
                }
            }
        }

        void cnt_Program_主畫面_領退藥_200_選擇領退藥(ref int cnt)
        {
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"請選擇領/退藥";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_02.GetAdress(), 狀態顯示);


            if (PLC_Device_主畫面_領藥按鈕.Bool)
            {
                狀態顯示 = "";
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
                狀態顯示 += $"模式".StringLength(8) + ": <領藥>";
                this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_02.GetAdress(), 狀態顯示);

                PLC_Device_主畫面_退藥按鈕.Bool = false;
                cnt++;
            }
            else if(PLC_Device_主畫面_退藥按鈕.Bool)
            {
                狀態顯示 = "";
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
                狀態顯示 += $"模式".StringLength(8) + ": <退藥>";
                this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_02.GetAdress(), 狀態顯示);
                PLC_Device_主畫面_領藥按鈕.Bool = false;
                cnt++;
            }    
        }
        void cnt_Program_主畫面_領退藥_200_檢查模式(ref int cnt)
        {
            PLC_Device_主畫面_領退藥按鈕致能.Bool = false;

            if (PLC_Device_主畫面_麻醉部模式.Bool)
            {
                cnt++;
                return;
            }
            else
            {
                if (PLC_Device_主畫面_領藥按鈕.Bool)
                {
                    cnt = 5000;
                    return;
                }
                if (PLC_Device_主畫面_退藥按鈕.Bool)
                {
                    cnt = 6000;
                    return;
                }
            }
          
        }

        void cnt_Program_主畫面_領退藥_300_選擇手術房(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            Dialog_手術房選擇 dialog_手術房選擇;
            this.Invoke(new Action(delegate
            {
                dialog_手術房選擇 = new Dialog_手術房選擇(this.sqL_DataGridView_參數設定_手術房設定_手術房列表);
                dialogResult = dialog_手術房選擇.ShowDialog();
                主畫面_領退藥_手術房 = dialog_手術房選擇.Value;
            }));
            if (dialogResult == DialogResult.Yes)
            {

                string 狀態顯示 = "";
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
                狀態顯示 += $"房代碼".StringLength(8) + $": <{主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.代碼]}>" + " \n";
                狀態顯示 += $"房名稱".StringLength(8) + $": <{主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.名稱]}>" + "\n";
                this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_03.GetAdress(), 狀態顯示);
                PLC_Device_主畫面_領退藥_狀態顯示_03.Bool = true;
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }
        }

        void cnt_Program_主畫面_領退藥_400_輸入病歷號及病人姓名(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            Dialog_病歷號及病人姓名輸入 dialog_病歷號及病人姓名輸入;
            this.Invoke(new Action(delegate
            {
                dialog_病歷號及病人姓名輸入 = new Dialog_病歷號及病人姓名輸入();
                dialogResult = dialog_病歷號及病人姓名輸入.ShowDialog();
                主畫面_領退藥_病歷號 = dialog_病歷號及病人姓名輸入.病歷號;
                主畫面_領退藥_病人姓名 = dialog_病歷號及病人姓名輸入.病人姓名;
            }));
            if (dialogResult == DialogResult.Yes)
            {

                string 狀態顯示 = "";
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
                狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
                狀態顯示 += $"病歷號".StringLength(8) + $": <{主畫面_領退藥_病歷號}>" + " \n";
                狀態顯示 += $"病人姓名".StringLength(8) + $": <{主畫面_領退藥_病人姓名}>" + " \n";
                this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_04.GetAdress(), 狀態顯示);
                PLC_Device_主畫面_領退藥_狀態顯示_04.Bool = true;
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }
        }

        void cnt_Program_主畫面_領退藥_500_功能選擇(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            Dialog_功能選擇 dialog_功能選擇;
            if (PLC_Device_主畫面_領藥按鈕.Bool)
            {
                this.Invoke(new Action(delegate
                {
                    dialog_功能選擇 = new Dialog_功能選擇();
                    dialogResult = dialog_功能選擇.ShowDialog();
                    主畫面_領退藥_功能選擇 = dialog_功能選擇.Value;
                }));
            }
            else
            {
                主畫面_領退藥_功能選擇 = enum_功能選擇.藥品;
            }
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Black, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"功能".StringLength(8) + $": <{主畫面_領退藥_功能選擇.GetEnumName()}>" + " \n";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_05.GetAdress(), 狀態顯示);
            PLC_Device_主畫面_領退藥_狀態顯示_05.Bool = true;
            if(主畫面_領退藥_功能選擇 == enum_功能選擇.套餐 && PLC_Device_主畫面_領藥按鈕.Bool)
            {
                cnt = 1000;
                return;
            }
            if (主畫面_領退藥_功能選擇 == enum_功能選擇.藥品 && PLC_Device_主畫面_領藥按鈕.Bool)
            {
                cnt = 2000;
                return;
            }
            if (主畫面_領退藥_功能選擇 == enum_功能選擇.藥品 && PLC_Device_主畫面_退藥按鈕.Bool)
            {
                cnt = 3000;
                return;
            }
    
            if (主畫面_領退藥_功能選擇 == enum_功能選擇.None)
            {
                cnt = 65500;
                return;
            }
        }

        //麻醉部模式
        void cnt_Program_主畫面_領退藥_1000_領藥_選擇套餐(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate 
            {
                Dialog_套餐選擇 dialog_套餐選擇 = new Dialog_套餐選擇(this.sqL_DataGridView_入庫作業_套餐資料, this.sqL_DataGridView_入庫作業_套餐內容);
                dialogResult = dialog_套餐選擇.ShowDialog();
                主畫面_領退藥_選擇套餐 = dialog_套餐選擇.Value;
            }));
            if(dialogResult == DialogResult.Yes)
            {
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }
       
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_檢查重複領取(ref int cnt)
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetAllRows(false);
            string 房號 = 主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.名稱].ObjectToString();
            string 套餐代碼 = 主畫面_領退藥_選擇套餐[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();

            DateTime dateTime_start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            DateTime dateTime_end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            list_value = list_value.GetRows((int)enum_交易記錄查詢資料.房名, 房號);
            list_value = list_value.GetRows((int)enum_交易記錄查詢資料.藥品碼, 套餐代碼);
            list_value = list_value.GetRowsInDate((int)enum_交易記錄查詢資料.操作時間, dateTime_start, dateTime_end);

            if(list_value.Count > 0)
            {
                string 操作時間 = list_value[0][(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString();
                DialogResult dialogResult = DialogResult.None;
                this.Invoke(new Action(delegate
                {
                    string str = $"房號 : {房號}\n" + $"套餐代碼 : {套餐代碼}\n" + $"操作時間 : {操作時間}\n" + "已領取過,是否重複領取?";
                    dialogResult = MyMessageBox.ShowDialog(str, MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel);
                }));
                if (dialogResult == DialogResult.No)
                {
                    cnt = 65500;
                    return;
                }
            }
            cnt++;
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_寫入取藥堆疊(ref int cnt)
        {
            object[] value = new object[new enum_主畫面_藥單列表().GetLength()];
            value[(int)enum_主畫面_藥單列表.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_主畫面_藥單列表.序號] = this.sqL_DataGridView_主畫面_領退藥作業列表.GetAllRows().Count + 1;
            value[(int)enum_主畫面_藥單列表.動作] = enum_交易記錄查詢動作.手輸領藥.GetEnumName();
            value[(int)enum_主畫面_藥單列表.藥品碼] = 主畫面_領退藥_選擇套餐[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼];
            value[(int)enum_主畫面_藥單列表.藥品名稱] = 主畫面_領退藥_選擇套餐[(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱];
            value[(int)enum_主畫面_藥單列表.交易量] = (-1).ToString();
            value[(int)enum_主畫面_藥單列表.房號] = 主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.名稱];
            value[(int)enum_主畫面_藥單列表.病歷號] = 主畫面_領退藥_病歷號;
            value[(int)enum_主畫面_藥單列表.病人姓名] = 主畫面_領退藥_病人姓名;
            value[(int)enum_主畫面_藥單列表.藥袋序號] = "";
            value[(int)enum_主畫面_藥單列表.單位] = "Package";
            value[(int)enum_主畫面_藥單列表.操作時間] = DateTime.Now.ToDateTimeString_6();
            value[(int)enum_主畫面_藥單列表.開方時間] = DateTime.Now.ToDateTimeString();
            value[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待作業.GetEnumName();


            this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_AddRow(value, true);
            cnt++;
            return;
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_檢查取藥堆疊(ref int cnt)
        {
            bool flag_ok = true;
            List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
            if(list_value.Count > 0)
            {
                for (int i = 0; i < list_value.Count; i++)
                {
                    if (list_value[i][(int)enum_主畫面_藥單列表.狀態].ObjectToString() != enum_主畫面_藥單列表_狀態.作業完成.GetEnumName())
                    {
                        flag_ok = false;
                    }
                }
            }
            if(flag_ok)
            {
                cnt++;
                return;
            }          
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_至出貨位置開始(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                PLC_Device_移動至出貨位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_至出貨位置結束(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_輸送帶開始(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                PLC_Device_輸送帶.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_輸送帶結束(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_開取藥門開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                PLC_Device_取物門_移動到開門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_1000_領藥_開取藥門結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                cnt++;
            }
        }


        void cnt_Program_主畫面_領退藥_2000_領藥_藥品選擇(ref int cnt)
        {          
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                Dialog_領藥_藥品選擇 Dialog_領藥_藥品選擇 = new Dialog_領藥_藥品選擇(this.Function_儲位管理_儲位資料_取得儲位資料(),this.sqL_DataGridView_入庫作業_藥品資料);
                dialogResult = Dialog_領藥_藥品選擇.ShowDialog();
                主畫面_領退藥_藥品選擇 = Dialog_領藥_藥品選擇.Value;
            }));
            if (dialogResult == DialogResult.Yes)
            {
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_寫入取藥堆疊(ref int cnt)
        {
            for(int i = 0; i < 主畫面_領退藥_藥品選擇.Count; i++)
            {
                object[] value = new object[new enum_主畫面_藥單列表().GetLength()];
                value[(int)enum_主畫面_藥單列表.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_主畫面_藥單列表.序號] = this.sqL_DataGridView_主畫面_領退藥作業列表.GetAllRows().Count + 1;
                value[(int)enum_主畫面_藥單列表.動作] = enum_交易記錄查詢動作.手輸領藥.GetEnumName();
                value[(int)enum_主畫面_藥單列表.藥品碼] = 主畫面_領退藥_藥品選擇[i][(int)enum_藥品選擇.藥品碼];
                value[(int)enum_主畫面_藥單列表.藥品名稱] = 主畫面_領退藥_藥品選擇[i][(int)enum_藥品選擇.藥品名稱];
                value[(int)enum_主畫面_藥單列表.交易量] = 主畫面_領退藥_藥品選擇[i][(int)enum_藥品選擇.數量].ObjectToString().StringToInt32() * -1;
                value[(int)enum_主畫面_藥單列表.房號] = 主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.名稱];
                value[(int)enum_主畫面_藥單列表.病歷號] = 主畫面_領退藥_病歷號;
                value[(int)enum_主畫面_藥單列表.病人姓名] = 主畫面_領退藥_病人姓名;
                value[(int)enum_主畫面_藥單列表.藥袋序號] = "";
                value[(int)enum_主畫面_藥單列表.單位] = 主畫面_領退藥_藥品選擇[i][(int)enum_藥品選擇.包裝單位].ObjectToString();
                value[(int)enum_主畫面_藥單列表.操作時間] = DateTime.Now.ToDateTimeString_6();
                value[(int)enum_主畫面_藥單列表.開方時間] = DateTime.Now.ToDateTimeString();
                value[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待作業.GetEnumName();


                this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_AddRow(value, true);
            }
           
            cnt++;
            return;
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_檢查取藥堆疊(ref int cnt)
        {
            bool flag_ok = true;
            List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
            if (list_value.Count > 0)
            {
                for (int i = 0; i < list_value.Count; i++)
                {
                    if (list_value[i][(int)enum_主畫面_藥單列表.狀態].ObjectToString() != enum_主畫面_藥單列表_狀態.作業完成.GetEnumName())
                    {
                        flag_ok = false;
                    }
                }
            }
            if (flag_ok)
            {
                cnt++;
                return;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_至出貨位置開始(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                PLC_Device_移動至出貨位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_至出貨位置結束(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_輸送帶開始(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                PLC_Device_輸送帶.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_輸送帶結束(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_開取藥門開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                PLC_Device_取物門_移動到開門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_2000_領藥_開取藥門結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                cnt++;
            }
        }


        void cnt_Program_主畫面_領退藥_3000_退藥_藥品選擇(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            Dialog_退藥_藥品選擇 dialog_退藥_藥品選擇;
            this.Invoke(new Action(delegate
            {
                dialog_退藥_藥品選擇 = new Dialog_退藥_藥品選擇(MySerialPort_Scanner, this.Function_儲位管理_儲位資料_取得儲位資料(),this.sqL_DataGridView_入庫作業_藥品資料);
                dialogResult = dialog_退藥_藥品選擇.ShowDialog();
                主畫面_領退藥_退藥藥品 = dialog_退藥_藥品選擇.Value;
                主畫面_領退藥_退藥數量 = dialog_退藥_藥品選擇.數量;
            }));
          
            if (dialogResult == DialogResult.Yes)
            {
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }
           
        }
        void cnt_Program_主畫面_領退藥_3000_退藥_空瓶實瓶選擇(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            Dialog_空瓶實瓶選擇 dialog_空瓶實瓶選擇;
            this.Invoke(new Action(delegate
            {
                dialog_空瓶實瓶選擇 = new Dialog_空瓶實瓶選擇();
                dialogResult = dialog_空瓶實瓶選擇.ShowDialog();
                this.enum_空瓶實瓶選擇 = dialog_空瓶實瓶選擇.enum_空瓶實瓶選擇;
            }));

            if (dialogResult == DialogResult.Yes)
            {
                cnt++;
                return;
            }
            else
            {
                cnt = 65500;
                return;
            }

        }
        void cnt_Program_主畫面_領退藥_3000_退藥_等待退藥開鎖打開(ref int cnt)
        {
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Red, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"<請打開退藥抽屜>" + " \n";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_06.GetAdress(), 狀態顯示);
            PLC_Device_主畫面_領退藥_狀態顯示_06.Bool = true;
            if (!PLC_Device_退藥鎖.Bool)
            {
                PLC_Device_退藥鎖.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_3000_退藥_退藥開鎖打開結束(ref int cnt)
        {
      
            if (!PLC_Device_退藥鎖.Bool)
            {
                if (PLC_Device_退藥鎖_OK.Bool)
                {
                    cnt++;
                    return;
                }
                else
                {
                    this.
                    MyTimer_主畫面_領退藥_退藥鎖逾時.StartTickTime(3000);
                    cnt = 3990;
                    return;
                }
            }
        }
        void cnt_Program_主畫面_領退藥_3000_退藥_開啟退藥掃碼頁面(ref int cnt)
        {
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                Dialog_退藥_掃碼 dialog_退藥_掃碼 = new Dialog_退藥_掃碼(MySerialPort_Scanner, 主畫面_領退藥_退藥藥品, 主畫面_領退藥_退藥數量 , PLC_Device_退藥鎖_輸入);
                dialogResult = dialog_退藥_掃碼.ShowDialog();
            }));
            if(dialogResult == DialogResult.No)
            {
                cnt = 65500;
                return;
            }
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Red, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"<退藥完成>" + " \n";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_06.GetAdress(), 狀態顯示);

            cnt++;
        }
        void cnt_Program_主畫面_領退藥_3000_退藥_寫入交易紀錄(ref int cnt)
        {
            string 動作 = enum_空瓶實瓶選擇.GetEnumName();

            string 藥品碼 = 主畫面_領退藥_退藥藥品[(int)enum_入庫作業_藥品資料.藥品碼].ObjectToString();
            string 藥品名稱 = 主畫面_領退藥_退藥藥品[(int)enum_入庫作業_藥品資料.藥品名稱].ObjectToString();
            string 藥袋序號 = "";
            string 房名 = 主畫面_領退藥_手術房[(int)enum_藥檔資料_手術房設定_手術房列表.名稱].ObjectToString();
            string 庫存量 = 0.ToString();
            string 交易量 = 主畫面_領退藥_退藥數量.ToString();
            string 結存量 = 0.ToString();
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString_6();
            string 開方時間 = DateTime.Now.ToDateTimeString_6();
            string 操作人 = this.主畫面_登入者姓名;
            string 備註 = "";

            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.房名] = 房名;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.備註] = 備註;
            this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);
            cnt++;
        }
        /*--------------------------------------------------------------------------*/
        void cnt_Program_主畫面_領退藥_3990_退藥_開鎖逾時(ref int cnt)
        {
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Red, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"<開鎖逾時>" + " \n";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_06.GetAdress(), 狀態顯示);
            PLC_Device_主畫面_領退藥_狀態顯示_06.Bool = true;
            if(MyTimer_主畫面_領退藥_退藥鎖逾時.IsTimeOut())
            {
                cnt = 65500;
                return;
            }
        }


        //一般模式
        void cnt_Program_主畫面_領退藥_5000_領藥_等待掃碼(ref int cnt)
        {
            if(plC_RJ_Button_主畫面_開始作業.Bool)
            {
                this.PLC_Device_Scanner_讀取藥單資料.Bool = false;
                List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未有掃碼資料!");
                    cnt = 5000;
                    return;
                }
                for (int i = 0; i < list_value.Count; i++)
                {
                    list_value[i][(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待作業.GetEnumName();
                }
                this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(list_value, false);
                plC_RJ_Button_主畫面_開始作業.Bool = false;
                cnt = 5100;
                return;
            }

            if (!this.PLC_Device_Scanner_讀取藥單資料.Bool)
            {
                this.PLC_Device_Scanner_讀取藥單資料.Bool = true;
                cnt++;
            }

        }
        void cnt_Program_主畫面_領退藥_5000_領藥_掃碼完成(ref int cnt)
        {
            if (plC_RJ_Button_主畫面_開始作業.Bool)
            {
                this.PLC_Device_Scanner_讀取藥單資料.Bool = false;
                List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未有掃碼資料!");
                    cnt = 5000;
                    return;
                }
                for (int i = 0; i < list_value.Count; i++)
                {
                    list_value[i][(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待作業.GetEnumName();
                }
                this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(list_value, false);
                plC_RJ_Button_主畫面_開始作業.Bool = false;
                cnt = 5100;
                return;
            }

            if (!this.PLC_Device_Scanner_讀取藥單資料.Bool)
            {
                if (PLC_Device_Scanner_讀取藥單資料_OK.Bool)
                {
                    cnt++;
                }
                else
                {
                    cnt = 6000;
                }
            }
        }
        void cnt_Program_主畫面_領退藥_5000_領藥_檢查重複領藥(ref int cnt)
        {
            string 藥品碼 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥品碼].ObjectToString();
            string 病歷號 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.病歷號].ObjectToString();
            string 開方時間 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.開方時間].ObjectToString();
            string[] 領藥作業列表_serchcolName = new string[] { enum_主畫面_藥單列表.藥品碼.GetEnumName(), enum_主畫面_藥單列表.病歷號.GetEnumName(), enum_主畫面_藥單列表.開方時間.GetEnumName() };
            string[] 交易紀錄_serchcolName = new string[] { enum_交易記錄查詢資料.藥品碼.GetEnumName(), enum_交易記錄查詢資料.病歷號.GetEnumName(), enum_交易記錄查詢資料.開方時間.GetEnumName() };
            string[] serchValue = new string[] { 藥品碼 , 病歷號 , 開方時間 };
            List<object[]> list_領藥作業列表 = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetRows(領藥作業列表_serchcolName, serchValue, false);
            if (list_領藥作業列表.Count > 0)
            {
                MyMessageBox.ShowDialog("此藥單正在領取中!");
                cnt = 5000;
                return;
            }
            if(plC_Button_重複領藥不檢查.Bool == false)
            {
                List<object[]> list_交易紀錄 = this.sqL_DataGridView_交易記錄查詢.SQL_GetRows(交易紀錄_serchcolName, serchValue, false);
                if (list_交易紀錄.Count > 0)
                {
                    MyMessageBox.ShowDialog("此藥單已領取過!");
                    cnt = 5000;
                    return;
                }
            }
          
            cnt++;
        }
        void cnt_Program_主畫面_領退藥_5000_領藥_寫入取藥堆疊(ref int cnt)
        {
            object[] value = new object[new enum_主畫面_藥單列表().GetLength()];
            value[(int)enum_主畫面_藥單列表.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_主畫面_藥單列表.序號] = this.sqL_DataGridView_主畫面_領退藥作業列表.GetAllRows().Count + 1;
            value[(int)enum_主畫面_藥單列表.動作] = enum_交易記錄查詢動作.掃碼領藥.GetEnumName();
            value[(int)enum_主畫面_藥單列表.藥品碼] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥品碼].ObjectToString();
            value[(int)enum_主畫面_藥單列表.藥品名稱] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥品名稱].ObjectToString();
            value[(int)enum_主畫面_藥單列表.交易量] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.使用數量].ObjectToString().StringToInt32() * -1;
            value[(int)enum_主畫面_藥單列表.房號] = "";
            value[(int)enum_主畫面_藥單列表.病歷號] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.病歷號].ObjectToString();
            value[(int)enum_主畫面_藥單列表.病人姓名] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.病人姓名].ObjectToString();
            value[(int)enum_主畫面_藥單列表.藥袋序號] = "";
            value[(int)enum_主畫面_藥單列表.單位] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.包裝單位].ObjectToString();
            value[(int)enum_主畫面_藥單列表.操作時間] = DateTime.Now.ToDateTimeString_6();
            value[(int)enum_主畫面_藥單列表.開方時間] = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.開方時間].ObjectToString();
            value[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待開始作業.GetEnumName();


            this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_AddRow(value, true);
            cnt++;
        }

        void cnt_Program_主畫面_領退藥_5100_領藥_檢查取藥堆疊(ref int cnt)
        {
            bool flag_ok = true;
            List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
            if (list_value.Count > 0)
            {
                for (int i = 0; i < list_value.Count; i++)
                {
                    if (list_value[i][(int)enum_主畫面_藥單列表.狀態].ObjectToString() != enum_主畫面_藥單列表_狀態.作業完成.GetEnumName())
                    {
                        flag_ok = false;
                    }
                }
            }
            if (flag_ok)
            {
                cnt++;
                return;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_至出貨位置開始(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                PLC_Device_移動至出貨位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_至出貨位置結束(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_輸送帶開始(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                PLC_Device_輸送帶.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_輸送帶結束(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_開取藥門開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                PLC_Device_取物門_移動到開門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_5100_領藥_開取藥門結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                cnt++;
            }
        }




        void cnt_Program_主畫面_領退藥_6000_退藥_等待掃碼(ref int cnt)
        {
            if (!this.PLC_Device_Scanner_讀取藥單資料.Bool)
            {
                this.PLC_Device_Scanner_讀取藥單資料.Bool = true;
                cnt++;
            }
            
        }
        void cnt_Program_主畫面_領退藥_6000_退藥_掃碼完成(ref int cnt)
        {
            if (!this.PLC_Device_Scanner_讀取藥單資料.Bool)
            {
                if(PLC_Device_Scanner_讀取藥單資料_OK.Bool)
                {
                    cnt++;
                }
                else
                {
                    cnt = 6000;
                }
            }

        }
        void cnt_Program_主畫面_領退藥_6000_退藥_輸入退藥數量(ref int cnt)
        {
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate 
            {
                dialogResult = dialog_NumPannel.ShowDialog();
                
            }));
            if (dialogResult != DialogResult.Yes)
            {
                cnt = 6000;
                return;
            }
            主畫面_領退藥_退藥數量 = dialog_NumPannel.Value;
            cnt++;
        }
        void cnt_Program_主畫面_領退藥_6000_退藥_等待退藥開鎖打開(ref int cnt)
        {
            string 狀態顯示 = "";
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetAlignmentString(PLC_MultiStateDisplay.TextValue.Alignment.Left);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontColorString(Color.Red, true);
            狀態顯示 += this.plC_MultiStateDisplay_主畫面_狀態顯示.GetFontString(new Font("標楷體", 24F, FontStyle.Bold), true);
            狀態顯示 += $"<請打開退藥抽屜>" + " \n";
            this.plC_MultiStateDisplay_主畫面_狀態顯示.SetTextValue(PLC_Device_主畫面_領退藥_狀態顯示_06.GetAdress(), 狀態顯示);
            PLC_Device_主畫面_領退藥_狀態顯示_06.Bool = true;
            if (!PLC_Device_退藥鎖.Bool)
            {
                PLC_Device_退藥鎖.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_主畫面_領退藥_6000_退藥_退藥開鎖打開結束(ref int cnt)
        {

            if (!PLC_Device_退藥鎖.Bool)
            {
                if (PLC_Device_退藥鎖_OK.Bool)
                {
                    cnt++;
                    return;
                }
                else
                {
                    this.
                    MyTimer_主畫面_領退藥_退藥鎖逾時.StartTickTime(6000);
                    cnt = 3990;
                    return;
                }
            }
        }
        void cnt_Program_主畫面_領退藥_6000_退藥_寫入交易紀錄(ref int cnt)
        {
            string 動作 = enum_交易記錄查詢動作.手輸退藥.GetEnumName();
            string 藥品碼 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥品碼].ObjectToString();
            string 藥品名稱 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥品名稱].ObjectToString();
            string 藥袋序號 = Scanner_讀取藥單資料_Array[(int)enum_Scanner_陣列內容.藥袋序號].ObjectToString();
            string 房名 = "";
            string 庫存量 = 0.ToString();
            string 交易量 = 主畫面_領退藥_退藥數量.ToString();
            string 結存量 = 0.ToString();
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString_6();
            string 開方時間 = DateTime.Now.ToDateTimeString_6();
            string 操作人 = this.主畫面_登入者姓名;
            string 備註 = "";

            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.房名] = 房名;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.備註] = 備註;
            this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);
            cnt++;
        }
        #endregion
        #region PLC_主畫面_取藥堆疊檢查
        MyUI.MyTimer MyTimer_主畫面_取藥堆疊檢查延遲 = new MyUI.MyTimer();
        PLC_Device PLC_Device_主畫面_取藥堆疊檢查 = new PLC_Device("S1025");
        PLC_Device PLC_Device_主畫面_取藥堆疊檢查_OK = new PLC_Device("S1026");
        private object[] 主畫面_取藥堆疊檢查_當前作業內容;
        int cnt_Program_主畫面_取藥堆疊檢查 = 65534;
        void sub_Program_主畫面_取藥堆疊檢查()
        {
            if (this.plC_ScreenPage_Main.PageText == "主畫面")
            {
                PLC_Device_主畫面_取藥堆疊檢查.Bool = true;
            }
            else
            {
                PLC_Device_主畫面_取藥堆疊檢查.Bool = false;
            }
            if (cnt_Program_主畫面_取藥堆疊檢查 == 65534)
            {
                while (true)
                {
                    List<object[]> list_value = this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
                    if (list_value.Count == 0) break;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.SQL_DeleteExtra(list_value, true);
                }
                PLC_Device_主畫面_取藥堆疊檢查.SetComment("PLC_主畫面_取藥堆疊檢查");
                PLC_Device_主畫面_取藥堆疊檢查_OK.SetComment("PLC_主畫面_取藥堆疊檢查_OK");
                PLC_Device_主畫面_取藥堆疊檢查.Bool = false;
                cnt_Program_主畫面_取藥堆疊檢查 = 65535;
            }
            if (cnt_Program_主畫面_取藥堆疊檢查 == 65535) cnt_Program_主畫面_取藥堆疊檢查 = 1;
            if (cnt_Program_主畫面_取藥堆疊檢查 == 1) cnt_Program_主畫面_取藥堆疊檢查_檢查按下(ref cnt_Program_主畫面_取藥堆疊檢查);
            if (cnt_Program_主畫面_取藥堆疊檢查 == 2) cnt_Program_主畫面_取藥堆疊檢查_初始化(ref cnt_Program_主畫面_取藥堆疊檢查);
            if (cnt_Program_主畫面_取藥堆疊檢查 == 3) cnt_Program_主畫面_取藥堆疊檢查_取得資料(ref cnt_Program_主畫面_取藥堆疊檢查);
            if (cnt_Program_主畫面_取藥堆疊檢查 == 4) cnt_Program_主畫面_取藥堆疊檢查_開始作業(ref cnt_Program_主畫面_取藥堆疊檢查);
            if (cnt_Program_主畫面_取藥堆疊檢查 == 5) cnt_Program_主畫面_取藥堆疊檢查_寫入交易紀錄(ref cnt_Program_主畫面_取藥堆疊檢查);
            if (cnt_Program_主畫面_取藥堆疊檢查 == 6) cnt_Program_主畫面_取藥堆疊檢查 = 65500;
            if (cnt_Program_主畫面_取藥堆疊檢查 > 1) cnt_Program_主畫面_取藥堆疊檢查_檢查放開(ref cnt_Program_主畫面_取藥堆疊檢查);

            if (cnt_Program_主畫面_取藥堆疊檢查 == 65500)
            {
                PLC_Device_主畫面_取藥堆疊檢查.Bool = false;
                PLC_Device_主畫面_取藥堆疊檢查_OK.Bool = false;
                cnt_Program_主畫面_取藥堆疊檢查 = 65535;
            }
            if(PLC_Device_主畫面_取藥堆疊檢查.Bool)
            {
                MyTimer_主畫面_取藥堆疊檢查延遲.TickStop();
                MyTimer_主畫面_取藥堆疊檢查延遲.StartTickTime(100);
            }
        }
        void cnt_Program_主畫面_取藥堆疊檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_主畫面_取藥堆疊檢查.Bool) cnt++;
        }
        void cnt_Program_主畫面_取藥堆疊檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_主畫面_取藥堆疊檢查.Bool) cnt = 65500;
        }
        void cnt_Program_主畫面_取藥堆疊檢查_初始化(ref int cnt)
        {
            if(MyTimer_主畫面_取藥堆疊檢查延遲.IsTimeOut())
            {
                cnt++;
            }
        }
        void cnt_Program_主畫面_取藥堆疊檢查_取得資料(ref int cnt)
        {
            List<object[]> list_value = sqL_DataGridView_主畫面_領退藥作業列表.SQL_GetAllRows(false);
            if(list_value.Count == 0)
            {
                cnt = 65500;
                return;
            }
            for(int i = 0; i < list_value.Count; i++)
            {
                if (list_value[i][(int)enum_主畫面_藥單列表.狀態].ObjectToString() == enum_主畫面_藥單列表_狀態.等待作業.GetEnumName())
                {
                    主畫面_取藥堆疊檢查_當前作業內容 = list_value[i];
                    主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.作業中.GetEnumName();
                    sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(主畫面_取藥堆疊檢查_當前作業內容, true);
                    cnt++;
                    return;
                }
            }
            cnt = 65500;
            return;
      
        }
        void cnt_Program_主畫面_取藥堆疊檢查_開始作業(ref int cnt)
        {
            string Code = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.藥品碼].ObjectToString();
            int 交易量 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.交易量].ObjectToString().StringToInt32();
            List<object[]> list_儲位資料 = this.Function_儲位管理_儲位資料_取得儲位資料();
            List<object[]> list_儲位資料_buf = list_儲位資料.GetRows((int)enum_儲位管理_儲位資料.藥品碼, Code);
            if (list_儲位資料_buf.Count == 0)
            {
                主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.未搜尋到儲位.GetEnumName();
                voice.SpeakOnTask("未搜尋到儲位");
                sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(主畫面_取藥堆疊檢查_當前作業內容, true);
                cnt = 65500;
                return;
            }
            else
            {
                int 總庫存 = 0;
                for (int i = 0; i < list_儲位資料_buf.Count; i++)
                {
                    總庫存 += list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.總庫存].ObjectToString().StringToInt32();
                }
                if (總庫存 < 交易量 * -1)
                {
                    主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.庫存不足.GetEnumName();
                    voice.SpeakOnTask("庫存不足");
                    sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(主畫面_取藥堆疊檢查_當前作業內容, true);
                    cnt = 65500;
                    return;
                }
                List<int> list_儲位數組 = Function_主畫面_取得儲位數組(list_儲位資料_buf);
                List<int> list_取藥數組 = Function_數組找目標數值加總組合(list_儲位數組, 交易量 * -1);
                if (list_取藥數組.Count == 0)
                {
                    主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.無法找出組合.GetEnumName();
                    voice.SpeakOnTask("無法找出組合");
                    sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(主畫面_取藥堆疊檢查_當前作業內容, true);
                    cnt = 65500;
                    return;
                }
                for (int i = 0; i < list_取藥數組.Count; i++)
                {
                    bool flag_OK = this.Function_主畫面_儲位取藥作業(list_儲位資料_buf, list_取藥數組[i]);
                    if (!flag_OK)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MyMessageBox.ShowDialog("取藥作業失敗!");
                        }));
                        cnt = 65500;
                        return;
                    }
                }

                for (int i = 0; i < list_儲位資料_buf.Count; i++)
                {
                    string IP = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                    int Port = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.Port].ObjectToString().StringToInt32();
                    this.wT32_GPADC.Set_ScreenPageInit(IP, Port, false);
                    this.wT32_GPADC.Set_ToPage(IP, Port, (int)StorageUI_WT32.enum_Page.主頁面);
                    this.wT32_GPADC.Set_JsonStringSend(IP, Port);
                }
                主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.庫存量] = 總庫存;
                主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.結存量] = 總庫存 + 交易量;
                主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.作業完成.GetEnumName();
                sqL_DataGridView_主畫面_領退藥作業列表.SQL_ReplaceExtra(主畫面_取藥堆疊檢查_當前作業內容, true);
            }
            cnt++;
        }
        void cnt_Program_主畫面_取藥堆疊檢查_寫入交易紀錄(ref int cnt)
        {

            string 動作 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.動作].ObjectToString();
            string 藥品碼 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.藥品碼].ObjectToString();
            string 藥品名稱 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.動作].ObjectToString();
            string 藥袋序號 = "";
            string 房名 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.房號].ObjectToString();
            string 庫存量 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.庫存量].ObjectToString();
            string 交易量 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.交易量].ObjectToString();
            string 結存量 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.結存量].ObjectToString();
            string 病人姓名 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.病人姓名].ObjectToString();
            string 病歷號 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.病歷號].ObjectToString();
            string 操作時間 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.操作時間].ObjectToString();
            string 開方時間 = 主畫面_取藥堆疊檢查_當前作業內容[(int)enum_主畫面_藥單列表.開方時間].ObjectToString();
            string 操作人 = this.主畫面_登入者姓名;
            string 備註 = "";

            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.房名] = 房名;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.備註] = 備註;
            this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);

            cnt++;
        }

        #endregion

        #region Functionn
        private bool Function_主畫面_儲位取藥作業(List<object[]> list_儲位資料, int 目標包裝數量)
        {
            int cnt = 0;
            object[] 儲位資料 = new object[new enum_儲位管理_儲位資料().GetLength()];
            int 層數 = 0;
            int 格數 = 0;
            while(true)
            {
                if(cnt  == 0)
                {
                    bool flag_OK = false;
                    for(int i = 0; i < list_儲位資料.Count; i++)
                    {
                        int 最小包裝數量 = list_儲位資料[i][(int)enum_儲位管理_儲位資料.單位包裝數量].ObjectToString().StringToInt32();
                        int 庫存 = list_儲位資料[i][(int)enum_儲位管理_儲位資料.庫存].ObjectToString().StringToInt32();
                        if (目標包裝數量 == 最小包裝數量)
                        {
                            if(庫存 > 0)
                            {
                                儲位資料 = list_儲位資料[i];
                                cnt++;
                                flag_OK = true;
                                break;
                            }
                        }
                    }
                    if (!flag_OK) break;
                }
                if(cnt == 1)
                {
                    Function_儲位管理_儲位資料_取得儲位層數及格數(儲位資料, ref 層數, ref 格數);
                    if (層數 - 1 < 0)
                    {
                        cnt = 65500;
                        return false;
                    }
                    if (格數 - 1 < 0)
                    {
                        cnt = 65500;
                        return false;
                    }
                    Console.WriteLine($"儲位 層數:{層數 - 1} , 格數{格數 - 1}");
                    cnt++;
                }
                if (cnt == 2)
                {
                  

                    string IP = 儲位資料[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                    int Port = 儲位資料[(int)enum_儲位管理_儲位資料.Port].ObjectToString().StringToInt32();
                    int Pannel_Width = WT32_GPADC.Pannel_Width;
                    int Pannel_Height = WT32_GPADC.Pannel_Height;
                    string Text;
                    Font font;
                    Color font_color;
                    Color back_color;
                    int border_size;
                    Color border_color;
                    int width;
                    int height;
                    int poX;
                    int poY;
                    bool flag_screen_refresh = true;
                    try
                    {
                        string jsonString = this.storageUI_WT32.GetUDPJsonString(IP);
                        StorageUI_WT32.UDP_READ uDP_READ = jsonString.JsonDeserializet<StorageUI_WT32.UDP_READ>();
                        if(uDP_READ.Screen_Page == (int)StorageUI_WT32.enum_Page.取藥中頁面)
                        {
                            flag_screen_refresh = false;
                        }
                    }
                    catch
                    {

                    }
                

                    if(flag_screen_refresh)
                    {
                        this.wT32_GPADC.Set_ToPage(IP, Port, (int)StorageUI_WT32.enum_Page.取藥中頁面);
                        this.wT32_GPADC.Set_JsonStringSend(IP, Port);
                        this.wT32_GPADC.Set_ScreenPageInit(IP, Port, false);
                        Text = "取藥中";
                        font = new Font("標楷體", 45, FontStyle.Bold);
                        width = 320;
                        height = 200;
                        poX = (WT32_GPADC.Pannel_Width - width) / 2;
                        poY = (WT32_GPADC.Pannel_Height - height) / 2;
                        font_color = Color.White;
                        back_color = Color.Red;
                        border_size = 2;
                        border_color = Color.RoyalBlue;
                        this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Center);

                    }
                    cnt++;
                }
                if (cnt == 3)
                {
                    PLC_Device_XY_Table_移動_層數.Value = 層數 - 1;
                    PLC_Device_XY_Table_移動_格數.Value = 格數 - 1;

  
                    if (!PLC_Device_XY_Table_移動.Bool)
                    {
                        Console.WriteLine($"XY Table開始移動...");
                        PLC_Device_XY_Table_移動.Bool = true;
                        cnt++;
                    }
            
                }
                if (cnt == 4)
                {

                    if (!PLC_Device_XY_Table_移動.Bool)
                    {
                        Console.WriteLine($"XY Table移動完成...");
                 
                        cnt++;
                    }
                }
                if (cnt == 5)
                {
                    PLC_Device_送料馬達出料_層數.Value = 層數 - 1;
                    PLC_Device_送料馬達出料_格數.Value = 格數 - 1;
                    if (!PLC_Device_送料馬達出料.Bool)
                    {
                        Console.WriteLine($"馬達開始出料...");
                        PLC_Device_送料馬達出料.Bool = true;
                        cnt++;
                    }
                }
                if (cnt == 6)
                {
                    if (!PLC_Device_送料馬達出料.Bool)
                    {
                        Console.WriteLine($"馬達出料完成...");
                        MyTimer_主畫面_領退藥_馬達出料延遲.TickStop();
                        MyTimer_主畫面_領退藥_馬達出料延遲.StartTickTime(500);
                        cnt++;
                    }
                }
                if (cnt == 7)
                {
                    this.Function_儲位管理_儲位資料_儲位資料庫存異動(儲位資料, -1);
                  
        
                    cnt++;
                }
                if(cnt == 8)
                {
                    if(MyTimer_主畫面_領退藥_馬達出料延遲.IsTimeOut())
                    {
                        cnt = 65500;
                        return true;
                    }
                   
                }


                
                System.Threading.Thread.Sleep(10);
            }
            return false;
        }
        private List<int> Function_主畫面_取得儲位數組(List<object[]> list_value)
        {
            List<int> list_數組 = new List<int>();
            for(int i = 0; i < list_value.Count; i++)
            {
                int 庫存 = list_value[i][(int)enum_儲位管理_儲位資料.庫存].ObjectToString().StringToInt32();
                int 最小包裝數量 = list_value[i][(int)enum_儲位管理_儲位資料.單位包裝數量].ObjectToString().StringToInt32();
                for (int k = 0; k < 庫存; k++)
                {
                    list_數組.Add(最小包裝數量);
                }
            }

            return list_數組;
        }
        private void Function_主畫面_新增藥單列表(string 藥品碼, string 交易量, string 藥袋序號, string 病人姓名, string 病歷號, DateTime 開方時間)
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetRows(enum_參數設定_藥檔資料.藥品碼.GetEnumName(), 藥品碼, false);
            if (list_藥品資料.Count == 0) return;
            if (Function_主畫面_檢查重複領取(藥品碼, 藥袋序號, 病歷號, 開方時間))
            {
                return;
            }
            object[] value = new object[new enum_主畫面_藥單列表().GetEnumNames().Length];
            value[(int)enum_主畫面_藥單列表.序號] = sqL_DataGridView_主畫面_領退藥作業列表.GetAllRows().Count + 1;
            value[(int)enum_主畫面_藥單列表.藥品碼] = 藥品碼;
            value[(int)enum_主畫面_藥單列表.藥品名稱] = list_藥品資料[0][(int)enum_主畫面_藥單列表.藥品名稱].ObjectToString();
            value[(int)enum_主畫面_藥單列表.交易量] = 交易量;
            value[(int)enum_主畫面_藥單列表.藥袋序號] = 藥袋序號;
            value[(int)enum_主畫面_藥單列表.病人姓名] = 病人姓名;
            value[(int)enum_主畫面_藥單列表.病歷號] = 病歷號;
            value[(int)enum_主畫面_藥單列表.開方時間] = 開方時間.ToDateTimeString();
            value[(int)enum_主畫面_藥單列表.操作時間] = DateTime.Now.ToDateTimeString_6();
            value[(int)enum_主畫面_藥單列表.狀態] = enum_主畫面_藥單列表_狀態.等待作業.GetEnumName();

            sqL_DataGridView_主畫面_領退藥作業列表.AddRow(value, true);
        }
        private bool Function_主畫面_檢查重複領取(string 藥品碼, string 藥袋序號, string 病歷號, DateTime 開方時間)
        {
            string[] serchvalue = new string[] { 藥品碼, 藥袋序號, 病歷號, 開方時間.ToDateTimeString() };
            string[] serchcolname = new string[] { enum_交易記錄查詢資料.藥品碼.GetEnumName(), enum_交易記錄查詢資料.藥袋序號.GetEnumName() , enum_交易記錄查詢資料.病歷號.GetEnumName() , enum_交易記錄查詢資料.開方時間.GetEnumName()};
            List<object[]> list_value = sqL_DataGridView_交易記錄查詢.SQL_GetRows(serchcolname, serchvalue, false);
            if (list_value.Count > 0) return true;
            list_value = sqL_DataGridView_主畫面_領退藥作業列表.GetRows(serchcolname, serchvalue, false);
            if (list_value.Count > 0) return true;
            return false;
        }
        #endregion
        #region Event
        private void SqL_DataGridView_主畫面_領退藥作業列表_DataGridRefreshEvent()
        {
            string 狀態 = "";
            for (int i = 0; i < this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows.Count; i++)
            {
                狀態 = this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].Cells[(int)enum_主畫面_藥單列表.狀態].Value.ToString();
                if (狀態 == enum_主畫面_藥單列表_狀態.等待作業.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_主畫面_藥單列表_狀態.作業中.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_主畫面_藥單列表_狀態.庫存不足.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_主畫面_藥單列表_狀態.無法找出組合.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_主畫面_藥單列表_狀態.未搜尋到儲位.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                if (狀態 == enum_主畫面_藥單列表_狀態.作業完成.GetEnumName())
                {
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                    this.sqL_DataGridView_主畫面_領退藥作業列表.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void plC_RJ_Button_主畫面_登入_MouseDownEvent(MouseEventArgs mevent)
        {
            if (plC_RJ_Button_主畫面_登入.Text == "登出")
            {
                this.MyTimer_主畫面_領退藥_重複登入延遲.TickStop();
                this.MyTimer_主畫面_領退藥_重複登入延遲.StartTickTime(100);
                PLC_Device_取物門_移動到關門位置.Bool = true;
                PLC_Device_主畫面_領退藥.Bool = false;
                cnt_Program_主畫面_領退藥 = 65501;
            }
            PLC_Device_主畫面_領退藥_登入按下.Bool = true;
        }
        #endregion
    }
}
