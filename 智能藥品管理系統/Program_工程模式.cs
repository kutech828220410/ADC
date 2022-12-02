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
namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {

        private void Program_工程模式_Init()
        {
       
        }
        private bool flag_Program_工程模式 = false;
        private void Program_工程模式()
        {
            if (this.plC_ScreenPage_Main.PageText == "工程模式")
            {
                if (!flag_Program_工程模式)
                {
                    this.Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.操作工程模式, this.rJ_TextBox_登入者姓名.Text , "");
                    flag_Program_工程模式 = true;
                }
            }
            else
            {
                flag_Program_工程模式 = false;
            }
      
        }

        #region Event
    
        #endregion
    }
}
