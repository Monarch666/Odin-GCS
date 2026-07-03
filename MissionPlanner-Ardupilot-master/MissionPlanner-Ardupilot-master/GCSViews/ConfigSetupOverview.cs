using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.GCSViews.ConfigurationView;

namespace MissionPlanner.GCSViews
{
    public class ConfigSetupOverview : MyUserControl
    {
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnWizard;
        private Panel pnlCardsContainer;
        private Panel pnlStatusBar;
        private Label lblStatusText;

        private CardPanel cardRadio;
        private CardPanel cardAccel;
        private CardPanel cardCompass;
        private CardPanel cardPower;

        public ConfigSetupOverview()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.DoubleBuffered = true;
            this.BackColor = MainV2.OdinTheme.Background;
            this.ForeColor = MainV2.OdinTheme.White;

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Setup & Calibration";
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitle.ForeColor = MainV2.OdinTheme.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            this.Controls.Add(lblTitle);

            // Subtitle
            lblSubtitle = new Label();
            lblSubtitle.Text = "Pre-flight sensor initialization and hardware checks.";
            lblSubtitle.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            lblSubtitle.ForeColor = Color.FromArgb(170, 175, 180);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(22, 65);
            this.Controls.Add(lblSubtitle);

            // Wizard Button
            btnWizard = new Button();
            btnWizard.Text = "🪄  RUN SETUP WIZARD";
            btnWizard.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnWizard.BackColor = MainV2.OdinTheme.Green; // Matrix Green
            btnWizard.ForeColor = Color.Black;            // Black text
            btnWizard.FlatStyle = FlatStyle.Flat;
            btnWizard.FlatAppearance.BorderSize = 0;
            btnWizard.Size = new Size(180, 40);
            btnWizard.Cursor = Cursors.Hand;
            btnWizard.Click += BtnWizard_Click;
            this.Controls.Add(btnWizard);

            // Cards Container
            pnlCardsContainer = new Panel();
            pnlCardsContainer.BackColor = Color.Transparent;
            this.Controls.Add(pnlCardsContainer);

            // 4 Cards
            cardRadio = new CardPanel("Radio", "[ RDO_01 ]", "100%", "Calibrated", false, "CH: 16", "SBUS", typeof(ConfigRadioInput));
            cardAccel = new CardPanel("Accel/Gyro", "[ IMU_02 ]", "Required", "Required", true, "ORIENT: UP", "ERR: >5%", typeof(ConfigAccelerometerCalibration));
            cardCompass = new CardPanel("Compass", "[ MAG_03 ]", "Calibrated", "Calibrated", false, "MAG 1", "EXT", typeof(ConfigHWCompass));
            cardPower = new CardPanel("Power", "[ PWR_04 ]", "22.4V", "Required", true, "BATT 1", "CURR: 0A", typeof(ConfigBatteryMonitoring));

            pnlCardsContainer.Controls.Add(cardRadio);
            pnlCardsContainer.Controls.Add(cardAccel);
            pnlCardsContainer.Controls.Add(cardCompass);
            pnlCardsContainer.Controls.Add(cardPower);

            // Status Bar at Bottom
            pnlStatusBar = new Panel();
            pnlStatusBar.Height = 40;
            pnlStatusBar.BackColor = Color.FromArgb(14, 16, 18);
            this.Controls.Add(pnlStatusBar);

            lblStatusText = new Label();
            lblStatusText.Text = "> SYS_INIT_OK  |  IMU: UNCAL  |  MAG: OK  |  BAT: 22.4V  |  LINK: 98%  |  AWAITING_CALIBRATION";
            lblStatusText.Font = new Font("Consolas", 10F, FontStyle.Bold);
            lblStatusText.ForeColor = MainV2.OdinTheme.Green;
            lblStatusText.AutoSize = true;
            lblStatusText.Location = new Point(15, 12);
            pnlStatusBar.Controls.Add(lblStatusText);

            this.Resize += ConfigSetupOverview_Resize;
        }

        private void BtnWizard_Click(object sender, EventArgs e)
        {
            // Fallback to Radio or first setup tab
            cardRadio.TriggerClick();
        }

        private void ConfigSetupOverview_Resize(object sender, EventArgs e)
        {
            // Position Wizard button
            btnWizard.Location = new Point(this.Width - btnWizard.Width - 30, 20);

            // Layout status bar at bottom
            pnlStatusBar.Location = new Point(20, this.Height - pnlStatusBar.Height - 20);
            pnlStatusBar.Width = this.Width - 40;

            // Layout cards container
            pnlCardsContainer.Location = new Point(20, 100);
            pnlCardsContainer.Size = new Size(this.Width - 40, pnlStatusBar.Top - pnlCardsContainer.Top - 20);

            // Layout individual cards
            int totalSpacing = 48; // 3 gaps of 16px
            int cardWidth = (pnlCardsContainer.Width - totalSpacing) / 4;
            if (cardWidth < 120) cardWidth = 120;

            cardRadio.Bounds = new Rectangle(0, 0, cardWidth, pnlCardsContainer.Height);
            cardAccel.Bounds = new Rectangle(cardWidth + 16, 0, cardWidth, pnlCardsContainer.Height);
            cardCompass.Bounds = new Rectangle((cardWidth + 16) * 2, 0, cardWidth, pnlCardsContainer.Height);
            cardPower.Bounds = new Rectangle((cardWidth + 16) * 3, 0, cardWidth, pnlCardsContainer.Height);
        }

        // Inner class representing a card
        private class CardPanel : Panel
        {
            private string title;
            private string tag;
            private string gaugeValue;
            private string statusText;
            private bool isRequired;
            private string metaLeft;
            private string metaRight;
            private Type targetPageType;

            public CardPanel(string title, string tag, string gaugeValue, string statusText, bool isRequired, string metaLeft, string metaRight, Type targetPageType)
            {
                this.title = title;
                this.tag = tag;
                this.gaugeValue = gaugeValue;
                this.statusText = statusText;
                this.isRequired = isRequired;
                this.metaLeft = metaLeft;
                this.metaRight = metaRight;
                this.targetPageType = targetPageType;

                this.DoubleBuffered = true;
                this.Cursor = Cursors.Hand;
                this.BackColor = MainV2.OdinTheme.Panel;
            }

            public void TriggerClick()
            {
                CardPanel_Click(this, EventArgs.Empty);
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                CardPanel_Click(this, e);
            }

            private void CardPanel_Click(object sender, EventArgs e)
            {
                Control p = this.Parent;
                while (p != null)
                {
                    if (p is ConfigSetupOverview)
                    {
                        ((ConfigSetupOverview)p).NavigateToPage(targetPageType);
                        break;
                    }
                    p = p.Parent;
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw technical green/slate border
                using (var borderPen = new Pen(MainV2.OdinTheme.Border, 1))
                {
                    g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
                }

                // Draw Title and Tag
                using (var titleFont = new Font("Segoe UI", 14F, FontStyle.Bold))
                using (var tagFont = new Font("Consolas", 8.5F, FontStyle.Bold))
                using (var textBrush = new SolidBrush(MainV2.OdinTheme.White))
                using (var tagBrush = new SolidBrush(Color.FromArgb(120, 130, 140)))
                {
                    g.DrawString(title, titleFont, textBrush, 16, 16);
                    SizeF tagSize = g.MeasureString(tag, tagFont);
                    g.DrawString(tag, tagFont, tagBrush, this.Width - tagSize.Width - 16, 20);
                }

                // Draw Circular Gauge in Center
                int centerY = this.Height / 2 - 10;
                int gaugeRadius = Math.Min(this.Width, this.Height) / 5;
                if (gaugeRadius < 45) gaugeRadius = 45;
                if (gaugeRadius > 65) gaugeRadius = 65;

                Rectangle circleRect = new Rectangle(this.Width / 2 - gaugeRadius, centerY - gaugeRadius, gaugeRadius * 2, gaugeRadius * 2);

                // Draw outer track
                using (var trackPen = new Pen(Color.FromArgb(28, 30, 34), 3))
                {
                    g.DrawEllipse(trackPen, circleRect);
                }

                Color accentColor = isRequired ? MainV2.OdinTheme.Orange : MainV2.OdinTheme.Green;
                // Draw active indicator
                using (var activePen = new Pen(accentColor, 2))
                {
                    activePen.DashStyle = DashStyle.Solid;
                    if (title == "Radio")
                    {
                        // Active arc
                        g.DrawArc(activePen, circleRect, -90, 360);
                    }
                    else if (title == "Compass")
                    {
                        // Draw needle line inside compass
                        g.DrawArc(activePen, circleRect, -90, 270);
                        int endX = (int)(this.Width / 2 + Math.Cos(-45 * Math.PI / 180) * (gaugeRadius - 10));
                        int endY = (int)(centerY + Math.Sin(-45 * Math.PI / 180) * (gaugeRadius - 10));
                        g.DrawLine(activePen, this.Width / 2, centerY, endX, endY);
                    }
                    else if (title == "Accel/Gyro")
                    {
                        // Draw two-way arrow icon representation inside circle
                        g.DrawArc(activePen, circleRect, -90, 180);
                        int arrowSize = 6;
                        g.DrawLine(activePen, this.Width / 2 - 6, centerY - 6, this.Width / 2 + 6, centerY + 6);
                        g.DrawLine(activePen, this.Width / 2 + 6, centerY + 6, this.Width / 2 + 6 - arrowSize, centerY + 6);
                        g.DrawLine(activePen, this.Width / 2 + 6, centerY + 6, this.Width / 2 + 6, centerY + 6 - arrowSize);
                    }
                    else
                    {
                        g.DrawArc(activePen, circleRect, -90, 290);
                    }
                }

                // Value inside circle
                if (title != "Accel/Gyro")
                {
                    using (var valueFont = new Font("Consolas", 11F, FontStyle.Bold))
                    using (var valueBrush = new SolidBrush(accentColor))
                    {
                        SizeF valSize = g.MeasureString(gaugeValue, valueFont);
                        g.DrawString(gaugeValue, valueFont, valueBrush, this.Width / 2 - valSize.Width / 2, centerY - valSize.Height / 2);
                    }
                }
                else
                {
                    // Draw Accel exchange arrows
                    using (var iconPen = new Pen(accentColor, 2))
                    {
                        g.DrawLine(iconPen, this.Width / 2 - 4, centerY - 6, this.Width / 2 + 4, centerY + 6);
                    }
                }

                // Draw Status Pill below gauge
                int pillY = centerY + gaugeRadius + 20;
                string pillText = statusText;
                Color pillBg = isRequired ? Color.FromArgb(60, 45, 0) : Color.FromArgb(24, 26, 30);
                Color pillFg = isRequired ? MainV2.OdinTheme.Orange : Color.FromArgb(170, 175, 180);

                using (var pillFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (var pillBrush = new SolidBrush(pillFg))
                using (var bgBrush = new SolidBrush(pillBg))
                using (var pillBorderPen = new Pen(isRequired ? MainV2.OdinTheme.Orange : Color.Transparent, 1))
                {
                    SizeF pillTextSize = g.MeasureString(pillText, pillFont);
                    int pillWidth = (int)pillTextSize.Width + 24;
                    int pillHeight = 24;
                    Rectangle pillRect = new Rectangle(this.Width / 2 - pillWidth / 2, pillY, pillWidth, pillHeight);

                    g.FillRectangle(bgBrush, pillRect);
                    if (isRequired)
                    {
                        g.DrawRectangle(pillBorderPen, pillRect);
                    }

                    g.DrawString(pillText, pillFont, pillBrush, this.Width / 2 - pillTextSize.Width / 2, pillY + 4);
                }

                // Draw Bottom Metadata
                int bottomY = this.Height - 32;
                using (var metaFont = new Font("Consolas", 8.5F, FontStyle.Bold))
                using (var metaLeftBrush = new SolidBrush(Color.FromArgb(120, 125, 130)))
                using (var metaRightBrush = new SolidBrush(title == "Power" ? Color.FromArgb(120, 125, 130) : (isRequired ? MainV2.OdinTheme.Orange : Color.FromArgb(0, 180, 255))))
                {
                    g.DrawString(metaLeft, metaFont, metaLeftBrush, 16, bottomY);
                    SizeF rightSize = g.MeasureString(metaRight, metaFont);
                    g.DrawString(metaRight, metaFont, metaRightBrush, this.Width - rightSize.Width - 16, bottomY);
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                this.BackColor = MainV2.OdinTheme.Panel2;
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                this.BackColor = MainV2.OdinTheme.Panel;
            }
        }

        private void NavigateToPage(Type pageType)
        {
            var bsv = FindBackstageView();
            if (bsv != null)
            {
                foreach (BackstageViewPage page in bsv.Pages)
                {
                    if (page.Page != null && page.Page.GetType() == pageType)
                    {
                        bsv.ActivatePage(page);
                        break;
                    }
                }
            }
        }

        private BackstageView FindBackstageView()
        {
            Control p = this.Parent;
            while (p != null)
            {
                if (p is BackstageView)
                    return (BackstageView)p;
                p = p.Parent;
            }
            return null;
        }
    }
}
