namespace 智能藥品管理系統
{
    partial class Dialog_退藥_藥品選擇
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.plC_RJ_Button_取消 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_確認 = new MyUI.PLC_RJ_Button();
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.sqL_DataGridView_儲位藥品 = new SQLUI.SQL_DataGridView();
            this.SuspendLayout();
            // 
            // plC_RJ_Button_取消
            // 
            this.plC_RJ_Button_取消.BackgroundColor = System.Drawing.Color.LightSlateGray;
            this.plC_RJ_Button_取消.Bool = false;
            this.plC_RJ_Button_取消.BorderColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_取消.BorderRadius = 5;
            this.plC_RJ_Button_取消.BorderSize = 0;
            this.plC_RJ_Button_取消.but_press = false;
            this.plC_RJ_Button_取消.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_取消.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_取消.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_取消.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_RJ_Button_取消.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_取消.Location = new System.Drawing.Point(604, 844);
            this.plC_RJ_Button_取消.Name = "plC_RJ_Button_取消";
            this.plC_RJ_Button_取消.OFF_文字內容 = "取消";
            this.plC_RJ_Button_取消.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_RJ_Button_取消.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_取消.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_取消.ON_文字內容 = "取消";
            this.plC_RJ_Button_取消.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 20.25F);
            this.plC_RJ_Button_取消.ON_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_取消.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_取消.Size = new System.Drawing.Size(185, 76);
            this.plC_RJ_Button_取消.State = false;
            this.plC_RJ_Button_取消.TabIndex = 38;
            this.plC_RJ_Button_取消.Text = "取消";
            this.plC_RJ_Button_取消.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_取消.Texts = "取消";
            this.plC_RJ_Button_取消.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_取消.字型鎖住 = false;
            this.plC_RJ_Button_取消.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_取消.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_取消.文字鎖住 = false;
            this.plC_RJ_Button_取消.讀取位元反向 = false;
            this.plC_RJ_Button_取消.讀寫鎖住 = false;
            this.plC_RJ_Button_取消.音效 = false;
            this.plC_RJ_Button_取消.顯示 = false;
            this.plC_RJ_Button_取消.顯示狀態 = false;
            // 
            // plC_RJ_Button_確認
            // 
            this.plC_RJ_Button_確認.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_確認.Bool = false;
            this.plC_RJ_Button_確認.BorderColor = System.Drawing.Color.RoyalBlue;
            this.plC_RJ_Button_確認.BorderRadius = 5;
            this.plC_RJ_Button_確認.BorderSize = 0;
            this.plC_RJ_Button_確認.but_press = false;
            this.plC_RJ_Button_確認.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_確認.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_確認.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_確認.Font = new System.Drawing.Font("微軟正黑體", 20.25F);
            this.plC_RJ_Button_確認.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_確認.Location = new System.Drawing.Point(795, 844);
            this.plC_RJ_Button_確認.Name = "plC_RJ_Button_確認";
            this.plC_RJ_Button_確認.OFF_文字內容 = "確認";
            this.plC_RJ_Button_確認.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 20.25F);
            this.plC_RJ_Button_確認.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_確認.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_確認.ON_文字內容 = "確認";
            this.plC_RJ_Button_確認.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 20.25F);
            this.plC_RJ_Button_確認.ON_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_確認.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_確認.Size = new System.Drawing.Size(185, 76);
            this.plC_RJ_Button_確認.State = false;
            this.plC_RJ_Button_確認.TabIndex = 37;
            this.plC_RJ_Button_確認.Text = "確認";
            this.plC_RJ_Button_確認.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_確認.Texts = "確認";
            this.plC_RJ_Button_確認.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_確認.字型鎖住 = false;
            this.plC_RJ_Button_確認.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_確認.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_確認.文字鎖住 = false;
            this.plC_RJ_Button_確認.讀取位元反向 = false;
            this.plC_RJ_Button_確認.讀寫鎖住 = false;
            this.plC_RJ_Button_確認.音效 = false;
            this.plC_RJ_Button_確認.顯示 = false;
            this.plC_RJ_Button_確認.顯示狀態 = false;
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.LightSkyBlue;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable1.BorderRadius = 12;
            this.rJ_Lable1.BorderSize = 0;
            this.rJ_Lable1.Dock = System.Windows.Forms.DockStyle.Top;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("微軟正黑體", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable1.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable1.Location = new System.Drawing.Point(5, 5);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.Size = new System.Drawing.Size(974, 102);
            this.rJ_Lable1.TabIndex = 39;
            this.rJ_Lable1.Text = "選擇退藥藥品";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.Black;
            // 
            // sqL_DataGridView_儲位藥品
            // 
            this.sqL_DataGridView_儲位藥品.AutoSelectToDeep = true;
            this.sqL_DataGridView_儲位藥品.backColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_儲位藥品.BorderColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_儲位藥品.BorderRadius = 0;
            this.sqL_DataGridView_儲位藥品.BorderSize = 2;
            this.sqL_DataGridView_儲位藥品.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_儲位藥品.cellStylBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_儲位藥品.cellStyleFont = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_儲位藥品.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_儲位藥品.columnHeaderBackColor = System.Drawing.Color.SkyBlue;
            this.sqL_DataGridView_儲位藥品.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_儲位藥品.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_儲位藥品.columnHeadersHeight = 18;
            this.sqL_DataGridView_儲位藥品.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqL_DataGridView_儲位藥品.DataBaseName = "adc_01";
            this.sqL_DataGridView_儲位藥品.Dock = System.Windows.Forms.DockStyle.Top;
            this.sqL_DataGridView_儲位藥品.Font = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_儲位藥品.ImageBox = false;
            this.sqL_DataGridView_儲位藥品.Location = new System.Drawing.Point(5, 107);
            this.sqL_DataGridView_儲位藥品.Name = "sqL_DataGridView_儲位藥品";
            this.sqL_DataGridView_儲位藥品.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_儲位藥品.Password = "user82822040";
            this.sqL_DataGridView_儲位藥品.Port = ((uint)(3306u));
            this.sqL_DataGridView_儲位藥品.rowHeaderBackColor = System.Drawing.Color.CornflowerBlue;
            this.sqL_DataGridView_儲位藥品.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_儲位藥品.RowsColor = System.Drawing.Color.White;
            this.sqL_DataGridView_儲位藥品.RowsHeight = 50;
            this.sqL_DataGridView_儲位藥品.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_儲位藥品.Server = "127.0.0.0";
            this.sqL_DataGridView_儲位藥品.Size = new System.Drawing.Size(974, 736);
            this.sqL_DataGridView_儲位藥品.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_儲位藥品.TabIndex = 40;
            this.sqL_DataGridView_儲位藥品.TableName = "medicine_page";
            this.sqL_DataGridView_儲位藥品.UserName = "root";
            this.sqL_DataGridView_儲位藥品.可拖曳欄位寬度 = false;
            this.sqL_DataGridView_儲位藥品.可選擇多列 = false;
            this.sqL_DataGridView_儲位藥品.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.sqL_DataGridView_儲位藥品.自動換行 = true;
            this.sqL_DataGridView_儲位藥品.表單字體 = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_儲位藥品.邊框樣式 = System.Windows.Forms.BorderStyle.None;
            this.sqL_DataGridView_儲位藥品.顯示首列 = false;
            this.sqL_DataGridView_儲位藥品.顯示首行 = true;
            this.sqL_DataGridView_儲位藥品.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.sqL_DataGridView_儲位藥品.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            // 
            // Dialog_退藥_藥品選擇
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(984, 932);
            this.ControlBox = false;
            this.Controls.Add(this.sqL_DataGridView_儲位藥品);
            this.Controls.Add(this.rJ_Lable1);
            this.Controls.Add(this.plC_RJ_Button_取消);
            this.Controls.Add(this.plC_RJ_Button_確認);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dialog_退藥_藥品選擇";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Dialog_退藥_藥品選擇_FormClosed);
            this.Load += new System.EventHandler(this.Dialog_退藥_藥品選擇_Load);
            this.Shown += new System.EventHandler(this.Dialog_退藥_藥品選擇_Shown);
            this.ResumeLayout(false);

        }

        #endregion
        private MyUI.PLC_RJ_Button plC_RJ_Button_取消;
        private MyUI.PLC_RJ_Button plC_RJ_Button_確認;
        private MyUI.RJ_Lable rJ_Lable1;
        private SQLUI.SQL_DataGridView sqL_DataGridView_儲位藥品;
    }
}