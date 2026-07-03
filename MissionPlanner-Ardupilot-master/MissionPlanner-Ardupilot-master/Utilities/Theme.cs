using System.Drawing;

namespace MissionPlanner.Utilities
{
    public static class Theme
    {
        public static readonly Color AccentOrange = Color.FromArgb(255, 90, 31); // #FF5A1F
        public static readonly Color Background = Color.FromArgb(10, 12, 14);     // #0A0C0E
        public static readonly Color Panel = Color.FromArgb(26, 29, 33);          // #1A1D21
        public static readonly Color PanelHover = Color.FromArgb(38, 41, 46);     // #26292E
        public static readonly Color Border = Color.FromArgb(59, 75, 55);         // #3B4B37
        public static readonly Color Green = Color.FromArgb(0, 255, 65);          // #00FF41
        public static readonly Color Amber = Color.FromArgb(255, 184, 0);         // #FFB800
        public static readonly Color White = Color.FromArgb(255, 255, 255);
        public static readonly Color GreyText = Color.FromArgb(140, 145, 150);
        public static readonly Color UnselectedText = Color.FromArgb(140, 145, 150);

        public static readonly Font HeaderFont = new Font("Segoe UI", 20F, FontStyle.Bold);
        public static readonly Font SubtitleFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font TitleFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font RegularFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font TechnicalFont = new Font("Consolas", 10F, FontStyle.Bold);
    }
}
