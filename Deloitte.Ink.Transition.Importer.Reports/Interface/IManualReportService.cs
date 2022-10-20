using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    internal interface IManualReportService
    {
        public DataTable ProcessAllXmlFiles(string? extractPathDirectory, bool isZipped);
        public void ProcessXmlSrcAndConvertContent(XDocument SourceDocument, string SourceFileName, string extractPathDirectory);
        public void ProcessPgroups(XDocument? SourceDocument, List<XElement>? pgroups, string extractPathDirectory, string SourceFileName);
        public void AddContentForManualReport(string? ConvertBodyElement, string? SourceBodyElement, string? TopicID, string? SourceFileName, string? CvtFileName, string FileType);
        public void ProcessContextFile(XDocument SourceDocument, string SourceFileName, string extractPathDirectory);
        public void ProcessPrefaceFile(XDocument SourceDocument, string SourceFileName, string extractPathDirectory);
    }
}
