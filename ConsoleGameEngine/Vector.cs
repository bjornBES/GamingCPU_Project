using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine {
	/// <summary> Vector of two floats. </summary>
	public struct Vector {
		public int X { get; set; }
		public int Y { get; set; }


		public static Vector Zero { get; private set; } = new Vector(0, 0);
		public Vector(int x, int y) {
			this.X = x;
			this.Y = y;
		}

		public void Rotate(int a) {
			Vector n = Vector.Zero;

			n.X = (int)(X * Math.Cos(a / 57.3f) - Y * Math.Sin(a / 57.3f));
			n.Y = (int)(X * Math.Sin(a / 57.3f) + Y * Math.Cos(a / 57.3f));

			X = n.X;
			Y = n.Y;
		}

		public static Vector operator + (Vector a, Vector b) {
			return new Vector(a.X + b.X, a.Y + b.Y);
		}
		public static Vector operator - (Vector a, Vector b) {
			return new Vector(a.X - b.X, a.Y - b.Y);
		}

		public static Vector operator / (Vector a, int b) {
			return new Vector((a.X / b), (a.Y / b));
		}
		public static Vector operator * (Vector a, int b) {
			return new Vector((a.X * b), (a.Y * b));
		}

		public static int Distance(Vector a, Vector b) {
			Vector dV = b - a;
            int d = (int)Math.Sqrt(Math.Pow(dV.X, 2) + Math.Pow(dV.Y, 2));
			return d;
		}
	}
}
