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
    public enum enum_入庫作業_藥品資料
    {
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        藥品條碼,
        包裝單位,
    }
    public enum enum_入庫作業_選擇儲位
    {
        IP,
        Port,
        儲位名稱,
        庫存,
        可放置盒數,
        位置,
    }
    public enum enum_入庫作業_效期及批號
    {
        效期,
        批號,
        數量,
    }
    public partial class Form1 : Form
    {
  
        private void Program_入庫作業_Init()
        {
           
            rJ_TextBox_入庫作業_數量.KeyPress += rJ_TextBox_CheckNum_KeyPress;
            this.sqL_DataGridView_入庫作業_套餐資料.Init(this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表);
            this.sqL_DataGridView_入庫作業_套餐資料.Set_ColumnVisible(false, new enum_藥檔資料_套餐設定_套餐列表().GetEnumNames());
            this.sqL_DataGridView_入庫作業_套餐資料.Set_ColumnVisible(true, enum_藥檔資料_套餐設定_套餐列表.套餐代碼, enum_藥檔資料_套餐設定_套餐列表.套餐名稱);
            this.sqL_DataGridView_入庫作業_套餐資料.Set_ColumnWidth(200, enum_藥檔資料_套餐設定_套餐列表.套餐代碼);
            this.sqL_DataGridView_入庫作業_套餐資料.Set_ColumnWidth(200, enum_藥檔資料_套餐設定_套餐列表.套餐名稱);
            this.sqL_DataGridView_入庫作業_套餐資料.RowEnterEvent += SqL_DataGridView_入庫作業_套餐資料_RowEnterEvent;

            this.sqL_DataGridView_入庫作業_套餐內容.Init(this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容);
            this.sqL_DataGridView_入庫作業_套餐內容.Set_ColumnVisible(false, new enum_藥檔資料_套餐設定_套餐內容().GetEnumNames());
            this.sqL_DataGridView_入庫作業_套餐內容.Set_ColumnVisible(true, enum_藥檔資料_套餐設定_套餐內容.藥品碼, enum_藥檔資料_套餐設定_套餐內容.藥品名稱, enum_藥檔資料_套餐設定_套餐內容.數量);
            this.sqL_DataGridView_入庫作業_套餐內容.Set_ColumnWidth(100, enum_藥檔資料_套餐設定_套餐內容.藥品碼);
            this.sqL_DataGridView_入庫作業_套餐內容.Set_ColumnWidth(200, enum_藥檔資料_套餐設定_套餐內容.藥品名稱);
            this.sqL_DataGridView_入庫作業_套餐內容.Set_ColumnWidth(60, enum_藥檔資料_套餐設定_套餐內容.數量);

            this.sqL_DataGridView_入庫作業_藥品資料.Init();
            this.sqL_DataGridView_入庫作業_藥品資料.RowDoubleClickEvent += sqL_DataGridView_入庫作業_藥品資料_RowDoubleClickEvent;
            this.sqL_DataGridView_入庫作業_藥品資料.RowEnterEvent += sqL_DataGridView_入庫作業_藥品資料_RowEnterEvent;
            this.sqL_DataGridView_入庫作業_選擇儲位.Init();
            this.sqL_DataGridView_入庫作業_選擇儲位.RowDoubleClickEvent += SqL_DataGridView_入庫作業_選擇儲位_RowDoubleClickEvent;
            this.sqL_DataGridView_入庫作業_選擇儲位.RowEnterEvent += SqL_DataGridView_入庫作業_選擇儲位_RowEnterEvent;
            this.sqL_DataGridView_入庫作業_效期及批號.Init();
            this.sqL_DataGridView_入庫作業_效期及批號.RowEnterEvent += SqL_DataGridView_入庫作業_效期及批號_RowEnterEvent;

            this.rJ_TextBox_入庫作業_藥品資料_藥品條碼.KeyPress += RJ_TextBox_入庫作業_藥品資料_藥品條碼_KeyPress;

            this.plC_RJ_Button_入庫作業_藥品資料_搜尋.MouseDownEvent += PlC_RJ_Button_入庫作業_藥品資料_搜尋_MouseDownEvent;
            this.plC_RJ_Button_入庫作業_取消作業.MouseDownEvent += PlC_RJ_Button_入庫作業_取消作業_MouseDownEvent;
            this.plC_RJ_Button_入庫作業_藥品資料_確認.MouseDownEvent += PlC_RJ_Button_入庫作業_藥品資料_確認_MouseDownEvent;
            this.plC_RJ_Button_入庫作業_套餐資料_確認.MouseDownEvent += PlC_RJ_Button_入庫作業_套餐資料_確認_MouseDownEvent;
            this.plC_RJ_Button_重置退藥資料.MouseDownEvent += PlC_RJ_Button_重置退藥資料_MouseDownEvent;


        }

    

        private bool flag_Program_入庫作業_Init = false;
        private void Program_入庫作業()
        {
            if(this.plC_ScreenPage_Main.PageText == "入庫作業")
            {
                string readline = this.MySerialPort_Scanner.ReadString();
                if (!readline.StringIsEmpty())
                {
                    this.Invoke(new Action(delegate
                    {
                        if (rJ_TextBox_入庫作業_藥品資料_藥品條碼.IsFocused)
                        {
                            readline = readline.Replace("\n", "");
                            readline = readline.Replace("\r", "");
                            rJ_TextBox_入庫作業_藥品資料_藥品條碼.Texts = readline;
                            this.RJ_TextBox_入庫作業_藥品資料_藥品條碼_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                        }

                        this.MySerialPort_Scanner.ClearReadByte();
                    }));
                }
                if (!flag_Program_入庫作業_Init)
                {
                    this.MySerialPort_Scanner.ClearReadByte();
                    this.Function_入庫作業_藥品資料更新DataGrid();
                    this.Function_入庫作業_套餐資料更新DataGrid();
                    flag_Program_入庫作業_Init = true;
                }
            }
            else
            {
                flag_Program_入庫作業_Init = false;
            }
            sub_Program_入庫作業();
        }

        #region PLC_入庫作業
        int 入庫作業_開鎖按鈕_poX = 0;
        int 入庫作業_開鎖按鈕_poY = 0;
        int 入庫作業_開鎖按鈕_width = 0;
        int 入庫作業_開鎖按鈕_height = 0;
        int 入庫作業_開鎖按鈕_層數 = 0;
        int 入庫作業_開鎖按鈕_格數 = 0;
        int 入庫作業_庫存 = 0;
        int 入庫作業_可放置盒數 = 0;


        PLC_Device PLC_Device_入庫作業 = new PLC_Device("S6205");
        PLC_Device PLC_Device_入庫作業_OK = new PLC_Device("S6206");

        PLC_Device PLC_Device_入庫作業_選擇藥品 = new PLC_Device("S6215");
        PLC_Device PLC_Device_入庫作業_選擇套餐 = new PLC_Device("S6217");

        PLC_Device PLC_Device_入庫作業_選擇儲位 = new PLC_Device("S6216");
        PLC_Device PLC_Device_入庫作業_輸入藥品資訊_確認 = new PLC_Device("S6220");
        PLC_Device PLC_Device_入庫作業_M8013 = new PLC_Device("M8013");
        int cnt_Program_入庫作業 = 65534;
        void sub_Program_入庫作業()
        {
            if(this.plC_ScreenPage_Main.PageText == "入庫作業")
            {
                PLC_Device_入庫作業.Bool = true;
            }
            else
            {
                PLC_Device_入庫作業.Bool = false;
            }
            if (cnt_Program_入庫作業 == 65534)
            {

                PLC_Device_入庫作業.SetComment("PLC_入庫作業");
                PLC_Device_入庫作業_OK.SetComment("PLC_入庫作業_OK");
                PLC_Device_入庫作業_選擇藥品.SetComment("PLC_Device_入庫作業_選擇藥品");
                PLC_Device_入庫作業_選擇儲位.SetComment("PLC_Device_入庫作業_選擇儲位");
                PLC_Device_入庫作業_輸入藥品資訊_確認.SetComment("PLC_Device_入庫作業_輸入藥品資訊_確認");
                PLC_Device_入庫作業.Bool = false;
                cnt_Program_入庫作業 = 65535;
            }
            if (cnt_Program_入庫作業 == 65535) cnt_Program_入庫作業 = 1;
            if (cnt_Program_入庫作業 == 1) cnt_Program_入庫作業_檢查按下(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 2) cnt_Program_入庫作業_初始化(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 3)
            {
                plC_RJ_Pannel_入庫作業_請選擇或掃描藥品.Set_Enable(true);
                cnt_Program_入庫作業 = 100;
            }

            if (cnt_Program_入庫作業 == 100) cnt_Program_入庫作業_100_檢查是否選取藥品(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 101)
            {
                plC_RJ_Pannel_入庫作業_請選擇或掃描藥品.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_選擇儲位.Set_Enable(true);
                cnt_Program_入庫作業 = 200;
            }

            if (cnt_Program_入庫作業 == 200) cnt_Program_入庫作業_200_取得儲位(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 201) cnt_Program_入庫作業_200_等待選擇儲位(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 202)
            {
                plC_RJ_Pannel_入庫作業_選擇儲位.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_輸入藥品資訊.Set_Enable(true);
                cnt_Program_入庫作業 = 300;
            }

            if (cnt_Program_入庫作業 == 300) cnt_Program_入庫作業_300_取得藥品資訊(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 301) cnt_Program_入庫作業_300_檢查藥品資訊(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 302)
            {
                plC_RJ_Pannel_入庫作業_輸入藥品資訊.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_等待觸控開鎖.Set_Enable(true);
                cnt_Program_入庫作業 = 400;
            }

            if (cnt_Program_入庫作業 == 400) cnt_Program_入庫作業_400_換至解鎖中頁面(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 401) cnt_Program_入庫作業_400_檢查換至解鎖中頁面完成(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 402) cnt_Program_入庫作業_400_繪製解鎖中頁面(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 403) cnt_Program_入庫作業_400_檢查解鎖中頁面觸控位置(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 404)
            {
                plC_RJ_Pannel_入庫作業_等待觸控開鎖.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_解鎖等待取出抽屜.Set_Enable(true);
                cnt_Program_入庫作業 = 500;
            }

            if (cnt_Program_入庫作業 == 500) cnt_Program_入庫作業_500_解鎖開始(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 501) cnt_Program_入庫作業_500_解鎖結束(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 502)
            {
                plC_RJ_Pannel_入庫作業_解鎖等待取出抽屜.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_等待補藥.Set_Enable(true);
                cnt_Program_入庫作業 = 600;
            }

            if (cnt_Program_入庫作業 == 600) cnt_Program_入庫作業_600_繪製補藥中(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 601) cnt_Program_入庫作業_600_等待補藥結束(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 602)
            {
                plC_RJ_Pannel_入庫作業_等待補藥.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_補藥完成寫入資料.Set_Enable(true);
                cnt_Program_入庫作業 = 700;
            }
            if (cnt_Program_入庫作業 == 700) cnt_Program_入庫作業_700_寫至SQL儲位(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 701) cnt_Program_入庫作業_700_換至主頁面(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 702)
            {
                plC_RJ_Pannel_入庫作業_補藥完成寫入資料.Set_Enable(false);
                cnt_Program_入庫作業 = 800;
            }
            if (cnt_Program_入庫作業 == 800) cnt_Program_入庫作業_800_詢問是否前推一格(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 801) cnt_Program_入庫作業_800_XY_Table開始移動(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 802) cnt_Program_入庫作業_800_XY_Table等待移動結束(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 803) cnt_Program_入庫作業_800_送料馬達前推一次開始(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 804) cnt_Program_入庫作業_800_送料馬達前推一次結束(ref cnt_Program_入庫作業);
            if (cnt_Program_入庫作業 == 805)
            {
                cnt_Program_入庫作業 = 65500;
            }

            if (cnt_Program_入庫作業 > 1) cnt_Program_入庫作業_檢查放開(ref cnt_Program_入庫作業);

            if (cnt_Program_入庫作業 == 65500)
            {
                this.PLC_Device_抽屜開鎖.Bool = false;
                this.Function_入庫作業_清除內容();
                PLC_Device_入庫作業.Bool = false;
                PLC_Device_入庫作業_OK.Bool = false;
                cnt_Program_入庫作業 = 65535;
            }

            #region Pannel閃爍
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_請選擇或掃描藥品);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_選擇儲位);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_輸入藥品資訊);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_等待觸控開鎖);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_解鎖等待取出抽屜);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_等待補藥);
            this.Function_入庫作業_Pannel閃爍(plC_RJ_Pannel_入庫作業_補藥完成寫入資料);

            #endregion
        }
        void cnt_Program_入庫作業_檢查按下(ref int cnt)
        {
            if (PLC_Device_入庫作業.Bool) cnt++;
        }
        void cnt_Program_入庫作業_檢查放開(ref int cnt)
        {
            if (!PLC_Device_入庫作業.Bool) cnt = 65500;
        }
        void cnt_Program_入庫作業_初始化(ref int cnt)
        {
            this.Function_入庫作業_清除內容();

            cnt++;
        }
        void cnt_Program_入庫作業_100_檢查是否選取藥品(ref int cnt)
        {
            if (PLC_Device_入庫作業_選擇藥品.Bool)
            {
                List<object[]> list_value = this.sqL_DataGridView_入庫作業_藥品資料.Get_All_Select_RowsValues();
                if (list_value.Count > 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.rJ_TextBox_入庫作業_藥品套餐碼.Texts = list_value[0][(int)enum_入庫作業_藥品資料.藥品碼].ObjectToString();
                        this.rJ_TextBox_入庫作業_藥品套餐名稱.Texts = list_value[0][(int)enum_入庫作業_藥品資料.藥品名稱].ObjectToString();
                        this.rJ_TextBox_入庫作業_藥品中文名稱.Texts = list_value[0][(int)enum_入庫作業_藥品資料.藥品中文名稱].ObjectToString();
                    }));
                    cnt++;
                }
            }
            if(PLC_Device_入庫作業_選擇套餐.Bool)
            {
                List<object[]> list_value = this.sqL_DataGridView_入庫作業_套餐資料.Get_All_Select_RowsValues();
                if (list_value.Count > 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.rJ_TextBox_入庫作業_藥品套餐碼.Texts = list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
                        this.rJ_TextBox_入庫作業_藥品套餐名稱.Texts = list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱].ObjectToString();
                        this.rJ_TextBox_入庫作業_藥品中文名稱.Texts = "";
                    }));
                    cnt++;
                }
            }
           
        }
        void cnt_Program_入庫作業_200_取得儲位(ref int cnt)
        {
            List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
            List<object[]> list_儲位資料_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();
            list_儲位資料_buf = list_儲位資料.GetRows((int)enum_儲位管理_儲位資料.藥品碼, this.rJ_TextBox_入庫作業_藥品套餐碼.Texts);

            for (int i = 0; i < list_儲位資料_buf.Count; i++)
            {
                object[] value = new object[new enum_入庫作業_選擇儲位().GetEnumNames().Length];
                value[(int)enum_入庫作業_選擇儲位.IP] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                value[(int)enum_入庫作業_選擇儲位.Port] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.Port].ObjectToString();
                value[(int)enum_入庫作業_選擇儲位.位置] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.位置].ObjectToString();
                value[(int)enum_入庫作業_選擇儲位.儲位名稱] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.儲位名稱].ObjectToString();
                value[(int)enum_入庫作業_選擇儲位.庫存] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.庫存].ObjectToString();
                value[(int)enum_入庫作業_選擇儲位.可放置盒數] = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.可放置盒數].ObjectToString();
                list_value.Add(value);
            }
            this.sqL_DataGridView_入庫作業_選擇儲位.RefreshGrid(list_value);
            cnt++;
        }
        void cnt_Program_入庫作業_200_等待選擇儲位(ref int cnt)
        {
            if (PLC_Device_入庫作業_選擇儲位.Bool)
            {
                cnt++;
            }

        }
        void cnt_Program_入庫作業_300_取得藥品資訊(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            List<object[]> list_value = new List<object[]>();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                入庫作業_庫存 = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.庫存].StringToInt32();
                入庫作業_可放置盒數 = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.可放置盒數].StringToInt32();
                Storage storage = Function_儲位管理_從SQL取得Storage(IP);
                if (storage != null)
                {
                    for (int i = 0; i < storage.List_Validity_period.Count; i++)
                    {
                        object[] value = new object[new enum_入庫作業_效期及批號().GetEnumNames().Length];
                        value[(int)enum_入庫作業_效期及批號.效期] = storage.List_Validity_period[i];
                        value[(int)enum_入庫作業_效期及批號.批號] = storage.List_Lot_number[i];
                        value[(int)enum_入庫作業_效期及批號.數量] = storage.List_Inventory[i];
                        list_value.Add(value);
                    }
                    List<object[]> list_效期_value = this.sqL_DataGridView_效期批號維護.SQL_GetAllRows(false);
                    for (int i = 0; i < list_效期_value.Count; i++)
                    {
                        object[] value = new object[new enum_入庫作業_效期及批號().GetEnumNames().Length];
                        value[(int)enum_入庫作業_效期及批號.效期] = list_效期_value[i][(int)enum_效期批號維護.效期].ToDateString();
                        value[(int)enum_入庫作業_效期及批號.批號] = list_效期_value[i][(int)enum_效期批號維護.批號].ObjectToString();
                        value[(int)enum_入庫作業_效期及批號.數量] = "0";
                        list_value.Add(value);
                    }
                    this.sqL_DataGridView_入庫作業_效期及批號.RefreshGrid(list_value);
                }
            }

            PLC_Device_入庫作業_輸入藥品資訊_確認.Bool = false;


            cnt++;

        }
        void cnt_Program_入庫作業_300_檢查藥品資訊(ref int cnt)
        {

            if(PLC_Device_入庫作業_輸入藥品資訊_確認.Bool)
            {
             
                string 效期 = $"{dateTimeComList_入庫作業_效期.Value.ToDateString()}";
                int 數量 = rJ_TextBox_入庫作業_數量.Texts.StringToInt32();
                string error_str = "";
                this.Invoke(new Action(delegate
                {
                    if (!效期.Check_Date_String())
                    {

                        error_str += "非法日期格式 \n";
                    }
                    if (數量 < 0)
                    {
                        this.rJ_TextBox_入庫作業_數量.Texts = "";
                        error_str += "非法數量格式 \n";
                    }
                }));
           
                if(!error_str.StringIsEmpty())
                {
                    PLC_Device_入庫作業_輸入藥品資訊_確認.Bool = false;
                    MyMessageBox.ShowDialog(error_str);
                    return;
                }
                if(入庫作業_庫存 + 數量 > 入庫作業_可放置盒數)
                {
                    PLC_Device_入庫作業_輸入藥品資訊_確認.Bool = false;
                    MyMessageBox.ShowDialog($"入庫後數量為[{入庫作業_庫存 + 數量}] 大於 可放置盒數[{入庫作業_可放置盒數}]");
                    System.Threading.Thread.Sleep(1000);
                    return;
                }
                PLC_Device_入庫作業_輸入藥品資訊_確認.Bool = false;
                cnt++;
            }


        }
        void cnt_Program_入庫作業_400_換至解鎖中頁面(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                int Port = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.Port].ObjectToString().StringToInt32();
                int flag_OK = 0;
                if (this.wT32_GPADC.Set_ToPage(IP, Port, (int)StorageUI_WT32.enum_Page.解鎖頁面)) flag_OK++;
                if (this.wT32_GPADC.Set_JsonStringSend(IP, Port)) flag_OK++;
                if (flag_OK == 2)
                {
                    cnt++;
                    return;
                }
                else
                {
                    MyMessageBox.ShowDialog("換至解鎖頁面失敗!");
                    cnt = 65500;
                    return;
                }
            }
          
        }
        void cnt_Program_入庫作業_400_檢查換至解鎖中頁面完成(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                string jsonstring = this.storageUI_WT32.GetUDPJsonString(IP);
                StorageUI_WT32.UDP_READ uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(jsonstring);
                if (uDP_READ != null)
                {
                    if (uDP_READ.ScreenPage_Init == true)
                    {
                        cnt++;
                    }
                }
            }
        }
        void cnt_Program_入庫作業_400_繪製解鎖中頁面(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                int Pannel_Width = WT32_GPADC.Pannel_Width;
                int Pannel_Height = WT32_GPADC.Pannel_Height;
                int Port = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.Port].ObjectToString().StringToInt32(); 
                int flag_OK = 0;
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
                int poY_buf = 0;
                Text = "入庫中";
                font = new Font("標楷體", 24, FontStyle.Bold);
                width = Pannel_Width - 20;
                height = TextRenderer.MeasureText(Text, font).Height + 10;           
                poX = (Pannel_Width - width) / 2;
                poY = 0;
                poY_buf += height;
                font_color = Color.White;
                back_color = Color.SkyBlue;
                border_size = 2;
                border_color = Color.SkyBlue;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Center)) flag_OK++;


                Text = $"效期 : {dateTimeComList_入庫作業_效期.Value.ToDateString()}";
                font = new Font("標楷體", 20, FontStyle.Bold);
                width = Pannel_Width - 100;
                height = TextRenderer.MeasureText(Text, font).Height;            
                poX = 20;
                poY_buf += 10;
                poY = poY_buf;
                poY_buf += height;
                font_color = Color.White;
                back_color = Color.HotPink;
                border_size = 2;
                border_color = Color.HotPink;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Left)) flag_OK++;

                Text = $"批號 : {rJ_TextBox_入庫作業_批號.Text}";
                font = new Font("標楷體", 20, FontStyle.Bold);
                width = Pannel_Width - 100;
                height = TextRenderer.MeasureText(Text, font).Height;
                poX = 20;
                poY_buf += 10;
                poY = poY_buf;
                poY_buf += height;
                font_color = Color.White;
                back_color = Color.HotPink;
                border_size = 2;
                border_color = Color.HotPink;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Left)) flag_OK++;

                Text = $"數量 : {rJ_TextBox_入庫作業_數量.Text}";
                font = new Font("標楷體", 20, FontStyle.Bold);
                width = Pannel_Width - 100;
                height = TextRenderer.MeasureText(Text, font).Height;
                poX = 20;
                poY_buf += 10;
                poY = poY_buf;
                poY_buf += height;
                font_color = Color.White;
                back_color = Color.HotPink;
                border_size = 2;
                border_color = Color.HotPink;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Left)) flag_OK++;


                Text = "解鎖";
                font = new Font("標楷體", 45, FontStyle.Bold);
                width = Pannel_Width - 20;
                height = TextRenderer.MeasureText(Text, font).Height + 20;
                poX = (Pannel_Width - width) / 2;
                poY_buf += 20;
                poY = poY_buf;
                poY_buf += height;
                font_color = Color.White;
                back_color = Color.RoyalBlue;
                border_size = 2;
                border_color = Color.RoyalBlue;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Center)) flag_OK++;

                入庫作業_開鎖按鈕_poX = poX;
                入庫作業_開鎖按鈕_poY = poY;
                入庫作業_開鎖按鈕_width = width;
                入庫作業_開鎖按鈕_height = height;

                if (this.wT32_GPADC.Set_ScreenPageInit(IP, Port, false)) flag_OK++;
                if (this.wT32_GPADC.Set_JsonStringSend(IP, Port)) flag_OK++;
                if (flag_OK == 7)
                {
                    cnt++;
                    return;
                }
                else
                {
                    MyMessageBox.ShowDialog("解鎖繪製失敗!");
                    cnt = 65500;
                    return;
                }
            }
        } 
        void cnt_Program_入庫作業_400_檢查解鎖中頁面觸控位置(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                string jsonstring = this.storageUI_WT32.GetUDPJsonString(IP);
                StorageUI_WT32.UDP_READ uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(jsonstring);
                if (uDP_READ != null)
                {
                    int xPos = uDP_READ.Touch_xPos;
                    int yPos = uDP_READ.Touch_yPos;
                    this.Invoke(new Action(delegate 
                    {
                        this.rJ_TextBox_入庫作業_X觸控位置.Texts = uDP_READ.Touch_xPos.ToString();
                        this.rJ_TextBox_入庫作業_Y觸控位置.Texts = uDP_READ.Touch_yPos.ToString();
                    }));
                    WT32_GPADC.TxMouseDownType txMouseDownType = WT32_GPADC.GetMouseDownType(xPos, yPos, 1, 1, WT32_GPADC.Pannel_Width, WT32_GPADC.Pannel_Height);
                    if(txMouseDownType != WT32_GPADC.TxMouseDownType.NONE)
                    {
                        cnt++;
                    }
                }
            }
        }
        void cnt_Program_入庫作業_500_解鎖開始(ref int cnt)
        {
            if(!PLC_Device_抽屜開鎖.Bool)
            {
                List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
                List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                List<object[]> list_value = new List<object[]>();
                list_value = list_儲位資料.GetRows((int)enum_儲位管理_儲位資料.IP, IP);
                if(list_value.Count > 0)
                {
                    string 位置 = list_value[0][(int)enum_儲位管理_儲位資料.位置].ObjectToString();
                    string[] ARRAY = myConvert.分解分隔號字串(位置, "-");
                    if (ARRAY.Length == 2)
                    {
                        int 層數 = ARRAY[0].StringToInt32() - 1;
                        int 格數 = ARRAY[1].StringToInt32() - 1;
                        this.PLC_Device_抽屜開鎖_層數.Value = 層數;
                        this.PLC_Device_抽屜開鎖_格數.Value = 格數;

                        入庫作業_開鎖按鈕_層數 = 層數;
                        入庫作業_開鎖按鈕_格數 = 格數;
                    }
                    else
                    {
                        MyMessageBox.ShowDialog($"位字串不合法! {位置}");
                        cnt = 65500;
                        return;
                    }
                }
                else
                {
                    MyMessageBox.ShowDialog($"查無開鎖儲位! {IP}");
                    cnt = 65500;
                    return;
                }
                PLC_Device_抽屜開鎖.Bool = true;
                cnt++;
            }
          
        }
        void cnt_Program_入庫作業_500_解鎖結束(ref int cnt)
        {
            if (!PLC_Device_抽屜開鎖.Bool)
            {
                if(!PLC_Device_抽屜開鎖_OK.Bool)
                {
                    plC_RJ_Pannel_入庫作業_解鎖等待取出抽屜.Set_Enable(false);
                    plC_RJ_Pannel_入庫作業_等待觸控開鎖.Set_Enable(true);
                    cnt = 403;
                    return;
                }
                cnt++;
            }
        }
        void cnt_Program_入庫作業_600_繪製補藥中(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                int Pannel_Width = WT32_GPADC.Pannel_Width;
                int Pannel_Height = WT32_GPADC.Pannel_Height;
                int Port = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.Port].ObjectToString().StringToInt32();
                int flag_OK = 0;
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

                Text = "補藥中";
                font = new Font("標楷體", 45, FontStyle.Bold);
                width = 入庫作業_開鎖按鈕_width;
                height = 入庫作業_開鎖按鈕_height;
                poX = 入庫作業_開鎖按鈕_poX;
                poY = 入庫作業_開鎖按鈕_poY;
                font_color = Color.White;
                back_color = Color.RoyalBlue;
                border_size = 2;
                border_color = Color.RoyalBlue;
                if (this.wT32_GPADC.Set_TextEx(IP, Port, Text, poX, poY, width, height, font, font_color, back_color, border_size, border_color, H_Pannel_lib.HorizontalAlignment.Center)) flag_OK++;
                if (flag_OK == 1)
                {
                    cnt++;
                    return;
                }
                else
                {
                    MyMessageBox.ShowDialog("補藥中繪製失敗!");
                    cnt = 65500;
                    return;
                }

            }
        }
        void cnt_Program_入庫作業_600_等待補藥結束(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();

                string jsonstring = this.storageUI_WT32.GetUDPJsonString(IP);
                StorageUI_WT32.UDP_READ uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(jsonstring);
                if (uDP_READ != null)
                {
                    if (uDP_READ.INPUT_LOCK == 1)
                    {
                        cnt++;
                        return;
                    }
                }
            }
        }
        void cnt_Program_入庫作業_700_寫至SQL儲位(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                Storage storage = this.storageUI_WT32.SQL_GetStorage(IP);
                if (storage != null)
                {
                    string 效期 = $"{dateTimeComList_入庫作業_效期.Value.ToDateString()}";
                    string 批號 = $"{rJ_TextBox_入庫作業_批號.Text}";
                    int 數量 = rJ_TextBox_入庫作業_數量.Texts.StringToInt32();

                    object[] value = new object[new enum_交易記錄查詢資料().GetEnumNames().Length];
                    value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_交易記錄查詢資料.動作] = enum_交易記錄查詢動作.入庫.GetEnumName();
                    value[(int)enum_交易記錄查詢資料.藥品碼] = storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value);
                    value[(int)enum_交易記錄查詢資料.藥品名稱] = storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Value);
                    value[(int)enum_交易記錄查詢資料.藥袋序號] = "";
                    value[(int)enum_交易記錄查詢資料.庫存量] = storage.Inventory.StringToInt32();
                    value[(int)enum_交易記錄查詢資料.交易量] = 數量;
                    value[(int)enum_交易記錄查詢資料.結存量] = 數量 + storage.Inventory.StringToInt32();
                    value[(int)enum_交易記錄查詢資料.病人姓名] = "";
                    value[(int)enum_交易記錄查詢資料.病歷號] = "";
                    value[(int)enum_交易記錄查詢資料.操作時間] = DateTime.Now.ToDateTimeString_6();
                    value[(int)enum_交易記錄查詢資料.開方時間] = DateTime.Now.ToDateTimeString_6();
                    value[(int)enum_交易記錄查詢資料.操作人] = this.rJ_TextBox_登入者姓名.Texts;
                    value[(int)enum_交易記錄查詢資料.備註] = $"效期[{效期}] 批號[{批號}]";
                    string 藥品碼 = value[(int)enum_交易記錄查詢資料.藥品碼].ObjectToString();
                    this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);

                    storage.效期庫存異動(效期, 批號, 數量.ToString());

                    DateTime dateTime = 效期.StringToDateTime();
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

                    this.storageUI_WT32.SQL_ReplaceStorage(storage);
                    cnt++;

                }
                else
                {
                    MyMessageBox.ShowDialog("找無關聯儲位!");
                    cnt = 65500;
                    return;
                }
            }
        }
        void cnt_Program_入庫作業_700_換至主頁面(ref int cnt)
        {
            List<object[]> list_選擇儲位資料 = this.sqL_DataGridView_入庫作業_選擇儲位.Get_All_Select_RowsValues();
            if (list_選擇儲位資料.Count > 0)
            {
                string IP = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.IP].ObjectToString();
                int Port = list_選擇儲位資料[0][(int)enum_入庫作業_選擇儲位.Port].ObjectToString().StringToInt32();
                int flag_OK = 0;
                if (this.wT32_GPADC.Set_ToPage(IP, Port, (int)StorageUI_WT32.enum_Page.主頁面)) flag_OK++;
                if (this.wT32_GPADC.Set_JsonStringSend(IP, Port)) flag_OK++;
                if (flag_OK == 2)
                {
                    cnt++;
                    return;
                }
                else
                {
                    MyMessageBox.ShowDialog("換至主頁面失敗!");
                    cnt = 65500;
                    return;
                }
            }
        }
     

        void cnt_Program_入庫作業_800_詢問是否前推一格(ref int cnt)
        {
            cnt = 65500;
            return;
            if (MyMessageBox.ShowDialog("是否往前送一格?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes)
            {
                cnt = 65500;
                return;
            }
            cnt++;
        }
        void cnt_Program_入庫作業_800_XY_Table開始移動(ref int cnt)
        {
            PLC_Device_XY_Table_移動_層數.Value = 入庫作業_開鎖按鈕_層數;
            PLC_Device_XY_Table_移動_格數.Value = 入庫作業_開鎖按鈕_格數;


            if (!PLC_Device_XY_Table_移動.Bool)
            {
                Console.WriteLine($"XY Table開始移動...");
                PLC_Device_XY_Table_移動.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_入庫作業_800_XY_Table等待移動結束(ref int cnt)
        {
            if (!PLC_Device_XY_Table_移動.Bool)
            {
                Console.WriteLine($"XY Table移動完成...");

                cnt++;
            }
        }
        void cnt_Program_入庫作業_800_送料馬達前推一次開始(ref int cnt)
        {
            PLC_Device_送料馬達出料_層數.Value = 入庫作業_開鎖按鈕_層數;
            PLC_Device_送料馬達出料_格數.Value = 入庫作業_開鎖按鈕_格數;
            if (!PLC_Device_送料馬達出料.Bool)
            {
                Console.WriteLine($"馬達開始出料...");
                PLC_Device_送料馬達出料.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_入庫作業_800_送料馬達前推一次結束(ref int cnt)
        {
            if (!PLC_Device_送料馬達出料.Bool)
            {
                Console.WriteLine($"馬達出料完成...");
                cnt++;
            }
        }
        #endregion

        #region Funtion
        private void Function_入庫作業_藥品資料更新DataGrid()
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_入庫作業_藥品資料.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();
            List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
            List<string> list_藥品碼 = new List<string>();
            list_藥品碼 = (from value in list_儲位資料
                        select value[(int)enum_儲位管理_儲位資料.藥品碼].ObjectToString()).Distinct().ToList();
            for (int i = 0; i < list_藥品碼.Count; i++)
            {
                list_藥品資料_buf = list_藥品資料.GetRows((int)enum_參數設定_藥檔資料.藥品碼, list_藥品碼[i]);
                if (list_藥品資料_buf.Count > 0)
                {
                    object[] value = new object[new enum_入庫作業_藥品資料().GetEnumValues().Length];
                    value[(int)enum_入庫作業_藥品資料.藥品碼] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
                    value[(int)enum_入庫作業_藥品資料.藥品名稱] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
                    value[(int)enum_入庫作業_藥品資料.藥品中文名稱] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
                    value[(int)enum_入庫作業_藥品資料.藥品條碼] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品條碼1].ObjectToString();
                    value[(int)enum_入庫作業_藥品資料.包裝單位] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
                    list_value.Add(value);
                }
            }
            this.sqL_DataGridView_入庫作業_藥品資料.RefreshGrid(list_value);
        }
        private void Function_入庫作業_套餐資料更新DataGrid()
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_入庫作業_藥品資料.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_套餐資料 = this.sqL_DataGridView_入庫作業_套餐資料.SQL_GetAllRows(false);
            List<object[]> list_套餐資料_buf = new List<object[]>();

            List<object[]> list_value = new List<object[]>();
            List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
            List<string> list_藥品碼 = new List<string>();
            list_藥品碼 = (from value in list_儲位資料
                        select value[(int)enum_儲位管理_儲位資料.藥品碼].ObjectToString()).Distinct().ToList();
            for (int i = 0; i < list_藥品碼.Count; i++)
            {
                list_套餐資料_buf = list_套餐資料.GetRows((int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼, list_藥品碼[i]);
                if (list_套餐資料_buf.Count > 0)
                {
                    list_value.Add(list_套餐資料_buf[0]);
                }
            }
            this.sqL_DataGridView_入庫作業_套餐資料.RefreshGrid(list_value);
        }
        private void Function_入庫作業_Pannel閃爍(PLC_RJ_Pannel rJ_Pannel)
        {
            if(rJ_Pannel.Get_Enable())
            {
                if (PLC_Device_M8013.Bool)
                {
                    rJ_Pannel.BorderSize = 5;
                    rJ_Pannel.BorderColor = Color.HotPink;
                }
                else
                {
                    rJ_Pannel.BorderSize = 2;
                }
            }
            else
            {
                rJ_Pannel.BorderSize = 2;
                rJ_Pannel.BorderColor = Color.DimGray;
            }
        }
        private void Function_入庫作業_清除內容()
        {
            this.Invoke(new Action(delegate 
            {
                PLC_Device_入庫作業_輸入藥品資訊_確認.Bool = false;


                plC_RJ_Pannel_入庫作業_請選擇或掃描藥品.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_選擇儲位.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_輸入藥品資訊.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_等待觸控開鎖.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_解鎖等待取出抽屜.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_等待補藥.Set_Enable(false);
                plC_RJ_Pannel_入庫作業_補藥完成寫入資料.Set_Enable(false);


                PLC_Device_入庫作業_選擇藥品.Bool = false;
                PLC_Device_入庫作業_選擇套餐.Bool = false;
                PLC_Device_入庫作業_選擇儲位.Bool = false;

               
                this.sqL_DataGridView_入庫作業_選擇儲位.ClearGrid();
                this.sqL_DataGridView_入庫作業_效期及批號.ClearGrid();

                this.rJ_TextBox_入庫作業_藥品套餐碼.Texts = "";
                this.rJ_TextBox_入庫作業_藥品套餐名稱.Texts = "";
                this.rJ_TextBox_入庫作業_藥品中文名稱.Texts = "";

                this.rJ_TextBox_入庫作業_批號.Texts = "";
                this.rJ_TextBox_入庫作業_數量.Texts = "";
                this.rJ_TextBox_入庫作業_X觸控位置.Texts = "";
                this.rJ_TextBox_入庫作業_Y觸控位置.Texts = "";
            }));
        }
        #endregion
        #region Event
        private void SqL_DataGridView_入庫作業_套餐資料_RowEnterEvent(object[] RowValue)
        {
            List<object[]> list_套餐內容 = this.sqL_DataGridView_入庫作業_套餐內容.SQL_GetAllRows(false);
            string 套餐代碼 = RowValue[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            list_套餐內容 = list_套餐內容.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼, 套餐代碼);
            this.sqL_DataGridView_入庫作業_套餐內容.RefreshGrid(list_套餐內容);
        }
        private void SqL_DataGridView_入庫作業_選擇儲位_RowDoubleClickEvent(object[] RowValue)
        {
            this.PLC_Device_入庫作業_選擇儲位.Bool = true;
        }
        private void SqL_DataGridView_入庫作業_選擇儲位_RowEnterEvent(object[] RowValue)
        {
            this.PLC_Device_入庫作業_選擇儲位.Bool = true;
        }
        private void sqL_DataGridView_入庫作業_藥品資料_RowDoubleClickEvent(object[] RowValue)
        {
            this.PLC_Device_入庫作業_選擇藥品.Bool = true;
        }
        private void sqL_DataGridView_入庫作業_藥品資料_RowEnterEvent(object[] RowValue)
        {
            //this.PLC_Device_入庫作業_選擇藥品.Bool = true;
        }
        private void SqL_DataGridView_入庫作業_效期及批號_RowEnterEvent(object[] RowValue)
        {
            if(RowValue != null)
            {
                string 效期 = RowValue[(int)enum_入庫作業_效期及批號.效期].ObjectToString();
                string 批號 = RowValue[(int)enum_入庫作業_效期及批號.批號].ObjectToString();
                DateTime dateTime = 效期.StringToDateTime();
                dateTimeComList_入庫作業_效期.Value = dateTime;
                rJ_TextBox_入庫作業_批號.Texts = 批號;
            }

        }
        private void PlC_RJ_Button_重置退藥資料_MouseDownEvent(MouseEventArgs mevent)
        {
            if (MyMessageBox.ShowDialog("是否重置退藥資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;

            string 動作 = enum_交易記錄查詢動作.退藥回收.GetEnumName();
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 房名 = "";
            string 庫存量 = "";
            string 交易量 = "";
            string 結存量 = "";
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString();
            string 開方時間 = DateTime.Now.ToDateTimeString();
            string 操作人 = this.rJ_TextBox_登入者姓名.Text;
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
        }
        private void RJ_TextBox_入庫作業_藥品資料_藥品條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                List<object[]> list_藥品資料 = this.sqL_DataGridView_入庫作業_藥品資料.SQL_GetAllRows(false);
                List<object[]> list_藥品資料_buf = new List<object[]>();
                List<object[]> list_value = new List<object[]>();
                List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
                List<string> list_藥品碼 = new List<string>();
                list_藥品碼 = (from value in list_儲位資料
                            select value[(int)enum_儲位管理_儲位資料.藥品碼].ObjectToString()).Distinct().ToList();
                for (int i = 0; i < list_藥品碼.Count; i++)
                {
                    list_藥品資料_buf = list_藥品資料.GetRows((int)enum_參數設定_藥檔資料.藥品碼, list_藥品碼[i]);
                    if (list_藥品資料_buf.Count > 0)
                    {
                        object[] value = new object[new enum_入庫作業_藥品資料().GetEnumValues().Length];
                        value[(int)enum_入庫作業_藥品資料.藥品碼] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
                        value[(int)enum_入庫作業_藥品資料.藥品名稱] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
                        value[(int)enum_入庫作業_藥品資料.藥品中文名稱] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
                        value[(int)enum_入庫作業_藥品資料.藥品條碼] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.藥品條碼1].ObjectToString();
                        value[(int)enum_入庫作業_藥品資料.包裝單位] = list_藥品資料_buf[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
                        list_value.Add(value);
                    }
                }
                list_value = list_value.GetRows((int)enum_入庫作業_藥品資料.藥品條碼, rJ_TextBox_入庫作業_藥品資料_藥品條碼.Texts);
                if (list_value.Count == 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        MyMessageBox.ShowDialog("未搜尋到此條碼!");
                    }));
                    return;
                }
                this.sqL_DataGridView_入庫作業_藥品資料.RefreshGrid(list_value);
            }
        }
        private void PlC_RJ_Button_入庫作業_套餐資料_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.PLC_Device_入庫作業_選擇套餐.Bool = true;
        }
        private void PlC_RJ_Button_入庫作業_藥品資料_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.PLC_Device_入庫作業_選擇藥品.Bool = true;
        }
        private void PlC_RJ_Button_入庫作業_藥品資料_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Function_入庫作業_藥品資料更新DataGrid();
        }
        private void PlC_RJ_Button_入庫作業_取消作業_MouseDownEvent(MouseEventArgs mevent)
        {
            cnt_Program_入庫作業 = 65500;
        }

        #endregion
    }
}
