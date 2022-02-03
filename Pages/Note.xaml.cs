using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Notes.Pages
{
    /// <summary>
    /// The note tab content page holding the note text.
    /// </summary>
    public sealed partial class Note : Page
    {
        public Note()
        {
            this.InitializeComponent();
        }

        private void NoteInput_Loading(FrameworkElement sender, object args)
        {
            // Auto-focus the note input TextBox as soon as it starts loading.
            // TextBox is loaded as soon as its parent tab is focused and opened
            ((TextBox)sender).Focus(FocusState.Programmatic);
        }

        private void NoteInput_LosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            // Keep the TextBox constantly focused by cancelling
            // the focus action and not letting the TextBox lose
            // focus to a different control. Does not cancel, for
            // example, when the app itself is losing focus, as expected
            args.TryCancel();
        }
    }
}
