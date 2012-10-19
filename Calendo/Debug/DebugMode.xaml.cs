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

        private void UpdateList()
        {
            this.listBox1.Items.Clear();
            for (int i = 0; i < tm.Entries.Count; i++)
            {
                this.listBox1.Items.Add("[" + tm.Entries[i].ID.ToString() + "] " + tm.Entries[i].Description);
            }
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
    }
}
