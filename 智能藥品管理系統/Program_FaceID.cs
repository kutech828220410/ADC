using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using ArcSoftFace.SDKModels;
using ArcSoftFace.SDKUtil;
using ArcSoftFace.Utils;
using ArcSoftFace.Entity;
using MyFaceID;
using MyUI;
using Basic;
namespace 智能藥品管理系統
{
    public partial class Form1 : Form
    {
        private string FaceTest_Name = "";
        private int FaceTest_index = -10;
        private int retryToSucess = 0;
        private ASF_SingleFace Liveness_SingleFace_person_page;
        private ASF_SingleFace Liveness_SingleFace_main_page;
        private void Program_FaceID_Init()
        {
            this.myFaceIDUI_Main.Offline_dat = myConfigClass.Offline_dat;
            //this.myFaceIDUI_Main.AppID = myConfigClass.AppID;
            //this.myFaceIDUI_Main.ActiveKey = myConfigClass.ActiveKey;
            //this.myFaceIDUI_Main.SdkKey = myConfigClass.SdkKey;

            this.myFaceIDUI_person_page.Offline_dat = myConfigClass.Offline_dat;
            //this.myFaceIDUI_person_page.AppID = myConfigClass.AppID;
            //this.myFaceIDUI_person_page.ActiveKey = myConfigClass.ActiveKey;
            //this.myFaceIDUI_person_page.SdkKey = myConfigClass.SdkKey;

            this.myFaceIDUI_Login.Offline_dat = myConfigClass.Offline_dat;

            
            this.myFaceIDUI_Main.Init(0);
            this.myFaceIDUI_Login.Init(this.myFaceIDUI_Main.Camera, true);
            this.myFaceIDUI_person_page.Init(this.myFaceIDUI_Main.Camera, true);
            this.myFaceIDUI_person_page.livenessClass.FaceFeatureExtractEvent += LivenessClass_FaceFeatureExtractEvent;

        }
        bool flag_主畫面_領退藥_識別登 = false;
        private void Program_FaceID()
        {
            if (plC_ScreenPage_Main.PageText == "主畫面" && cnt_Program_主畫面_領退藥 == 100)
            {
                if(!PLC_Device_主畫面_領退藥_識別登入)
                {
                    if(flag_主畫面_領退藥_識別登)
                    {
                        myFaceIDUI_Main.ClearCanvas();

                        retryToSucess = 0;
                        FaceTest_index = -10;
                        flag_主畫面_領退藥_識別登 = false;
                
                    }
                    return;
                }
                else
                {
                    flag_主畫面_領退藥_識別登 = true;
                }
                using (Bitmap bitmap = myFaceIDUI_Main.GetBitmapFromCam())
                {

                    myFaceIDUI_Main.ShowRegisterROI = false;

                    List<ASF_SingleFace> list_ASF_SingleFace = new List<ASF_SingleFace>();
                    myFaceIDUI_Main.DrawBitmapToCanvas(bitmap);

                    if (myFaceIDUI_Main.livenessClass.IsDone)
                    {
                        Liveness_SingleFace_main_page = myFaceIDUI_Main.livenessClass.ASF_SingleFace_Result;

                        if (Liveness_SingleFace_main_page.CompareIndex != -1)
                        {
                            if (Liveness_SingleFace_main_page.CompareIndex < this.List_人員資料.Count)
                            {
                                FaceTest_Name = List_人員資料[Liveness_SingleFace_main_page.CompareIndex][(int)enum_人員資料.姓名].ObjectToString();
                            }
                        }

                        if (Liveness_SingleFace_main_page.CompareIndex == FaceTest_index && Liveness_SingleFace_main_page.CompareIndex != -1)
                        {
                            retryToSucess++;
                        }
                        else
                        {

                            retryToSucess = 0;
                        }

                        FaceTest_index = Liveness_SingleFace_main_page.CompareIndex;
                        if (Liveness_SingleFace_main_page.IsLive != 1)
                        {
                            FaceTest_index = -10;
                            retryToSucess = 0;
                        }


                        myFaceIDUI_Main.livenessClass.Trigger(bitmap);
                    }
                    if (Liveness_SingleFace_main_page != null)
                    {
                        if (Liveness_SingleFace_main_page.CompareIndex == -1) FaceTest_Name = "";
                        myFaceIDUI_Main.Draw_DetectFace(Liveness_SingleFace_main_page, FaceTest_Name, 2, new Font("標楷體", 16));
                    }

                    if(PLC_Device_主畫面_領退藥_識別登入)myFaceIDUI_Main.RrfreshCanvas();
                    else myFaceIDUI_Main.ClearCanvas();

                }
            }
            if (plC_ScreenPage_Main.PageText == "登入畫面")
            {
                using (Bitmap bitmap = myFaceIDUI_Login.GetBitmapFromCam())
                {

                    myFaceIDUI_Login.ShowRegisterROI = false;

                    List<ASF_SingleFace> list_ASF_SingleFace = new List<ASF_SingleFace>();
                    myFaceIDUI_Login.DrawBitmapToCanvas(bitmap);

                    if (myFaceIDUI_Login.livenessClass.IsDone)
                    {
                        Liveness_SingleFace_main_page = myFaceIDUI_Login.livenessClass.ASF_SingleFace_Result;

                        if (Liveness_SingleFace_main_page.CompareIndex != -1)
                        {
                            if (Liveness_SingleFace_main_page.CompareIndex < this.List_人員資料.Count)
                            {
                                FaceTest_Name = List_人員資料[Liveness_SingleFace_main_page.CompareIndex][(int)enum_人員資料.姓名].ObjectToString();
                            }
                        }

                        if (Liveness_SingleFace_main_page.CompareIndex == FaceTest_index && Liveness_SingleFace_main_page.CompareIndex != -1)
                        {


                            retryToSucess++;
                        }
                        else
                        {

                            retryToSucess = 0;
                        }

                        FaceTest_index = Liveness_SingleFace_main_page.CompareIndex;
                        if (Liveness_SingleFace_main_page.IsLive != 1)
                        {
                            FaceTest_index = -10;
                            retryToSucess = 0;
                        }


                        myFaceIDUI_Login.livenessClass.Trigger(bitmap);
                    }
                    if (Liveness_SingleFace_main_page != null)
                    {
                        if (Liveness_SingleFace_main_page.CompareIndex == -1) FaceTest_Name = "";
                        myFaceIDUI_Login.Draw_DetectFace(Liveness_SingleFace_main_page, FaceTest_Name, 2, new Font("標楷體", 16));
                    }

                    myFaceIDUI_Login.RrfreshCanvas();

                }
            }
            if (plC_ScreenPage_Main.PageText == "人員資料")
            {
                using (Bitmap bitmap = myFaceIDUI_person_page.GetBitmapFromCam())
                {

                    myFaceIDUI_person_page.ShowRegisterROI = false;

                    List<ASF_SingleFace> list_ASF_SingleFace = new List<ASF_SingleFace>();
                    myFaceIDUI_person_page.DrawBitmapToCanvas(bitmap);

                    if (myFaceIDUI_person_page.livenessClass.IsDone)
                    {
                        Liveness_SingleFace_person_page = myFaceIDUI_person_page.livenessClass.ASF_SingleFace_Result;
                        myFaceIDUI_person_page.livenessClass.Trigger(bitmap);
                    }
                    if (Liveness_SingleFace_person_page != null)
                    {

                        myFaceIDUI_person_page.Draw_DetectFace(Liveness_SingleFace_person_page, 2, new Font("微軟正黑體", 12));

                    }

                    myFaceIDUI_person_page.RrfreshCanvas();

                }
            }
         
        }
        private void LivenessClass_FaceFeatureExtractEvent(ArcSoftFace.SDKModels.FaceFeature faceFeature , ASF_SingleFace aSF_SingleFace)
        {
            if(aSF_SingleFace.IsLive == 1)
            {
                flag_人員資料_取得註冊圖案 = false;
            }
            
        }
        private FaceFeature GetFaceFeature()
        {
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(5000);
            FaceFeature faceFeature = new FaceFeature();
            while (true)
            {
                if (!flag_人員資料_取得註冊圖案)
                {
                    if (myFaceIDUI_person_page.livenessClass.FaceFeature.featureSize == 0)
                    {
                        flag_人員資料_取得註冊圖案 = true;
                    }
                    else
                    {
                        return myFaceIDUI_person_page.livenessClass.FaceFeature;
                    }

                }
                if (myTimer.IsTimeOut())
                {
                    return faceFeature;
                }
            }
        }
    }
}
