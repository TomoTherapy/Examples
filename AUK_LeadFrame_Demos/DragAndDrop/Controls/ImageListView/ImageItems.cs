using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Windows.Markup;
using DeepObjectDector.sub.lib;
using DeepObjectDector.sub.lib.err;

namespace DeepObjectDector.sub.control.ImageListView
{
    public enum Direction
    {
        NW,
        N,
        NE,
        W,
        E,
        SW,
        S,
        SE,
        All,
        None
    }
    
    public class SizeConRectangleItem : IXmlSerializable
    {
       /*
        * member val
        */
        private String m_Name;
        private double m_Width;
        private double m_Height;
        private double m_Top;
        private double m_left;
        private Rectangle m_SizeConRect_ref;
        private String m_Tag;

        /*
         * property
         */
        public String Name
        {
            get { return (String)m_Name; }
            set { m_Name = value; }
        }
        public double Width
        {
            get { return (double)m_Width; }
            set { m_Width = value; }
        }
        public double Height
        {
            get { return (double)m_Height; }
            set { m_Height = value; }
        }
        public double Top
        {
            get { return (double)m_Top; }
            set { m_Top = value; }
        }
        public double Left
        {
            get { return (double)m_left; }
            set { m_left = value; }
        }
        public Rectangle sizeConRect_ref
        {
            get { return (Rectangle)m_SizeConRect_ref; }
            set { m_SizeConRect_ref = value; }
        }
        public String p_Tag
        {
            get { return m_Tag; }
            set { m_Tag = (String)value; }
        }
        /*
         * construct
         */
        public SizeConRectangleItem()
        {
            m_Name  = null;
            m_Width = 0.0;
            m_Height = 0.0;
            m_Top   = 0.0; 
            m_left  = 0.0;
        }
        /*
         * *method
         */
        public XmlSchema GetSchema() { return null; }
        public void ReadXml(XmlReader reader)
        {
            /*
             *  private String m_Name;
                private double m_Width;
                private double m_Height;
                private double m_Top;
                private double m_left;
                private Rectangle m_SizeConRect_ref;
             * */
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SizeConMarkers")
            {
                m_Name = reader["Name"];
                m_Width = double.Parse(reader["Width"]);
                m_Height = double.Parse(reader["Height"]);
                m_Top = double.Parse(reader["Top"]);
                m_left = double.Parse(reader["left"]);
                m_Tag = reader["Tag"];
                if (reader["SizeConMarkers"] != null)
                    m_SizeConRect_ref = (Rectangle)XamlReader.Parse(reader["SizeConMarkers"]);
                
                reader.Read();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", m_Name);
            writer.WriteAttributeString("Width", String.Format("{0}",m_Width));
            writer.WriteAttributeString("Height", String.Format("{0}",m_Height));
            writer.WriteAttributeString("Top", String.Format("{0}",m_Top));
            writer.WriteAttributeString("left", String.Format("{0}",m_left));
            writer.WriteAttributeString("Tag", m_Tag);
            writer.WriteAttributeString("SizeConMarkers", XamlWriter.Save(m_SizeConRect_ref));
        }
    }
    public class OutLineRectangleItem : IXmlSerializable
    {
        /*
         * member val
         */
        private String m_Name;//"OutMarker_" + rectangle_hashCode()
        private double m_Width;
        private double m_Height;
        private double m_Top;
        private double m_Left;
        private uint m_ImgHash;//"IMG_xxxxx" 의 XXXX 
        private String m_ImgFileName;
        private Marker m_Marker_ref;
        //private Dictionary<Direction, SizeConRectangleItem> m_SizeConRects_ref; LDH8282 사용안함
        /*
         * property
         */
        public String Name
        {
            get { return (String)m_Name;  }
            set { m_Name = value; }
        }
        public double Width
        {
            get { return (double)m_Width; }
            set { m_Width = value; }
        }
        public double Height
        {
            get { return (double)m_Height; }
            set { m_Height = value; }
        }
        public double Top
        {
            get { return (double)m_Top; }
            set { m_Top = value; }
        }
        public double Left
        {
            get { return (double)m_Left; }
            set { m_Left = value; }
        }
        public uint ImgHash
        {
            get { return (uint)m_ImgHash; }
            set { m_ImgHash = value; }
        }
        public String ImgFileName
        {
            get { return (String) m_ImgFileName; }
            set { m_ImgFileName = value; }
        }
        public Marker marker_ref
        {
            get { return (Marker)m_Marker_ref; }
            set { m_Marker_ref = value; }
        }
        /*LDH8282 사용안함
        public Dictionary<Direction, SizeConRectangleItem> sizeConRect_ref
        {
            get { return (Dictionary<Direction, SizeConRectangleItem>)m_SizeConRects_ref; }
            set { m_SizeConRects_ref = value; }
        }*/

        /*
         * construct
         */
        public OutLineRectangleItem()
        {
            m_Name  = null;
            m_Width = 0.0;
            m_Height = 0.0;
            m_Top   = 0.0; 
            m_Left  = 0.0;
            m_ImgHash = 0;
            //m_SizeConRects_ref = new Dictionary<Direction, SizeConRectangleItem>(); LDH8282 사용안함
        }

        /*
         * method
         */
        public XmlSchema GetSchema() { return null; }
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "OutLineMarkers")
            {
                m_Name = reader["Name"];
                m_Width = double.Parse(reader["Width"]);
                m_Height = double.Parse(reader["Height"]);
                m_Top = double.Parse(reader["Top"]);
                m_Left = double.Parse(reader["Left"]);
                m_ImgHash = UInt32.Parse(reader["ImgHash"]);
                m_ImgFileName = reader["ImgFileName"];

                if (reader.ReadToDescendant("Marker_ref"))
                {
                    RectMarker marker = new RectMarker();
                    marker.ReadXml(reader);
                    m_Marker_ref = marker;
                }
                /*LDH8282 사용안함
                if (reader.LocalName == "SizeConMakers")
                {
                    //m_Tools
                    m_SizeConRects_ref.Clear();
                    Direction key = Direction.None;
                    String strKey = null;
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SizeConMakers")
                    {
                        strKey = reader.GetAttribute("Key");
                        key = inner_GetDirectionStrToEnum(strKey);
                        SizeConRectangleItem evt = new SizeConRectangleItem();
                        evt.ReadXml(reader);
                        m_SizeConRects_ref.Add(key, evt);//<- 여기서 터짐
                    }
                }*/
                reader.Read();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", m_Name);
            writer.WriteAttributeString("Width", String.Format("{0}",m_Width));
            writer.WriteAttributeString("Height", String.Format("{0}",m_Height));
            writer.WriteAttributeString("Top", String.Format("{0}",m_Top));
            writer.WriteAttributeString("Left", String.Format("{0}",m_Left));
            writer.WriteAttributeString("ImgHash", String.Format("{0}",m_ImgHash));
            writer.WriteAttributeString("ImgFileName", String.Format("{0}",m_ImgFileName));
            //LDH8282 시리얼 수정 해야함
            writer.WriteStartElement("Marker_ref");
            if (inner_IdentifiedWriteXml(writer) == -2)
            {
                writer.WriteEndElement();
                throw new _ImageItemException(-2);
            }
            writer.WriteEndElement();
            /*LDH8282 사용안함
            foreach (var key in m_SizeConRects_ref.Keys)
            {
                writer.WriteStartElement("SizeConMakers");
                writer.WriteAttributeString("Key", key.ToString());
                SizeConRectangleItem evt = m_SizeConRects_ref[key];
                evt.WriteXml(writer);
                writer.WriteEndElement();
            }*/
        }
        private Direction inner_GetDirectionStrToEnum(String strKey)
        {
            switch(strKey)
            {
                case"NW":
                    return Direction.NW;
                case"N":
                    return Direction.N;
                case"NE":
                    return Direction.NE;
                case"W":
                    return Direction.W;
                case"E":
                    return Direction.E;
                case"SW":
                    return Direction.SW;
                case"S":
                    return Direction.S;
                case"SE":
                    return Direction.SE;
                case"ALL":
                    return Direction.All;
                default:
                    return Direction.None;
            }
        }
        private long inner_IdentifiedWriteXml(XmlWriter writer)
        {
            MARKERTYPE type = m_Marker_ref.p_Type;
            switch (type)
            {
                case  MARKERTYPE.RECTANGLE:
                    RectMarker rectMarker = (RectMarker)m_Marker_ref;
                    rectMarker.WriteXml(writer);
                    break;
                default:
                    return -2;
            }

            return 0;
        }
    }
    public class ImageItem : FrameworkElement, IXmlSerializable
    {
        /*
         * DependencyProperty
         */
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
          "image_uri", typeof(String), typeof(ImageItem), new PropertyMetadata(null));//int나 값이 있을때는 꼭 0으로
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
           "fileName", typeof(String), typeof(ImageItem), new PropertyMetadata(null));//int나 값이 있을때는 꼭 0으로

        /*
         * member_var 
         */
        private Dictionary<String, OutLineRectangleItem> m_OutLineMarkers;//IMG_xxxx 참조
        private Dictionary<String, OutLineRectangleItem> m_DectedMarkers;//IMG_xxxx 참조
        //LDH0000 20190409 픽쳐박스 타입 object 변경
        private Marker m_EnterMarker;
        private Marker m_SelMarker;
        private Marker m_preSelMarker;
        private uint m_ImgId;

        /*
         * property
         */
        public Dictionary<String, OutLineRectangleItem> p_OutLineMarkers_ref//IMG_xxxx 참조
        {
            get { return (Dictionary<String, OutLineRectangleItem>)m_OutLineMarkers; }
            set { m_OutLineMarkers = value; }
        }
        public Dictionary<String, OutLineRectangleItem> p_DectedMarkers//IMG_xxxx 참조
        {
            get { return (Dictionary<String, OutLineRectangleItem>)m_DectedMarkers; }
            set { m_DectedMarkers = value; }
        }
        public String p_Image_uri //BitMap로 바꿔야한다.
         {
            get
            {
                return (String)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }
        public String p_FileName 
        {
            get
            {
                return (String)GetValue(FileNameProperty);
            }
            set
            {
                SetValue(FileNameProperty, value);
            }
        }
        public Marker p_EnterMarker
        {
            get { return m_EnterMarker; }
            set { m_EnterMarker = (Marker)value; }
        }
        public Marker p_SelMarker
        {
            get { return m_SelMarker; }
            set { m_SelMarker = (Marker)value; }
        }
        public Marker p_PreSelMarker
        {
            get { return m_preSelMarker; }
            set { m_preSelMarker = (Marker)value; }
        }
        public uint p_ImgId
        {
            get { return m_ImgId; }
            set { m_ImgId = (uint) value; }
        }

        /*
         * construct
         */
        public ImageItem()
        {
            m_OutLineMarkers = new Dictionary<String, OutLineRectangleItem>();
            m_DectedMarkers = new Dictionary<String, OutLineRectangleItem>();
            m_EnterMarker = null;
            m_SelMarker = null;
            m_preSelMarker = null;
            m_ImgId = 0;
        }

        /*
         * *method
         */
        public XmlSchema GetSchema() { return null; }
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ImageItems")
            {
                p_Image_uri = reader["Image_uri"];
                p_FileName = reader["FileName"];

                if(reader["Enternemo"] != null)
                    m_EnterMarker = (Marker)XamlReader.Parse(reader["EnterMarker"]);//LDH8282 시리얼 수정 해야함
                if (reader["Selnemo"] != null)
                    m_SelMarker = (Marker)XamlReader.Parse(reader["SelMarker"]);//LDH8282 시리얼 수정 해야함
                if (reader["preSelnemo"] != null)
                    m_preSelMarker = (Marker)XamlReader.Parse(reader["preSelMarker"]);//LDH8282 시리얼 수정 해야함
                if (reader["ImgId"] != null)
                    m_ImgId = UInt32.Parse(reader["ImgId"]);

                if (reader.ReadToDescendant("OutLineMarkers"))
                {
                    m_OutLineMarkers.Clear();
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "OutLineMarkers")
                    {
                        String key = reader.GetAttribute("Key");
                        OutLineRectangleItem evt = new OutLineRectangleItem();
                        evt.ReadXml(reader);
                        m_OutLineMarkers.Add(key, evt);//<- 여기서 터짐
                    }

                    m_DectedMarkers.Clear();
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "DectedMarkers")
                    {
                        String key = reader.GetAttribute("Key");
                        OutLineRectangleItem evt = new OutLineRectangleItem();
                        evt.ReadXml(reader);
                        m_DectedMarkers.Add(key, evt);//Dictionary<int, List<OutLineRectangleItem>>();
                    }
                }
                reader.Read();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Image_uri", p_Image_uri);
            writer.WriteAttributeString("FileName", p_FileName);

            if (m_EnterMarker != null)
                writer.WriteAttributeString("Enternemo", XamlWriter.Save(m_EnterMarker));
            if (m_SelMarker != null)
                writer.WriteAttributeString("Selnemo", XamlWriter.Save(m_SelMarker));
            if (m_preSelMarker != null)
                writer.WriteAttributeString("preSelnemo", XamlWriter.Save(m_preSelMarker));

            writer.WriteAttributeString("ImgId", String.Format("{0}", m_ImgId));

            foreach (var key in m_OutLineMarkers.Keys)
            {
                writer.WriteStartElement("OutLineMarkers");
                writer.WriteAttributeString("Key", key);
                OutLineRectangleItem evt = m_OutLineMarkers[key];
                evt.WriteXml(writer);
                writer.WriteEndElement();
            }

            foreach (var key in m_DectedMarkers.Keys)
            {
                writer.WriteStartElement("DectedMarkers");
                writer.WriteAttributeString("Key", key);
                writer.WriteAttributeString("Value", XamlWriter.Save(m_DectedMarkers[key]));
                writer.WriteEndElement();
            }
        }
    }
}
