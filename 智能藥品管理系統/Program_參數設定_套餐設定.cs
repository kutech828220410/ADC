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
    public enum enum_藥檔資料_套餐設定_套餐列表
    {
        GUID,
        套餐代碼,
        套餐名稱,
    }
    public enum enum_藥檔資料_套餐設定_套餐內容
    {
        GUID,
        套餐代碼,
        藥品碼,
        藥品名稱,
        單位,
        數量
    }
    public partial class Form1 : Form
    {
    
        private void Program_參數設定_套餐設定_Init()
        {
            this.sqL_DataGridView_參數設定_套餐設定.Init(this.sqL_DataGridView_參數設定_藥檔資料);
            this.sqL_DataGridView_參數設定_套餐設定.Set_ColumnVisible(false, new enum_參數設定_藥檔資料().GetEnumNames());
            this.sqL_DataGridView_參數設定_套餐設定.Set_ColumnVisible(true, enum_參數設定_藥檔資料.藥品碼, enum_參數設定_藥檔資料.藥品名稱, enum_參數設定_藥檔資料.藥品中文名稱, enum_參數設定_藥檔資料.包裝單位);
            this.sqL_DataGridView_參數設定_套餐設定.Set_ColumnWidth(350, enum_參數設定_藥檔資料.藥品名稱);
            this.sqL_DataGridView_參數設定_套餐設定.Set_ColumnWidth(350, enum_參數設定_藥檔資料.藥品中文名稱);

            this.plC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品碼搜尋.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品碼搜尋_MouseDownEvent;
            this.plC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品名稱搜尋.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品名稱搜尋_MouseDownEvent;
            this.plC_RJ_Button_參數設定_套餐設定_藥品搜尋_中文名稱搜尋.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_中文名稱搜尋_MouseDownEvent;
            this.plC_RJ_Button_參數設定_套餐設定_填入資料.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_填入資料_MouseDownEvent;
            this.plC_RJ_Button_藥檔資料_套餐設定_刪除.MouseDownEvent += PlC_RJ_Button_藥檔資料_套餐設定_刪除_MouseDownEvent;

            this.rJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品碼.KeyPress += RJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品碼_KeyPress;
            this.rJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品名稱.KeyPress += RJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品名稱_KeyPress;
            this.rJ_TextBox_參數設定_套餐設定_藥品搜尋_中文名稱.KeyPress += RJ_TextBox_參數設定_套餐設定_藥品搜尋_中文名稱_KeyPress;

            this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.Init();
            if(!this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_CreateTable();
            }
            this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.RowEnterEvent += SqL_DataGridView_藥檔資料_套餐設定_套餐列表_RowEnterEvent;


            this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.Init();
            if (!this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_CreateTable();
            }

            
            this.plC_RJ_Button_參數設定_套餐設定_套餐列表_新增.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_套餐列表_新增_MouseDownEvent;
            this.plC_RJ_Button_參數設定_套餐設定_套餐列表_刪除.MouseDownEvent += PlC_RJ_Button_參數設定_套餐設定_套餐列表_刪除_MouseDownEvent;
            this.plC_RJ_Button_藥檔資料_套餐設定_修改數量.MouseDownEvent += PlC_RJ_Button_藥檔資料_套餐設定_修改數量_MouseDownEvent;

        }

   

        private bool flag_Program_參數設定_套餐設定 = false;
        private void Program_參數設定_套餐設定()
        {
            if (this.plC_ScreenPage_Main.PageText == "參數設定" && this.plC_ScreenPage_參數設定.PageText == "套餐設定")
            {
                if (!flag_Program_參數設定_套餐設定)
                {
                    this.sqL_DataGridView_參數設定_套餐設定.SQL_GetAllRows(true);
                    this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_GetAllRows(true);
                    flag_Program_參數設定_套餐設定 = true;
                }
            }
            else
            {
                flag_Program_參數設定_套餐設定 = false;
            }
        }

        #region Function

        #endregion
        #region Event
        private void SqL_DataGridView_藥檔資料_套餐設定_套餐列表_RowEnterEvent(object[] RowValue)
        {
            string Code = RowValue[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            List<object[]> list_value = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼, Code);
            this.rJ_TextBox_參數設定_套餐設定_套餐列表_套餐代碼.Texts = RowValue[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            this.rJ_TextBox_參數設定_套餐設定_套餐列表_套餐名稱.Texts = RowValue[(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱].ObjectToString();
            this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_參數設定_套餐設定_套餐列表_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            if (MyMessageBox.ShowDialog("是否刪除此套餐設定?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                string 套餐代碼 = list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
                this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_DeleteExtra(list_value, true);
            }
        }
        private void PlC_RJ_Button_參數設定_套餐設定_套餐列表_新增_MouseDownEvent(MouseEventArgs mevent)
        {
            string Code = rJ_TextBox_參數設定_套餐設定_套餐列表_套餐代碼.Texts;
            if (Code.StringIsEmpty()) return;
            List<object[]> list_value = this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_GetAllRows(true);
            List<object[]> list_藥品資料 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
            list_藥品資料 = list_藥品資料.GetRows((int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼, Code);
            if(list_藥品資料.Count > 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("此套餐代碼與藥品碼有重複!");
                }));
            }
            list_value = list_value.GetRows((int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼, Code);
            if (list_value.Count == 0)
            {
                object[] value = new object[new enum_藥檔資料_套餐設定_套餐列表().GetLength()];
                value[(int)enum_藥檔資料_套餐設定_套餐列表.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼] = rJ_TextBox_參數設定_套餐設定_套餐列表_套餐代碼.Texts;
                value[(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱] = rJ_TextBox_參數設定_套餐設定_套餐列表_套餐名稱.Texts;

                this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_AddRow(value, true);
            }
            else
            {
                string GUID = list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.GUID].ObjectToString();
                list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼] = rJ_TextBox_參數設定_套餐設定_套餐列表_套餐代碼.Texts;
                list_value[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐名稱] = rJ_TextBox_參數設定_套餐設定_套餐列表_套餐名稱.Texts;

                this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.SQL_ReplaceExtra(list_value, true);
            }


        }
        private void RJ_TextBox_參數設定_套餐設定_藥品搜尋_中文名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_中文名稱搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品名稱_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品名稱搜尋_MouseDownEvent(null);
            }
        }
        private void RJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品碼搜尋_MouseDownEvent(null);
            }
        }
        private void PlC_RJ_Button_參數設定_套餐設定_填入資料_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_套餐列表 = this.sqL_DataGridView_藥檔資料_套餐設定_套餐列表.Get_All_Select_RowsValues();
            List<object[]> list_藥檔資料 = this.sqL_DataGridView_參數設定_套餐設定.Get_All_Select_RowsValues();
            List<object[]> list_套餐內容 = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_GetAllRows(true);
            if (list_套餐列表.Count == 0)
            {
                this.Invoke(new Action(delegate 
                {
                    MyMessageBox.ShowDialog("未選擇套餐!");
                }));
                return;
            }
            if (list_藥檔資料.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("未選擇藥品!");
                }));
                return;
            }
            string 套餐代碼 = list_套餐列表[0][(int)enum_藥檔資料_套餐設定_套餐列表.套餐代碼].ObjectToString();
            string 藥品碼 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
            string 藥品名稱 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
            string 單位 = list_藥檔資料[0][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
            list_套餐內容 = list_套餐內容.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼, 套餐代碼);
            list_套餐內容 = list_套餐內容.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.藥品碼, 藥品碼);
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate 
            {
                dialogResult = dialog_NumPannel.ShowDialog();
            }));

            if (dialogResult != DialogResult.Yes) return;
            if (dialog_NumPannel.Value == 0) return;
            string 數量 = dialog_NumPannel.Value.ToString();
            if (list_套餐內容.Count == 0)
            {
                object[] value = new object[new enum_藥檔資料_套餐設定_套餐內容().GetLength()];
                value[(int)enum_藥檔資料_套餐設定_套餐內容.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼] = 套餐代碼;
                value[(int)enum_藥檔資料_套餐設定_套餐內容.藥品碼] = 藥品碼;
                value[(int)enum_藥檔資料_套餐設定_套餐內容.藥品名稱] = 藥品名稱;
                value[(int)enum_藥檔資料_套餐設定_套餐內容.單位] = 單位;
                value[(int)enum_藥檔資料_套餐設定_套餐內容.數量] = 數量;

                this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_AddRow(value, true);

            }
            else
            {
                list_套餐內容[0][(int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼] = 套餐代碼;
                list_套餐內容[0][(int)enum_藥檔資料_套餐設定_套餐內容.藥品碼] = 藥品碼;
                list_套餐內容[0][(int)enum_藥檔資料_套餐設定_套餐內容.藥品名稱] = 藥品名稱;
                list_套餐內容[0][(int)enum_藥檔資料_套餐設定_套餐內容.單位] = 單位;
                list_套餐內容[0][(int)enum_藥檔資料_套餐設定_套餐內容.數量] = 數量;

                this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_ReplaceExtra(list_套餐內容[0], true);
            }
        }
        private void PlC_RJ_Button_藥檔資料_套餐設定_修改數量_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            DialogResult dialogResult = DialogResult.None;
            this.Invoke(new Action(delegate
            {
                dialogResult = dialog_NumPannel.ShowDialog();
            }));

            if (dialogResult != DialogResult.Yes) return;
            if (dialog_NumPannel.Value == 0) return;
            string 數量 = dialog_NumPannel.Value.ToString();

            list_value[0][(int)enum_藥檔資料_套餐設定_套餐內容.數量] = 數量;
            this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_ReplaceExtra(list_value[0], true);
        }
        private void PlC_RJ_Button_藥檔資料_套餐設定_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.Get_All_Select_RowsValues();
            if (list_value.Count == 0) return;
            if (MyMessageBox.ShowDialog("是否刪除選取套餐內容?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_DeleteExtra(list_value, true);
            }
        }
        private void PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品碼搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string value = rJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品碼.Texts;
            List<object[]> list_value = this.sqL_DataGridView_參數設定_套餐設定.SQL_GetAllRows(false);
            list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品碼, value);
            this.sqL_DataGridView_參數設定_套餐設定.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_藥品名稱搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string value = rJ_TextBox_參數設定_套餐設定_藥品搜尋_藥品名稱.Texts;
            List<object[]> list_value = this.sqL_DataGridView_參數設定_套餐設定.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_參數設定_藥檔資料.藥品名稱, value);
            this.sqL_DataGridView_參數設定_套餐設定.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_參數設定_套餐設定_藥品搜尋_中文名稱搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            string value = rJ_TextBox_參數設定_套餐設定_藥品搜尋_中文名稱.Texts;
            List<object[]> list_value = this.sqL_DataGridView_參數設定_套餐設定.SQL_GetAllRows(false);
            list_value = list_value.GetRowsByLike((int)enum_參數設定_藥檔資料.藥品中文名稱, value);
            this.sqL_DataGridView_參數設定_套餐設定.RefreshGrid(list_value);
        }
        #endregion
    }
}
