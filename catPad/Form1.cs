using System;
using System.IO;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Drawing;

namespace catPad
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.TextBox textBox;
        MenuStrip menuStrip;
        string currentFilePath = null;
        bool unsavedChanges = false;
        float currentFontSize = 10;

        ToolStripButton zoomInButton;
        ToolStripButton zoomOutButton;

        public Form1()
        {
            InitializeComponent();

            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);

            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("&File");

            ToolStripMenuItem newMenuItem = new ToolStripMenuItem("&New", null, new EventHandler(NewMenuItem_Click), Keys.Control | Keys.N);
            fileMenuItem.DropDownItems.Add(newMenuItem);

            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("&Open", null, new EventHandler(OpenMenuItem_Click), Keys.Control | Keys.O);
            fileMenuItem.DropDownItems.Add(openMenuItem);

            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("&Save", null, new EventHandler(SaveMenuItem_Click), Keys.Control | Keys.S);
            fileMenuItem.DropDownItems.Add(saveMenuItem);

            ToolStripMenuItem saveAsMenuItem = new ToolStripMenuItem("Save &As", null, new EventHandler(SaveAsMenuItem_Click));
            fileMenuItem.DropDownItems.Add(saveAsMenuItem);

            ToolStripMenuItem exportAsDocMenuItem = new ToolStripMenuItem("Export as &Doc", null, new EventHandler(ExportAsDocMenuItem_Click));
            fileMenuItem.DropDownItems.Add(exportAsDocMenuItem);

            menuStrip.Items.Add(fileMenuItem);

            zoomInButton = new ToolStripButton("Zoom In");
            zoomInButton.Click += new EventHandler(ZoomInButton_Click);

            zoomOutButton = new ToolStripButton("Zoom Out");
            zoomOutButton.Click += new EventHandler(ZoomOutButton_Click);

            menuStrip.Items.Add(zoomInButton);
            menuStrip.Items.Add(zoomOutButton);

            textBox = new System.Windows.Forms.TextBox
            {
                Location = new System.Drawing.Point(0, menuStrip.Height),
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Microsoft Sans Serif", currentFontSize)
            };

            textBox.TextChanged += TextBox_TextChanged;

            this.Controls.Add(textBox);

            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            unsavedChanges = true;
            UpdateTitle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox.Text = "Hello, world!";
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
                string[] lines = textBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                Document document = new Document();
                Section section = document.AddSection();

                foreach (string line in lines)
                {
                    Paragraph paragraph = section.AddParagraph();
                    if (line.StartsWith("*"))
                    {
                        // Bullet point
                        paragraph.ListFormat.ApplyBulletStyle();
                        paragraph.AppendText(line.Substring(1));
                    }
                    else if (line.StartsWith("|"))
                    {
                        // Title
                        TextRange textRange = paragraph.AppendText(line.Substring(1));
                        textRange.CharacterFormat.Bold = true;
                        textRange.CharacterFormat.FontSize = 20;
                    }

                    else if (line.StartsWith("||"))
                    {
                        // Title
                        TextRange textRange = paragraph.AppendText(line.Substring(2));
                        textRange.CharacterFormat.Bold = true;
                        textRange.CharacterFormat.FontSize = 15;
                    }

                    else
                    {
                        // Regular text
                        paragraph.AppendText(line);

                    }
                }

                document.SaveToFile(saveFileDialog.FileName, FileFormat.Docx);
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
            if (currentFontSize < 72)  // Maximum size of 72
            {
                currentFontSize += 2;
                textBox.Font = new Font("Microsoft Sans Serif", currentFontSize);
            }
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            if (currentFontSize > 2)  // Minimum size of 2
            {
                currentFontSize -= 2;
                textBox.Font = new Font("Microsoft Sans Serif", currentFontSize);
            }
        }
    }
}