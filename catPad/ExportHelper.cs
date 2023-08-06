using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace Helpers
{

    public static class ExportHelper
    {
        public static void ExportTextToCsv(string text, TextProcessor textProcessor)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Export as CSV"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string csvText = textProcessor.ProcessToCsv(text);
                File.WriteAllText(saveFileDialog.FileName, csvText);
            }
        }

        public static void ExportTextToDoc(string text, TextProcessor textProcessor)

        {

                   SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Export as Doc"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Document doc = textProcessor.ProcessToDoc(text);
                doc.SaveToFile(saveFileDialog.FileName, FileFormat.Docx);
            }
        }
    }

}