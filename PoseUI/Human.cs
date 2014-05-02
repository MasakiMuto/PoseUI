using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace PoseUI
{
	public class Human
	{
		class Body
		{
			public float Rotation;
			public Joint Center;

			public Body()
			{
				Center = new Joint(new Point(600, 600));
			}

		}

		class Head
		{
			readonly double Radius = 15;
			public Joint Center;

			public Ellipse Ellipse { get; private set; }

			public Head(Joint parent, Point p)
			{
				Center = new Joint(p);
				parent.AddChild(Center);
				Ellipse = new Ellipse()
				{
					Width = Radius * 2, 
					Height = Radius * 2
				};
			}

			public void Update()
			{
				Ellipse.SetValue(Canvas.LeftProperty, Center.GetAbsolutePosition().X - Radius);
				Ellipse.SetValue(Canvas.TopProperty, Center.GetAbsolutePosition().Y - Radius);
			}
		}

		class Arm
		{
			public Joint Root, Joint, Head;

			public Polyline Line { get; private set; }

			public Arm(Joint parent, Point root, Point joint, Point head)
			{
				Root = new Joint(root);
				Joint = new Joint(joint);
				Head = new Joint(head);
				Root.AddChild(Joint);
				Joint.AddChild(Head);
				parent.AddChild(Root);
				Line = new Polyline();
				Update();
			}

			public Arm Mirror()
			{
				var m = new Arm(Root.Parent, Root.Position.Mirror(), Joint.Position.Mirror(), Head.Position.Mirror());
				return m;
			}

			public void Update()
			{
				Line.Points = new Windows.UI.Xaml.Media.PointCollection() { Root.GetAbsolutePosition(), Joint.GetAbsolutePosition(), Head.GetAbsolutePosition() };
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
			canvas.Children.Add(head.Ellipse);
			head.Update();
			canvas.Children.Add(leftArm.Line);
			canvas.Children.Add(rightArm.Line);
			canvas.Children.Add(leftLeg.Line);
			canvas.Children.Add(rightLeg.Line);
		}

		public void Draw()
		{

		}

	}
}
