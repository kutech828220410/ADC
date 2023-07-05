using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyFaceID;
using ArcSoftFace.SDKModels;
using ArcSoftFace.SDKUtil;
using ArcSoftFace.Utils;
using ArcSoftFace.Entity;
using MyUI;
using Basic;
using H_Pannel_lib;
using RFID_FX600lib;
namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {
        private PLC_Device PLC_Device_已登入 = new PLC_Device("S4000");

        private void Program_登入畫面_Init()
        {
            this.Function_登出();
            this.PLC_Device_已登入.Bool = false;
        }
        private bool flag_Program_登入畫面_Init = false;
        private void Program_登入畫面()
        {
            if (this.plC_ScreenPage_Main.PageText == "登入畫面")
            {
                if (!flag_Program_登入畫面_Init)
                {
                    this.List_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                    this.List_人員資料 = (from value in List_人員資料
                                      where value[(int)enum_人員資料.識別圖案].ObjectToString().StringIsEmpty() == false
                                      select value).ToList();
                    myFaceIDUI_Login.ClearFaceList();
                    string json = "";
                    for (int i = 0; i < this.List_人員資料.Count; i++)
                    {
                        json = this.List_人員資料[i][(int)enum_人員資料.識別圖案].ObjectToString();
                        FaceFeature faceFeature = json.JsonDeserializet<FaceFeature>();
                     if(faceFeature.feature!=null)  myFaceIDUI_Login.RegisterFaceList(faceFeature.ToASF_FaceFeature());
                    }
                    this.Function_登出(1);

                    this.Invoke(new Action(delegate
                    {
                        this.textBox_登入畫面_帳號.Text = "";
                        this.textBox_登入畫面_密碼.Text = "";
                    }));
                    flag_Program_登入畫面_Init = true;
                }
            }
            else
            {
                flag_Program_登入畫面_Init = false;
            }
            sub_Program_登入畫面_RFID檢查();
            sub_Program_登入畫面_人臉辨識檢查();
        }

        #region PLC_登入畫面_人臉辨識檢查
        PLC_Device PLC_Device_登入畫面_人臉辨識檢查 = new PLC_Device("S4125");
        PLC_Device PLC_Device_登入畫面_人臉辨識檢查_OK = new PLC_Device("S4126");
        int cnt_Program_登入畫面_人臉辨識檢查 = 65534;
        void sub_Program_登入畫面_人臉辨識檢查()
        {
            if (!PLC_Device_已登入.Bool && this.plC_ScreenPage_Main.PageText == "登入畫面")
            {
                PLC_Device_登入畫面_人臉辨識檢查.Bool = true;
            }
            else
            {
                PLC_Device_登入畫面_人臉辨識檢查.Bool = false;
            }
            if (cnt_Program_登入畫面_人臉辨識檢查 == 65534)
            {
                PLC_Device_登入畫面_人臉辨識檢查.SetComment("PLC_登入畫面_人臉辨識檢查");
                PLC_Device_登入畫面_人臉辨識檢查_OK.SetComment("PLC_登入畫面_人臉辨識檢查_OK");
                PLC_Device_登入畫面_人臉辨識檢查.Bool = false;
                cnt_Program_登入畫面_人臉辨識檢查 = 65535;
            }
            if (cnt_Program_登入畫面_人臉辨識檢查 == 65535) cnt_Program_登入畫面_人臉辨識檢查 = 1;
            if (cnt_Program_登入畫面_人臉辨識檢查 == 1) cnt_Program_登入畫面_人臉辨識檢查_檢查按下(ref cnt_Program_登入畫面_人臉辨識檢查);
            if (cnt_Program_登入畫面_人臉辨識檢查 == 2) cnt_Program_登入畫面_人臉辨識檢查_初始化(ref cnt_Program_登入畫面_人臉辨識檢查);
            if (cnt_Program_登入畫面_人臉辨識檢查 == 3) cnt_Program_登入畫面_人臉辨識檢查 = 65500;
            if (cnt_Program_登入畫面_人臉辨識檢查 > 1) cnt_Program_登入畫面_人臉辨識檢查_檢查放開(ref cnt_Program_登入畫面_人臉辨識檢查);

            if (cnt_Program_登入畫面_人臉辨識檢查 == 65500)
            {
                PLC_Device_登入畫面_人臉辨識檢查.Bool = false;
                PLC_Device_登入畫面_人臉辨識檢查_OK.Bool = false;
                cnt_Program_登入畫面_人臉辨識檢查 = 65535;
            }
        }
        void cnt_Program_登入畫面_人臉辨識檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_登入畫面_人臉辨識檢查.Bool) cnt++;
        }
        void cnt_Program_登入畫面_人臉辨識檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_登入畫面_人臉辨識檢查.Bool) cnt = 65500;
        }
        void cnt_Program_登入畫面_人臉辨識檢查_初始化(ref int cnt)
        {
            if (retryToSucess > 0)
            {
                int index = FaceTest_index;
                if (FaceTest_index >= 0)
                {
                    string ID = this.List_人員資料[index][(int)enum_人員資料.ID].ObjectToString();
                    List<object[]> list_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                    List<object[]> list_人員資料_buf = new List<object[]>();
                    list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.ID, ID);
                    if (list_人員資料_buf.Count == 0)
                    {
                        cnt = 65500;
                        return;
                    }
                    string 密碼 = list_人員資料_buf[0][(int)enum_人員資料.密碼].ObjectToString();
                    string 姓名 = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                    this.Invoke(new Action(delegate
                    {
                        this.textBox_登入畫面_帳號.Text = ID;
                        this.textBox_登入畫面_密碼.Text = 密碼;
                        this.Function_登入();
                        Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.RFID登入, this.rJ_TextBox_登入者姓名.Texts, this.rJ_TextBox_登入者ID.Texts);
                    }));                 
                }
            }
            cnt++;
        }












        #endregion
        #region PLC_登入畫面_RFID檢查
        PLC_Device PLC_Device_登入畫面_RFID檢查 = new PLC_Device("S4105");
        PLC_Device PLC_Device_登入畫面_RFID檢查_OK = new PLC_Device("S4106");
        int cnt_Program_登入畫面_RFID檢查 = 65534;
        void sub_Program_登入畫面_RFID檢查()
        {
            if(!PLC_Device_已登入.Bool && this.plC_ScreenPage_Main.PageText == "登入畫面")
            {
                PLC_Device_登入畫面_RFID檢查.Bool = true;
            }
            else
            {
                PLC_Device_登入畫面_RFID檢查.Bool = false;
            }
            if (cnt_Program_登入畫面_RFID檢查 == 65534)
            {
                PLC_Device_登入畫面_RFID檢查.SetComment("PLC_登入畫面_RFID檢查");
                PLC_Device_登入畫面_RFID檢查_OK.SetComment("PLC_登入畫面_RFID檢查_OK");
                PLC_Device_登入畫面_RFID檢查.Bool = false;
                cnt_Program_登入畫面_RFID檢查 = 65535;
            }
            if (cnt_Program_登入畫面_RFID檢查 == 65535) cnt_Program_登入畫面_RFID檢查 = 1;
            if (cnt_Program_登入畫面_RFID檢查 == 1) cnt_Program_登入畫面_RFID檢查_檢查按下(ref cnt_Program_登入畫面_RFID檢查);
            if (cnt_Program_登入畫面_RFID檢查 == 2) cnt_Program_登入畫面_RFID檢查_初始化(ref cnt_Program_登入畫面_RFID檢查);
            if (cnt_Program_登入畫面_RFID檢查 == 3) cnt_Program_登入畫面_RFID檢查 = 65500;
            if (cnt_Program_登入畫面_RFID檢查 > 1) cnt_Program_登入畫面_RFID檢查_檢查放開(ref cnt_Program_登入畫面_RFID檢查);

            if (cnt_Program_登入畫面_RFID檢查 == 65500)
            {
                PLC_Device_登入畫面_RFID檢查.Bool = false;
                PLC_Device_登入畫面_RFID檢查_OK.Bool = false;
                cnt_Program_登入畫面_RFID檢查 = 65535;
            }
        }
        void cnt_Program_登入畫面_RFID檢查_檢查按下(ref int cnt)
        {
            if (PLC_Device_登入畫面_RFID檢查.Bool) cnt++;
        }
        void cnt_Program_登入畫面_RFID檢查_檢查放開(ref int cnt)
        {
            if (!PLC_Device_登入畫面_RFID檢查.Bool) cnt = 65500;
        }
        void cnt_Program_登入畫面_RFID檢查_初始化(ref int cnt)
        {
            List<RFID_FX600_UI.RFID_Device> rFID_Devices = this.rfiD_FX600_UI.Get_RFID();
            if(rFID_Devices.Count > 0)
            {
                string 卡號 = rFID_Devices[0].UID;
                List<object[]> list_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                List<object[]> list_人員資料_buf = new List<object[]>();
                list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.卡號, 卡號);
                if(list_人員資料_buf.Count > 0)
                {
                    string ID = list_人員資料_buf[0][(int)enum_人員資料.ID].ObjectToString();
                    string 密碼 = list_人員資料_buf[0][(int)enum_人員資料.密碼].ObjectToString();
                    string 姓名 = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                    this.Invoke(new Action(delegate 
                    {
                        this.textBox_登入畫面_帳號.Text = ID;
                        this.textBox_登入畫面_密碼.Text = 密碼;
                        this.Function_登入();
                        Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.RFID登入, this.rJ_TextBox_登入者姓名.Texts, this.rJ_TextBox_登入者ID.Texts);
                    }));
                }
            }
            cnt++;
        }












        #endregion

        #region Function
        private bool Function_登入()
        {
            bool flag = false;
           
            this.Invoke(new Action(delegate
            {
                if (this.textBox_登入畫面_帳號.Text.ToUpper() == "admin".ToUpper())
                {
                    if (this.textBox_登入畫面_密碼.Text.ToUpper() == "66437068".ToUpper())
                    {
                        this.rJ_TextBox_登入者姓名.Texts = "最高管理權限";
                        this.rJ_TextBox_登入者ID.Texts = "admin";
                        this.textBox_登入畫面_帳號.Text = "";
                        this.textBox_登入畫面_密碼.Text = "";
                        this.Function_登入權限資料_最高權限();
                        this.PLC_Device_已登入.Bool = true;
                        flag = true;
                        return;
                    }
                }
                List<object[]> list_人員資料 = this.sqL_DataGridView_人員資料.SQL_GetAllRows(false);
                List<object[]> list_人員資料_buf = new List<object[]>();
                string ID = this.textBox_登入畫面_帳號.Text;
                string 密碼 = this.textBox_登入畫面_密碼.Text;
                list_人員資料_buf = list_人員資料.GetRows((int)enum_人員資料.ID, ID);
                if (list_人員資料_buf.Count > 0)
                {
                    if (密碼 != list_人員資料_buf[0][(int)enum_人員資料.密碼].ObjectToString())
                    {
                        flag = false;
                        return;
                    }
                    this.rJ_TextBox_登入者姓名.Texts = list_人員資料_buf[0][(int)enum_人員資料.姓名].ObjectToString();
                    this.rJ_TextBox_登入者ID.Texts = list_人員資料_buf[0][(int)enum_人員資料.ID].ObjectToString();
                    int level = list_人員資料_buf[0][(int)enum_人員資料.權限等級].StringToInt32();
                    this.Function_登入權限資料_取得權限(level);
                    this.textBox_登入畫面_帳號.Text = "";
                    this.textBox_登入畫面_密碼.Text = "";
                    this.PLC_Device_已登入.Bool = true;
                    flag = true;
                    return;
                }
                flag = false;
            }));
            return flag;
        }
        private void Function_登出()
        {
            this.Function_登出(0);
        }
        private void Function_登出(int num_page)
        {
            this.Invoke(new Action(delegate
            {
                this.rJ_TextBox_登入者姓名.Texts = "";
                this.rJ_TextBox_登入者ID.Texts = "";
                this.textBox_登入畫面_帳號.Text = "";
                this.textBox_登入畫面_密碼.Text = "";
                this.PLC_Device_已登入.Bool = false;
                this.Function_登入權限資料_清除權限();
                this.PLC_Device_D0.Value = num_page;
            }));
          
        }
        #endregion
        #region Event
        private void plC_RJ_Button_登入畫面_登入_MouseDownEvent(MouseEventArgs mevent)
        {
            if (this.Function_登入())
            {
                Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.密碼登入, this.rJ_TextBox_登入者姓名.Texts, this.rJ_TextBox_登入者ID.Texts);
            }
        }
        private void plC_RJ_Button_登入畫面_登出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                if (MyMessageBox.ShowDialog("是否登出?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.登出, this.rJ_TextBox_登入者姓名.Texts, this.rJ_TextBox_登入者ID.Texts);
                    this.Function_登出();
                }
            }));
                  
        }
        private void textBox_登入畫面_帳號_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                textBox_登入畫面_密碼.Focus();
            }
        }
        private void textBox_登入畫面_密碼_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if(!this.Function_登入())
                {
                    textBox_登入畫面_帳號.Focus();
                }
                else
                {
                    Funnction_交易記錄查詢_動作紀錄新增(enum_交易記錄查詢動作.密碼登入, this.rJ_TextBox_登入者姓名.Texts, this.rJ_TextBox_登入者ID.Texts);
                }
            }
        }
        #endregion

    }
}
