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

namespace Calendo
{
    /// <summary>
    /// Interaction logic for DebugMode.xaml
    /// </summary>
    public partial class DebugMode : Window
    {
        // NOTE: This is a testing class with a GUI interface
        TaskManager tm = new TaskManager();
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
            if (e.Key == Key.Enter)
            {
                tm.PerformCommand(textBox1.Text);
                UpdateList();
                textBox1.Text = "";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            tm.Add(this.textBox1.Text);
            this.textBox1.Text = "";
            UpdateList();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.Items.Count > 0 && this.listBox1.SelectedIndex >= 0)
            {
                tm.Remove(tm.Entries[this.listBox1.SelectedIndex].ID);
            }
            UpdateList();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            tm.Undo();
            UpdateList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //DataDriver dd = new DataDriver();
            //dd.Test();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            //Debug.Assert(false);
            SettingsManager sm = new SettingsManager();
            sm.GetSetting("null");
            sm.SetSetting("test", "a");
            MessageBox.Show(sm.GetSetting("test"));
        }
    }
}
