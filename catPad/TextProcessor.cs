using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace Helpers
{
    public class TextProcessor
    {
        public string EscapeCsvValue(string value)
        {
            value = value.Replace("\"", "\"\"");
            if (value.Contains(",") || value.Contains("\n") || value.Contains("\""))
            {
                value = $"\"{value}\"";
            }
            return value;
        }

        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void SaveFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public string[] SplitLines(string content)
        {
            return content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }


        public string ProcessToMarkdownPreview(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            StringBuilder markdownPreview = new StringBuilder();

            foreach (string line in lines)
            {
                if (line.StartsWith("* "))
                {
                    // Bullet point
                    markdownPreview.AppendLine(line);
                }
                else if (line.StartsWith("# "))
                {
                    // Title
                    markdownPreview.AppendLine(line);
                }
                else if (line.StartsWith("## "))
                {
                    // Sub Title
                    markdownPreview.AppendLine(line);
                }
                else if (Regex.IsMatch(line, @"^\d+\. ")) // Checking for an ordered list
                {
                    // Ordered list
                    markdownPreview.AppendLine(line);
                }
                else
                {
                    // Regular text
                    markdownPreview.AppendLine(line);
                }
            }

            return markdownPreview.ToString();
        }
        public string ProcessToCsv(string text)
        {
            string[] tickets = text.Split(new[] { "///" }, StringSplitOptions.None);
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("title,description,issueType");

            foreach (string ticket in tickets)
            {
                string[] lines = ticket.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string title = "";
                string issueType = "story";
                StringBuilder description = new StringBuilder();

                bool isGlobalSection = false;

                foreach (string line in lines)
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

                    if (line.StartsWith("# "))
                    {
                        title = line.Substring(2);
                    }
                    else if (line.StartsWith("=type:"))
                    {
                        issueType = line.Substring(6);
                    }
                    else if (line.StartsWith("##"))
                    {
                        description.AppendLine(line.Substring(3));
                    }
                    else
                    {
                        description.AppendLine(line);
                    }
                }

                title = EscapeCsvValue(title);
                string descriptionText = EscapeCsvValue(description.ToString());
                issueType = EscapeCsvValue(issueType);

                csv.AppendLine($"{title},{descriptionText},{issueType}");
            }

            return csv.ToString();
        }

        public Document ProcessToDoc(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            Document document = new Document();
            Section section = document.AddSection();

            bool isGlobalSection = false;

            foreach (string line in lines)
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

                Paragraph paragraph = section.AddParagraph();
                if (line.StartsWith("* "))
                {
                    // Bullet point
                    paragraph.ListFormat.ApplyBulletStyle();
                    paragraph.AppendText(line.Substring(2));
                }
                else if (line.StartsWith("# "))
                {
                    // Title
                    TextRange textRange = paragraph.AppendText(line.Substring(2));
                    textRange.CharacterFormat.Bold = true;
                    textRange.CharacterFormat.FontSize = 20;
                }
                else if (line.StartsWith("## "))
                {
                    // Sub Title
                    TextRange textRange = paragraph.AppendText(line.Substring(3));
                    textRange.CharacterFormat.Bold = true;
                    textRange.CharacterFormat.FontSize = 15;
                }
                else if (Regex.IsMatch(line, @"^\d+\. ")) // Checking for an ordered list
                {
                    // Ordered list
                    paragraph.ListFormat.ApplyNumberedStyle();
                    paragraph.AppendText(line.Substring(line.IndexOf('.') + 2));
                }
                else
                {
                    // Regular text
                    paragraph.AppendText(line);
                }
            }

            return document;
        }
    }





}
