using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PoseUI
{
	public sealed partial class MainPage : Page
	{
		Human Human;

		public MainPage()
		{
			this.InitializeComponent();
			Human = new Human(canvas);
			SetTransform();
		}

		void SetTransform()
		{
			var t = canvas.RenderTransform as CompositeTransform;
			var h = Window.Current.Bounds.Height;
			var w = Window.Current.Bounds.Width;
			double originalWidth = 300;
			double originalHeight = 300;
			var scale = Math.Min(h / originalHeight, w / originalWidth);
			t.ScaleY = scale;
			t.ScaleX = scale;
		}

		private void canvas_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			var t = canvas.RenderTransform as CompositeTransform;
			t.Rotation += e.Delta.Rotation;
			
		}
	}
}
