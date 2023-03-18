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
    public partial class Form1 : Form
    {
        private PLC_Device PLC_Device_X軸_偏移距離 = new PLC_Device("D4010");
        private PLC_Device PLC_Device_X軸_加減速度 = new PLC_Device("D4011");
        private PLC_Device PLC_Device_X軸_JOG速度 = new PLC_Device("D4012");
        private PLC_Device PLC_Device_X軸_運轉速度 = new PLC_Device("D4013");

        private PLC_Device PLC_Device_Y軸_偏移距離 = new PLC_Device("D4020");
        private PLC_Device PLC_Device_Y軸_加減速度 = new PLC_Device("D4021");
        private PLC_Device PLC_Device_Y軸_JOG速度 = new PLC_Device("D4022");
        private PLC_Device PLC_Device_Y軸_運轉速度 = new PLC_Device("D4023");

        private PLC_Device PLC_Device_取物門_偏移距離 = new PLC_Device("D4030");
        private PLC_Device PLC_Device_取物門_加減速度 = new PLC_Device("D4031");
        private PLC_Device PLC_Device_取物門_JOG速度 = new PLC_Device("D4032");
        private PLC_Device PLC_Device_取物門_運轉速度 = new PLC_Device("D4033");
        private PLC_Device PLC_Device_取物門_開門位置 = new PLC_Device("D4034");
        public enum enum_軸號
        {
            X軸 = 0,
            Y軸 = 1,
            取物門 = 2,
        }


        private void PLSV(enum_軸號 axis_num, int active_speed, int Tacc)
        {
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, true);
            this.dmC1000B1.PLSV((int)axis_num, active_speed, 0, Tacc, LeadShineUI.DMC1000B.enum_Axis_SpeedMode.T_Mode);
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, false);
        }
        private void DRVA(enum_軸號 axis_num, int target_position, int active_speed, int Tacc)
        {
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, true);
            this.dmC1000B1.DRVA((int)axis_num, target_position, active_speed, 0, Tacc, LeadShineUI.DMC1000B.enum_Axis_SpeedMode.T_Mode);
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, false);
        }
        private void AxisStop(enum_軸號 axis_num)
        {
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, true);
            this.dmC1000B1.AxisStop((int)axis_num);
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, false);
        }
        private void AxisStopEmg(enum_軸號 axis_num)
        {
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, true);
            this.dmC1000B1.AxisStopEmg((int)axis_num);
            this.dmC1000B1.Set_PC_ControlEnable((int)axis_num, false);
        }
        private bool Get_ORG(enum_軸號 axis_num)
        {
            return this.dmC1000B1.Get_Axis_Input((int)axis_num, LeadShineUI.DMC1000B_Basic.enum_Axis_Input.ORG);
        }
        private bool Get_EL_P(enum_軸號 axis_num)
        {
            return this.dmC1000B1.Get_Axis_Input((int)axis_num, LeadShineUI.DMC1000B_Basic.enum_Axis_Input.P_EL);
        }
        private bool Get_EL_N(enum_軸號 axis_num)
        {
            return this.dmC1000B1.Get_Axis_Input((int)axis_num, LeadShineUI.DMC1000B_Basic.enum_Axis_Input.N_EL);
        }
        private bool Get_Axis_Ready(enum_軸號 axis_num, int target_position)
        {
            return (this.dmC1000B1.GetAxisCmdPos((int)axis_num) == target_position);
        }
        private bool 取得送料馬達出料原點(int index)
        {
            return this.ioC12801.GetInput(0, index + 1);
        }
        private bool 取得取藥防夾訊號()
        {
            return this.ioC12801.GetInput(0, 29);
        }
        static bool flag_送料馬達輸出 = false;
        static MyTimer MyTimer_送料馬達輸出_ON = new MyTimer();
        static MyTimer MyTimer_送料馬達輸出_OFF = new MyTimer();
        private void 送料馬達輸出(int 格數, int 層數)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i != 格數) this.ioC12801.SetOutput(0, i + 1, false);
                if (i != 層數) this.ioC12801.SetOutput(0, i + 9, false);
            }
            this.ioC12801.SetOutput(0, 格數 + 1, true);
            this.ioC12801.SetOutput(0, 層數 + 9, true);
            return;
            if (flag_送料馬達輸出)
            {
                MyTimer_送料馬達輸出_ON.StartTickTime(1);
                this.ioC12801.SetOutput(0, 格數 + 1, true);
                this.ioC12801.SetOutput(0, 層數 + 9, true);
                if(MyTimer_送料馬達輸出_ON.IsTimeOut())
                {
                    flag_送料馬達輸出 = !flag_送料馬達輸出;
                }
            }
            if(!flag_送料馬達輸出)
            {
                MyTimer_送料馬達輸出_OFF.StartTickTime(2);
                this.ioC12801.SetOutput(0, 格數 + 1, false);
                this.ioC12801.SetOutput(0, 層數 + 9, false);
                if (MyTimer_送料馬達輸出_OFF.IsTimeOut())
                {
                    flag_送料馬達輸出 = !flag_送料馬達輸出;
                }
            }
        }
        private bool 取得送料馬達輸出(int 格數, int 層數)
        {
            if(!this.ioC12801.GetOutput(0, 格數 + 1))
            {
                return false;
            }
            if(!this.ioC12801.GetOutput(0, 層數 + 9))
            {
                return false;
            }
            return true;
        }
        private void 送料馬達停止(int 格數)
        {
            this.ioC12801.SetOutput(0, 格數 + 1, false);
        }
        private void 送料馬達停止()
        {
            for (int i = 0; i < 6; i++)
            {
                this.ioC12801.SetOutput(0, i + 1, false);
                this.ioC12801.SetOutput(0, i + 9, false);
            }
        }
        private void Set_CommandPos(enum_軸號 axis_num)
        {
            this.dmC1000B1.SetAxisCmdPos((int)axis_num, 0);
        }

        PLC_Device PLC_Device_退藥抽屜開啟警報 = new PLC_Device("S810");
        PLC_Device PLC_Device_取藥門開啟警報 = new PLC_Device("S811");
        PLC_Device PLC_Device_維修門開啟警報 = new PLC_Device("S812");

        private void Program_PLC()
        {
            sub_Program_抽屜開鎖();

            sub_Program_X軸_Jog_N();
            sub_Program_X軸_Jog_P();
            sub_Program_X軸_復歸();

            sub_Program_Y軸_Jog_N();
            sub_Program_Y軸_Jog_P();
            sub_Program_Y軸_復歸();

            sub_Program_取物門_Jog_N();
            sub_Program_取物門_Jog_P();
            sub_Program_取物門_復歸();
            sub_Program_取物門_移動到開門位置();
            sub_Program_取物門_移動到關門位置();

            sub_Program_XY_Table_移動();
            sub_Program_全軸復歸();
            sub_Program_送料馬達出料();
            sub_Program_出料一次();
            sub_Program_輸送帶();
            sub_Program_移動至出貨位置();
            sub_Program_退藥鎖();
            sub_Program_面板鎖();
            sub_Program_X軸安全位置移動();
            sub_Program_清空出料盤();

            if (!PLC_Device_退藥鎖_輸入.Bool)
            {
                PLC_Device_退藥抽屜開啟警報.Bool = true;
            }
            else
            {
                PLC_Device_退藥抽屜開啟警報.Bool = false;
            }


            if(!PLC_Device_面板鎖_上輸入.Bool || !PLC_Device_面板鎖_下輸入.Bool)
            {
                PLC_Device_維修門開啟警報.Bool = true;
            }
            else
            {
                PLC_Device_維修門開啟警報.Bool = false;
            }
        }

        #region PLC_抽屜開鎖
        List<object[]> list_抽屜開鎖_儲位資料 = new List<object[]>();
        PLC_Device PLC_Device_抽屜開鎖 = new PLC_Device("S7005");
        PLC_Device PLC_Device_抽屜開鎖_OK = new PLC_Device("S7006");
        PLC_Device PLC_Device_抽屜開鎖_層數 = new PLC_Device("D4000");
        PLC_Device PLC_Device_抽屜開鎖_格數 = new PLC_Device("D4001");
        MyTimer MyTimer_抽屜開鎖_開鎖時間 = new MyTimer("D4002");
        List<PLC_Device> List_PLC_Device_抽屜層數 = new List<PLC_Device>();
        List<PLC_Device> List_PLC_Device_抽屜格數 = new List<PLC_Device>();
        int cnt_Program_抽屜開鎖 = 65534;
        void sub_Program_抽屜開鎖()
        {
            if (cnt_Program_抽屜開鎖 == 65534)
            {
                PLC_Device_抽屜開鎖.SetComment("PLC_抽屜開鎖");
                PLC_Device_抽屜開鎖_OK.SetComment("PLC_抽屜開鎖_OK");
                PLC_Device_抽屜開鎖_層數.SetComment("PLC_Device_抽屜開鎖_層數");
                PLC_Device_抽屜開鎖_格數.SetComment("PLC_Device_抽屜開鎖_格數");
                MyTimer_抽屜開鎖_開鎖時間.SetComment("PLC_Device_抽屜開鎖_開鎖時間");

                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y70"));
                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y71"));
                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y72"));
                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y73"));
                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y74"));
                List_PLC_Device_抽屜層數.Add(new PLC_Device("Y75"));

                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y60"));
                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y61"));
                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y62"));
                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y63"));
                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y64"));
                List_PLC_Device_抽屜格數.Add(new PLC_Device("Y65"));

                for (int i = 0; i < List_PLC_Device_抽屜層數.Count; i++) List_PLC_Device_抽屜層數[i].Bool = false;
                for (int i = 0; i < List_PLC_Device_抽屜格數.Count; i++) List_PLC_Device_抽屜格數[i].Bool = false;
                PLC_Device_抽屜開鎖.Bool = false;
                cnt_Program_抽屜開鎖 = 65535;
            }
            if (cnt_Program_抽屜開鎖 == 65535) cnt_Program_抽屜開鎖 = 1;
            if (cnt_Program_抽屜開鎖 == 1) cnt_Program_抽屜開鎖_檢查按下(ref cnt_Program_抽屜開鎖);
            if (cnt_Program_抽屜開鎖 == 2) cnt_Program_抽屜開鎖_初始化(ref cnt_Program_抽屜開鎖);
            if (cnt_Program_抽屜開鎖 == 3) cnt_Program_抽屜開鎖_開始輸出(ref cnt_Program_抽屜開鎖);
            if (cnt_Program_抽屜開鎖 == 4) cnt_Program_抽屜開鎖_檢查輸出完成(ref cnt_Program_抽屜開鎖);
            if (cnt_Program_抽屜開鎖 == 5) cnt_Program_抽屜開鎖 = 65500;
            if (cnt_Program_抽屜開鎖 > 1) cnt_Program_抽屜開鎖_檢查放開(ref cnt_Program_抽屜開鎖);

            if (cnt_Program_抽屜開鎖 == 65500)
            {

                for (int i = 0; i < List_PLC_Device_抽屜層數.Count; i++) List_PLC_Device_抽屜層數[i].Bool = false;
                for (int i = 0; i < List_PLC_Device_抽屜格數.Count; i++) List_PLC_Device_抽屜格數[i].Bool = false;
                this.MyTimer_抽屜開鎖_開鎖時間.TickStop();
                this.MyTimer_抽屜開鎖_開鎖時間.StartTickTime();

                PLC_Device_抽屜開鎖.Bool = false;
                cnt_Program_抽屜開鎖 = 65535;
            }
        }
        void cnt_Program_抽屜開鎖_檢查按下(ref int cnt)
        {
            if (PLC_Device_抽屜開鎖.Bool) cnt++;
        }
        void cnt_Program_抽屜開鎖_檢查放開(ref int cnt)
        {
            if (!PLC_Device_抽屜開鎖.Bool) cnt = 65500;
        }
        void cnt_Program_抽屜開鎖_初始化(ref int cnt)
        {
            this.list_抽屜開鎖_儲位資料 = this.Function_儲位管理_儲位資料_取得儲位資料(false);
            for (int i = 0; i < List_PLC_Device_抽屜層數.Count; i++) List_PLC_Device_抽屜層數[i].Bool = false;
            for (int i = 0; i < List_PLC_Device_抽屜格數.Count; i++) List_PLC_Device_抽屜格數[i].Bool = false;
            this.MyTimer_抽屜開鎖_開鎖時間.TickStop();
            this.MyTimer_抽屜開鎖_開鎖時間.StartTickTime();
            PLC_Device_抽屜開鎖_OK.Bool = false;
            cnt++;
        }
        void cnt_Program_抽屜開鎖_開始輸出(ref int cnt)
        {
            int 層數 = PLC_Device_抽屜開鎖_層數.Value;
            int 格數 = PLC_Device_抽屜開鎖_格數.Value;
            this.List_PLC_Device_抽屜層數[層數].Bool = true;
            this.List_PLC_Device_抽屜格數[格數].Bool = true;
            cnt++;
        }
        void cnt_Program_抽屜開鎖_檢查輸出完成(ref int cnt)
        {
        
            int 層數 = PLC_Device_抽屜開鎖_層數.Value;
            int 格數 = PLC_Device_抽屜開鎖_格數.Value;
            string str = $"{層數 + 1}-{格數 + 1}";
            List<object[]> list_value = this.list_抽屜開鎖_儲位資料.GetRows((int)enum_儲位管理_儲位資料.位置, str);
            if (MyTimer_抽屜開鎖_開鎖時間.IsTimeOut())
            {
                cnt++;
                return;
            }
            if (list_value.Count > 0)
            {
                string IP = list_value[0][(int)enum_儲位管理_儲位資料.IP].ObjectToString();
                string jsonstring = this.storageUI_WT32.GetUDPJsonString(IP);
                StorageUI_WT32.UDP_READ uDP_READ = this.wT32_GPADC.Get_JSON_String_Class(jsonstring);
                if(uDP_READ != null)
                {
                    if (uDP_READ.INPUT_LOCK == 0)
                    {
                        PLC_Device_抽屜開鎖_OK.Bool = true;
                        cnt++;
                        return;
                    }
                }
            
            }
        }




        #endregion
        #region PLC_X軸_Jog_N
        PLC_Device PLC_Device_X軸_Jog_N = new PLC_Device("S7025");
        PLC_Device PLC_Device_X軸_Jog_N_OK = new PLC_Device("S7026");
        int cnt_Program_X軸_Jog_N = 65534;
        void sub_Program_X軸_Jog_N()
        {
            if (cnt_Program_X軸_Jog_N == 65534)
            {
                PLC_Device_X軸_Jog_N.SetComment("PLC_X軸_Jog_N");
                PLC_Device_X軸_Jog_N_OK.SetComment("PLC_X軸_Jog_N_OK");
                PLC_Device_X軸_Jog_N.Bool = false;
                cnt_Program_X軸_Jog_N = 65535;
            }
            if (cnt_Program_X軸_Jog_N == 65535) cnt_Program_X軸_Jog_N = 1;
            if (cnt_Program_X軸_Jog_N == 1) cnt_Program_X軸_Jog_N_檢查按下(ref cnt_Program_X軸_Jog_N);
            if (cnt_Program_X軸_Jog_N == 2) cnt_Program_X軸_Jog_N_初始化(ref cnt_Program_X軸_Jog_N);
            if (cnt_Program_X軸_Jog_N == 3) cnt_Program_X軸_Jog_N = 65500;
            if (cnt_Program_X軸_Jog_N > 1) cnt_Program_X軸_Jog_N_檢查放開(ref cnt_Program_X軸_Jog_N);

            if (cnt_Program_X軸_Jog_N == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                PLC_Device_X軸_Jog_N.Bool = false;
                PLC_Device_X軸_Jog_N_OK.Bool = false;
                cnt_Program_X軸_Jog_N = 65535;
            }
        }
        void cnt_Program_X軸_Jog_N_檢查按下(ref int cnt)
        {
            if (PLC_Device_X軸_Jog_N.Bool) cnt++;
        }
        void cnt_Program_X軸_Jog_N_檢查放開(ref int cnt)
        {
            if (!PLC_Device_X軸_Jog_N.Bool) cnt = 65500;
        }
        void cnt_Program_X軸_Jog_N_初始化(ref int cnt)
        {
            PLSV(enum_軸號.X軸, -PLC_Device_X軸_JOG速度.Value, PLC_Device_X軸_加減速度.Value);

        }












        #endregion
        #region PLC_X軸_Jog_P
        PLC_Device PLC_Device_X軸_Jog_P = new PLC_Device("S7045");
        PLC_Device PLC_Device_X軸_Jog_P_OK = new PLC_Device("S7046");
        int cnt_Program_X軸_Jog_P = 65534;
        void sub_Program_X軸_Jog_P()
        {
            if (cnt_Program_X軸_Jog_P == 65534)
            {
                PLC_Device_X軸_Jog_P.SetComment("PLC_X軸_Jog_P");
                PLC_Device_X軸_Jog_P_OK.SetComment("PLC_X軸_Jog_P_OK");
                PLC_Device_X軸_Jog_P.Bool = false;
                cnt_Program_X軸_Jog_P = 65535;
            }
            if (cnt_Program_X軸_Jog_P == 65535) cnt_Program_X軸_Jog_P = 1;
            if (cnt_Program_X軸_Jog_P == 1) cnt_Program_X軸_Jog_P_檢查按下(ref cnt_Program_X軸_Jog_P);
            if (cnt_Program_X軸_Jog_P == 2) cnt_Program_X軸_Jog_P_初始化(ref cnt_Program_X軸_Jog_P);
            if (cnt_Program_X軸_Jog_P == 3) cnt_Program_X軸_Jog_P = 65500;
            if (cnt_Program_X軸_Jog_P > 1) cnt_Program_X軸_Jog_P_檢查放開(ref cnt_Program_X軸_Jog_P);

            if (cnt_Program_X軸_Jog_P == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                PLC_Device_X軸_Jog_P.Bool = false;
                PLC_Device_X軸_Jog_P_OK.Bool = false;
                cnt_Program_X軸_Jog_P = 65535;
            }
        }
        void cnt_Program_X軸_Jog_P_檢查按下(ref int cnt)
        {
            if (PLC_Device_X軸_Jog_P.Bool) cnt++;
        }
        void cnt_Program_X軸_Jog_P_檢查放開(ref int cnt)
        {
            if (!PLC_Device_X軸_Jog_P.Bool) cnt = 65500;
        }
        void cnt_Program_X軸_Jog_P_初始化(ref int cnt)
        {
            PLSV(enum_軸號.X軸, +PLC_Device_X軸_JOG速度.Value, PLC_Device_X軸_加減速度.Value);

        }












        #endregion
        #region PLC_X軸_復歸
        PLC_Device PLC_Device_X軸_復歸 = new PLC_Device("S7065");
        PLC_Device PLC_Device_X軸_復歸_OK = new PLC_Device("S7066");
        PLC_Device PLC_Device_X軸_復歸_高速速度 = new PLC_Device("K10000");
        PLC_Device PLC_Device_X軸_復歸_中速速度 = new PLC_Device("K2000");
        PLC_Device PLC_Device_X軸_復歸_低速速度 = new PLC_Device("K100");
        MyTimer MyTimer_X軸_復歸_歸零延遲 = new MyTimer();
        MyTimer MyTimer_X軸_復歸_轉向延遲 = new MyTimer();
        int cnt_Program_X軸_復歸 = 65534;
        void sub_Program_X軸_復歸()
        {
            if (cnt_Program_X軸_復歸 == 65534)
            {
                PLC_Device_X軸_復歸.SetComment("PLC_X軸_復歸");
                PLC_Device_X軸_復歸_OK.SetComment("PLC_X軸_復歸_OK");
                cnt_Program_X軸_復歸 = 65535;
            }
            if (cnt_Program_X軸_復歸 == 65535) cnt_Program_X軸_復歸 = 1;
            if (cnt_Program_X軸_復歸 == 1) cnt_Program_X軸_復歸_檢查按下(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 2) cnt_Program_X軸_復歸_初始化(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 3) cnt_Program_X軸_復歸_高速往負極限(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 4) cnt_Program_X軸_復歸_等待高速往負極限(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 5) cnt_Program_X軸_復歸_中速從負極限靠近原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 6) cnt_Program_X軸_復歸_等待中速靠近原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 7) cnt_Program_X軸_復歸_中速離開原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 8) cnt_Program_X軸_復歸_等待中速離開原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 9) cnt_Program_X軸_復歸_低速靠近原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 10) cnt_Program_X軸_復歸_等待低速靠近原點(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 11) cnt_Program_X軸_復歸_第一次Pos歸零(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 12) cnt_Program_X軸_復歸_開始偏移(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 13) cnt_Program_X軸_復歸_等待偏移完成(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 14) cnt_Program_X軸_復歸_第二次Pos歸零(ref cnt_Program_X軸_復歸);
            if (cnt_Program_X軸_復歸 == 15) cnt_Program_X軸_復歸 = 65500;
            if (cnt_Program_X軸_復歸 > 1) cnt_Program_X軸_復歸_檢查放開(ref cnt_Program_X軸_復歸);

            if (cnt_Program_X軸_復歸 == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                PLC_Device_X軸_復歸.Bool = false;
                cnt_Program_X軸_復歸 = 65535;
            }
        }
        void cnt_Program_X軸_復歸_檢查按下(ref int cnt)
        {
            if (PLC_Device_X軸_復歸.Bool) cnt++;
        }
        void cnt_Program_X軸_復歸_檢查放開(ref int cnt)
        {
            if (!PLC_Device_X軸_復歸.Bool) cnt = 65500;
        }
        void cnt_Program_X軸_復歸_初始化(ref int cnt)
        {
            PLC_Device_X軸_復歸_OK.Bool = false;
            cnt++;
        }
        void cnt_Program_X軸_復歸_高速往負極限(ref int cnt)
        {
            this.PLSV(enum_軸號.X軸, -PLC_Device_X軸_復歸_高速速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸_復歸_等待高速往負極限(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.X軸))
            {
                this.MyTimer_X軸_復歸_轉向延遲.TickStop();
                this.MyTimer_X軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
            if (this.Get_EL_N(enum_軸號.X軸))
            {
                this.MyTimer_X軸_復歸_轉向延遲.TickStop();
                this.MyTimer_X軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_中速從負極限靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.X軸, PLC_Device_X軸_復歸_中速速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸_復歸_等待中速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.X軸))
            {
                this.MyTimer_X軸_復歸_轉向延遲.TickStop();
                this.MyTimer_X軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_中速離開原點(ref int cnt)
        {
            this.PLSV(enum_軸號.X軸, PLC_Device_X軸_復歸_中速速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸_復歸_等待中速離開原點(ref int cnt)
        {
            if (!this.Get_ORG(enum_軸號.X軸))
            {
                this.MyTimer_X軸_復歸_轉向延遲.TickStop();
                this.MyTimer_X軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_低速靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.X軸, -PLC_Device_X軸_復歸_低速速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸_復歸_等待低速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.X軸))
            {
                this.MyTimer_X軸_復歸_歸零延遲.TickStop();
                this.MyTimer_X軸_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_第一次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_X軸_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_開始偏移(ref int cnt)
        {
            this.DRVA(enum_軸號.X軸, PLC_Device_X軸_偏移距離.Value, PLC_Device_X軸_運轉速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸_復歸_等待偏移完成(ref int cnt)
        {
            if (this.Get_Axis_Ready(enum_軸號.X軸, PLC_Device_X軸_偏移距離.Value))
            {
                this.MyTimer_X軸_復歸_歸零延遲.TickStop();
                this.MyTimer_X軸_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_X軸_復歸_第二次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_X軸_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.X軸);
                cnt++;
                return;
            }
        }
        #endregion

        #region PLC_Y軸_Jog_N
        PLC_Device PLC_Device_Y軸_Jog_N = new PLC_Device("S7105");
        PLC_Device PLC_Device_Y軸_Jog_N_OK = new PLC_Device("S7106");
        int cnt_Program_Y軸_Jog_N = 65534;
        void sub_Program_Y軸_Jog_N()
        {
            if (cnt_Program_Y軸_Jog_N == 65534)
            {
                PLC_Device_Y軸_Jog_N.SetComment("PLC_Y軸_Jog_N");
                PLC_Device_Y軸_Jog_N_OK.SetComment("PLC_Y軸_Jog_N_OK");
                PLC_Device_Y軸_Jog_N.Bool = false;
                cnt_Program_Y軸_Jog_N = 65535;
            }
            if (cnt_Program_Y軸_Jog_N == 65535) cnt_Program_Y軸_Jog_N = 1;
            if (cnt_Program_Y軸_Jog_N == 1) cnt_Program_Y軸_Jog_N_檢查按下(ref cnt_Program_Y軸_Jog_N);
            if (cnt_Program_Y軸_Jog_N == 2) cnt_Program_Y軸_Jog_N_初始化(ref cnt_Program_Y軸_Jog_N);
            if (cnt_Program_Y軸_Jog_N == 3) cnt_Program_Y軸_Jog_N = 65500;
            if (cnt_Program_Y軸_Jog_N > 1) cnt_Program_Y軸_Jog_N_檢查放開(ref cnt_Program_Y軸_Jog_N);

            if (cnt_Program_Y軸_Jog_N == 65500)
            {
                this.AxisStop(enum_軸號.Y軸);
                PLC_Device_Y軸_Jog_N.Bool = false;
                PLC_Device_Y軸_Jog_N_OK.Bool = false;
                cnt_Program_Y軸_Jog_N = 65535;
            }
        }
        void cnt_Program_Y軸_Jog_N_檢查按下(ref int cnt)
        {
            if (PLC_Device_Y軸_Jog_N.Bool) cnt++;
        }
        void cnt_Program_Y軸_Jog_N_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Y軸_Jog_N.Bool) cnt = 65500;
        }
        void cnt_Program_Y軸_Jog_N_初始化(ref int cnt)
        {
            PLSV(enum_軸號.Y軸, -PLC_Device_Y軸_JOG速度.Value, PLC_Device_Y軸_加減速度.Value);

        }












        #endregion
        #region PLC_Y軸_Jog_P
        PLC_Device PLC_Device_Y軸_Jog_P = new PLC_Device("S7125");
        PLC_Device PLC_Device_Y軸_Jog_P_OK = new PLC_Device("S7126");
        int cnt_Program_Y軸_Jog_P = 65534;
        void sub_Program_Y軸_Jog_P()
        {
            if (cnt_Program_Y軸_Jog_P == 65534)
            {
                PLC_Device_Y軸_Jog_P.SetComment("PLC_Y軸_Jog_P");
                PLC_Device_Y軸_Jog_P_OK.SetComment("PLC_Y軸_Jog_P_OK");
                PLC_Device_Y軸_Jog_P.Bool = false;
                cnt_Program_Y軸_Jog_P = 65535;
            }
            if (cnt_Program_Y軸_Jog_P == 65535) cnt_Program_Y軸_Jog_P = 1;
            if (cnt_Program_Y軸_Jog_P == 1) cnt_Program_Y軸_Jog_P_檢查按下(ref cnt_Program_Y軸_Jog_P);
            if (cnt_Program_Y軸_Jog_P == 2) cnt_Program_Y軸_Jog_P_初始化(ref cnt_Program_Y軸_Jog_P);
            if (cnt_Program_Y軸_Jog_P == 3) cnt_Program_Y軸_Jog_P = 65500;
            if (cnt_Program_Y軸_Jog_P > 1) cnt_Program_Y軸_Jog_P_檢查放開(ref cnt_Program_Y軸_Jog_P);

            if (cnt_Program_Y軸_Jog_P == 65500)
            {
                this.AxisStop(enum_軸號.Y軸);
                PLC_Device_Y軸_Jog_P.Bool = false;
                PLC_Device_Y軸_Jog_P_OK.Bool = false;
                cnt_Program_Y軸_Jog_P = 65535;
            }
        }
        void cnt_Program_Y軸_Jog_P_檢查按下(ref int cnt)
        {
            if (PLC_Device_Y軸_Jog_P.Bool) cnt++;
        }
        void cnt_Program_Y軸_Jog_P_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Y軸_Jog_P.Bool) cnt = 65500;
        }
        void cnt_Program_Y軸_Jog_P_初始化(ref int cnt)
        {
            PLSV(enum_軸號.Y軸, +PLC_Device_Y軸_JOG速度.Value, PLC_Device_Y軸_加減速度.Value);

        }












        #endregion
        #region PLC_Y軸_復歸
        PLC_Device PLC_Device_Y軸_復歸 = new PLC_Device("S7145");
        PLC_Device PLC_Device_Y軸_復歸_OK = new PLC_Device("S7146");
        PLC_Device PLC_Device_Y軸_復歸_高速速度 = new PLC_Device("K5000");
        PLC_Device PLC_Device_Y軸_復歸_中速速度 = new PLC_Device("K1000");
        PLC_Device PLC_Device_Y軸_復歸_低速速度 = new PLC_Device("K100");
        MyTimer MyTimer_Y軸_復歸_歸零延遲 = new MyTimer();
        MyTimer MyTimer_Y軸_復歸_轉向延遲 = new MyTimer();
        int cnt_Program_Y軸_復歸 = 65534;
        void sub_Program_Y軸_復歸()
        {
            if (cnt_Program_Y軸_復歸 == 65534)
            {
                PLC_Device_Y軸_復歸.SetComment("PLC_Y軸_復歸");
                PLC_Device_Y軸_復歸_OK.SetComment("PLC_Y軸_復歸_OK");
                cnt_Program_Y軸_復歸 = 65535;
            }
            if (cnt_Program_Y軸_復歸 == 65535) cnt_Program_Y軸_復歸 = 1;
            if (cnt_Program_Y軸_復歸 == 1) cnt_Program_Y軸_復歸_檢查按下(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 2) cnt_Program_Y軸_復歸_初始化(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 3) cnt_Program_Y軸_復歸_高速往負極限(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 4) cnt_Program_Y軸_復歸_等待高速往負極限(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 5) cnt_Program_Y軸_復歸_中速從負極限靠近原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 6) cnt_Program_Y軸_復歸_等待中速靠近原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 7) cnt_Program_Y軸_復歸_中速離開原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 8) cnt_Program_Y軸_復歸_等待中速離開原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 9) cnt_Program_Y軸_復歸_低速靠近原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 10) cnt_Program_Y軸_復歸_等待低速靠近原點(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 11) cnt_Program_Y軸_復歸_第一次Pos歸零(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 12) cnt_Program_Y軸_復歸_開始偏移(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 13) cnt_Program_Y軸_復歸_等待偏移完成(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 14) cnt_Program_Y軸_復歸_第二次Pos歸零(ref cnt_Program_Y軸_復歸);
            if (cnt_Program_Y軸_復歸 == 15) cnt_Program_Y軸_復歸 = 65500;
            if (cnt_Program_Y軸_復歸 > 1) cnt_Program_Y軸_復歸_檢查放開(ref cnt_Program_Y軸_復歸);

            if (cnt_Program_Y軸_復歸 == 65500)
            {
                this.AxisStop(enum_軸號.Y軸);
                PLC_Device_Y軸_復歸.Bool = false;
                cnt_Program_Y軸_復歸 = 65535;
            }
        }
        void cnt_Program_Y軸_復歸_檢查按下(ref int cnt)
        {
            if (PLC_Device_Y軸_復歸.Bool) cnt++;
        }
        void cnt_Program_Y軸_復歸_檢查放開(ref int cnt)
        {
            if (!PLC_Device_Y軸_復歸.Bool) cnt = 65500;
        }
        void cnt_Program_Y軸_復歸_初始化(ref int cnt)
        {
            PLC_Device_Y軸_復歸_OK.Bool = false;
            cnt++;
        }
        void cnt_Program_Y軸_復歸_高速往負極限(ref int cnt)
        {
            this.PLSV(enum_軸號.Y軸, -PLC_Device_Y軸_復歸_高速速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_Y軸_復歸_等待高速往負極限(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.Y軸))
            {
                this.MyTimer_Y軸_復歸_轉向延遲.TickStop();
                this.MyTimer_Y軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
            if (this.Get_EL_N(enum_軸號.Y軸))
            {
                this.MyTimer_Y軸_復歸_轉向延遲.TickStop();
                this.MyTimer_Y軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_中速從負極限靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.Y軸, PLC_Device_Y軸_復歸_中速速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_Y軸_復歸_等待中速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.Y軸))
            {
                this.MyTimer_Y軸_復歸_轉向延遲.TickStop();
                this.MyTimer_Y軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_中速離開原點(ref int cnt)
        {
            this.PLSV(enum_軸號.Y軸, PLC_Device_Y軸_復歸_中速速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_Y軸_復歸_等待中速離開原點(ref int cnt)
        {
            if (!this.Get_ORG(enum_軸號.Y軸))
            {
                this.MyTimer_Y軸_復歸_轉向延遲.TickStop();
                this.MyTimer_Y軸_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_低速靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.Y軸, -PLC_Device_Y軸_復歸_低速速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_Y軸_復歸_等待低速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.Y軸))
            {
                this.MyTimer_Y軸_復歸_歸零延遲.TickStop();
                this.MyTimer_Y軸_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_第一次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_Y軸_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_開始偏移(ref int cnt)
        {
            this.DRVA(enum_軸號.Y軸, PLC_Device_Y軸_偏移距離.Value, PLC_Device_Y軸_運轉速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_Y軸_復歸_等待偏移完成(ref int cnt)
        {
            if (this.Get_Axis_Ready(enum_軸號.Y軸, PLC_Device_Y軸_偏移距離.Value))
            {
                this.MyTimer_Y軸_復歸_歸零延遲.TickStop();
                this.MyTimer_Y軸_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        void cnt_Program_Y軸_復歸_第二次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_Y軸_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.Y軸);
                cnt++;
                return;
            }
        }
        #endregion

        #region PLC_取物門_Jog_N
        PLC_Device PLC_Device_取物門_Jog_N = new PLC_Device("S7205");
        PLC_Device PLC_Device_取物門_Jog_N_OK = new PLC_Device("S7206");
        int cnt_Program_取物門_Jog_N = 65534;
        void sub_Program_取物門_Jog_N()
        {
            if (cnt_Program_取物門_Jog_N == 65534)
            {
                PLC_Device_取物門_Jog_N.SetComment("PLC_取物門_Jog_N");
                PLC_Device_取物門_Jog_N_OK.SetComment("PLC_取物門_Jog_N_OK");
                PLC_Device_取物門_Jog_N.Bool = false;
                cnt_Program_取物門_Jog_N = 65535;
            }
            if (cnt_Program_取物門_Jog_N == 65535) cnt_Program_取物門_Jog_N = 1;
            if (cnt_Program_取物門_Jog_N == 1) cnt_Program_取物門_Jog_N_檢查按下(ref cnt_Program_取物門_Jog_N);
            if (cnt_Program_取物門_Jog_N == 2) cnt_Program_取物門_Jog_N_初始化(ref cnt_Program_取物門_Jog_N);
            if (cnt_Program_取物門_Jog_N == 3) cnt_Program_取物門_Jog_N = 65500;
            if (cnt_Program_取物門_Jog_N > 1) cnt_Program_取物門_Jog_N_檢查放開(ref cnt_Program_取物門_Jog_N);

            if (cnt_Program_取物門_Jog_N == 65500)
            {
                this.AxisStop(enum_軸號.取物門);
                PLC_Device_取物門_Jog_N.Bool = false;
                PLC_Device_取物門_Jog_N_OK.Bool = false;
                cnt_Program_取物門_Jog_N = 65535;
            }
        }
        void cnt_Program_取物門_Jog_N_檢查按下(ref int cnt)
        {
            if (PLC_Device_取物門_Jog_N.Bool) cnt++;
        }
        void cnt_Program_取物門_Jog_N_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取物門_Jog_N.Bool) cnt = 65500;
        }
        void cnt_Program_取物門_Jog_N_初始化(ref int cnt)
        {
            PLSV(enum_軸號.取物門, -PLC_Device_取物門_JOG速度.Value, PLC_Device_取物門_加減速度.Value);

        }












        #endregion
        #region PLC_取物門_Jog_P
        PLC_Device PLC_Device_取物門_Jog_P = new PLC_Device("S7225");
        PLC_Device PLC_Device_取物門_Jog_P_OK = new PLC_Device("S7226");
        int cnt_Program_取物門_Jog_P = 65534;
        void sub_Program_取物門_Jog_P()
        {
            if (cnt_Program_取物門_Jog_P == 65534)
            {
                PLC_Device_取物門_Jog_P.SetComment("PLC_取物門_Jog_P");
                PLC_Device_取物門_Jog_P_OK.SetComment("PLC_取物門_Jog_P_OK");
                PLC_Device_取物門_Jog_P.Bool = false;
                cnt_Program_取物門_Jog_P = 65535;
            }
            if (cnt_Program_取物門_Jog_P == 65535) cnt_Program_取物門_Jog_P = 1;
            if (cnt_Program_取物門_Jog_P == 1) cnt_Program_取物門_Jog_P_檢查按下(ref cnt_Program_取物門_Jog_P);
            if (cnt_Program_取物門_Jog_P == 2) cnt_Program_取物門_Jog_P_初始化(ref cnt_Program_取物門_Jog_P);
            if (cnt_Program_取物門_Jog_P == 3) cnt_Program_取物門_Jog_P = 65500;
            if (cnt_Program_取物門_Jog_P > 1) cnt_Program_取物門_Jog_P_檢查放開(ref cnt_Program_取物門_Jog_P);

            if (cnt_Program_取物門_Jog_P == 65500)
            {
                this.AxisStop(enum_軸號.取物門);
                PLC_Device_取物門_Jog_P.Bool = false;
                PLC_Device_取物門_Jog_P_OK.Bool = false;
                cnt_Program_取物門_Jog_P = 65535;
            }
        }
        void cnt_Program_取物門_Jog_P_檢查按下(ref int cnt)
        {
            if (PLC_Device_取物門_Jog_P.Bool) cnt++;
        }
        void cnt_Program_取物門_Jog_P_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取物門_Jog_P.Bool) cnt = 65500;
        }
        void cnt_Program_取物門_Jog_P_初始化(ref int cnt)
        {
            PLSV(enum_軸號.取物門, +PLC_Device_取物門_JOG速度.Value, PLC_Device_取物門_加減速度.Value);

        }












        #endregion
        #region PLC_取物門_復歸
        PLC_Device PLC_Device_取物門_復歸 = new PLC_Device("S7245");
        PLC_Device PLC_Device_取物門_復歸_OK = new PLC_Device("S7246");
        PLC_Device PLC_Device_取物門_復歸_高速速度 = new PLC_Device("K5000");
        PLC_Device PLC_Device_取物門_復歸_中速速度 = new PLC_Device("K1000");
        PLC_Device PLC_Device_取物門_復歸_低速速度 = new PLC_Device("K100");
        MyTimer MyTimer_取物門_復歸_歸零延遲 = new MyTimer();
        MyTimer MyTimer_取物門_復歸_轉向延遲 = new MyTimer();
        int cnt_Program_取物門_復歸 = 65534;
        void sub_Program_取物門_復歸()
        {
            if (cnt_Program_取物門_復歸 == 65534)
            {
                PLC_Device_取物門_復歸.SetComment("PLC_取物門_復歸");
                PLC_Device_取物門_復歸_OK.SetComment("PLC_取物門_復歸_OK");
                cnt_Program_取物門_復歸 = 65535;
            }
            if (cnt_Program_取物門_復歸 == 65535) cnt_Program_取物門_復歸 = 1;
            if (cnt_Program_取物門_復歸 == 1) cnt_Program_取物門_復歸_檢查按下(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 2) cnt_Program_取物門_復歸_初始化(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 3) cnt_Program_取物門_復歸_高速往負極限(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 4) cnt_Program_取物門_復歸_等待高速往負極限(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 5) cnt_Program_取物門_復歸_中速從負極限靠近原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 6) cnt_Program_取物門_復歸_等待中速靠近原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 7) cnt_Program_取物門_復歸_中速離開原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 8) cnt_Program_取物門_復歸_等待中速離開原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 9) cnt_Program_取物門_復歸_低速靠近原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 10) cnt_Program_取物門_復歸_等待低速靠近原點(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 11) cnt_Program_取物門_復歸_第一次Pos歸零(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 12) cnt_Program_取物門_復歸_開始偏移(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 13) cnt_Program_取物門_復歸_等待偏移完成(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 14) cnt_Program_取物門_復歸_第二次Pos歸零(ref cnt_Program_取物門_復歸);
            if (cnt_Program_取物門_復歸 == 15) cnt_Program_取物門_復歸 = 65500;
            if (cnt_Program_取物門_復歸 > 1) cnt_Program_取物門_復歸_檢查放開(ref cnt_Program_取物門_復歸);

            if (cnt_Program_取物門_復歸 == 65500)
            {
                this.AxisStop(enum_軸號.取物門);
                PLC_Device_取物門_復歸.Bool = false;
                cnt_Program_取物門_復歸 = 65535;
            }
        }
        void cnt_Program_取物門_復歸_檢查按下(ref int cnt)
        {
            if (PLC_Device_取物門_復歸.Bool) cnt++;
        }
        void cnt_Program_取物門_復歸_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取物門_復歸.Bool) cnt = 65500;
        }
        void cnt_Program_取物門_復歸_初始化(ref int cnt)
        {
            PLC_Device_取物門_復歸_OK.Bool = false;
            cnt++;
        }
        void cnt_Program_取物門_復歸_高速往負極限(ref int cnt)
        {
            this.PLSV(enum_軸號.取物門, -PLC_Device_取物門_復歸_高速速度.Value, PLC_Device_取物門_加減速度.Value);
            cnt++;
        }
        void cnt_Program_取物門_復歸_等待高速往負極限(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.取物門))
            {
                this.MyTimer_取物門_復歸_轉向延遲.TickStop();
                this.MyTimer_取物門_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
            if (this.Get_EL_N(enum_軸號.取物門))
            {
                this.MyTimer_取物門_復歸_轉向延遲.TickStop();
                this.MyTimer_取物門_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_中速從負極限靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.取物門, PLC_Device_取物門_復歸_中速速度.Value, PLC_Device_取物門_加減速度.Value);
            cnt++;
        }
        void cnt_Program_取物門_復歸_等待中速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.取物門))
            {
                this.MyTimer_取物門_復歸_轉向延遲.TickStop();
                this.MyTimer_取物門_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_中速離開原點(ref int cnt)
        {
            this.PLSV(enum_軸號.取物門, PLC_Device_取物門_復歸_中速速度.Value, PLC_Device_取物門_加減速度.Value);
            cnt++;
        }
        void cnt_Program_取物門_復歸_等待中速離開原點(ref int cnt)
        {
            if (!this.Get_ORG(enum_軸號.取物門))
            {
                this.MyTimer_取物門_復歸_轉向延遲.TickStop();
                this.MyTimer_取物門_復歸_轉向延遲.StartTickTime(100);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_低速靠近原點(ref int cnt)
        {
            this.PLSV(enum_軸號.取物門, -PLC_Device_取物門_復歸_低速速度.Value, PLC_Device_取物門_加減速度.Value);
            cnt++;
        }
        void cnt_Program_取物門_復歸_等待低速靠近原點(ref int cnt)
        {
            if (this.Get_ORG(enum_軸號.取物門))
            {
                this.MyTimer_取物門_復歸_歸零延遲.TickStop();
                this.MyTimer_取物門_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_第一次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_取物門_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_開始偏移(ref int cnt)
        {
            this.DRVA(enum_軸號.取物門, PLC_Device_取物門_偏移距離.Value, PLC_Device_取物門_運轉速度.Value, PLC_Device_取物門_加減速度.Value);
            cnt++;
        }
        void cnt_Program_取物門_復歸_等待偏移完成(ref int cnt)
        {
            if (this.Get_Axis_Ready(enum_軸號.取物門, PLC_Device_取物門_偏移距離.Value))
            {
                this.MyTimer_取物門_復歸_歸零延遲.TickStop();
                this.MyTimer_取物門_復歸_歸零延遲.StartTickTime(300);
                this.AxisStopEmg(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_復歸_第二次Pos歸零(ref int cnt)
        {
            if (this.MyTimer_取物門_復歸_歸零延遲.IsTimeOut())
            {
                this.Set_CommandPos(enum_軸號.取物門);
                cnt++;
                return;
            }
        }
        #endregion
        #region PLC_取物門_移動到開門位置
        PLC_Device PLC_Device_取物門_移動到開門位置 = new PLC_Device("S7265");
        PLC_Device PLC_Device_取物門_移動到開門位置_OK = new PLC_Device("S7266");
        int cnt_Program_取物門_移動到開門位置 = 65534;
        void sub_Program_取物門_移動到開門位置()
        {
            if (cnt_Program_取物門_移動到開門位置 == 65534)
            {
                PLC_Device_取物門_移動到開門位置.SetComment("PLC_取物門_移動到開門位置");
                PLC_Device_取物門_移動到開門位置_OK.SetComment("PLC_取物門_移動到開門位置_OK");
                PLC_Device_取物門_移動到開門位置.Bool = false;
                cnt_Program_取物門_移動到開門位置 = 65535;
            }
            if (cnt_Program_取物門_移動到開門位置 == 65535) cnt_Program_取物門_移動到開門位置 = 1;
            if (cnt_Program_取物門_移動到開門位置 == 1) cnt_Program_取物門_移動到開門位置_檢查按下(ref cnt_Program_取物門_移動到開門位置);
            if (cnt_Program_取物門_移動到開門位置 == 2) cnt_Program_取物門_移動到開門位置_初始化(ref cnt_Program_取物門_移動到開門位置);
            if (cnt_Program_取物門_移動到開門位置 == 3) cnt_Program_取物門_移動到開門位置_到開門位置(ref cnt_Program_取物門_移動到開門位置);
            if (cnt_Program_取物門_移動到開門位置 == 4) cnt_Program_取物門_移動到開門位置_等待到開門位置(ref cnt_Program_取物門_移動到開門位置);
            if (cnt_Program_取物門_移動到開門位置 == 5) cnt_Program_取物門_移動到開門位置 = 65500;
            if (cnt_Program_取物門_移動到開門位置 > 1) cnt_Program_取物門_移動到開門位置_檢查放開(ref cnt_Program_取物門_移動到開門位置);

            if (cnt_Program_取物門_移動到開門位置 == 65500)
            {
                this.AxisStop(enum_軸號.取物門);
                PLC_Device_取物門_移動到開門位置.Bool = false;
                PLC_Device_取物門_移動到開門位置_OK.Bool = false;
                cnt_Program_取物門_移動到開門位置 = 65535;
            }
        }
        void cnt_Program_取物門_移動到開門位置_檢查按下(ref int cnt)
        {
            if (PLC_Device_取物門_移動到開門位置.Bool) cnt++;
        }
        void cnt_Program_取物門_移動到開門位置_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool) cnt = 65500;
        }
        void cnt_Program_取物門_移動到開門位置_初始化(ref int cnt)
        {
            cnt++;
        }
        void cnt_Program_取物門_移動到開門位置_到開門位置(ref int cnt)
        {
            this.DRVA(enum_軸號.取物門, PLC_Device_取物門_開門位置.Value, PLC_Device_取物門_運轉速度.Value, PLC_Device_取物門_加減速度.Value);

            cnt++;
        }
        void cnt_Program_取物門_移動到開門位置_等待到開門位置(ref int cnt)
        {
            if (this.Get_Axis_Ready(enum_軸號.取物門, PLC_Device_取物門_開門位置.Value))
            {
                cnt++;
                return;
            }
        }










        #endregion
        #region PLC_取物門_移動到關門位置
        PLC_Device PLC_Device_取物門_移動到關門位置 = new PLC_Device("S7285");
        PLC_Device PLC_Device_取物門_移動到關門位置_OK = new PLC_Device("S7286");
        PLC_Device PLC_Device_取物門_移動到關門位置_防夾訊號 = new PLC_Device("X74");
        MyTimer MyTimere_取物門_移動到關門位置_防夾開門延遲 = new MyTimer();
        int cnt_Program_取物門_移動到關門位置 = 65534;
        void sub_Program_取物門_移動到關門位置()
        {
            if (cnt_Program_取物門_移動到關門位置 == 65534)
            {
                PLC_Device_取物門_移動到關門位置.SetComment("PLC_取物門_移動到關門位置");
                PLC_Device_取物門_移動到關門位置_OK.SetComment("PLC_取物門_移動到關門位置_OK");
                PLC_Device_取物門_移動到關門位置.Bool = false;
                cnt_Program_取物門_移動到關門位置 = 65535;
            }
            if (cnt_Program_取物門_移動到關門位置 == 65535) cnt_Program_取物門_移動到關門位置 = 1;
            if (cnt_Program_取物門_移動到關門位置 == 1) cnt_Program_取物門_移動到關門位置_檢查按下(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 2) cnt_Program_取物門_移動到關門位置_初始化(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 3) cnt_Program_取物門_移動到關門位置_到關門位置(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 4) cnt_Program_取物門_移動到關門位置_等待到關門位置(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 5) cnt_Program_取物門_移動到關門位置 = 65500;

            if (cnt_Program_取物門_移動到關門位置 == 200) cnt_Program_取物門_移動到關門位置_200_等待開始延遲到達(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 201) cnt_Program_取物門_移動到關門位置_200_回開門位置開始(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 202) cnt_Program_取物門_移動到關門位置_200_回開門位置結束(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 203) cnt_Program_取物門_移動到關門位置_200_等待延遲到達(ref cnt_Program_取物門_移動到關門位置);
            if (cnt_Program_取物門_移動到關門位置 == 204) cnt_Program_取物門_移動到關門位置 = 2;


            if (cnt_Program_取物門_移動到關門位置 > 1) cnt_Program_取物門_移動到關門位置_檢查放開(ref cnt_Program_取物門_移動到關門位置);



            if (cnt_Program_取物門_移動到關門位置 == 65500)
            {
                this.AxisStop(enum_軸號.取物門);
                PLC_Device_取物門_移動到關門位置.Bool = false;
                PLC_Device_取物門_移動到開門位置.Bool = false;
                PLC_Device_取物門_移動到關門位置_OK.Bool = false;
                cnt_Program_取物門_移動到關門位置 = 65535;
            }
        }
        void cnt_Program_取物門_移動到關門位置_檢查按下(ref int cnt)
        {
            if (PLC_Device_取物門_移動到關門位置.Bool) cnt++;
        }
        void cnt_Program_取物門_移動到關門位置_檢查放開(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到關門位置.Bool) cnt = 65500;
        }
        void cnt_Program_取物門_移動到關門位置_初始化(ref int cnt)
        {
            cnt++;
        }
        void cnt_Program_取物門_移動到關門位置_到關門位置(ref int cnt)
        {
            this.DRVA(enum_軸號.取物門, 0, PLC_Device_取物門_運轉速度.Value, PLC_Device_取物門_加減速度.Value);

            cnt++;
        }
        void cnt_Program_取物門_移動到關門位置_等待到關門位置(ref int cnt)
        {
            if(取得取藥防夾訊號())
            {
                this.AxisStopEmg(enum_軸號.取物門);
                MyTimere_取物門_移動到關門位置_防夾開門延遲.TickStop();
                MyTimere_取物門_移動到關門位置_防夾開門延遲.StartTickTime(100);
                cnt = 200;
                return;
            }
            if (this.Get_Axis_Ready(enum_軸號.取物門, 0))
            {
                cnt++;
                return;
            }
        }
        void cnt_Program_取物門_移動到關門位置_200_等待開始延遲到達(ref int cnt)
        {
            if (MyTimere_取物門_移動到關門位置_防夾開門延遲.IsTimeOut())
            {
                cnt++;
            }
        }
        void cnt_Program_取物門_移動到關門位置_200_回開門位置開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                PLC_Device_取物門_移動到開門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_取物門_移動到關門位置_200_回開門位置結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                MyTimere_取物門_移動到關門位置_防夾開門延遲.TickStop();
                MyTimere_取物門_移動到關門位置_防夾開門延遲.StartTickTime(2000);
                cnt++;
            }
        }
        void cnt_Program_取物門_移動到關門位置_200_等待延遲到達(ref int cnt)
        {
            if (MyTimere_取物門_移動到關門位置_防夾開門延遲.IsTimeOut())
            {
                cnt++;
            }
        }






        #endregion

        #region PLC_XY_Table_移動
        PLC_Device PLC_Device_XY_Table_移動 = new PLC_Device("S7305");
        PLC_Device PLC_Device_XY_Table_移動_層數 = new PLC_Device("D4050");
        PLC_Device PLC_Device_XY_Table_移動_格數 = new PLC_Device("D4051");
        PLC_Device PLC_Device_XY_Table_移動_層數_索引 = new PLC_Device("Z10");
        PLC_Device PLC_Device_XY_Table_移動_格數_索引 = new PLC_Device("Z11");

        PLC_Device PLC_Device_XY_Table_移動_Y軸位置 = new PLC_Device("D4060Z10");
        PLC_Device PLC_Device_XY_Table_移動_X軸位置 = new PLC_Device("D4070Z11");
      

        PLC_Device PLC_Device_XY_Table_移動_OK = new PLC_Device("S7306");

        int cnt_Program_XY_Table_移動 = 65534;
        void sub_Program_XY_Table_移動()
        {
            if (cnt_Program_XY_Table_移動 == 65534)
            {
                PLC_Device_XY_Table_移動.SetComment("PLC_XY_Table_移動");
                PLC_Device_XY_Table_移動_OK.SetComment("PLC_XY_Table_移動_OK");
                PLC_Device_XY_Table_移動.Bool = false;
                cnt_Program_XY_Table_移動 = 65535;
            }
            if (cnt_Program_XY_Table_移動 == 65535) cnt_Program_XY_Table_移動 = 1;
            if (cnt_Program_XY_Table_移動 == 1) cnt_Program_XY_Table_移動_檢查按下(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 2) cnt_Program_XY_Table_移動_初始化(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 3) cnt_Program_XY_Table_移動_開始X軸開始移動至安全位置(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 4) cnt_Program_XY_Table_移動_X軸開始移動至安全位置結束(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 5) cnt_Program_XY_Table_移動_開始移動(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 6) cnt_Program_XY_Table_移動_檢查移動完成(ref cnt_Program_XY_Table_移動);
            if (cnt_Program_XY_Table_移動 == 7) cnt_Program_XY_Table_移動 = 65500;
            if (cnt_Program_XY_Table_移動 > 1) cnt_Program_XY_Table_移動_檢查放開(ref cnt_Program_XY_Table_移動);

            if (cnt_Program_XY_Table_移動 == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                this.AxisStop(enum_軸號.Y軸);
                PLC_Device_X軸安全位置移動.Bool = false;
                PLC_Device_XY_Table_移動.Bool = false;
                PLC_Device_XY_Table_移動_OK.Bool = false;
                cnt_Program_XY_Table_移動 = 65535;
            }
        }
        void cnt_Program_XY_Table_移動_檢查按下(ref int cnt)
        {
            if (PLC_Device_XY_Table_移動.Bool) cnt++;
        }
        void cnt_Program_XY_Table_移動_檢查放開(ref int cnt)
        {
            if (!PLC_Device_XY_Table_移動.Bool) cnt = 65500;
        }
        void cnt_Program_XY_Table_移動_初始化(ref int cnt)
        {
            PLC_Device_XY_Table_移動_層數_索引.Value = PLC_Device_XY_Table_移動_層數.Value;
            PLC_Device_XY_Table_移動_格數_索引.Value = PLC_Device_XY_Table_移動_層數.Value * 10 + PLC_Device_XY_Table_移動_格數.Value;
            cnt++;
        }
        void cnt_Program_XY_Table_移動_開始X軸開始移動至安全位置(ref int cnt)
        {
            if(!PLC_Device_X軸安全位置移動.Bool)
            {
                PLC_Device_X軸安全位置移動.Bool = true;
                cnt++;
            }
        
        }
        void cnt_Program_XY_Table_移動_X軸開始移動至安全位置結束(ref int cnt)
        {
            if (!PLC_Device_X軸安全位置移動.Bool)
            {
                cnt++;
            }

        }
        void cnt_Program_XY_Table_移動_開始移動(ref int cnt)
        {
            this.DRVA(enum_軸號.X軸, PLC_Device_XY_Table_移動_X軸位置.Value, PLC_Device_X軸_運轉速度.Value, PLC_Device_X軸_加減速度.Value);
            this.DRVA(enum_軸號.Y軸, PLC_Device_XY_Table_移動_Y軸位置.Value, PLC_Device_Y軸_運轉速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_XY_Table_移動_檢查移動完成(ref int cnt)
        {
            int num = 0;
            if (this.Get_Axis_Ready(enum_軸號.X軸, PLC_Device_XY_Table_移動_X軸位置.Value))
            {
                num++;
            }
            if (this.Get_Axis_Ready(enum_軸號.Y軸, PLC_Device_XY_Table_移動_Y軸位置.Value))
            {
                num++;
            }
            if (num == 2)
            {
                cnt++;
            }
        }



        #endregion
        #region PLC_全軸復歸
        Dialog_系統復位中 dialog_系統復位中 = new Dialog_系統復位中();
        PLC_Device PLC_Device_全軸復歸 = new PLC_Device("S7325");
        PLC_Device PLC_Device_全軸復歸_OK = new PLC_Device("S7326");
        PLC_Device PLC_Device_全軸復歸_系統復位中FLOW = new PLC_Device("S800");
        int cnt_Program_全軸復歸 = 65534;
        void sub_Program_全軸復歸()
        {
            if (cnt_Program_全軸復歸 == 65534)
            {
                PLC_Device_全軸復歸_OK.Bool = false;
                PLC_Device_全軸復歸_系統復位中FLOW.Bool = false;
                PLC_Device_全軸復歸.SetComment("PLC_全軸復歸");
                PLC_Device_全軸復歸_OK.SetComment("PLC_全軸復歸_OK");
                cnt_Program_全軸復歸 = 65535;
            }
            if (cnt_Program_全軸復歸 == 65535) cnt_Program_全軸復歸 = 1;
            if (cnt_Program_全軸復歸 == 1) cnt_Program_全軸復歸_檢查按下(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 2) cnt_Program_全軸復歸_初始化(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 3) cnt_Program_全軸復歸_X軸及取物門復歸開始(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 4) cnt_Program_全軸復歸_X軸及取物門復歸結束(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 5) cnt_Program_全軸復歸_Y軸復歸開始(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 6) cnt_Program_全軸復歸_等待復歸結束(ref cnt_Program_全軸復歸);
            if (cnt_Program_全軸復歸 == 7) cnt_Program_全軸復歸 = 65500;
            if (cnt_Program_全軸復歸 > 1) cnt_Program_全軸復歸_檢查放開(ref cnt_Program_全軸復歸);

            if (cnt_Program_全軸復歸 == 65500)
            {
                //this.Invoke(new Action(delegate
                //{
                //    this.dialog_系統復位中.Close();
                //}));
                PLC_Device_X軸_復歸.Bool = false;
                PLC_Device_Y軸_復歸.Bool = false;
                PLC_Device_取物門_復歸.Bool = false;

                PLC_Device_全軸復歸.Bool = false;
                PLC_Device_全軸復歸_系統復位中FLOW.Bool = false;
                cnt_Program_全軸復歸 = 65535;
            }
        }
        void cnt_Program_全軸復歸_檢查按下(ref int cnt)
        {
            if (PLC_Device_全軸復歸.Bool) cnt++;
        }
        void cnt_Program_全軸復歸_檢查放開(ref int cnt)
        {
            if (!PLC_Device_全軸復歸.Bool) cnt = 65500;
        }
        void cnt_Program_全軸復歸_初始化(ref int cnt)
        {
            PLC_Device_全軸復歸_OK.Bool = false;
            PLC_Device_全軸復歸_系統復位中FLOW.Bool = true;
            //this.Invoke(new Action(delegate
            //{
            //    this.dialog_系統復位中.Show();
            //}));
            cnt++;
        }
        void cnt_Program_全軸復歸_X軸及取物門復歸開始(ref int cnt)
        {
            if(!PLC_Device_X軸_復歸.Bool && !PLC_Device_取物門_復歸.Bool)
            {
                PLC_Device_X軸_復歸.Bool = true;
                PLC_Device_取物門_復歸.Bool = true;
                cnt++;
            }           
        }
        void cnt_Program_全軸復歸_X軸及取物門復歸結束(ref int cnt)
        {
            if (!PLC_Device_X軸_復歸.Bool && !PLC_Device_取物門_復歸.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_全軸復歸_Y軸復歸開始(ref int cnt)
        {
            if (!PLC_Device_Y軸_復歸.Bool)
            {
                PLC_Device_Y軸_復歸.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_全軸復歸_等待復歸結束(ref int cnt)
        {
            if (!PLC_Device_X軸_復歸.Bool && !PLC_Device_Y軸_復歸.Bool && !PLC_Device_取物門_復歸.Bool)
            {
                PLC_Device_全軸復歸_OK.Bool = true;
                cnt++;
            }
        }










        #endregion
        #region PLC_送料馬達出料
        PLC_Device PLC_Device_送料馬達出料 = new PLC_Device("S7345");
        PLC_Device PLC_Device_送料馬達出料_OK = new PLC_Device("S7346");
        PLC_Device PLC_Device_送料馬達出料_持續作業 = new PLC_Device("S7347");
        PLC_Device PLC_Device_送料馬達出料_整層動作 = new PLC_Device("S7348");
        PLC_Device PLC_Device_送料馬達出料_層數 = new PLC_Device("D4050");
        PLC_Device PLC_Device_送料馬達出料_格數 = new PLC_Device("D4051");
        PLC_Device PLC_Device_送料馬達出料_連續次數現在值 = new PLC_Device("D4055");
        PLC_Device PLC_Device_送料馬達出料_連續次數設定值 = new PLC_Device("D4056");
        MyTimer MyTimer_送料馬達出料逾時時間 = new MyTimer();
        MyTimer MyTimer_送料馬達訊號檢查延遲 = new MyTimer();
        MyTimer MyTimer_送料馬達訊號持續作業延遲 = new MyTimer();
        MyTimer MyTimer_送料馬達結束延遲 = new MyTimer("D4052");


        List<PLC_Device> List_PLC_Device_送料馬達出料_原點 = new List<PLC_Device>();
        List<PLC_Device> List_PLC_Device_送料馬達出料_層數 = new List<PLC_Device>();
        List<PLC_Device> List_PLC_Device_送料馬達出料_格數 = new List<PLC_Device>();
        int cnt_Program_送料馬達出料 = 65534;
        void sub_Program_送料馬達出料()
        {
            if (cnt_Program_送料馬達出料 == 65534)
            {
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X40"));
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X41"));
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X42"));
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X43"));
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X44"));
                List_PLC_Device_送料馬達出料_原點.Add(new PLC_Device("X45"));

                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y50"));
                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y51"));
                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y52"));
                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y53"));
                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y54"));
                List_PLC_Device_送料馬達出料_層數.Add(new PLC_Device("Y55"));

                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y40"));
                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y41"));
                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y42"));
                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y43"));
                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y44"));
                List_PLC_Device_送料馬達出料_格數.Add(new PLC_Device("Y45"));


                PLC_Device_送料馬達出料.SetComment("PLC_送料馬達出料");
                PLC_Device_送料馬達出料_OK.SetComment("PLC_送料馬達出料_OK");
                PLC_Device_送料馬達出料.Bool = false;
                cnt_Program_送料馬達出料 = 65535;
            }
           
            if (cnt_Program_送料馬達出料 == 65535) cnt_Program_送料馬達出料 = 1;
            if (cnt_Program_送料馬達出料 == 1) cnt_Program_送料馬達出料_檢查按下(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 2) cnt_Program_送料馬達出料_初始化(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 3) cnt_Program_送料馬達出料_輸出(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 4) cnt_Program_送料馬達出料_等待訊號檢查延遲(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 5) cnt_Program_送料馬達出料_等待輸出訊號到達(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 6) cnt_Program_送料馬達出料_等待輸出訊號離開(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 7) cnt_Program_送料馬達出料_等待結束延遲(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 8) cnt_Program_送料馬達出料_持續作業延遲(ref cnt_Program_送料馬達出料);
            if (cnt_Program_送料馬達出料 == 9) cnt_Program_送料馬達出料 = 65500;
            if (cnt_Program_送料馬達出料 > 1) cnt_Program_送料馬達出料_檢查放開(ref cnt_Program_送料馬達出料);


            if (cnt_Program_送料馬達出料 == 65500)
            {
                for (int i = 0; i < List_PLC_Device_送料馬達出料_層數.Count; i++)
                {
                    List_PLC_Device_送料馬達出料_層數[i].Bool = false;
                }
                for (int i = 0; i < List_PLC_Device_送料馬達出料_格數.Count; i++)
                {
                    List_PLC_Device_送料馬達出料_格數[i].Bool = false;
                }
                this.送料馬達停止();
                PLC_Device_送料馬達出料.Bool = false;
                PLC_Device_送料馬達出料_OK.Bool = false;
                PLC_Device_輸送帶_輸出.Bool = false;
                cnt_Program_送料馬達出料 = 65535;
            }
        }
        void cnt_Program_送料馬達出料_檢查按下(ref int cnt)
        {
            if (PLC_Device_送料馬達出料.Bool) cnt++;
        }
        void cnt_Program_送料馬達出料_檢查放開(ref int cnt)
        {
            if (!PLC_Device_送料馬達出料.Bool) cnt = 65500;
        }
        void cnt_Program_送料馬達出料_初始化(ref int cnt)
        {
            for (int i = 0; i < List_PLC_Device_送料馬達出料_層數.Count; i++)
            {
                List_PLC_Device_送料馬達出料_層數[i].Bool = false;
            }
            for (int i = 0; i < List_PLC_Device_送料馬達出料_格數.Count; i++)
            {
                List_PLC_Device_送料馬達出料_格數[i].Bool = false;
            }
            MyTimer_送料馬達出料逾時時間.TickStop();
            MyTimer_送料馬達出料逾時時間.StartTickTime(2000);
            MyTimer_送料馬達訊號檢查延遲.TickStop();
            MyTimer_送料馬達訊號檢查延遲.StartTickTime(0);
            PLC_Device_送料馬達出料_連續次數現在值.Value = 0;
       
            cnt++;
        }
        void cnt_Program_送料馬達出料_輸出(ref int cnt)
        {
            int 層數 = PLC_Device_送料馬達出料_層數.Value;
            int 格數 = PLC_Device_送料馬達出料_格數.Value;
            if (PLC_Device_送料馬達出料_整層動作.Bool)
            {

            }
            this.送料馬達輸出(格數, 層數);
            //List_PLC_Device_送料馬達出料_層數[層數].Bool = true;
            //List_PLC_Device_送料馬達出料_格數[格數].Bool = true;

            cnt++;
        }
        void cnt_Program_送料馬達出料_等待訊號檢查延遲(ref int cnt)
        {
            int 層數 = PLC_Device_送料馬達出料_層數.Value;
            int 格數 = PLC_Device_送料馬達出料_格數.Value;
            this.送料馬達輸出(格數, 層數);
            if (MyTimer_送料馬達訊號檢查延遲.IsTimeOut())
            {
                cnt++;
                return;
            }
        }
        void cnt_Program_送料馬達出料_等待輸出訊號到達(ref int cnt)
        {
            int 層數 = PLC_Device_送料馬達出料_層數.Value;
            int 格數 = PLC_Device_送料馬達出料_格數.Value;
            this.送料馬達輸出(格數, 層數);
            if (MyTimer_送料馬達出料逾時時間.IsTimeOut())
            {
                cnt = 65535;
                return;
            }
            if (!取得送料馬達輸出(格數, 層數)) return;
            if (取得送料馬達出料原點(格數))
            {
  
                cnt++;
                return;
            }

        }
        void cnt_Program_送料馬達出料_等待輸出訊號離開(ref int cnt)
        {
            int 層數 = PLC_Device_送料馬達出料_層數.Value;
            int 格數 = PLC_Device_送料馬達出料_格數.Value;
            this.送料馬達輸出(格數, 層數);
            if(MyTimer_送料馬達出料逾時時間.IsTimeOut())
            {
                cnt = 65535;
                return;
            }
            if (!取得送料馬達輸出(格數, 層數)) return;
            if (!取得送料馬達出料原點(格數))
            {
                MyTimer_送料馬達結束延遲.TickStop();
                MyTimer_送料馬達結束延遲.StartTickTime();
                cnt++;
                return;
            }
       
        }
        void cnt_Program_送料馬達出料_等待結束延遲(ref int cnt)
        {
            if (MyTimer_送料馬達結束延遲.IsTimeOut())
            {
                this.送料馬達停止();
                MyTimer_送料馬達訊號持續作業延遲.TickStop();
                MyTimer_送料馬達訊號持續作業延遲.StartTickTime(500);

                cnt++;
                return;
            }

        }
        void cnt_Program_送料馬達出料_持續作業延遲(ref int cnt)
        {
            if(!PLC_Device_送料馬達出料_持續作業.Bool)
            {
                cnt++;
                return;
            }
            else
            {
                if (MyTimer_送料馬達訊號持續作業延遲.IsTimeOut())
                {

                    if (PLC_Device_送料馬達出料_連續次數設定值.Value != 0)
                    {
                        if (PLC_Device_送料馬達出料_連續次數現在值.Value >= PLC_Device_送料馬達出料_連續次數設定值.Value)
                        {
                            cnt++;
                            return;
                        }
                    }
                    MyTimer_送料馬達出料逾時時間.TickStop();
                    MyTimer_送料馬達出料逾時時間.StartTickTime(2000);
                    MyTimer_送料馬達訊號檢查延遲.TickStop();
                    MyTimer_送料馬達訊號檢查延遲.StartTickTime(0);
                    PLC_Device_送料馬達出料_連續次數現在值.Value++;
                    cnt = 3;
                    return;
                }
            }
           

        }








        #endregion
        #region PLC_出料一次
        PLC_Device PLC_Device_出料一次 = new PLC_Device("S7365");
        PLC_Device PLC_Device_出料一次_OK = new PLC_Device("S7366");
        int cnt_Program_出料一次 = 65534;
        void sub_Program_出料一次()
        {
            if (cnt_Program_出料一次 == 65534)
            {
                PLC_Device_出料一次.SetComment("PLC_出料一次");
                PLC_Device_出料一次_OK.SetComment("PLC_出料一次_OK");
                cnt_Program_出料一次 = 65535;
            }
            if (cnt_Program_出料一次 == 65535) cnt_Program_出料一次 = 1;
            if (cnt_Program_出料一次 == 1) cnt_Program_出料一次_檢查按下(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 2) cnt_Program_出料一次_初始化(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 3) cnt_Program_出料一次_XY_Table_移動開始(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 4) cnt_Program_出料一次_等待XY_Table_移動結束(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 5) cnt_Program_出料一次_送料馬達出料開始(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 6) cnt_Program_出料一次_等待送料馬達出料結束(ref cnt_Program_出料一次);
            if (cnt_Program_出料一次 == 7) cnt_Program_出料一次 = 65500;
            if (cnt_Program_出料一次 > 1) cnt_Program_出料一次_檢查放開(ref cnt_Program_出料一次);

            if (cnt_Program_出料一次 == 65500)
            {
                PLC_Device_XY_Table_移動.Bool = false;
                PLC_Device_送料馬達出料.Bool = false;

                PLC_Device_出料一次.Bool = false;
                PLC_Device_出料一次_OK.Bool = false;
                cnt_Program_出料一次 = 65535;
            }
        }
        void cnt_Program_出料一次_檢查按下(ref int cnt)
        {
            if (PLC_Device_出料一次.Bool) cnt++;
        }
        void cnt_Program_出料一次_檢查放開(ref int cnt)
        {
            if (!PLC_Device_出料一次.Bool) cnt = 65500;
        }
        void cnt_Program_出料一次_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_出料一次_XY_Table_移動開始(ref int cnt)
        {
            if (!PLC_Device_XY_Table_移動.Bool)
            {
                PLC_Device_XY_Table_移動.Bool = true;

                cnt++;
            }
        }
        void cnt_Program_出料一次_等待XY_Table_移動結束(ref int cnt)
        {
            if (!PLC_Device_XY_Table_移動.Bool)
            {

                cnt++;
            }
        }
        void cnt_Program_出料一次_送料馬達出料開始(ref int cnt)
        {
            if (!PLC_Device_送料馬達出料.Bool)
            {
                PLC_Device_送料馬達出料.Bool = true;

                cnt++;
            }
        }
        void cnt_Program_出料一次_等待送料馬達出料結束(ref int cnt)
        {
            if (!PLC_Device_送料馬達出料.Bool)
            {

                cnt++;
            }
        }









        #endregion
        #region PLC_輸送帶
        PLC_Device PLC_Device_輸送帶 = new PLC_Device("S7385");
        PLC_Device PLC_Device_輸送帶_輸出 = new PLC_Device("Y46");
        PLC_Device PLC_Device_輸送帶_OK = new PLC_Device("S7386");
        MyTimer MyTimer_輸送帶_輸出時間 = new MyTimer("D4200");
        int cnt_Program_輸送帶 = 65534;
        void sub_Program_輸送帶()
        {
            if (cnt_Program_輸送帶 == 65534)
            {
                PLC_Device_輸送帶.SetComment("PLC_輸送帶");
                PLC_Device_輸送帶_OK.SetComment("PLC_輸送帶_OK");
                cnt_Program_輸送帶 = 65535;
            }
            if (cnt_Program_輸送帶 == 65535) cnt_Program_輸送帶 = 1;
            if (cnt_Program_輸送帶 == 1) cnt_Program_輸送帶_檢查按下(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 2) cnt_Program_輸送帶_初始化(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 3) cnt_Program_輸送帶_輸出開始(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 4) cnt_Program_輸送帶_輸出時間到達結束(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 5) cnt_Program_輸送帶_X軸回零點(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 6) cnt_Program_輸送帶_X軸回零點完成(ref cnt_Program_輸送帶);
            if (cnt_Program_輸送帶 == 7) cnt_Program_輸送帶 = 65500;
            if (cnt_Program_輸送帶 > 1) cnt_Program_輸送帶_檢查放開(ref cnt_Program_輸送帶);

            if (cnt_Program_輸送帶 == 65500)
            {
                PLC_Device_輸送帶_輸出.Bool = false;

                PLC_Device_輸送帶.Bool = false;
                PLC_Device_輸送帶_OK.Bool = false;
                cnt_Program_輸送帶 = 65535;
            }
        }
        void cnt_Program_輸送帶_檢查按下(ref int cnt)
        {
            if (PLC_Device_輸送帶.Bool) cnt++;
        }
        void cnt_Program_輸送帶_檢查放開(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool) cnt = 65500;
        }
        void cnt_Program_輸送帶_初始化(ref int cnt)
        {
            PLC_Device_輸送帶_輸出.Bool = false;
            cnt++;
        }
        void cnt_Program_輸送帶_輸出開始(ref int cnt)
        {
            PLC_Device_輸送帶_輸出.Bool = true;
            MyTimer_輸送帶_輸出時間.TickStop();
            MyTimer_輸送帶_輸出時間.StartTickTime();
            cnt++;
        }
        void cnt_Program_輸送帶_輸出時間到達結束(ref int cnt)
        {
            if (MyTimer_輸送帶_輸出時間.IsTimeOut())
            {

                cnt++;
            }
        }
        void cnt_Program_輸送帶_X軸回零點(ref int cnt)
        {
            this.DRVA(enum_軸號.X軸, 0, PLC_Device_X軸_運轉速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_輸送帶_X軸回零點完成(ref int cnt)
        {
            int num = 0;
            if (this.Get_Axis_Ready(enum_軸號.X軸, 0))
            {
                num++;
            }

            if (num == 1)
            {
                cnt++;
            }
        }









        #endregion
        #region PLC_移動至出貨位置
        PLC_Device PLC_Device_移動至出貨位置 = new PLC_Device("S7405");
        PLC_Device PLC_Device_移動至出貨位置_OK = new PLC_Device("S7406");
        PLC_Device PLC_Device_移動至出貨位置_X位置 = new PLC_Device("D4210");
        PLC_Device PLC_Device_移動至出貨位置_Y位置 = new PLC_Device("D4211");
        int cnt_Program_移動至出貨位置 = 65534;
        void sub_Program_移動至出貨位置()
        {
            if (cnt_Program_移動至出貨位置 == 65534)
            {
                PLC_Device_移動至出貨位置.SetComment("PLC_移動至出貨位置");
                PLC_Device_移動至出貨位置_OK.SetComment("PLC_移動至出貨位置_OK");
                cnt_Program_移動至出貨位置 = 65535;
            }
            if (cnt_Program_移動至出貨位置 == 65535) cnt_Program_移動至出貨位置 = 1;
            if (cnt_Program_移動至出貨位置 == 1) cnt_Program_移動至出貨位置_檢查按下(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 2) cnt_Program_移動至出貨位置_初始化(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 3) cnt_Program_移動至出貨位置_開始X軸開始移動至安全位置(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 4) cnt_Program_移動至出貨位置_X軸開始移動至安全位置結束(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 5) cnt_Program_移動至出貨位置_Y軸開始移動至出貨位置(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 6) cnt_Program_移動至出貨位置_等待Y軸移動至出貨位置(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 7) cnt_Program_移動至出貨位置_X軸開始移動至出貨位置(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 8) cnt_Program_移動至出貨位置_等待X軸移動至出貨位置(ref cnt_Program_移動至出貨位置);
            if (cnt_Program_移動至出貨位置 == 9) cnt_Program_移動至出貨位置 = 65500;
            if (cnt_Program_移動至出貨位置 > 1) cnt_Program_移動至出貨位置_檢查放開(ref cnt_Program_移動至出貨位置);

            if (cnt_Program_移動至出貨位置 == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                this.AxisStop(enum_軸號.Y軸);
                PLC_Device_移動至出貨位置.Bool = false;
                PLC_Device_移動至出貨位置_OK.Bool = false;
                cnt_Program_移動至出貨位置 = 65535;
            }
        }
        void cnt_Program_移動至出貨位置_檢查按下(ref int cnt)
        {
            if (PLC_Device_移動至出貨位置.Bool) cnt++;
        }
        void cnt_Program_移動至出貨位置_檢查放開(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool) cnt = 65500;
        }
        void cnt_Program_移動至出貨位置_初始化(ref int cnt)
        {
            this.AxisStop(enum_軸號.X軸);
            this.AxisStop(enum_軸號.Y軸);
            cnt++;
        }
        void cnt_Program_移動至出貨位置_開始X軸開始移動至安全位置(ref int cnt)
        {
            if (!PLC_Device_X軸安全位置移動.Bool)
            {
                PLC_Device_X軸安全位置移動.Bool = true;
                cnt++;
            }

        }
        void cnt_Program_移動至出貨位置_X軸開始移動至安全位置結束(ref int cnt)
        {
            if (!PLC_Device_X軸安全位置移動.Bool)
            {
                cnt++;
            }

        }
        void cnt_Program_移動至出貨位置_Y軸開始移動至出貨位置(ref int cnt)
        {
            this.DRVA(enum_軸號.Y軸, PLC_Device_移動至出貨位置_Y位置.Value, PLC_Device_Y軸_運轉速度.Value, PLC_Device_Y軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_移動至出貨位置_等待Y軸移動至出貨位置(ref int cnt)
        {
            int num = 0;
            if (this.Get_Axis_Ready(enum_軸號.Y軸, PLC_Device_移動至出貨位置_Y位置.Value))
            {
                num++;
            }

            if(num == 1 )cnt++;
        }
        void cnt_Program_移動至出貨位置_X軸開始移動至出貨位置(ref int cnt)
        {
            this.DRVA(enum_軸號.X軸, PLC_Device_移動至出貨位置_X位置.Value, PLC_Device_X軸_運轉速度.Value, PLC_Device_X軸_加減速度.Value);

            cnt++;
        }
        void cnt_Program_移動至出貨位置_等待X軸移動至出貨位置(ref int cnt)
        {
            int num = 0;
            if (this.Get_Axis_Ready(enum_軸號.X軸, PLC_Device_移動至出貨位置_X位置.Value))
            {
                num++;
            }

            if (num == 1) cnt++;
        }









        #endregion
        #region PLC_退藥鎖
        PLC_Device PLC_Device_退藥鎖 = new PLC_Device("S7425");
        PLC_Device PLC_Device_退藥鎖_輸出 = new PLC_Device("Y77");
        PLC_Device PLC_Device_退藥鎖_輸入 = new PLC_Device("X77");
        PLC_Device PLC_Device_退藥鎖_OK = new PLC_Device("S7426");
        MyTimer MyTimer_退藥鎖_輸出時間 = new MyTimer("D4201");
        int cnt_Program_退藥鎖 = 65534;
        void sub_Program_退藥鎖()
        {
            if (cnt_Program_退藥鎖 == 65534)
            {
                PLC_Device_退藥鎖.SetComment("PLC_退藥鎖");
                PLC_Device_退藥鎖_OK.SetComment("PLC_退藥鎖_OK");
                cnt_Program_退藥鎖 = 65535;
            }
            if (cnt_Program_退藥鎖 == 65535) cnt_Program_退藥鎖 = 1;
            if (cnt_Program_退藥鎖 == 1) cnt_Program_退藥鎖_檢查按下(ref cnt_Program_退藥鎖);
            if (cnt_Program_退藥鎖 == 2) cnt_Program_退藥鎖_初始化(ref cnt_Program_退藥鎖);
            if (cnt_Program_退藥鎖 == 3) cnt_Program_退藥鎖_輸出開始(ref cnt_Program_退藥鎖);
            if (cnt_Program_退藥鎖 == 4) cnt_Program_退藥鎖_輸出時間到達結束(ref cnt_Program_退藥鎖);
            if (cnt_Program_退藥鎖 == 5) cnt_Program_退藥鎖 = 65500;
            if (cnt_Program_退藥鎖 > 1) cnt_Program_退藥鎖_檢查放開(ref cnt_Program_退藥鎖);

            if (cnt_Program_退藥鎖 == 65500)
            {
                PLC_Device_退藥鎖_輸出.Bool = false;

                PLC_Device_退藥鎖.Bool = false;
                cnt_Program_退藥鎖 = 65535;
            }
        }
        void cnt_Program_退藥鎖_檢查按下(ref int cnt)
        {
            if (PLC_Device_退藥鎖.Bool) cnt++;
        }
        void cnt_Program_退藥鎖_檢查放開(ref int cnt)
        {
            if (!PLC_Device_退藥鎖.Bool) cnt = 65500;
        }
        void cnt_Program_退藥鎖_初始化(ref int cnt)
        {
            PLC_Device_退藥鎖_輸出.Bool = false;
            cnt++;
        }
        void cnt_Program_退藥鎖_輸出開始(ref int cnt)
        {
            PLC_Device_退藥鎖_輸出.Bool = true;
            MyTimer_退藥鎖_輸出時間.TickStop();
            MyTimer_退藥鎖_輸出時間.StartTickTime();
            cnt++;
        }
        void cnt_Program_退藥鎖_輸出時間到達結束(ref int cnt)
        {
            if (!PLC_Device_退藥鎖_輸入.Bool)
            {
                PLC_Device_退藥鎖_OK.Bool = true;
                cnt++;
                return;
            }
            if (MyTimer_退藥鎖_輸出時間.IsTimeOut())
            {
                PLC_Device_退藥鎖_OK.Bool = false;
                cnt++;
                return;
            }
        }










        #endregion
        #region PLC_面板鎖
        PLC_Device PLC_Device_面板鎖 = new PLC_Device("S7445");
        PLC_Device PLC_Device_面板鎖_輸出 = new PLC_Device("Y76");
        PLC_Device PLC_Device_面板鎖_上輸入 = new PLC_Device("X75");
        PLC_Device PLC_Device_面板鎖_下輸入 = new PLC_Device("X76");
        PLC_Device PLC_Device_面板鎖_OK = new PLC_Device("S7446");
        MyTimer MyTimer_面板鎖_輸出時間 = new MyTimer("D4202");
        int cnt_Program_面板鎖 = 65534;
        void sub_Program_面板鎖()
        {
            if (cnt_Program_面板鎖 == 65534)
            {
                PLC_Device_面板鎖.SetComment("PLC_面板鎖");
                PLC_Device_面板鎖_OK.SetComment("PLC_面板鎖_OK");
                cnt_Program_面板鎖 = 65535;
            }
            if (cnt_Program_面板鎖 == 65535) cnt_Program_面板鎖 = 1;
            if (cnt_Program_面板鎖 == 1) cnt_Program_面板鎖_檢查按下(ref cnt_Program_面板鎖);
            if (cnt_Program_面板鎖 == 2) cnt_Program_面板鎖_初始化(ref cnt_Program_面板鎖);
            if (cnt_Program_面板鎖 == 3) cnt_Program_面板鎖_輸出開始(ref cnt_Program_面板鎖);
            if (cnt_Program_面板鎖 == 4) cnt_Program_面板鎖_輸出時間到達結束(ref cnt_Program_面板鎖);
            if (cnt_Program_面板鎖 == 5) cnt_Program_面板鎖 = 65500;
            if (cnt_Program_面板鎖 > 1) cnt_Program_面板鎖_檢查放開(ref cnt_Program_面板鎖);

            if (cnt_Program_面板鎖 == 65500)
            {
                PLC_Device_面板鎖_輸出.Bool = false;

                PLC_Device_面板鎖.Bool = false;
                PLC_Device_面板鎖_OK.Bool = false;
                cnt_Program_面板鎖 = 65535;
            }
        }
        void cnt_Program_面板鎖_檢查按下(ref int cnt)
        {
            if (PLC_Device_面板鎖.Bool) cnt++;
        }
        void cnt_Program_面板鎖_檢查放開(ref int cnt)
        {
            if (!PLC_Device_面板鎖.Bool) cnt = 65500;
        }
        void cnt_Program_面板鎖_初始化(ref int cnt)
        {
            PLC_Device_面板鎖_輸出.Bool = false;
            cnt++;
        }
        void cnt_Program_面板鎖_輸出開始(ref int cnt)
        {
            PLC_Device_面板鎖_輸出.Bool = true;
            MyTimer_面板鎖_輸出時間.TickStop();
            MyTimer_面板鎖_輸出時間.StartTickTime();
            cnt++;
        }
        void cnt_Program_面板鎖_輸出時間到達結束(ref int cnt)
        {
            if (MyTimer_面板鎖_輸出時間.IsTimeOut())
            {

                cnt++;
            }
        }










        #endregion
        #region PLC_X軸安全位置移動
        PLC_Device PLC_Device_X軸安全位置移動 = new PLC_Device("S7465");

        PLC_Device PLC_Device_X軸安全位置移動_X軸位置 = new PLC_Device("D4212");
        PLC_Device PLC_Device_X軸安全位置移動_OK = new PLC_Device("S7466");

        int cnt_Program_X軸安全位置移動 = 65534;
        void sub_Program_X軸安全位置移動()
        {
            if (cnt_Program_X軸安全位置移動 == 65534)
            {
                PLC_Device_X軸安全位置移動.SetComment("PLC_X軸安全位置移動");
                PLC_Device_X軸安全位置移動_OK.SetComment("PLC_X軸安全位置移動_OK");
                PLC_Device_X軸安全位置移動.Bool = false;
                cnt_Program_X軸安全位置移動 = 65535;
            }
            if (cnt_Program_X軸安全位置移動 == 65535) cnt_Program_X軸安全位置移動 = 1;
            if (cnt_Program_X軸安全位置移動 == 1) cnt_Program_X軸安全位置移動_檢查按下(ref cnt_Program_X軸安全位置移動);
            if (cnt_Program_X軸安全位置移動 == 2) cnt_Program_X軸安全位置移動_初始化(ref cnt_Program_X軸安全位置移動);
            if (cnt_Program_X軸安全位置移動 == 3) cnt_Program_X軸安全位置移動_開始移動(ref cnt_Program_X軸安全位置移動);
            if (cnt_Program_X軸安全位置移動 == 4) cnt_Program_X軸安全位置移動_檢查移動完成(ref cnt_Program_X軸安全位置移動);
            if (cnt_Program_X軸安全位置移動 == 5) cnt_Program_X軸安全位置移動 = 65500;
            if (cnt_Program_X軸安全位置移動 > 1) cnt_Program_X軸安全位置移動_檢查放開(ref cnt_Program_X軸安全位置移動);

            if (cnt_Program_X軸安全位置移動 == 65500)
            {
                this.AxisStop(enum_軸號.X軸);
                PLC_Device_X軸安全位置移動.Bool = false;
                PLC_Device_X軸安全位置移動_OK.Bool = false;
                cnt_Program_X軸安全位置移動 = 65535;
            }
        }
        void cnt_Program_X軸安全位置移動_檢查按下(ref int cnt)
        {
            if (PLC_Device_X軸安全位置移動.Bool) cnt++;
        }
        void cnt_Program_X軸安全位置移動_檢查放開(ref int cnt)
        {
            if (!PLC_Device_X軸安全位置移動.Bool) cnt = 65500;
        }
        void cnt_Program_X軸安全位置移動_初始化(ref int cnt)
        {

            cnt++;
        }
        void cnt_Program_X軸安全位置移動_開始移動(ref int cnt)
        {
            this.DRVA(enum_軸號.X軸, PLC_Device_X軸安全位置移動_X軸位置.Value, PLC_Device_X軸_運轉速度.Value, PLC_Device_X軸_加減速度.Value);
            cnt++;
        }
        void cnt_Program_X軸安全位置移動_檢查移動完成(ref int cnt)
        {
            int num = 0;
            if (this.Get_Axis_Ready(enum_軸號.X軸, PLC_Device_X軸安全位置移動_X軸位置.Value))
            {
                num++;
            }
  
            if (num == 1)
            {
                cnt++;
            }
        }



        #endregion
        #region PLC_清空出料盤
        PLC_Device PLC_Device_清空出料盤 = new PLC_Device("S6900");
        PLC_Device PLC_Device_清空出料盤_致能 = new PLC_Device("S6901");
        PLC_Device PLC_Device_清空出料盤_OK = new PLC_Device("");
        int cnt_Program_清空出料盤 = 65534;
        void sub_Program_清空出料盤()
        {
            if (cnt_Program_清空出料盤 == 65534)
            {
                PLC_Device_清空出料盤.SetComment("PLC_清空出料盤");
                PLC_Device_清空出料盤_OK.SetComment("PLC_清空出料盤_OK");
                PLC_Device_清空出料盤.Bool = false;
                cnt_Program_清空出料盤 = 65535;
            }
            if (cnt_Program_清空出料盤 == 65535) cnt_Program_清空出料盤 = 1;
            if (cnt_Program_清空出料盤 == 1) cnt_Program_清空出料盤_檢查按下(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 2) cnt_Program_清空出料盤_初始化(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 3) cnt_Program_清空出料盤_取物門_移動到關門位置開始(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 4) cnt_Program_清空出料盤_取物門_移動到關門位置結束(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 5) cnt_Program_清空出料盤_移動至出貨位置開始(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 6) cnt_Program_清空出料盤_移動至出貨位置結束(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 7) cnt_Program_清空出料盤_輸送帶開始(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 8) cnt_Program_清空出料盤_輸送帶結束(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 9) cnt_Program_清空出料盤_移動到開門位置開始(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 10) cnt_Program_清空出料盤_移動到開門位置結束(ref cnt_Program_清空出料盤);
            if (cnt_Program_清空出料盤 == 11) cnt_Program_清空出料盤 = 65500;
            if (cnt_Program_清空出料盤 > 1) cnt_Program_清空出料盤_檢查放開(ref cnt_Program_清空出料盤);

            if (cnt_Program_清空出料盤 == 65500)
            {
                PLC_Device_取物門_移動到開門位置.Bool = false;
                PLC_Device_取物門_移動到關門位置.Bool = false;
                PLC_Device_移動至出貨位置.Bool = false;
                PLC_Device_輸送帶.Bool = false;
                //PLC_Device_清空出料盤_致能.Bool = true;
                PLC_Device_清空出料盤.Bool = false;
                PLC_Device_清空出料盤_OK.Bool = false;
                cnt_Program_清空出料盤 = 65535;
            }
            if (PLC_Device_最高權限.Bool) PLC_Device_清空出料盤_致能.Bool = true;
        }
        void cnt_Program_清空出料盤_檢查按下(ref int cnt)
        {
            if (PLC_Device_清空出料盤.Bool) cnt++;
        }
        void cnt_Program_清空出料盤_檢查放開(ref int cnt)
        {
            if (!PLC_Device_清空出料盤.Bool) cnt = 65500;
        }
        void cnt_Program_清空出料盤_初始化(ref int cnt)
        {
            PLC_Device_取物門_移動到開門位置.Bool = false;
            PLC_Device_取物門_移動到關門位置.Bool = false;
            //PLC_Device_清空出料盤_致能.Bool = false;
            PLC_Device_移動至出貨位置.Bool = false;
            PLC_Device_輸送帶.Bool = false;
            cnt++;
        }
        void cnt_Program_清空出料盤_取物門_移動到關門位置開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到關門位置.Bool)
            {
                PLC_Device_取物門_移動到關門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_取物門_移動到關門位置結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到關門位置.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_移動至出貨位置開始(ref int cnt)
        {
            if(!PLC_Device_移動至出貨位置.Bool)
            {
                PLC_Device_移動至出貨位置.Bool = true;
                cnt++;
            }        
        }
        void cnt_Program_清空出料盤_移動至出貨位置結束(ref int cnt)
        {
            if (!PLC_Device_移動至出貨位置.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_輸送帶開始(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                PLC_Device_輸送帶.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_輸送帶結束(ref int cnt)
        {
            if (!PLC_Device_輸送帶.Bool)
            {
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_移動到開門位置開始(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                PLC_Device_取物門_移動到開門位置.Bool = true;
                cnt++;
            }
        }
        void cnt_Program_清空出料盤_移動到開門位置結束(ref int cnt)
        {
            if (!PLC_Device_取物門_移動到開門位置.Bool)
            {
                cnt++;
            }
        }










        #endregion

        #region Function
        List<int> Function_數組找目標數值加總組合(List<int> 單格所有庫存數量數組, int 目標數量)
        {
            List<int> list_result = new List<int>();
            List<int[]> List_Result單格所有庫存數量數組 = new List<int[]>();
            List<int> List_Current單格所有庫存數量數組 = new List<int>();
            List<int[]> List_sub_Current單格所有庫存數量數組 = new List<int[]>();
            int BasicValueIndex = 0;
            int sub_BasicValueIndex = 0;
            int tempValue累積數值 = 0;
            int tempValue累積數值_buf = 0;
            bool flag_HaveSameObj = false;
            int int_HaveSameObj = 0;
            int[] Current單格所有庫存數量數組 = List_Current單格所有庫存數量數組.ToArray();
            MyIntComparer mic = new MyIntComparer();
            單格所有庫存數量數組.Sort(mic);
            while (true)
            {
                List_Current單格所有庫存數量數組.Clear();
                tempValue累積數值 = 0;
                sub_BasicValueIndex = BasicValueIndex + 1;
                if (sub_BasicValueIndex >= 單格所有庫存數量數組.Count)
                {
                    tempValue累積數值 = 單格所有庫存數量數組[BasicValueIndex];
                    if (tempValue累積數值 == 目標數量)
                    {
                        List_Result單格所有庫存數量數組.Add(new int[] { 單格所有庫存數量數組[BasicValueIndex] });
                    }
                    break;
                }
                tempValue累積數值 += 單格所有庫存數量數組[BasicValueIndex];

                if (tempValue累積數值 >= 目標數量)
                {
                    if (tempValue累積數值 == 目標數量)
                    {
                        List_Result單格所有庫存數量數組.Add(new int[] { 單格所有庫存數量數組[BasicValueIndex] });
                    }
                    BasicValueIndex++;
                    continue;
                }
                else
                {
                    List_sub_Current單格所有庫存數量數組.Clear();
                    while (true)
                    {
                        if (sub_BasicValueIndex >= 單格所有庫存數量數組.Count)
                        {
                            BasicValueIndex++;
                            break;
                        }
                        List_Current單格所有庫存數量數組.Clear();
                        List_Current單格所有庫存數量數組.Add(單格所有庫存數量數組[BasicValueIndex]);
                        tempValue累積數值 = 單格所有庫存數量數組[BasicValueIndex];
                        for (int i = sub_BasicValueIndex; i < 單格所有庫存數量數組.Count; i++)
                        {
                            tempValue累積數值_buf = tempValue累積數值 + 單格所有庫存數量數組[i];
                            if (tempValue累積數值_buf > 目標數量)
                            {
                                continue;
                            }
                            else if (tempValue累積數值_buf == 目標數量)
                            {
                                List_Current單格所有庫存數量數組.Add(單格所有庫存數量數組[i]);
                                Current單格所有庫存數量數組 = List_Current單格所有庫存數量數組.ToArray();
                                flag_HaveSameObj = false;
                                foreach (int[] CheckArray in List_Result單格所有庫存數量數組)
                                {
                                    int_HaveSameObj = 0;
                                    if (CheckArray.Length != Current單格所有庫存數量數組.Length)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        for (int k = 0; k < CheckArray.Length; k++)
                                        {
                                            if (CheckArray[k] == Current單格所有庫存數量數組[k]) int_HaveSameObj++;
                                        }
                                        if (int_HaveSameObj == CheckArray.Length)
                                        {
                                            flag_HaveSameObj = true;
                                            break;
                                        }
                                    }
                                }
                                if (!flag_HaveSameObj) List_Result單格所有庫存數量數組.Add(List_Current單格所有庫存數量數組.ToArray());
                                break;
                            }
                            else if (tempValue累積數值_buf < 目標數量)
                            {
                                List_Current單格所有庫存數量數組.Add(單格所有庫存數量數組[i]);
                                tempValue累積數值 = tempValue累積數值_buf;
                            }
                        }
                        sub_BasicValueIndex++;
                    }
                }

            }
            if(List_Result單格所有庫存數量數組.Count > 0)
            {
                for (int i = 0; i < List_Result單格所有庫存數量數組[0].Length; i++)
                {
                    list_result.Add(List_Result單格所有庫存數量數組[0][i]);
                }
           
            }
            return list_result;
        }
        class MyIntComparer : IComparer<int>
        {
            //重寫int比較器，|x|>|y|返回正數，|x|=|y|返回0，|x|<|y|返回負數  
            public int Compare(int x, int y)
            {
                return -(x - y);
            }
        }
        #endregion
    }
}
