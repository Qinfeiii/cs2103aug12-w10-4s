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
        
        private string authCode = "";

        /// <summary>
        /// Get the authorization code
        /// </summary>
        public string AuthorizationCode
        {
            get { return authCode; }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            authCode = this.textBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            Brush activeBrush = (Brush)this.Resources[(object)"Brush_CriticalBorder"];
            FormBorder.BorderBrush = activeBrush;
            FormShadow.Color = ((SolidColorBrush)activeBrush).Color;
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            Brush activeBrush = (Brush)this.Resources[(object)"Brush_InactiveBorder"];
            FormBorder.BorderBrush = activeBrush;
            FormShadow.Color = ((SolidColorBrush)activeBrush).Color;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.SizeAll;
            DragMove();
            this.Cursor = Cursors.Arrow;
        }
    }
}
