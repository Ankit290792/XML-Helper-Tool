using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    public interface IElementsReport
    {
        public abstract DataTable processXrefReports(string _element);
    }
}
