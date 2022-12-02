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
using SQLUI;
using MyUI;

namespace 智能藥品管理系統
{
    public partial class Dialog_退藥_藥品選擇 : Form
    {
        public object[] Value
        {
            get
            {
                return this.sqL_DataGridView_儲位藥品.GetRowValues();
            }
        }
        public int 數量 = 0;
        MyThread MyThread_Program;
        MySerialPort mySerialPort;
        private List<object[]> list_儲位資料 = new List<object[]>();
        private SQL_DataGridView sqL_DataGridView_入庫作業_藥品資料;
        public Dialog_退藥_藥品選擇(MySerialPort mySerialPort, List<object[]> list_儲位資料, SQL_DataGridView sqL_DataGridView_入庫作業_藥品資料)
        {
            InitializeComponent();
            this.sqL_DataGridView_入庫作業_藥品資料 = sqL_DataGridView_入庫作業_藥品資料;
            this.list_儲位資料 = list_儲位資料;
            this.mySerialPort = mySerialPort;
        }

        private void Dialog_退藥_藥品選擇_Load(object sender, EventArgs e)
        {
            this.sqL_DataGridView_儲位藥品.Init(this.sqL_DataGridView_入庫作業_藥品資料);
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;

            this.MyThread_Program = new MyThread();
            this.MyThread_Program.Add_Method(sub_porgram);
            this.MyThread_Program.SetSleepTime(100);
            this.MyThread_Program.AutoRun(true);
            this.MyThread_Program.Trigger();
        }

        private void sub_porgram()
        {
            string readline = this.mySerialPort.ReadString();
            if (!readline.StringIsEmpty())
            {
                readline = readline.Replace("\n", "");
                readline = readline.Replace("\r", "");

                List<object[]> list_藥品資料 = this.sqL_DataGridView_儲位藥品.SQL_GetAllRows(false);
                List<object[]> list_藥品資料_buf = new List<object[]>();
                List<object[]> list_value = new List<object[]>();
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
                list_value = list_value.GetRows((int)enum_入庫作業_藥品資料.藥品條碼, readline);
                this.sqL_DataGridView_儲位藥品.RefreshGrid(list_value);
                this.mySerialPort.ClearReadByte();
            }
        }

        private void Dialog_退藥_藥品選擇_Shown(object sender, EventArgs e)
        {
            this.Function_藥品資料更新DataGrid();
        }
        private void PlC_RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if(this.sqL_DataGridView_儲位藥品.Get_All_Select_RowsValues().Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇藥品!");
                    return;
                }
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                if (dialog_NumPannel.ShowDialog() == DialogResult.No) return;
                if (dialog_NumPannel.Value <= 0)
                {
                    MyMessageBox.ShowDialog("數量不得為'0'!");
                    return;
                }
                this.數量 = dialog_NumPannel.Value;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }

        private void Function_藥品資料更新DataGrid()
        {
            List<object[]> list_藥品資料 = this.sqL_DataGridView_儲位藥品.SQL_GetAllRows(false);
            List<object[]> list_藥品資料_buf = new List<object[]>();
            List<object[]> list_value = new List<object[]>();
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
            this.sqL_DataGridView_儲位藥品.RefreshGrid(list_value);
        }
        private void Dialog_退藥_藥品選擇_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MyThread_Program != null)
            {
                MyThread_Program.Abort();
                MyThread_Program = null;
            }
        }
    }
}
