using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PoseUI
{
	public class Joint
	{
		public Point Position;
		public Joint Parent { get; private set; }
		List<Joint> Children;
		double Length;

		public Joint()
		{
			Children = new List<Joint>();
		}

		public Joint(Point p)
			: this()
		{
			Position = p;
			Length = Math.Sqrt(Position.X * Position.X + Position.Y * Position.Y);
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
}
