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

        /// <summary>
        /// Creates a new tab in the given TabView and opens it.
        /// </summary>
        /// <param name="tabView">The TabView to add the new tab in</param>
        private void CreateTab(MUXC.TabView tabView)
        {
            var tab = new MUXC.TabViewItem()
            {
                Header = "Untitled.txt",
                IconSource = new MUXC.SymbolIconSource() { Symbol = Symbol.Document }
            };

            var content = new Frame();
            tab.Content = content;

            tabView.TabItems.Add(tab);
            content.Navigate(typeof(Pages.Note));

            // Open the newly created tab
            tabView.SelectedItem = tab;
        }

        /// <summary>
        /// Closes the passed tab in the given TabView.
        /// Closes the app itself if the last open tab is closed too.
        /// </summary>
        /// <param name="tabView">The TabView containing the tab</param>
        /// <param name="tab">The tab to close</param>
        private void CloseTab(MUXC.TabView tabView, MUXC.TabViewItem tab)
        {
            //TODO: Confirm before closing the tab if its changed but unsaved
            tabView.TabItems.Remove(tab);

            // If all tabs have been closed, close the app itself
            if (tabView.TabItems.Count == 0)
            {
                Application.Current.Exit();
            }
        }

        private void Tabs_Loading(FrameworkElement sender, object args)
        {
            //TODO: Load tabs and their content, if any, from the previous session
            var tabView = (MUXC.TabView)sender;

            // If no tabs were left open in the previous session
            // or if the app has been launched for the first time,
            // start fresh with a new empty note at app launch
            if (tabView.TabItems.Count == 0)
            {
                CreateTab(tabView);
            }
        }

        private void Tabs_AddTabButtonClick(MUXC.TabView sender, object args)
        {
            CreateTab(sender);
        }

        private void Tabs_TabCloseRequested(MUXC.TabView sender, MUXC.TabViewTabCloseRequestedEventArgs args)
        {
            CloseTab(sender, args.Tab);
        }
    }
}
