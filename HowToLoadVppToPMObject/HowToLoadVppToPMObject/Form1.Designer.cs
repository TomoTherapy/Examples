namespace HowToLoadVppToPMObject
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
            this.cogPMAlignEdit = new Cognex.VisionPro.PMAlign.CogPMAlignEditV2();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // cogPMAlignEdit
            // 
            this.cogPMAlignEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cogPMAlignEdit.Location = new System.Drawing.Point(207, 12);
            this.cogPMAlignEdit.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogPMAlignEdit.Name = "cogPMAlignEdit";
            this.cogPMAlignEdit.Size = new System.Drawing.Size(771, 553);
            this.cogPMAlignEdit.SuspendElectricRuns = false;
            this.cogPMAlignEdit.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(41, 155);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "PMTool 불러오기";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(38, 218);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "PM 패턴 불러오기";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 577);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cogPMAlignEdit);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.cogPMAlignEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public Cognex.VisionPro.PMAlign.CogPMAlignEditV2 cogPMAlignEdit;
    }
}

