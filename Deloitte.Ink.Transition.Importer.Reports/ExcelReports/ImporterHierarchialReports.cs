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
    public class ImporterHierarchialReports : CoreService, IHierarchialReports
    {

        private readonly ITransitionCoreServices _coreServices;
        public DirectoryInfo? SourceDirectoryInfo { get; private set; }
        public ILocationService LocationService => _coreServices.LocationService;

        public ITransitionContextService TransitionContextService => _coreServices.TransitionContextService;
        public ITransitionCoreServices CoreServices => _coreServices;
        public IWorkflowResolver WorkflowResolver { get; set; }
        public ILogger<ImporterHierarchialReports> Logger { get; }
        DataTable dtReport;


        public ImporterHierarchialReports(ITransitionCoreServices coreServices, IWorkflowResolver workflowResolver, ILogger<ImporterHierarchialReports> logger)
        {
            _coreServices = coreServices;
            WorkflowResolver = workflowResolver;
            Logger = logger;
            dtReport = new DataTable();
            dtReport.Columns.Add("TopicID");
            dtReport.Columns.Add("Body");

        }
        public DataTable ProcessImpReports(bool isZipped)
        {
            DataTable hierarchyReport = new DataTable();
            if (!isZipped)
            {
                hierarchyReport = ProcessTopics(LocationService.OutputDirectoryInfo.FullName);
                Logger.LogInformation("Topics are processed");
            }
            else
            {
                DirectoryInfo directoryInfo = LocationService.InputDirectoryInfo;

                if (Directory.Exists(directoryInfo.FullName))
                {
                    FileInfo[] Files = directoryInfo.GetFiles("*.zip");
                    if (Files != null && Files.Count() > 0)
                    {
                        var zipFileFullName = LocationService.InputDirectoryInfo.FullName + "\\" + Files[0].Name;
                        if (zipFileFullName.ToUpper().Contains("IMP="))
                        {

                            if (isZipped)
                            {
                                UnZipPackage(zipFileFullName, LocationService.OutputDirectoryInfo.FullName);
                                Logger.LogInformation($"Unzipped the package : {zipFileFullName}");
                            }
                            //Process Topics
                            hierarchyReport = ProcessTopics(LocationService.OutputDirectoryInfo.FullName);
                            Logger.LogInformation("Topics are processed");

                           
                        }
                        else
                            Logger.LogInformation("Not a valid package, place an importer package");
                    }
                    else
                        Logger.LogInformation("No package found");
                }
                else
                {
                    Logger.LogInformation("Directory path not found.");
                }
            }
            return hierarchyReport;
        }

       
        public DataTable ProcessTopics(string extractPathDirectory)
        {

            string sourceChunkFile;
            DirectoryInfo directoryInfo = new DirectoryInfo(extractPathDirectory);
            var xmlFiles = Directory.GetFiles(extractPathDirectory, "*.xml", SearchOption.AllDirectories).ToList();

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
                        var topic = sourceDocument.XPathSelectElement("topic");
                        if (topic != null)
                            ProcessTopicsAndSubtopics(topic, rootName);
                        else if (topic == null && rootName.ToLower() == "map")
                        {
                            var topicId = sourceDocument.XPathSelectElement("map")?.Attribute("id")?.Value;
                            AddRowInTblReports(topicId, "map");
                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed :" + fileName);
                    Console.WriteLine(ex);
                }
            });
            return dtReport;

        }

        public void ProcessTopicsAndSubtopics(XElement? topicElement, string rootName)
        {

            var topicId = topicElement?.Attribute("id")?.Value ?? rootName;
            var Body = GetTopicBody(topicElement);

            AddRowInTblReports(topicId, Body);

            var subTopics = topicElement?.XPathSelectElements("topic").ToList();
            if (subTopics != null && subTopics.Count > 0)
            {
                subTopics?.ForEach(topic =>
                {
                    ProcessTopicsAndSubtopics(topic, rootName);
                });
            }
        }
        public void AddRowInTblReports(string TopicID, string Body)
        {
            DataRow dtRow = dtReport.NewRow();
            dtRow["TopicID"] = TopicID;
            dtRow["Body"] = Body;
            dtReport.Rows.Add(dtRow);
        }

       
    }
}
