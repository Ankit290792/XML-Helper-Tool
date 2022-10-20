using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deloitte.Ink.Transition.Importer.Reports.Constants
{
    internal class Constant
    {
    }
    public static class JsonSerializerDeserializer
    {

        public static string SerializeObjectToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }


        public static T DeserializeJsonToObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }

    public static class FileExtensionConstants
    {

        public const string TctxFile = ".tctx";

        public const string MapFile = ".xml";

        public const string MetFile = ".met";
        public const string XmlFile = ".xml";

        public const string ZipFile = ".zip";

        public const string Docx = ".docx";

        public const string Xlsx = ".xlsx";

        public const string Pptx = ".pptx";

        public const string Pdf = ".pdf";
    }
}
