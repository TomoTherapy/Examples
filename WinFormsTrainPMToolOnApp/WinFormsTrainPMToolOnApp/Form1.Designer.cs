namespace WinFormsTrainPMToolOnApp
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.MainDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.cogPMAlignEditor = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.PatternDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MainDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PatternDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // MainDisplay
            // 
            this.MainDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.MainDisplay.ColorMapLowerRoiLimit = 0D;
            this.MainDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.MainDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.MainDisplay.ColorMapUpperRoiLimit = 1D;
            this.MainDisplay.DoubleTapZoomCycleLength = 2;
            this.MainDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.MainDisplay.Location = new System.Drawing.Point(12, 35);
            this.MainDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.MainDisplay.MouseWheelSensitivity = 1D;
            this.MainDisplay.Name = "MainDisplay";
            this.MainDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("MainDisplay.OcxState")));
            this.MainDisplay.Size = new System.Drawing.Size(474, 390);
            this.MainDisplay.TabIndex = 0;
            // 
            // cogPMAlignEditor
            // 
            this.cogPMAlignEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cogPMAlignEditor.Location = new System.Drawing.Point(507, 35);
            this.cogPMAlignEditor.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogPMAlignEditor.Name = "cogPMAlignEditor";
            this.cogPMAlignEditor.Size = new System.Drawing.Size(674, 614);
            this.cogPMAlignEditor.SuspendElectricRuns = false;
            this.cogPMAlignEditor.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(45, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(146, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "이미지 불러오기";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(45, 473);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "트레인 영역생성";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(45, 502);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(146, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "트레인";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // PatternDisplay
            // 
            this.PatternDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.PatternDisplay.ColorMapLowerRoiLimit = 0D;
            this.PatternDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.PatternDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.PatternDisplay.ColorMapUpperRoiLimit = 1D;
            this.PatternDisplay.DoubleTapZoomCycleLength = 2;
            this.PatternDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.PatternDisplay.Location = new System.Drawing.Point(246, 431);
            this.PatternDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.PatternDisplay.MouseWheelSensitivity = 1D;
            this.PatternDisplay.Name = "PatternDisplay";
            this.PatternDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("PatternDisplay.OcxState")));
            this.PatternDisplay.Size = new System.Drawing.Size(240, 218);
            this.PatternDisplay.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(513, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "툴 잘 됐나 확인용 에디터";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "메인 코그디스플레이";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 609);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "패턴 디스플레이";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 449);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 478);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 507);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PatternDisplay);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cogPMAlignEditor);
            this.Controls.Add(this.MainDisplay);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.MainDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEditor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PatternDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Cognex.VisionPro.CogRecordDisplay MainDisplay;
        private Cognex.VisionPro.PMAlign.CogPMAlignEditV2 cogPMAlignEditor;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private Cognex.VisionPro.CogRecordDisplay PatternDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}

