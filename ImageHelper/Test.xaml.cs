﻿using ImageHelper.ViewModel;
using System.Windows;

namespace ImageHelper
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private Utils _utils = new Utils();

        public Test()
        {
            InitializeComponent();

        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

            //ImageBox.Source = (_utils.DrawText("Test", , Convert.ToInt32(TextSizeSlider.Value)));
        }
    }
}
