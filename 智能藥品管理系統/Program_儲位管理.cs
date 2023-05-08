using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
using Basic;
using H_Pannel_lib;
namespace 智能藥品管理系統
{
    public enum enum_儲位管理_儲位資料
    {
        IP,
        Port,
        儲位名稱,
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        包裝單位,
        庫存,
        單位包裝數量,
        可放置盒數,
        可放置總藥量,
        總庫存,
        位置,
    }
    public partial class Form1 : Form
    {

    
        public class ICP_儲位管理_儲位資料 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                string IP_1 = y[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
       
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
        private Storage storage_儲位管理_儲位資料_複製格式;
        private MyThread MyThread_儲位管理_檢查主畫面;
        private MyThread MyThread_儲位管理_檢查IO;

        private void Program_儲位管理_Init()
        {
            this.sqL_DataGridView_儲位管理_參數設定.Init();
            if (!this.sqL_DataGridView_儲位管理_參數設定.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_儲位管理_參數設定.SQL_CreateTable();
            }
            this.sqL_DataGridView_儲位管理_套餐資料.Init(this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表);
            this.sqL_DataGridView_儲位管理_套餐資料.Set_ColumnVisible(false, new enum_藥檔資料_套餐設定_套餐列表().GetEnumNames());
            this.sqL_DataGridView_儲位管理_套餐資料.Set_ColumnVisible(true, enum_藥檔資料_套餐設定_套餐列表.套餐代碼, enum_藥檔資料_套餐設定_套餐列表.套餐名稱);
            this.sqL_DataGridView_儲位管理_套餐資料.Set_ColumnWidth(200, enum_藥檔資料_套餐設定_套餐列表.套餐代碼);
            this.sqL_DataGridView_儲位管理_套餐資料.Set_ColumnWidth(200, enum_藥檔資料_套餐設定_套餐列表.套餐名稱);

            this.sqL_DataGridView_儲位管理_參數設定.RowDoubleClickEvent += SqL_DataGridView_儲位管理_參數設定_RowDoubleClickEvent;
            this.sqL_DataGridView_儲位管理_儲位資料.Init();
            this.sqL_DataGridView_儲位管理_儲位資料.DataGridRefreshEvent += SqL_DataGridView_儲位管理_儲位資料_DataGridRefreshEvent;
            this.sqL_DataGridView_儲位管理_儲位資料.RowEnterEvent += SqL_DataGridView_儲位管理_儲位資料_RowEnterEvent;

            this.MyThread_儲位管理_檢查主畫面 = new MyThread();
            this.MyThread_儲位管理_檢查主畫面.Add_Method(sub_Program_儲位管理_檢查主畫面);
            this.MyThread_儲位管理_檢查主畫面.AutoRun(true);
            this.MyThread_儲位管理_檢查主畫面.SetSleepTime(100);
            this.MyThread_儲位管理_檢查主畫面.Trigger();

            this.MyThread_儲位管理_檢查IO = new MyThread();
            this.MyThread_儲位管理_檢查IO.Add_Method(sub_Program_儲位管理_檢查IO);
            this.MyThread_儲位管理_檢查IO.AutoRun(true);
            this.MyThread_儲位管理_檢查IO.SetSleepTime(100);
            this.MyThread_儲位管理_檢查IO.Trigger();

            this.rJ_TextBox_儲位管理_參數設定_藥品條碼.KeyPress += RJ_TextBox_儲位管理_參數設定_藥品條碼_KeyPress;
            this.plC_RJ_Button_儲位管理_套餐資料_填入儲位.MouseDownEvent += PlC_RJ_Button_儲位管理_套餐資料_填入儲位_MouseDownEvent;
            this.plC_RJ_Button_儲位管理_儲位資料_新增效期測試.MouseDownEvent += PlC_RJ_Button_儲位管理_儲位資料_新增效期測試_MouseDownEvent;

        }

   

        private bool flag_Program_儲位管理 = false;
        private void Program_儲位管理()
        {
            if(this.plC_ScreenPage_Main.PageText == "儲位管理")
            {
                string readline = this.MySerialPort_Scanner.ReadString();
                if (!readline.StringIsEmpty())
                {
                    this.Invoke(new Action(delegate
                    {
                        if (rJ_TextBox_儲位管理_參數設定_藥品條碼.IsFocused)
                        {
                            readline = readline.Replace("\n", "");
                            readline = readline.Replace("\r", "");
                            rJ_TextBox_儲位管理_參數設定_藥品條碼.Texts = readline;
                            this.RJ_TextBox_儲位管理_參數設定_藥品條碼_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                        }

                        this.MySerialPort_Scanner.ClearReadByte();
                    }));
                }

                if (!flag_Program_儲位管理)
                {
                    this.MySerialPort_Scanner.ClearReadByte();
                    this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());
                    this.sqL_DataGridView_儲位管理_套餐資料.SQL_GetAllRows(true);
                    flag_Program_儲位管理 = true;
                }
            }
            else
            {
                flag_Program_儲位管理 = false;
            }

            sub_Program_儲位管理_更新WT32UI();
        }

        #region PLC_儲位管理_更新WT32UI
        PLC_Device PLC_Device_儲位管理_更新WT32UI = new PLC_Device("S6005");
        PLC_Device PLC_Device_儲位管理_更新WT32UI_OK = new PLC_Device("S6006");
        int cnt_Program_儲位管理_更新WT32UI = 65534;
        void sub_Program_儲位管理_更新WT32UI()
        {
            if (this.plC_ScreenPage_Main.PageText == "儲位管理")
            {
                PLC_Device_儲位管理_更新WT32UI.Bool = true;
            }
            else
            {
                PLC_Device_儲位管理_更新WT32UI.Bool = false;
            }
            if (cnt_Program_儲位管理_更新WT32UI == 65534)
            {
                PLC_Device_儲位管理_更新WT32UI.SetComment("PLC_儲位管理_更新WT32UI");
                PLC_Device_儲位管理_更新WT32UI_OK.SetComment("PLC_儲位管理_更新WT32UI_OK");
                PLC_Device_儲位管理_更新WT32UI.Bool = false;
                cnt_Program_儲位管理_更新WT32UI = 65535;
            }
            if (cnt_Program_儲位管理_更新WT32UI == 65535) cnt_Program_儲位管理_更新WT32UI = 1;
            if (cnt_Program_儲位管理_更新WT32UI == 1) cnt_Program_儲位管理_更新WT32UI_檢查按下(ref cnt_Program_儲位管理_更新WT32UI);
            if (cnt_Program_儲位管理_更新WT32UI == 2) cnt_Program_儲位管理_更新WT32UI_初始化(ref cnt_Program_儲位管理_更新WT32UI);
            if (cnt_Program_儲位管理_更新WT32UI == 3) cnt_Program_儲位管理_更新WT32UI = 65500;
            if (cnt_Program_儲位管理_更新WT32UI > 1) cnt_Program_儲位管理_更新WT32UI_檢查放開(ref cnt_Program_儲位管理_更新WT32UI);

            if (cnt_Program_儲位管理_更新WT32UI == 65500)
            {
                PLC_Device_儲位管理_更新WT32UI.Bool = false;
                PLC_Device_儲位管理_更新WT32UI_OK.Bool = false;
                cnt_Program_儲位管理_更新WT32UI = 65535;
            }
        }
        void cnt_Program_儲位管理_更新WT32UI_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位管理_更新WT32UI.Bool) cnt++;
        }
        void cnt_Program_儲位管理_更新WT32UI_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位管理_更新WT32UI.Bool) cnt = 65500;
        }
        void cnt_Program_儲位管理_更新WT32UI_初始化(ref int cnt)
        {
            List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
            if (list_儲位資料 == null)
            {
                cnt = 65500;
                return;
            }
            if (list_儲位資料.Count == 0)
            {
                cnt = 65500;
                return;
            }
            if (list_儲位資料[0][(int)enum_儲位管理_儲位資料.IP] == null)
            {
                cnt = 65500;
                return;
            }
            string IP = list_儲位資料[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            string JsonString = this.storageUI_WT32.GetUDPJsonString(IP);
            if(!JsonString.StringIsEmpty())this.wT32_GPADC.Set_JSON_String(JsonString);
            cnt++;
        }




        #endregion
        #region PLC_儲位管理_檢查主畫面
        PLC_Device PLC_Device_儲位管理_檢查主畫面 = new PLC_Device("S6025");
        PLC_Device PLC_Device_儲位管理_檢查主畫面_OK = new PLC_Device("S6026");
        int cnt_Program_儲位管理_檢查主畫面 = 65534;
        void sub_Program_儲位管理_檢查主畫面()
        {
            this.PLC_Device_儲位管理_檢查主畫面.Bool = true;
            if (cnt_Program_儲位管理_檢查主畫面 == 65534)
            {
                PLC_Device_儲位管理_檢查主畫面.SetComment("PLC_儲位管理_檢查主畫面");
                PLC_Device_儲位管理_檢查主畫面_OK.SetComment("PLC_儲位管理_檢查主畫面_OK");
                PLC_Device_儲位管理_檢查主畫面.Bool = false;
                cnt_Program_儲位管理_檢查主畫面 = 65535;
            }
            if (cnt_Program_儲位管理_檢查主畫面 == 65535) cnt_Program_儲位管理_檢查主畫面 = 1;
            if (cnt_Program_儲位管理_檢查主畫面 == 1) cnt_Program_儲位管理_檢查主畫面_檢查按下(ref cnt_Program_儲位管理_檢查主畫面);
            if (cnt_Program_儲位管理_檢查主畫面 == 2) cnt_Program_儲位管理_檢查主畫面_初始化(ref cnt_Program_儲位管理_檢查主畫面);
            if (cnt_Program_儲位管理_檢查主畫面 == 3) cnt_Program_儲位管理_檢查主畫面 = 65500;
            if (cnt_Program_儲位管理_檢查主畫面 > 1) cnt_Program_儲位管理_檢查主畫面_檢查放開(ref cnt_Program_儲位管理_檢查主畫面);

            if (cnt_Program_儲位管理_檢查主畫面 == 65500)
            {
                PLC_Device_儲位管理_檢查主畫面.Bool = false;
                PLC_Device_儲位管理_檢查主畫面_OK.Bool = false;
                cnt_Program_儲位管理_檢查主畫面 = 65535;
            }
        }
        void cnt_Program_儲位管理_檢查主畫面_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位管理_檢查主畫面.Bool) cnt++;
        }
        void cnt_Program_儲位管理_檢查主畫面_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位管理_檢查主畫面.Bool) cnt = 65500;
        }
        void cnt_Program_儲位管理_檢查主畫面_初始化(ref int cnt)
        {
            try
            {
                List<string> list_Jsonstring = this.storageUI_WT32.GetAllUDPJsonString();
                List<string> list_IP = new List<string>();
                List<StorageUI_WT32.UDP_READ> list_uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(list_Jsonstring);

                for (int i = 0; i < list_uDP_READ.Count; i++)
                {
                    if (list_uDP_READ[i].ScreenPage_Init == true && list_uDP_READ[i].Screen_Page == 10)
                    {
                        list_IP.Add(list_uDP_READ[i].IP);
                    }
                }
                if (list_IP.Count > 0)
                {
                    string IP = "";
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < list_IP.Count; i++)
                    {
                        IP = list_IP[i];
                        Storage storage = this.storageUI_WT32.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        taskList.Add(Task.Run(() =>
                        {
                            this.storageUI_WT32.Set_DrawPannelJEPG(storage);
                            this.wT32_GPADC.Set_JsonStringSend(storage);
                        }));
                    }
                    Task.WhenAll(taskList.ToArray()).Wait();

                }
            }
            catch
            { }

            



            cnt++;
        }





        #endregion
        #region PLC_儲位管理_檢查IO
        PLC_Device PLC_Device_儲位管理_檢查IO = new PLC_Device("S6045");
        PLC_Device PLC_Device_儲位管理_檢查IO_OK = new PLC_Device("S6046");
        PLC_Device PLC_Device_儲位管理_檢查IO_M8013 = new PLC_Device("M8013");
        int cnt_Program_儲位管理_檢查IO = 65534;
        void sub_Program_儲位管理_檢查IO()
        {
            this.PLC_Device_儲位管理_檢查IO.Bool = true;
            if (cnt_Program_儲位管理_檢查IO == 65534)
            {
                PLC_Device_儲位管理_檢查IO.SetComment("PLC_儲位管理_檢查IO");
                PLC_Device_儲位管理_檢查IO_OK.SetComment("PLC_儲位管理_檢查IO_OK");
                PLC_Device_儲位管理_檢查IO.Bool = false;
                cnt_Program_儲位管理_檢查IO = 65535;
            }
            if (cnt_Program_儲位管理_檢查IO == 65535) cnt_Program_儲位管理_檢查IO = 1;
            if (cnt_Program_儲位管理_檢查IO == 1) cnt_Program_儲位管理_檢查IO_檢查按下(ref cnt_Program_儲位管理_檢查IO);
            if (cnt_Program_儲位管理_檢查IO == 2) cnt_Program_儲位管理_檢查IO_初始化(ref cnt_Program_儲位管理_檢查IO);
            if (cnt_Program_儲位管理_檢查IO == 3) cnt_Program_儲位管理_檢查IO = 65500;
            if (cnt_Program_儲位管理_檢查IO > 1) cnt_Program_儲位管理_檢查IO_檢查放開(ref cnt_Program_儲位管理_檢查IO);

            if (cnt_Program_儲位管理_檢查IO == 65500)
            {
                PLC_Device_儲位管理_檢查IO.Bool = false;
                PLC_Device_儲位管理_檢查IO_OK.Bool = false;
                cnt_Program_儲位管理_檢查IO = 65535;
            }
        }
        void cnt_Program_儲位管理_檢查IO_檢查按下(ref int cnt)
        {
            if (PLC_Device_儲位管理_檢查IO.Bool) cnt++;
        }
        void cnt_Program_儲位管理_檢查IO_檢查放開(ref int cnt)
        {
            if (!PLC_Device_儲位管理_檢查IO.Bool) cnt = 65500;
        }
        void cnt_Program_儲位管理_檢查IO_初始化(ref int cnt)
        {
            List<string> list_Jsonstring = this.storageUI_WT32.GetAllUDPJsonString();
            List<StorageUI_WT32.UDP_READ> list_uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(list_Jsonstring);
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_uDP_READ.Count; i++)
            {
                StorageUI_WT32.UDP_READ value = list_uDP_READ[i];
                taskList.Add(Task.Run(() =>
                {
                    if (value.Screen_Page == (int)StorageUI_WT32.enum_Page.解鎖頁面)
                    {
                        if (value.INPUT_LOCK == 1)
                        {
                            if (this.PLC_Device_儲位管理_檢查IO_M8013.Bool)
                            {
                                if (value.OUTPUT_LED_GREEN == 0)
                                {
                                    this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, true);
                                    this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                                }
                            }
                            else
                            {
                                if (value.OUTPUT_LED_GREEN == 1)
                                {
                                    this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, false);
                                    this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                                }
                            }
                            if (value.OUTPUT_LED_RED == 1)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, false);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                        }
                        else
                        {
                            if (this.PLC_Device_儲位管理_檢查IO_M8013.Bool)
                            {
                                if (value.OUTPUT_LED_RED == 0)
                                {
                                    this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, true);
                                    this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                                }
                            }
                            else
                            {
                                if (value.OUTPUT_LED_RED == 1)
                                {
                                    this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, false);
                                    this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                                }
                            }
                            if (value.OUTPUT_LED_GREEN == 1)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, false);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                        }
                    }
                    else
                    {
                        if (value.INPUT_LOCK == 1)
                        {
                            if (value.OUTPUT_LED_GREEN == 0)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, true);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                            if (value.OUTPUT_LED_RED == 1)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, false);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                        }
                        else
                        {
                            if (value.OUTPUT_LED_GREEN == 1)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, false);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                            if (value.OUTPUT_LED_RED == 0)
                            {
                                this.wT32_GPADC.Set_LED(value.IP, value.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, true);
                                this.wT32_GPADC.Set_JsonStringSend(value.IP, value.Port);
                            }
                        }
                    }
                   

                }));
            }
            try
            {
                Task.WhenAll(taskList.ToArray()).Wait();
            }
            catch
            {

            }
            

            cnt++;
        }
        #endregion

        #region Function
        private void Function_儲位管理_儲位資料_儲位資料庫存異動(object[] value, int 異動量)
        {
            string IP = value[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            Storage storage = this.Function_儲位管理_從SQL取得Storage(IP);
            List<string> list_效期 = new List<string>();
            List<string> list_異動量 = new List<string>();
            storage.庫存異動(異動量 , out list_效期,out list_異動量);
            this.storageUI_WT32.SQL_ReplaceStorage(storage);
        }
        private void Function_儲位管理_儲位資料_取得儲位層數及格數(object[] value, ref int 層數, ref int 格數)
        {
            層數 = -1;
            格數 = -1;
            string 位置 = value[(int)enum_儲位管理_儲位資料.位置].ObjectToString();
            string[] array = myConvert.分解分隔號字串(位置, '-');
            if(array.Length == 2)
            {
                層數 = array[0].StringToInt32();
                格數 = array[1].StringToInt32();
            }
        }
        private List<object[]> Function_儲位管理_儲位資料_取得儲位資料()
        {
            return this.Function_儲位管理_儲位資料_取得儲位資料(true);
        }
        private List<object[]> Function_儲位管理_儲位資料_取得儲位資料(bool UpdateToSQL)
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_儲位管理_參數設定.SQL_GetAllRows(false);
            List<object[]> list_套餐資料 = this.sqL_DataGridView_儲位管理_套餐資料.SQL_GetAllRows(false);
            List<Storage> list_StorageTable = this.storageUI_WT32.SQL_GetAllStorage();
            List<object[]> list_value = new List<object[]>();
            List<Storage> list_Replace_Storage = new List<Storage>();
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_套餐資料_buf = new List<object[]>();
            string IP = "";
            string 儲位名稱 = "";
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥品中文名稱 = "";
            string 包裝單位 = "";
            string 庫存 = "";
            string 單位包裝數量 = "";
            string 總庫存 = "";
            string 位置 = "";

            string 藥品碼_Title = "";
            string 儲位名稱_Title = "";
            string 藥品中文名稱_Title = "";
            string 藥品名稱_Title = "";
            for (int i = 0; i < list_StorageTable.Count; i++)
            {
                object[] value = new object[new enum_儲位管理_儲位資料().GetLength()];
                bool flag_replace = false;
                Storage storage = list_StorageTable[i];

                IP = storage.GetValue(Storage.ValueName.IP, Storage.ValueType.Value).ObjectToString();
                儲位名稱 = storage.GetValue(Storage.ValueName.儲位名稱, Storage.ValueType.Value).ObjectToString();
                藥品碼 = storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value).ObjectToString();
                藥品名稱 = storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Value).ObjectToString(); ;
                藥品中文名稱 = storage.GetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.Value).ObjectToString(); ;
                包裝單位 = storage.GetValue(Storage.ValueName.包裝單位, Storage.ValueType.Value).ObjectToString(); ;
                單位包裝數量 = storage.GetValue(Storage.ValueName.最小包裝單位數量, Storage.ValueType.Value).ObjectToString();
                庫存 = storage.GetValue(Storage.ValueName.庫存, Storage.ValueType.Value).ObjectToString();

                藥品碼_Title = storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.Title).ObjectToString();
                儲位名稱_Title = storage.GetValue(Storage.ValueName.儲位名稱, Storage.ValueType.Title).ObjectToString();
                藥品中文名稱_Title = storage.GetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.Title).ObjectToString();
                藥品名稱_Title = storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Title).ObjectToString();

                if (藥品碼_Title != "藥碼")
                {
                    storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Title, "藥碼");
                    flag_replace = true;
                }
                if (儲位名稱_Title != "儲位")
                {
                    storage.SetValue(Storage.ValueName.儲位名稱, Storage.ValueType.Title, "儲位");
                    flag_replace = true;
                }
                if (藥品名稱_Title != "None")
                {
                    storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Title, "None");
                    flag_replace = true;
                }
                if (藥品中文名稱_Title != "None")
                {
                    storage.SetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.Title, "None");
                    flag_replace = true;
                }
                if (!藥品碼.StringIsEmpty())
                {
                    list_藥品資料_buf = (from Value in list_藥品資料
                                     where Value[(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString() == 藥品碼
                                     select Value).ToList();

                    list_套餐資料_buf = (from Value in list_套餐資料
                                     where Value[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString() == 藥品碼
                                     select Value).ToList();

                    if (list_藥品資料_buf.Count > 0)
                    {
                        if (藥品名稱 != list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString()) flag_replace = true;
                        if (藥品中文名稱 != list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString()) flag_replace = true;
                        if (包裝單位 != list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString()) flag_replace = true;

                        藥品名稱 = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
                        藥品中文名稱 = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
                        包裝單位 = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
                    }
                    else if (list_套餐資料_buf.Count > 0)
                    {
                        if (藥品名稱 != list_套餐資料_buf[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱].ObjectToString()) flag_replace = true;
                        if (藥品中文名稱 != "") flag_replace = true;
                        if (包裝單位 != "Package") flag_replace = true;

                        藥品名稱 = list_套餐資料_buf[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱].ObjectToString();
                        藥品中文名稱 = "";
                        包裝單位 = "Package";
                    }
                    else
                    {
                        藥品碼 = "";
                        藥品名稱 = "";
                        藥品中文名稱 = "";
                        包裝單位 = "";
                        storage.Clear();
                        flag_replace = true;
                    }
                }
                else
                {
                    藥品名稱 = "";
                    藥品中文名稱 = "";
                    包裝單位 = "";
                    flag_replace = true;
                }


                storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Value, 藥品名稱);
                storage.SetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.Value, 藥品中文名稱);
                storage.SetValue(Storage.ValueName.包裝單位, Storage.ValueType.Value, 包裝單位);

                if (storage.Max_Inventory <= 0) storage.Max_Inventory = 1;

                if (單位包裝數量.StringToInt32() < 0)
                {
                    flag_replace = true;
                    單位包裝數量 = "1";
                    storage.SetValue(Storage.ValueName.最小包裝單位數量, Storage.ValueType.Value, 單位包裝數量);
                }
                if (庫存.StringToInt32() < 0)
                {
                    flag_replace = true;
                    庫存 = "0";
                    storage.SetValue(Storage.ValueName.庫存, Storage.ValueType.Value, 庫存);
                }
                總庫存 = (單位包裝數量.StringToInt32() * 庫存.StringToInt32()).ToString();

                value[(int)enum_儲位管理_儲位資料.IP] = storage.IP;
                value[(int)enum_儲位管理_儲位資料.Port] = storage.Port;
                value[(int)enum_儲位管理_儲位資料.儲位名稱] = 儲位名稱;
                value[(int)enum_儲位管理_儲位資料.藥品碼] = 藥品碼;
                value[(int)enum_儲位管理_儲位資料.藥品名稱] = 藥品名稱;
                value[(int)enum_儲位管理_儲位資料.藥品中文名稱] = 藥品中文名稱;
                value[(int)enum_儲位管理_儲位資料.包裝單位] = 包裝單位;
                value[(int)enum_儲位管理_儲位資料.單位包裝數量] = 單位包裝數量;
                value[(int)enum_儲位管理_儲位資料.可放置盒數] = storage.Max_Inventory;
                value[(int)enum_儲位管理_儲位資料.可放置總藥量] = storage.Max_Inventory * (單位包裝數量.StringToInt32());
                value[(int)enum_儲位管理_儲位資料.庫存] = 庫存;
                value[(int)enum_儲位管理_儲位資料.總庫存] = 總庫存;
                value[(int)enum_儲位管理_儲位資料.位置] = 位置;
                if (flag_replace) list_Replace_Storage.Add(storage);
                list_value.Add(value);
            }
            if (UpdateToSQL) this.storageUI_WT32.SQL_ReplaceStorage(list_Replace_Storage);
            list_value.Sort(new ICP_儲位管理_儲位資料());
            for (int i = 0; i < list_value.Count; i++)
            {
                list_value[i][(int)enum_儲位管理_儲位資料.位置] = $"{(i / 6) + 1}-{(i % 6) + 1}";
            }
            if (UpdateToSQL)
            {
                List<object[]> list_select_row = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
                if (list_select_row.Count > 0)
                {
                    string _IP = list_select_row[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                    Storage storage = this.storageUI_WT32.SQL_GetStorage(_IP);
                    this.wT32_GPADC.Set_Stroage(storage);
                }
            }


            return list_value;
        }
        private Storage Function_儲位管理_從SQL取得Storage(string IP)
        {
            Storage storage = null;
            storage = this.storageUI_WT32.SQL_GetStorage(IP);
            return storage;
        }
        #endregion
        #region Event
        private void RJ_TextBox_儲位管理_參數設定_藥品條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                List<object[]> list_value = this.sqL_DataGridView_儲位管理_參數設定.SQL_GetAllRows(false);
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品條碼1, rJ_TextBox_儲位管理_參數設定_藥品條碼.Text);
                this.sqL_DataGridView_儲位管理_參數設定.RefreshGrid(list_value);
            }
        }
        private void SqL_DataGridView_儲位管理_儲位資料_RowEnterEvent(object[] RowValue)
        {
            
            Storage storage = this.wT32_GPADC.CurrentStorage;
            if (storage != null) this.storageUI_WT32.SQL_ReplaceStorage(storage);

            rJ_TextBox_儲位管理_儲位資料_儲位名稱.Text = RowValue[(int)enum_儲位管理_儲位資料.儲位名稱].ObjectToString();
            rJ_TextBox_儲位管理_儲位資料_包裝數量.Text = RowValue[(int)enum_儲位管理_儲位資料.單位包裝數量].ObjectToString();
            rJ_TextBox_儲位管理_儲位資料_可放置盒數.Text = RowValue[(int)enum_儲位管理_儲位資料.可放置盒數].ObjectToString();

            string IP = RowValue[(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            string JsonString = this.storageUI_WT32.GetUDPJsonString(IP);
            storage = this.storageUI_WT32.SQL_GetStorage(IP);
            this.wT32_GPADC.Set_Stroage(storage);
        }
        private void SqL_DataGridView_儲位管理_參數設定_RowDoubleClickEvent(object[] RowValue)
        {

        }
        private void SqL_DataGridView_儲位管理_儲位資料_DataGridRefreshEvent()
        {


        }
        private void plC_RJ_Button_儲位管理_參數設定_填入儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_儲位管理_參數設定.Get_All_Select_RowsValues();
            List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
            if (list_藥品資料.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇參數設定!");
                }));         
                return;
            }
            if (list_儲位資料.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇儲位資料!");
                }));
                return;
            }
            string IP = list_儲位資料[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            string 藥品碼 = list_藥品資料[0][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
            Storage storage = this.storageUI_WT32.SQL_GetStorage(IP);
            if (storage != null)
            {
                this.wT32_GPADC.Set_Main_Page(storage);
                storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value, 藥品碼);

                storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Title, "");
                storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Title, "");

                this.storageUI_WT32.SQL_ReplaceStorage(storage);

                this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());
            }


        }
        private void plC_RJ_Button_儲位管理_參數設定_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {

            List<object[]> list_value = this.sqL_DataGridView_儲位管理_參數設定.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<object[]> list_value_result = new List<object[]>();
            List<List<object[]>> list_list_value = new List<List<object[]>>();
            if (!this.rJ_TextBox_儲位管理_參數設定_藥品碼.Text.StringIsEmpty())
            {
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品碼, rJ_TextBox_儲位管理_參數設定_藥品碼.Text);
            }
            if (!this.rJ_TextBox_儲位管理_參數設定_藥品名稱.Text.StringIsEmpty())
            {
                list_value = list_value.GetRowsByLike((int)enum_參數設定_藥檔資料.藥品名稱, rJ_TextBox_儲位管理_參數設定_藥品名稱.Text);
            }
            if (!this.rJ_TextBox_儲位管理_參數設定_藥品條碼.Text.StringIsEmpty())
            {
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品條碼1, rJ_TextBox_儲位管理_參數設定_藥品條碼.Text);
            }

            this.sqL_DataGridView_儲位管理_參數設定.RefreshGrid(list_value);
        }
        private void plC_RJ_Button_儲位管理_儲位資料_寫入_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
            if (list_儲位資料.Count == 0)
            {
                MyMessageBox.ShowDialog("未選擇儲位資料!");
                return;
            }
            if(rJ_TextBox_儲位管理_儲位資料_包裝數量.Text.StringToInt32() < 0)
            {
                MyMessageBox.ShowDialog("'包裝數量'請輸入合法字元'!");
                return;
            }
            List<Storage> list_Storage = this.storageUI_WT32.SQL_GetAllStorage();

            List<Storage> list_Storage_replace = new List<Storage>();
            List<Task> taskList = new List<Task>();
            string IP = "";
            for (int i = 0; i < list_儲位資料.Count; i++)
            {

                IP = list_儲位資料[i][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                Storage storage = list_Storage.SortByIP(IP);
                if (storage != null)
                {
                    taskList.Add(Task.Run(() =>
                    {
                        this.wT32_GPADC.Set_Main_Page(storage);
                        storage.SetValue(Storage.ValueName.儲位名稱, Storage.ValueType.Value, rJ_TextBox_儲位管理_儲位資料_儲位名稱.Text);
                        storage.SetValue(Storage.ValueName.最小包裝單位數量, Storage.ValueType.Value, rJ_TextBox_儲位管理_儲位資料_包裝數量.Text);
                        storage.Max_Inventory = rJ_TextBox_儲位管理_儲位資料_可放置盒數.Text.StringToInt32();
                        if (storage.Max_Inventory <= 0) storage.Max_Inventory = 1;
                    }));


                    list_Storage_replace.Add(storage);
                }
            }
            this.storageUI_WT32.SQL_ReplaceStorage(list_Storage_replace);

            this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());

            
        }
        private void plC_RJ_Button_儲位管理_儲位資料_複製格式_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.wT32_GPADC.CurrentStorage != null)
            {
                this.storage_儲位管理_儲位資料_複製格式 = this.wT32_GPADC.CurrentStorage.DeepClone(); 
            }
          
        }
        private void plC_RJ_Button_儲位管理_儲位資料_貼上格式_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.storage_儲位管理_儲位資料_複製格式 != null)
            {
                List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
                if (list_儲位資料.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位資料!");
                    return;
                }
                this.wT32_GPADC.CurrentStorage.PasteFormat(this.storage_儲位管理_儲位資料_複製格式);
                List<Storage> list_Storage = this.storageUI_WT32.SQL_GetAllStorage();
            
                List<Storage> list_Storage_replace = new List<Storage>();
                List<Task> taskList = new List<Task>();
                string IP = "";
                for(int i = 0; i < list_儲位資料.Count; i++)
                {
                    
                    IP = list_儲位資料[i][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                    Storage storage = list_Storage.SortByIP(IP);
                    if (storage != null)
                    {
                        taskList.Add(Task.Run(() =>
                        {
                            this.wT32_GPADC.Set_Main_Page(storage);
                        }));
                
 
                        storage.PasteFormat(this.storage_儲位管理_儲位資料_複製格式);
                        list_Storage_replace.Add(storage);
                    }
                }
                this.storageUI_WT32.SQL_ReplaceStorage(list_Storage_replace);
                Task.WhenAll(taskList).Wait();
                //this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());

            }
        }
        private void plC_RJ_Button_儲位管理_儲位資料_清除儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            if(MyMessageBox.ShowDialog("是否清除儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
                if (list_儲位資料.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇儲位資料!");
                    return;
                }
                string IP = list_儲位資料[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                string 藥品碼 = "";
                Storage storage = this.storageUI_WT32.SQL_GetStorage(IP);
                if (storage != null)
                {
                    this.wT32_GPADC.Set_Main_Page(storage);
                    storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value, 藥品碼);
                    this.storageUI_WT32.SQL_ReplaceStorage(storage);

                    this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());
                }
            }
        }
        private void plC_RJ_Button_儲位管理_儲位資料_效期庫存異動_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_Validity_period_Inventory_Setting dialog_Validity_Period_Inventory_Setting = new Dialog_Validity_period_Inventory_Setting();


            List<string> list_Validity_period = this.wT32_GPADC.CurrentStorage.List_Validity_period;
            List<string> list_Inventory = this.wT32_GPADC.CurrentStorage.List_Inventory;
            List<string> list_Lot_number = this.wT32_GPADC.CurrentStorage.List_Lot_number;
            dialog_Validity_Period_Inventory_Setting.Set_Value(list_Validity_period, list_Lot_number, list_Inventory);
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                dialogResult = dialog_Validity_Period_Inventory_Setting.ShowDialog();
            }));
            if (dialogResult == DialogResult.Yes)
            {
                Storage storage = this.wT32_GPADC.CurrentStorage;
                int 單位包裝數量 = storage.GetValue(Storage.ValueName.最小包裝單位數量, Storage.ValueType.Value).StringToInt32();
                int 異動前數量 = storage.Inventory.StringToInt32()* 單位包裝數量;
                int 異動後數量 = 0;
       
                for (int i = 0; i < list_Validity_period.Count; i++)
                {
                    storage.效期庫存覆蓋(list_Validity_period[i], list_Lot_number[i], list_Inventory[i], true);
                }
                異動後數量 = storage.Inventory.StringToInt32()* 單位包裝數量;


                object[] value = new object[new enum_交易記錄查詢資料().GetEnumNames().Length];
                value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_交易記錄查詢資料.動作] = enum_交易記錄查詢動作.效期庫存異動.GetEnumName();
                value[(int)enum_交易記錄查詢資料.藥品碼] = storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value);
                value[(int)enum_交易記錄查詢資料.藥品名稱] = storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Value);
                value[(int)enum_交易記錄查詢資料.藥袋序號] = "";
                value[(int)enum_交易記錄查詢資料.庫存量] = 異動前數量;
                value[(int)enum_交易記錄查詢資料.交易量] = 異動後數量 - 異動前數量;
                value[(int)enum_交易記錄查詢資料.結存量] = 異動後數量;
                value[(int)enum_交易記錄查詢資料.病人姓名] = "";
                value[(int)enum_交易記錄查詢資料.病歷號] = "";
                value[(int)enum_交易記錄查詢資料.操作時間] = DateTime.Now.ToDateTimeString_6();
                value[(int)enum_交易記錄查詢資料.開方時間] = DateTime.Now.ToDateTimeString_6();
                value[(int)enum_交易記錄查詢資料.備註] = "";

                this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);

                this.storageUI_WT32.SQL_ReplaceStorage(storage);
                this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());

            }
        }
        private void PlC_RJ_Button_儲位管理_套餐資料_填入儲位_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_套餐資料 = this.sqL_DataGridView_儲位管理_套餐資料.Get_All_Select_RowsValues();
            List<object[]> list_儲位資料 = this.sqL_DataGridView_儲位管理_儲位資料.Get_All_Select_RowsValues();
            if (list_套餐資料.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇套餐資料!");
                }));
                return;
            }
            if (list_儲位資料.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇儲位資料!");
                }));
                return;
            }
            string IP = list_儲位資料[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
            string 套餐代碼 = list_套餐資料[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            Storage storage = this.storageUI_WT32.SQL_GetStorage(IP);
            if (storage != null)
            {
                this.wT32_GPADC.Set_Main_Page(storage);
                storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Title, enum_藥檔資料_套餐設定_套餐列表.套餐代碼.GetEnumName());
                storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value, 套餐代碼);

                storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Title, enum_藥檔資料_套餐設定_套餐列表.套餐名稱.GetEnumName());

                this.storageUI_WT32.SQL_ReplaceStorage(storage);

                this.sqL_DataGridView_儲位管理_儲位資料.RefreshGrid(this.Function_儲位管理_儲位資料_取得儲位資料());
            }
        }
        private void PlC_RJ_Button_儲位管理_儲位資料_新增效期測試_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.wT32_GPADC.CurrentStorage == null) return;
            Dialog_DateTime dialog_DateTime = new Dialog_DateTime();
            DateTime dateTime = DateTime.Now;
            this.Invoke(new Action(delegate 
            {
                if (dialog_DateTime.ShowDialog() != DialogResult.Yes) return;
                dateTime = dialog_DateTime.Value;
            }));
            string 藥品碼 = this.wT32_GPADC.CurrentStorage.Code;
            string 批號 = "test";
            List<object[]> list_value = this.sqL_DataGridView_效期批號維護.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_效期批號維護.藥品碼, 藥品碼);
            list_value = list_value.GetRowsInDate((int)enum_效期批號維護.效期, dateTime);
            this.sqL_DataGridView_效期批號維護.SQL_DeleteExtra(list_value, false);
            object[] value_效期 = new object[new enum_效期批號維護().GetLength()];
            value_效期[(int)enum_效期批號維護.GUID] = Guid.NewGuid().ToString();
            value_效期[(int)enum_效期批號維護.藥品碼] = 藥品碼;
            value_效期[(int)enum_效期批號維護.效期] = dateTime.ToDateString();
            value_效期[(int)enum_效期批號維護.批號] = 批號;
            value_效期[(int)enum_效期批號維護.加入時間] = DateTime.Now.ToDateTimeString_6();

            this.sqL_DataGridView_效期批號維護.SQL_AddRow(value_效期, false);



        }
        #endregion
    }
}
