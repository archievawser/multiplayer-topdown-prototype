using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
    class Camera
    {
        public Camera(int windowWidth, int windowHeight)
		{
			const int zoom = 5;
			windowWidth /= zoom;
			windowHeight /= zoom;

			float halfWidth = windowWidth / 2f;
			float halfHeight = windowHeight / 2f;
			mWindowWidth = windowWidth;
			mWindowHeight = windowHeight;
			Projection = Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, -halfHeight, halfHeight, 0f, 1f);
			Current = this;
		}

		public Rectangle GetFrustum()
		{
			return new Rectangle(0, 0, 100, 100);//-(int)(Transform.Translation.X + mWindowWidth / 2), (int)(Transform.Translation.Y + mWindowHeight / 2), mWindowWidth / 2, mWindowHeight / 2);
		}

		public static Camera Current;
		public Matrix Projection;
        public Matrix Transform = Matrix.Identity;
		private int mWindowWidth;
		private int mWindowHeight;
    }
}
