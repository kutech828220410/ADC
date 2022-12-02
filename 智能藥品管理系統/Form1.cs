using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using MySql.Data.MySqlClient;
using SQLUI;
using H_Pannel_lib;
[assembly: AssemblyVersion("1.0.1.0")]
[assembly: AssemblyFileVersion("1.0.1.0")]
namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {
        private Voice voice = new Voice();
        private MyThread MyThread_ProgramPLC;
        private MyThread MyThread_FaceID;
        private MyConvert myConvert = new MyConvert();
        private PLC_Device PLC_Device_M8013 = new PLC_Device("M8013");
        private PLC_Device PLC_Device_D0 = new PLC_Device("D0");

        #region DBConfigClass
        private const string DBConfigFileName = "DBConfig.txt";
        public class DBConfigClass
        {
            private SQL_DataGridView.ConnentionClass dB_Basic = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_person_page = new SQL_DataGridView.ConnentionClass();
            private SQL_DataGridView.ConnentionClass dB_Medicine_Cloud = new SQL_DataGridView.ConnentionClass();

            public SQL_DataGridView.ConnentionClass DB_Basic { get => dB_Basic; set => dB_Basic = value; }
            public SQL_DataGridView.ConnentionClass DB_person_page { get => dB_person_page; set => dB_person_page = value; }
            public SQL_DataGridView.ConnentionClass DB_Medicine_Cloud { get => dB_Medicine_Cloud; set => dB_Medicine_Cloud = value; }
        }
        private void LoadDBConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($".//{DBConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(new DBConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{DBConfigFileName}");
                Application.Exit();
            }
            else
            {
                dBConfigClass = Basic.Net.JsonDeserializet<DBConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<DBConfigClass>(dBConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{DBConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{DBConfigFileName}檔案失敗!");
                }

            }
        }
        public DBConfigClass dBConfigClass = new DBConfigClass();
        #endregion
        #region MyConfigClass
        private const string MyConfigFileName = "myconfig.txt";
        public class MyConfigClass
        {
            private string dataBaseName = "adc_01";
            private string iP = "";
            private string userName = "";
            private string password = "";
            private uint port = 0;
            private MySql.Data.MySqlClient.MySqlSslMode mySqlSslMode = MySql.Data.MySqlClient.MySqlSslMode.None;

            private string activeKey = "";
            private string appID = "";
            private string sdkKey = "";
            private string offline_dat = "";

            public string DataBaseName { get => dataBaseName; set => dataBaseName = value; }
            public string IP { get => iP; set => iP = value; }
            public string UserName { get => userName; set => userName = value; }
            public string Password { get => password; set => password = value; }
            public uint Port { get => port; set => port = value; }
            public MySqlSslMode MySqlSslMode { get => mySqlSslMode; set => mySqlSslMode = value; }

            public string ActiveKey { get => activeKey; set => activeKey = value; }
            public string AppID { get => appID; set => appID = value; }
            public string SdkKey { get => sdkKey; set => sdkKey = value; }
            public string Offline_dat { get => offline_dat; set => offline_dat = value; }
        }
        public MyConfigClass myConfigClass = new MyConfigClass();
        private void LoadMyConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($".//{MyConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{MyConfigFileName}");
                Application.Exit();
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }

            }

            //this.ftp_DounloadUI1.FTP_Server = myConfigClass.FTP_Server;
        }
        #endregion
        #region FtpConfigClass
        private const string FtpConfigFileName = "FtpConfig.txt";
        public FtpConfigClass ftpConfigClass = new FtpConfigClass();
        public class FtpConfigClass
        {
            private string fTP_Server = "";
            private string fTP_username = "";
            private string fTP_password = "";

            public string FTP_Server { get => fTP_Server; set => fTP_Server = value; }
            public string FTP_username { get => fTP_username; set => fTP_username = value; }
            public string FTP_password { get => fTP_password; set => fTP_password = value; }
        }
        private void LoadFtpConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($".//{FtpConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<FtpConfigClass>(new FtpConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{FtpConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{FtpConfigFileName}檔案失敗!");
                }
                MyMessageBox.ShowDialog($"未建立參數文件!請至子目錄設定{FtpConfigFileName}");
                Application.Exit();
            }
            else
            {
                ftpConfigClass = Basic.Net.JsonDeserializet<FtpConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<FtpConfigClass>(ftpConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($".//{FtpConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{FtpConfigFileName}檔案失敗!");
                }

            }

            this.ftp_DounloadUI.FTP_Server = ftpConfigClass.FTP_Server;
            this.ftp_DounloadUI.Username = ftpConfigClass.FTP_username;
            this.ftp_DounloadUI.Password = ftpConfigClass.FTP_password;
            string updateVersion = this.ftp_DounloadUI.GetFileVersion();
            if (this.ftp_DounloadUI.CheckUpdate(this.ProductVersion, updateVersion))
            {
                if (Basic.MyMessageBox.ShowDialog(string.Format("有新版本是否更新? (Ver : {0})", updateVersion), "Update", Basic.MyMessageBox.enum_BoxType.Asterisk, Basic.MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    this.Invoke(new Action(delegate { this.Update(); }));
                }
            }
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.label_Version.Text = $"Version {ProductVersion}";
            LoadMyConfig();
            LoadDBConfig();
            LoadFtpConfig();


            Dialog_NumPannel.form = this.FindForm();
            MyMessageBox.form = this.FindForm();
            MyMessageBox.音效 = false;
            this.plC_UI_Init1.Run(this.FindForm(), this.lowerMachine_Panel1);
            this.plC_UI_Init1.UI_Finished_Event += PlC_UI_Init1_UI_Finished_Event;    
        }


        private void PlC_UI_Init1_UI_Finished_Event()
        {
            PLC_UI_Init.Set_PLC_ScreenPage(this.pannel_Main01, this.plC_ScreenPage_Main);
            PLC_UI_Init.Set_PLC_ScreenPage(this.pannel_Main02, this.plC_ScreenPage_Main);
            PLC_UI_Init.Set_PLC_ScreenPage(this.panel_參數設定, this.plC_ScreenPage_參數設定);
            PLC_UI_Init.Set_PLC_ScreenPage(this.panel_系統, this.plC_ScreenPage_系統);



            SQLUI.SQL_DataGridView.SQL_Set_Properties(dBConfigClass.DB_Basic, this.FindForm());
        

            this.Program_系統_Init();
            this.Program_工程模式_Init();
            this.Program_FaceID_Init();
            this.Program_人員資料_Init();
            this.Program_參數設定_藥檔資料_Init();
            this.Program_參數設定_套餐設定_Init();
            this.Program_參數設定_手術房設定_Init();
            this.Program_儲位管理_Init();
            this.Program_入庫作業_Init();
            this.Program_交易紀錄查詢_Init();
            this.Program_登入畫面_Init();
            this.Program_主畫面_Init();
            this.Program_Scanner_Init();
     
            this.plC_UI_Init1.Add_Method(this.Program_參數設定_藥檔資料);
            this.plC_UI_Init1.Add_Method(this.Program_參數設定_套餐設定);
            this.plC_UI_Init1.Add_Method(this.Program_參數設定_手術房設定);
            this.plC_UI_Init1.Add_Method(this.Program_儲位管理);
            this.plC_UI_Init1.Add_Method(this.Program_人員資料);
            this.plC_UI_Init1.Add_Method(this.Program_入庫作業);
            this.plC_UI_Init1.Add_Method(this.Program_工程模式);
            this.plC_UI_Init1.Add_Method(this.Program_交易紀錄查詢);
            this.plC_UI_Init1.Add_Method(this.Program_登入畫面);
            this.plC_UI_Init1.Add_Method(this.Program_主畫面);
            this.plC_UI_Init1.Add_Method(this.Program_Scanner);
           

            this.MyThread_FaceID = new MyThread();
            this.MyThread_FaceID.SetSleepTime(100);
            this.MyThread_FaceID.Add_Method(this.Program_FaceID);
            this.MyThread_FaceID.AutoRun(true);
            this.MyThread_FaceID.Trigger();

  
            this.MyThread_ProgramPLC = new MyThread();
            this.MyThread_ProgramPLC.SetSleepTime(1);
            this.MyThread_ProgramPLC.Add_Method(this.Program_PLC);
            this.MyThread_ProgramPLC.AutoRun(true);
            this.MyThread_ProgramPLC.Trigger();
        }

        #region PLC_Method
        PLC_Device PLC_Device_Method = new PLC_Device("");
        PLC_Device PLC_Device_Method_OK = new PLC_Device("");
        int cnt_Program_Method = 65534;
        void sub_Program_Method()
        {
            if (cnt_Program_Method == 65534)
            {
                PLC_Device_Method.SetComment("PLC_Method");
                PLC_Device_Method_OK.SetComment("PLC_Method_OK");
                PLC_Device_Method.Bool = false;
                cnt_Program_Method = 65535;
            }
            if (cnt_Program_Method == 65535) cnt_Program_Method = 1;
            if (cnt_Program_Method == 1) cnt_Program_Method_檢查按下(ref cnt_Program_Method);
            if (cnt_Program_Method == 2) cnt_Program_Method_初始化(ref cnt_Program_Method);
            if (cnt_Program_Method == 3) cnt_Program_Method = 65500;
            if (cnt_Program_Method > 1) cnt_Program_Method_檢查放開(ref cnt_Program_Method);

            if (cnt_Program_Method == 65500)
            {
                PLC_Device_Method.Bool = false;
                PLC_Device_Method_OK.Bool = false;
                cnt_Program_Method = 65535;
            }
        }
        void cnt_Program_Method_檢查按下(ref int cnt)
        {
            if (PLC_Device_Method.Bool) cnt++;
        }
        void cnt_Program_Method_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Method.Bool) cnt = 65500;
        }
        void cnt_Program_Method_初始化(ref int cnt)
        {

            cnt++;
        }












        #endregion
        private void Update()
        {
            if (this.ftp_DounloadUI.DownloadFile())
            {
                if (this.ftp_DounloadUI.SaveFile())
                {
                    this.ftp_DounloadUI.RunFile(this.FindForm());
                }
                else
                {
                    Basic.MyMessageBox.ShowDialog("安裝檔存檔失敗!");
                }
            }
            else
            {
                Basic.MyMessageBox.ShowDialog("下載失敗!");
            }
        }
        private void rJ_TextBox_CheckNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar <= 57 && (int)e.KeyChar >= 48) || (int)e.KeyChar == 8) // 8 > BackSpace
            {

                e.Handled = false;
            }
            else e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<int> list_value = new List<int>();

            list_value.Add(2);
            list_value.Add(3);
            list_value.Add(3);
            list_value.Add(3);
            list_value.Add(3);
            list_value.Add(3);
            list_value.Add(1);
            List<int> value = this.Function_數組找目標數值加總組合(list_value, 10);


        }


    }
}
