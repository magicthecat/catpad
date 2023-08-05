namespace Helpers
{

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