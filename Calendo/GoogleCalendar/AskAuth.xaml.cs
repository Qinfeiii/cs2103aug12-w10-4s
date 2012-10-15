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

namespace Calendo.GoogleCalendar
{
    /// <summary>
    /// Interaction logic for AskAuthxaml.xaml
    /// </summary>
    public partial class AskAuth : Window
    {
        public AskAuth()
        {
            InitializeComponent();
        }
        public string authCode = "";
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            authCode = this.textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
