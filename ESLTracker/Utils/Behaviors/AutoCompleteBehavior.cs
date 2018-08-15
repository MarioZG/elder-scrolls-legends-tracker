using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFTextBoxAutoComplete
{
    public static class AutoCompleteBehavior
    {
        private static ILogger logger = MasserContainer.Container.GetInstance<ILogger>();

        private static TextChangedEventHandler onTextChanged = new TextChangedEventHandler(OnTextChanged);
        private static KeyEventHandler onKeyDown = new KeyEventHandler(OnPreviewKeyDown);
        
        /// <summary>
        /// The collection to search for matches from.
        /// </summary>
        public static readonly DependencyProperty AutoCompleteItemsSource =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteItemsSource",
                typeof(IEnumerable<String>),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(null, OnAutoCompleteItemsSource)
            );
        /// <summary>
        /// Whether or not to ignore case when searching for matches.
        /// </summary>
        public static readonly DependencyProperty AutoCompleteStringComparison =
            DependencyProperty.RegisterAttached
            (
                "AutoCompleteStringComparison",
                typeof(StringComparison),
                typeof(AutoCompleteBehavior),
                new UIPropertyMetadata(StringComparison.Ordinal)
            );

        #region Items Source
        public static IEnumerable<String> GetAutoCompleteItemsSource(DependencyObject obj)
        {
            object objRtn = obj.GetValue(AutoCompleteItemsSource);
            if (objRtn is IEnumerable<String>)
                return (objRtn as IEnumerable<String>);

            return null;
        }

        public static void SetAutoCompleteItemsSource(DependencyObject obj, IEnumerable<String> value)
        {
            obj.SetValue(AutoCompleteItemsSource, value);
        }

        private static void OnAutoCompleteItemsSource(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (sender == null)
                return;

            //If we're being removed, remove the callbacks
            if (e.NewValue == null)
            {
                tb.TextChanged -= onTextChanged;
                tb.PreviewKeyDown -= onKeyDown;
            }
            else if (e.OldValue == null)
            {
                //New source.  Add the callbacks
                tb.TextChanged += onTextChanged;
                tb.PreviewKeyDown += onKeyDown;
            }
        }
            #endregion

            #region String Comparison
            public static StringComparison GetAutoCompleteStringComparison(DependencyObject obj)
        {
            return (StringComparison)obj.GetValue(AutoCompleteStringComparison);
        }

        public static void SetAutoCompleteStringComparison(DependencyObject obj, StringComparison value)
        {
            obj.SetValue(AutoCompleteStringComparison, value);
        }
        #endregion

        /// <summary>
        /// Used for moving the caret to the end of the suggested auto-completion text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = e.OriginalSource as TextBox;
            if (tb == null)
                return;

            logger.Trace($"OnPreviewKeyDown: tb.text={tb.Text};tb.caret index={tb.CaretIndex};tb.selstart={tb.SelectionStart};tb.sellen={tb.SelectionLength};");
            if (e.Key == Key.Enter)
            {

                //If we pressed enter and if the selected text goes all the way to the end, move our caret position to the end
                if (tb.SelectionLength > 0 && (tb.SelectionStart + tb.SelectionLength == tb.Text.Length))
                {
                    tb.SelectionStart = tb.CaretIndex = tb.Text.Length;
                    tb.SelectionLength = 0;
                }
            }
            else if (e.Key == Key.Down)
            {
                e.Handled = true;

                string match = GetNextSuggestion(tb, false);

                //Nothing.  Leave 'em alone
                if (!String.IsNullOrEmpty(match))
                {
                    SuggestMatch(tb, match);
                }
            }
            else if (e.Key == Key.Up)
            {
                e.Handled = true;

                string match = GetNextSuggestion(tb, true);

                //Nothing.  Leave 'em alone
                if (!String.IsNullOrEmpty(match))
                {
                    SuggestMatch(tb, match);
                }
            }
        }



        /// <summary>
        /// Search for auto-completion suggestions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if
            (
                (from change in e.Changes where change.RemovedLength > 0 select change).Any() &&
                (from change in e.Changes where change.AddedLength > 0 select change).Any() == false
            )
                return;

            TextBox tb = e.OriginalSource as TextBox;

            logger.Trace($"OnTextChanged: tb.text={tb.Text};tb.caret index={tb.CaretIndex};tb.selstart={tb.SelectionStart};tb.sellen={tb.SelectionLength};");

            if (sender == null)
                return;

            String match = GetCurrentSuggestions(tb).FirstOrDefault();

            if (!String.IsNullOrEmpty(match))
            {
                SuggestMatch(tb, match);
            }
        }

        private static IEnumerable<string> GetCurrentSuggestions(TextBox tb)
        {
            IEnumerable<string> suggestions = new List<string>();
            IEnumerable<String> values = GetAutoCompleteItemsSource(tb);

            //No reason to search if we don't have any values.
            if (values == null)
                return suggestions;

            string text = tb.Text.Substring(0, tb.CaretIndex);
            //No reason to search if there's nothing there.
            if (String.IsNullOrEmpty(text))
                return suggestions;

            Int32 textLength = text.Length;

            StringComparison comparer = GetAutoCompleteStringComparison(tb);
            //Do search and changes here.
            suggestions = 
                (
                    from
                        value
                    in
                    (
                        from subvalue
                        in values
                        where subvalue != null && subvalue.Length >= textLength
                        select subvalue
                    )
                    where value.Substring(0, textLength).Equals(text, comparer)
                    select value
                );

            return suggestions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="direction">1 - next, key down; -1 - prev, key up</param>
        /// <returns></returns>
        private static string GetNextSuggestion(TextBox tb, bool reverse)
        {
            var suggestions = GetCurrentSuggestions(tb);
            if (reverse)
            {
                suggestions = suggestions.Reverse();
            }
            String match = suggestions.Where(s => s.CompareTo(tb.Text) == (reverse ? -1 : 1)).FirstOrDefault();

            if (String.IsNullOrEmpty(match))
            {
                //cycle through - get first
                match = suggestions.FirstOrDefault();
            }

            return match;
        }

        private static void SuggestMatch(TextBox tb, string match)
        {
            int caretIndex = tb.CaretIndex;
            tb.TextChanged -= onTextChanged;
            tb.Text = match;
            tb.CaretIndex = tb.SelectionStart = caretIndex;
            tb.SelectionLength = (match.Length - caretIndex);
            tb.TextChanged += onTextChanged;
        }
    }
}
