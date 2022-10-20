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
    internal class ElementsWorkflow : Workflows, IElementsReportWorkflow
    {
        public readonly IElementsReport _elementReportService;
        public ElementsWorkflow(ITransitionCoreServices coreServices, ILogger<IElementsReportWorkflow> logger, IElementsReport elementsRepService) : base(coreServices)
        {
            _elementReportService = elementsRepService;
        }

        public  override DataTable GenerateExcelReport(string inputValue)
        {
            return _elementReportService.processXrefReports(inputValue.Split(':')[1].ToString());
        }

        public override DataTable GeneratePublicationReport(bool isZipped)
        {
            throw new NotImplementedException();
        }
    }
}
