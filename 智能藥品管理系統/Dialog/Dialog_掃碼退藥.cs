using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
using MyUI;
using SQLUI;

namespace 智能藥品管理系統
{
    public enum enum_掃碼退藥
    {
        GUID,
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        包裝數量,
        數量,
        動作,
        庫存量,
        結存量,
    }
    public partial class Dialog_掃碼退藥 : Form
    {
        private MySerialPort mySerialPort;
        private MyThread MyThread_porgram;
        private MyTimer MyTimer_SannerTimeOut = new MyTimer();
        private MyTimer MyTimer_SannerRxDelay = new MyTimer();
        private SQLUI.SQL_DataGridView sQL_DataGridView_藥檔資料;
        private PLC_Device pLC_Device_抽屜感應;

        public List<object[]> Value
        {
            get
            {
                return this.sqL_DataGridView_退藥藥品.GetAllRows();
            }
        }

        public static Form form;
        public DialogResult ShowDialog()
        {
            if (form == null)
            {
                base.ShowDialog();
            }
            else
            {
                form.Invoke(new Action(delegate
                {
                    base.ShowDialog();
                }));
            }

            return this.DialogResult;
        }

        public Dialog_掃碼退藥(MySerialPort mySerialPort, PLC_Device pLC_Device_抽屜感應 , SQLUI.SQL_DataGridView sQL_DataGridView_藥檔資料)
        {
            InitializeComponent();
            this.mySerialPort = mySerialPort;
            this.pLC_Device_抽屜感應 = pLC_Device_抽屜感應;

            this.sqL_DataGridView_參數設定_藥檔資料.Init(sQL_DataGridView_藥檔資料);
            this.sqL_DataGridView_參數設定_藥檔資料.Set_ColumnWidth(100, "藥品碼");

        }

        private void Dialog_掃碼退藥_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;

            this.plC_Button_空瓶繳回.btnClick += PlC_Button_空瓶繳回_btnClick;
            this.plC_Button_實瓶繳回.btnClick += PlC_Button_實瓶繳回_btnClick;
            this.plC_Button_手輸退藥.btnClick += PlC_Button_手輸退藥_btnClick;

            this.rJ_Button_選擇藥品.MouseDownEvent += RJ_Button_選擇藥品_MouseDownEvent;

            this.MyThread_porgram = new MyThread();
            this.MyThread_porgram.Add_Method(sub_program);
            this.MyThread_porgram.Add_Method(this.plC_Button_空瓶繳回.Run);
            this.MyThread_porgram.Add_Method(this.plC_Button_實瓶繳回.Run);
            this.MyThread_porgram.Add_Method(this.plC_Button_手輸退藥.Run);

            this.MyThread_porgram.AutoRun(true);
            this.MyThread_porgram.SetSleepTime(100);
            this.MyThread_porgram.Trigger();
            MyTimer_SannerTimeOut.TickStop();
            MyTimer_SannerTimeOut.StartTickTime(1000);
            MyTimer_SannerRxDelay.TickStop();
            MyTimer_SannerRxDelay.StartTickTime(1000);
            PlC_Button_空瓶繳回_btnClick(null, null);

            this.sqL_DataGridView_退藥藥品.Init();
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
        }

   

        int cnt = 0;
        private string 藥品條碼 = "";
        private void sub_program()
        {
            if(pLC_Device_抽屜感應.Bool)
            {
                if(this.rJ_Lable_抽屜狀態.Text !="抽屜關閉")
                {
                    this.Invoke(new Action(delegate
                    {
                        this.rJ_Lable_抽屜狀態.Text = "抽屜關閉";
                    }));
                }
            }
            else
            {
                if (this.rJ_Lable_抽屜狀態.Text != "抽屜開啟")
                {
                    this.Invoke(new Action(delegate
                    {
                        this.rJ_Lable_抽屜狀態.Text = "抽屜開啟";
                    }));
                }
            }

        }

        #region Event
        private void RJ_Button_選擇藥品_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_藥檔資料 = this.sqL_DataGridView_參數設定_藥檔資料.Get_All_Select_RowsValues();

            if(list_藥檔資料.Count ==0)
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            string 藥品碼 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
            string 藥品名稱 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
            string 藥品中文名稱 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
            string 包裝單位 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
            string 動作 = this.plC_Button_空瓶繳回.Bool ? "空瓶繳回" : "實瓶繳回";
            if (this.plC_Button_手輸退藥.Bool)
            {
                動作 = "手輸退藥";
            }
        

            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
            int 數量 = dialog_NumPannel.Value;
            if (數量 == 0)
            {
                List<object[]> list_退藥藥品 = this.sqL_DataGridView_退藥藥品.GetRows((int)enum_掃碼退藥.藥品碼, 藥品碼, false);
                this.sqL_DataGridView_退藥藥品.DeleteExtra(list_退藥藥品, true);
            }
            else
            {
                List<object[]> list_退藥藥品 = this.sqL_DataGridView_退藥藥品.GetRows((int)enum_掃碼退藥.藥品碼, 藥品碼, false);
                list_退藥藥品 = list_退藥藥品.GetRows((int)enum_掃碼退藥.動作, 動作);

                object[] value = new object[new enum_掃碼退藥().GetLength()];
                if (list_退藥藥品.Count == 0)
                {
                    value[(int)enum_掃碼退藥.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_掃碼退藥.藥品碼] = 藥品碼;
                    value[(int)enum_掃碼退藥.藥品名稱] = 藥品名稱;
                    value[(int)enum_掃碼退藥.藥品中文名稱] = 藥品中文名稱;
                    value[(int)enum_掃碼退藥.包裝數量] = 包裝單位;
                    value[(int)enum_掃碼退藥.數量] = 數量.ToString();
                    value[(int)enum_掃碼退藥.動作] = 動作;
                    value[(int)enum_掃碼退藥.庫存量] = "0";
                    value[(int)enum_掃碼退藥.結存量] = "0";
                    this.sqL_DataGridView_退藥藥品.AddRow(value, true);
                }
                else
                {
                    value = list_退藥藥品[0];
                    value[(int)enum_掃碼退藥.藥品碼] = 藥品碼;
                    value[(int)enum_掃碼退藥.藥品名稱] = 藥品名稱;
                    value[(int)enum_掃碼退藥.藥品中文名稱] = 藥品中文名稱;
                    value[(int)enum_掃碼退藥.包裝數量] = 包裝單位;
                    value[(int)enum_掃碼退藥.數量] = 數量.ToString();
                    value[(int)enum_掃碼退藥.動作] = 動作;
                    value[(int)enum_掃碼退藥.庫存量] = "0";
                    value[(int)enum_掃碼退藥.結存量] = "0";
                    this.sqL_DataGridView_退藥藥品.ReplaceExtra(value, true);
                }
            }
        }
        private void PlC_Button_空瓶繳回_btnClick(object sender, EventArgs e)
        {
            this.plC_Button_空瓶繳回.Bool = true;
            this.plC_Button_實瓶繳回.Bool = false;
            this.plC_Button_手輸退藥.Bool = false;
        }
        private void PlC_Button_實瓶繳回_btnClick(object sender, EventArgs e)
        {
            this.plC_Button_空瓶繳回.Bool = false;
            this.plC_Button_實瓶繳回.Bool = true;
            this.plC_Button_手輸退藥.Bool = false;
        }
        private void PlC_Button_手輸退藥_btnClick(object sender, EventArgs e)
        {
            this.plC_Button_空瓶繳回.Bool = false;
            this.plC_Button_實瓶繳回.Bool = false;
            this.plC_Button_手輸退藥.Bool = true;

            //this.plC_Button_空瓶繳回.Enabled = false;
            //this.plC_Button_實瓶繳回.Enabled = false;

        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_退藥藥品 = this.sqL_DataGridView_退藥藥品.GetAllRows();
            List<object[]> list_退藥藥品_ADD = new List<object[]>();

            bool flag_alert = false;
            this.Invoke(new Action(delegate
            {
                for (int i = 0; i < list_退藥藥品.Count; i++)
                {
                    string 動作 = list_退藥藥品[i][(int)enum_掃碼退藥.動作].ObjectToString();
                    if (動作 == "手輸退藥")
                    {
                        if (flag_alert == false)
                        {
                            if (MyMessageBox.ShowDialog("※注意※ 選取手輸退藥會數量會扣除ADC庫存,是否繼續?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes)
                            {
                                flag_alert = true;
                                return;
                            }

                        }
                    }
                    else
                    {
                        list_退藥藥品_ADD.Add(list_退藥藥品[i]);
                        continue;
                    }
                    string 藥碼 = list_退藥藥品[i][(int)enum_掃碼退藥.藥品碼].ObjectToString();
                    string 藥名 = list_退藥藥品[i][(int)enum_掃碼退藥.藥品名稱].ObjectToString();
                    int 數量 = list_退藥藥品[i][(int)enum_掃碼退藥.數量].ObjectToString().StringToInt32();
                    數量 = 數量 * -1;
                    List<object[]> list_儲位資料 = Form1.Function_儲位管理_儲位資料_取得儲位資料(藥碼);
                    int 總庫存 = 0;
                    for (int k = 0; k < list_儲位資料.Count; k++)
                    {
                        總庫存 += list_儲位資料[k][(int)enum_儲位管理_儲位資料.總庫存].ObjectToString().StringToInt32();

                    }
                    if (總庫存 < 數量 * -1)
                    {
                        MyMessageBox.ShowDialog($"<{藥碼}> {藥名} 庫存不足無法退藥");
                        continue;
                    }
                    List<Form1.Class_取藥數組> list_Class_取藥數組 = Form1.Function_主畫面_取得取藥數組(list_儲位資料, 藥碼);
                    list_Class_取藥數組 = Form1.Function_主畫面_取藥數組運算(list_Class_取藥數組, 數量);
                    if (list_Class_取藥數組.Count == 0)
                    {
                        MyMessageBox.ShowDialog("找無最佳組合!!");
                        continue;
                    }
                    for (int k = 0; k < list_Class_取藥數組.Count; k++)
                    {
                        List<object[]> list_儲位資料_buf = list_儲位資料.GetRows((int)enum_儲位管理_儲位資料.IP, list_Class_取藥數組[i].IP);
                        if (list_儲位資料_buf.Count > 0)
                        {
                            Form1.Function_儲位管理_儲位資料_儲位資料庫存異動(list_儲位資料_buf[0], -1);
                            string IP = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                            int Port = list_儲位資料_buf[i][(int)enum_儲位管理_儲位資料.Port].ObjectToString().StringToInt32();
                            Form1._wT32_GPADC.Set_ScreenPageInit(IP, Port, false);
                            Form1._wT32_GPADC.Set_ToPage(IP, Port, (int)H_Pannel_lib.StorageUI_WT32.enum_Page.主頁面);
                            Form1._wT32_GPADC.Set_JsonStringSend(IP, Port);

                        }
                    }
                    list_退藥藥品[i][(int)enum_掃碼退藥.數量] = 數量;
                    list_退藥藥品[i][(int)enum_掃碼退藥.庫存量] = 總庫存;
                    list_退藥藥品[i][(int)enum_掃碼退藥.結存量] = 總庫存 + 數量;
                    list_退藥藥品_ADD.Add(list_退藥藥品[i]);
                }
          
                this.sqL_DataGridView_退藥藥品.RefreshGrid(list_退藥藥品_ADD);

                this.MyThread_porgram.Abort();
                this.MyThread_porgram = null;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.MyThread_porgram.Abort();
                this.MyThread_porgram = null;

                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
        #endregion
    }
}
