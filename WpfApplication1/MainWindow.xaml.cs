using Microsoft.Surface.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            var item = new ScatterViewItem();
            item.ContainerManipulationCompleted += item_ContainerManipulationCompleted;
            item.ContainerManipulationStarted += item_ContainerManipulationStarted;
            item.BorderBrush = PlayGround.BorderBrush;
            item.BorderThickness = new Thickness(2d);
            item.Content = "My toy";
            PlayGround.Items.Add(item);
            item.BringIntoBounds();
            item.BringIntoView();
        }

        void item_ContainerManipulationStarted(object sender, ContainerManipulationStartedEventArgs e)
        {
            LabelStatus.Content = "";
        }

        //this is run when an item on the playground has been moved by the user
        void item_ContainerManipulationCompleted(object sender, ContainerManipulationCompletedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("hello container");
            Console.Out.WriteLine("Manipulation is starting:");
            Console.Out.WriteLine(e.Source);
            var item = sender as ScatterViewItem;
            if (item != null)
            {
                if (ItemIsOutsideView(item))
                {
                    LabelStatus.Content = "Why do I ALWAYS have to clean up your toys?!";
                    item.Center = ViewCenter;
                }
            }
        }

        /// <summary>
        /// Check if the item is touching the border of the playground
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ItemIsOutsideView(ScatterViewItem item)
        {
            double verticalOffset = item.ActualHeight / 2;
            double horizontalOffset = item.ActualWidth / 2;
            Point p1 = new Point(item.ActualCenter.X, item.ActualCenter.Y);
            p1.Offset(horizontalOffset, verticalOffset);
            if (PointIsOutsideView(p1))
            {
                return true;
            }
            p1 = new Point(item.ActualCenter.X, item.ActualCenter.Y);
            p1.Offset(-horizontalOffset, -verticalOffset);
            if (PointIsOutsideView(p1))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a point is outside the view borders
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool PointIsOutsideView(Point p)
        {
            if (p.X < 0 || p.X > PlayGround.ActualWidth)
                return true;
            if (p.Y < 0 || p.Y > PlayGround.ActualHeight)
                return true;
            return false;
        }


        private Point ViewCenter
        {
            get
            {
                double width = PlayGround.ActualWidth;
                double height = PlayGround.ActualHeight;
                Point p = new Point(width / 2, height / 2);
                return p;
            }
        }
    }
}
