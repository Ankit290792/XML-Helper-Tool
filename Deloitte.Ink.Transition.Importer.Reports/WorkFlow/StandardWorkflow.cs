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
    internal class StandardWorkflow : Workflows, IStandardWorkflow
    {
        public readonly IStandardReportService _standardReportService;
        public StandardWorkflow(ITransitionCoreServices coreServices, ILogger<StandardWorkflow> logger,     IStandardReportService _standardRepService ) : base(coreServices)
        {
            _standardReportService = _standardRepService;
        }


        public override DataTable GeneratePublicationReport(bool isZipped)
        {
           return _standardReportService.ProcessAllXmlFiles(LocationService.OutputDirectoryInfo.FullName, isZipped);
        }

        public override DataTable GenerateExcelReport(string inputValue)
        {
            throw new NotImplementedException();
        }

    }
}
