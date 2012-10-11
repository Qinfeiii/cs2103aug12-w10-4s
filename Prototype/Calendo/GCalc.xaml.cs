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
using System.Runtime.InteropServices;

namespace Calendo
{
    /// <summary>
    /// Interaction logic for GCalc.xaml
    /// </summary>
    public partial class GCalc : Window
    {
        public GCalc()
        {
            InitializeComponent();
        }
        [DllImport("user32.dll")]

        static extern int GetWindowLong(IntPtr hwnd, int index);



        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);



        [DllImport("user32.dll")]

        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);



        [DllImport("user32.dll")]

        static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        const int GWL_EXSTYLE = -20;

        const int WS_EX_DLGMODALFRAME = 0x0001;

        const int SWP_NOSIZE = 0x0001;

        const int SWP_NOMOVE = 0x0002;

        const int SWP_NOZORDER = 0x0004;

        const int SWP_FRAMECHANGED = 0x0020;
        const uint WM_SETICON = 0x0080;

        protected override void OnSourceInitialized(EventArgs e)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);

 

            // Update the window's non-client area to reflect the changes

            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

            base.OnSourceInitialized(e);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
