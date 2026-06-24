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
            RealTimeAttack,
            InGameTime
        }
        public SumofBestMode TimingMode { get; set; }

        public enum SumofBestAccuracy
        {
            ZeroDecimal,
            OneDecimal,
            TwoDecimal
        }
        public SumofBestAccuracy Accuracy { get; set; }
        public bool ShowTrailingZeroes { get; set; }
        
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
            TimingMode = SumofBestMode.InGameTime;
            Accuracy = SumofBestAccuracy.ZeroDecimal;
            Display2Rows = false;

            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", this, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideResetColor.DataBindings.Add("Checked", this, "OverrideSoBColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnResetColor.DataBindings.Add("BackColor", this, "SoBColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkTrailingZeroes.DataBindings.Add("Checked", this, "ShowTrailingZeroes", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ChapterSumofBestSettings_Load(object sender, EventArgs e)
        {
            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideResetColor_CheckedChanged(null, null);

            rdoModeRealTimeAttack.Checked = TimingMode == SumofBestMode.RealTimeAttack;
            rdoModeInGameTime.Checked = TimingMode == SumofBestMode.InGameTime;

            rdoDecimalZero.Checked = Accuracy == SumofBestAccuracy.ZeroDecimal;
            rdoDecimalOne.Checked = Accuracy == SumofBestAccuracy.OneDecimal;
            rdoDecimalTwo.Checked = Accuracy == SumofBestAccuracy.TwoDecimal;

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
            Accuracy = SettingsHelper.ParseEnum<SumofBestAccuracy>(element["Accuracy"]);
            ShowTrailingZeroes = SettingsHelper.ParseBool(element["ShowTrailingZeroes"]);
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
            return SettingsHelper.CreateSetting(document, parent, "Version", "0.0.1") ^
                SettingsHelper.CreateSetting(document, parent, "TextColor", TextColor) ^
                SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
                SettingsHelper.CreateSetting(document, parent, "SoBColor", SoBColor) ^
                SettingsHelper.CreateSetting(document, parent, "OverrideSoBColor", OverrideSoBColor) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
                SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
                SettingsHelper.CreateSetting(document, parent, "TimingMode", TimingMode) ^
                SettingsHelper.CreateSetting(document, parent, "Accuracy", Accuracy) ^
                SettingsHelper.CreateSetting(document, parent, "ShowTrailingZeroes", ShowTrailingZeroes) ^
                SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows);
        }

        private void rdoModeRealTimeAttack_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void rdoModeInGameTime_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMode();
        }

        private void rdoDecimalZero_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoDecimalOne_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        private void rdoDecimalTwo_CheckedChanged(object sender, EventArgs e)
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

        private void chkOverrideResetColor_CheckedChanged(object sender, EventArgs e)
        {
            resetColorLabel.Enabled = btnResetColor.Enabled = chkOverrideResetColor.Checked;
        }

        private void UpdateMode()
        {
            if (rdoModeRealTimeAttack.Checked)
                TimingMode = SumofBestMode.RealTimeAttack;
            else if (rdoModeInGameTime.Checked)
                TimingMode = SumofBestMode.InGameTime;
            else
                TimingMode = SumofBestMode.InGameTime;
        }
        /* rdoModeRealTimeAttack.Checked = TimingMode == SumofBestMode.RealTimeAttack;
            rdoModeInGameTime.Checked = TimingMode == SumofBestMode.InGameTime;

            rdoDecimalZero.Checked = Accuracy == SumofBestAccuracy.ZeroDecimal;
            rdoDecimalOne.Checked = Accuracy == SumofBestAccuracy.OneDecimal;
            rdoDecimalTwo.Checked = Accuracy */ 

        private void UpdateAccuracy()
        {
            /* if (decimalsCheckbox.Checked)
                Accuracy = SumofBestAccuracy.ZeroDecimal; */
            if (rdoDecimalZero.Checked)
                Accuracy = SumofBestAccuracy.ZeroDecimal;
            else if (rdoDecimalOne.Checked)
                Accuracy = SumofBestAccuracy.OneDecimal;
            else if (rdoDecimalTwo.Checked)
                Accuracy = SumofBestAccuracy.TwoDecimal;
            else
                Accuracy = SumofBestAccuracy.ZeroDecimal;
        }
    }
}
