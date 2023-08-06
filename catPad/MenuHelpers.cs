namespace Helpers
{

    public class Menu
    {



        public static void CreateStripMenuButtons(MenuStrip menuStrip, params (string Name, Image Image, EventHandler ClickEvent, Keys? ShortcutKeys)[] itemParams)
        {
            foreach (var itemParam in itemParams)
            {
                var menuItem = new ToolStripMenuItem(itemParam.Name, itemParam.Image, itemParam.ClickEvent);
                if (itemParam.ShortcutKeys.HasValue)
                {
                    menuItem.ShortcutKeys = itemParam.ShortcutKeys.Value;
                }
                menuStrip.Items.Add(menuItem);
            }
        }

        public static void CreateMenuItems(ToolStripMenuItem parentMenuItem, params (string title, EventHandler eventHandler, Keys? shortcutKeys)[] menuItems)
        {
            foreach (var (title, eventHandler, shortcutKeys) in menuItems)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(title, null, eventHandler);

                if (shortcutKeys.HasValue)
                {
                    menuItem.ShortcutKeys = shortcutKeys.Value;
                }

                parentMenuItem.DropDownItems.Add(menuItem);
            }
        }


    }


}