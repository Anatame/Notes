using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using MUXC = Microsoft.UI.Xaml.Controls;

namespace Notes.Pages
{
    /// <summary>
    /// The note tab content page holding the note text and relevant meta information.
    /// </summary>
    public sealed partial class Note : Page
    {
        public Note()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Updates the caret position information of the TextBox in
        /// CaretPosition on the status bar in terms of lines and columns.
        /// </summary>
        /// <param name="textBox">The TextBox whose caret position is to be displayed.</param>
        private void UpdateCaretPositionInfo(TextBox textBox)
        {
            var line = 1;
            var column = 1;

            // Loop from the note beginning till the caret's position to count lines and columns
            for (var i = 0; i < textBox.SelectionStart; ++i)
            {
                // New line encountered, increase line count by 1 and reset
                // column to 1 (since we are at the beginning of a new line)
                if (textBox.Text[i] == '\r')
                {
                    column = 1;
                    ++line;

                    continue;
                }

                // Increase column by 1 for every (non-newline) character encountered
                ++column;
            }

            // Update caret position info in the status bar in the form "Ln <line>, Col <column>"
            CaretPosition.Text = $"Ln {line}, Col {column}";
        }

        private void NoteInput_Loading(FrameworkElement sender, object args)
        {
            // Auto-focus the note input TextBox as soon as it starts loading.
            // TextBox is loaded as soon as its parent tab is focused and opened
            ((TextBox)sender).Focus(FocusState.Programmatic);
        }

        private void NoteInput_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Update the caret position info in the status bar in lines and columns
            UpdateCaretPositionInfo((TextBox)sender);
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
