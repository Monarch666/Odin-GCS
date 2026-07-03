using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public static class StyleHelper
    {
        public static void ApplyPanelStyle(Panel p, string title, string sysTag)
        {
            p.BackColor = Theme.Panel;
            p.Tag = $"{title}|{sysTag}";
            p.Paint += (sender, e) =>
            {
                Panel panel = sender as Panel;
                if (panel == null) return;

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw thin technical border
                using (var borderPen = new Pen(Theme.Border, 1))
                {
                    g.DrawRectangle(borderPen, 0, 0, panel.Width - 1, panel.Height - 1);
                }

                // Render Title and Bracket Tag
                if (!string.IsNullOrEmpty(title))
                {
                    using (var titleFont = new Font("Segoe UI", 11F, FontStyle.Bold))
                    using (var tagFont = new Font("Consolas", 8F, FontStyle.Bold))
                    using (var titleBrush = new SolidBrush(Theme.White))
                    using (var tagBrush = new SolidBrush(Color.FromArgb(100, 110, 120)))
                    {
                        g.DrawString(title, titleFont, titleBrush, 16, 12);

                        if (!string.IsNullOrEmpty(sysTag))
                        {
                            string fullTag = $"[ {sysTag} ]";
                            SizeF tagSize = g.MeasureString(fullTag, tagFont);
                            g.DrawString(fullTag, tagFont, tagBrush, panel.Width - tagSize.Width - 16, 15);
                        }
                    }
                }
            };
        }

        public static void ApplyButtonStyle(Button btn, bool isAccent)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = isAccent ? 0 : 1;
            btn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            if (isAccent)
            {
                btn.BackColor = Theme.AccentOrange;
                btn.ForeColor = Color.Black;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 120, 70);
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.FromArgb(180, 70, 70); // Cancel / red style
                btn.FlatAppearance.BorderColor = Color.FromArgb(180, 70, 70);
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 180, 70, 70);
            }

            btn.Cursor = Cursors.Hand;
        }

        public static void ApplyLabelStyle(Label lbl, Font font, Color color)
        {
            lbl.Font = font;
            lbl.ForeColor = color;
            lbl.BackColor = Color.Transparent;
        }

        public static void ApplyCheckboxStyle(CheckBox chk)
        {
            chk.FlatStyle = FlatStyle.Flat;
            chk.FlatAppearance.BorderColor = Theme.Border;
            chk.ForeColor = Theme.White;
            chk.Font = Theme.RegularFont;
            chk.Cursor = Cursors.Hand;
        }
    }
}
