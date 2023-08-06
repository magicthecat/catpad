using System.Text.RegularExpressions;

namespace Helpers
{

    public static class MarkdownHelper
    {
        public static string GetFormattedRtfFromMarkdown(string markdownText)
        {
            RichTextBox richTextBox = new RichTextBox();
            var lines = markdownText.Split('\n');
            List<string> orderedList = null;
            int orderedListLineNum = 1;
            bool isGlobalSection = false;


            foreach (var line in lines)
            {

                if (line.Trim() == "/* GLOBALS")
                {
                    isGlobalSection = true;
                }

                if (line.Trim() == "*/")
                {
                    isGlobalSection = false;
                    continue;
                }

                if (isGlobalSection)
                {
                    continue;
                }

                if (Regex.IsMatch(line, @"^\d+\. ")) // Check if line starts with a number followed by a dot indicating an ordered list.
                {
                    // We're in an ordered list
                    if (orderedList == null)
                    {
                        // This is the first line of the ordered list
                        orderedList = new List<string>();
                    }

                    // Remove the original order number and store the line in the temporary list
                    orderedList.Add(line.Substring(line.IndexOf('.') + 2));
                }
                else
                {
                    // We're not in an ordered list anymore
                    if (orderedList != null)
                    {
                        // Append each line from the ordered list with a new order number
                        foreach (var orderedLine in orderedList)
                        {
                            richTextBox.SelectionIndent = 10;  // Indent ordered list.
                            richTextBox.AppendText(orderedListLineNum.ToString() + ". " + orderedLine);
                            richTextBox.SelectionIndent = 0;
                            richTextBox.AppendText("\n");
                            orderedListLineNum++;
                        }

                        // Reset the ordered list
                        orderedList = null;
                        orderedListLineNum = 1;
                    }

                    if (line.StartsWith("# "))
                    {
                        // Add a title
                        richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold);
                        richTextBox.AppendText(line.Substring(2));
                        richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
                    }
                    else if (line.StartsWith("## "))
                    {
                        // Add a subtitle
                        richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Bold | FontStyle.Italic);
                        richTextBox.AppendText(line.Substring(3));
                        richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);
                    }
                    else if (line.StartsWith("* "))
                    {
                        // Add a bullet point
                        richTextBox.SelectionBullet = true;
                        richTextBox.AppendText(line.Substring(2));
                        richTextBox.SelectionBullet = false;
                    }

                    else if (line.Trim() == "///")
                    {
                        // Add a divider
                        richTextBox.AppendText(new string('_', 30));  // Use 30 underscore characters for the divider
                        richTextBox.AppendText("\n");
                    }
                    else
                    {
                        // Add regular text
                        richTextBox.AppendText(line);
                    }

                    richTextBox.AppendText("\n");
                }
            }

            // Handle case where the last line of the text is part of an ordered list
            if (orderedList != null)
            {
                // Append each line from the ordered list with a new order number
                foreach (var orderedLine in orderedList)
                {
                    richTextBox.SelectionIndent = 10;  // Indent ordered list.
                    richTextBox.AppendText(orderedListLineNum.ToString() + ". " + orderedLine);
                    richTextBox.SelectionIndent = 0;
                    richTextBox.AppendText("\n");
                    orderedListLineNum++;
                }
            }

            string result = richTextBox.Rtf;
            richTextBox.Dispose();
            return result;
        }


    public static string CorrectMarkdown(string text)
    {
        // Replace *string at the start of a line with * string
        text = Regex.Replace(text, @"(\n|^)\*(\w)", "$1* $2");

        // Replace #string and ##string at the start of a line with # string and ## string
        text = Regex.Replace(text, @"(\n|^)#(\w)", "$1# $2");
        text = Regex.Replace(text, @"(\n|^)##(\w)", "$1## $2");

        // Replace 1.string and 1)string at the start of a line with 1. string and 1) string
        text = Regex.Replace(text, @"(\n|^)(\d+\.)\s*(\w)", "$1$2 $3");
        text = Regex.Replace(text, @"(\n|^)(\d+\))\s*(\w)", "$1$2 $3");

        // Initialize dictionary to store global variables
        Dictionary<string, string> globals = new Dictionary<string, string>();

        // Split the text into lines
        string[] lines = text.Split('\n');

        // Process each line
        for (int i = 0; i < lines.Length; i++)
        {
            // Check for GLOBALS block start
            if (lines[i].Trim() == "/* GLOBALS")
            {
                // Process lines until end of GLOBALS block
                while (lines[++i].Trim() != "*/")
                {
                    // Split line into name and value
                    string[] parts = lines[i].Split(':', 2);
                    if (parts.Length == 2)
                    {
                        // Remove '=' from variable name and trim both name and value
                        string name = parts[0].Trim().TrimStart('=');
                        string value = parts[1].Trim();

                        // Add to globals dictionary
                        globals[name] = value;
                    }
                }
            }

            // Replace variables in line
            foreach (var global in globals)
            {
                lines[i] = lines[i].Replace("{" + global.Key + "}", global.Value);
            }
        }

        // Combine lines back into single string
        text = string.Join("\n", lines);

        return text;
    }

    }



}