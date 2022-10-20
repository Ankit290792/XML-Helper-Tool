using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Deloitte.Ink.Transition.Importer.Reports.Models;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Deloitte.Ink.Transition.Importer.Reports.Services
{
    public class WorkflowResolver : IWorkflowResolver
    {
        private readonly ITransitionCoreServices _coreServices;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkflowResolver> _logger;
        List<TopicReport> lstTopicData = new List<TopicReport>();
        public Dictionary<string, Type> _reportWorkflow;
        public WorkflowResolver(ITransitionCoreServices coreServices,
          IServiceProvider serviceProvider,
          ILogger<WorkflowResolver> logger)
        {
            _coreServices = coreServices;
            _serviceProvider = serviceProvider;
            _logger = logger;
            LoadMappingDetails();
        }
        public void LoadMappingDetails()
        {
            _reportWorkflow = new Dictionary<string, Type>();
            _reportWorkflow.Add("48082111", typeof(IStandardWorkflow));
            _reportWorkflow.Add("48082110", typeof(IManualWorkflow));
            _reportWorkflow.Add("60194861", typeof(IPracticalGuideWorkflow));
            _reportWorkflow.Add("60194869", typeof(IRoadmapWorkflow));
            _reportWorkflow.Add("65511719", typeof(IAlertsAndAnnouncements));
            _reportWorkflow.Add("60194864", typeof(IAlertsAndAnnouncements));
            _reportWorkflow.Add("3", typeof(IHierarchyReportWorkflow));
            _reportWorkflow.Add("2", typeof(IElementsReportWorkflow));
        }
        public IWorkflows GetWorkflow()
        {
            _coreServices.Initialize();
            var pubtype = _coreServices.TransitionContextService.ContentMetadata.PublicationType;

            var workflowType = GetWorkflowType(pubtype);
            var workflow = _serviceProvider.GetRequiredService(workflowType);
            return (IWorkflows)workflow;
        }

        public Type GetWorkflowType(string? publicationType)
        {
            if (_reportWorkflow.TryGetValue(publicationType, out var toReturn))
                return toReturn;
            return null;
        }
        public IWorkflows GetWorkflow(string type)
        {
            var workflowType = GetWorkflowType(type);
            var workflow = _serviceProvider.GetRequiredService(workflowType);
            return (IWorkflows)workflow;
        }

    }
}
