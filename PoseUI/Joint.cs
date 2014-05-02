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
		Point position;
		public Point Position 
		{
			get { return position; }
			set 
			{
				if (!ConstrainLength)
				{
					position = value;
				}
				else
				{
					var a = Math.Atan2(value.Y, value.X);
					position = Util.FromPolar(Length, a);
				}
			}
		}
		public Joint Parent { get; private set; }
		List<Joint> Children;
		double Length;
		public bool ConstrainLength { get; set; }

		public Joint()
		{
			Children = new List<Joint>();
			ConstrainLength = false;
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
