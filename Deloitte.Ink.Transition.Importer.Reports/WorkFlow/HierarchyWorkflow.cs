using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deloitte.Ink.Transition.Importer.Reports.Common;
using Deloitte.Ink.Transition.Importer.Reports.ExcelReports;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using Microsoft.Extensions.Logging;

namespace Deloitte.Ink.Transition.Importer.Reports.WorkFlow
{
    public class HierarchyWorkflow : Workflows, IHierarchyReportWorkflow
    {
        public readonly IHierarchialReports hierarchyReportService;
        public HierarchyWorkflow(ITransitionCoreServices coreServices, ILogger<IElementsReportWorkflow> logger, IHierarchialReports _hierarchyReportService) : base(coreServices)
        {
            hierarchyReportService = _hierarchyReportService;
        }

        public override DataTable GenerateExcelReport(string inputValue)
        {
            throw new NotImplementedException();
        }

        public override DataTable GeneratePublicationReport(bool isZipped)
        {
            return hierarchyReportService.ProcessImpReports(isZipped);
        }
    }
}
