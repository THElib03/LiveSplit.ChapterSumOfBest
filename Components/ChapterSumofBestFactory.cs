using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;

namespace LiveSplit.Components
{
    public class ChapterSumofBestFactory : IComponentFactory
    {
        // The displayed name of the component in the Layout Editor.
        public string ComponentName => "Chapter Sum of Best";

        public string Description => "Displays the sum of best time for a chapter or subsplit group.";

        // The sub-menu this component will appear under when adding the component to the layout.
        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new ChapterSumofBestComponent(state);

        public string UpdateName => ComponentName;

        public string UpdateURL => "https://raw.githubusercontent.com/THElib03/";

        public string XMLURL => UpdateURL + "Components/update.LiveSplit.ResetChance.xml";

        public Version Version => Version.Parse("0.1.0");
    }
}
