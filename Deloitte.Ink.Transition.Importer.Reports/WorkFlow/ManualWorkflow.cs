using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deloitte.Ink.Transition.Importer.Reports.Common;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using Microsoft.Extensions.Logging;

namespace Deloitte.Ink.Transition.Importer.Reports.WorkFlow
{
    internal class ManualWorkflow : Workflows, IManualWorkflow
    {
        public readonly IManualReportService _manualReportService;
        public ManualWorkflow(ITransitionCoreServices coreServices, ILogger<ManualWorkflow> logger, IManualReportService _manualRepService) : base(coreServices)
        {
            _manualReportService = _manualRepService;
        }


        public override DataTable GeneratePublicationReport(bool isZipped)
        {
            return _manualReportService.ProcessAllXmlFiles(LocationService.OutputDirectoryInfo.FullName, isZipped);
        }
        public override DataTable GenerateExcelReport(string inputValue)
        {
            throw new NotImplementedException();
        }

    }
}
