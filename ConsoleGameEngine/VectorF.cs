using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine {
	/// <summary> Vector of two floats. </summary>
	public struct VectorF {
		public float X { get; set; }
		public float Y { get; set; }


		public static VectorF Zero { get; private set; } = new VectorF(0, 0);
		public VectorF(float x, float y) {
			this.X = x;
			this.Y = y;
		}

		public Point ToPoint => new Point((int)Math.Round(X, 0), (int)Math.Round(Y, 0));

		public void Rotate(float a) {
			VectorF n = VectorF.Zero;

			n.X = (float)(X * Math.Cos(a / 57.3f) - Y * Math.Sin(a / 57.3f));
			n.Y = (float)(X * Math.Sin(a / 57.3f) + Y * Math.Cos(a / 57.3f));

			X = n.X;
			Y = n.Y;
		}

		public static VectorF operator + (VectorF a, VectorF b) {
			return new VectorF(a.X + b.X, a.Y + b.Y);
		}
		public static VectorF operator - (VectorF a, VectorF b) {
			return new VectorF(a.X - b.X, a.Y - b.Y);
		}

		public static VectorF operator / (VectorF a, float b) {
			return new VectorF((a.X / b), (a.Y / b));
		}
		public static VectorF operator * (VectorF a, float b) {
			return new VectorF((a.X * b), (a.Y * b));
		}

		public static float Distance(VectorF a, VectorF b) {
			VectorF dV = b - a;
			float d = (float)Math.Sqrt(Math.Pow(dV.X, 2) + Math.Pow(dV.Y, 2));
			return d;
		}
	}
}
