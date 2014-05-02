using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace PoseUI
{
	public class Human
	{
		interface IDraggableNode
		{
			IEnumerable<Shape> Shapes { get; }
			void UpdateShape();
		}

		class Body
		{
			public float Rotation;
			public Joint Center;

			public Body()
			{
				Center = new Joint(new Point(0, 0));
			}

		}

		class Head : IDraggableNode
		{
			readonly double Radius = 15;
			public Joint Center;

			Ellipse Ellipse { get; set; }
			ControlBox box;
			

			public Head(Joint parent, Point p)
			{
				Center = new Joint(p);
				parent.AddChild(Center);
				Ellipse = new Ellipse()
				{
					Width = Radius * 2, 
					Height = Radius * 2
				};
				box = new ControlBox(Center, this);
			}

			public IEnumerable<Shape> Shapes
			{
				get
				{
					yield return Ellipse;
					yield return box.Shape;
				}
			}

			public void UpdateShape()
			{
				Ellipse.SetValue(Canvas.LeftProperty, Center.GetAbsolutePosition().X - Radius);
				Ellipse.SetValue(Canvas.TopProperty, Center.GetAbsolutePosition().Y - Radius);
			}
		}

		class Arm : IDraggableNode
		{
			public Joint Root, Joint, Head;

			Polyline Line { get; set; }

			ControlBox jointBox, headBox;

			public Arm(Joint parent, Point root, Point joint, Point head)
			{
				Root = new Joint(root);
				Joint = new Joint(joint);
				Head = new Joint(head);
				Root.AddChild(Joint);
				Joint.AddChild(Head);
				parent.AddChild(Root);
				Line = new Polyline();
				jointBox = new ControlBox(Joint, this);
				headBox = new ControlBox(Head, this);
				UpdateShape();
				Joint.ConstrainLength = true;
				Head.ConstrainLength = true;
			}

			public Arm Mirror()
			{
				var m = new Arm(Root.Parent, Root.Position.Mirror(), Joint.Position.Mirror(), Head.Position.Mirror());
				return m;
			}

			public IEnumerable<Shape> Shapes
			{
				get 
				{
					yield return Line;
					yield return jointBox.Shape;
					yield return headBox.Shape;
				}
			}

			public void UpdateShape()
			{
				jointBox.Update();
				headBox.Update();
				Line.Points = new Windows.UI.Xaml.Media.PointCollection() { Root.GetAbsolutePosition(), Joint.GetAbsolutePosition(), Head.GetAbsolutePosition() };
			}
		}

		class ControlBox
		{
			Joint Target;
			IDraggableNode Parent;

			readonly double Size = 10;
			public Rectangle Shape { get; private set; }
			Point currentPoint;

			public ControlBox(Joint target, IDraggableNode parent)
			{
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
				Shape.ManipulationCompleted += (s, e) => Shape.Fill.SetValue(SolidColorBrush.ColorProperty, Colors.Red);
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
				currentPoint = currentPoint.Add(tr.Inverse.TransformPoint( e.Delta.Translation));
				Target.Position = currentPoint;
				//Target.Position = t.Inverse.TransformPoint(currentPoint);
				
				Update();
				Parent.UpdateShape();
				e.Handled = true;
			}

			public void Update()
			{
				Shape.SetValue(Canvas.LeftProperty, Target.GetAbsolutePosition().X - Size / 2);
				Shape.SetValue(Canvas.TopProperty, Target.GetAbsolutePosition().Y - Size / 2);
			}

		}

		Body body;
		Head head;
		Arm leftArm, rightArm, leftLeg, rightLeg;
		private Windows.UI.Xaml.Controls.Panel canvas;

		Polygon bodyShape;

		public Human(Windows.UI.Xaml.Controls.Panel canvas)
		{
			this.canvas = canvas;

			body = new Body();
			head = new Head(body.Center, new Point(0, -90));
			leftArm = new Arm(body.Center, new Point(-30, -80), Util.FromPolar(50, Math.PI * 0.7), Util.FromPolar(50, Math.PI));
			rightArm = leftArm.Mirror();
			leftLeg = new Arm(body.Center, new Point(0, 0), Util.FromPolar(60, Math.PI * 0.6), Util.FromPolar(50, Math.PI * 0.5));
			rightLeg = leftLeg.Mirror();

			bodyShape = new Polygon()
			{
				Points = new Windows.UI.Xaml.Media.PointCollection() { leftArm.Root.GetAbsolutePosition(), rightArm.Root.GetAbsolutePosition(), leftLeg.Root.GetAbsolutePosition() }
			};
			canvas.Children.Add(bodyShape);
			AddShapes(head.Shapes);
			head.UpdateShape();
			AddShapes(leftArm.Shapes);
			AddShapes(leftLeg.Shapes);
			AddShapes(rightArm.Shapes);
			AddShapes(rightLeg.Shapes);

		}

		void AddShapes(IEnumerable<Shape> shapes)
		{
			foreach (var item in shapes)
			{
				canvas.Children.Add(item);
			}
		}
		

	}
}
