using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deloitte.Ink.Common;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Common;
using System.Data;

namespace Deloitte.Ink.Transition.Importer.Reports
{
    public interface IProcessor : IReport<Processor>
    {
        new Task ProcessAsync();
        public abstract void PublicationReport(bool isZipped);
        public abstract void PublicationReport(string userInput, bool isZipped, string reportType);

    }
    public class Processor : ContentReport<Processor>, IProcessor
    {
        private readonly ILogger<Processor> _logger;
        private readonly IWorkflowResolver _workflowResolver;
        private readonly ILocationService _locationService;
        private readonly ITransitionCoreServices _coreServices;

        public Processor(ITransitionCoreServices coreServices, ILogger<Processor> logger, ILocationService locationService, IWorkflowResolver workflowResolver) : base(coreServices, workflowResolver, logger)
        {
            _locationService = locationService;
            _logger = logger;
            _workflowResolver = workflowResolver;
            _coreServices = coreServices;
        }

        public override Task ProcessAsync()
        {
            try
            {

                //Inputs
                _locationService.SetUpDirectories();
                Console.WriteLine("\n \n -------------------------------------------------------------------------------------------------");
                Console.WriteLine("Compare Source & Convert file:  Press 0 to process unzipped files and 1 to process zip files");
                Console.WriteLine("Element Report : Press 2:<xElementName> to process xelements");
                Console.WriteLine("Hierarchy Report:  Press 3:0 to process unzipped files and 3:1 to process zip files");
                Console.WriteLine("\n \n -------------------------------------------------------------------------------------------------");
                var userInput = Console.ReadLine();
                ProcessReports(input: userInput.ToString());





                return Task.CompletedTask;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed :" + ex.Message.ToString());
                return Task.FromException(ex);
            }

        }

        public void ProcessReports(string input)
        {
            CleanUpDirectories(extractPathDirectory: LocationService.OutputDirectoryInfo.FullName, importerPackageLocation: LocationService.InputDirectoryInfo.FullName);

            DirectoryInfo directoryInfo = LocationService.InputDirectoryInfo;
            bool isZipped = input == "1" ? true : false;


            if (input.Contains("2"))
            {
                PublicationReport(input, isZipped, "2");
            }

            else if (input.Contains(":")  && input.Contains("3"))
            {
                bool _isZipped = input.Split(':')[1].ToString().Trim() == "1" ? true : false;
                PublicationReport(input.ToString(), _isZipped, "3");
            }

            else 
            {
                InitializeReport(isZipped);
                PublicationReport(isZipped);

            }
        }

        public void PublicationReport(string userInput, bool isZipped, string reportType)
        {
            DataTable dtReport = new DataTable();
            var workflow = WorkflowResolver.GetWorkflow(userInput?.Split(':')[0].ToString().Trim());
            _logger.LogInformation($"Workflow :{workflow}");
            if (reportType == "2")
            {
                dtReport = workflow.GenerateExcelReport(userInput);
            }
            else
            {
                dtReport = workflow.GeneratePublicationReport(isZipped);
            }

            _logger.LogInformation($"Table Content added");

            string csvFileName = ExportDataToExcel(dtReport, LocationService.ExportDirectoryInfo.FullName, "ElementExport_" + DateTime.Now.ToString("yyyyMMddHHmmss")); ;
            _logger.LogInformation($"CSV report generated at : {csvFileName}");

            CleanUpDirectories(LocationService.OutputDirectoryInfo.FullName, LocationService.InputDirectoryInfo.FullName);
            _logger.LogInformation("Cleaned Up Directories");

        }

        public void PublicationReport(bool isZipped)
        {
            var workflow = WorkflowResolver.GetWorkflow();
            _logger.LogInformation($"Workflow :{workflow}");

            DataTable dtReport = workflow.GeneratePublicationReport(isZipped);
            _logger.LogInformation($"Table Content added");

            string csvFileName = ExportDataToExcel(dtReport, LocationService.ExportDirectoryInfo.FullName, _coreServices.TransitionContextService.ContentMetadata?.PublicationName);
            _logger.LogInformation($"CSV report generated at : {csvFileName}");

            CleanUpDirectories(LocationService.OutputDirectoryInfo.FullName, LocationService.InputDirectoryInfo.FullName);
            _logger.LogInformation("Cleaned Up Directories");

        }
    }
}
