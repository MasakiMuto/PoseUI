using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PoseUI
{
	interface IDraggableNode
	{
		IEnumerable<Shape> Shapes { get; }
		void UpdateShape();
	}

	class ControlBox
	{
		Joint Target;
		IDraggableNode Parent;
		Human human;

		readonly double Size = 10;
		public Rectangle Shape { get; private set; }
		Point currentPoint;

		public ControlBox(Joint target, IDraggableNode parent, Human human)
		{
			this.human = human;
			Target = target;
			Parent = parent;
			Shape = new Rectangle()
			{
				Width = Size,
				Height = Size
			};
			Shape.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateY | Windows.UI.Xaml.Input.ManipulationModes.TranslateX;
			Shape.ManipulationDelta += shape_ManipulationDelta;
			Shape.Fill = new SolidColorBrush(Colors.Red);
			Shape.Stroke = new SolidColorBrush(Colors.Green);
			Shape.ManipulationStarted += (s, e) =>
			{
				currentPoint = Target.Position;
				Shape.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Blue);
			};
			Shape.ManipulationCompleted += (s, e) =>
			{
				Shape.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Red);
				human.Push();
			};
			Update();
		}

		void shape_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
		{
			var t = (Shape.Parent as Canvas).RenderTransform as CompositeTransform;
			var tr = new CompositeTransform()
			{
				ScaleX = t.ScaleX,
				ScaleY = t.ScaleY,
				Rotation = t.Rotation
			};
			currentPoint = currentPoint.Add(tr.Inverse.TransformPoint(e.Delta.Translation));
			Target.Position = currentPoint;

			Parent.UpdateShape();
			e.Handled = true;
		}

		public void Update()
		{
			Shape.SetValue(Canvas.LeftProperty, Target.GetAbsolutePosition().X - Size / 2);
			Shape.SetValue(Canvas.TopProperty, Target.GetAbsolutePosition().Y - Size / 2);
		}

	}

}
