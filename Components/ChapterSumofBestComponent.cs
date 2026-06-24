using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.Components
{
    public class ChapterSumofBestComponent : IComponent
    {
        protected InfoTimeComponent InternalComponent { get; set; }
        public ChapterSumofBestSettings Settings { get; set; }
        protected LiveSplitState CurrentState { get; set; }
        public string ComponentName => "Chapter SoB";
        
        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        private readonly SectionList sectionList;
        private IRun previousRun;
        protected int ScrollOffset { get; set; }
        private int currentSplit;
        private int currentSectionIndex;
        private int lastSplitIndex = -1;
        private int lastSectionIndex = -1;

        private SplitTimeFormatter Formatter { get; set; }
        private TimeSpan? SumofBestValue { get; set; }

        public ChapterSumofBestComponent(LiveSplitState state)
        {
            Formatter = new SplitTimeFormatter();
            Settings = new ChapterSumofBestSettings();
            InternalComponent = new InfoTimeComponent("Chapter SoB", null, Formatter);
            sectionList = new SectionList();
            sectionList.UpdateSplits(state.Run);
            previousRun = state.Run;

            state.OnStart += state_OnStart;
            state.OnSplit += state_OnSplit;
            state.OnSkipSplit += state_OnSkipSplit;
            state.OnUndoSplit += state_OnUndoSplit;
            state.OnReset += state_OnReset;
            state.OnScrollDown += state_OnScrollDown;
            state.OnScrollUp += state_OnScrollUp;
            state.RunManuallyModified += state_RunManuallyModified;
            CurrentState = state;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.Run != previousRun)
            {
                sectionList.UpdateSplits(state.Run);
                previousRun = state.Run;
                lastSplitIndex = -1;
                lastSectionIndex = -1;
            }

            int runningSectionIndex = Math.Min(Math.Max(state.CurrentSplitIndex, 0), state.Run.Count - 1);
            ScrollOffset = Math.Min(Math.Max(ScrollOffset, -runningSectionIndex), state.Run.Count - runningSectionIndex - 1);
            currentSplit = ScrollOffset + runningSectionIndex;
            currentSectionIndex = sectionList.GetSection(currentSplit);

            if (currentSplit != lastSplitIndex || currentSectionIndex != lastSectionIndex)
            {
                UpdateSumofBest(state);
                lastSplitIndex = currentSplit;
                lastSectionIndex = currentSectionIndex;
            }

            InternalComponent.TimeValue = SumofBestValue;
            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        private void state_OnStart(object sender, EventArgs e)
        {
            ScrollOffset = 0;
            UpdateSumofBest((LiveSplitState)sender);
        }

        private void state_OnSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
            UpdateSumofBest((LiveSplitState)sender);
        }

        private void state_OnSkipSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
            UpdateSumofBest((LiveSplitState)sender);
        }

        private void state_OnUndoSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
            UpdateSumofBest((LiveSplitState)sender);
        }

        private void state_OnReset(object sender, TimerPhase e)
        {
            ScrollOffset = 0;
            UpdateSumofBest((LiveSplitState)sender);
        }

        private void state_OnScrollUp(object sender, EventArgs e)
        {
            ScrollOffset--;
        }

        private void state_OnScrollDown(object sender, EventArgs e)
        {
            ScrollOffset++;
        }

        private void state_RunManuallyModified(object sender, EventArgs e)
        {
            sectionList.UpdateSplits(((LiveSplitState)sender).Run);
        }

        public void UpdateSumofBest(LiveSplitState state)
        {
            Debug.Print("SoBValue before calc is: " + SumofBestValue + ", entering ");
            Debug.Print("SectionList contains " + sectionList.count + " located at the following splits: " + sectionList.anchorIndices.ToString());
            if (currentSectionIndex == -1)
            {
                SumofBestValue = TimeSpan.Zero;
                return;
            }

            var currSection = sectionList.Sections[currentSectionIndex];
            int start = currSection.startIndex;
            int end = currSection.endIndex;
            Debug.Print("Current Section is " + currSection + " starting at split " + start + " and ending at " + end);

            if(start <= end)
            {
                SumofBestValue = LocalSumOfBest(state.Run, start, end, TimingMethod.GameTime);
            }
            else
            {
                SumofBestValue = TimeSpan.Zero;
            }

            Debug.Print("SoBValue after calc is: " + SumofBestValue);
            //int start, end;
            //Debug.Print("Current value of SoB at " + SumofBestValue);
            //if (sectionList.IsMajorSplit(currentSplit))
            //{
            //    start = currentSplit + 1;
            //    end = sectionList.GetNextMajorSplit(state.Run, currentSplit) - 1;
            //    Debug.Print("Major split detected, start index at " + start + " and end index at " + end);
            //}
            //else if (currentSectionIndex != -1)
            //{
            //    Debug.Print("Normal section detected, current section is " + currentSectionIndex);
            //    var currSection = sectionList.Sections[currentSectionIndex];
            //    start = currSection.startIndex;
            //    end = currSection.endIndex;
            //    Debug.Print("Start index decided at " + start + " and end index at " + end);
            //}
            //else
            //{
            //    SumofBestValue = TimeSpan.Zero;
            //    Debug.Print("Unable to determine major split or section indexes, falling back to 0");
            //    return;
            //}

            //if (start <= end)
            //{
            //    var predictions = new TimeSpan?[state.Run.Count() + 1];
            //    SumofBestValue = SumOfBest.CalculateSumOfBest(state.Run, start, end, predictions, false, true, TimingMethod.GameTime);
            //}
            //else
            //{
            //    SumofBestValue = TimeSpan.Zero;
            //}
            //Debug.Print("New value of SoB at " + SumofBestValue);
        }

        private TimeSpan? LocalSumOfBest(IRun run, int startIndex, int endIndex, TimingMethod method)
        {
            TimeSpan? sum = TimeSpan.Zero;
            
            for(int i = startIndex; i <= endIndex; i++)
            {
                var bestTime = run[i].BestSegmentTime[method];
                if (bestTime != null) sum += bestTime;
            }

            return sum;
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.A > 0)
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawBackground(g, state, HorizontalWidth, height);

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideSoBColor ? Settings.SoBColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawBackground(g, state, width, VerticalHeight);

            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideSoBColor ? Settings.SoBColor : state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void Dispose()
        {
            CurrentState.OnStart -= state_OnStart;
            CurrentState.OnSplit -= state_OnSplit;
            CurrentState.OnSkipSplit -= state_OnSkipSplit;
            CurrentState.OnUndoSplit -= state_OnUndoSplit;
            CurrentState.OnReset -= state_OnReset;
        }
    }
}
