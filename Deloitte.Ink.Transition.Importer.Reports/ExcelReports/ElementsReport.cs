using Deloitte.Ink.Transition.Importer.Reports.Constants;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using System.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using Deloitte.Ink.Transition.Importer.Reports.Extensions;
using Deloitte.Ink.Transition.Importer.Reports.Common;


namespace Deloitte.Ink.Transition.Importer.Reports.ExcelReports
{

    public class ElementsReport : CoreService, IElementsReport
    {

        private readonly ITransitionCoreServices _coreServices;
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        public ILocationService LocationService => _coreServices.LocationService;

        public ITransitionContextService TransitionContextService => _coreServices.TransitionContextService;
        public ITransitionCoreServices CoreServices => _coreServices;

        public ILogger<ElementsReport> Logger { get; }
        DataTable dtxrefReport;

        public List<string> lstXRefType;

        public ElementsReport(ITransitionCoreServices coreServices, ILogger<ElementsReport> logger)
        {
            _coreServices = coreServices;

            Logger = logger;
            dtxrefReport = new DataTable();
            dtxrefReport.Columns.Add("FileName");
            dtxrefReport.Columns.Add("Element");
            lstXRefType = new List<string>() { "xref", "xref-document", "xref-external" , "xref-fn", "xref-DTL" , "xref-3rdParty" };
        }
        public DataTable processXrefReports(string _element)
        {
            string sourceChunkFile;


            DirectoryInfo directoryInfo = LocationService.InputDirectoryInfo;

            if (Directory.Exists(directoryInfo.FullName))
            {
                FileInfo[] Files = directoryInfo.GetFiles("*.zip");

                var zipFileFullName = LocationService.InputDirectoryInfo.FullName + "\\" + Files[0].Name;
                UnZipPackage(zipFileFullName, LocationService.OutputDirectoryInfo.FullName);

                Logger.LogInformation($"Unzipped the package : {zipFileFullName}");
                var xmlFiles = Directory.GetFiles(LocationService.OutputDirectoryInfo.FullName, "*.xml", SearchOption.AllDirectories).ToList();
                xmlFiles.ForEach(sourcefile =>
                {
                    string fileName = sourcefile.Split('\\').Last();
                    try
                    {
                        Console.WriteLine("Processing: " + fileName);
                        sourceChunkFile = sourcefile;
                        var sourceDocument = XDocumentFile.GetSourceDocument(sourceChunkFile);

                        string rootName = sourceDocument == null ? "" : sourceDocument.Root.Name.LocalName;
                        if (sourceDocument != null)
                        {
                            if (_element == "xref")
                            {
                                foreach (string xrefType in lstXRefType)
                                {
                                    var xElements = sourceDocument.Descendants(xrefType).ToList();
                                    ProcessElements(xElements, fileName);
                                }
                            }
                            else
                            {
                                var xElements = sourceDocument.Descendants(_element).ToList();
                                ProcessElements(xElements, fileName);

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("failed :" + fileName);
                        Console.WriteLine(ex);
                    }

                    // string csvFileName = ExportImpDataToExcel(dtxrefReport, LocationService.ExportDirectoryInfo.FullName, zipFileFullName);
                    //Delete the files

                });

            }
            return dtxrefReport;
        }
        public void ProcessElements(List<XElement> lstXElemnt, string fileName)
        {
            lstXElemnt.ForEach(xrefDocument =>
            {
                AddRowInTblReports(fileName, xrefDocument.ToString());
            });
        }

        public void AddRowInTblReports(string fileName, string element)
        {
            DataRow dtRow = dtxrefReport.NewRow();
            dtRow["FileName"] = fileName;
            dtRow["Element"] = element;
            dtxrefReport.Rows.Add(dtRow);
        }

    }
}
