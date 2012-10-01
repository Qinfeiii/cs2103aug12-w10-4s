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

namespace Calendo
{
    /// <summary>
    /// Interaction logic for MockAuto.xaml
    /// </summary>
    public partial class MockAuto : Window
    {
        public MockAuto()
        {
            InitializeComponent();
        }
        private string cmdtext = "";
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox1.Text != "")
            {
                grid2.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                grid2.Visibility = System.Windows.Visibility.Hidden;
            }
            if (textBox1.Text == "add")
            {
                t1.Foreground = Brushes.Blue;
                t1.FontWeight = FontWeights.Bold;
            }
            
            string lastword = textBox1.Text.Split(' ').Last();
            if (lastword == textBox1.Text)
            {
                cmdtext = textBox1.Text;
                t1.Text = cmdtext;
                t1.FontWeight = FontWeights.Bold;
            }

            if (textBox1.Text.Contains("add") && lastword != "add" && !textBox1.Text.Contains("date:"))
            {
                t2.FontWeight = FontWeights.Bold;
            }
            else
            {
                t2.FontWeight = FontWeights.Regular;
            }
            if (textBox1.Text.Contains("add") && textBox1.Text.Contains("date:") && lastword != "date:" && !textBox1.Text.Contains("time:"))
            {
                t4.FontWeight = FontWeights.Bold;
            }
            else
            {
                t4.FontWeight = FontWeights.Regular;
            }
            if (textBox1.Text.Contains("add") && textBox1.Text.Contains("time:") && lastword != "time:")
            {
                t6.FontWeight = FontWeights.Bold;
            }
            else
            {
                t6.FontWeight = FontWeights.Regular;
            }

            if (textBox1.Text.Contains("date:"))
            {
                t3.Foreground = Brushes.Blue;
                t3.FontWeight = FontWeights.Bold;
                t4.Foreground = Brushes.Gray;
            }
            else
            {
                t3.Foreground = Brushes.Gray;
                t3.FontWeight = FontWeights.Regular;
            }
            if (textBox1.Text.Contains("time:"))
            {
                t5.Foreground = Brushes.Blue;
                t5.FontWeight = FontWeights.Bold;
                t6.Foreground = Brushes.Gray;
            }
            else
            {
                t5.Foreground = Brushes.Gray;
                t5.FontWeight = FontWeights.Regular;
            }

            if (textBox1.Text.Contains("add"))
            {
                t2.Text = "[Description]";
                t3.Text = "date:";
                t4.Text = "[DD/MM]";
                t5.Text = "time:";
                t6.Text = "[HH:MM]";
            }
            else if (textBox1.Text.Contains("remove"))
            {
                t2.Text = "[Task ID]";
            } 
            else
            {
                t2.Text = "";
                t3.Text = "";
                t4.Text = "";
                t5.Text = "";
                t6.Text = "";
            }


        }
    }
}
