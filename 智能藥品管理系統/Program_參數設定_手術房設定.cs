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
    public enum enum_藥檔資料_手術房設定_手術房列表
    {
        GUID,
        代碼,
        名稱,
    }
    public partial class Form1 : Form
    {
  
        private void Program_參數設定_手術房設定_Init()
        {
            this.sqL_DataGridView_參數設定_手術房設定_手術房列表.Init();
            if(!this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_CreateTable();
            }
            this.sqL_DataGridView_參數設定_手術房設定_手術房列表.RowEnterEvent += SqL_DataGridView_參數設定_手術房設定_手術房列表_RowEnterEvent;
            this.plC_RJ_Button_參數設定_手術房設定_新增.MouseDownEvent += PlC_RJ_Button_參數設定_手術房設定_新增_MouseDownEvent;
            this.plC_RJ_Button_參數設定_手術房設定_刪除.MouseDownEvent += PlC_RJ_Button_參數設定_手術房設定_刪除_MouseDownEvent;
        }

  

        private bool flag_Program_參數設定_手術房設定 = false;
        private void Program_參數設定_手術房設定()
        {
            if (this.plC_ScreenPage_Main.PageText == "參數設定" && this.plC_ScreenPage_參數設定.PageText == "手術房設定")
            {
                if (!flag_Program_參數設定_手術房設定)
                {
                    this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_GetAllRows(true);
                    flag_Program_參數設定_手術房設定 = true;
                }
            }
            else
            {
                flag_Program_參數設定_手術房設定 = false;
            }
        }


        #region Event
        private void SqL_DataGridView_參數設定_手術房設定_手術房列表_RowEnterEvent(object[] RowValue)
        {
            string 代碼 = RowValue[(int)enum_藥檔資料_手術房設定_手術房列表.代碼].ObjectToString();
            string 名稱 = RowValue[(int)enum_藥檔資料_手術房設定_手術房列表.名稱].ObjectToString();
            rJ_TextBox_參數設定_手術房設定_代碼.Texts = 代碼;
            rJ_TextBox_參數設定_手術房設定_名稱.Texts = 名稱;
        }
        private void PlC_RJ_Button_參數設定_手術房設定_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_參數設定_手術房設定_手術房列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            this.Invoke(new Action(delegate
            {
                if (MyMessageBox.ShowDialog("是否刪除選取手術房?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_DeleteExtra(list_value, true);
                }
            }));
        }
        private void PlC_RJ_Button_參數設定_手術房設定_新增_MouseDownEvent(MouseEventArgs mevent)
        {
            string 代碼 = rJ_TextBox_參數設定_手術房設定_代碼.Texts;
            string 名稱 = rJ_TextBox_參數設定_手術房設定_名稱.Texts;
            if(代碼.StringIsEmpty())
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("代碼名稱不得為空白!");
                }));
                return;
            }
            List<object[]> list_value = this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_藥檔資料_手術房設定_手術房列表.代碼, 代碼);
            if (list_value.Count == 0)
            {
                object[] value = new object[new enum_藥檔資料_手術房設定_手術房列表().GetLength()];
                value[(int)enum_藥檔資料_手術房設定_手術房列表.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_藥檔資料_手術房設定_手術房列表.代碼] = 代碼;
                value[(int)enum_藥檔資料_手術房設定_手術房列表.名稱] = 名稱;
                this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_AddRow(value, true);
            }
            else
            {
                list_value[0][(int)enum_藥檔資料_手術房設定_手術房列表.名稱] = 名稱;
                this.sqL_DataGridView_參數設定_手術房設定_手術房列表.SQL_ReplaceExtra(list_value[0], true);
            }



        }
        #endregion
    }
}
