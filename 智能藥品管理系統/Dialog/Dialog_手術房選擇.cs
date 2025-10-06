using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 智能藥品管理系統;
using Basic;
namespace 智能藥品管理系統
{
    public partial class Dialog_手術房選擇 : Form
    {
        public object[] Value;
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
        private SQLUI.SQL_DataGridView sQL_DataGridView;
        public Dialog_手術房選擇(SQLUI.SQL_DataGridView sQL_DataGridView)
        {
            InitializeComponent();
            this.sQL_DataGridView = sQL_DataGridView;
        }
        private void Dialog_手術房選擇_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;

            this.sqL_DataGridView_手術房列表.Init(sQL_DataGridView);
            List<object[]> list_var = this.sqL_DataGridView_手術房列表.SQL_GetAllRows(false);

            list_var.Sort(new Form1.ICP_operation_room());

            this.Value = new object[new enum_藥檔資料_手術房設定_手術房列表().GetLength()];
        }

        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<object[]> list_value = this.sqL_DataGridView_手術房列表.Get_All_Select_RowsValues();
                if (list_value.Count == 0)
                {
                    MyMessageBox.ShowDialog("未選擇手術房!");
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
