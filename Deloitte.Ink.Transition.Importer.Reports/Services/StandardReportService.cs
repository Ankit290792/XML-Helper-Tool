
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
    internal class StandardReportService : CoreService, IStandardReportService
    {
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        string _SourceFileName = "";
        DataTable dtReport;
        private readonly ILogger<StandardReportService> _logger;
        Utils _utils;
        public int ReportCount { get; private set; }
        public StandardReportService(ILogger<StandardReportService> logger, IOptions<LocationConfig> locationConfig)
        {
            dtReport = new DataTable();
            dtReport.Columns.Add("SourceFileName");
            dtReport.Columns.Add("SourceELementSNo");
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

        public DataTable ProcessAllXmlFiles(string? extractPathDirectory, bool isZipped)
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
                        AddContentForStandardReport(string.Empty, string.Empty, string.Empty, fileName, "",  "Invalid xml file");
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

            ProcessContextFile(SourceDocument, SourceFileName, extractPathDirectory);
            var pnums = SourceDocument?.XPathSelectElements("//pnum");
            var punmtitles = SourceDocument?.XPathSelectElements("//pnum-title");
            string rootName = SourceDocument?.Root.Name.LocalName;
            if ((pnums != null && pnums.Any()) || (punmtitles != null && punmtitles.Any()))
            {

                var parasPnum = SourceDocument?.XPathSelectElements($"{rootName}/paras/para/pnum")?.ToList();
                if (parasPnum.Any())
                {
                    var paras = SourceDocument?.XPathSelectElements($"{rootName}/paras")?.Elements().ToList();
                    paras?.ForEach(para =>
                                {
                                    BuildTopicFile(para, extractPathDirectory, SourceFileName);
                                });

                }
                else
                {
                    ProcessPrefaceFile(SourceDocument, SourceFileName, extractPathDirectory);
                }
                var pgroups = SourceDocument?.Root?.XPathSelectElements("pgroup | guidance | policy").ToList();
                ProcessPgroups(SourceDocument, pgroups, extractPathDirectory, SourceFileName);


            }
            else
            {

                ProcessArticleTopics(SourceDocument, SourceFileName, extractPathDirectory);
            }


        }


        public void BuildTopicFile(XElement element, string extractPathDirectory, string SourceFileName)
        {
            var rootName = element?.Name.LocalName;
            var SourceBodyElement = GetSourceParaInnerXmlContent(element);
            var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();

            if (!string.IsNullOrEmpty(TopicID))
                TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

            var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
            var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
            string ConvertBodyElement;
            if (pgroupDocument != null)
            {
                ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
            }
            else
                ConvertBodyElement = "";
            AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "guidance");

        }



        public void ProcessPgroups(XDocument? SourceDocument, List<XElement>? pgroups, string extractPathDirectory, string SourceFileName)
        {
            pgroups?.ForEach(pgroup =>
            {
                try
                {
                    var rootName = pgroup?.Name.LocalName;
                    var SourceBodyElement = pgroup?.XPathSelectElement($"title")?.Value?.ToString().Trim();
                    var TopicID = pgroup?.Attribute("id")?.Value?.ToString().Trim();
                    if (!string.IsNullOrEmpty(TopicID))
                        TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

                    var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                    var pgroupDocument = (pgroupFileName == null) ? null : XDocumentFile.GetSourceDocument(pgroupFileName);
                    var ConvertBodyElement = pgroupDocument == null ? "" : GetConvertedContextFileContent(pgroupDocument);

                    AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "pgroup");
                    if (pgroup.Name.LocalName.Equals("pgroup"))
                    {
                        var nestedElements = pgroup?.Elements()?.ToList();

                        nestedElements?.ForEach(element =>
                        {
                            try
                            {
                                if (element.Name.LocalName.Equals("paras"))
                                {
                                    #region to add paras
                                    //var rootName = element?.Name.LocalName;
                                    //var SourceBodyElement = _utils.SanatizeTopicBody(element?.Value?.ToString().Trim()).Replace(System.Environment.NewLine, "");
                                    //var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();
                                    //if (!string.IsNullOrEmpty(TopicID))
                                    //    TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;
                                    //var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                                    //var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                                    //if (pgroupDocument != null)
                                    //    ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                                    //else
                                    //    ConvertBodyElement = "";

                                    //AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "paras");

                                    #endregion
                                    var paraELement = element?.Elements().ToList();

                                    paraELement?.ForEach(_para =>
                                    {
                                        try
                                        {
                                            var rootName = _para?.Name.LocalName;

                                            var SourceBodyElement = GetSourceParaInnerXmlContent(_para);// _utils.SanatizeTopicBody(_para?.Value?.ToString().Trim().Replace(".").Replace(System.Environment.NewLine, "");

                                            var TopicID = _para?.Attribute("id")?.Value?.ToString().Trim();
                                            if (!string.IsNullOrEmpty(TopicID))
                                                TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;
                                            var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();

                                            var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                                            if (pgroupDocument != null && rootName == "para")
                                                ConvertBodyElement = GetConvertedParaXmlContent(pgroupDocument);
                                            else if (pgroupDocument != null && rootName != "para")
                                                ConvertBodyElement = GetConvertedNonParaXmlContent(pgroupDocument);
                                            else
                                                ConvertBodyElement = "";



                                            AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "para|p|table");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("failed :" + _para?.Attribute("id")?.Value?.ToString().Trim());
                                            Console.WriteLine(ex);
                                        }
                                    });
                                }


                                else if (element.Name.LocalName.Equals("guidance"))
                                {
                                    var rootName = element?.Name.LocalName;
                                    var SourceBodyElement = GetSourceParaXmlContent(element);
                                    var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();

                                    if (!string.IsNullOrEmpty(TopicID))
                                        TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

                                    var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                                    var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);

                                    if (pgroupDocument != null)
                                    {
                                        ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                                    }
                                    else
                                        ConvertBodyElement = "";
                                    AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "guidance");


                                }
                                else if (element.Name.LocalName.Equals("policy"))
                                {
                                    var rootName = element?.Name.LocalName;
                                    var SourceBodyElement = GetSourceParaXmlContent(element);
                                    var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();

                                    if (!string.IsNullOrEmpty(TopicID))
                                        TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

                                    var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                                    var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                                    if (pgroupDocument != null)
                                        ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                                    else
                                        ConvertBodyElement = "";
                                    AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "policy");

                                }
                                else if (!element.Name.LocalName.Equals("pgroup") && !element.Name.LocalName.Equals("title"))
                                {
                                    var rootName = element?.Name.LocalName;
                                    var SourceBodyElement = element?.XPathSelectElement($"{rootName}/title")?.Value?.ToString().Trim();
                                    var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();
                                    if (!string.IsNullOrEmpty(TopicID))
                                        TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;
                                    var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                                    var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                                    if (pgroupDocument != null)
                                        ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                                    else
                                        ConvertBodyElement = "";
                                    AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "other Elements");


                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("failed :" + element?.Attribute("id")?.Value?.ToString().Trim());
                                Console.WriteLine(ex);
                            }

                        });

                        var children = pgroup?.XPathSelectElements("pgroup").ToList();
                        if (children.Any())
                            ProcessPgroups(SourceDocument, children, extractPathDirectory, SourceFileName);

                    }
                    else if (pgroup.Name.LocalName.Equals("policy"))
                    {
                        // rootName = pgroup?.Name.LocalName;
                        SourceBodyElement = pgroup?.XPathSelectElement($"{rootName}/title")?.Value?.ToString().Trim();
                        TopicID = pgroup?.Attribute("id")?.Value?.ToString().Trim();
                        if (!string.IsNullOrEmpty(TopicID))
                            TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;
                        pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                        pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                        if (pgroupDocument != null)
                        {
                            ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                        }
                        else
                            ConvertBodyElement = "";

                        AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "policy");


                    }

                    else if (pgroup.Name.LocalName.Equals("guidance"))
                    {
                        // rootName = pgroup?.Name.LocalName;
                        SourceBodyElement = pgroup?.XPathSelectElement($"{rootName}/title")?.Value?.ToString().Trim();
                        TopicID = pgroup?.Attribute("id")?.Value?.ToString().Trim();
                        if (!string.IsNullOrEmpty(TopicID))
                            TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;
                        pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                        pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                        if (pgroupDocument != null)
                            ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                        else
                        {
                            ConvertBodyElement = "";

                        }
                        AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "guidance");


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed :" + pgroup?.Attribute("id")?.Value?.ToString().Trim());
                    Console.WriteLine(ex);
                }
            });


        }



        public void ProcessContextFile(XDocument SourceDocument, string SourceFileName, string extractPathDirectory)
        {
            var rootName = SourceDocument?.Root?.Name.LocalName;
            var SourceBodyElement = SourceDocument?.XPathSelectElement($"{rootName}/title")?.Value?.ToString().Trim();
            var TopicID = SourceDocument?.XPathSelectElement($"{rootName}/title")?.Attribute("id")?.Value?.ToString().Trim();
            string cvtFolderName = Path.ChangeExtension(SourceFileName, null);
            var contextFileName = Directory.GetFiles(extractPathDirectory + "\\cvt\\" + cvtFolderName + "\\", "*_ctx.xml", SearchOption.AllDirectories).FirstOrDefault();
            var ContextDocument = XDocumentFile.GetSourceDocument(contextFileName);
            var ConvertBodyElement = GetConvertedContextFileContent(ContextDocument);

            AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, contextFileName?.Split('\\').Last(), "ctx");


        }

        public void ProcessPrefaceFile(XDocument? SourceDocument, string SourceFileName, string extractPathDirectory)
        {
            var rootName = SourceDocument?.Root?.Name.LocalName;
            var SourceBodyElement = SourceDocument?.XPathSelectElement($"{rootName}/title")?.Value?.ToString().Trim();
            SourceBodyElement = SourceBodyElement + SourceDocument?.XPathSelectElement($"{rootName}/paras")?.Value?.ToString().Trim();
            SourceBodyElement = SanatizeTopicBody(SourceBodyElement);
            var TopicID = SourceDocument?.XPathSelectElement($"{rootName}/title")?.Attribute("id")?.Value?.ToString().Trim();
            string cvtFolderName = Path.ChangeExtension(SourceFileName, null);
            var contextFileName = Directory.GetFiles(extractPathDirectory + "\\cvt\\" + cvtFolderName + "\\", "*_preface.xml", SearchOption.AllDirectories).FirstOrDefault();
            var ContextDocument = XDocumentFile.GetSourceDocument(contextFileName);
            var ConvertBodyElement = GetConvertedPrefaceContent(ContextDocument);
            AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, contextFileName?.Split('\\').Last(), "preface");



        }

        public void ProcessArticleTopics(XDocument? SourceDocument, string SourceFileName, string extractPathDirectory)
        {
            var rootName = SourceDocument?.Root?.Name.LocalName;
            var SourceBodyElement = "";
            var elements = SourceDocument?.XPathSelectElements($"{rootName}")?.Elements();
            elements?.ToList().ForEach(element =>
            {
                if (element.Name.LocalName != "dtl-general-prolog" || element.Name.LocalName != "dtl-topic-prolog" || element.Name.LocalName != "titlealts")
                {
                    SourceBodyElement = SourceBodyElement + element?.Value?.ToString().Trim();
                }

            }

            );
            SourceBodyElement = SanatizeTopicBody(SourceBodyElement);



            var TopicID = SourceDocument?.XPathSelectElement($"{rootName}/title")?.Attribute("id")?.Value?.ToString().Trim();
            if (!string.IsNullOrEmpty(TopicID))
                TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

            var articleFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
            var articelDocument = (articleFileName == null) ? null : XDocumentFile.GetSourceDocument(articleFileName);
            var ConvertBodyElement = articelDocument == null ? "" : GetConvertedArticleContent(articelDocument);

            // var contextFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", "*_preface.xml", SearchOption.AllDirectories).FirstOrDefault();

            AddContentForStandardReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, articleFileName?.Split('\\').Last(), "article");



        }
        public void AddContentForStandardReport(string? ConvertBodyElement, string? SourceBodyElement, string? TopicID, string? SourceFileName, string? CvtFileName, string FileType)
        {




            DataRow dtRow = dtReport.NewRow();
            dtRow["TopicID"] = TopicID;

            string compConvertBodyElement = regexWhiteSpace.Replace((ConvertBodyElement?.Replace(" ", string.Empty)) == null ? "" : ConvertBodyElement, "");
            string compSourceBodyElement = regexWhiteSpace.Replace((SourceBodyElement?.Replace(" ", string.Empty)) == null ? "" : SourceBodyElement, "");

            bool chkSourceAndCvtText = string.Compare(compSourceBodyElement, compConvertBodyElement, true) == 0 ? true : false;

            bool chkCellLengthExceeded = SourceBodyElement?.Length > 30004 ? true : false;
            dtRow["CellLengthExceeded"] = chkCellLengthExceeded == false ? "" : true;

            dtRow["ConvertBodyElement"] = chkCellLengthExceeded ? ConvertBodyElement?.Substring(0, 30000) + "...." : ConvertBodyElement;
            dtRow["SourceBodyElement"] = chkCellLengthExceeded ? SourceBodyElement?.Substring(0, 30000) + "...." : SourceBodyElement;



            dtRow["IsMatched"] = chkSourceAndCvtText;// string.Compare(SourceBodyElement, ConvertBodyElement, true) == 0 ? true : false;
            dtRow["SourceFileName"] = SourceFileName;

            dtRow["CvtFileName"] = CvtFileName;
            dtRow["FileType"] = FileType;

            if (_SourceFileName == SourceFileName)
                ReportCount++;
            else
            {
                ReportCount = 1;
                _SourceFileName = "";
            }

            if (ConvertBodyElement == "" && SourceBodyElement == "")
                dtRow["FileType"] = "Image|Binary";
            else
                dtRow["FileType"] = FileType;
            dtRow["SourceELementSNo"] = ReportCount;


            dtReport.Rows.Add(dtRow);
            if (string.IsNullOrEmpty(ConvertBodyElement))
                _logger.LogInformation($"CVT file not found: SourceFileName = {SourceFileName} , TopicID = {TopicID} , CvtFileName= {CvtFileName} , FileType {FileType}");

            _SourceFileName = SourceFileName;
            //StandardReport stdReport = new StandardReport();

            //stdReport.TopicID = TopicID;
            //stdReport.ConvertBodyElement = ConvertBodyElement;
            //stdReport.SourceBodyElement = SourceBodyElement;

            //stdReport.IsMatched = string.Compare(stdReport.SourceBodyElement, stdReport.ConvertBodyElement, true) == 0 ? true : false;
            //stdReport.SourceFileName = SourceFileName;
            //stdReport.CvtFileName = CvtFileName;
            //stdReport.FileType = FileType;

            //lstTopic.Add(stdReport);
        }


        #region


        //public virtual List<StandardReport> ProcessAllSRCXmlFiles(string extractPathDirectory)
        //{

        //    string sourceChunkFile;
        //    // DirectoryInfo directoryInfo = new DirectoryInfo(extractPathDirectory +"\\src");
        //    var xmlSourceFiles = Directory.GetFiles(extractPathDirectory + "\\src", "*.xml", SearchOption.AllDirectories).ToList();

        //    xmlSourceFiles.ForEach(sourcefile =>
        //    {
        //        string fileName = sourcefile.Split('\\').Last();
        //        try
        //        {
        //            sourceChunkFile = sourcefile;
        //            var sourceDocument = XDocumentFile.GetSourceDocument(sourceChunkFile);

        //            // string rootName = sourceDocument == null ? "" : sourceDocument.CreateNavigator\;
        //            if (sourceDocument != null)
        //            {
        //                var xmlContent = sourceDocument.XPathSelectElement("dtl-general");
        //                var dtlGeneralProlog = sourceDocument.XPathSelectElement("dtl-general-prolog");
        //                dtlGeneralProlog?.RemoveAll();
        //                if (xmlContent != null)
        //                    ProcessXmlContent(xmlContent, fileName, extractPathDirectory);

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("failed :" + fileName);
        //            Console.WriteLine(ex);
        //        }
        //    });
        //    return lstTopic;

        //}

        //public void ProcessXmlContent(XElement? topicElement, string filename, string extractPathDirectory)
        //{

        //    var xmlCvtFiles = Directory.GetFiles(extractPathDirectory + "\\cvt", "*.xml", SearchOption.AllDirectories).ToList();
        //    var pubmapFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", "*_pubmap.xml", SearchOption.AllDirectories).FirstOrDefault();
        //    string sectionfilename = "";
        //    StringBuilder sbconvertedContent = new StringBuilder();
        //    if (pubmapFileName != null)
        //    {
        //        var pubmapDocument = XDocumentFile.GetSourceDocument(pubmapFileName);
        //        var listTopicref = pubmapDocument.XPathSelectElements("//topicref").ToList();

        //        if (listTopicref != null && listTopicref.Count() > 0)
        //        {
        //            listTopicref.ForEach(topicref =>
        //            {

        //                try
        //                {
        //                    var subTopicref = topicref.XPathSelectElements("topicref").ToList();
        //                    if (subTopicref != null && subTopicref.Count() > 0)
        //                    {
        //                        subTopicref.ForEach(subtopicref =>
        //                        {

        //                            AddStandardReport(subtopicref, topicElement, filename, extractPathDirectory);
        //                        });

        //                    }
        //                    else
        //                    {
        //                        AddStandardReport(topicref, topicElement, filename, extractPathDirectory);
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("failed :" + sectionfilename);
        //                    Console.WriteLine(ex);
        //                }
        //            });
        //        }

        //    }
        //}



        //public void AddStandardReport(XElement? topicref, XElement? topicElement, string filename, string extractPathDirectory)
        //{
        //    StandardReport stdReport;
        //    stdReport = new StandardReport();
        //    try
        //    {
        //        Dictionary<string, string> dictBody = GetCvtTopicContent(topicref, extractPathDirectory, filename);
        //        stdReport.TopicID = "";
        //        stdReport.ConvertBodyElement = dictBody.Values.ToString();
        //        stdReport.SourceBodyElement = GetSourceXmlContent(topicElement, topicref, dictBody.Keys.ToString());

        //        stdReport.IsMatched = string.Compare(stdReport.SourceBodyElement, stdReport.ConvertBodyElement, true) == 0 ? true : false;
        //        stdReport.SourceFileName = "";
        //        stdReport.CvtFileName = "";
        //        stdReport.FileType = "";
        //        lstTopic.Add(stdReport);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("failed :" + filename);
        //        Console.WriteLine(ex);
        //    }

        //}

        //public virtual string GetContentFromConvertedFolder(string extractPathDirectory, string filename)
        //{
        //    string sectionfilename = "";
        //    StringBuilder sbconvertedContent = new StringBuilder();
        //    // DirectoryInfo directoryInfo = new DirectoryInfo(extractPathDirectory + "\\src");
        //    var xmlCvtFiles = Directory.GetFiles(extractPathDirectory + "\\cvt", "*.xml", SearchOption.AllDirectories).ToList();
        //    var pubmapFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", "*_pubmap.xml", SearchOption.AllDirectories).FirstOrDefault();

        //    if (pubmapFileName != null)
        //    {


        //        var pubmapDocument = XDocumentFile.GetSourceDocument(pubmapFileName);
        //        var listTopicref = pubmapDocument.XPathSelectElements("topicref").ToList();

        //        if (listTopicref != null && listTopicref.Count() > 0)
        //        {
        //            listTopicref.ForEach(topicref =>
        //            {

        //                try
        //                {
        //                    string topicrefHref = topicref.Value;
        //                    sectionfilename = extractPathDirectory + "\\cvt" + "\\" + filename + topicrefHref + ".xml";
        //                    var topicContent = XDocumentFile.GetSourceDocument(sectionfilename);
        //                    if (topicContent != null)
        //                    {
        //                        var xmlContent = topicContent.XPathSelectElement("body");
        //                        if (xmlContent != null)
        //                        {
        //                            if (topicref.Name.LocalName.ToString().Contains("_preface"))
        //                            {
        //                                sbconvertedContent.Append(GetConvertedPrefaceContent(xmlContent));
        //                            }
        //                            else if (topicref.Name.LocalName.ToString().Contains("ctx"))
        //                            {

        //                            }
        //                            else
        //                                sbconvertedContent.Append(GetConvertedXmlContent(xmlContent));
        //                        }


        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("failed :" + sectionfilename);
        //                    Console.WriteLine(ex);
        //                }
        //            });
        //        }

        //    }
        //    return sbconvertedContent.ToString();
        //}
        //public Dictionary<string, string> GetCvtTopicContent(XElement? topicref, string extractPathDirectory, string filename)
        //{
        //    Dictionary<string, string> dictTopic = new Dictionary<string, string>();
        //    try
        //    {
        //        string topicBody = "";
        //        var topicrefHref = topicref?.Value;
        //        string sectionfilename = extractPathDirectory + "\\cvt" + "\\" + filename + topicrefHref + ".xml";
        //        var topicContent = XDocumentFile.GetSourceDocument(sectionfilename);
        //        if (topicContent != null)
        //        {
        //            var xmlContent = topicContent.XPathSelectElement("body");
        //            var topicId = topicContent.XPathSelectElement("title")?.Attribute("id")?.Value;
        //            if (xmlContent != null && topicId != null)
        //            {
        //                topicBody = GetConvertedXmlContent(xmlContent);
        //                dictTopic.Add(key: topicId, topicBody);
        //            }


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("failed :" + filename);
        //        Console.WriteLine(ex);
        //    }
        //    return dictTopic;

        //}

        #endregion
    }
}
