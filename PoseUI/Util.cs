using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace PoseUI
{
	static class Util
	{
		public static Point Add(this Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point Mirror(this Point p)
		{
			return new Point(-p.X, p.Y);
		}
	}
}
