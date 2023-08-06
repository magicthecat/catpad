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

        Form previewForm = new Form();

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            splitContainer = UserInterfaceHelper.CreateAndInitializeSplitContainer();
            this.Controls.Add(splitContainer);

            sidePanel = UserInterfaceHelper.CreateAndInitializeSidePanel();
            splitContainer.Panel2.Controls.Add(sidePanel);


            menuStrip = MenuHelper.CreateMainMenuStrip(
       NewMenuItem_Click,
       OpenMenuItem_Click,
       SaveMenuItem_Click,
       SaveAsMenuItem_Click,
       ExportAsDocMenuItem_Click,
       ExportAsCsvMenuItem_Click,
       FullScreenMenuItem_Click,
       ZoomInButton_Click, 
       ZoomOutButton_Click,
       FormatMarkdownButton_Click,
       PreviewMarkdownButton_Click,
       QuickUpdate_Click
   );

            this.Controls.Add(menuStrip);


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
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Down)  // Check for Ctrl + Down Arrow
            {
                UserInterfaceHelper.FocusAndScrollToEnd(textBox);
            }
        }

        private void QuickUpdate_Click(object sender, EventArgs e)
        {
            FormatMarkdownButton_Click(null, EventArgs.Empty);
            PreviewMarkdownButton_Click(null, EventArgs.Empty);
            UserInterfaceHelper.FocusAndScrollToEnd(textBox);
           // textBox.Focus();
           // textBox.SelectionStart = textBox.Text.Length;
           // textBox.ScrollToCaret(); // Scrolls the content of the control to the current caret position.
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
            textBox.Text = MarkdownHelper.CorrectMarkdown(textBox.Text);
            UpdateHeadingCount();
        }
    
        private void FullScreenMenuItem_Click(object sender, EventArgs e)
        {
            UserInterfaceHelper.ToggleFullScreen(this);
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
            this.Text = UserInterfaceHelper.GetUpdatedTitle(currentFilePath, unsavedChanges);
        }
   private void ZoomInButton_Click(object sender, EventArgs e) => UserInterfaceHelper.ApplyToControls(control => UserInterfaceHelper.ZoomIn(control, 2), textBox, richTextBox);
   private void ZoomOutButton_Click(object sender, EventArgs e) => UserInterfaceHelper.ApplyToControls(control => UserInterfaceHelper.ZoomOut(control, 2), textBox, richTextBox);
private void ExportAsCsvMenuItem_Click(object sender, EventArgs e) => ExportHelper.ExportTextToCsv(textBox.Text, textProcessor);
        private void ExportAsDocMenuItem_Click(object sender, EventArgs e) =>ExportHelper.ExportTextToDoc(textBox.Text, textProcessor);

        private void PreviewMarkdownButton_Click(object sender, EventArgs e)
        {
               string formattedRtf = MarkdownHelper.GetFormattedRtfFromMarkdown(textBox.Text);
    richTextBox.Rtf = formattedRtf;
        }

       
    }
}