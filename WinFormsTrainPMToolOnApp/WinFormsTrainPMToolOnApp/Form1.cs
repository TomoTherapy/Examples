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

namespace WinFormsTrainPMToolOnApp
{
    public partial class Form1 : Form
    {
        CogPMAlignTool PMTool;

        public Form1()
        {
            InitializeComponent();

            PMTool = new CogPMAlignTool();//툴생성
            cogPMAlignEditor.Subject = PMTool;//툴 확인용 에디터
        }

        //이미지 불러오기
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "bmp|*.bmp" };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CogImage8Grey cogImage = new CogImage8Grey(new Bitmap(dialog.FileName));

                MainDisplay.Image = cogImage;
                MainDisplay.Fit();
                PMTool.InputImage = cogImage;
            }
        }

        //영역표시툴 생성 후 코그 디스플레이에 표시
        private void button2_Click(object sender, EventArgs e)
        {
            if (PMTool.Pattern.Trained)
            {
                PMTool.Pattern.Untrain();
            }

            PMTool.Pattern.TrainImage = PMTool.InputImage;

            //CogRectangleAffine 생성
            CogRectangleAffine rec = PMTool.Pattern.TrainRegion as CogRectangleAffine;//CogPMAlignTool.Pattern.TrainRegion을 레퍼런스로
            rec.GraphicDOFEnable = CogRectangleAffineDOFConstants.All;
            rec.Interactive = true;
            rec.TipText = "학습 영역";
            rec.SetCenterLengthsRotationSkew(300, 300, 300, 300, 0, 0);

            //CogCoordinateAxes 생성 (오리진 좌표)
            CogCoordinateAxes axes = new CogCoordinateAxes();
            axes.Transform = PMTool.Pattern.Origin;//CogPMAlignTool.Pattern.Origin을 레퍼런스로
            axes.TipText = "중심좌표";
            axes.GraphicDOFEnable = CogCoordinateAxesDOFConstants.All;
            axes.Interactive = true;
            axes.MouseCursor = CogStandardCursorConstants.ManipulableGraphic;
            axes.OriginX = 300;
            axes.OriginY = 300;

            //코그디스플레이의 InteractiveGraphics 프라퍼티에 생성한 CogRectangleAffine, CogCoordinateAxes를 Add.
            MainDisplay.InteractiveGraphics.Add(rec, "RectangleAffine", false);
            MainDisplay.InteractiveGraphics.Add(axes, "Origin", false);
        }

        //Train
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                PMTool.Pattern.Train();//화면에서 지정한 영역이 이미 CogPMAlignTool 의 Pattern 프라퍼티 안에 있기 때문에 Train() 메서드를 실행하면 학습완료.

                MainDisplay.InteractiveGraphics.Clear();//필요없어진 InteractiveGraphics 제거

                PatternDisplay.Image = PMTool.Pattern.GetTrainedPatternImage();
                PatternDisplay.Fit();
            }
            catch (Exception ex)
            {
                if (PMTool.Pattern.Trained) PMTool.Pattern.Untrain();

                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}

