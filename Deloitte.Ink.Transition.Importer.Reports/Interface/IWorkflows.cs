using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    public interface IWorkflows
    {

        ITransitionCoreServices CoreServices { get; }
        ILocationService LocationService { get; }
        ITransitionContextService TransitionContextService { get; }
        DataTable GeneratePublicationReport(bool isZipped);
        public abstract DataTable GenerateExcelReport(string inputValue);
    }
    public interface IStandardWorkflow : IWorkflows
    {
      
    }
    public interface IManualWorkflow : IWorkflows
    {

    }
    public interface IRoadmapWorkflow : IWorkflows
    {

    }
    public interface IPracticalGuideWorkflow : IWorkflows
    {

    }
    public interface IHierarchyReportWorkflow : IWorkflows
    {
      
    }
    public interface IElementsReportWorkflow : IWorkflows
    {
      
    }



}
