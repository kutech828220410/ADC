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
using MyFaceID;
using ArcSoftFace.SDKModels;
using ArcSoftFace.SDKUtil;
using ArcSoftFace.Utils;
using ArcSoftFace.Entity;
using RFID_FX600lib;
using MySQL_Login;
namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {
        public enum enum_人員資料
        {
            GUID,
            ID,
            姓名,
            性別,
            密碼,
            單位,
            權限等級,
            卡號,
            識別圖案,
        }
        private bool flag_人員資料_取得註冊圖案 = false;
        private List<PLC_Device> List_PLC_Device_權限管理 = new List<PLC_Device>();
        private List<LoginDataWebAPI.Class_login_data> List_class_Login_Data = new List<LoginDataWebAPI.Class_login_data>();
        private List<LoginDataWebAPI.Class_login_data_index> List_class_Login_Data_index = new List<LoginDataWebAPI.Class_login_data_index>();
        private void Program_人員資料_Init()
        {
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_人員資料, this.dBConfigClass.DB_person_page);
            this.sqL_DataGridView_人員資料.Init();
            if (!this.sqL_DataGridView_人員資料.SQL_IsTableCreat())
            {
                this.sqL_DataGridView_人員資料.SQL_CreateTable();
            }
            this.sqL_DataGridView_人員資料.RowDoubleClickEvent += SqL_DataGridView_人員資料_RowDoubleClickEvent;
            this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);

            this.comboBox_人員資料_權限等級.SelectedIndex = 0;

            this.plC_RJ_ComboBox_權限管理_權限等級.Items.Clear();
            int level_num = (int)this.loginUI.Level_num;
            for (int i = 1; i <= level_num; i++)
            {
                this.plC_RJ_ComboBox_權限管理_權限等級.Items.Add(i.ToString("00"));
            }
            for (int i = 0; i < 256; i++) this.List_PLC_Device_權限管理.Add(new PLC_Device($"S{39000 + i}"));

        }
        private bool flag_Program_人員資料_Init = false;
        private void Program_人員資料()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料")
            {
                if (!flag_Program_人員資料_Init)
                {
                    this.List_class_Login_Data = this.loginUI.Get_login_data();
                    this.List_class_Login_Data_index = this.loginUI.Get_login_data_index();
                    this.plC_RJ_ComboBox_權限管理_權限等級.SetValue(0);
                    this.loginIndex_Pannel.Set_Login_Data_Index(this.List_class_Login_Data_index);
                    this.loginIndex_Pannel.Set_Login_Data(this.List_class_Login_Data[0]);
                    this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
                    flag_Program_人員資料_Init = true;
                }
            }
            else
            {
                flag_Program_人員資料_Init = false;
            }
            this.sub_Program_人員資料_讀取RFID();

        }
        #region PLC_人員資料_讀取RFID
        PLC_Device PLC_Device_人員資料_讀取RFID = new PLC_Device("S6405");
        PLC_Device PLC_Device_人員資料_讀取RFID_OK = new PLC_Device("S6406");
        int cnt_Program_人員資料_讀取RFID = 65534;
        void sub_Program_人員資料_讀取RFID()
        {
            if (this.plC_ScreenPage_Main.PageText == "人員資料") PLC_Device_人員資料_讀取RFID.Bool = true;
            else PLC_Device_人員資料_讀取RFID.Bool = false;

            if (cnt_Program_人員資料_讀取RFID == 65534)
            {
                PLC_Device_人員資料_讀取RFID.SetComment("PLC_人員資料_讀取RFID");
                PLC_Device_人員資料_讀取RFID_OK.SetComment("PLC_人員資料_讀取RFID_OK");
                PLC_Device_人員資料_讀取RFID.Bool = false;
                cnt_Program_人員資料_讀取RFID = 65535;
            }
            if (cnt_Program_人員資料_讀取RFID == 65535) cnt_Program_人員資料_讀取RFID = 1;
            if (cnt_Program_人員資料_讀取RFID == 1) cnt_Program_人員資料_讀取RFID_檢查按下(ref cnt_Program_人員資料_讀取RFID);
            if (cnt_Program_人員資料_讀取RFID == 2) cnt_Program_人員資料_讀取RFID_初始化(ref cnt_Program_人員資料_讀取RFID);
            if (cnt_Program_人員資料_讀取RFID == 3) cnt_Program_人員資料_讀取RFID = 65500;
            if (cnt_Program_人員資料_讀取RFID > 1) cnt_Program_人員資料_讀取RFID_檢查放開(ref cnt_Program_人員資料_讀取RFID);

            if (cnt_Program_人員資料_讀取RFID == 65500)
            {
                PLC_Device_人員資料_讀取RFID.Bool = false;
                PLC_Device_人員資料_讀取RFID_OK.Bool = false;
                cnt_Program_人員資料_讀取RFID = 65535;
            }
        }
        void cnt_Program_人員資料_讀取RFID_檢查按下(ref int cnt)
        {
            if (PLC_Device_人員資料_讀取RFID.Bool) cnt++;
        }
        void cnt_Program_人員資料_讀取RFID_檢查放開(ref int cnt)
        {
            if (!PLC_Device_人員資料_讀取RFID.Bool) cnt = 65500;
        }
        void cnt_Program_人員資料_讀取RFID_初始化(ref int cnt)
        {
            List<RFID_FX600_UI.RFID_Device> rFID_Devices = this.rfiD_FX600_UI.Get_RFID();
            for (int i = 0; i < rFID_Devices.Count; i++)
            {
                this.Invoke(new Action(delegate { this.rJ_TextBox_人員資料_卡號.Texts = rFID_Devices[i].UID; }));
                break;
            }
            cnt++;
        }












        #endregion

        #region Function
        private string Function_人員資料_檢查內容(object[] value)
        {
            string str_error = "";
            List<string> list_error = new List<string>();
            if(value[(int)enum_人員資料.姓名].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'姓名'欄位不得空白!");
            }
            if (value[(int)enum_人員資料.ID].ObjectToString().StringIsEmpty())
            {
                list_error.Add("'ID'欄位不得空白!");
            }
            for(int i = 0; i < list_error.Count; i++)
            {
                str_error += $"{(i + 1).ToString("00")}. {list_error[i]}";
                if (i != list_error.Count - 1) str_error += "\n";
            }
            return str_error;
        }
        private void Function_人員資料_清除內容()
        {
            this.Invoke(new Action(delegate 
            {
                this.rJ_TextBox_人員資料_ID.Text = "";
                this.rJ_TextBox_人員資料_姓名.Text = "";
                this.rJ_TextBox_人員資料_密碼.Text = "";
                this.rJ_TextBox_人員資料_單位.Text = "";
                this.rJ_TextBox_人員資料_卡號.Text = "";
                this.comboBox_人員資料_權限等級.Text = "";
                this.rJ_TextBox_人員資料_識別圖案.Text = "";
            }));
 
        }
    
        #endregion
        #region Event
        private void plC_RJ_Button_人員資料_權限管理_上傳_MouseDownEvent(MouseEventArgs mevent)
        {
            for (int i = 0; i < List_class_Login_Data.Count; i++)
            {
                this.loginUI.Set_login_data(List_class_Login_Data[i]);
            }
            MyMessageBox.ShowDialog("權限更動完成!");
        }
        private void plC_RJ_ComboBox_權限管理_權限等級_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int level = plC_RJ_ComboBox_權限管理_權限等級.Text.StringToInt32();
            if(level > 0)
            {
                List<LoginDataWebAPI.Class_login_data> List_class_Login_Data_buf = new List<LoginDataWebAPI.Class_login_data>();
                List_class_Login_Data_buf = (from value in List_class_Login_Data
                                             where value.level.StringToInt32() == level
                                             select value).ToList();
                if (List_class_Login_Data_buf.Count > 0)
                {
                    this.loginIndex_Pannel.Set_Login_Data(List_class_Login_Data_buf[0]);
                }
            }
        }
        private void SqL_DataGridView_人員資料_RowDoubleClickEvent(object[] RowValue)
        {
            this.rJ_TextBox_人員資料_ID.Text = RowValue[(int)enum_人員資料.ID].ObjectToString();
            this.rJ_TextBox_人員資料_姓名.Text = RowValue[(int)enum_人員資料.姓名].ObjectToString();
            string 性別 = RowValue[(int)enum_人員資料.性別].ObjectToString();
            if(性別 == "男")
            {
                rJ_RatioButton_人員資料_男.Checked = true;
            }
            else if (性別 == "女")
            {
                rJ_RatioButton_人員資料_女.Checked = true;
            }
            this.rJ_TextBox_人員資料_密碼.Text = RowValue[(int)enum_人員資料.密碼].ObjectToString();
            this.rJ_TextBox_人員資料_單位.Text = RowValue[(int)enum_人員資料.單位].ObjectToString();
            this.comboBox_人員資料_權限等級.Text = RowValue[(int)enum_人員資料.權限等級].ObjectToString();
            this.rJ_TextBox_人員資料_卡號.Text = RowValue[(int)enum_人員資料.卡號].ObjectToString();
            this.rJ_TextBox_人員資料_識別圖案.Text = RowValue[(int)enum_人員資料.識別圖案].ObjectToString();
        }
        private void plC_RJ_Button_人員資料_登錄_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                string 性別 = rJ_RatioButton_人員資料_男.Checked ? "男" : "女";
                List<object[]> list_value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                List<object[]> list_value_buf = new List<object[]>();
                list_value_buf = (from value in list_value
                                  where value[(int)enum_人員資料.ID].ObjectToString() == this.rJ_TextBox_人員資料_ID.Text
                                  select value).ToList();
                if (list_value_buf.Count == 0)
                {
                    object[] value = new object[enum_人員資料.GUID.GetEnumNames().Length];
                    value[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                    value[(int)enum_人員資料.ID] = this.rJ_TextBox_人員資料_ID.Text;
                    value[(int)enum_人員資料.姓名] = this.rJ_TextBox_人員資料_姓名.Text;
                    value[(int)enum_人員資料.性別] = 性別;
                    value[(int)enum_人員資料.密碼] = this.rJ_TextBox_人員資料_密碼.Text;
                    value[(int)enum_人員資料.單位] = this.rJ_TextBox_人員資料_單位.Text;
                    value[(int)enum_人員資料.卡號] = this.rJ_TextBox_人員資料_卡號.Text;
                    value[(int)enum_人員資料.權限等級] = this.comboBox_人員資料_權限等級.Text;
                    value[(int)enum_人員資料.識別圖案] = this.rJ_TextBox_人員資料_識別圖案.Text;
                    string str_error = this.Function_人員資料_檢查內容(value);
                    if (!str_error.StringIsEmpty())
                    {
                        MyMessageBox.ShowDialog(str_error);
                        return;
                    }
                    this.sqL_DataGridView_人員資料.SQL_AddRow(value, false);
                }
                else
                {
                    object[] value = new object[enum_人員資料.GUID.GetEnumNames().Length];
                    if (MyMessageBox.ShowDialog("此ID已註冊,是否覆寫?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                    {
                        value[(int)enum_人員資料.GUID] = list_value_buf[0][(int)enum_人員資料.GUID].ObjectToString();
                        value[(int)enum_人員資料.ID] = this.rJ_TextBox_人員資料_ID.Text;
                        value[(int)enum_人員資料.姓名] = this.rJ_TextBox_人員資料_姓名.Text;
                        value[(int)enum_人員資料.性別] = 性別;
                        value[(int)enum_人員資料.密碼] = this.rJ_TextBox_人員資料_密碼.Text;
                        value[(int)enum_人員資料.單位] = this.rJ_TextBox_人員資料_單位.Text;
                        value[(int)enum_人員資料.卡號] = this.rJ_TextBox_人員資料_卡號.Text;
                        value[(int)enum_人員資料.權限等級] = this.comboBox_人員資料_權限等級.Text;
                        if (MyMessageBox.ShowDialog("是否寫入識別資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                        {
                            value[(int)enum_人員資料.識別圖案] = this.rJ_TextBox_人員資料_識別圖案.Text;
                        }
                        else
                        {
                            value[(int)enum_人員資料.識別圖案] = list_value_buf[0][(int)enum_人員資料.識別圖案].ObjectToString();
                        }
                        string str_error = this.Function_人員資料_檢查內容(value);
                        if (!str_error.StringIsEmpty())
                        {
                            MyMessageBox.ShowDialog(str_error);
                            return;
                        }
                        this.sqL_DataGridView_人員資料.SQL_Replace(enum_人員資料.GUID.GetEnumName(), value[(int)enum_人員資料.GUID].ObjectToString(), value, false);
                    }

                }
                this.Function_人員資料_清除內容();
                this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
            }));
       
        }
        private void plC_RJ_Button_人員資料_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                List<object[]> list_value = this.sqL_DataGridView_人員資料.Get_All_Select_RowsValues();
                if (list_value.Count == 0) MyMessageBox.ShowDialog("未選取資料!");
                if (MyMessageBox.ShowDialog($"是否刪除選取資料‧共{list_value.Count}筆", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    string GUID = "";
                    for (int i = 0; i < list_value.Count; i++)
                    {
                        GUID = list_value[i][(int)enum_人員資料.GUID].ObjectToString();
                        this.sqL_DataGridView_人員資料.SQL_Delete(enum_人員資料.GUID.GetEnumName(), GUID, false);
                    }
                }
                this.Function_人員資料_清除內容();
                this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
            }));

        }
        private void plC_RJ_Button_人員資料_清除內容_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Function_人員資料_清除內容();
        }
        private void plC_RJ_Button_人員資料_匯出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
                {
                    DataTable dataTable = this.sqL_DataGridView_人員資料.GetDataTable().DeepClone();
                    CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
                    MyMessageBox.ShowDialog("匯出完成!");
                }
            }));
    
        }
        private void plC_RJ_Button_人員資料_匯入_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (this.openFileDialog_LoadExcel.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    DataTable dataTable = new DataTable();
                    CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                    DataTable datatable_buf = dataTable.ReorderTable(new enum_人員資料());
                    if (datatable_buf == null)
                    {
                        this.Cursor = Cursors.Default;
                        MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                        return;
                    }
                    List<object[]> list_SQL_Value = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
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
                        string ID = dr[enum_人員資料.ID.GetEnumName()].ObjectToString();
                        object[] src_obj = new string[new enum_人員資料().GetEnumNames().Length];
                        list_Value_buf = (from value in list_SQL_Value
                                          where value[(int)enum_人員資料.ID].ObjectToString() == ID
                                          select value).ToList();
                        if (list_Value_buf.Count != 0) flag_replace = true;


                        if (flag_replace) src_obj = list_Value_buf[0];

                        string str_error = this.Function_人員資料_檢查內容(src_obj);
                        if (!str_error.StringIsEmpty())
                        {
                            MyMessageBox.ShowDialog(str_error);
                            continue;
                        }

                        for (int i = 0; i < src_obj.Length; i++)
                        {
                            if (i == (int)enum_人員資料.GUID)
                            {
                                if (!flag_replace) src_obj[(int)enum_人員資料.GUID] = Guid.NewGuid().ToString();
                                continue;
                            }
                            src_obj[i] = dr[i].ObjectToString();
                        }


                        string 性別 = src_obj[(int)enum_人員資料.性別].ObjectToString();
                        string 權限等級 = src_obj[(int)enum_人員資料.權限等級].StringToInt32().ToString("00");
                        if (性別 != "男" && 性別 != "女") 性別 = "男";
                        if (權限等級 == "-01") 權限等級 = "01";
                        src_obj[(int)enum_人員資料.性別] = 性別;
                        src_obj[(int)enum_人員資料.權限等級] = 權限等級;
                        if (list_Value_buf.Count == 0) list_Add.Add(src_obj);
                        else
                        {
                            list_Replace_SerchValue.Add((new string[] { src_obj[(int)enum_人員資料.GUID].ObjectToString() }));
                            list_Replace_Value.Add(src_obj);
                        }
                    }
                    this.sqL_DataGridView_人員資料.SQL_ReplaceExtra(enum_人員資料.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
                    this.sqL_DataGridView_人員資料.SQL_AddRows(list_Add, false);
                    this.sqL_DataGridView_人員資料.SQL_GetAllRows(true);
                    this.Cursor = Cursors.Default;
                    MyMessageBox.ShowDialog($"匯入完成! (新增 {list_Add.Count} 筆資料 , 覆蓋 {list_Replace_SerchValue.Count} 筆資料)");
                }
            }));
            
        }
        async private void plC_RJ_Button_人員資料_取得識別圖案_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.plC_RJ_Button_人員資料_取得識別圖案.Text = "識別中...";
            }));
         
            flag_人員資料_取得註冊圖案 = true;
            Task<FaceFeature> task = Task<FaceFeature>.Factory.StartNew(GetFaceFeature);
            FaceFeature faceFeature = await task;
            if (faceFeature.featureSize == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("取得圖案失敗!");
                }));
              
            }
            else
            {
                this.Invoke(new Action(delegate
                {
                    MyMessageBox.ShowDialog("取得圖案成功!");
                }));
                this.rJ_TextBox_人員資料_識別圖案.Text = faceFeature.JsonSerializationt();
           
            }
            this.Invoke(new Action(delegate
            {
                this.plC_RJ_Button_人員資料_取得識別圖案.Text = "取得識別圖案";
            }));
       
        }
        #endregion

        private void Function_登入權限資料_取得權限(int level)
        {
            LoginDataWebAPI.Class_login_data class_Login_Data = this.loginUI.Get_login_data(level);
            if (class_Login_Data != null)
            {
                for (int i = 0; i < class_Login_Data.data.Count; i++)
                {
                    this.List_PLC_Device_權限管理[i].Bool = class_Login_Data.data[i];
                }
            }
        }
        private void Function_登入權限資料_最高權限()
        {
            for (int i = 0; i < 256; i++)
            {
                this.List_PLC_Device_權限管理[i].Bool = true;
            }
        }
        private void Function_登入權限資料_清除權限()
        {
            for (int i = 0; i < 256; i++)
            {
                this.List_PLC_Device_權限管理[i].Bool = false;
            }
        }

    }
}
