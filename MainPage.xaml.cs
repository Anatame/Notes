using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Notes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Make the TabView interactive while
            // maintaining the mobility of the app window
            ChangeTitleBar();
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayRightInset;
            }

            CustomDragRegion.Height = ShellTitlebarInset.Height = sender.Height;
        }

        /// <summary>
        /// Change the core title bar with a custom title bar to allow
        /// interaction with TabView extended into the title bar area
        /// while still having a draggable region to move the app window
        /// </summary>
        private void ChangeTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Set new title bar element as a draggable region.
            Window.Current.SetTitleBar(CustomDragRegion);
        }

        private void Tabs_Loading(FrameworkElement sender, object args)
        {

        }

        private void Tabs_AddTabButtonClick(MUXC.TabView sender, object args)
        {

        }

        private void Tabs_TabCloseRequested(MUXC.TabView sender, MUXC.TabViewTabCloseRequestedEventArgs args)
        {

        }
    }
}
