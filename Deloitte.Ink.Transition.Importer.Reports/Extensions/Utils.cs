using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Deloitte.Ink.Transition.Importer.Reports.Extensions
{
    public class Utils
    {

        public string SanatizeTopicBody(string _element)
        {
            string sanatizedElement = string.Empty;
            sanatizedElement = Regex.Replace(_element, "<.*?>", String.Empty);
            sanatizedElement = Regex.Replace(sanatizedElement, @"\t\r\n?|\n", "");

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            sanatizedElement = regex.Replace(sanatizedElement, " ");

            sanatizedElement = sanatizedElement.Length > 30000 ? sanatizedElement.Substring(0, 30000) + "...." : sanatizedElement;
            return sanatizedElement.Trim().Replace(System.Environment.NewLine, " ");


        }
        public string SanatizePublicationTopic(string _element)
        {
            string sanatizedElement = string.Empty;
            sanatizedElement = Regex.Replace(_element, "<.*?>", String.Empty);
            sanatizedElement = Regex.Replace(sanatizedElement, @"\t\r\n?|\n", "");

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            sanatizedElement = regex.Replace(sanatizedElement, " ");

            //sanatizedElement = sanatizedElement.Length > 30000 ? sanatizedElement.Substring(0, 30000) + "...." : sanatizedElement;
            return sanatizedElement.Trim().Replace(System.Environment.NewLine, " ");


        }

        public string RemoveNoiseFromString(string _element)
        {
            _element = _element.Replace('“', '"')
                               .Replace('”', '"')
                                .Replace("…", "...")
                               .Replace('‒', '-')
                                   .Replace('─', '-')
                                    .Replace('’', '-')
                                 .Replace('‘', '\'')
                                .Replace('–', '-');

            return _element;
        }
        public virtual string ExportDataToExcel(DataTable tblReport, string? exportPath, string? zipFileFullName)
        {
            string csvFileName = "";
            try
            {


                csvFileName = Path.Combine(exportPath + "\\" + zipFileFullName?.Split('\\').Last() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                Workbook workbookExportList = new Workbook();

                // Get a reference to the first worksheet in the workbook where data is to be exported
                Worksheet worksheetWithExportedList = workbookExportList.Worksheets[0];


                ImportTableOptions _ioption = new ImportTableOptions();
                // Fill the data from the list into the worksheet starting from a specified cell
                worksheetWithExportedList.Cells.ImportData(tblReport, 0, 0, _ioption);

                int index = worksheetWithExportedList.ConditionalFormattings.Add();

                FormatConditionCollection fcs = worksheetWithExportedList.ConditionalFormattings[index];

                // Sets the conditional format range.
                CellArea ca = new CellArea();

                ca.StartRow = 0;
                ca.EndRow = tblReport.Rows.Count;
                ca.StartColumn = 0;
                ca.EndColumn = 8;

                fcs.AddArea(ca);
                int conditionIndex = fcs.AddCondition(FormatConditionType.CellValue, OperatorType.Equal, "false", "False");

                // Sets the background color.
                FormatCondition fc = fcs[conditionIndex];

                fc.Style.BackgroundColor = Color.Red;

                // Save the output Excel file containing the exported list
                workbookExportList.Save(csvFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed");
            }
            return csvFileName;
        }

        public virtual string ExportImpDataToExcel(DataTable tblReport, string? exportPath, string? zipFileFullName)
        {
            string csvFileName = "";
            try
            {


                csvFileName = Path.Combine(exportPath + "\\" + zipFileFullName?.Split('\\').Last() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");

                Workbook workbookExportList = new Workbook();

                // Get a reference to the first worksheet in the workbook where data is to be exported
                Worksheet worksheetWithExportedList = workbookExportList.Worksheets[0];


                ImportTableOptions _ioption = new ImportTableOptions();
                // Fill the data from the list into the worksheet starting from a specified cell
                worksheetWithExportedList.Cells.ImportData(tblReport, 0, 0, _ioption);

                int index = worksheetWithExportedList.ConditionalFormattings.Add();

                FormatConditionCollection fcs = worksheetWithExportedList.ConditionalFormattings[index];

                // Sets the conditional format range.
                CellArea ca = new CellArea();

                ca.StartRow = 0;
                ca.EndRow = tblReport.Rows.Count;
                ca.StartColumn = 0;
                ca.EndColumn = 2;
               // Save the output Excel file containing the exported list
                workbookExportList.Save(csvFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed");
            }
            return csvFileName;
        }
    }
}
