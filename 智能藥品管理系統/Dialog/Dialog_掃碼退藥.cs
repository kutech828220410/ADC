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


        }

        private void Dialog_掃碼退藥_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;

            this.plC_Button_空瓶繳回.btnClick += PlC_Button_空瓶繳回_btnClick;
            this.plC_Button_實瓶繳回.btnClick += PlC_Button_實瓶繳回_btnClick;

            this.rJ_Button_選擇藥品.MouseDownEvent += RJ_Button_選擇藥品_MouseDownEvent;

            this.MyThread_porgram = new MyThread();
            this.MyThread_porgram.Add_Method(sub_program);
            this.MyThread_porgram.Add_Method(this.plC_Button_空瓶繳回.Run);
            this.MyThread_porgram.Add_Method(this.plC_Button_實瓶繳回.Run);
         
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
            //if (cnt == 0)
            //{
            //    if(MyTimer_SannerTimeOut.IsTimeOut())
            //    {
            //        string readline = mySerialPort.ReadString();
            //        if (!readline.StringIsEmpty())
            //        {
            //            MyTimer_SannerRxDelay.TickStop();
            //            MyTimer_SannerRxDelay.StartTickTime(200);
            //            cnt++;
            //        }
            //    }
                
            //}
            //if (cnt == 2)
            //{
            //    if (MyTimer_SannerRxDelay.IsTimeOut())
            //    {
            //        string readline = mySerialPort.ReadString();
            //        if (!readline.StringIsEmpty())
            //        {
            //            readline = readline.Replace("\n", "");
            //            readline = readline.Replace("\r", "");
            //            藥品條碼 = readline;
                       
            //            mySerialPort.ClearReadByte();
            //            cnt++;
            //        }
            //    }
            //}
            //if (cnt == 3)
            //{
            //    List<object[]> list_藥檔資料 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
            //    list_藥檔資料 = list_藥檔資料.GetRows((int)enum_參數設定_藥檔資料.藥品條碼1, 藥品條碼);
            //    if (list_藥檔資料.Count == 0)
            //    {
            //        MyMessageBox.ShowDialog($"找無此藥品條碼 : {藥品條碼}");
            //        cnt = 65500;
            //        return;
            //    }
            //    cnt = 65500;
            //    return;
            //}
            //if (cnt == 65500)
            //{
            //    MyTimer_SannerTimeOut.TickStop();
            //    MyTimer_SannerTimeOut.StartTickTime(1000);
            //    cnt = 0;
            //}
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
                    this.sqL_DataGridView_退藥藥品.ReplaceExtra(value, true);
                }
            }
        }
        private void PlC_Button_空瓶繳回_btnClick(object sender, EventArgs e)
        {
            this.plC_Button_空瓶繳回.Bool = true;
            this.plC_Button_實瓶繳回.Bool = false;
        }
        private void PlC_Button_實瓶繳回_btnClick(object sender, EventArgs e)
        {
            this.plC_Button_空瓶繳回.Bool = false;
            this.plC_Button_實瓶繳回.Bool = true;
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.MyThread_porgram.Abort();
                this.MyThread_porgram = null;

                if (!pLC_Device_抽屜感應.Bool)
                {
                    MyMessageBox.ShowDialog("退藥抽屜未關上!");
                    return;
                }

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
