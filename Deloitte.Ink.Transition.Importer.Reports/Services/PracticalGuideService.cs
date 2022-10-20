
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
using Aspose.Cells;
using System.Collections;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Deloitte.Ink.Transition.Importer.Reports.Interface;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    internal class PracticalGuideService : CoreService, IPracticalGuideService
    {
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        string _SourceFileName = "";
        DataTable dtReport;
        private readonly ILogger<ManualReportService> _logger;
        Utils _utils;
        StringBuilder sbTopicContent;
        public int ReportCount { get; private set; }
        public PracticalGuideService(ILogger<ManualReportService> logger, IOptions<LocationConfig> locationConfig)
        {
            dtReport = new DataTable();
            dtReport.Columns.Add("SourceFileName");
            dtReport.Columns.Add("CvtFileName");
            dtReport.Columns.Add("TopicID");
            dtReport.Columns.Add("SourceBodyElement");
            dtReport.Columns.Add("ConvertBodyElement");
            dtReport.Columns.Add("FileType");
            dtReport.Columns.Add("IsMatched");
            dtReport.Columns.Add("CellLengthExceeded");
            //  dtReport.Columns.Add("CvtFileCount");
            _utils = new Utils();

            _logger = logger;
        }

        public DataTable ProcessAllXmlFiles(ITransitionCoreServices coreServices, string? extractPathDirectory, bool isZipped)
        {

            string sourceChunkFile;
            // DirectoryInfo directoryInfo = new DirectoryInfo(extractPathDirectory +"\\src");
            var xmlSourceFiles = Directory.GetFiles(extractPathDirectory + "\\src", "*.xml", SearchOption.AllDirectories).ToList();
            if (xmlSourceFiles.Count == 0 && isZipped == true)
            {
                var getZipFile = Directory.GetFiles(extractPathDirectory + "\\src", "*.zip", SearchOption.AllDirectories).ToList();

                //Flattening zip files into folder directories - especially in case of published packages
                getZipFile.ForEach(zip =>
                {
                    string publicationWorkingDirectoyPath = zip.Split('\\').Last(); //Path.Combine(LocationService.WorkingDirectoryInfo.FullName, zip.Name.Replace(FileExtensionConstants.ZipFile, string.Empty));

                    // Directory.CreateDirectory(publicationWorkingDirectoyPath);
                    ZipFile.ExtractToDirectory(zip, extractPathDirectory + "\\src");

                    var encoding = Encoding.GetEncoding(1252);


                    // DirectoryInfo directoryInfo = new DirectoryInfo(extractPathDirectory +"\\src");
                    var xmlSourceFiles = Directory.GetFiles(extractPathDirectory + "\\src");
                    // zip.Delete();
                });
                xmlSourceFiles = Directory.GetFiles(extractPathDirectory + "\\src", "*.xml", SearchOption.AllDirectories).ToList();
            }

            ProcessContextFile(coreServices.TransitionContextService.ContentMetadata.PublicationTitle, "", extractPathDirectory);

            xmlSourceFiles.ForEach(sourcefile =>
            {
                string fileName = sourcefile.Split('\\').Last();
                try
                {
                    sourceChunkFile = sourcefile;
                    var sourceDocument = XDocumentFile.GetSourceDocument(sourceChunkFile);
                    Console.WriteLine("Processing :" + fileName);
                    // string rootName = sourceDocument == null ? "" : sourceDocument.CreateNavigator\;
                    if ((sourceDocument?.Root?.Name == "dtl-topic" || sourceDocument?.Root?.Name == "dtl-general" ) && sourceDocument != null)
                        ProcessXmlSrcAndConvertContent(sourceDocument, fileName, extractPathDirectory);
                    else
                        AddContentForPracticalGuideReport(string.Empty, string.Empty, string.Empty, fileName, string.Empty, "Invalid xml file");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed :" + fileName);
                    Console.WriteLine(ex);
                }
            });
            return dtReport;

        }
        public void ProcessXmlSrcAndConvertContent(XDocument SourceDocument, string SourceFileName, string extractPathDirectory)
        {
            BuildTopicFile(SourceDocument, extractPathDirectory, SourceFileName);

        }
        public void BuildTopicFile(XDocument? SourceDocument, string extractPathDirectory, string SourceFileName)
        {
            string cvtFileName = ""; string TopicID = "";

            sbTopicContent = new StringBuilder();
            var rootName = SourceDocument.Root.Name.LocalName;
            if (rootName == "dtl-topic" || rootName == "dtl-general")
            {

                var dtlTopic = SourceDocument?.XPathSelectElement($"dtl-topic | dtl-general");


                var SourceBodyElement = GetSourceParaXmlContent(dtlTopic);
                var TopicTitle = SourceDocument?.XPathSelectElement($"dtl-topic/title |  dtl-general/title")?.Value?.ToString().Trim();
                TopicTitle = RemoveNoiseWithTrimmingLeadingSpaces(TopicTitle);

                var cvtFilesInOutputFolder = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).ToList();

                foreach (var cvtFile in cvtFilesInOutputFolder)
                {

                    var cvtDocument = XDocumentFile.GetSourceDocument(cvtFile);
                    var topicElement = cvtDocument.XPathSelectElement($"topic");
                    var topicTitle = cvtDocument?.XPathSelectElement($"topic/title")?.Value;
                    if (topicTitle != null && topicTitle.ToString().ToLower().Contains(TopicTitle.ToLower()))
                    {
                        cvtFileName = cvtFile.Split('\\').Last();
                        TopicID = cvtDocument.XPathSelectElement($"topic")?.Attribute("id")?.Value;
                        ProcessTopicsAndSubtopics(topicElement, rootName);
                        AddContentForPracticalGuideReport(sbTopicContent.ToString(), SourceBodyElement, TopicID, SourceFileName, cvtFileName, "topic");
                        break;

                    }


                }
            }
        }



        public void ProcessTopicsAndSubtopics(XElement? topicElement, string rootName)
        {
            var Body = GetPublicationTopicBody(topicElement);
            sbTopicContent.Append(topicElement?.XPathSelectElement($"title").Value?.Trim());
            sbTopicContent.Append(Body);

            var subTopics = topicElement?.XPathSelectElements("topic").ToList();
            if (subTopics != null && subTopics.Count > 0)
            {
                subTopics?.ForEach(topic =>
                {
                    ProcessTopicsAndSubtopics(topic, rootName);
                });
            }
        }
        public static string RemoveNoiseWithTrimmingLeadingSpaces(string noiseyString)
        {
            //  remove tabs, linebreaks, carriage returns
            noiseyString = Regex.Replace(noiseyString, @"\t|\n|\r", " ", RegexOptions.Compiled);

            //  remove all non-ascii characters -> https://stackoverflow.com/questions/123336/how-can-you-strip-non-ascii-characters-from-a-string-in-c
            //noiseyString = Regex.Replace(noiseyString, @"[^\u0000-\u007F]+", " ", RegexOptions.Compiled);

            //  replace all white space of two or more characters with a single space
            noiseyString = Regex.Replace(noiseyString, "\\s+", " ", RegexOptions.Compiled);

            return noiseyString?.TrimStart();
        }


        public void AddContentForPracticalGuideReport(string? ConvertBodyElement, string? SourceBodyElement, string? TopicID, string? SourceFileName, string? CvtFileName, string FileType)
        {
            DataRow dtRow = dtReport.NewRow();
            dtRow["TopicID"] = TopicID;

            string compConvertBodyElement = regexWhiteSpace.Replace((ConvertBodyElement?.Replace(" ", string.Empty)) == null ? "" : ConvertBodyElement, "");
            string compSourceBodyElement = regexWhiteSpace.Replace((SourceBodyElement?.Replace(" ", string.Empty)) == null ? "" : SourceBodyElement, "");

            bool chkSourceAndCvtText = string.Compare(compSourceBodyElement, compConvertBodyElement, true) == 0 ? true : false;

            bool chkCellLengthExceeded = SourceBodyElement?.Length > 30000 ? true : false;
            dtRow["CellLengthExceeded"] = chkCellLengthExceeded == false ? "" : true;

            dtRow["ConvertBodyElement"] = chkCellLengthExceeded ? ConvertBodyElement?.Substring(0, 30000) + "...." : ConvertBodyElement;
            dtRow["SourceBodyElement"] = chkCellLengthExceeded ? SourceBodyElement?.Substring(0, 30000) + "...." : SourceBodyElement;



            dtRow["IsMatched"] = chkSourceAndCvtText;// string.Compare(SourceBodyElement, ConvertBodyElement, true) == 0 ? true : false;
            dtRow["SourceFileName"] = SourceFileName;

            dtRow["CvtFileName"] = CvtFileName;
            dtRow["FileType"] = FileType;

            if (ConvertBodyElement == "" && SourceBodyElement == "")
                dtRow["FileType"] = "Image|Binary";
            else
                dtRow["FileType"] = FileType;


            dtReport.Rows.Add(dtRow);
            if (string.IsNullOrEmpty(ConvertBodyElement))
                _logger.LogInformation($"CVT file not found: SourceFileName = {SourceFileName} , TopicID = {TopicID} , CvtFileName= {CvtFileName} , FileType {FileType}");



        }

        public void ProcessContextFile(string? pubTitle, string SourceFileName, string? extractPathDirectory)
        {
            string cvtFolderName = Path.ChangeExtension(SourceFileName, null);
            var contextFileName = Directory.GetFiles(extractPathDirectory + "\\cvt\\" + "\\", "*_ctx.xml", SearchOption.AllDirectories).FirstOrDefault();
            var ContextDocument = XDocumentFile.GetSourceDocument(contextFileName);
            var rootName = ContextDocument?.Root?.Name.LocalName;
            var SourceBodyElement = pubTitle;
            var TopicID = ContextDocument?.XPathSelectElement($"{rootName}/title")?.Attribute("id")?.Value?.ToString().Trim();


            var ConvertBodyElement = GetConvertedContextFileContent(ContextDocument);

            AddContentForPracticalGuideReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, contextFileName?.Split('\\').Last(), "ctx");


        }



    }
}
