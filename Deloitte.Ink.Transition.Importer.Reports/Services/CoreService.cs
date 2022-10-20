
using Deloitte.Ink.Transition.Importer.Reports.Extensions;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Data;
using System.Reflection;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    public class CoreService : Utils
    {
        public static readonly Regex regexWhiteSpace = new Regex(@"\s+");

        public DirectoryInfo? SourceDirectoryInfo { get; private set; }

        TopicReport? topicRow;

        DataTable dtReport = new DataTable();
        Utils _utils;
        string articleBody = string.Empty;
        internal CoreService()
        {

            dtReport = new DataTable();
            dtReport.Columns.Add("TopicID");
            dtReport.Columns.Add("Body");


            _utils = new Utils();
        }
        public virtual void UnZipPackage(string importerPackageLocation, string extractPathDirectory)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding(1252);
            if (!System.IO.Directory.Exists(extractPathDirectory))
                System.IO.Directory.CreateDirectory(extractPathDirectory);
            ZipFile.ExtractToDirectory(importerPackageLocation, extractPathDirectory);

        }



        public virtual string GetTopicBody(XElement topicElement)
        {
            string body = string.Empty;
            if (topicElement != null)
            {

                var elementBody = topicElement.XPathSelectElement("body")?.Value ?? string.Empty;
                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetPublicationTopicBody(XElement topicElement)
        {
            string body = string.Empty;
            if (topicElement != null)
            {

                var elementBody = topicElement.XPathSelectElement("body")?.Value ?? string.Empty;
                body = _utils.SanatizePublicationTopic(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }

        public virtual string GetSourceXmlContent(XElement? xmlElement, XElement? topicref, string topicID)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                var elementBody = xmlElement.Attribute(topicID)?.Value ?? string.Empty;
                // var elementBody = xmlElement?.Value ?? string.Empty;
                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }

        public virtual string GetConvertedPrefaceContent(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                var title = xmlElement.XPathSelectElement("topic/title")?.Value.Replace(".", "") ?? string.Empty;


                var elementBody = xmlElement.XPathSelectElement("topic/body")?.Value ?? string.Empty;

                elementBody = title + elementBody;
                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetConvertedXmlContent(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                var title = xmlElement.XPathSelectElement("topic/title")?.Value.Replace(".", "") ?? string.Empty;
                var elementBody = xmlElement.XPathSelectElement("topic/body")?.Value ?? string.Empty;

                elementBody = title + elementBody;
                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetConvertedParaXmlContent(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                var title = xmlElement.XPathSelectElement("topic/title")?.Value?.Replace(".", "") ?? string.Empty;
                var elementBody = xmlElement.XPathSelectElement("topic/body")?.Value ?? string.Empty;

                elementBody = title + elementBody;
                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }

        public virtual string GetSourceParaXmlContent(XElement? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                bool isPnum = false;
                //var title = xmlElement.XPathSelectElement("topic/title")?.Value ?? string.Empty;
                var innerParaElements = xmlElement.Elements().ToList();
                if (innerParaElements != null && innerParaElements.Count > 0)
                {

                    innerParaElements.ForEach(e =>
                    {
                        if (e.Name.LocalName == "title" || e.Name.LocalName == "pgroup" || e.Name == "paras" || e.Name == "para")
                            body = body + e?.Value?.Trim();
                    });
                }
                else
                    body = body + xmlElement?.Value?.Trim();
                body = _utils.SanatizePublicationTopic(body).Replace(System.Environment.NewLine, "");

            }

            return body;
        }

        public virtual string GetSourceParaInnerXmlContent(XElement? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                bool isPnum = false;
                //var title = xmlElement.XPathSelectElement("topic/title")?.Value ?? string.Empty;
                var innerParaElements = xmlElement.Elements().ToList();
                if (innerParaElements != null && innerParaElements.Count > 0)
                {

                    innerParaElements.ForEach(e =>
                    {
                        if (e.Name.LocalName == "pnum" || e.Name.LocalName == "pnum-title")
                            body = body + e?.Value.Replace(".", "")?.Trim();
                        else
                            body = body + e?.Value?.Trim();

                    });
                }
                else
                    body = body + xmlElement?.Value?.Trim();
                body = _utils.SanatizePublicationTopic(body).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetSourceManualInnerXmlContent(XElement? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                bool isPnum = false;
                //var title = xmlElement.XPathSelectElement("topic/title")?.Value ?? string.Empty;
                var innerParaElements = xmlElement.Elements().ToList();
                if (innerParaElements != null && innerParaElements.Count > 0)
                {

                    innerParaElements.ForEach(e =>
                    {
                        if (e.Name.LocalName == "pnum" || e.Name.LocalName == "pnum-title")
                            body = body + e?.Value.Replace(".", "")?.Trim();
                        else
                            body = body + e?.Value?.Trim();

                    });
                }
                else
                    body = body + xmlElement?.Value?.Trim();
                body = _utils.SanatizePublicationTopic(body).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetConvertedNonParaXmlContent(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                //var title = xmlElement.XPathSelectElement("topic/title")?.Value ?? string.Empty;
                var elementBody = xmlElement.XPathSelectElement("topic/body")?.Value ?? string.Empty;


                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }

        public virtual string GetConvertedXmlBody(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                //var title = xmlElement.XPathSelectElement("topic/title")?.Value ?? string.Empty;
                var elementBody = xmlElement.XPathSelectElement("topic/body")?.Value ?? string.Empty;


                body = _utils.SanatizeTopicBody(elementBody).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetConvertedContextFileContent(XDocument? xmlElement)
        {
            string body = string.Empty;
            if (xmlElement != null)
            {
                var title = xmlElement.XPathSelectElement("topic/title")?.Value.Replace(".", "") ?? string.Empty;


                body = _utils.SanatizeTopicBody(title).Replace(System.Environment.NewLine, "");

            }

            return body;
        }
        public virtual string GetConvertedArticleContent(XDocument? xmlElement)
        {
            string body = "";
            if (xmlElement != null)
            {
                articleBody = string.Empty;
                body = ProcessCvtTopicsAndSubtopics(xmlElement.XPathSelectElements("topic").ToList());
                body = _utils.SanatizeTopicBody(body).Replace(System.Environment.NewLine, "");


            }

            return body;
        }
        public string ProcessCvtTopicsAndSubtopics(List<XElement>? topicElement)
        {
            topicElement?.ForEach(tElement =>
            {

                articleBody = articleBody + tElement?.XPathSelectElement("title")?.Value?.ToString();
                articleBody = articleBody + tElement?.XPathSelectElement("body")?.Value?.ToString();

                var subTopics = tElement?.XPathSelectElements("topic").ToList();
                if (subTopics != null && subTopics.Count > 0)
                {
                    subTopics?.ForEach(topic =>
                    {
                        articleBody = articleBody + topic?.XPathSelectElement("title")?.Value?.ToString();
                        articleBody = articleBody + topic?.XPathSelectElement("body")?.Value?.ToString();
                        var innerTopics = topic?.XPathSelectElements("topic").ToList();
                        if (innerTopics != null && innerTopics.Count > 0)
                        {

                            ProcessCvtTopicsAndSubtopics(topic?.XPathSelectElements("topic").ToList());
                        }

                    });
                }
            });
            return articleBody;
        }

        public virtual void CleanUpDirectories(string? extractPathDirectory, string? importerPackageLocation)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path: extractPathDirectory == null ? "" : extractPathDirectory);
            directoryInfo?.Delete(true);

            // directoryInfo = new DirectoryInfo(importerPackageLocation);
            // directoryInfo?.Delete(true);

            SetUpDirectories();

        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i]?.GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public virtual void SetUpDirectories()
        {

            var rootDirectory = new DirectoryInfo("C:\\Temp\\ReportImporter");
            if (!rootDirectory.Exists)
                rootDirectory.Create();

            SourceDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpOutput"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpOutput")))
            {
                SourceDirectoryInfo.Create();
            }


            SourceDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpInput"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpInput")))
            {
                SourceDirectoryInfo.Create();
            }

            SourceDirectoryInfo = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "ImpExport"));
            if (!Directory.Exists(Path.Combine(rootDirectory.FullName, "ImpExport")))
            {
                SourceDirectoryInfo.Create();
            }
        }


    }
}
