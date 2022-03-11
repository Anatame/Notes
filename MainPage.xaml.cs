using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Notes
{
    /// <summary>
    /// The main page of the app containing the tabs.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Keeps track of the settings tab. `null` if settings isn't opened.
        /// </summary>
        public static MUXC.TabViewItem SettingsTab { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            //TODO: Load the settings tab from previous state (default to `null` if settings wasn't open previously)
            SettingsTab = null;

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
        /// Changes the core title bar with a custom title bar to allow
        /// interaction with TabView extended into the title bar area
        /// while still having a draggable region to move the app window.
        /// </summary>
        private void ChangeTitleBar()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Register a handler for when the size of the overlaid caption control changes
            // For example, when the app moves to a screen with a different DPI
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Set new title bar element as a draggable region
            Window.Current.SetTitleBar(CustomDragRegion);
        }

        /// <summary>
        /// Creates a new tab in the given TabView and opens it.
        /// </summary>
        /// <param name="tabView">The TabView to add the new tab in.</param>
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
        /// Opens settings in a new tab if it already isn't opened,
        /// else just focuses on the already opened settings tab.
        /// </summary>
        /// <param name="tabView">The TabView to open the settings tab in.</param>
        private void OpenSettings(MUXC.TabView tabView)
        {
            // No settings tab is open, create a new one
            if (SettingsTab is null)
            {
                SettingsTab = new MUXC.TabViewItem()
                {
                    Header = "Settings",
                    IconSource= new MUXC.SymbolIconSource() { Symbol = Symbol.Setting }
                };

                var content = new Frame();
                SettingsTab.Content = content;

                tabView.TabItems.Add(SettingsTab);
                content.Navigate(typeof(Pages.Settings));
            }

            // Focus on the settings tab
            tabView.SelectedItem = SettingsTab;
        }

        /// <summary>
        /// Closes the passed tab in the given TabView.
        /// Closes the app itself if the last open tab is closed too.
        /// </summary>
        /// <param name="tabView">The TabView containing the tab.</param>
        /// <param name="tab">The tab to close.</param>
        private void CloseTab(MUXC.TabView tabView, MUXC.TabViewItem tab)
        {
            //TODO: Confirm before closing the tab if its changed but unsaved

            // Settings is being closed, set the settings tracker (`SettingsTab`) to null
            if (tab == SettingsTab)
            {
                SettingsTab = null;
            }

            tabView.TabItems.Remove(tab);

            // If all tabs have been closed, close the application itself
            // "Closing" here means suspending the app, and letting Windows
            // manage the app life cycle based on user activity and necessity
            if (tabView.TabItems.Count == 0)
            {
                _ = ApplicationView.GetForCurrentView().TryConsolidateAsync();
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

        private void AddTabSplitButton_Click(MUXC.SplitButton sender, MUXC.SplitButtonClickEventArgs args)
        {
            CreateTab(Tabs);
        }

        private void Tabs_TabCloseRequested(MUXC.TabView sender, MUXC.TabViewTabCloseRequestedEventArgs args)
        {
            CloseTab(sender, args.Tab);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings(Tabs);
        }

        /// <summary>
        /// Gets or sets the app requested theme dynamically at runtime.
        /// </summary>
        public static ElementTheme AppTheme
        {
            get
            {
                return ((FrameworkElement)Window.Current.Content).RequestedTheme;
            }

            set
            {
                ((FrameworkElement)Window.Current.Content).RequestedTheme = value;
            }
        }

        /// <summary>
        /// Gets or sets the app requested theme, based on theme code, dynamically at runtime.
        /// </summary>
        public static int AppThemeCode
        {
            get { return (int)AppTheme; }
            set { AppTheme = (ElementTheme)value; }
        }
    }
}
