using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    public interface IWorkflowResolver
    {
        //  Task<string> ProcessReport(string extractPathDirectory, string importerPackageLocation, string exportLocation, string zipFileFullName);
        IWorkflows GetWorkflow();
        Type GetWorkflowType(string? publicationType);
        IWorkflows GetWorkflow(string type);
    }
}
