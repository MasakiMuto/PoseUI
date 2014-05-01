using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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
			public Joint Center;

			public Head(Joint parent, Point p)
			{
				Center = new Joint(p);
				parent.AddChild(Center);
			}
		}

		class Arm
		{
			public Joint Root, Joint, Head;

			public Arm(Joint parent, Point root, Point joint, Point head)
			{
				Root = new Joint(root);
				Joint = new Joint(joint);
				Head = new Joint(head);
				Root.AddChild(Joint);
				Joint.AddChild(Head);
				parent.AddChild(Root);
			}

			public Arm Mirror()
			{
				var m = new Arm(Root.Parent, Root.Position.Mirror(), Joint.Position.Mirror(), Head.Position.Mirror());
				return m;
			}

			public Polyline GetLine()
			{
				return new Polyline()
				{
					Points = new Windows.UI.Xaml.Media.PointCollection() {Root.GetAbsolutePosition(), Joint.GetAbsolutePosition(), Head.GetAbsolutePosition() }
				};
			}
		}

		class Joint
		{
			public Point Position;
			public Joint Parent { get; private set; }
			List<Joint> Children;

			public Joint()
			{
				Children = new List<Joint>();
			}

			public Joint(Point p)
				: this()
			{
				Position = p;
			}

			public Point GetAbsolutePosition()
			{
				if (Parent == null)
				{
					return Position;
				}
				else
				{
					return Parent.GetAbsolutePosition().Add(Position);
				}
			}

			public void AddChild(Joint child)
			{
				child.Parent = this;
				Children.Add(child);
			}

			

		}

		Body body;
		Head head;
		Arm leftArm, rightArm, leftLeg, rightLeg;
		private Windows.UI.Xaml.Controls.Panel Canvas;

		const int HeadRadius = 50;

		public Human(Windows.UI.Xaml.Controls.Panel Canvas)
		{
			this.Canvas = Canvas;

			body = new Body();
			head = new Head(body.Center, new Point(0, -75));
			leftArm = new Arm(body.Center, new Point(-50, -50), new Point(-50, 20), new Point(-50, 0));
			rightArm = leftArm.Mirror();
			leftLeg = new Arm(body.Center, new Point(0, 100), new Point(-30, 50), new Point(0, 50));
			rightLeg = leftLeg.Mirror();

			Canvas.Children.Add(new Polygon()
			{
				Points = new Windows.UI.Xaml.Media.PointCollection() { leftArm.Root.GetAbsolutePosition(), rightArm.Root.GetAbsolutePosition(), leftLeg.Root.GetAbsolutePosition()}
			});
			Canvas.Children.Add(new Ellipse() { Width = HeadRadius, Height = HeadRadius, Margin = new Windows.UI.Xaml.Thickness(head.Center.GetAbsolutePosition().X - HeadRadius / 2, head.Center.GetAbsolutePosition().Y - HeadRadius / 2, 0, 0) });
			Canvas.Children.Add(leftArm.GetLine());
			Canvas.Children.Add(rightArm.GetLine());
			Canvas.Children.Add(leftLeg.GetLine());
			Canvas.Children.Add(rightLeg.GetLine());
		}

		public void Draw()
		{

		}

	}
}
