using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Calendo.CommandProcessing;
using Calendo.Data;

namespace Calendo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AutoSuggest AutoSuggestViewModel;
        private ProcessString CommandProcessor;

        public MainWindow()
        {
            InitializeComponent();
            AutoSuggestViewModel = new AutoSuggest();
            DataContext = AutoSuggestViewModel;
            CommandProcessor = new ProcessString();
            UpdateItemsList();
        }

        private void TbxCommandBarLostFocus(object sender, RoutedEventArgs e)
        {
            if (tbxCommandBar.Text.Length == 0)
            {
                txbEnterCommand.Visibility = Visibility.Visible;
            }
        }

        private void TbxCommandBarGotFocus(object sender, RoutedEventArgs e)
        {
            txbEnterCommand.Visibility = Visibility.Collapsed;
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
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;

                btnRestore.Visibility = Visibility.Visible;
                btnMaximise.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnRestore.Visibility = Visibility.Collapsed;
                btnMaximise.Visibility = Visibility.Visible;
            }
        }

        private void TbxCommandBarKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                // Select the first item in the auto-suggest list, and give it focus.
                lsbAutoSuggestList.SelectedIndex = 0;
                lsbAutoSuggestList.Focus();
            }
            else
            {
                string inputString = tbxCommandBar.Text;
                if (e.Key == Key.Return && inputString.Length > 0)
                {
                    CommandProcessor.ExecuteCommand(inputString);
                    UpdateItemsList();

                    tbxCommandBar.Clear();
                }
            }
        }

        private void UpdateItemsList()
        {
            Dictionary<int, Entry> itemDictionary = new Dictionary<int, Entry>();

            int count = 1;
            foreach (Entry currentEntry in CommandProcessor.TaskManager.Entries)
            {
                itemDictionary.Add(count, currentEntry);
                count++;
            }

            lsbItemsList.ItemsSource = itemDictionary;
        }

        private void LsbAutoSuggestListKeyDown(object sender, KeyEventArgs e)
        {
            // This is on KeyDown, as KeyUp triggers after the SelectedIndex has already changed.
            // (Results in the first item being impossible to select via keyboard.)
            if (e.Key == Key.Up && lsbAutoSuggestList.SelectedIndex == 0)
            {
                tbxCommandBar.Focus();
                lsbAutoSuggestList.SelectedIndex = -1;
            }
            else if (e.Key == Key.Return)
            {
                SetCommandFromSuggestion();
            }
        }

        private void SetCommandFromSuggestion()
        {
            string suggestion = (string) lsbAutoSuggestList.SelectedItem;
            bool isInputCommand = suggestion != null && suggestion.First() == AutoSuggest.COMMAND_INDICATOR;
            if (isInputCommand)
            {
                string command = suggestion.Split()[0];
                tbxCommandBar.Text = command;
                tbxCommandBar.Focus();
                tbxCommandBar.SelectionStart = command.Length;

                bdrAutoSuggestBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void TbxCommandBarTextChanged(object sender, TextChangedEventArgs e)
        {
            //AutoSuggestViewModel.SetSuggestions(tbxCommandBar.Text);

            //bdrAutoSuggestBorder.Visibility = Visibility.Visible;
        }

        private void BtnSettingsClick(object sender, RoutedEventArgs e)
        {
            DebugMode dm = new DebugMode();
            dm.Show();
        }

        private void LsbAutoSuggestListMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetCommandFromSuggestion();
        }

        private void LsbItemsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = lsbItemsList.SelectedItem;
            if (selectedItem != null)
            {
                KeyValuePair<int, Entry> selectedPair = (KeyValuePair<int, Entry>)selectedItem;
                Entry selectedEntry = selectedPair.Value;

                if (tbxCommandBar.Text.Length == 0)
                {
                    int selectedIndex = selectedPair.Key;
                    tbxCommandBar.Text = "/change " + selectedIndex;
                    tbxCommandBar.Focus();
                    tbxCommandBar.SelectionStart = tbxCommandBar.Text.Length;
                }
            }
        }
    }
}
