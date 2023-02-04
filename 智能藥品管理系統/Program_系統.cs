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
    public enum enum_藥檔資料
    {
        GUID,
        藥品碼,
        中文名稱,
        藥品名稱,
        藥品學名,
        健保碼,
        包裝單位,
        包裝數量,
        最小包裝單位,
        最小包裝數量,
        藥品條碼1,
        藥品條碼2,
    }
    public enum enum_效期批號維護
    {
        GUID,
        藥品碼,
        效期,
        批號,
        加入時間,
    }
    public partial class Form1 : Form
    {
        private void Program_系統_Init()
        {
            SQLUI.SQL_DataGridView.SQL_Set_Properties(sqL_DataGridView_雲端藥檔, dBConfigClass.DB_Medicine_Cloud);
            this.sqL_DataGridView_雲端藥檔.Init();

            this.sqL_DataGridView_效期批號維護.Init();
            if(!sqL_DataGridView_效期批號維護.SQL_IsTableCreat())
            {
                sqL_DataGridView_效期批號維護.SQL_CreateTable();
            }

            this.rfiD_FX600_UI.Init(2, RFID_FX600lib.RFID_FX600_UI.Baudrate._115200, "COM1");
            this.storageUI_WT32.Init(myConfigClass.DataBaseName, myConfigClass.UserName, myConfigClass.Password, myConfigClass.IP, myConfigClass.Port, myConfigClass.MySqlSslMode); ;
            this.wT32_GPADC.Init(storageUI_WT32.List_UDP_Local);

            this.loginUI.Login_data_DataBasename = myConfigClass.DataBaseName;
            this.loginUI.Login_data_UserName = myConfigClass.UserName;
            this.loginUI.Login_data_Password = myConfigClass.Password;
            this.loginUI.Login_data_Server = myConfigClass.IP;
            this.loginUI.Login_data_Port = myConfigClass.Port;
            this.loginUI.Login_data_mySqlSslMode = myConfigClass.MySqlSslMode;
            this.loginUI.Login_data_index_DataBasename = myConfigClass.DataBaseName;
            this.loginUI.Login_data_index_UserName = myConfigClass.UserName;
            this.loginUI.Login_data_index_Password = myConfigClass.Password;
            this.loginUI.Login_data_index_Server = myConfigClass.IP;
            this.loginUI.Login_data_index_Port = myConfigClass.Port;
            this.loginUI.Login_data_index_mySqlSslMode = myConfigClass.MySqlSslMode;
            this.loginUI.Init();


            this.plC_RJ_Button_雲端藥檔_取得資料.MouseDownEvent += PlC_RJ_Button_雲端藥檔_取得資料_MouseDownEvent;
        }

        private void PlC_RJ_Button_雲端藥檔_取得資料_MouseDownEvent(MouseEventArgs mevent)
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);

            List<object[]> list_value = this.sqL_DataGridView_雲端藥檔.SQL_GetAllRows(false);
            Console.WriteLine($"取得雲端藥檔資料共{list_value.Count}筆,耗時:{myTimer.ToString()}ms");
            this.sqL_DataGridView_雲端藥檔.RefreshGrid(list_value);
            Console.WriteLine($"顯示雲端藥檔資料,耗時:{myTimer.ToString()}ms");

        }
    }
}
