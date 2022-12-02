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
    public enum enum_功能選擇
    {
        None,
        套餐,
        藥品,
    }
  
    public partial class Dialog_功能選擇 : Form
    {
        public enum_功能選擇 Value = enum_功能選擇.None;
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
        public Dialog_功能選擇()
        {
            InitializeComponent();
        }

        private void Dialog_功能選擇_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_套餐.MouseDownEvent += PlC_RJ_Button_套餐_MouseDownEvent;
            this.plC_RJ_Button_藥品.MouseDownEvent += PlC_RJ_Button_藥品_MouseDownEvent;

        }

        private void PlC_RJ_Button_藥品_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Value = enum_功能選擇.藥品;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_套餐_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                Value = enum_功能選擇.套餐;
                this.Close();
            }));
        }
    }
}
