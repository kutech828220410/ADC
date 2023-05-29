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
using H_Pannel_lib;
namespace 智能藥品管理系統
{
    public enum enum_交易記錄查詢動作
    {
        掃碼領藥,
        手輸領藥,
        批次領藥,
        掃碼退藥,
        手輸退藥,
        重複領藥,
        人臉識別登入,
        RFID登入,
        密碼登入,
        登出,
        操作工程模式,
        效期庫存異動,
        入庫,
        實瓶繳回,
        空瓶繳回,
        退藥回收,
        None,
    }

    public enum enum_交易記錄查詢資料
    {
        GUID,
        動作,
        藥品碼,
        藥品名稱,
        藥袋序號,
        房名,
        庫存量,
        交易量,
        結存量,
        操作人,
        病人姓名,
        病歷號,
        操作時間,
        開方時間,
        備註,
    }
    public partial class Form1 : Form
    {      
        private void Program_交易紀錄查詢_Init()
        {
            this.sqL_DataGridView_交易記錄查詢.Init();
            if(!this.sqL_DataGridView_交易記錄查詢.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_交易記錄查詢.SQL_CreateTable();
            }
            this.plC_RJ_Button_交易紀錄查詢_刪除.MouseDownEvent += PlC_RJ_Button_交易紀錄查詢_刪除_MouseDownEvent;
        }

      

        private void Program_交易紀錄查詢()
        {

        }
        #region Function
        void Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作 enum_交易記錄查詢動作, string 操作人, string 備註)
        {
            string 動作 = enum_交易記錄查詢動作.GetEnumName();
            string 藥品碼 = "";
            string 藥品名稱 = "";
            string 藥袋序號 = "";
            string 房名 = "";
            string 庫存量 = "";
            string 交易量 = "";
            string 結存量 = "";
            string 病人姓名 = "";
            string 病歷號 = "";
            string 操作時間 = DateTime.Now.ToDateTimeString_6();
            string 開方時間 = DateTime.Now.ToDateTimeString_6();


            object[] value = new object[new enum_交易記錄查詢資料().GetLength()];
            value[(int)enum_交易記錄查詢資料.GUID] = Guid.NewGuid().ToString();
            value[(int)enum_交易記錄查詢資料.動作] = 動作;
            value[(int)enum_交易記錄查詢資料.藥品碼] = 藥品碼;
            value[(int)enum_交易記錄查詢資料.藥品名稱] = 藥品名稱;
            value[(int)enum_交易記錄查詢資料.藥袋序號] = 藥袋序號;
            value[(int)enum_交易記錄查詢資料.房名] = 房名;
            value[(int)enum_交易記錄查詢資料.庫存量] = 庫存量;
            value[(int)enum_交易記錄查詢資料.交易量] = 交易量;
            value[(int)enum_交易記錄查詢資料.結存量] = 結存量;
            value[(int)enum_交易記錄查詢資料.病人姓名] = 病人姓名;
            value[(int)enum_交易記錄查詢資料.病歷號] = 病歷號;
            value[(int)enum_交易記錄查詢資料.操作人] = 操作人;
            value[(int)enum_交易記錄查詢資料.操作時間] = 操作時間;
            value[(int)enum_交易記錄查詢資料.開方時間] = 開方時間;
            value[(int)enum_交易記錄查詢資料.備註] = 備註;
            this.sqL_DataGridView_交易記錄查詢.SQL_AddRow(value, false);
        }
        #endregion
        #region Event
        private void PlC_RJ_Button_交易紀錄查詢_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.Get_All_Select_RowsValues();

            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            if (MyMessageBox.ShowDialog($"是否刪除資料,共<{list_value.Count}>筆資料", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) != DialogResult.Yes) return;


            this.sqL_DataGridView_交易記錄查詢.SQL_DeleteExtra(list_value, false);
            this.sqL_DataGridView_交易記錄查詢.DeleteExtra(list_value, true);

        }
        private void plC_RJ_Button_交易紀錄查詢_搜尋_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_交易記錄查詢.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<List<object[]>> list_list_value = new List<List<object[]>>();
            DateTime dateTime_start = new DateTime();
            DateTime dateTime_end = new DateTime();

            if (plC_RJ_ChechBox_交易紀錄查詢_操作時間.Checked)
            {
                dateTime_start = dateTimePicker_交易記錄查詢_操作時間_起始.Value;
                dateTime_start = new DateTime(dateTime_start.Year, dateTime_start.Month, dateTime_start.Day, 0, 0, 0);

                dateTime_end = dateTimePicker_交易記錄查詢_操作時間_結束.Value;
                dateTime_end = new DateTime(dateTime_end.Year, dateTime_end.Month, dateTime_end.Day, 23, 59, 59);
                list_value = list_value.GetRowsInDate((int)enum_交易記錄查詢資料.操作時間, dateTime_start, dateTime_end);
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_開方時間.Checked)
            {
                dateTime_start = dateTimePicker_交易記錄查詢_開方時間_起始.Value;
                dateTime_start = new DateTime(dateTime_start.Year, dateTime_start.Month, dateTime_start.Day, 0, 0, 0);

                dateTime_end = dateTimePicker_交易記錄查詢_開方時間_結束.Value;
                dateTime_end = new DateTime(dateTime_end.Year, dateTime_end.Month, dateTime_end.Day, 23, 59, 59);
                list_value = list_value.GetRowsInDate((int)enum_交易記錄查詢資料.開方時間, dateTime_start, dateTime_end);
            }

            if (plC_RJ_ChechBox_交易紀錄查詢_掃碼領藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.掃碼領藥.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_手輸領藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.手輸領藥.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_批次領藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.批次領藥.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_掃碼退藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.掃碼退藥.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_手輸退藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.手輸退藥.GetEnumName()));
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.空瓶繳回.GetEnumName()));
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.實瓶繳回.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_重複領藥.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.重複領藥.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_入庫.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.入庫.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_人臉識別登入.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.人臉識別登入.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_RFID登入.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.RFID登入.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_密碼登入.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.密碼登入.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_登出.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.登出.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_操作工程模式.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.操作工程模式.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_操作工程模式.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.退藥回收.GetEnumName()));
            }
            if (plC_RJ_ChechBox_交易紀錄查詢_效期庫存異動.Checked)
            {
                list_list_value.Add(list_value.GetRows((int)enum_交易記錄查詢資料.動作, enum_交易記錄查詢動作.效期庫存異動.GetEnumName()));
            }
            foreach(List<object[]> list_value_temp in list_list_value)
            {
                foreach(object[] value_temp in list_value_temp)
                {
                    list_value_buf.Add(value_temp);
                }
            }
            if (list_value_buf.Count == 0) list_value_buf = list_value;
            if (!textBox_交易記錄查詢_藥品碼.Text.StringIsEmpty())
            {
                list_value_buf = list_value_buf.GetRows((int)enum_交易記錄查詢資料.藥品碼, textBox_交易記錄查詢_藥品碼.Text);
            }
            if (!textBox_交易記錄查詢_藥品名稱.Text.StringIsEmpty())
            {
                list_value_buf = list_value_buf.GetRowsByLike((int)enum_交易記錄查詢資料.藥品名稱, textBox_交易記錄查詢_藥品名稱.Text);
            }   
            if (!textBox_交易記錄查詢_藥袋序號.Text.StringIsEmpty())
            {
                list_value_buf = list_value_buf.GetRowsByLike((int)enum_交易記錄查詢資料.藥袋序號, textBox_交易記錄查詢_藥袋序號.Text);
            }
            list_value_buf.Sort(new TraddingComparerby());
            this.sqL_DataGridView_交易記錄查詢.RefreshGrid(list_value_buf);
        }
        #endregion

        public class TraddingComparerby : IComparer<object[]>
        {
            //實作Compare方法
            //依Speed由小排到大。
            public int Compare(object[] x, object[] y)
            {
                DateTime datetime1 = x[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                DateTime datetime2 = y[(int)enum_交易記錄查詢資料.操作時間].ToDateTimeString_6().StringToDateTime();
                int compare = DateTime.Compare(datetime1, datetime2);
                if (compare != 0) return compare;
                int 結存量1 = x[(int)enum_交易記錄查詢資料.結存量].StringToInt32();
                int 結存量2 = y[(int)enum_交易記錄查詢資料.結存量].StringToInt32();
                if (結存量1 > 結存量2)
                {
                    return -1;
                }
                else if (結存量1 < 結存量2)
                {
                    return 1;
                }
                else if (結存量1 == 結存量2) return 0;
                return 0;

            }
        }
    }
}
