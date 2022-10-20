using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Deloitte.Ink.Transition.Importer.Reports.Interface
{
    internal interface IPracticalGuideService
    {
        public DataTable ProcessAllXmlFiles(ITransitionCoreServices coreServices, string? extractPathDirectory, bool isZipped);
        public void ProcessXmlSrcAndConvertContent(XDocument SourceDocument, string SourceFileName, string extractPathDirectory);
        public void BuildTopicFile(XDocument? SourceDocument, string extractPathDirectory, string SourceFileName);
        public void ProcessTopicsAndSubtopics(XElement? topicElement, string rootName);
        public void ProcessContextFile(string? pubTitle, string SourceFileName, string extractPathDirectory);
 
    }
}
