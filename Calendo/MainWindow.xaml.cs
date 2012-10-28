using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Shapes;
using Calendo.AutoSuggest;
using Calendo.Logic;
using Calendo.Data;
using System.Windows.Interop;
using EntryType = Calendo.Data.EntryType;

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
        }

        // Fixes for maximize
        void FormSourceInitialized(object sender, EventArgs e)
        {
            System.IntPtr handle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(FormMaximize.WindowProc));
        }
        // End Fixes

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
                // Toggling of WindowStyle is needed to get around the window
                // overlapping the TaskBar if it is maximized when WindowStyle is None.
                /*
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
                */
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
                selectedItem.Focus();
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
            string searchString = CommandBar.Text.ToLowerInvariant().Trim();
            if (CommandBar.Text != "")
            {
                TaskList.Items.Filter = delegate(object o)
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
                                                };
            }
            else
            {
                TaskList.Items.Filter = null;
            }
        }

        private void FocusOnTaskList()
        {
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

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            DebugMode dm = new DebugMode();
            dm.Show();
        }

        private void AutoSuggestListMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetCommandFromSuggestion();
        }

        private void TaskListDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ChangeSelectedTask();
        }

        private void ChangeSelectedTask()
        {
            string command = "/change";
            FillCommandOnSelectedTask(command);
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
            // Disabled as behavior conflicts with several controls
            //FocusOnTaskList();
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

            if(selectedIndex != -1)
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
                KeyValuePair<int, Entry> selectedPair = (KeyValuePair<int, Entry>) selectedItem;
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
            SelectTaskFromCommandButton(sender);
            ChangeSelectedTask();
        }

        private void SelectTaskFromCommandButton(object sender)
        {
            // Find the Grid that this button was in.
            Button senderButton = sender as Button;
            FrameworkElement currentItem = senderButton.Parent as FrameworkElement;
            Grid relevantItem = null;
            while (relevantItem == null)
            {
                currentItem = currentItem.Parent as FrameworkElement;
                relevantItem = currentItem as Grid;
            }

            KeyValuePair<int, Entry> selectedPair = (KeyValuePair<int, Entry>)relevantItem.DataContext;
            TaskList.SelectedIndex = selectedPair.Key - 1;
        }

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
            double newWidth = Width - resizeX;
            if (newWidth >= MinWidth)
            {
                Width = newWidth;
                Left += resizeX;
            }
        }

        // Resize on right of window
        private void ResizeRight()
        {
            resizeX += CURSOR_OFFSET;
            if (resizeX < MinWidth)
            {
                Width = MinWidth;
            }
            else
            {
                Width = resizeX;
            }
        }

        // Resize on top of window
        private void ResizeTop()
        {
            resizeY -= CURSOR_OFFSET;
            double newHeight = Height - resizeY;
            if (newHeight >= MinHeight)
            {
                Height = newHeight;
                Top += resizeY;
            }
        }

        // Resize on bottom of window
        private void ResizeBottom()
        {
            resizeY += CURSOR_OFFSET;
            if (resizeY < MinHeight)
            {
                Height = MinHeight;
            }
            else
            {
                Height = resizeY;
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            SelectTaskFromCommandButton(sender);
            DeleteSelectedTask();
        }
    }
}
