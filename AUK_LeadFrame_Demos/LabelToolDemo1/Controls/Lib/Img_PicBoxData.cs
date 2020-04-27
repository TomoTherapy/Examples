using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using DeepObjectDector.sub.control.ImageListView;
using DeepObjectDector.sub.lib;

namespace DeepObjectDector.sub.doc
{
    class Img_PicBoxData : IDisposable
    {
        // Serial Free 
        /*
         * *membar var
         */
        private bool m_alreadyDisposed = false;
        private Dictionary<String, PictureBox> m_PicBoxs;
        private Dictionary<String, PictureBox> m_DectedPicBoxs;
        private String m_ImgFileName;
        private TaskClass m_TskObj;

        /*
         * *property
         */
        public Dictionary<String, PictureBox> p_PicBoxs
        {
            get { return m_PicBoxs; }
            set { m_PicBoxs = (Dictionary<String, PictureBox>) value; }
        }
        public Dictionary<String, PictureBox> p_DectedPicBoxs
        {
            get { return m_DectedPicBoxs; }
            set { m_DectedPicBoxs = (Dictionary<String, PictureBox>)value; }
        }
        public String p_ImgFileName
        {
            get { return m_ImgFileName; }
            set { m_ImgFileName = (String) value; }
        }
        public TaskClass p_TskObj
        {
            get { return m_TskObj; }
            set { m_TskObj = (TaskClass)value; }
        }

        /*
         * *constructor
         */
        public Img_PicBoxData()
        {
            m_PicBoxs = new Dictionary<String, PictureBox>();
            m_DectedPicBoxs = new Dictionary<String, PictureBox>();
            m_ImgFileName = null;
            m_TskObj = null;
        }

        public void Dispose()
        {
            Dispose(true);
            // Finalization이 수행되지 않도록 한다
            GC.SuppressFinalize(true);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            // 여러 번 dipose를 수행하지 않도록 한다.
            if (m_alreadyDisposed)
                return;

            if (isDisposing)
            {
                // 해야할 일: managed(GC가관리되는) 리소스를 해제한다.
                foreach (var pic in m_PicBoxs.Values)
                    pic.Dispose();

                foreach (var pic in m_DectedPicBoxs.Values)
                    pic.Dispose();

                m_PicBoxs.Clear();
                m_DectedPicBoxs.Clear();
                m_PicBoxs = null;
                m_DectedPicBoxs = null;
            }

            // 해야할일: unmanaged(GC가 관리되지않는) 리소스를 해제한다.
            // disposed 플래그를 설정한다.
            m_alreadyDisposed = false;
        }
    }

    class PictureBox : IDisposable
    {
        /*
         * *member var
         */
        private bool m_alreadyDisposed = false;
        private OutLineRectangleItem m_PicBoxObj;

        /*
         * *property
         */
        public OutLineRectangleItem p_PicBoxObj
        {
            get { return m_PicBoxObj; }
            set { m_PicBoxObj = (OutLineRectangleItem) value; }
        }
        public void Dispose()
        {
            Dispose(true);
            // Finalization이 수행되지 않도록 한다
            GC.SuppressFinalize(true);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            // 여러 번 dipose를 수행하지 않도록 한다.
            if (m_alreadyDisposed)
                return;

            if (isDisposing)
            {
                // 해야할 일: managed(GC가관리되는) 리소스를 해제한다.
                RectMarker rectMarker = (RectMarker) m_PicBoxObj.marker_ref;
                rectMarker.p_SizeConrects.Clear();
                //rectMarker.p_SizeConrects = null;
                m_PicBoxObj = null;
            }

            // 해야할일: unmanaged(GC가 관리되지않는) 리소스를 해제한다.
            // disposed 플래그를 설정한다.
            m_alreadyDisposed = false;
        }
        /*
         * *constructor
         */
        public PictureBox()
        {
            m_PicBoxObj = new OutLineRectangleItem();
        }
    }
}
