namespace Helpers
{

   
    public static class FileOperationHelper
    {
        public static string OpenFileAndGetContent(TextBox targetTextBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                targetTextBox.Text = File.ReadAllText(openFileDialog.FileName);
                return openFileDialog.FileName;
            }
            return null;
        }

        public static string SaveFile(TextBox sourceTextBox, string currentFilePath = null)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    Title = "Save As"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return null;
                }

            }

            File.WriteAllText(currentFilePath, sourceTextBox.Text);
            return currentFilePath;
        }

        public static void CreateNew(TextBox targetTextBox, out string currentFilePath)
        {
            targetTextBox.Clear();
            currentFilePath = null;
        }
    }

}

