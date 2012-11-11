//@author A0080860H
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Calendo.AutoSuggest;
using Calendo.Logic;
using System.Windows.Interop;
using EntryType = Calendo.Logic.EntryType;

namespace Calendo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UiViewModel ViewModel;

        private const double CURSOR_OFFSET = 10;
        private double resizeX = 0;
        private double resizeY = 0;
        private bool isResize = false;

        public static RoutedCommand UndoCommand = new RoutedCommand();
        public static RoutedCommand RedoCommand = new RoutedCommand();
        public static RoutedCommand DelCommand = new RoutedCommand();


        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += new EventHandler(FormSourceInitialized);
            ViewModel = new UiViewModel();
           

            UndoCommand.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            RedoCommand.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
            DelCommand.InputGestures.Add(new KeyGesture(Key.Delete));
            DataContext = ViewModel;
            //ViewModel.ShowMessage("Ready.");
        }

        
        //@author A0080933E
        private void FormSourceInitialized(object sender, EventArgs e)
        {
            // Fix for maximize
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(FormMaximize.WindowProc));
        }

        //@author A0080860H
        private void CommandBarLostFocus(object sender, RoutedEventArgs e)
        {
            if (CommandBar.Text.Length == 0)
            {
                EnterCommandWatermark.Visibility = Visibility.Visible;
            }
        }

        private void CommandBarGotFocus(object sender, RoutedEventArgs e)
        {
            EnterCommandWatermark.Visibility = Visibility.Collapsed;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximiseWindow(sender, e);
            }

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimiseWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximiseWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Visible;
                MaximiseButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.BorderThickness = new Thickness(15);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximiseButton.Visibility = Visibility.Visible;
            }
        }

        private void CommandBarKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                // Select the first item in the auto-suggest list, and give it focus.
                AutoSuggestList.SelectedIndex = 0;
                ListBoxItem selectedItem = AutoSuggestList.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                if (selectedItem != null)
                {
                    selectedItem.Focus();
                }
            }
            else if (e.Key == Key.Return)
            {
                string inputString = CommandBar.Text;
                if (inputString.Length > 0)
                {
                    ViewModel.ExecuteCommand(inputString);
                    CommandBar.Clear();
                    FilterListContents();
                }
            }
            else if (e.Key == Key.Escape)
            {
                FocusOnTaskList();
            }
            else if (!CommandBar.Text.StartsWith("/"))
            {
                FilterListContents();
            }
        }

        private void FilterListContents()
        {
            // This method gets called during window initialisation, when
            // there isn't actually a TaskList element yet - hence this check.
            if (TaskList != null)
            {
                TaskList.Items.Filter = o => CategoryFilter(o) && SearchFilter(o);
            }
        }

        private bool SearchFilter(object o)
        {
            string searchString = CommandBar.Text.ToLowerInvariant().Trim();
            if (searchString != "")
            {
                KeyValuePair<int, Entry> currentPair = (KeyValuePair<int, Entry>)o;
                Entry currentEntry = currentPair.Value;
                if (currentEntry != null)
                {
                    string lowercaseDescription =
                        currentEntry.Description.ToLowerInvariant();
                    return lowercaseDescription.Contains(searchString);
                }
                return false;
            }
            return true;
        }

        private bool CategoryFilter(object o)
        {
            KeyValuePair<int, Entry> currentPair = (KeyValuePair<int, Entry>)o;
            Entry currentEntry = currentPair.Value;
            switch (FilterSelector.SelectedIndex)
            {
                case 0: // All items.
                    return true;
                case 1: // Next week.
                    if (currentEntry != null)
                    {
                        bool isEntryWithinNextWeek = currentEntry.StartTime.CompareTo(DateTime.Now.AddDays(7)) <= 0;
                        bool isEntryFloating = currentEntry.Type == EntryType.Floating;
                        return !isEntryFloating && isEntryWithinNextWeek;
                    }
                    break;
                case 2: // Overdue.
                    if (currentEntry != null)
                    {
                        bool isEntryOverdue = UiTaskHelper.IsTaskOverdue(currentEntry);
                        return isEntryOverdue;
                    }
                    break;
                case 3: // Ongoing.
                    if (currentEntry != null)
                    {
                        bool isEntryOngoing = UiTaskHelper.IsTaskOngoing(currentEntry);
                        return isEntryOngoing;
                    }
                    break;
            }
            return false;
        }

        private void FocusOnTaskList()
        {
            TaskList.SelectedIndex = -1;
            TaskList.Focus();
            AutoSuggestBorder.Visibility = Visibility.Collapsed;
        }

        private void AutoSuggestListKeyDown(object sender, KeyEventArgs e)
        {
            // This is on KeyDown, as KeyUp triggers after the SelectedIndex has already changed.
            // (Results in the first item being impossible to select via keyboard.)
            if (e.Key == Key.Up && AutoSuggestList.SelectedIndex == 0)
            {
                CommandBar.Focus();
                AutoSuggestList.SelectedIndex = -1;
            }
            else if (e.Key == Key.Escape)
            {
                FocusOnTaskList();
            }
        }

        private void SetCommandFromSuggestion()
        {
            string suggestion = ((AutoSuggestEntry)AutoSuggestList.SelectedItem).Command;
            bool isInputCommand = suggestion != null && suggestion.First() == AutoSuggest.AutoSuggest.COMMAND_INDICATOR;
            if (isInputCommand)
            {
                string command = suggestion.Split()[0];
                CommandBar.Text = command;
                CommandBar.Focus();
                CommandBar.SelectionStart = command.Length;

                AutoSuggestBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void CommandBarTextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.SetSuggestions(CommandBar.Text);

            AutoSuggestBorder.Visibility = ViewModel.SuggestionList.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AutoSuggestListMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetCommandFromSuggestion();
        }

        private void UndoHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteCommand("/undo");
        }

        private void RedoHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ExecuteCommand("/redo");
        }

        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            FocusOnTaskList();
        }

        private void DeleteHandler(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelectedTask();
        }

        private void DeleteSelectedTask()
        {
            string command = "/remove";
            ExecuteCommandOnSelectedTask(command);
        }

        private void ExecuteCommandOnSelectedTask(string command)
        {
            int selectedIndex = GetSelectedIndex();

            if (selectedIndex != -1)
            {
                ViewModel.ExecuteCommand(command + " " + selectedIndex);
            }
        }

        private void FillCommandOnSelectedTask(string command)
        {
            int selectedIndex = GetSelectedIndex();

            if (selectedIndex != -1)
            {
                CommandBar.Text = command + " " + selectedIndex;
                CommandBar.Focus();
                CommandBar.SelectionStart = CommandBar.Text.Length;
            }
        }

        private int GetSelectedIndex()
        {
            var selectedItem = TaskList.SelectedItem;
            int selectedIndex = -1;
            if (selectedItem != null)
            {
                KeyValuePair<int, Entry> selectedPair = (KeyValuePair<int, Entry>)selectedItem;
                selectedIndex = selectedPair.Key;
            }
            return selectedIndex;
        }

        private void AutoSuggestListKeyUp(object sender, KeyEventArgs e)
        {
            // This is on KeyUp (and not KeyDown) to prevent the event
            // from bubbling through to the Command Bar - would cause
            // the command to be filled, then instantly executed.
            if (e.Key == Key.Return || e.Key == Key.Space)
            {
                SetCommandFromSuggestion();
            }
        }

        private void ChangeButtonClick(object sender, RoutedEventArgs e)
        {
            TextBox relevantBox = GetTextBoxFromCommandButton(sender);

            if (relevantBox != null)
            {
                relevantBox.IsReadOnly = false;
                relevantBox.Focusable = true;
                relevantBox.Focus();
                relevantBox.SelectionStart = relevantBox.Text.Length;
            }
        }

        private static TextBox GetTextBoxFromCommandButton(object sender)
        {
            Button senderButton = sender as Button;
            FrameworkElement currentItem = senderButton.Parent as FrameworkElement;
            Grid parentGrid = null;
            while (parentGrid == null)
            {
                currentItem = currentItem.Parent as FrameworkElement;
                parentGrid = currentItem as Grid;
            }

            TextBox relevantBox = null;
            foreach (UIElement element in parentGrid.Children)
            {
                relevantBox = element as TextBox;
                if (relevantBox != null)
                {
                    break;
                }
            }
            return relevantBox;
        }

        private void SelectTaskFromCommandButton(object sender)
        {
            // Find the Grid that this sender was in.
            Button senderButton = sender as Button;
            FrameworkElement currentItem = senderButton.Parent as FrameworkElement;
            Grid relevantItem = null;
            while (relevantItem == null)
            {
                currentItem = currentItem.Parent as FrameworkElement;
                relevantItem = currentItem as Grid;
            }

            KeyValuePair<int, Entry> selectedPair = (KeyValuePair<int, Entry>)relevantItem.DataContext;
            TaskList.SelectedItem = selectedPair;
        }

        //@author A0080933E
        private void ResizeStart(object sender, MouseEventArgs e)
        {
            isResize = true;
            ((Rectangle)sender).CaptureMouse();
        }

        private void ResizeEnd(object sender, MouseEventArgs e)
        {
            isResize = false;
            ((Rectangle)sender).ReleaseMouseCapture();
        }

        private void Resize(object sender, MouseEventArgs e)
        {
            if (!isResize)
            {
                return;
            }
            resizeX = e.GetPosition(this).X;
            resizeY = e.GetPosition(this).Y;
            switch (((Rectangle)sender).Name)
            {
                case "ResizeTopLeft":
                    ResizeLeft();
                    ResizeTop();
                    break;
                case "ResizeTopMiddle":
                    ResizeTop();
                    break;
                case "ResizeTopRight":
                    ResizeRight();
                    ResizeTop();
                    break;
                case "ResizeMiddleLeft":
                    ResizeLeft();
                    break;
                case "ResizeMiddleRight":
                    ResizeRight();
                    break;
                case "ResizeBottomLeft":
                    ResizeLeft();
                    ResizeBottom();
                    break;
                case "ResizeBottomMiddle":
                    ResizeBottom();
                    break;
                case "ResizeBottomRight":
                    ResizeRight();
                    ResizeBottom();
                    break;
            }
        }

        // Resize on left of window
        private void ResizeLeft()
        {
            resizeX -= CURSOR_OFFSET;
            double newWidth = this.Width - resizeX;
            if (newWidth >= this.MinWidth)
            {
                this.Width = newWidth;
                this.Left += resizeX;
            }
        }

        // Resize on right of window
        private void ResizeRight()
        {
            resizeX += CURSOR_OFFSET;
            if (resizeX < this.MinWidth)
            {
                this.Width = this.MinWidth;
            }
            else
            {
                this.Width = resizeX;
            }
        }

        // Resize on top of window
        private void ResizeTop()
        {
            resizeY -= CURSOR_OFFSET;
            double newHeight = this.Height - resizeY;
            if (newHeight >= this.MinHeight)
            {
                this.Height = newHeight;
                this.Top += resizeY;
            }
        }

        // Resize on bottom of window
        private void ResizeBottom()
        {
            resizeY += CURSOR_OFFSET;
            if (resizeY < this.MinHeight)
            {
                this.Height = this.MinHeight;
            }
            else
            {
                this.Height = resizeY;
            }
        }

        //@author A0080860H
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            SelectTaskFromCommandButton(sender);
            DeleteSelectedTask();
        }

        private void TextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ChangeEntryFromTextBox(sender);
        }

        private void ChangeEntryFromTextBox(object sender)
        {
            TextBox currentTextBox = sender as TextBox;
            if (!currentTextBox.IsReadOnly)
            {
                currentTextBox.IsReadOnly = true;
                currentTextBox.Focusable = false;

                KeyValuePair<int, Entry> currentPair = (KeyValuePair<int, Entry>)currentTextBox.DataContext;
                int currentTask = currentPair.Key;

                if (currentTextBox.Text != currentPair.Value.Description)
                {
                    ViewModel.ExecuteCommand("/change " + currentTask + " " + currentTextBox.Text);
                }
            }
        }

        private void TextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                ChangeEntryFromTextBox(sender);
            }
        }

        private void FilterSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterListContents();
        }
    }
}
