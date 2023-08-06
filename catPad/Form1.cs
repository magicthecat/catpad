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

        private void Form1_KeyDown(object sender, KeyEventArgs e) => UserInterfaceHelper.HandleHotKey(e, Keys.Down, ctrl: true, action: () => UserInterfaceHelper.FocusAndScrollToEnd(textBox));
        private void QuickUpdate_Click(object sender, EventArgs e)
        {
            FormatMarkdownButton_Click(null, EventArgs.Empty);
            PreviewMarkdownButton_Click(null, EventArgs.Empty);
            UserInterfaceHelper.FocusAndScrollToEnd(textBox);
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
    

        private void Form1_Load(object sender, EventArgs e)
        {
            
            textBox.Text = "/* GLOBALS */";
            UserInterfaceHelper.ResizeTextBoxBasedOnParent(textBox, this, menuStrip);
            SetUnsavedChangesAndTitle(true);

        }

        private void FullScreenMenuItem_Click(object sender, EventArgs e) => UserInterfaceHelper.ToggleFullScreen(this);
        private void TextBox_TextChanged(object sender, EventArgs e) =>SetUnsavedChangesAndTitle(true);
        
     private void Form1_Resize(object sender, EventArgs e) => UserInterfaceHelper.ResizeTextBoxBasedOnParent(textBox, this, menuStrip);
     private void OpenMenuItem_Click(object sender, EventArgs e) => PerformFileOperation(() => FileOperationHelper.OpenFileAndGetContent(textBox), false);
    
     
         private void SaveAsMenuItem_Click(object sender, EventArgs e) =>   PerformFileOperation(() => FileOperationHelper.SaveFile(textBox), false);

  
 
     private void SaveMenuItem_Click(object sender, EventArgs e) => PerformFileOperation(() => FileOperationHelper.SaveFile(textBox, currentFilePath), false);

        private void ZoomInButton_Click(object sender, EventArgs e) => UserInterfaceHelper.ApplyToControls(control => UserInterfaceHelper.ZoomIn(control, 2), textBox, richTextBox);
        private void ZoomOutButton_Click(object sender, EventArgs e) => UserInterfaceHelper.ApplyToControls(control => UserInterfaceHelper.ZoomOut(control, 2), textBox, richTextBox);
        private void ExportAsCsvMenuItem_Click(object sender, EventArgs e) => ExportHelper.ExportTextToCsv(textBox.Text, textProcessor);
        private void ExportAsDocMenuItem_Click(object sender, EventArgs e) =>ExportHelper.ExportTextToDoc(textBox.Text, textProcessor);

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                // If the document has a name, just save it.
                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    SaveMenuItem_Click(sender, e);
                }
                else
                {
                    // Document is unnamed. Show the Save As dialog.
                    SaveAsMenuItem_Click(sender, e);

                    // If the user canceled the Save As dialog, currentFilePath will still be null.
                    // In this case, we don't create a new document.
                    if (string.IsNullOrEmpty(currentFilePath))
                    {
                        return;
                    }
                }
            }

            FileOperationHelper.CreateNew(textBox, out currentFilePath);
                    SetUnsavedChangesAndTitle(false);
        }

        private void SetUnsavedChangesAndTitle(bool changes)
        {
            unsavedChanges = changes;
            this.Text = UserInterfaceHelper.GetUpdatedTitle(currentFilePath, unsavedChanges);
        }

          private void PreviewMarkdownButton_Click(object sender, EventArgs e)
        {
            string formattedRtf = MarkdownHelper.GetFormattedRtfFromMarkdown(textBox.Text);
            richTextBox.Rtf = formattedRtf;
        }

private void PerformFileOperation(Func<string> operation, bool savedState)
{
    currentFilePath = operation();
    unsavedChanges = savedState;
    this.Text = UserInterfaceHelper.GetUpdatedTitle(currentFilePath, unsavedChanges);
}
     
      
       
    }
}