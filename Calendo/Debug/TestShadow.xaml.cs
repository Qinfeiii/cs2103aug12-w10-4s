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
    /// Interaction logic for TestResize.xaml
    /// </summary>
    public partial class TestShadow : Window
    {
        private const double cursorOffset = 10; // Attempt to place resize at where cursor is
        private double resizeX = 0;
        private double resizeY = 0;
        private bool isResize = false;

        public TestShadow()
        {
            InitializeComponent();
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
            resizeX -= cursorOffset;
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
            resizeX += cursorOffset;
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
            resizeY -= cursorOffset;
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
            resizeY += cursorOffset;
            if (resizeY < MinHeight)
            {
                Height = MinHeight;
            }
            else
            {
                Height = resizeY;
            }
        }

        // Drag the window
        private void Move(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.SizeAll;
            DragMove();
            this.Cursor = Cursors.Arrow;
        }

        // Demonstrates that the border can be changed on the fly
        private void ChangeBorderButton(object sender, RoutedEventArgs e)
        {
            Brush activeBrush = (Brush)this.Resources[(object)"Brush_CriticalBorder"];
            FormBorder.BorderBrush = activeBrush;
            FormShadow.Color = ((SolidColorBrush)activeBrush).Color;
        }

        private void WindowClose(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            Brush activeBrush = (Brush)this.Resources[(object)"Brush_ActiveBorder"];
            FormBorder.BorderBrush = activeBrush;
            FormShadow.Color = ((SolidColorBrush)activeBrush).Color;
        }

        private void WindowDeactivated(object sender, EventArgs e)
        {
            Brush activeBrush = (Brush)this.Resources[(object)"Brush_InactiveBorder"];
            FormBorder.BorderBrush = activeBrush;
            FormShadow.Color = ((SolidColorBrush)activeBrush).Color;
        }
    }
}
