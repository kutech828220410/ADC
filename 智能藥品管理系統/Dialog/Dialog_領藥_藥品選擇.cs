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
    public enum enum_藥品選擇
    {
        GUID,
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        藥品條碼,
        包裝單位,
        數量
    }
    public partial class Dialog_領藥_藥品選擇 : Form
    {
        private SQL_DataGridView sqL_DataGridView_入庫作業_藥品資料;
        private List<object[]> list_儲位資料 = new List<object[]>();
        public List<object[]> Value
        {
            get
            {
                return this.sqL_DataGridView_選擇藥品.GetAllRows();
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

        public Dialog_領藥_藥品選擇(List<object[]> list_儲位資料, SQL_DataGridView  sqL_DataGridView_入庫作業_藥品資料)
        {
            InitializeComponent();

            this.list_儲位資料 = list_儲位資料;
            this.sqL_DataGridView_入庫作業_藥品資料 = sqL_DataGridView_入庫作業_藥品資料;
        }

        private void Dialog_領藥_藥品選擇_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;
            this.rJ_Button_選擇藥品.MouseDownEvent += RJ_Button_選擇藥品_MouseDownEvent;
            this.plC_RJ_Button_刪除選取藥品.MouseDownEvent += PlC_RJ_Button_刪除選取藥品_MouseDownEvent;
            this.sqL_DataGridView_儲位藥品.Init(sqL_DataGridView_入庫作業_藥品資料);
            this.sqL_DataGridView_選擇藥品.Init();

         
        }
        private void Dialog_領藥_藥品選擇_Shown(object sender, EventArgs e)
        {
            this.Function_藥品資料更新DataGrid();
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

        private void RJ_Button_選擇藥品_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_儲位藥品 = this.sqL_DataGridView_儲位藥品.Get_All_Select_RowsValues();
            if (list_儲位藥品.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("請選擇儲位藥品!");
                }));
            }
            int 數量 = 0;
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                dialogResult = dialog_NumPannel.ShowDialog();
                數量 = dialog_NumPannel.Value;
            }));
            if (dialogResult != DialogResult.Yes) return;
            string 藥品碼 = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.藥品碼].ObjectToString();
            List<object[]> list_選擇藥品 = this.sqL_DataGridView_選擇藥品.GetAllRows();
            List<object[]> list_選擇藥品_buf = list_選擇藥品.GetRows((int)enum_藥品選擇.藥品碼, 藥品碼);

            if (list_選擇藥品_buf.Count == 0)
            {
                object[] value = new object[new enum_藥品選擇().GetLength()];
                value[(int)enum_藥品選擇.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_藥品選擇.藥品碼] = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.藥品碼].ObjectToString();
                value[(int)enum_藥品選擇.藥品中文名稱] = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.藥品中文名稱].ObjectToString();
                value[(int)enum_藥品選擇.藥品名稱] = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.藥品名稱].ObjectToString();
                value[(int)enum_藥品選擇.藥品條碼] = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.藥品條碼].ObjectToString();
                value[(int)enum_藥品選擇.包裝單位] = list_儲位藥品[0][(int)enum_入庫作業_藥品資料.包裝單位].ObjectToString();
                value[(int)enum_藥品選擇.數量] = 數量;
                this.sqL_DataGridView_選擇藥品.AddRow(value, true);
            }
            else
            {
                list_選擇藥品_buf[0][(int)enum_藥品選擇.數量] = 數量;
                this.sqL_DataGridView_選擇藥品.ReplaceExtra(list_選擇藥品_buf[0], true);
            }


        }
        private void PlC_RJ_Button_刪除選取藥品_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_選擇藥品 = this.sqL_DataGridView_選擇藥品.Get_All_Select_RowsValues();
            this.sqL_DataGridView_選擇藥品.DeleteExtra(list_選擇藥品, true);
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
                if (Value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇藥品!");
                    return;
                }
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }

    }
}
