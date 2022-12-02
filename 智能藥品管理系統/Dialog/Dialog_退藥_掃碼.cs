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
using MyUI;
using SQLUI;
namespace 智能藥品管理系統
{
    public partial class Dialog_退藥_掃碼 : Form
    {
        private enum enum_scanstate
        {
            請掃描藥品,
            掃描成功,
            條碼不一致,  
        }
        private object[] 儲位資料;
        private int 應入數量;
        private string 藥品條碼;
        private MySerialPort mySerialPort;
        private MyThread MyThread_porgram;
        private MyTimer MyTimer_SannerTimeOut = new MyTimer();
        private PLC_Device pLC_Device_抽屜感應;
        public Dialog_退藥_掃碼(MySerialPort mySerialPort, object[] 儲位資料 , int 應入數量 , PLC_Device pLC_Device_抽屜感應)
        {
            InitializeComponent();

            this.儲位資料 = 儲位資料;
            this.應入數量 = 應入數量;
            this.mySerialPort = mySerialPort;
            this.pLC_Device_抽屜感應 = pLC_Device_抽屜感應;
        }

        private void Dialog_退藥_掃碼_Load(object sender, EventArgs e)
        {
            this.plC_RJ_Button_確認.MouseDownEvent += PlC_RJ_Button_確認_MouseDownEvent;
            this.plC_RJ_Button_取消.MouseDownEvent += PlC_RJ_Button_取消_MouseDownEvent;

            this.rJ_TextBox_藥品碼.Text = this.儲位資料[(int)enum_入庫作業_藥品資料.藥品碼].ObjectToString();
            this.rJ_TextBox_藥品名稱.Text = this.儲位資料[(int)enum_入庫作業_藥品資料.藥品名稱].ObjectToString();
            this.rJ_TextBox_應入數量.Text = 應入數量.ToString();
            this.rJ_TextBox_已入數量.Text = "0";

            this.藥品條碼 = this.儲位資料[(int)enum_入庫作業_藥品資料.藥品條碼].ObjectToString();
            if (this.藥品條碼.StringIsEmpty())
            {
                this.rJ_TextBox_已入數量.Text = 應入數量.ToString();
                this.rJ_Lable_State.Text = "數量到達,請關閉抽屜";
                cnt = 3;
            }
            this.MyThread_porgram = new MyThread();
            this.MyThread_porgram.Add_Method(sub_program);
            this.MyThread_porgram.AutoRun(true);
            this.MyThread_porgram.SetSleepTime(100);
            this.MyThread_porgram.Trigger();
            MyTimer_SannerTimeOut.StartTickTime(100);

        }

        int cnt = 0;
        private void sub_program()
        {
            if (cnt == 0)
            {
                this.Invoke(new Action(delegate
                {
                    this.rJ_Lable_State.Text = enum_scanstate.請掃描藥品.GetEnumName();
                }));
                mySerialPort.ClearReadByte();
                cnt++;
            }
            if(cnt == 1)
            {
                string readline = mySerialPort.ReadString();
                if (!readline.StringIsEmpty())
                {
                    readline = readline.Replace("\n", "");
                    readline = readline.Replace("\r", "");
                    if (readline == 藥品條碼)
                    {
                        MyTimer_SannerTimeOut.TickStop();
                        MyTimer_SannerTimeOut.StartTickTime(1000);
                        this.Invoke(new Action(delegate
                        {
                            this.rJ_Lable_State.Text = enum_scanstate.掃描成功.GetEnumName();
                            this.rJ_TextBox_已入數量.Text = (this.rJ_TextBox_已入數量.Text.StringToInt32() + 1).ToString();
                        }));

                    }
                    if (readline != 藥品條碼)
                    {
                        MyTimer_SannerTimeOut.TickStop();
                        MyTimer_SannerTimeOut.StartTickTime(1000);
                        this.Invoke(new Action(delegate
                        {
                            this.rJ_Lable_State.Text = enum_scanstate.條碼不一致.GetEnumName();
                        }));

                    }
                    mySerialPort.ClearReadByte();
                    cnt++;
                }
         
            }
            if (cnt == 2)
            {
                if (MyTimer_SannerTimeOut.IsTimeOut())
                {
                    cnt++;
                }
            }
            if (cnt == 3)
            {
                if (應入數量 == this.rJ_TextBox_已入數量.Text.StringToInt32())
                {
                    this.Invoke(new Action(delegate
                    {
                        this.rJ_Lable_State.Text = "數量到達,請關閉抽屜";
                    }));
                    cnt = 100;
                }
                else
                {
                    cnt = 0;
                }
            }

            if (pLC_Device_抽屜感應.Bool)
            {
                PlC_RJ_Button_確認_MouseDownEvent(null);
            }
        }

        private void PlC_RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.No;
                this.MyThread_porgram.Abort();
                this.MyThread_porgram = null;
                this.Close();
            }));
        }
        private void PlC_RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                if (應入數量 != this.rJ_TextBox_已入數量.Text.StringToInt32())
                {
                    MyMessageBox.ShowDialog("數量未到達!");
                    if (pLC_Device_抽屜感應.Bool)
                    {
                        PlC_RJ_Button_取消_MouseDownEvent(null);
                    }
                    return;
                }

                if(!pLC_Device_抽屜感應.Bool)
                {
                    MyMessageBox.ShowDialog("退藥抽屜未關上!");
                    return;
                }
                this.DialogResult = DialogResult.Yes;
                this.MyThread_porgram.Abort();
                this.MyThread_porgram = null;
                this.Close();
            }));
        }
    }
}
