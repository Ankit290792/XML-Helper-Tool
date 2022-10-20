using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Deloitte.Ink.Transition.Importer.Reports.Extensions
{
    internal abstract class XDocumentFile : XDocument
    {
        public static XDocument GetSourceDocument(string? filePath)
        {
            if (filePath != null)
                return GetXDocument(filePath);
            else
                return null;
        }
        /// <summary>
        /// Removes unnecessary string in XML string and returns XDocument
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static XDocument GetXDocument(string? uri)
        {
            string xmlString = File.ReadAllText(uri);
            xmlString = xmlString.Replace("&included-domains;&hi-d-att;(topic hi-d) &ut-d-att;(topic ut-d)", string.Empty);
            var xDocToTransform = XDocument.Parse(xmlString, LoadOptions.PreserveWhitespace);

            return xDocToTransform;
        }
    }
}
