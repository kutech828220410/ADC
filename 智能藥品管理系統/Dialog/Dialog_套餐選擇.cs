using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLUI;
using Basic;
namespace 智能藥品管理系統
{
    
    public partial class Dialog_套餐選擇 : Form
    {
        public object[] Value;
        
        private SQL_DataGridView sQL_DataGridView_套餐列表;
        private SQL_DataGridView sQL_DataGridView_套餐內容;

        public Dialog_套餐選擇(SQL_DataGridView sQL_DataGridView_套餐列表 , SQL_DataGridView sQL_DataGridView_套餐內容)
        {
            InitializeComponent();
            this.sQL_DataGridView_套餐列表 = sQL_DataGridView_套餐列表;
            this.sQL_DataGridView_套餐內容 = sQL_DataGridView_套餐內容;
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
        private void Dialog_套餐選擇_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;

            this.sqL_DataGridView_套餐列表.Init(sQL_DataGridView_套餐列表);
            this.sqL_DataGridView_套餐列表.RowEnterEvent += SqL_DataGridView_套餐列表_RowEnterEvent;
            this.sqL_DataGridView_套餐內容.Init(sQL_DataGridView_套餐內容);

            this.sqL_DataGridView_套餐列表.SQL_GetAllRows(true);
        }

        private void SqL_DataGridView_套餐列表_RowEnterEvent(object[] RowValue)
        {
            string 套餐代碼 = RowValue[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            List<object[]> list_value = this.sqL_DataGridView_套餐內容.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼, 套餐代碼);
            this.sqL_DataGridView_套餐內容.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                List<object[]> list_value = this.sqL_DataGridView_套餐列表.Get_All_Select_RowsValues();
                if(list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇套餐!");
                    return;
                }
                Value = list_value[0];
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }));
        }
    }
}
