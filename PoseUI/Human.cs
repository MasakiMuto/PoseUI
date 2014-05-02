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
			

			public Head(Joint parent, Point p, Human human)
			{
				Center = new Joint(p);
				parent.AddChild(Center);
				Ellipse = new Ellipse()
				{
					Width = Radius * 2, 
					Height = Radius * 2
				};
				box = new ControlBox(Center, this, human);
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
				box.Update();
			}
		}

		class Arm : IDraggableNode
		{
			public Joint Root, Joint, Head;
			readonly Human human;

			Polyline Line { get; set; }

			ControlBox jointBox, headBox;

			public Arm(Joint parent, Point root, Point joint, Point head, Human human)
			{
				this.human = human;
				Root = new Joint(root);
				Joint = new Joint(joint);
				Head = new Joint(head);
				Root.AddChild(Joint);
				Joint.AddChild(Head);
				parent.AddChild(Root);
				Line = new Polyline();
				jointBox = new ControlBox(Joint, this, human);
				headBox = new ControlBox(Head, this, human);
				UpdateShape();
				Joint.ConstrainLength = true;
				Head.ConstrainLength = true;
			}

			public Arm Mirror()
			{
				var m = new Arm(Root.Parent, Root.Position.Mirror(), Joint.Position.Mirror(), Head.Position.Mirror(), human);
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

		
		Body body;
		Head head;
		Arm leftArm, rightArm, leftLeg, rightLeg;
		private Windows.UI.Xaml.Controls.Panel canvas;

		Polygon bodyShape;

		Arm[] arms;

		class StateManager
		{
			struct State
			{
				Point[] points;

				public State(Human current)
				{
					points = new Point[1 + 2 * 4];
					points[0] = current.head.Center.Position;
					int i = 1;
					foreach (var item in current.arms)
					{
						points[i] = item.Joint.Position;
						i++;
						points[i] = item.Head.Position;
						i++;
					}
				}

				public Point this[int i]
				{
					get { return points[i]; }
				}
			}

			List<State> states;
			int index;
			readonly Human human;

			public StateManager(Human human)
			{
				states = new List<State>();
				index = -1;
				this.human = human;
			}

			public void Push()
			{
				var s = new State(human);
				Push(s);
			}

			void Push(State s)
			{
				if (index != states.Count - 1)//履歴先頭でない
				{
					states.RemoveRange(index + 1, states.Count - index - 1);
				}
				states.Add(s);
				index++;
			}

			public void Undo()
			{
				if (index <= 0)
				{
					return;
				}
				index--;
				Pop(states[index]);
			}

			public void Redo()
			{
				if (states.Count < index + 2)
				{
					return;
				}
				index++;
				Pop(states[index]);
			}

			void Pop(State s)
			{
				human.head.Center.Position = s[0];
				int i = 1;
				foreach (var item in human.arms)
				{
					item.Joint.Position = s[i];
					i++;
					item.Head.Position = s[i];
					i++;
				}
				human.Update();
			}

			public void Reset()
			{
				if (index >= 0)
				{
					var s = states[0];
					Push(s);
					Pop(s);
				}
			}

		}

		StateManager State;

		public Human(Windows.UI.Xaml.Controls.Panel canvas)
		{
			this.canvas = canvas;

			body = new Body();
			head = new Head(body.Center, new Point(0, -90), this);
			leftArm = new Arm(body.Center, new Point(-30, -80), Util.FromPolar(50, Math.PI * 0.7), Util.FromPolar(50, Math.PI), this);
			rightArm = leftArm.Mirror();
			leftLeg = new Arm(body.Center, new Point(0, 0), Util.FromPolar(60, Math.PI * 0.6), Util.FromPolar(50, Math.PI * 0.5), this);
			rightLeg = leftLeg.Mirror();
			arms = new[]{leftArm, leftLeg, rightArm, rightLeg};

			bodyShape = new Polygon()
			{
				Points = new Windows.UI.Xaml.Media.PointCollection() { leftArm.Root.GetAbsolutePosition(), rightArm.Root.GetAbsolutePosition(), leftLeg.Root.GetAbsolutePosition() }
			};
			canvas.Children.Add(bodyShape);
			AddShapes(head.Shapes);
			head.UpdateShape();
			foreach (var arm in arms)
			{
				AddShapes(arm.Shapes);
			}
			State = new StateManager(this);
			State.Push();
		}

		void AddShapes(IEnumerable<Shape> shapes)
		{
			foreach (var item in shapes)
			{
				canvas.Children.Add(item);
			}
		}

		void Update()
		{
			head.UpdateShape();
			foreach (var item in arms)
			{
				item.UpdateShape();
			}
		}

		public void Push()
		{
			State.Push();
		}

		public void Undo()
		{
			State.Undo();
		}

		public void Redo()
		{
			State.Redo();
		}

		public void Reset()
		{
			State.Reset();
		}
	}
}
