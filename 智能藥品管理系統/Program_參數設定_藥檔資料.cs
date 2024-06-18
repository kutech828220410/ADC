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
    public enum enum_參數設定_藥檔資料
    {
        GUID,
        藥品碼,
        藥品名稱,
        藥品中文名稱,
        藥品條碼1,
        包裝單位,
        庫存,
        安全庫存,
    }
    public partial class Form1 : Form
    {
       

        private void Program_參數設定_藥檔資料_Init()
        {
            this.sqL_DataGridView_參數設定_藥檔資料.Init();
            if (!this.sqL_DataGridView_參數設定_藥檔資料.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_參數設定_藥檔資料.SQL_CreateTable();
            }
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
            this.sqL_DataGridView_參數設定_藥檔資料.RowDoubleClickEvent += SqL_DataGridView_參數設定_藥檔資料_RowDoubleClickEvent;
            this.sqL_DataGridView_參數設定_藥檔資料.DataGridRowsChangeEvent += SqL_DataGridView_參數設定_藥檔資料_DataGridRowsChangeEvent;
            this.sqL_DataGridView_參數設定_藥檔資料.DataGridRefreshEvent += SqL_DataGridView_參數設定_藥檔資料_DataGridRefreshEvent;

            this.rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.KeyPress += RJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼_KeyPress;

            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_匯入.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_匯入_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_匯出.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_匯出_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_清除內容.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_清除內容_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_刪除.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_刪除_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_登錄.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_登錄_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示全部.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示全部_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_搜尋條件_搜尋.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_搜尋_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示有儲位藥品.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示有儲位藥品_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_資料內容_檢查藥檔.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_資料內容_檢查藥檔_MouseDownEvent;
            this.plC_RJ_Button_參數設定_藥檔資料_自動填入.MouseDownEvent += PlC_RJ_Button_參數設定_藥檔資料_自動填入_MouseDownEvent;
        }

     

        private bool flag_Program_參數設定_藥檔資料 = false;
        private void Program_參數設定_藥檔資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "參數設定" && this.plC_ScreenPage_參數設定.PageText == "藥檔資料")
            {
                string readline = this.MySerialPort_Scanner.ReadString();
                if (!readline.StringIsEmpty())
                {
                    this.Invoke(new Action(delegate
                    {
                        if(rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.IsFocused)
                        {
                            readline = readline.Replace("\n", "");
                            readline = readline.Replace("\r", "");
                            rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.Texts = readline;
                            this.RJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼_KeyPress(null, new KeyPressEventArgs((char)Keys.Enter));
                        }
                        else
                        {
                            readline = readline.Replace("\n", "");
                            readline = readline.Replace("\r", "");
                            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Texts = readline;
                        }
                        this.MySerialPort_Scanner.ClearReadByte();
                    }));
                }

                if (!flag_Program_參數設定_藥檔資料)
                {
                    this.MySerialPort_Scanner.ClearReadByte();
                    // this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
                    flag_Program_參數設定_藥檔資料 = true;
                }
            }
            else
            {
                flag_Program_參數設定_藥檔資料 = false;
            }
        }
        private string Function_參數設定_藥檔資料_檢查內容(object[] value)
        {
            string str_error = "";
            List<string> list_error = new List<string>();
            if (value[(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'藥品碼'欄位不得空白!");
            }
            if (value[(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'藥品名稱'欄位不得空白!");
            }
       
            if (value[(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'包裝單位'欄位不得空白!");
            }
            if (value[(int)enum_參數設定_藥檔資料.庫存].ObjectToString().StringToInt32() < 0)
            {
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_庫存.Text = "0";
            }
            if (value[(int)enum_參數設定_藥檔資料.安全庫存].ObjectToString().StringToInt32() < 0)
            {
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_安全庫存.Text = "0";
            }
            for (int i = 0; i < list_error.Count; i++)
            {
                str_error += $"{(i + 1).ToString("00")}. {list_error[i]}";
                if (i != list_error.Count - 1) str_error += "\n";
            }
            return str_error;
        }
        private void Function_參數設定_藥檔資料_清除內容()
        {
            this.Invoke(new Action(delegate 
            {
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品碼.Text = "";
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品名稱.Text = "";
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品中文名稱.Text = "";
         
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Text = "";
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_庫存.Text = "";
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_安全庫存.Text = "";
                this.rJ_TextBox_參數設定_藥檔資料_資料內容_包裝單位.Text = "";
            }));
          
        }
        #region Event
        private void RJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                List<object[]> list_value = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品條碼1, rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.Texts);
                if (list_value.Count == 0)
                {
                    this.Invoke(new Action(delegate 
                    {
                        MyMessageBox.ShowDialog("未搜尋到此條碼!");
                    }));
                    return;
                }
                this.sqL_DataGridView_參數設定_藥檔資料.RefreshGrid(list_value);
            }
        }
        private void SqL_DataGridView_參數設定_藥檔資料_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
         
            List<object[]> list_套餐內容 = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_GetAllRows(false);
            List<object[]> list_套餐內容_buf = new List<object[]>();
            List<object[]> list_儲位資訊 = Function_儲位管理_儲位資料_取得儲位資料(false);
            List<object[]> list_儲位資訊_buf = new List<object[]>();
            string 藥品碼 = "";
            string 套餐代碼 = "";
            int 套餐內容數量 = 0;
            int 儲位庫存數量 = 0;
            int 庫存 = 0;
            for (int i = 0; i < RowsList.Count; i++)
            {
                庫存 = 0;
                藥品碼 = RowsList[i][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
                list_儲位資訊_buf = list_儲位資訊.GetRows((int)enum_儲位管理_儲位資料.藥品碼, 藥品碼);
                for (int k = 0; k < list_儲位資訊_buf.Count; k++)
                {
                    庫存 += list_儲位資訊_buf[k][(int)enum_儲位管理_儲位資料.庫存].StringToInt32() * list_儲位資訊_buf[k][(int)enum_儲位管理_儲位資料.單位包裝數量].StringToInt32();
                }
                list_套餐內容_buf = list_套餐內容.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.藥品碼, 藥品碼);
                for (int k = 0; k < list_套餐內容_buf.Count; k++)
                {
                    套餐代碼 = list_套餐內容_buf[k][(int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼].ObjectToString();
                    套餐內容數量 = list_套餐內容_buf[k][(int)enum_藥檔資料_套餐設定_套餐內容.數量].ObjectToString().StringToInt32();
                    list_儲位資訊_buf = list_儲位資訊.GetRows((int)enum_儲位管理_儲位資料.藥品碼, 套餐代碼);
                    for (int m = 0; m < list_儲位資訊_buf.Count; m++)
                    {
                        儲位庫存數量 = list_儲位資訊_buf[m][(int)enum_儲位管理_儲位資料.庫存].ObjectToString().StringToInt32();
                        庫存 += 儲位庫存數量 * 套餐內容數量;
                    }
                }

                RowsList[i][(int)enum_參數設定_藥檔資料.庫存] = 庫存.ToString();
            }
            RowsList.Sort(new ICP_參數設定_藥檔資料());

        }
        private void SqL_DataGridView_參數設定_藥檔資料_RowDoubleClickEvent(object[] RowValue)
        {
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品碼.Text = RowValue[(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品名稱.Text = RowValue[(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品中文名稱.Text = RowValue[(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Text = RowValue[(int)enum_參數設定_藥檔資料.藥品條碼1].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_庫存.Text = RowValue[(int)enum_參數設定_藥檔資料.庫存].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_安全庫存.Text = RowValue[(int)enum_參數設定_藥檔資料.安全庫存].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_包裝單位.Text = RowValue[(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString();
        }
        private void SqL_DataGridView_參數設定_藥檔資料_DataGridRefreshEvent()
        {
            int 庫存 = 0;
            int 安全庫存 = 0;
            for (int i = 0; i < this.sqL_DataGridView_參數設定_藥檔資料.dataGridView.Rows.Count; i++)
            {
                庫存 = this.sqL_DataGridView_參數設定_藥檔資料.dataGridView.Rows[i].Cells[(int)enum_參數設定_藥檔資料.庫存].Value.ToString().StringToInt32();
                安全庫存 = this.sqL_DataGridView_參數設定_藥檔資料.dataGridView.Rows[i].Cells[(int)enum_參數設定_藥檔資料.安全庫存].Value.ToString().StringToInt32();
                if (庫存 < 安全庫存 && 安全庫存 != 0)
                {
                    this.sqL_DataGridView_參數設定_藥檔資料.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    this.sqL_DataGridView_參數設定_藥檔資料.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }

            }
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<object[]> list_value_result = new List<object[]>();
            List<List<object[]>> list_list_value = new List<List<object[]>>();
            if (!this.rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品碼.Text.StringIsEmpty())
            {
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品碼, rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品碼.Text);
                // list_list_value.Add(list_value_buf);
            }
            if (!this.rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品名稱.Text.StringIsEmpty())
            {
                list_value = list_value.GetRowsByLike((int)enum_參數設定_藥檔資料.藥品名稱, rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品名稱.Text);
                //list_list_value.Add(list_value_buf);
            }
          
       
        
            if (!this.rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.Text.StringIsEmpty())
            {
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.藥品條碼1, rJ_TextBox_參數設定_藥檔資料_搜尋條件_藥品條碼.Text);
                // list_list_value.Add(list_value_buf);
            }
            if (!this.rJ_TextBox_參數設定_藥檔資料_搜尋條件_包裝單位.Text.StringIsEmpty())
            {
                list_value = list_value.GetRows((int)enum_參數設定_藥檔資料.包裝單位, rJ_TextBox_參數設定_藥檔資料_搜尋條件_包裝單位.Text);
                // list_list_value.Add(list_value_buf);
            }
            //for (int i = 0; i < list_list_value.Count; i++)
            //{
            //    for (int k = 0; k < list_list_value[i].Count; k++)
            //    {
            //        list_value_result.Add(list_list_value[i][k]);
            //    }
            //}

            this.sqL_DataGridView_參數設定_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示有儲位藥品_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_藥檔資料 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_套餐內容 = this.sqL_DataGridView_藥檔資料_套餐設定_套餐內容.SQL_GetAllRows(false);
            List<object[]> list_套餐內容_buf = new List<object[]>();
            List<object[]> list_藥檔資料_buf = new List<object[]>();
            List<object[]> list_儲位資料 = Function_儲位管理_儲位資料_取得儲位資料(false);
            List<object[]> list_value = new List<object[]>();
            List<string> list_藥品碼 = (from value in list_儲位資料
                                     where value[(int)enum_儲位管理_儲位資料.包裝單位].ObjectToString() != "Package"
                                     select value[(int)enum_儲位管理_儲位資料.藥品碼].ObjectToString()).Distinct().ToList();

            List<string> list_套餐代碼 = (from value in list_儲位資料
                                      where value[(int)enum_儲位管理_儲位資料.包裝單位].ObjectToString() == "Package"
                                      select value[(int)enum_儲位管理_儲位資料.藥品碼].ObjectToString()).Distinct().ToList();
            for (int i = 0; i < list_套餐代碼.Count; i++)
            {
                list_套餐內容_buf = list_套餐內容.GetRows((int)enum_藥檔資料_套餐設定_套餐內容.套餐代碼, list_套餐代碼[i]);
                for (int k = 0; k < list_套餐內容_buf.Count; k++)
                {
                    list_藥品碼.Add(list_套餐內容_buf[k][(int)enum_藥檔資料_套餐設定_套餐內容.藥品碼].ObjectToString());                  
                }
            }
            list_藥品碼 = list_藥品碼.Distinct().ToList();
            for (int i = 0; i < list_藥品碼.Count; i++)
            {
                list_藥檔資料_buf = list_藥檔資料.GetRows((int)enum_參數設定_藥檔資料.藥品碼, list_藥品碼[i]);
                if(list_藥檔資料_buf.Count > 0)
                {
                    list_value.Add(list_藥檔資料_buf[0]);
                }
            }
            this.sqL_DataGridView_參數設定_藥檔資料.RefreshGrid(list_value);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_搜尋條件_顯示全部_MouseDownEvent(MouseEventArgs mevent)
        {
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_登錄_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_values = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_valuse_buf = new List<object[]>();
            list_valuse_buf = (from value in list_values
                               where value[(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString() == this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品碼.Text
                               select value).ToList();
            if (list_valuse_buf.Count == 0)
            {
                object[] value = new object[new enum_參數設定_藥檔資料().GetEnumNames().Length];
                value[(int)enum_參數設定_藥檔資料.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_參數設定_藥檔資料.藥品碼] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品碼.Text;
                value[(int)enum_參數設定_藥檔資料.藥品名稱] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品名稱.Text;
                value[(int)enum_參數設定_藥檔資料.藥品中文名稱] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品中文名稱.Text;
                value[(int)enum_參數設定_藥檔資料.藥品條碼1] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Text;
                value[(int)enum_參數設定_藥檔資料.包裝單位] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_包裝單位.Text;
                value[(int)enum_參數設定_藥檔資料.庫存] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_庫存.Text;
                value[(int)enum_參數設定_藥檔資料.安全庫存] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_安全庫存.Text;
                string str_error = this.Function_參數設定_藥檔資料_檢查內容(value);
                if (!str_error.StringIsEmpty())
                {
                    this.Invoke(new Action(delegate
                    {
                        MyMessageBox.ShowDialog(str_error);
                    }));
            
                    return;
                }
                this.sqL_DataGridView_參數設定_藥檔資料.SQL_AddRow(value, false);
            }
            else
            {
                object[] value = list_valuse_buf[0];
                string GUID = value[(int)enum_參數設定_藥檔資料.GUID].ObjectToString();
                value[(int)enum_參數設定_藥檔資料.藥品名稱] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品名稱.Text;
                value[(int)enum_參數設定_藥檔資料.藥品中文名稱] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品中文名稱.Text;
                value[(int)enum_參數設定_藥檔資料.藥品條碼1] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Text;
                value[(int)enum_參數設定_藥檔資料.包裝單位] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_包裝單位.Text;
                value[(int)enum_參數設定_藥檔資料.庫存] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_庫存.Text;
                value[(int)enum_參數設定_藥檔資料.安全庫存] = this.rJ_TextBox_參數設定_藥檔資料_資料內容_安全庫存.Text;
                string str_error = this.Function_參數設定_藥檔資料_檢查內容(value);
                if (!str_error.StringIsEmpty())
                {
                    this.Invoke(new Action(delegate
                    {
                        MyMessageBox.ShowDialog(str_error);
                    }));
                    return;
                }
                this.sqL_DataGridView_參數設定_藥檔資料.SQL_Replace(enum_參數設定_藥檔資料.GUID.GetEnumName(), GUID, value, false);
            }
            this.Function_參數設定_藥檔資料_清除內容();
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_參數設定_藥檔資料.Get_All_Select_RowsValues();
            if (list_value.Count == 0) MyMessageBox.ShowDialog("未選取資料!");
            if (MyMessageBox.ShowDialog($"是否刪除選取資料‧共{list_value.Count}筆", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
            {
                string GUID = "";
                for (int i = 0; i < list_value.Count; i++)
                {
                    GUID = list_value[i][(int)enum_參數設定_藥檔資料.GUID].ObjectToString();
                    this.sqL_DataGridView_參數設定_藥檔資料.SQL_Delete(enum_參數設定_藥檔資料.GUID.GetEnumName(), GUID, false);
                }
            }
            this.Function_人員資料_清除內容();
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_清除內容_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Function_參數設定_藥檔資料_清除內容();
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
                {
                    DataTable dataTable = this.sqL_DataGridView_參數設定_藥檔資料.GetDataTable().DeepClone();
                    CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
                    this.Invoke(new Action(delegate
                    {
                        MyMessageBox.ShowDialog("匯出完成!");
                    }));

                }
            }));
      
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.openFileDialog_LoadExcel.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    DataTable dataTable = new DataTable();
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_參數設定_藥檔資料());
                    if (datatable_buf == null)
                    {
                        this.Cursor = Cursors.Default;
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        return;
                    }
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
                    List<object[]> list_Add = new List<object[]>();
                    List<object[]> list_Delete_ColumnName = new List<object[]>();
                    List<object[]> list_Delete_SerchValue = new List<object[]>();
                    List<string[]> list_Replace_SerchValue = new List<string[]>();
                    List<object[]> list_Replace_Value = new List<object[]>();
                    List<object[]> list_Value_buf = new List<object[]>();
                    bool flag_replace = false;
                    foreach (System.Data.DataRow dr in datatable_buf.Rows)
                    {
                        flag_replace = false;
                        string 藥品碼 = dr[enum_參數設定_藥檔資料.藥品碼.GetEnumName()].ObjectToString();
                        object[] src_obj = new string[new enum_參數設定_藥檔資料().GetEnumNames().Length];
                        list_Value_buf = (from value in list_SQL_Value
                                          where value[(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString() == 藥品碼
                                          select value).ToList();
                        if (list_Value_buf.Count != 0) flag_replace = true;


                        if (flag_replace) src_obj = list_Value_buf[0];

                        for (int i = 0; i < src_obj.Length; i++)
                        {
                            if (i == (int)enum_參數設定_藥檔資料.GUID)
                            {
                                if (!flag_replace) src_obj[(int)enum_參數設定_藥檔資料.GUID] = Guid.NewGuid().ToString();
                                continue;
                            }
                            src_obj[i] = dr[i].ObjectToString();
                        }

                        string str_error = this.Function_參數設定_藥檔資料_檢查內容(src_obj);
                        if (!str_error.StringIsEmpty())
                        {
                            continue;
                        }

                        if (list_Value_buf.Count == 0) list_Add.Add(src_obj);
                        else
                        {
                            list_Replace_SerchValue.Add((new string[] { src_obj[(int)enum_參數設定_藥檔資料.GUID].ObjectToString() }));
                            list_Replace_Value.Add(src_obj);
                        }
                    }
                    this.sqL_DataGridView_參數設定_藥檔資料.SQL_ReplaceExtra(enum_參數設定_藥檔資料.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
                    this.sqL_DataGridView_參數設定_藥檔資料.SQL_AddRows(list_Add, false);
                    this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(true);
                    this.Cursor = Cursors.Default;

                    this.Invoke(new Action(delegate
                    {
                        MyMessageBox.ShowDialog($"匯入完成! (新增 {list_Add.Count} 筆資料 , 覆蓋 {list_Replace_SerchValue.Count} 筆資料)");

                    }));
                }
            }));
          
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_資料內容_檢查藥檔_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_雲端藥檔 = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            List<object[]> list_雲端藥檔_buf = new List<object[]>();
            List<object[]> list_本地藥檔 = this.sqL_DataGridView_參數設定_藥檔資料.SQL_GetAllRows(false);
            List<object[]> list_本地藥檔_replace = new List<object[]>();
            for (int i = 0; i < list_本地藥檔.Count; i++)
            {
                string 藥品碼 = list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品碼].ObjectToString();
                list_雲端藥檔_buf = list_雲端藥檔.GetRows((int)enum_藥檔資料.藥品碼, 藥品碼);
                if(list_雲端藥檔_buf.Count > 0)
                {
                    bool replace = false;
                    if (list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品名稱].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品名稱].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品中文名稱].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_藥檔資料.中文名稱].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_參數設定_藥檔資料.包裝單位].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_藥檔資料.包裝數量].ObjectToString()) replace = true;
                    if (list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品條碼1].ObjectToString() != list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品條碼1].ObjectToString()) replace = true;

                    list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品名稱] = list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品名稱];
                    list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品中文名稱] = list_雲端藥檔_buf[0][(int)enum_藥檔資料.中文名稱];
                    list_本地藥檔[i][(int)enum_參數設定_藥檔資料.包裝單位] = list_雲端藥檔_buf[0][(int)enum_藥檔資料.包裝單位];
                    list_本地藥檔[i][(int)enum_參數設定_藥檔資料.藥品條碼1] = list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品條碼1];
                    if(replace)
                    {
                        list_本地藥檔_replace.Add(list_本地藥檔[i]);
                    }

                }
            }
            this.sqL_DataGridView_參數設定_藥檔資料.SQL_ReplaceExtra(list_本地藥檔_replace , true);
        }
        private void PlC_RJ_Button_參數設定_藥檔資料_自動填入_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_雲端藥檔 = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            List<object[]> list_雲端藥檔_buf = new List<object[]>();
            string 藥品碼 = this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品碼.Text;
            list_雲端藥檔_buf = list_雲端藥檔.GetRows((int)enum_藥檔資料.藥品碼, 藥品碼);
            if(list_雲端藥檔_buf.Count == 0)
            {
                MyMessageBox.ShowDialog("查無資料!");
                return;
            }
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品名稱.Text = list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品名稱].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品中文名稱.Text = list_雲端藥檔_buf[0][(int)enum_藥檔資料.中文名稱].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_藥品條碼.Text = list_雲端藥檔_buf[0][(int)enum_藥檔資料.藥品條碼1].ObjectToString();
            this.rJ_TextBox_參數設定_藥檔資料_資料內容_包裝單位.Text = list_雲端藥檔_buf[0][(int)enum_藥檔資料.包裝單位].ObjectToString();
        }
        #endregion
        private class ICP_參數設定_藥檔資料 : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string temp0 = x[(int)enum_參數設定_藥檔資料.庫存].ObjectToString();
                string temp1 = y[(int)enum_參數設定_藥檔資料.庫存].ObjectToString();

                return temp0.CompareTo(temp1) * -1;


            }
        }
    }
}
