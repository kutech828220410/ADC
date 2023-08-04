using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 智能藥品管理系統
{
    public enum enum_空瓶實瓶選擇
    {
        實瓶繳回,
        空瓶繳回,
    }

    public partial class Dialog_空瓶實瓶選擇 : Form
    {
        public enum_空瓶實瓶選擇 enum_空瓶實瓶選擇 = new enum_空瓶實瓶選擇();

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

        public Dialog_空瓶實瓶選擇()
        {
            InitializeComponent();
        }

        private void Dialog_空瓶實瓶選擇_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_空瓶.MouseDownEvent += PlC_RJ_Button_空瓶_MouseDownEvent;
            this.plC_RJ_Button_實瓶.MouseDownEvent += PlC_RJ_Button_實瓶_MouseDownEvent;
        }

        private void PlC_RJ_Button_實瓶_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.enum_空瓶實瓶選擇 = enum_空瓶實瓶選擇.實瓶繳回;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_空瓶_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.enum_空瓶實瓶選擇 = enum_空瓶實瓶選擇.空瓶繳回;
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
    }
}
