namespace Callback_Normal_Mono
{
    partial class Callback_Normal_Mono
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
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_Play = new System.Windows.Forms.Button();
            this.btn_Grab = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Open = new System.Windows.Forms.Button();
            this.pictureBox_Display = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point(1041, 124);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(102, 23);
            this.btn_Stop.TabIndex = 12;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Play
            // 
            this.btn_Play.Location = new System.Drawing.Point(1041, 95);
            this.btn_Play.Name = "btn_Play";
            this.btn_Play.Size = new System.Drawing.Size(102, 23);
            this.btn_Play.TabIndex = 13;
            this.btn_Play.Text = "Play";
            this.btn_Play.UseVisualStyleBackColor = true;
            this.btn_Play.Click += new System.EventHandler(this.btn_Play_Click);
            // 
            // btn_Grab
            // 
            this.btn_Grab.CausesValidation = false;
            this.btn_Grab.Location = new System.Drawing.Point(1042, 66);
            this.btn_Grab.Name = "btn_Grab";
            this.btn_Grab.Size = new System.Drawing.Size(102, 23);
            this.btn_Grab.TabIndex = 11;
            this.btn_Grab.Text = "Grab";
            this.btn_Grab.UseVisualStyleBackColor = true;
            this.btn_Grab.Click += new System.EventHandler(this.btn_Grab_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(1042, 37);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(102, 23);
            this.btn_Close.TabIndex = 9;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Open
            // 
            this.btn_Open.Location = new System.Drawing.Point(1042, 8);
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(102, 23);
            this.btn_Open.TabIndex = 10;
            this.btn_Open.Text = "Open";
            this.btn_Open.UseVisualStyleBackColor = true;
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // pictureBox_Display
            // 
            this.pictureBox_Display.Location = new System.Drawing.Point(11, 8);
            this.pictureBox_Display.Name = "pictureBox_Display";
            this.pictureBox_Display.Size = new System.Drawing.Size(1024, 768);
            this.pictureBox_Display.TabIndex = 8;
            this.pictureBox_Display.TabStop = false;
            // 
            // Callback_Normal_Mono
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 785);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Play);
            this.Controls.Add(this.btn_Grab);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Open);
            this.Controls.Add(this.pictureBox_Display);
            this.Name = "Callback_Normal_Mono";
            this.Text = "Callback_Normal_Mono";
            this.Load += new System.EventHandler(this.Callback_Normal_Mono_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Callback_Normal_Mono_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Display)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btn_Stop;
        internal System.Windows.Forms.Button btn_Play;
        internal System.Windows.Forms.Button btn_Grab;
        internal System.Windows.Forms.Button btn_Close;
        internal System.Windows.Forms.Button btn_Open;
        internal System.Windows.Forms.PictureBox pictureBox_Display;
    }
}

