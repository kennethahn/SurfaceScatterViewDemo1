using Microsoft.Surface.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            cleanedUpTimes = new Dictionary<ScatterViewItem, int>();
            PlayGround.Cursor = Cursors.Hand;
        }

        private Dictionary<ScatterViewItem, int> cleanedUpTimes;

        private void ButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            var item = new ScatterViewItem();
            item.ContainerManipulationCompleted += item_ContainerManipulationCompleted;
            item.ContainerManipulationStarted += item_ContainerManipulationStarted;
            item.ContainerManipulationDelta += item_ContainerManipulationDelta;
            item.BorderBrush = PlayGround.BorderBrush;

            item.AngularDeceleration = 0.00001;
            item.Deceleration = 0.01;
            item.BorderThickness = new Thickness(2d);
            item.Content = "My toy";
            PlayGround.Items.Add(item);
            ResetItem(item);
            LabelStatus.Content = "";
            item.Content = new Ellipse() { Fill = Brushes.Aqua };
        }

        void item_ContainerManipulationDelta(object sender, ContainerManipulationDeltaEventArgs e)
        {
            Trace.WriteLine("Delta: " + e.HorizontalChange + ", " + e.VerticalChange);
            Trace.WriteLine("Orientation: " + e.RotationalChange);
        }

        void item_ContainerManipulationStarted(object sender, ContainerManipulationStartedEventArgs e)
        {
            LabelStatus.Content = "";
        }

        //this is run when an item on the playground has been moved by the user
        void item_ContainerManipulationCompleted(object sender, ContainerManipulationCompletedEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("hello container");
            var item = sender as ScatterViewItem;
            if (ItemIsOutsideView(item))
            {
                PlayGround.Items.Remove(item);
                LabelStatus.Content = " You discarded an item";
            }
            else
            {
                ResetItem(item);
            }
        }

        private void ResetItem(ScatterViewItem item)
        {

            if (item.ActualCenter.X.ToString().Equals("NaN"))
            {
                item.Center = ViewCenter;
                item.Orientation = 0;
            }
            else // slowly reset item 
            {
                Duration duration = new Duration(TimeSpan.FromSeconds(1));
                Storyboard stb = new Storyboard();

                PointAnimation moveCenter = new PointAnimation();  // animate the movement
                Point endPoint = ViewCenter;
                moveCenter.From = item.ActualCenter;
                moveCenter.To = endPoint;
                moveCenter.Duration = duration;
                moveCenter.FillBehavior = FillBehavior.Stop;
                stb.Children.Add(moveCenter);
                Storyboard.SetTarget(moveCenter, item);
                Storyboard.SetTargetProperty(moveCenter, new PropertyPath(ScatterViewItem.CenterProperty));

                var rotationAnimation = new DoubleAnimation(); // animate the rotation
                rotationAnimation.From = item.ActualOrientation;
                rotationAnimation.To = item.ActualOrientation > 0 ? 0 : 360;
                rotationAnimation.Duration = duration;
                rotationAnimation.FillBehavior = FillBehavior.Stop;
                stb.Children.Add(rotationAnimation);
                Storyboard.SetTarget(rotationAnimation, item);
                Storyboard.SetTargetProperty(rotationAnimation, new PropertyPath(ScatterViewItem.OrientationProperty));

                stb.Begin(this);  // run the storyboard
                
                item.Center = endPoint;  // do the actual movement of the item
                item.Orientation = 0;
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
