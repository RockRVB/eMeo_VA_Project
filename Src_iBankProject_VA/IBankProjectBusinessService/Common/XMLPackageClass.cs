using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace VTMModelLibrary
{
    public class XMLPackageClass
    {
        static Dictionary<string, XmlDocument> xmlDocuments = null;
        
        /// <summary>
        /// XML 对象字典
        /// </summary>
        public static Dictionary<string, XmlDocument> XmlDocuments
        {
            get
            {
                if (xmlDocuments == null)
                {
                    xmlDocuments = new Dictionary<string, XmlDocument>();
                    //string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\Project\XMLFileNameConfig.xml";

                    foreach (var item in FileNames)
                    {
                        string fileNameItem = AppDomain.CurrentDomain.BaseDirectory + @item.Value;
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(fileNameItem);
                        xmlDocuments.Add(item.Key, xdoc);
                    }
                }

                return xmlDocuments;
            }
        }

        static Dictionary<string, string> fileNames = null;

        /// <summary>
        /// XML文件路径字典
        /// </summary>
        public static Dictionary<string, string> FileNames
        {
            get
            {
                if (fileNames == null)
                {
                    string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Config\Project\XMLFileNameConfig.xml";
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(fileName);
                    fileNames = GetValueDic(xmldoc, "XMLFileNameConfig.FileName");

                    //fileNames = new Dictionary<string, string>();
                    //fileNames.Add("Project", @"Config\Project\Project.xml");
                }
                return fileNames;
            }
        }

        /// <summary>
        /// 获取子节点项的Key与Value
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <param name="pointPath"></param>
        /// <returns></returns>
        static Dictionary<string, string> GetValueDic(XmlDocument xmldoc, string pointPath)
        {
            Dictionary<string, string> attValueList = new Dictionary<string, string>();

            if (xmldoc != null)
            {
                try
                {
                    string point = pointPath.Substring(pointPath.IndexOf('.') + 1);

                    if (!string.IsNullOrEmpty(point))
                    {
                        XmlNode rootNode = xmldoc.SelectSingleNode("Root");
                        XmlNode resultNode = rootNode.SelectSingleNode(point);
                        if (resultNode != null)
                        {
                            foreach (XmlNode item in resultNode.ChildNodes)
                            {
                                if (item.Attributes != null)
                                {
                                    string keyName = item.Attributes["key"].InnerText;

                                    string attValue = item.Attributes["value"].Value;
                                    attValueList.Add(keyName, attValue);
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
            }
            return attValueList;
        }

        /// <summary>
        /// 获取XML节点值
        /// </summary>
        /// <param name="pointPath">节点路径：文件名.节点.ItemKey名字, 
        /// 例：MainConfig.PrintReceiveMsg.[IsSupportFunction]
        /// MainConfig：文件名, PrintReceiveMsg：节点名,  [IsSupportFunction]：ItemKey名
        /// </param>
        /// <returns>XML节点key对应的value值</returns>
        public static string GetXmlAttValue(string pointPath)
        {
            string attValue = string.Empty;
            try
            {
                string[] points = pointPath.Split('.');
                string fileName = string.Empty;
                if (points.Length > 0)
                {
                    fileName = points[0];
                }
                Dictionary<string, XmlDocument> xmlDocs = XmlDocuments;

                if (xmlDocs.Count > 0)
                {
                    XmlDocument xmldoc = xmlDocs[fileName];
                    string point = pointPath.Substring(pointPath.IndexOf('.') + 1);

                    point = point.Substring(0, point.IndexOf('[') - 1);
                    XmlNode rootNode = xmldoc.SelectSingleNode("Root");
                    XmlNode resultNode = rootNode.SelectSingleNode(point);

                    string attName = pointPath.Substring(pointPath.IndexOf("[") + 1);
                    attName = attName.Substring(0, attName.Length - 1);

                    if (resultNode != null)
                    {
                        foreach (XmlNode item in resultNode.ChildNodes)
                        {
                            if (item.Attributes != null)
                            {
                                string keyName = item.Attributes["key"].InnerText;
                                string keyName2 = item.Attributes["key"].Value;

                                if (keyName == attName)
                                {
                                    attValue = item.Attributes["value"].Value;
                                    return attValue;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return attValue;
        }

        /// <summary>
        /// 设置XML节点值
        /// </summary>
        /// <param name="pointPath">节点路径：文件名.节点.ItemKey名字, 
        /// 例：MainConfig.PrintReceiveMsg.[IsSupportFunction]
        /// MainConfig：文件名, PrintReceiveMsg：节点名,  [IsSupportFunction]：ItemKey名
        /// </param>
        /// <param name="value">要更新的值</param>
        /// <returns>true:设置成功， false：设置失败</returns>
        public static bool SetXmlAttValue(string pointPath,string value)
        {
            string attValue = string.Empty;
            try
            {
                string[] points = pointPath.Split('.');
                string fileName = string.Empty;
                if (points.Length > 0)
                {
                    fileName = points[0];
                }
                Dictionary<string, XmlDocument> xmlDocs = XmlDocuments;

                if (xmlDocs.Count > 0)
                {
                    XmlDocument xmldoc = xmlDocs[fileName];
                    string point = pointPath.Substring(pointPath.IndexOf('.') + 1);

                    point = point.Substring(0, point.IndexOf('[') - 1);
                    XmlNode rootNode = xmldoc.SelectSingleNode("Root");
                    XmlNode resultNode = rootNode.SelectSingleNode(point);

                    string attName = pointPath.Substring(pointPath.IndexOf("[") + 1);
                    attName = attName.Substring(0, attName.Length - 1);

                    if (resultNode != null)
                    {
                        foreach (XmlNode item in resultNode.ChildNodes)
                        {
                            if (item.Attributes != null)
                            {
                                string keyName = item.Attributes["key"].Value;//.InnerText ;
                                //string keyName2 = item.Attributes["key"].Value;

                                if (keyName == attName)
                                {
                                    //item.Attributes["value"].Value = value;
                                    item.Attributes["value"].Value=value;//.InnerText = value;                                    
                                    break;
                                }
                            }
                        }
                    }

                    xmldoc.Save(FileNames[fileName]);
                    return true;
                }
            }
            catch
            {

            }
            //return attValue;

            return false;
        }

        /// <summary>
        /// 获取XML子节点字典
        /// </summary>
        /// <param name="pointPath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetXmlAttValueDic(string pointPath)
        {
            Dictionary<string, string> attValueList = new Dictionary<string, string>();
            try
            {
                string[] points = pointPath.Split('.');
                string fileName = string.Empty;
                if (points.Length > 0)
                {
                    fileName = points[0];
                }
                Dictionary<string, XmlDocument> xmlDocs = XmlDocuments;
                if (xmlDocs.Count > 0)
                {
                    XmlDocument xmldoc = xmlDocs[fileName];

                    attValueList = GetValueDic(xmldoc, pointPath);
                }
            }
            catch
            {

            }
            return attValueList;
        }

    }
}
