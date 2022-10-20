
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
    internal class ManualReportService : CoreService, IManualReportService
    {
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        string _SourceFileName = "";
        DataTable dtReport;
        private readonly ILogger<ManualReportService> _logger;
        Utils _utils;
        public int ReportCount { get; private set; }
        public ManualReportService(ILogger<ManualReportService> logger, IOptions<LocationConfig> locationConfig)
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

                    if ((sourceDocument?.Root?.Name == "dtl-topic" || sourceDocument?.Root?.Name == "dtl-general" ) && sourceDocument != null)
                        ProcessXmlSrcAndConvertContent(sourceDocument, fileName, extractPathDirectory);
                    else
                        AddContentForManualReport(string.Empty, string.Empty, string.Empty, fileName, "", "Invalid xml file");

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

            ProcessPrefaceFile(SourceDocument, SourceFileName, extractPathDirectory);
            var pgroups = SourceDocument?.Root?.XPathSelectElements("pgroup | guidance | policy").ToList();
            ProcessPgroups(SourceDocument, pgroups, extractPathDirectory, SourceFileName);

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

                    AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "pgroup");
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



                                            AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "para|p|table");
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
                                    var SourceBodyElement = GetSourceManualInnerXmlContent(element);
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
                                    AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "guidance");


                                }
                                else if (element.Name.LocalName.Equals("policy"))
                                {
                                    var rootName = element?.Name.LocalName;
                                    var SourceBodyElement = GetSourceManualInnerXmlContent(element);
                                    var TopicID = element?.Attribute("id")?.Value?.ToString().Trim();

                                    if (!string.IsNullOrEmpty(TopicID))
                                        TopicID = TopicID.Length > 50 ? TopicID.Substring(0, 50) : TopicID;

                                    var pgroupFileName = Directory.GetFiles(extractPathDirectory + "\\cvt", TopicID + "*.xml", SearchOption.AllDirectories).FirstOrDefault();
                                    var pgroupDocument = XDocumentFile.GetSourceDocument(pgroupFileName);
                                    if (pgroupDocument != null)
                                        ConvertBodyElement = GetConvertedXmlContent(pgroupDocument);
                                    else
                                        ConvertBodyElement = "";
                                    AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "policy");

                                }
                                else if (element.Name.LocalName.Equals("paras"))
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
                                    AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "policy");

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
                                    AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "other Elements");


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

                        AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "policy");


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
                        AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, pgroupFileName?.Split('\\').Last(), "guidance");


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed :" + pgroup?.Attribute("id")?.Value?.ToString().Trim());
                    Console.WriteLine(ex);
                }
            });


        }


        public void AddContentForManualReport(string? ConvertBodyElement, string? SourceBodyElement, string? TopicID, string? SourceFileName, string? CvtFileName, string FileType)
        {




            DataRow dtRow = dtReport.NewRow();
            dtRow["TopicID"] = TopicID;
            dtRow["ConvertBodyElement"] = ConvertBodyElement;
            dtRow["SourceBodyElement"] = SourceBodyElement;


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

            AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, contextFileName?.Split('\\').Last(), "ctx");


        }

        public void ProcessPrefaceFile(XDocument SourceDocument, string SourceFileName, string extractPathDirectory)
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
            AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, contextFileName?.Split('\\').Last(), "preface");



        }

        public void ProcessArticleTopics(XDocument SourceDocument, string SourceFileName, string extractPathDirectory)
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

            AddContentForManualReport(ConvertBodyElement, SourceBodyElement, TopicID, SourceFileName, articleFileName?.Split('\\').Last(), "article");



        }
    }
}
