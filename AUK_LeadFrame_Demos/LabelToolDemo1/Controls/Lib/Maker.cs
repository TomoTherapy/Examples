using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Windows.Markup;
using DeepObjectDector.sub.lib.err;
using DeepObjectDector.sub.control.ImageListView;

namespace DeepObjectDector.sub.lib
{
    /*
     * *EventDeligate
     */
    public enum MARKERTYPE
    {
        Unknown = -1,
        RECTANGLE = 0,
        POLYLINE,
    }
    enum VALIDTYPE
    {
        UNKNOWN = -1,
        MOVING = 0,
        RESIZING,

    }

    public class Marker : FrameworkElement
    {
       /*
        * * delegate
        */
        
        //OutRect
        public MouseEventHandler outRectEnter;
        public MouseEventHandler outRectLeave;
        public MouseButtonEventHandler outRectUp;
        public MouseButtonEventHandler outRectDown;
        public MouseEventHandler outRectMove;

        //sizeCon
        public MouseEventHandler sizeConEnter;
        public MouseEventHandler sizeConLeave;
        public MouseEventHandler sizeConUp;
        public MouseEventHandler sizeConDown;
        public MouseEventHandler sizeConMove;
        
        /*
         * *memberVar
         */
        protected MARKERTYPE m_Type;
        protected String m_Tag;
        //protected Point m_Pos;
        protected Canvas m_cv; //Canvas의 좌표위치를 알기위한 용도로 Read만 사용할 것 
        protected Boolean m_alreadyDisposed = false;
        protected Point m_First_Click_Pos;
        protected Point m_movPos;

        /*
         * * constructor
         */
        public Marker()
        { }
        public Marker(Canvas cv=null, MARKERTYPE type = MARKERTYPE.Unknown)
        {
            m_cv = cv;
            m_Type = type;
        }

        /*
         * *property
         */
        public Canvas p_cv
        {
            get { return m_cv; }
            set { m_cv = (Canvas)value; }
        }
        public Point p_First_Click_Pos
        {
            get { return m_First_Click_Pos; }
            set { m_First_Click_Pos = (Point) value; }
        }
        public MARKERTYPE p_Type
        {
            get { return m_Type; }
        }
        public String p_Tag
        {
            get { return m_Tag; }
            set { m_Tag = (String) value; }
        }

        /*
         * *method
         */
        static public void MK_GeneratorKeyValue(ref String key)
        {
            //UUID 생성
            Guid keyVal = Guid.NewGuid();
            String[] temp = keyVal.ToString().Split('-');

            StringBuilder strFactory = new StringBuilder();
            foreach (String str in temp)
                strFactory.Append(str);

            key = strFactory.ToString();
            return;
        }
    }

    class RectMarker : Marker, IDisposable, IXmlSerializable
    {
        /*
         * memberVar
         */
        private const int DRAG_HANDLE_SIZE = 9;
        private StackPanel m_PicBox;
        private Rectangle m_Rect;
        private TextBlock m_Label;
        private Dictionary<Direction, SizeConRectangleItem> m_SizeConrects;
        
        /*
         * * constructor
         */
        public RectMarker()
        {
            base.m_cv = null;
            base.m_Type = MARKERTYPE.RECTANGLE;
            m_SizeConrects = new Dictionary<Direction, SizeConRectangleItem>();
        }

        public RectMarker(Canvas cv, double height=50.0f, double width=50.0f, String label="label") : base(cv, MARKERTYPE.RECTANGLE)
        {
            inner_AllocMarker(Brushes.Red, height, width, label);
            m_SizeConrects = new Dictionary<Direction, SizeConRectangleItem>();
        }

        /*
         * * property
         */
        public StackPanel p_PicBox
        {
            get { return m_PicBox; }
            set { m_PicBox = (StackPanel) value;}
        }
        public Rectangle p_Rect
        {
            get { return m_Rect; }
            set { m_Rect = (Rectangle)value; }
        }
        public TextBlock p_Label
        {
            get { return m_Label; }
            set { m_Label = (TextBlock) value; }
        }
        public Dictionary<Direction, SizeConRectangleItem> p_SizeConrects
        {
            get { return m_SizeConrects; }
        }
        public Brush p_Stroke
        {
            get { return (m_Rect.Stroke); }
            set { m_Rect.Stroke = (Brush)value; }
        }
        public double p_StrokeThickness
        {
            get { return m_Rect.StrokeThickness; }
            set { m_Rect.StrokeThickness = (double)value; }
        }
        /*
         * * method
         */
        public void Dispose()
        {
            Dispose(true);
            // Finalization이 수행되지 않도록 한다
            GC.SuppressFinalize(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            // 여러 번 dipose를 수행하지 않도록 한다.
            if (base.m_alreadyDisposed)
                return;

            if (isDisposing)
            {
                // 해야할 일: managed(GC가관리되는) 리소스를 해제한다.
                base.m_cv = null;
            }

            // 해야할일: unmanaged(GC가 관리되지않는) 리소스를 해제한다.
            // disposed 플래그를 설정한다.
            m_alreadyDisposed = false;
        }
        public XmlSchema GetSchema() { return null; }
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Marker_ref")
            {
                base.m_Tag = reader["Tag"];
                base.m_cv = (Canvas)XamlReader.Parse(reader["cv"]);
                base.m_Type = (MARKERTYPE)XamlReader.Parse(reader["type"]);
                m_PicBox = (StackPanel)XamlReader.Parse(reader["PicBox"]);
                m_Rect = (Rectangle)XamlReader.Parse(reader["Rect"]);
                m_Label = (TextBlock)XamlReader.Parse(reader["Label"]);
                
                if (reader.ReadToDescendant("SizeConMarkers"))
                {
                    //m_Tools
                    m_SizeConrects.Clear();
                    Direction key = Direction.None;
                    String strKey = null;
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SizeConMarkers")
                    {
                        strKey = reader.GetAttribute("Key");
                        key = MK_GetDirectionStr2Enum(strKey);
                        SizeConRectangleItem evt = new SizeConRectangleItem();
                        evt.ReadXml(reader);
                        m_SizeConrects.Add(key, evt);//<- 여기서 터짐
                    }
                }
                reader.Read();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Tag", base.m_Tag);
            writer.WriteAttributeString("cv", XamlWriter.Save(base.m_cv));
            writer.WriteAttributeString("type", XamlWriter.Save(base.m_Type));
            writer.WriteAttributeString("PicBox", XamlWriter.Save(m_PicBox));
            writer.WriteAttributeString("Rect", XamlWriter.Save(m_Rect));
            writer.WriteAttributeString("Label", XamlWriter.Save(m_Label));

            foreach (var key in m_SizeConrects.Keys)
            {
                writer.WriteStartElement("SizeConMarkers");
                writer.WriteAttributeString("Key", key.ToString());
                SizeConRectangleItem evt = m_SizeConrects[key];
                evt.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        static public Direction MK_GetDirectionStr2Enum(String str)
        {
            Direction dir = Direction.None;
            switch (str)
            {
                case "NW":
                    dir = Direction.NW;
                    break;

                case "N":
                    dir = Direction.N;
                    break;

                case "NE":
                    dir = Direction.NE;
                    break;

                case "W":
                    dir = Direction.W;
                    break;

                case "E":
                    dir = Direction.E;
                    break;

                case "SW":
                    dir = Direction.SW;
                    break;

                case "S":
                    dir = Direction.S;
                    break;

                case "SE":
                    dir = Direction.SE;
                    break;

                default:
                    break;
            }
            return dir;
        }

        //MK_OUtNemoChgLabel
        public ERR_RESULT MK_OutNemoChgLabel(String label)
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                if (label == null || label == "")
                    return result;

                m_Label.Text = label;
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
        }

        //MK_OutNemoMove
        public ERR_RESULT MK_OutNemoMove(float x_pos, float y_pos)
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                Canvas.SetLeft(m_PicBox, x_pos);
                Canvas.SetTop(m_PicBox, y_pos);

                //LDH8282 SizeCon 조정
                
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
        }
        //MK_OutNemoResize 
        public ERR_RESULT MK_OutNemoResize(float height = 50.0f, float width = 50.0f)
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                m_PicBox.Height = height;
                m_PicBox.Width = width;

                //LDH8282 SizeCon 조정
                
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }

        }
        //LDH8282 MK_SizeConAlloc
        public ERR_RESULT MK_SizeConAlloc()
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
                //around the control, so when the drag handles are drawn they will be seem
                //connected in the middle.
                double x = Canvas.GetLeft(m_PicBox);
                double y = Canvas.GetTop(m_PicBox);

                y = y + m_Label.Height;

                //define the 8 drag handles, that has the size o;f DRAG_HANDLE_SIZE
                //NW
                Rectangle NW = new Rectangle();
                NW.Name = "RECT_NW";

                SizeConRectangleItem NW_Item = new SizeConRectangleItem();
                NW_Item.Name = NW.Name;
                NW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
                NW_Item.Top = y - DRAG_HANDLE_SIZE / 2;
                NW_Item.sizeConRect_ref = NW;
                Canvas.SetLeft(NW, NW_Item.Left);
                Canvas.SetTop(NW, NW_Item.Top);

                //event set
                NW.MouseEnter += OnSizeConRect_Enter;
                NW.MouseLeave += OnSizeConRect_Leave;
                NW.MouseUp += OnSizeConRect_Up;
                NW.MouseDown += OnSizeConRect_Down;
                NW.MouseMove += OnSizeConRect_Move;

                //append
                inner_initSizeRectagle(NW);
                base.m_cv.Children.Add(NW);
                m_SizeConrects[Direction.NW] = NW_Item;

                //N
                Rectangle N = new Rectangle();
                N.Name = "RECT_N";

                SizeConRectangleItem N_Item = new SizeConRectangleItem();
                N_Item.Name = N.Name;
                N_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
                N_Item.Top = y - DRAG_HANDLE_SIZE / 2;
                N_Item.sizeConRect_ref = N;
                Canvas.SetLeft(N, N_Item.Left);
                Canvas.SetTop(N, N_Item.Top);

                //event set
                N.MouseEnter += OnSizeConRect_Enter;
                N.MouseLeave += OnSizeConRect_Leave;
                N.MouseUp += OnSizeConRect_Up;
                N.MouseDown += OnSizeConRect_Down;
                N.MouseMove += OnSizeConRect_Move;
                //append
                inner_initSizeRectagle(N);
                m_cv.Children.Add(N);
                m_SizeConrects[Direction.N] = N_Item;

                //NE
                Rectangle NE = new Rectangle();
                NE.Name = "RECT_NE";

                SizeConRectangleItem NE_Item = new SizeConRectangleItem();
                NE_Item.Name = NE.Name;
                NE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
                NE_Item.Top = y - DRAG_HANDLE_SIZE / 2;
                NE_Item.sizeConRect_ref = NE;
                Canvas.SetLeft(NE, NE_Item.Left);
                Canvas.SetTop(NE, NE_Item.Top);

                //event set
                NE.MouseEnter += OnSizeConRect_Enter;
                NE.MouseLeave += OnSizeConRect_Leave;
                NE.MouseUp += OnSizeConRect_Up;
                NE.MouseDown += OnSizeConRect_Down;
                NE.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(NE);
                m_cv.Children.Add(NE);
                m_SizeConrects[Direction.NE] = NE_Item;

                //W
                Rectangle W = new Rectangle();
                W.Name = "RECT_W";

                SizeConRectangleItem W_Item = new SizeConRectangleItem();
                W_Item.Name = W.Name;
                W_Item.Left = x - DRAG_HANDLE_SIZE / 2;
                W_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
                W_Item.sizeConRect_ref = W;
                Canvas.SetLeft(W, W_Item.Left);
                Canvas.SetTop(W, W_Item.Top);

                //event set
                W.MouseEnter += OnSizeConRect_Enter;
                W.MouseLeave += OnSizeConRect_Leave;
                W.MouseUp += OnSizeConRect_Up;
                W.MouseDown += OnSizeConRect_Down;
                W.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(W);
                m_cv.Children.Add(W);
                m_SizeConrects[Direction.W] = W_Item;

                //E
                Rectangle E = new Rectangle();
                E.Name = "RECT_E";

                SizeConRectangleItem E_Item = new SizeConRectangleItem();
                E_Item.Name = E.Name;
                E_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
                E_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
                E_Item.sizeConRect_ref = E;
                Canvas.SetLeft(E, E_Item.Left);
                Canvas.SetTop(E, E_Item.Top);

                //event set
                E.MouseEnter += OnSizeConRect_Enter;
                E.MouseLeave += OnSizeConRect_Leave;
                E.MouseUp += OnSizeConRect_Up;
                E.MouseDown += OnSizeConRect_Down;
                E.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(E);
                m_cv.Children.Add(E);
                m_SizeConrects[Direction.E] = E_Item;

                //SW
                Rectangle SW = new Rectangle();
                SW.Name = "RECT_SW";

                SizeConRectangleItem SW_Item = new SizeConRectangleItem();
                SW_Item.Name = SW.Name;
                SW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
                SW_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
                SW_Item.sizeConRect_ref = SW;
                Canvas.SetLeft(SW, SW_Item.Left);
                Canvas.SetTop(SW, SW_Item.Top);

                //event set
                SW.MouseEnter += OnSizeConRect_Enter;
                SW.MouseLeave += OnSizeConRect_Leave;
                SW.MouseUp += OnSizeConRect_Up;
                SW.MouseDown += OnSizeConRect_Down;
                SW.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(SW);
                m_cv.Children.Add(SW);
                m_SizeConrects[Direction.SW] = SW_Item;

                //S
                Rectangle S = new Rectangle();
                S.Name = "RECT_S";

                SizeConRectangleItem S_Item = new SizeConRectangleItem();
                S_Item.Name = S.Name;
                S_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
                S_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
                S_Item.sizeConRect_ref = S;
                Canvas.SetLeft(S, S_Item.Left);
                Canvas.SetTop(S, S_Item.Top);

                //event set
                S.MouseEnter += OnSizeConRect_Enter;
                S.MouseLeave += OnSizeConRect_Leave;
                S.MouseUp += OnSizeConRect_Up;
                S.MouseDown += OnSizeConRect_Down;
                S.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(S);
                m_cv.Children.Add(S);
                m_SizeConrects[Direction.S] = S_Item;

                //SE
                Rectangle SE = new Rectangle();
                SE.Name = "RECT_SE";

                SizeConRectangleItem SE_Item = new SizeConRectangleItem();
                SE_Item.Name = SE.Name;
                SE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
                SE_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
                SE_Item.sizeConRect_ref = SE;
                Canvas.SetLeft(SE, SE_Item.Left);
                Canvas.SetTop(SE, SE_Item.Top);

                //event set
                SE.MouseEnter += OnSizeConRect_Enter;
                SE.MouseLeave += OnSizeConRect_Leave;
                SE.MouseUp += OnSizeConRect_Up;
                SE.MouseDown += OnSizeConRect_Down;
                SE.MouseMove += OnSizeConRect_Move;
                
                //append
                inner_initSizeRectagle(SE);
                m_cv.Children.Add(SE);
                m_SizeConrects[Direction.SE] = SE_Item;

                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            finally
            { }
        }
        public ERR_RESULT MK_RemoveRectMark(Canvas cv)
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                base.m_cv = cv;
                //캔버스에서 아웃라인네모 제거               
                m_cv.Children.Remove(m_PicBox);

                //캔버스에서 사이즈네모 제거
                m_cv.Children.Remove(m_SizeConrects[Direction.NW].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.N].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.NE].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.W].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.E].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.SW].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.S].sizeConRect_ref as Rectangle);
                m_cv.Children.Remove(m_SizeConrects[Direction.SE].sizeConRect_ref as Rectangle);

                m_SizeConrects.Clear();
                m_SizeConrects = null;
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            finally
            { }
        }
        //MK_SizeConVisible
        public ERR_RESULT MK_SizeConVisible(Visibility enable)
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                Dictionary<Direction, SizeConRectangleItem> sizeConrects = m_SizeConrects;
                if (sizeConrects == null)
                    throw new _MainException(-1); ;

                Rectangle sizeTempRect = null;
                sizeTempRect = sizeConrects[Direction.NW].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.N].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.NE].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.E].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.W].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.SW].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.S].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                sizeTempRect = sizeConrects[Direction.SE].sizeConRect_ref as Rectangle;
                sizeTempRect.Visibility = enable;
                sizeTempRect = null;

                return result;
            }

            catch (_MarkerException err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessOrigin.SetErrResult(err);
                return result;
            }
            finally
            { }
        }

        public Boolean MK_ValidCanvasOutLine(Point curr_Pos, VALIDTYPE type)
        {
            //네모 이동 제한
            Boolean isIn = true;

            Point movPos = new Point();
            Point afterPos = new Point();

            movPos.X = curr_Pos.X - m_First_Click_Pos.X;
            movPos.Y = curr_Pos.Y - m_First_Click_Pos.Y;

            afterPos.X = Canvas.GetLeft(m_PicBox) + movPos.X;
            afterPos.Y = Canvas.GetTop(m_PicBox) + movPos.Y;

            double left_space = 5; //(zoomAndPanControl.ActualWidth) / 2;
            double right_space = m_cv.Width - 5; //(zoomAndPanControl.ActualWidth) / 2;
            double top_space = 5; //(zoomAndPanControl.ActualHeight) / 2;
            double bottom_space = m_cv.Height - 5; //(zoomAndPanControl.ActualHeight) / 2;

            double left_curr = afterPos.X;
            double right_curr = afterPos.X;
            double top_curr = afterPos.Y;
            double bottom_curr = afterPos.Y;

            if (type == VALIDTYPE.MOVING)
            {
                right_curr += m_Rect.Width;
                bottom_curr += m_Label.Height + m_Rect.Height;//Label + Rect
            }

            //when moving outlineNemo
            if (left_curr < left_space + 2 || right_curr > right_space - 2)//좌측 우측 경계면 검증
                isIn = false;
            else if (top_curr < top_space + 2 || bottom_curr > bottom_space - 2)// 위 아래 경계면 검증
                isIn = false;
            else
                isIn = true;

            return isIn;
        }
        public void MK_RectMoving(RectMarker rectMarker, Point curr_Pos)
        {
            StackPanel curr_pBox = rectMarker.p_PicBox; 

            Point movPos = new Point();
            Point afterPos = new Point();

            ///////////////////////////////////////
            movPos.X = curr_Pos.X - p_First_Click_Pos.X;
            movPos.Y = curr_Pos.Y - p_First_Click_Pos.Y;

            afterPos.X = Canvas.GetLeft(curr_pBox) + movPos.X;
            afterPos.Y = Canvas.GetTop(curr_pBox) + movPos.Y;

            Canvas.SetLeft(curr_pBox, afterPos.X);
            Canvas.SetTop(curr_pBox, afterPos.Y);
        }
        public void MK_RectMoving(Rectangle rect, Point curr_Pos)
        {
            Rectangle curr_rect = rect;

            Point movPos = new Point();
            Point afterPos = new Point();

            ///////////////////////////////////////
            movPos.X = curr_Pos.X - p_First_Click_Pos.X;
            movPos.Y = curr_Pos.Y - p_First_Click_Pos.Y;

            afterPos.X = Canvas.GetLeft(curr_rect) + movPos.X;
            afterPos.Y = Canvas.GetTop(curr_rect) + movPos.Y;

            Canvas.SetLeft(curr_rect, afterPos.X);
            Canvas.SetTop(curr_rect, afterPos.Y);
        }
        public void MK_MoveSizeControlRect(Point curr_Pos)
        {
            Dictionary<Direction, SizeConRectangleItem> conRects = m_SizeConrects;
            /*
            uint imgId = 0;
            System.Collections.IList selected_imgs = (System.Collections.IList)IMG_LSTVIEW_UI.SelectedItems;
            if (selected_imgs.Count == 0)
                return;

            inner_GetSelLstItemImgID(ref imgId, selected_imgs);
            if (m_ImgPicBoxBuffer.ContainsKey(imgId) == false)
                return;

            String outRectId = (String)outRect.Tag;
            PictureBox picBox = m_ImgPicBoxBuffer[imgId].p_PicBoxs[outRectId];
            conRects = picBox.p_PicBoxObj.sizeConRect_ref;

            if (conRects == null)
                return;
            */
            Rectangle conRect = null;
            conRect = conRects[Direction.NW].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.N].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.NE].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.W].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.E].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.SW].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.S].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);

            conRect = conRects[Direction.SE].sizeConRect_ref as Rectangle;
            MK_RectMoving(conRect, curr_Pos);
        }
        public void MK_OutRectSizing()
        {
            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);

            //left, top 마지노선 버그 추가해야함
            m_movPos.X = (m_movPos.X <= 5) ? 5.0f : m_movPos.X; //Left
            m_movPos.Y = (m_movPos.Y <= 5) ? 5.0f : m_movPos.Y; //top

            //right, bottom 마지노선 구현 완료
            m_movPos.X = (m_movPos.X >= m_cv.Width - 10) ? m_cv.Width - 5 : m_movPos.X;
            m_movPos.Y = (m_movPos.Y >= m_cv.Height - 10) ? m_cv.Height - 5 : m_movPos.Y;

            mov_Width = m_movPos.X - m_First_Click_Pos.X;
            mov_Height = m_movPos.Y - m_First_Click_Pos.Y;

            if (mov_Height < 0 && mov_Width < 0) // 마우스가 클릭 기준점보다 위쪽과 왼쪽으로 이동할때 (↖)
            {
                Canvas.SetTop(m_PicBox, m_movPos.Y);
                Canvas.SetLeft(m_PicBox, m_movPos.X);
            }
            else if (mov_Height < 0 && mov_Width >= 0) // 마우스가 클릭 기준점보다 위쪽과 오른쪽으로 이동 할때 (↗)
                Canvas.SetTop(m_PicBox, m_movPos.Y);

            else if (mov_Height >= 0 && mov_Width < 0) // 마우스가 클릭기준점보다 아래와 왼쪽으로 이동할때 (↙)
                Canvas.SetLeft(m_Rect, m_movPos.X);

            // (↘)은 Default 
            m_Rect.Width = Math.Abs(mov_Width);
            m_Rect.Height = Math.Abs(mov_Height);
        }
        public void MK_CreateSizeControlRect()
        {
            //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
            //around the control, so when the drag handles are drawn they will be seem
            //connected in the middle.

            Dictionary<Direction, SizeConRectangleItem> conRect = m_SizeConrects;
            String keyValue = null;
            

            double x = Canvas.GetLeft(m_PicBox);
            double y = Canvas.GetTop(m_PicBox) + m_Label.Height;// TextBlock Offset 만큼 변경

            //define the 8 drag handles, that has the size o;f DRAG_HANDLE_SIZE
            //NW
            Rectangle NW = new Rectangle();
            NW.Name = "RECT_NW";
            MK_GeneratorKeyValue(ref keyValue);
            NW.Tag = keyValue;

            SizeConRectangleItem NW_Item = new SizeConRectangleItem();
            NW_Item.Name = NW.Name;
            NW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            NW_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            NW_Item.sizeConRect_ref = NW;
            NW_Item.p_Tag = keyValue;

            Canvas.SetLeft(NW, NW_Item.Left);
            Canvas.SetTop(NW, NW_Item.Top);

            //event set
            NW.MouseEnter += OnSizeConRect_Enter;
            NW.MouseLeave += OnSizeConRect_Leave;
            NW.MouseUp += OnSizeConRect_Up;
            NW.MouseDown += OnSizeConRect_Down;
            NW.MouseMove += OnSizeConRect_Move;

            //append
            inner_ResizeRectagle(NW);
            m_cv.Children.Add(NW);
            conRect[Direction.NW] = NW_Item;

            //N
            Rectangle N = new Rectangle();
            N.Name = "RECT_N";
            MK_GeneratorKeyValue(ref keyValue);
            N.Tag = keyValue;

            SizeConRectangleItem N_Item = new SizeConRectangleItem();
            N_Item.Name = N.Name;
            N_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
            N_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            N_Item.sizeConRect_ref = N;
            N_Item.p_Tag = keyValue;
            Canvas.SetLeft(N, N_Item.Left);
            Canvas.SetTop(N, N_Item.Top);

            //event set
            N.MouseEnter += OnSizeConRect_Enter;
            N.MouseLeave += OnSizeConRect_Leave;
            N.MouseUp += OnSizeConRect_Up;
            N.MouseDown += OnSizeConRect_Down;
            N.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(N);
            m_cv.Children.Add(N);
            conRect[Direction.N] = N_Item;

            //NE
            Rectangle NE = new Rectangle();
            NE.Name = "RECT_NE";
            MK_GeneratorKeyValue(ref keyValue);
            NE.Tag = keyValue;
            SizeConRectangleItem NE_Item = new SizeConRectangleItem();
            NE_Item.Name = NE.Name;
            NE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            NE_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            NE_Item.sizeConRect_ref = NE;
            NE_Item.p_Tag = keyValue;
            Canvas.SetLeft(NE, NE_Item.Left);
            Canvas.SetTop(NE, NE_Item.Top);

            //event set
            NE.MouseEnter += OnSizeConRect_Enter;
            NE.MouseLeave += OnSizeConRect_Leave;
            NE.MouseUp += OnSizeConRect_Up;
            NE.MouseDown += OnSizeConRect_Down;
            NE.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(NE);
            m_cv.Children.Add(NE);
            conRect[Direction.NE] = NE_Item;

            //W
            Rectangle W = new Rectangle();
            W.Name = "RECT_W";
            MK_GeneratorKeyValue(ref keyValue);
            W.Tag = keyValue;

            SizeConRectangleItem W_Item = new SizeConRectangleItem();
            W_Item.Name = W.Name;
            W_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            W_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
            W_Item.sizeConRect_ref = W;
            W_Item.p_Tag = keyValue;
            Canvas.SetLeft(W, W_Item.Left);
            Canvas.SetTop(W, W_Item.Top);

            //event set
            W.MouseEnter += OnSizeConRect_Enter;
            W.MouseLeave += OnSizeConRect_Leave;
            W.MouseUp += OnSizeConRect_Up;
            W.MouseDown += OnSizeConRect_Down;
            W.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(W);
            m_cv.Children.Add(W);
            conRect[Direction.W] = W_Item;

            //E
            Rectangle E = new Rectangle();
            E.Name = "RECT_E";
            MK_GeneratorKeyValue(ref keyValue);
            E.Tag = keyValue;

            SizeConRectangleItem E_Item = new SizeConRectangleItem();
            E_Item.Name = E.Name;
            E_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            E_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
            E_Item.sizeConRect_ref = E;
            E_Item.p_Tag = keyValue;
            Canvas.SetLeft(E, E_Item.Left);
            Canvas.SetTop(E, E_Item.Top);

            //event set
            E.MouseEnter += OnSizeConRect_Enter;
            E.MouseLeave += OnSizeConRect_Leave;
            E.MouseUp += OnSizeConRect_Up;
            E.MouseDown += OnSizeConRect_Down;
            E.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(E);
            m_cv.Children.Add(E);
            conRect[Direction.E] = E_Item;

            //SW
            Rectangle SW = new Rectangle();
            SW.Name = "RECT_SW";
            MK_GeneratorKeyValue(ref keyValue);
            SW.Tag = keyValue;

            SizeConRectangleItem SW_Item = new SizeConRectangleItem();
            SW_Item.Name = SW.Name;
            SW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            SW_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            SW_Item.sizeConRect_ref = SW;
            SW_Item.p_Tag = keyValue;
            Canvas.SetLeft(SW, SW_Item.Left);
            Canvas.SetTop(SW, SW_Item.Top);

            //event set
            SW.MouseEnter += OnSizeConRect_Enter;
            SW.MouseLeave += OnSizeConRect_Leave;
            SW.MouseUp += OnSizeConRect_Up;
            SW.MouseDown += OnSizeConRect_Down;
            SW.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(SW);
            m_cv.Children.Add(SW);
            conRect[Direction.SW] = SW_Item;

            //S
            Rectangle S = new Rectangle();
            S.Name = "RECT_S";
            MK_GeneratorKeyValue(ref keyValue);
            S.Tag = keyValue;

            SizeConRectangleItem S_Item = new SizeConRectangleItem();
            S_Item.Name = S.Name;
            S_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
            S_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            S_Item.sizeConRect_ref = S;
            S_Item.p_Tag = keyValue;
            Canvas.SetLeft(S, S_Item.Left);
            Canvas.SetTop(S, S_Item.Top);

            //event set
            S.MouseEnter += OnSizeConRect_Enter;
            S.MouseLeave += OnSizeConRect_Leave;
            S.MouseUp += OnSizeConRect_Up;
            S.MouseDown += OnSizeConRect_Down;
            S.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(S);
            m_cv.Children.Add(S);
            conRect[Direction.S] = S_Item;

            //SE
            Rectangle SE = new Rectangle();
            SE.Name = "RECT_SE";
            MK_GeneratorKeyValue(ref keyValue);
            SE.Tag = keyValue;

            SizeConRectangleItem SE_Item = new SizeConRectangleItem();
            SE_Item.Name = SE.Name;
            SE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            SE_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            SE_Item.sizeConRect_ref = SE;
            SE_Item.p_Tag = keyValue;
            Canvas.SetLeft(SE, SE_Item.Left);
            Canvas.SetTop(SE, SE_Item.Top);

            //event set
            SE.MouseEnter += OnSizeConRect_Enter;
            SE.MouseLeave += OnSizeConRect_Leave;
            SE.MouseUp += OnSizeConRect_Up;
            SE.MouseDown += OnSizeConRect_Down;
            SE.MouseMove += OnSizeConRect_Move;
            //append
            inner_ResizeRectagle(SE);
            m_cv.Children.Add(SE);
            conRect[Direction.SE] = SE_Item;
        }
        public void MK_ReMoveSizeControlRect()
        {
            //define the border to be drawn, it will be offset by DRAG_HANDLE_SIZE / 2
            //around the control, so when the drag handles are drawn they will be seem
            //connected in the middle.

            Dictionary<Direction, SizeConRectangleItem> conRect = m_SizeConrects;

            double x = Canvas.GetLeft(m_PicBox);
            double y = Canvas.GetTop(m_PicBox) + m_Label.Height;// TextBlock Offset 만큼 변경

            //define the 8 drag handles, that has the size o;f DRAG_HANDLE_SIZE
            //NW
            Rectangle tempSizeRect = null;

            SizeConRectangleItem NW_Item = conRect[Direction.NW];
            tempSizeRect = NW_Item.sizeConRect_ref;

            NW_Item.Name = tempSizeRect.Name;
            NW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            NW_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            NW_Item.sizeConRect_ref = tempSizeRect;
            NW_Item.p_Tag = (string) tempSizeRect.Tag;

            Canvas.SetLeft(tempSizeRect, NW_Item.Left);
            Canvas.SetTop(tempSizeRect, NW_Item.Top);

            //N
            SizeConRectangleItem N_Item = conRect[Direction.N];
            tempSizeRect = N_Item.sizeConRect_ref;

            N_Item.Name = tempSizeRect.Name;
            N_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
            N_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            N_Item.sizeConRect_ref = tempSizeRect;
            N_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, N_Item.Left);
            Canvas.SetTop(tempSizeRect, N_Item.Top);

            //NE
            SizeConRectangleItem NE_Item = conRect[Direction.NE];
            tempSizeRect = NE_Item.sizeConRect_ref;
            NE_Item.Name = tempSizeRect.Name;
            NE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            NE_Item.Top = y - DRAG_HANDLE_SIZE / 2;
            NE_Item.sizeConRect_ref = tempSizeRect;
            NE_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, NE_Item.Left);
            Canvas.SetTop(tempSizeRect, NE_Item.Top);

            //W
            SizeConRectangleItem W_Item = conRect[Direction.W];
            tempSizeRect = W_Item.sizeConRect_ref;

            W_Item.Name = tempSizeRect.Name;
            W_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            W_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
            W_Item.sizeConRect_ref = tempSizeRect;
            W_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, W_Item.Left);
            Canvas.SetTop(tempSizeRect, W_Item.Top);

            //E
            SizeConRectangleItem E_Item = conRect[Direction.E]; ;
            tempSizeRect = E_Item.sizeConRect_ref;

            E_Item.Name = tempSizeRect.Name;
            E_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            E_Item.Top = y + m_Rect.Height / 2 - DRAG_HANDLE_SIZE / 2;
            E_Item.sizeConRect_ref = tempSizeRect;
            E_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, E_Item.Left);
            Canvas.SetTop(tempSizeRect, E_Item.Top);

            //SW
            SizeConRectangleItem SW_Item = conRect[Direction.SW];
            tempSizeRect = SW_Item.sizeConRect_ref;

            SW_Item.Name = tempSizeRect.Name;
            SW_Item.Left = x - DRAG_HANDLE_SIZE / 2;
            SW_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            SW_Item.sizeConRect_ref = tempSizeRect;
            SW_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, SW_Item.Left);
            Canvas.SetTop(tempSizeRect, SW_Item.Top);

            //S
            SizeConRectangleItem S_Item = conRect[Direction.S];
            tempSizeRect = S_Item.sizeConRect_ref;

            S_Item.Name = tempSizeRect.Name;
            S_Item.Left = x + m_Rect.Width / 2 - DRAG_HANDLE_SIZE / 2;
            S_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            S_Item.sizeConRect_ref = tempSizeRect;
            S_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, S_Item.Left);
            Canvas.SetTop(tempSizeRect, S_Item.Top);

            //SE
            SizeConRectangleItem SE_Item = conRect[Direction.SE];
            tempSizeRect = SE_Item.sizeConRect_ref;

            SE_Item.Name = tempSizeRect.Name;
            SE_Item.Left = x + m_Rect.Width - DRAG_HANDLE_SIZE / 2;
            SE_Item.Top = y + m_Rect.Height - DRAG_HANDLE_SIZE / 2;
            SE_Item.sizeConRect_ref = tempSizeRect;
            SE_Item.p_Tag = (string)tempSizeRect.Tag; 

            Canvas.SetLeft(tempSizeRect, SE_Item.Left);
            Canvas.SetTop(tempSizeRect, SE_Item.Top);
        }
        public void MK_RemoveSizeControlRect()
        {
            SizeConRectangleItem sizeItem = null;

            sizeItem = m_SizeConrects[Direction.NW];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.N];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.NE];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.W];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.E];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.SW];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.S];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

            sizeItem = m_SizeConrects[Direction.SE];
            m_cv.Children.Remove(sizeItem.sizeConRect_ref);

        }
        public void MK_ChangeOutLineRectSize(Rectangle sizeRect, Direction dir)
        {
            /*
            Rectangle outNemo = null;
            inner_FindOutNemo(sizeRect, ref outNemo, dir);
            if (outNemo == null)
                return;
            */

            switch (dir)
            {
                case Direction.NW:
                    inner_SetSize_NW();
                    break;

                case Direction.N:
                    inner_SetSize_N();
                    break;

                case Direction.NE:
                    inner_SetSize_NE();
                    break;

                case Direction.W:
                    inner_SetSize_W();
                    break;

                case Direction.E:
                    inner_SetSize_E();
                    break;

                case Direction.SW:
                    inner_SetSize_SW();
                    break;

                case Direction.S:
                    inner_SetSize_S();
                    break;

                case Direction.SE:
                    inner_SetSize_SE();
                    break;

                case Direction.None:
                    break;
            }
        }
        public ERR_RESULT MK_Re_RegisteRectEvent()
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                if (m_Rect == null)
                    return result;

                m_PicBox.Children.Clear();
                m_PicBox.Children.Add(m_Label);
                m_PicBox.Children.Add(m_Rect);

                m_Rect.MouseEnter += OnRect_Enter;
                m_Rect.MouseLeave += OnRect_Leave;
                m_Rect.MouseMove += OnRect_Move;
                m_Rect.MouseUp += OnRect_Up;
                m_Rect.MouseDown += OnRect_Down;
                
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
        }
        public ERR_RESULT MK_Re_RegisteSizeNemoEvent()
        {
            ERR_RESULT result = new ERR_RESULT();
            SizeConRectangleItem sizeItem = null;
            try
            {
                sizeItem = m_SizeConrects[Direction.NW];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.N];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.NE];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.W];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.E];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.SW];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.S];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;

                sizeItem = m_SizeConrects[Direction.SE];
                sizeItem.sizeConRect_ref.MouseEnter += OnSizeConRect_Enter;
                sizeItem.sizeConRect_ref.MouseLeave += OnSizeConRect_Leave;
                sizeItem.sizeConRect_ref.MouseUp += OnSizeConRect_Up;
                sizeItem.sizeConRect_ref.MouseDown += OnSizeConRect_Down;
                sizeItem.sizeConRect_ref.MouseMove += OnSizeConRect_Move;
                 
                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
        }

        /*
         * *innerMethod
         */
        private ERR_RESULT inner_AllocMarker(SolidColorBrush lineColor, double width, double height, String label = "label")
        {
            ERR_RESULT result = new ERR_RESULT();
            String keyValue = null;
            try
            {
                // Alloc Rect
                m_Rect = new Rectangle();
                MK_GeneratorKeyValue(ref keyValue);
                m_Rect.Tag = keyValue;
                base.m_Tag = keyValue; //외부에서 여기를 주로 사용함.
                m_Rect.Stroke = lineColor;
                m_Rect.Fill = Brushes.Blue;
                m_Rect.Opacity = 0.5;
                m_Rect.Height = height;
                m_Rect.Width = width;

                // Reg Event 
                m_Rect.MouseEnter += OnRect_Enter;
                m_Rect.MouseLeave += OnRect_Leave;
                m_Rect.MouseMove += OnRect_Move;
                m_Rect.MouseUp += OnRect_Up;
                m_Rect.MouseDown += OnRect_Down;

                // Alloc TextBlock
                m_Label = new TextBlock();
                m_Label.Text = label;
                m_Label.Height = 16;

                // Arrange
                m_PicBox = new StackPanel();
                m_PicBox.Orientation = Orientation.Vertical;

                m_PicBox.Children.Add(m_Label);
                m_PicBox.Children.Add(m_Rect);

                return result;
            }
            catch (_MarkerException err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
            catch(Exception err)
            {
                result = ErrProcessXml.SetErrResult(err);
                return result;
            }
        }
        private void inner_initSizeRectagle(Rectangle rect)
        {
            rect.Stroke = Brushes.White;
            rect.Fill = Brushes.Black;
            rect.Width = DRAG_HANDLE_SIZE;
            rect.Height = DRAG_HANDLE_SIZE;
        }
                
        private void inner_ConSizeRectSet(Point curr_Pos)
        {
            Dictionary<Direction, SizeConRectangleItem> conRects = null;
            conRects = m_SizeConrects;
            if (conRects == null)
                return;

            Rectangle conRect = null;
            conRect = conRects[Direction.NW].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.NW);

            conRect = conRects[Direction.N].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.N);

            conRect = conRects[Direction.NE].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.NE);

            conRect = conRects[Direction.W].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.W);

            conRect = conRects[Direction.E].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.E);

            conRect = conRects[Direction.SW].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.SW);

            conRect = conRects[Direction.S].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.S);

            conRect = conRects[Direction.SE].sizeConRect_ref as Rectangle;
            inner_SetSizeConRectPos(conRect, curr_Pos, Direction.SE);
        }
        private void inner_SetSizeConRectPos(Rectangle sizeRect, Point curr_Pos, Direction dir)
        {
            if (sizeRect == null)
                return;

            Rectangle curr_SizeRect = sizeRect;
            Rectangle Outline_rect = m_Rect;

            Point movPos = new Point();
            Point afterPos = new Point();

            movPos.X = curr_Pos.X - m_First_Click_Pos.X;
            movPos.Y = curr_Pos.Y - m_First_Click_Pos.Y;

            switch (dir)
            {
                case Direction.NW:
                    Canvas.SetLeft(curr_SizeRect, curr_Pos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, curr_Pos.Y - DRAG_HANDLE_SIZE / 2);
                    break;

                case Direction.N:
                    afterPos.X = curr_Pos.X + (movPos.X + Outline_rect.Width) / 2;
                    afterPos.Y = (movPos.Y + Outline_rect.Height);
                    Canvas.SetLeft(curr_SizeRect, afterPos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, curr_Pos.Y - DRAG_HANDLE_SIZE / 2);
                    break;
                case Direction.NE:
                    afterPos.X = movPos.X + (curr_Pos.X + Outline_rect.Width);
                    Canvas.SetLeft(curr_SizeRect, afterPos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, curr_Pos.Y - DRAG_HANDLE_SIZE / 2);
                    break;

                case Direction.W:
                    afterPos.Y = curr_Pos.Y + (movPos.Y + Outline_rect.Height) / 2;
                    Canvas.SetLeft(curr_SizeRect, curr_Pos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, afterPos.Y - DRAG_HANDLE_SIZE / 2);
                    break;

                case Direction.E:
                    afterPos.X = movPos.X + (curr_Pos.X + Outline_rect.Width);
                    afterPos.Y = curr_Pos.Y + (movPos.Y + Outline_rect.Height) / 2;
                    Canvas.SetLeft(curr_SizeRect, afterPos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, afterPos.Y - DRAG_HANDLE_SIZE / 2);
                    break;
                case Direction.SW:
                    afterPos.Y = movPos.Y + (curr_Pos.Y + Outline_rect.Height);
                    Canvas.SetLeft(curr_SizeRect, curr_Pos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, afterPos.Y - DRAG_HANDLE_SIZE / 2);
                    break;
                case Direction.S:
                    afterPos.X = curr_Pos.X + (movPos.X + Outline_rect.Width) / 2;
                    afterPos.Y = movPos.Y + (curr_Pos.Y + Outline_rect.Height);
                    Canvas.SetLeft(curr_SizeRect, afterPos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, afterPos.Y - DRAG_HANDLE_SIZE / 2);
                    break;

                case Direction.SE:
                    afterPos.X = movPos.X + (curr_Pos.X + Outline_rect.Width);
                    afterPos.Y = movPos.Y + (curr_Pos.Y + Outline_rect.Height);
                    Canvas.SetLeft(curr_SizeRect, afterPos.X - DRAG_HANDLE_SIZE / 2);
                    Canvas.SetTop(curr_SizeRect, afterPos.Y - DRAG_HANDLE_SIZE / 2);
                    break;

                default:
                    break;
            }
        }
        private void inner_SetSize_NW()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            double len_X = m_First_Click_Pos.X - m_movPos.X;//늘어나야할 width
            double len_Y = m_First_Click_Pos.Y - m_movPos.Y;//늘어나야할 height

            double rect_width = len_X + OutLineRect.Width;
            double rect_height = len_Y + OutLineRect.Height;

            mov_Width = OutLineRect.Width + len_X;
            mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
                return;

            OutLineRect.Width = Math.Abs(mov_Width);
            OutLineRect.Height = Math.Abs(mov_Height);
            Canvas.SetTop(m_PicBox, m_movPos.Y - m_Label.Height);//label의 Offset
            Canvas.SetLeft(m_PicBox, m_movPos.X);

            //LDH8282 아래의 함수를 대체
            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_N()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            m_movPos.X = Canvas.GetLeft(m_PicBox);//Left, Right 고정
            //double len_X = m_First_Click_rect_Pos.X - m_movPos.X;//늘어나야할 width
            double len_Y = m_First_Click_Pos.Y - m_movPos.Y;//늘어나야할 height

            mov_Width = OutLineRect.Width;
            mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
                return;

            //OutLineRect.Width = Math.Abs(mov_Width);
            OutLineRect.Height = Math.Abs(mov_Height);
            Canvas.SetTop(m_PicBox, m_movPos.Y - m_Label.Height);
            Canvas.SetLeft(m_PicBox, m_movPos.X);


            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_NE()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);

            double len_X = m_movPos.X - (m_First_Click_Pos.X + OutLineRect.Width);//늘어나야할 width
            double len_Y = m_First_Click_Pos.Y - m_movPos.Y;//늘어나야할 height

            mov_Width = OutLineRect.Width + len_X;
            mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
                return;

            OutLineRect.Width = mov_Width;
            OutLineRect.Height = mov_Height;

            Canvas.SetTop(m_PicBox, m_movPos.Y - m_Label.Height);//Left 값 고정
            Canvas.SetLeft(m_PicBox, m_First_Click_Pos.X);

            m_movPos.X = Canvas.GetLeft(m_PicBox);
            inner_ConSizeRectSet(m_movPos);

            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_W()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;

            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            m_movPos.Y = Canvas.GetTop(m_PicBox);//Left, Right 고정

            double len_X = m_First_Click_Pos.X - m_movPos.X;//늘어나야할 width
            //double len_Y = m_First_Click_rect_Pos.Y - m_movPos.Y;//늘어나야할 height

            mov_Width = OutLineRect.Width + len_X;
            //mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4)
                return;

            OutLineRect.Width = mov_Width;
            //OutLineRect.Height = mov_Height;

            m_movPos.Y = Canvas.GetTop(m_PicBox) + m_Label.Height ;
            Canvas.SetLeft(m_PicBox, m_movPos.X);

            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_E()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다
            double mov_Width = 0;

            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            m_movPos.Y = Canvas.GetTop(OutLineRect);//Left, Right 고정

            double len_X = m_movPos.X - (m_First_Click_Pos.X + OutLineRect.Width);//늘어나야할 width
            //double len_Y = m_First_Click_rect_Pos.Y - m_movPos.Y;//늘어나야할 height

            mov_Width = OutLineRect.Width + len_X;
            //mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4)
                return;

            OutLineRect.Width = mov_Width;
            //OutLineRect.Height = mov_Height;

            m_movPos.Y = Canvas.GetTop(m_PicBox) + m_Label.Height;
            Canvas.SetLeft(OutLineRect, m_First_Click_Pos.X);

            m_movPos.X = Canvas.GetLeft(m_PicBox);
            
            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_SW()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            double len_X = m_First_Click_Pos.X - m_movPos.X;//늘어나야할 width
            double len_Y = m_movPos.Y - (m_First_Click_Pos.Y + OutLineRect.Height);//늘어나야할 height

            double rect_width = len_X + OutLineRect.Width;
            double rect_height = len_Y + OutLineRect.Height;

            mov_Width = OutLineRect.Width + len_X;
            mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
                return;

            OutLineRect.Width = Math.Abs(mov_Width);
            OutLineRect.Height = Math.Abs(mov_Height);
            //Canvas.SetTop(OutLineRect, m_movPos.Y);//Top값 고정
            Canvas.SetLeft(m_PicBox, m_movPos.X);

            m_movPos.Y = Canvas.GetTop(m_PicBox) + m_Label.Height;
            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_S()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);
            m_movPos.X = Canvas.GetLeft(m_PicBox);//Left, Right 고정
            //double len_X = m_First_Click_rect_Pos.X - m_movPos.X;//늘어나야할 width
            double len_Y = m_movPos.Y - (m_First_Click_Pos.Y + OutLineRect.Height);//늘어나야할 height

            mov_Width = OutLineRect.Width;
            mov_Height = OutLineRect.Height + len_Y;

            if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
                return;

            //OutLineRect.Width = Math.Abs(mov_Width);
            OutLineRect.Height = Math.Abs(mov_Height);
            //Canvas.SetTop(OutLineRect, m_movPos.Y);
            Canvas.SetLeft(m_PicBox, m_movPos.X);

            m_movPos.Y = Canvas.GetTop(m_PicBox) + m_Label.Height;
            inner_ConSizeRectSet(m_movPos);
            m_First_Click_Pos = m_movPos;
        }
        private void inner_SetSize_SE()
        {
            Rectangle OutLineRect = m_Rect;//사이즈변경 네모의 ID를 이용 아웃라인 네모를 가져온다

            double mov_Width = 0;
            double mov_Height = 0;
            //마우스 포지션
            m_movPos = Mouse.GetPosition(m_cv);

            double len_X = m_movPos.X - (m_First_Click_Pos.X + OutLineRect.Width);//늘어나야할 width
            double len_Y = m_movPos.Y - (m_First_Click_Pos.Y + OutLineRect.Height);//늘어나야할 height

            mov_Width = OutLineRect.Width + len_X;
            mov_Height = OutLineRect.Height + len_Y;

            // if (mov_Width < DRAG_HANDLE_SIZE * 4 || mov_Height < DRAG_HANDLE_SIZE * 4)
            //     return;

            OutLineRect.Width = mov_Width;
            OutLineRect.Height = mov_Height;

            //Canvas.SetTop(OutLineRect, m_movPos.Y);//Left 값 고정
            //Canvas.SetLeft(OutLineRect, m_First_Click_rect_Pos.X);

            m_movPos.X = Canvas.GetLeft(m_PicBox);
            m_movPos.Y = Canvas.GetTop(m_PicBox) + m_Label.Height;
            inner_ConSizeRectSet(m_movPos);

            m_First_Click_Pos = m_movPos;
        }
        
        private void inner_ResizeRectagle(Rectangle rect)
        {
            rect.Stroke = Brushes.White;
            rect.Fill = Brushes.White;
            rect.Width = DRAG_HANDLE_SIZE;
            rect.Height = DRAG_HANDLE_SIZE;
        }
        /*
         * callback
         */
        //OutNemo MainWindow 의 등록된 이벤트 델리게이트 구현
        private void OnRect_Enter(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = outRectEnter;
            if (handler != null)
            {
                handler(this, e);
            }
            return;
        }
        private void OnRect_Leave(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = outRectLeave;
            if (handler != null)
            {
                handler(this, e);
            }
            return;
        }
        private void OnRect_Up(object sender, MouseButtonEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseButtonEventHandler handler = outRectUp;
            if (handler != null)
            {
                handler(this, e);
            }
            return;
        }
        private void OnRect_Down(object sender, MouseButtonEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseButtonEventHandler handler = outRectDown;
            if (handler != null)
            {
                handler(this, e);
            }
            return;
        }
        private void OnRect_Move(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = outRectMove;
            if (handler != null)
            {
                handler(this, e);
            }
            return;
        }

        //SizeCon MainWindow 의 등록된 이벤트 델리게이트구현
        private void OnSizeConRect_Enter(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = sizeConEnter;
            if (handler != null)
            {
                handler(sender, e);
            }
            return;
        }
        private void OnSizeConRect_Leave(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = sizeConLeave;
            if (handler != null)
            {
                handler(sender, e);
            }
            return;
        }
        private void OnSizeConRect_Up(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = sizeConUp;
            if (handler != null)
            {
                handler(sender, e);
            }
            return;
        }
        private void OnSizeConRect_Down(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = sizeConDown;
            if (handler != null)
            {
                handler(sender, e);
            }
            return;
        }
        private void OnSizeConRect_Move(object sender, MouseEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            MouseEventHandler handler = sizeConMove;
            if (handler != null)
            {
                handler(sender, e);
            }
            return;
        }
    }
}
