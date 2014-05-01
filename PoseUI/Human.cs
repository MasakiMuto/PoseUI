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
		struct Body
		{
			public float Rotation;
		}

		struct Head
		{
			public Point Center;
		}

		struct Arm
		{
			public Point Tail, Joint, Head;
		}

		class Joint
		{
			Point Position;
			Joint Parent;
			List<Joint> Children;
		}

		Body body;
		Head head;
		Arm leftArm, rightArm, leftLeg, rightLeg;



	}
}
