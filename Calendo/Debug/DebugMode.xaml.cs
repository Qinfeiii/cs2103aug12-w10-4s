using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Calendo.Data;
using Calendo.Logic;
using Calendo.GoogleCalendar;

namespace Calendo
{
    /// <summary>
    /// Interaction logic for DebugMode.xaml
    /// </summary>
    public partial class DebugMode : Window
    {
        // NOTE: This is a testing class with a GUI interface
        // Used for exploratory testing for TaskManager, SettingsManager, CommandProcessor
        TaskManager tm = new TaskManager();
        CommandProcessor cp = new CommandProcessor();
        public DebugMode()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private Dictionary<int, Entry> entryDictionary;
        private void UpdateList()
        {
            //this.listBox1.Items.Clear();
            entryDictionary = new Dictionary<int, Entry>();

            for (int i = 0; i < tm.Entries.Count; i++)
            {
                //this.listBox1.Items.Add("[" + tm.Entries[i].ID.ToString() + "] " + tm.Entries[i].Description);
                entryDictionary.Add(i, tm.Entries[i]);
            }
            this.listBox1.ItemsSource = entryDictionary;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            // Used for testing CP
            if (e.Key == Key.Enter)
            {
                cp.ExecuteCommand(textBox1.Text);
                tm.Load(); // Update TM with changes done by CP
                UpdateList();
                textBox1.Text = "";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Bypass CP (warning: This is not sync with CP - so CP will use outdated list)
            tm.Add(this.textBox1.Text);
            this.textBox1.Text = "";
            UpdateList();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // Bypass CP (warning: This is not sync with CP - so CP will use outdated list)
            if (this.listBox1.Items.Count > 0 && this.listBox1.SelectedIndex >= 0)
            {
                tm.Remove(tm.Entries[this.listBox1.SelectedIndex].ID);
            }
            UpdateList();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // Bypass CP (warning: This is not sync with CP - so CP will use outdated list)
            tm.Undo();
            UpdateList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("This button is not in use");
            MessageBox.Show(GoogleCalendar.GoogleCalendar.Import());
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager sm = new SettingsManager();
            sm.GetSetting("null");
            sm.SetSetting("test", "a");
            MessageBox.Show(sm.GetSetting("test"));
        }

        private void buttonclose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonnew_Click(object sender, RoutedEventArgs e)
        {
            TestShadow ts = new TestShadow();
            ts.Show();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            JSON<string> test = new JSON<string>();
            string testDate = test.DateToJSON(DateTime.Now);
            MessageBox.Show(testDate);
            DateTime testDateTime = test.JSONToDate(testDate).ToLocalTime();
            MessageBox.Show(testDateTime.ToString());
        }

        private void listBox1_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void buttonInput_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            KeyValuePair<int, Entry> dContext = (KeyValuePair<int, Entry>)btn.DataContext;

            Entry currentEntry = dContext.Value;
            JSON<Entry> jsonParse = new JSON<Entry>();
            MessageBox.Show(jsonParse.Serialize(currentEntry));
        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            TextBox currentTextbox = sender as TextBox;
            KeyValuePair<int, Entry> dContext = (KeyValuePair<int, Entry>)currentTextbox.DataContext;
            if (e.Key == Key.Return)
            {
                // Request change command if needed
                if (currentTextbox.Text != dContext.Value.Description)
                {
                    // Show command in textbox
                    this.textBox1.Text = "/change " + (dContext.Key + 1).ToString() + " " + currentTextbox.Text;
                    currentTextbox.Text = dContext.Value.Description;
                }
                currentTextbox.IsReadOnly = true;
            }
        }

        private void TextBox_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            TextBox currentTextbox = sender as TextBox;
            currentTextbox.IsReadOnly = false;
            currentTextbox.Focusable = true;
            currentTextbox.Focus();
            e.Handled = true; // Required, so that focus do not go back to list item
        }

        private void TextBox_LostKeyboardFocus_1(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox currentTextbox = sender as TextBox;
            if (!currentTextbox.IsReadOnly)
            {
                // Request change command if needed
                KeyValuePair<int, Entry> dContext = (KeyValuePair<int, Entry>)currentTextbox.DataContext;
                if (currentTextbox.Text != dContext.Value.Description)
                {
                    // Show command in textbox
                    this.textBox1.Text = "/change " + (dContext.Key + 1).ToString() + " " + currentTextbox.Text;
                    currentTextbox.Text = dContext.Value.Description;
                }
            }
            currentTextbox.IsReadOnly = true;
            currentTextbox.Focusable = false;
        }

        private void TextBox_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            TextBox currentTextbox = sender as TextBox;
            KeyValuePair<int, Entry> dContext = (KeyValuePair<int, Entry>)currentTextbox.DataContext;
            if (currentTextbox.IsReadOnly)
            {
                currentTextbox.Focusable = false; // So that can select list item
            }
        }
    }
}
