using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using DeepObjectDector.sub.control.ImageListView;
using DeepObjectDector.sub.lib.err;
using System.Windows.Markup;

namespace DeepObjectDector.sub.doc
{
    public class ResultEventArgs : EventArgs
    {
        private ERR_RESULT m_NewArea;

        public ResultEventArgs(ERR_RESULT result)
        {
            m_NewArea = result;
        }
        public ERR_RESULT NewArea
        {
            get { return m_NewArea;}
        }
    }
    /*
     *  
     */

    public class SAVEPATHClass : DependencyObject, IXmlSerializable
    {
        /*
         * *event Deligate
         */
        static public event EventHandler<ResultEventArgs> SendErrCall;

        /*
         * * memberVar
         */
        private String m_LocalPath; //로컬 주소

        /*
         * * property
         */
        public String localPath
        {
            get { return m_LocalPath; }
            set { m_LocalPath = (String)value; }
        }

        /*
         * * constructor
         */
        public SAVEPATHClass()
        {
            m_LocalPath = Environment.CurrentDirectory;
        }

        /*
         * * method
         */
        public virtual XmlSchema GetSchema() { return null; }
        public virtual void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SAVEPATHClass")
                {
                    m_LocalPath = reader["LocalPath"];
                }
                reader.Read();
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public virtual void WriteXml(XmlWriter writer)
        {
            try
            {
                writer.WriteAttributeString("LocalPath", m_LocalPath);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        protected ERR_RESULT Fol_AddFolder(String prjName)
        {
            ERR_RESULT result = new ERR_RESULT();
            String totPath = null;
            try
            {
                //솔루션 폴더에 프로젝트 생성
                totPath += m_LocalPath + "/" + prjName;
                if( Directory.Exists(totPath) == false)
                    Directory.CreateDirectory(totPath);
                
                return result;
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
        }
        protected ERR_RESULT Fol_DelFolder(String prjName)
        {
            ERR_RESULT result = new ERR_RESULT();
            String totPath = null;
            try
            {
                totPath += m_LocalPath + "/" + prjName;

                DirectoryInfo folder = new DirectoryInfo(totPath);

                if (Directory.Exists(totPath))
                {
                    folder.Delete(true);//LDH9999 하위폴더가 있어도 강제 삭제 추후
                }
                
                return result;
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
        }
        protected void Fol_OnExcuteErrOut(ResultEventArgs e)
        {

            EventHandler<ResultEventArgs> handler = SendErrCall;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class WorkSpaceClass : SAVEPATHClass, IXmlSerializable
    {
        /*
         * *memberVar
         */
        private ObservableCollection<SolutionClass> m_Solutions;

       /*
        * *proeprty
        */
        public ObservableCollection<SolutionClass> p_Solutions
        {
            get { return m_Solutions; }
        }

        /*
         * *constructor
         */
        public WorkSpaceClass()
        {
            m_Solutions = new ObservableCollection<SolutionClass>();
            m_Solutions.CollectionChanged += Sol_Collection_Changed;
            inner_InitialRootFolder();
        }
        
        /*
         * * method
         */
        public override XmlSchema GetSchema() { return null; }
        public override void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "WorkSpaceClass")
                {
                    if (reader.ReadToDescendant("Solution"))
                    {
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Solution")
                        {
                            m_Solutions.Clear();
                            SolutionClass evt = new SolutionClass();
                            evt.ReadXml(reader);
                            m_Solutions.Add(evt);
                        }
                    }
                    reader.Read();
                }
            }
            catch(Exception err)
            {
                throw err;
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            try
            {
                foreach (SolutionClass evt in m_Solutions)
                {
                    writer.WriteStartElement("Solution");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        
        /// <summary>
        /// Solution 폴더를 생성. 
        /// </summary>
        /// <returns>0:Success </returns>
        private ERR_RESULT inner_InitialRootFolder()
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                String rootPath = base.localPath + "/solution";   
                //1. Soulution 폴더가 있는가?

                if (Directory.Exists(rootPath))
                {
                    SolutionClass sol = new SolutionClass();
                    m_Solutions.Add(sol);
                    return result;
                }
                else //1-1. 없다면 폴더 생성
                {
                    //DocSolution 객체선언
                    SolutionClass sol = new SolutionClass();
                    m_Solutions.Add(sol);
                }
                return result;
            }
            catch (_MainException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            finally
            { }
        }

        /*
         * *callback
         */
        public void Sol_Collection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            //FolderFileChange
            ERR_RESULT result = new ERR_RESULT();
            SolutionClass sol = null;
            int index = 0;
            String totPath = null;
            try
            {
                //Solution Folder 생성
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        sol = (SolutionClass)e.NewItems[index];
                        //makepath
                        totPath = sol.p_Name;
                        //addfolder
                        result = base.Fol_AddFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        //LDH9999 현재는 사용하지 않음 
                        //Soltution은 Root임.
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        //LDH9999 현재는 사용하지 않음 
                        //Soltution은 Root임.
                        break;
                    default:
                        throw new _DocException(-2);
                }
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }
    }
    public class SolutionClass : SAVEPATHClass, IXmlSerializable 
    {
        /*
         * *memberVar
         */
        private String m_Name;
        private ObservableCollection<ProjectClass> m_Projects;

        /*
         * *proeprty
         */
        public String p_Name
        {
            get { return m_Name; }
        }
        public ObservableCollection<ProjectClass> p_Projects
        {
            get { return m_Projects; }
        }
        
        /*
         * *constructor
         */
        public SolutionClass()
        {
            m_Name = "solution";
            m_Projects = new ObservableCollection<ProjectClass>();
            m_Projects.CollectionChanged += Prj_Collection_Changed;
        }

        /*
         * *method
         */
        public override XmlSchema GetSchema() { return null; }
        public override void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Solution")
                {
                    m_Name = reader["Name"];
                    if (reader.ReadToDescendant("Projects"))
                    {
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Projects")
                        {
                            ProjectClass evt = new ProjectClass();
                            evt.ReadXml(reader);
                            m_Projects.Add(evt);
                        }
                    }
                    reader.Read();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            try
            {
                writer.WriteAttributeString("Name", m_Name);

                foreach (ProjectClass evt in m_Projects)
                {
                    writer.WriteStartElement("Projects");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        
        /*
         * *callback
         */
        public void Prj_Collection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            //FolderFileChange
            ERR_RESULT result = new ERR_RESULT();
            int index = 0;
            try
            {
                if (sender == null)
                    throw new _DocException(-1);

                //ObservableCollection<ProjectClass> prjs = null;
                ProjectClass prj = null;
                String totPath = null;
                //Solution Folder 생성
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        prj = (ProjectClass) e.NewItems[0];
                        //makepath
                        totPath = p_Name + "/" + prj.p_Name;
                        //addfolder
                        result = base.Fol_AddFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        index = e.OldStartingIndex;
                        prj = (ProjectClass)e.OldItems[0];
                        //makepath
                        totPath = p_Name + "/" + prj.p_Name;
                        //addfolder
                        result = base.Fol_DelFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;
                    default:
                        throw new _DocException();
                }
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }
    }
    public class ProjectClass : SAVEPATHClass, IXmlSerializable 
    {
        /*
         * *memberVar
         */
        private String m_Name;
        private ObservableCollection<TaskClass> m_Tasks;
        private SolutionClass m_pareants;
        /*
         * *proeprty
         */
        public String p_Name
        {
            get { return m_Name; }
            set { m_Name = (String) value; }
        }
        public ObservableCollection<TaskClass> p_Tasks
        {
            get { return m_Tasks; }
        }
        public SolutionClass p_Pareants
        {
            get { return m_pareants; }
            set { m_pareants = (SolutionClass) value; }
        }

        /*
         * *constructor
         */
        public ProjectClass()
        {
            m_Tasks = new ObservableCollection<TaskClass>();
            m_Tasks.CollectionChanged += Task_Collection_Changed;
        }
        /*
         * * method
         */
        public override XmlSchema GetSchema() { return null; }
        public override void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Projects")
                {
                    m_Name = reader["Name"];
                    m_pareants = (SolutionClass)XamlReader.Parse(reader["Pareants"]);
                    if (reader.ReadToDescendant("Tasks"))
                    {
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Tasks")
                        {
                            TaskClass evt = new TaskClass();
                            evt.ReadXml(reader);
                            m_Tasks.Add(evt);
                        }
                    }
                    reader.Read();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", m_Name);
            writer.WriteAttributeString("Pareants", XamlWriter.Save(m_pareants));
            foreach (TaskClass evt in m_Tasks)
            {
                writer.WriteStartElement("Tasks");
                evt.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /*
         * *callback
         */
        public void Task_Collection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            //FolderFileChange
            ERR_RESULT result = new ERR_RESULT();
            int index = 0;
            try
            {
                if (sender == null)
                    throw new _DocException(-1);

                TaskClass tsk = null;
                String totPath = null;
                //Solution Folder 생성
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        tsk = (TaskClass)e.NewItems[0];
                        //makepath
                        totPath = p_Pareants.p_Name + "/" + p_Name + "/" + tsk.p_Name;
                        //addfolder
                        result = base.Fol_AddFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        index = e.OldStartingIndex;
                        tsk = (TaskClass)e.OldItems[0];
                        //makepath
                        totPath = p_Pareants.p_Name + "/" + p_Name + "/" + tsk.p_Name;
                        //addfolder
                        result = base.Fol_DelFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;
                    default:
                        throw new _DocException();
                }
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }
    }
    public class TaskClass : SAVEPATHClass, IXmlSerializable 
    {
        /*
         * *enum
         */
        public enum IMGTYPE
        {
            Unknown = -1,
            jpg = 0,
        }
        /*
         * *struct
         */
        public struct IMAGEFILEPATH : IXmlSerializable
        {
            public String originPath;
            public String targetPath;
            public String FileName;
            public String id;

            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader reader)
            {
                try
                {
                    if (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        originPath = reader["originPath"];
                        targetPath = reader["targetPath"];
                        FileName = reader["FileName"];
                        id = reader["id"];
                    }
                    reader.Read();
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
            public void WriteXml(XmlWriter writer)
            {
                try
                {
                    writer.WriteAttributeString("originPath", originPath);
                    writer.WriteAttributeString("targetPath", targetPath);
                    writer.WriteAttributeString("FileName", FileName);
                    writer.WriteAttributeString("id", id);
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
            //public IMGTYPE type; //LDH9999 추후
        }

        /*
         * *Dependency properdy
         */
        //public static readonly DependencyProperty ImageFileMapsProperty =
        //        DependencyProperty.Register("p_ImageFileMaps", typeof(Dictionary<int, IMAGEFILEPATH>), typeof(TaskClass),
        //                                    new FrameworkPropertyMetadata(new Dictionary<int, IMAGEFILEPATH>(), ImageFilePath_PropertyChanged));

        /*
         * *memberVar
         */
        private String m_Name;
        private ObservableCollection<ToolClass> m_Tools;
        private ProjectClass m_pareants;
        //Image_Path 리스트 추가
        private ObservableDictionary<UInt32, IMAGEFILEPATH> m_ImageFileMaps;
        private List<UInt32> m_ImageFileKeys;
        /*
         * *proeprty
         */
        public String p_Name
        {
            get { return m_Name; }
            set { m_Name = (String)value; }
        }
        public ObservableCollection<ToolClass> p_Tools
        {
            get { return m_Tools; }
        }
        public ProjectClass p_Pareants
        {
            get { return m_pareants; }
            set { m_pareants = (ProjectClass)value; }
        }
        public List<UInt32> p_ImageFileKeys
        {
            get { return m_ImageFileKeys; }
        }

        public ObservableDictionary<UInt32, IMAGEFILEPATH> p_ImageFileMaps
        {
            get
            {
                return m_ImageFileMaps;
            }
        }
        /*
        public ObservableDictionary<int, IMAGEFILEPATH> p_ImageFileMaps
        {
            get
            {
                return (ObservableDictionary<int, IMAGEFILEPATH>)GetValue(ImageFileMapsProperty);
            }
        }*/

        /*
         * *constructor
         */
        static TaskClass()
        {
        
        }
        public TaskClass()
        {
            m_Tools = new ObservableCollection<ToolClass>();
            p_Tools.CollectionChanged += Tool_Collection_Changed;
            m_ImageFileKeys = new List<UInt32>();
            m_ImageFileMaps = new ObservableDictionary<UInt32, IMAGEFILEPATH>();
            m_ImageFileMaps.CollectionChanged += Task_Collection_Change;
        }
        
        /*
         * * method
         */
        public void Tool_Collection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            //FolderFileChange
            ERR_RESULT result = new ERR_RESULT();
            int index = 0;
            try
            {
                if (sender == null)
                    throw new _DocException(-1);

                ToolClass tol = null;
                String totPath = null;
                //Solution Folder 생성
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        index = e.NewStartingIndex;
                        tol = (ToolClass)e.NewItems[0];
                        //makepath
                        totPath = m_pareants.p_Pareants.p_Name + "/" + m_pareants.p_Name + "/" + p_Name + "/" + tol.p_Name;
                        //addfolder
                        result = base.Fol_AddFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        index = e.OldStartingIndex;
                        tol = (ToolClass)e.OldItems[0];
                        //makepath
                        totPath = m_pareants.p_Pareants.p_Name + "/" + m_pareants.p_Name + "/" + p_Name + "/" + tol.p_Name;
                        //addfolder
                        result = base.Fol_DelFolder(totPath);
                        if (result.errCode != 0)
                            throw new _DocException(result.errCode);
                        break;
                    default:
                        throw new _DocException(-2);
                }
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }

        private void Task_Collection_Change(object sender, NotifyCollectionChangedEventArgs e)
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                //이미지 파일 패스 Add/Remove시
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        result = inner_cpyImgFile();
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        //LDH9999 이미지 파일제거시 tsk의 파일경로도 삭제 해야함
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        //LDH9999 교체 부분 수정 요함
                        break;
                    default:
                        throw new _DocException(-2);
                }

            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }
        private ERR_RESULT inner_cpyImgFile()
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                for (int index = 0; index < m_ImageFileKeys.Count; index++)
                {
                    UInt32 key = m_ImageFileKeys[index];
                    if (File.Exists(p_ImageFileMaps[key].targetPath) != true)
                        File.Copy(p_ImageFileMaps[key].originPath, p_ImageFileMaps[key].targetPath);
                }

                return result;
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                return result;
            }
            finally
            {
            }
        }
        public override XmlSchema GetSchema() { return null; }
        public override void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Tasks")
                {
                    m_Name = reader["Name"];
                    m_pareants = (ProjectClass)XamlReader.Parse(reader["Pareants"]);
                    
                    if (reader.ReadToDescendant("Tools"))
                    {
                        //m_Tools
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Tools")
                        {
                            ToolClass evt = new ToolClass();
                            evt.ReadXml(reader);
                            m_Tools.Add(evt);
                        }
                    }
                    //m_ImageFileKeys
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ImageFileKeys")
                    {
                        UInt32 key = UInt32.Parse(reader["Key"]);
                        m_ImageFileKeys.Add(key);
                        reader.Read();
                    }
                    
                    //m_ImageFileMaps
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ImageFileMaps")
                    {
                        UInt32 key = UInt32.Parse(reader["Key"]);
                        IMAGEFILEPATH evt = new IMAGEFILEPATH();
                        evt.ReadXml(reader);
                        m_ImageFileMaps.Add(key, evt);//<- 여기서 터짐
                    }
                    reader.Read();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        public override void WriteXml(XmlWriter writer)
        {
            try
            {
                writer.WriteAttributeString("Name", m_Name);
                writer.WriteAttributeString("Pareants", XamlWriter.Save(m_pareants));
                //m_Tools
                foreach (ToolClass evt in m_Tools)
                {
                    writer.WriteStartElement("Tools");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }
                //m_ImageFileKeys
                foreach (uint key in m_ImageFileKeys)
                {
                    writer.WriteStartElement("ImageFileKeys");
                    writer.WriteAttributeString("Key", key.ToString());
                    writer.WriteEndElement();
                }
                //m_ImageFileMaps
                foreach (var key in m_ImageFileMaps.Keys)
                {
                    writer.WriteStartElement("ImageFileMaps");
                    writer.WriteAttributeString("Key", key.ToString());
                    IMAGEFILEPATH evt = m_ImageFileMaps[key];
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }

    public class ToolClass : SAVEPATHClass, IXmlSerializable
    {
        /*
         * *memberVar
         */
        private String m_Name;
        private ObservableCollection<ImageItem> m_ImageItems;
        private TaskClass m_pareants;
        //LDH9999 PropertyParams struct 추후 구현
        //Objects 구현
        private List<ImageObject> m_TrainObjects;
        private List<ImageObject> m_ValidObjects;
        /*
         * *proeprty
         */
        public String p_Name
        {
            get { return m_Name; }
            set { m_Name = (String)value; }
        }
        public ObservableCollection<ImageItem> p_ImageItems
        {
            get { return m_ImageItems; }
            set { m_ImageItems = (ObservableCollection<ImageItem>)value; }
        }
        public TaskClass p_Pareants
        {
            get { return m_pareants; }
            set { m_pareants = (TaskClass)value; }
        }
        public List<ImageObject> trainObjects
        {
            get { return m_TrainObjects; }
            set { m_TrainObjects = (List<ImageObject>)value; }
        }
        public List<ImageObject> validObjects
        {
            get { return m_ValidObjects; }
            set { m_ValidObjects = (List<ImageObject>)value; }
        }

        /*
         * *constructor
         */
        public ToolClass()
        {
            m_ImageItems = new ObservableCollection<ImageItem>();
            p_ImageItems.CollectionChanged += Image_Collection_Changed;
            m_TrainObjects = new List<ImageObject>();
            m_ValidObjects = new List<ImageObject>();
        }
        

        /*
         * * method
         */
        public void Image_Collection_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            //This will get called when the collection is changed
            //FolderFileChange
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        break;

                    default:
                        throw new _DocException(-2);
                }
            }
            catch (_DocException err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
            catch (Exception err)
            {
                result = ErrProcess.SetErrResult(err);
                base.Fol_OnExcuteErrOut(new ResultEventArgs(result));
            }
        }

        public override XmlSchema GetSchema() { return null; }
        public override void ReadXml(XmlReader reader)
        {
            try
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Tools")
                {
                    m_Name = reader["Name"];
                    m_pareants = (TaskClass)XamlReader.Parse(reader["Pareants"]);

                    if (reader.ReadToDescendant("ImageItems"))
                    {
                        m_ImageItems.Clear();
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ImageItems")
                        {
                            ImageItem evt = new ImageItem();
                            evt.ReadXml(reader);
                            m_ImageItems.Add(evt);
                        }

                        m_TrainObjects.Clear();
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "TrainObjects")
                        {
                            ImageObject evt = new ImageObject();
                            evt.ReadXml(reader);
                            m_TrainObjects.Add(evt);
                        }

                        m_ValidObjects.Clear();
                        while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ValidObjects")
                        {
                            ImageObject evt = new ImageObject();
                            evt.ReadXml(reader);
                            m_ValidObjects.Add(evt);
                        }
                    }
                    reader.Read();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            try
            {
                writer.WriteAttributeString("Name", m_Name);
                writer.WriteAttributeString("Pareants", XamlWriter.Save(m_pareants));

                foreach (ImageItem evt in m_ImageItems)
                {
                    writer.WriteStartElement("ImageItems");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }

                foreach (ImageObject evt in m_TrainObjects)
                {
                    writer.WriteStartElement("TrainObjects");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }

                foreach (ImageObject evt in m_ValidObjects)
                {
                    writer.WriteStartElement("ValidObjects");
                    evt.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
    
    public class ImageObject : IXmlSerializable
    {
       /*
        * *struct
        */
        public struct IMAGEPOSITION
        {
            public String label;
            public Point LT_pos;//leftTop (x,y)
            public UInt32 width;
            public UInt32 height;

            public XmlSchema GetSchema() { return null; }
            public void ReadXml(XmlReader reader)
            {
                try
                {
                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "IMAGEFILEPATH")
                    {
                        label = reader["label"];
                        LT_pos = (Point)XamlReader.Parse(reader["LT_pos"]);
                        width = UInt32.Parse(reader["width"]);
                        height = UInt32.Parse(reader["height"]);
                    }
                    reader.Read();
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
            public void WriteXml(XmlWriter writer)
            {
                try
                {
                    writer.WriteAttributeString("label", label);
                    writer.WriteAttributeString("LT_pos", XamlWriter.Save(LT_pos));
                    writer.WriteAttributeString("width", String.Format("{0}", width));
                    writer.WriteAttributeString("height", String.Format("{0}", height));
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }
        
        /*
         * *memberVar
         */
        public String m_ImgRef;
        public List<IMAGEPOSITION> m_Objects;
        
        /*
         * * constructor
         */
        public ImageObject()
        {
            m_Objects = new List<IMAGEPOSITION>();
        }

        /*
         * * property
         */
        public String imgRef
        {
            get
            {
                return m_ImgRef;
            }
            set
            {
                m_ImgRef = value;
            }
        }
        public List<IMAGEPOSITION> objects
        {
            get
            {
                return m_Objects;
            }
        }

        public XmlSchema GetSchema() { return null; }
        public void ReadXml(XmlReader reader)
        {
            //LDH0000 ImageObject의 네임이 바뀜
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                m_ImgRef = reader["ImgRef"];
                String targetLocalName = reader.LocalName;
                if (reader.ReadToDescendant("Objects"))
                {
                    while (reader.MoveToContent() == XmlNodeType.Element)
                    {
                        IMAGEPOSITION evt = new IMAGEPOSITION();
                        evt.ReadXml(reader);
                        m_Objects.Add(evt);
                    }
                }
                reader.Read();
            }
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ImgRef", m_ImgRef);

            foreach (IMAGEPOSITION evt in m_Objects)
            {
                writer.WriteStartElement("Objects");
                evt.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}
