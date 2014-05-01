using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

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
				Center = new Joint();
			}
		}

		class Head
		{
			public Joint Center;

			public Head(Joint parent, Point p)
			{
				Center = new Joint(p);
				parent.AddChild(parent);
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

		public Human()
		{
			body = new Body();
			head = new Head(body.Center, new Point(0, -100));
			leftArm = new Arm(body.Center, new Point(-50, -50), new Point(-50, 0), new Point(-50, 0));
			rightArm = leftArm.Mirror();
			leftLeg = new Arm(body.Center, new Point(-30, 100), new Point(0, 50), new Point(0, 50));
			rightLeg = leftLeg.Mirror();
		}

		public void Draw()
		{

		}

	}
}
