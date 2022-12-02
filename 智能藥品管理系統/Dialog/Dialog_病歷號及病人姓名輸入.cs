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
namespace 智能藥品管理系統
{
    public partial class Dialog_病歷號及病人姓名輸入 : Form
    {
        public string 病歷號
        {
            get
            {
                if (this.rJ_TextBox_病歷號.Texts.StringIsEmpty()) return "無";
                return this.rJ_TextBox_病歷號.Texts;
            }
        }
        public string 病人姓名
        {
            get
            {
                if (this.rJ_TextBox_病人姓名.Texts.StringIsEmpty()) return "無";
                return this.rJ_TextBox_病人姓名.Texts;
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

        public Dialog_病歷號及病人姓名輸入()
        {
            InitializeComponent();
        }

        private void Dialog_病歷號及病人姓名輸入_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
        }

        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
    }
}
