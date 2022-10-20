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
    internal class RoadmapsWorkflow : Workflows, IPracticalGuideWorkflow
    {
        public readonly IPracticalGuideService _practGuideService;
        public readonly ITransitionCoreServices _coreService;
        public RoadmapsWorkflow(ITransitionCoreServices coreServices, ILogger<IPracticalGuideWorkflow> logger, IPracticalGuideService _manualRepService) : base(coreServices)
        {
            _practGuideService = _manualRepService;
            _coreService = coreServices;
        }

        public override DataTable GeneratePublicationReport(bool isZipped)
        {
            return _practGuideService.ProcessAllXmlFiles(_coreService, LocationService.OutputDirectoryInfo.FullName, isZipped);
        }
        public override DataTable GenerateExcelReport(string inputValue)
        {
            throw new NotImplementedException();
        }

    }
}
