using System;
using System.IO;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;
using Helpers;

namespace catPad
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.TextBox textBox;

        string currentFilePath = null;
        bool unsavedChanges = false;
        float currentFontSize = 10;

        MenuStrip menuStrip;
        SplitContainer splitContainer;
        Panel sidePanel;
        RichTextBox richTextBox;

        ToolStripLabel headingCountLabel;



        TextProcessor textProcessor = new TextProcessor();

        public Form1()
        {
            InitializeComponent();



            // Initializing SplitContainer
            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Panel1MinSize = 0;
            this.Controls.Add(splitContainer);

            // Initializing Side Panel
            sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Fill;
            sidePanel.BackColor = Color.LightGray;
            splitContainer.Panel2.Controls.Add(sidePanel);

            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);

            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("&File");
            menuStrip.Items.Add(fileMenuItem);




            Helpers.Menu.CreateMenuItems(fileMenuItem,
          ("&New", new EventHandler(NewMenuItem_Click), Keys.Control | Keys.N),
          ("&Open", new EventHandler(OpenMenuItem_Click), Keys.Control | Keys.O),
          ("Save &As", new EventHandler(SaveAsMenuItem_Click), null),
          ("Export as &Doc", new EventHandler(ExportAsDocMenuItem_Click), null),
          ("Export as &CSV", new EventHandler(ExportAsCsvMenuItem_Click), null),
           ("Full Screen", new EventHandler(FullScreenMenuItem_Click), Keys.Control | Keys.Shift | Keys.A)
      );


            Helpers.Menu.CreateStripMenuButtons(menuStrip,
  ("Zoom In", null, new EventHandler(ZoomInButton_Click), Keys.Control | Keys.Shift | Keys.Oemplus),
    ("Zoom Out", null, new EventHandler(ZoomOutButton_Click), Keys.Control | Keys.Shift | Keys.OemMinus),
    ("Preview", null, new EventHandler(PreviewMarkdownButton_Click), Keys.Control | Keys.Shift | Keys.P),
            ("Format Markdown", null, new EventHandler(FormatMarkdownButton_Click), Keys.Control | Keys.Shift | Keys.F)

);

            headingCountLabel = new ToolStripLabel();
            menuStrip.Items.Add(headingCountLabel);

            textBox = new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(0, menuStrip.Height),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Microsoft Sans Serif", currentFontSize),
                Dock = DockStyle.Fill
            };


            richTextBox = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };
            sidePanel.Controls.Add(richTextBox);


            splitContainer.Panel1.Controls.Add(textBox);


            textBox.TextChanged += TextBox_TextChanged;

            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void UpdateHeadingCount()
        {
            var lines = textBox.Text.Split('\n');
            int count = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("# "))
                {
                    count++;
                }
            }

            headingCountLabel.Text = $"Heading count: {count}";
        }

        private void FormatMarkdownButton_Click(object sender, EventArgs e)
        {
            textBox.Text = CorrectMarkdown(textBox.Text);
            UpdateHeadingCount();
        }
        private string CorrectMarkdown(string text)
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


        private void FullScreenMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // Get the screen size
                Rectangle screen = Screen.FromControl(this).WorkingArea;

                // Calculate the desired size (5% gap on each side)
                int width = (int)(screen.Width * 0.9);
                int height = (int)(screen.Height * 0.9);

                // Calculate the desired position (centered on the screen)
                int left = (screen.Width - width) / 2;
                int top = (screen.Height - height) / 2;

                // Apply the size and position
                this.StartPosition = FormStartPosition.Manual;
                this.SetBounds(left, top, width, height);
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {

            unsavedChanges = true;
            UpdateTitle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox.Text = "/* GLOBALS */";
            ResizeTextBox();
            UpdateTitle();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeTextBox();
        }

        private void ResizeTextBox()
        {
            textBox.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height - menuStrip.Height);
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Open"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = openFileDialog.FileName;
                textBox.Text = File.ReadAllText(currentFilePath);
                unsavedChanges = false;
                UpdateTitle();
            }
        }

        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Save As"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, textBox.Text);
                unsavedChanges = false;
                UpdateTitle();
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                File.WriteAllText(currentFilePath, textBox.Text);
                unsavedChanges = false;
                UpdateTitle();
            }
            else
            {
                SaveAsMenuItem_Click(sender, e);
            }
        }


        private void ExportAsDocMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Word Documents (*.docx)|*.docx",
                Title = "Export as Doc"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Document doc = textProcessor.ProcessToDoc(textBox.Text);
                doc.SaveToFile(saveFileDialog.FileName, FileFormat.Docx);
            }
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                // If there are unsaved changes, save the current document first
                SaveMenuItem_Click(sender, e);
            }

            // Clear the text box and reset the current file path to create a new document
            textBox.Clear();
            currentFilePath = null;
            unsavedChanges = false;
            UpdateTitle();
        }
        private void UpdateTitle()
        {
            string title = "CatPad - ";
            if (string.IsNullOrEmpty(currentFilePath))
            {
                title += "new document";
            }
            else
            {
                title += Path.GetFileName(currentFilePath);
            }

            if (unsavedChanges)
            {
                title += "*";
            }

            this.Text = title;
        }


        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            // Create the Font object for the textBox.
            Font oldFont = textBox.Font;
            textBox.Font = Helpers.FontHelper.IncreaseFontSize(oldFont, 2);
            oldFont.Dispose();

            // Create the Font object for the richTextBox.
            oldFont = richTextBox.Font;
            richTextBox.Font = Helpers.FontHelper.IncreaseFontSize(oldFont, 2);
            oldFont.Dispose();
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            // Create the Font object for the textBox.
            Font oldFont = textBox.Font;
            textBox.Font = Helpers.FontHelper.DecreaseFontSize(oldFont, 2);
            oldFont.Dispose();

            // Create the Font object for the richTextBox.
            oldFont = richTextBox.Font;
            richTextBox.Font = Helpers.FontHelper.DecreaseFontSize(oldFont, 2);
            oldFont.Dispose();
        }

        private void ExportAsCsvMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Export as CSV"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string csvText = textProcessor.ProcessToCsv(textBox.Text);
                File.WriteAllText(saveFileDialog.FileName, csvText);
            }
        }


        private void PreviewMarkdownButton_Click(object sender, EventArgs e)
        {
            UpdateMarkdownPreview();
        }

        private void UpdateMarkdownPreview()
        {
            richTextBox.Clear();

            var lines = textBox.Text.Split('\n');
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
        }
    }
}