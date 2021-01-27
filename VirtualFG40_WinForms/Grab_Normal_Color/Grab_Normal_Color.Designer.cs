namespace Grab_Normal_Color
{
    partial class Grab_Normal_Color
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox_Display = new System.Windows.Forms.PictureBox();
            this.timer_Grab = new System.Windows.Forms.Timer(this.components);
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_Play = new System.Windows.Forms.Button();
            this.btn_Grab = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Open = new System.Windows.Forms.Button();
            this.btn_WBOnce = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Display
            // 
            this.pictureBox_Display.Location = new System.Drawing.Point(9, 8);
            this.pictureBox_Display.Name = "pictureBox_Display";
            this.pictureBox_Display.Size = new System.Drawing.Size(1024, 768);
            this.pictureBox_Display.TabIndex = 1;
            this.pictureBox_Display.TabStop = false;
            // 
            // timer_Grab
            // 
            this.timer_Grab.Interval = 30;
            this.timer_Grab.Tick += new System.EventHandler(this.timer_Grab_Tick);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point(1039, 124);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(102, 23);
            this.btn_Stop.TabIndex = 5;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Play
            // 
            this.btn_Play.Location = new System.Drawing.Point(1039, 95);
            this.btn_Play.Name = "btn_Play";
            this.btn_Play.Size = new System.Drawing.Size(102, 23);
            this.btn_Play.TabIndex = 6;
            this.btn_Play.Text = "Play";
            this.btn_Play.UseVisualStyleBackColor = true;
            this.btn_Play.Click += new System.EventHandler(this.btn_Play_Click);
            // 
            // btn_Grab
            // 
            this.btn_Grab.CausesValidation = false;
            this.btn_Grab.Location = new System.Drawing.Point(1040, 66);
            this.btn_Grab.Name = "btn_Grab";
            this.btn_Grab.Size = new System.Drawing.Size(102, 23);
            this.btn_Grab.TabIndex = 4;
            this.btn_Grab.Text = "Grab";
            this.btn_Grab.UseVisualStyleBackColor = true;
            this.btn_Grab.Click += new System.EventHandler(this.btn_Grab_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(1040, 37);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(102, 23);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Open
            // 
            this.btn_Open.Location = new System.Drawing.Point(1040, 8);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(102, 23);
            this.btn_Open.TabIndex = 3;
            this.btn_Open.Text = "Open";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // btn_WBOnce
            // 
            this.btn_WBOnce.Location = new System.Drawing.Point(1040, 154);
            this.btn_WBOnce.Name = "btn_WBOnce";
            this.btn_WBOnce.Size = new System.Drawing.Size(102, 23);
            this.btn_WBOnce.TabIndex = 7;
            this.btn_WBOnce.Text = "WB Once";
            this.btn_WBOnce.UseVisualStyleBackColor = true;
            this.btn_WBOnce.Click += new System.EventHandler(this.btn_WBOnce_Click);
            // 
            // Grab_Normal_Mono8
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 785);
            this.Controls.Add(this.btn_WBOnce);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Play);
            this.Controls.Add(this.btn_Grab);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.pictureBox_Display);
            this.Name = "Grab_Normal_Mono8";
            this.Text = "Grab_Normal_Color";
            this.Load += new System.EventHandler(this.Grab_Normal_Mono8_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Grab_Normal_Mono8_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.PictureBox pictureBox_Display;
        internal System.Windows.Forms.Timer timer_Grab;
        internal System.Windows.Forms.Button btn_Stop;
        internal System.Windows.Forms.Button btn_Play;
        internal System.Windows.Forms.Button btn_Grab;
        internal System.Windows.Forms.Button btn_Close;
        internal System.Windows.Forms.Button btn_Open;
        private System.Windows.Forms.Button btn_WBOnce;

    }
}

