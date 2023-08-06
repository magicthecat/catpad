namespace Helpers
{

    public class MenuHelper
    {

        public static MenuStrip CreateMainMenuStrip(
             EventHandler newMenuItemClick,
             EventHandler openMenuItemClick,
             EventHandler saveMenuItemClick,
             EventHandler saveAsMenuItemClick,
             EventHandler exportAsDocMenuItemClick,
             EventHandler exportAsCsvMenuItemClick,
             EventHandler fullScreenMenuItemClick,
             EventHandler zoomInButtonClick,
             EventHandler zoomOutButtonClick,
             EventHandler formatMarkdownButtonClick,
             EventHandler previewMarkdownButtonClick,
             EventHandler quickUpdateClick
         )
        {
            MenuStrip menuStrip = new MenuStrip();

            // Create the file menu item and its dropdowns
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("&File");
            CreateFileMenuItems(fileMenuItem,
                newMenuItemClick, openMenuItemClick, saveMenuItemClick, saveAsMenuItemClick,
                exportAsDocMenuItemClick, exportAsCsvMenuItemClick, fullScreenMenuItemClick);
            menuStrip.Items.Add(fileMenuItem);

            // Create the toolbar buttons for the menuStrip
            CreateStripMenuButtons(menuStrip,
                zoomInButtonClick, zoomOutButtonClick, formatMarkdownButtonClick,
                previewMarkdownButtonClick, quickUpdateClick);

            return menuStrip;
        }

        private static void CreateFileMenuItems(
            ToolStripMenuItem parentItem,
            EventHandler newMenuItemClick,
            EventHandler openMenuItemClick,
            EventHandler saveMenuItemClick,
            EventHandler saveAsMenuItemClick,
            EventHandler exportAsDocMenuItemClick,
            EventHandler exportAsCsvMenuItemClick,
            EventHandler fullScreenMenuItemClick
        )
        {
            Helpers.Menu.CreateMenuItems(parentItem,
                ("&New", newMenuItemClick, Keys.Control | Keys.N),
                ("&Open", openMenuItemClick, Keys.Control | Keys.O),
                ("Save", saveMenuItemClick, Keys.Control | Keys.S),
                ("Save &As", saveAsMenuItemClick, null),
                ("Export as &Doc", exportAsDocMenuItemClick, null),
                ("Export as &CSV", exportAsCsvMenuItemClick, null),
                ("Full Screen", fullScreenMenuItemClick, Keys.Control | Keys.Shift | Keys.A)
            );
        }

        private static void CreateStripMenuButtons(MenuStrip menuStrip,
            EventHandler zoomInButtonClick,
            EventHandler zoomOutButtonClick,
            EventHandler formatMarkdownButtonClick,
            EventHandler previewMarkdownButtonClick,
            EventHandler quickUpdateClick
        )
        {
            Helpers.Menu.CreateStripMenuButtons(menuStrip,
                ("Zoom In", null, zoomInButtonClick, Keys.Control | Keys.Shift | Keys.Oemplus),
                ("Zoom Out", null, zoomOutButtonClick, Keys.Control | Keys.Shift | Keys.OemMinus),
                ("Format Markdown", null, formatMarkdownButtonClick, Keys.Control | Keys.Shift | Keys.F),
                ("Preview", null, previewMarkdownButtonClick, Keys.Control | Keys.Shift | Keys.P),
                ("Quick Update", null, quickUpdateClick, Keys.Control | Keys.Shift | Keys.Enter)
            );
        }


    }

    public class UserInterfaceHelper
    {

        public static SplitContainer CreateAndInitializeSplitContainer()
        {
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Panel1MinSize = 0
            };
            return splitContainer;
        }

        public static Panel CreateAndInitializeSidePanel()
        {
            var sidePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray
            };
            return sidePanel;
        }

        public static void ToggleFullScreen(Form form)
        {
            if (form.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Normal;
            }
            else
            {
                // Get the screen size
                Rectangle screen = Screen.FromControl(form).WorkingArea;

                // Calculate the desired size (5% gap on each side)
                int width = (int)(screen.Width * 0.9);
                int height = (int)(screen.Height * 0.9);

                // Calculate the desired position (centered on the screen)
                int left = (screen.Width - width) / 2;
                int top = (screen.Height - height) / 2;

                // Apply the size and position
                form.StartPosition = FormStartPosition.Manual;
                form.SetBounds(left, top, width, height);
            }
        }

    public static void ApplyToControls(Action<Control> action, params Control[] controls)
    {
        foreach (var control in controls)
        {
            action(control);
        }
    }

    public static void ZoomIn(Control control, int zoomFactor)
    {
        Font oldFont = control.Font;
        control.Font = FontHelper.IncreaseFontSize(oldFont, zoomFactor);
        oldFont.Dispose();
    }

    public static void ZoomOut(Control control, int zoomFactor)
    {
        Font oldFont = control.Font;
        control.Font = FontHelper.DecreaseFontSize(oldFont, zoomFactor);
        oldFont.Dispose();
    }

        public static string GetUpdatedTitle(string currentFilePath, bool unsavedChanges)
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

            return title;
        }

           public static void  FocusAndScrollToEnd(TextBox textBox)
    {
        textBox.Focus();
        textBox.SelectionStart = textBox.Text.Length;
        textBox.ScrollToCaret();
    }


    }

    public class FontHelper
    {
        public static Font IncreaseFontSize(Font oldFont, float increment)
        {
            Font newFont = new Font(oldFont.FontFamily, oldFont.Size + increment, oldFont.Style);
            return newFont;
        }

        public static Font DecreaseFontSize(Font oldFont, float decrement)
        {
            float newSize = Math.Max(1, oldFont.Size - decrement);
            Font newFont = new Font(oldFont.FontFamily, newSize, oldFont.Style);
            return newFont;
        }
    }

}