using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.Components
{
    public partial class ChapterSumofBestSettings : UserControl
    {
        public Color TextColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color SoBColor { get; set; }
        public bool OverrideSoBColor { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public enum SumofBestMode
        {
            CurrentMethod,
            RealTimeAttack,
            InGameTime
        }
        public SumofBestMode TimingMode { get; set; }
        public TimeAccuracy Accuracy { get; set; }

        public GeneralTimeFormatter ChapterSumOfBestTimerFormater { get; set; } = new GeneralTimeFormatter()
        {
            NullFormat = NullFormat.Dash,
            Accuracy = TimeAccuracy.Tenths
        };
        
        public bool Display2Rows { get; set; }

        public LayoutMode Mode { get; set; }
        
        public ChapterSumofBestSettings()
        {
            InitializeComponent();

            TextColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            SoBColor = Color.FromArgb(255, 255, 255);
            OverrideSoBColor = false;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            TimingMode = SumofBestMode.CurrentMethod;
            Accuracy = TimeAccuracy.Tenths;
            Display2Rows = false;

            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ChapterSumofBestSettings_Load(object sender, EventArgs e)
        {
            chkOverrideTextColor_CheckedChanged(null, null);

            rdoModeRealTimeAttack.Checked = TimingMode == SumofBestMode.RealTimeAttack;
            rdoModeInGameTime.Checked = TimingMode == SumofBestMode.InGameTime;

            rdoSeconds.Checked = Accuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = Accuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = Accuracy == TimeAccuracy.Hundredths;
            rdoMilliseconds.Checked = Accuracy == TimeAccuracy.Milliseconds;

            if (Mode == LayoutMode.Horizontal)
            {
                chkTwoRows.Enabled = false;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.Checked = true;
            }
            else
            {
                chkTwoRows.Enabled = true;
                chkTwoRows.DataBindings.Clear();
                chkTwoRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            TextColor = SettingsHelper.ParseColor(element["TextColor"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            SoBColor = SettingsHelper.ParseColor(element["SoBColor"]);
            OverrideSoBColor = SettingsHelper.ParseBool(element["OverrideSoBColor"]);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
            TimingMode = SettingsHelper.ParseEnum<SumofBestMode>(element["TimingMode"]);
            //Accuracy = SettingsHelper.ParseEnum<TimeAccuracy>(element["Accuracy"]);
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "0.1.0") ^
                SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
                SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
                SettingsHelper.CreateSetting(document, parent, "SoBColor", SoBColor) ^
                SettingsHelper.CreateSetting(document, parent, "OverrideSoBColor", OverrideSoBColor) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
                SettingsHelper.CreateSetting(document, parent, "TimingMode", TimingMode) ^
                SettingsHelper.CreateSetting(document, parent, "Accuracy", Accuracy) ^
                SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        private void rdoModeCurrentMethod_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void rdoModeRealTimeAttack_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void rdoModeInGameTime_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoHundredths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoMilliseconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void ColorButtonClicked(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        private void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            textColorLabel.Enabled = btnTextColor.Enabled = chkOverrideTextColor.Checked;
        }

        private void UpdateMode()
        {
            TimingMode =
                rdoModeCurrentMethod.Checked ? SumofBestMode.CurrentMethod :
                rdoModeInGameTime.Checked ? SumofBestMode.InGameTime :
                SumofBestMode.RealTimeAttack;
        }

        private void UpdateAccuracy()
        {
            Accuracy =
                rdoSeconds.Checked ? TimeAccuracy.Seconds :
                rdoTenths.Checked ? TimeAccuracy.Tenths :
                rdoHundredths.Checked ? TimeAccuracy.Hundredths :
                TimeAccuracy.Milliseconds;

            ChapterSumOfBestTimerFormater.Accuracy = Accuracy;
        }
    }
}
