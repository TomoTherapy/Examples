using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DeepObjectDector.sub.lib.err;
using DeepObjectDector.sub.doc;

namespace DeepObjectDector.sub.lib.xml
{
    class DpXmlLib
    {
       /*
        * *memberval
        */
        private XDocument m_xDoc = null;  //doc
        private XElement m_Xroot = null;  //root
        private String m_CurrLocal = null;
        private WorkSpaceClass m_WorkSpaceObj;
       /*
        * *property
        */

       /*
        * *constructor
        */
        public DpXmlLib()
        {
            m_xDoc = new XDocument(new XDeclaration("1.0", "UTF-16", null));
            m_CurrLocal = Environment.CurrentDirectory + "\\" + "test.xml";
        }

       /*
        * *method
        */
        /// <summary>
        /// Root 노드 생성
        /// </summary>
        /// <param name="rootName">루트네임</param>
        /// <returns>ERR_RESULT</returns>
        public ERR_RESULT XML_ExportInit(WorkSpaceClass workspaceobj)
        {
            ERR_RESULT result = new ERR_RESULT();
            m_WorkSpaceObj = workspaceobj;
            
            try
            {
                if (m_xDoc == null)
                    m_xDoc = new XDocument();

                m_Xroot = new XElement(m_WorkSpaceObj.p_Solutions[0].p_Name);
                m_xDoc.Add(m_Xroot);

                return result;
            }
            catch (_XmlException err)
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
        /// <summary>
        /// XML Export 
        /// </summary>
        /// <returns></returns>
        public ERR_RESULT XML_ExportStart(WorkSpaceClass workSpaceObj)
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                if (workSpaceObj != null)
                    m_WorkSpaceObj = workSpaceObj;

                //Project 노드 생성
                result = inner_SelfCreateNode<ProjectClass>(m_WorkSpaceObj.p_Solutions[0].p_Projects);

                //파일생성
                m_xDoc.Save(m_CurrLocal);
                
                return result;
            }
            catch (_XmlException err)
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
        public ERR_RESULT XML_Free()
        {
            ERR_RESULT result = new ERR_RESULT();

            try
            {
                m_Xroot.RemoveAll();
                m_xDoc.RemoveNodes();
                
                m_Xroot = null;
                m_xDoc = null;
                return result;
            }
            catch (_XmlException err)
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

        /// <summary>
        /// Prj,Tsk,Tol 노드 생성 재귀함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs">Doc클래스의 List</param>
        /// <param name="parentsNode">부모 XMLNode</param>
        /// <returns>ERR_RESULT</returns>
        private ERR_RESULT inner_SelfCreateNode<T>(ObservableCollection<T> objs, XElement parentsNode = null)
        {
            ERR_RESULT result = new ERR_RESULT();
            const String nameSpaceStr = "DeepObjectDector.sub.doc";
            XElement node = null;

            try
            {
                foreach (T it in objs)
                {
                    switch (it.GetType().ToString())
                    {
                        case (nameSpaceStr + ".ProjectClass"):
                            node = new XElement("Project");
                            node.Add(new XAttribute("Name", (it as ProjectClass).p_Name));

                            if ((it as ProjectClass).p_Tasks.Count != 0)
                                result = inner_SelfCreateNode<TaskClass>((it as ProjectClass).p_Tasks, node);
                            
                            if (result.errCode != 0)
                                return result;

                            //root 노드 결합
                            m_Xroot.Add(node);
                            break;

                        case (nameSpaceStr + ".TaskClass"):
                            node = new XElement("Task");
                            node.Add(new XAttribute("Name", (it as TaskClass).p_Name));

                            result = inner_CreateImgFilesNode(node, (it as TaskClass));
                            if (result.errCode != 0)
                                return result;

                            if ((it as TaskClass).p_Tools.Count != 0)
                            {
                                result = inner_SelfCreateNode<ToolClass>((it as TaskClass).p_Tools, node);
                                if (result.errCode != 0)
                                    return result;
                            }
                            //Task 노드 결합
                            parentsNode.Add(node);
                            break;

                        case (nameSpaceStr + ".ToolClass"):
                            //PropertyParams
                            node = new XElement("Tool");
                            node.Add(new XAttribute("Name", (it as ToolClass).p_Name));

                            result = inner_CreatePropertyParams((it as ToolClass), node);
                            if (result.errCode != 0)
                                return result;

                            //train node
                            result = inner_CreateObjects((it as ToolClass).trainObjects, node, "train");
                            if (result.errCode != 0)
                                return result;

                            //valid node
                            result = inner_CreateObjects((it as ToolClass).validObjects, node, "valid");
                            if (result.errCode != 0)
                                return result;

                            //test node
                            result = inner_CreateTest((it as ToolClass).p_Pareants, node);
                            if (result.errCode != 0)
                                return result;

                            parentsNode.Add(node);
                            break;
                        default:
                            throw new _MainException(-2);
                    }
                }
                
                return result;
            }
            catch (_XmlException err)
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

        private ERR_RESULT inner_CreateImgFilesNode(XElement tskNode, TaskClass tskObj)
        {
            ERR_RESULT result = new ERR_RESULT();
            XElement imageNode = null;

            try
            {
                //tsk FilePath
                String tskPath = Environment.CurrentDirectory + "\\" + tskObj.p_Pareants.p_Pareants.p_Name;//solution
                tskPath += "\\" + tskObj.p_Pareants.p_Name; //Project
                tskPath += "\\" + tskObj.p_Name; //task
                
                int fileCount = tskObj.p_ImageFileMaps.Count();
                //1. ImageFile 노드 생성
                XElement imgFilesNode = new XElement("ImageFiles");
                XAttribute countAtt = new XAttribute("Count", fileCount);
                XAttribute pathAtt = new XAttribute("Path", tskPath);//
                
                imgFilesNode.Add(countAtt);
                imgFilesNode.Add(pathAtt);

                String idName = null;
                for (int index = 0; index < fileCount; index++)
                {
                    UInt32 key = tskObj.p_ImageFileKeys[index];
                    //2. Image 노드 생성
                    //Name att
                    idName = "IMG_" + ((UInt32) key).ToString();
                    imageNode = new XElement("Image", new XAttribute("Name", idName));

                    //LDH9999 type att 속성은 추후
                    imageNode.Value = tskObj.p_ImageFileMaps[key].FileName;
                    imgFilesNode.Add(imageNode);
                }

                //tsk노드에 추가
                tskNode.Add(imgFilesNode);
                return result;
            }
            catch (_XmlException err)
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
        private ERR_RESULT inner_CreatePropertyParams(ToolClass tolObj, XElement tolNode)
        {
            ERR_RESULT result = new ERR_RESULT();
            try
            {
                XElement paramNode = new XElement("PropertyParams");

                //LDH9999 추후 상수값으로 지정되어있음
                //LDH8282 xml 수정
                XElement classNameNode = new XElement("className", "ID_1");
                XElement epochNode = new XElement("epoch", 300);
                XElement batch_sizeNode = new XElement("batch_size", 10);
                XElement weight_decayNode = new XElement("weight_decay", 0.005);
                XElement learning_rateNode = new XElement("learning_rate", "0.0002,0.0001,0.00001");
                XElement global_stepNode = new XElement("global_step", "10000,20000");
                XElement continue_trainingNode = new XElement("continue_training", 0);

                paramNode.Add(classNameNode);
                paramNode.Add(epochNode);
                paramNode.Add(batch_sizeNode);
                paramNode.Add(weight_decayNode);
                paramNode.Add(learning_rateNode);
                paramNode.Add(global_stepNode);
                paramNode.Add(continue_trainingNode);

                tolNode.Add(paramNode);

                return result;
            }
            catch (_XmlException err)
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
        public ERR_RESULT inner_CreateObjects(List<ImageObject> objects, XElement node, String nodeName)
        {
            ERR_RESULT result = new ERR_RESULT();
            XElement tempNode = new XElement(nodeName);
            XElement objectsNode = null;
            try
            {
                foreach (ImageObject obj in objects)
                {
                    //픽처 박스 정보가 없으면 skip
                    if (obj.objects.Count == 0)
                        continue;

                    objectsNode = new XElement("objects", new XAttribute("ImgRef", obj.imgRef));
                    
                    //objects 추가
                    result = inner_AddImgObject(obj, objectsNode);
                    if (result.errCode != 0)
                        return result;

                    tempNode.Add(objectsNode);
                }

                node.Add(tempNode);
                return result;
            }
            catch (_XmlException err)
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
        private ERR_RESULT inner_AddImgObject(ImageObject obj, XElement objectsNode)
        {
            ERR_RESULT result = new ERR_RESULT();
            XElement objNode = null;
            XElement labelNode = null;
            XElement posNode = null;
            XElement sizeNode = null;
            try
            {
                foreach (ImageObject.IMAGEPOSITION pos in obj.objects)
                {
                    objNode = new XElement("object");
                    labelNode = new XElement("label", pos.label);

                    String posVal = pos.LT_pos.X.ToString() + "," + pos.LT_pos.Y.ToString();
                    posNode = new XElement("pos", posVal);

                    String sizeVal = pos.width + "," + pos.height;
                    sizeNode = new XElement("size", sizeVal);

                    objNode.Add(labelNode);
                    objNode.Add(posNode);
                    objNode.Add(sizeNode);

                    objectsNode.Add(objNode);
                }

                return result;
            }
            catch (_XmlException err)
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
        private ERR_RESULT inner_CreateTest(TaskClass tskObj, XElement objectsNode)
        {
            ERR_RESULT result = new ERR_RESULT();
            XElement testNode = null;
            XElement imgRefNode = null;
            try
            {
                testNode = new XElement("test");
                
                foreach (UInt32 key in tskObj.p_ImageFileKeys)
                {
                    TaskClass.IMAGEFILEPATH img = tskObj.p_ImageFileMaps[key];
                    imgRefNode = new XElement("ImgRef", new XAttribute("Ref", img.id));
                    testNode.Add(imgRefNode);
                }

                objectsNode.Add(testNode);
                return result;
            }
            catch (_XmlException err)
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
       /*
        * *callback
        */
    }
}
