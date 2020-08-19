using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HowToLoadVppToPMObject
{
    public partial class Form1 : Form
    {
        CogPMAlignTool PMTool;

        public Form1()
        {
            InitializeComponent();
        }

        //툴 자체를 불러오는 방법
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "vpp|*.vpp" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PMTool = CogSerializer.LoadObjectFromFile(dialog.FileName) as CogPMAlignTool;
                cogPMAlignEdit.Subject = PMTool;
            }
        }

        //패턴만 불러오는 방법
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "vpp|*.vpp" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PMTool.Pattern = CogSerializer.LoadObjectFromFile(dialog.FileName) as CogPMAlignPattern;
            }
        }
    }
}
