using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	public class InputComponent : Component
	{
		public override void PreUpdate()
		{
			mKeyboardState = Keyboard.GetState();
			mMouseState = Mouse.GetState();

			base.PreUpdate();
		}

		public bool IsKeyDown(Keys key)
		{
			return mKeyboardState.IsKeyDown(key);
		}

		public bool IsLeftMouseButtonDown()
		{
			return mMouseState.LeftButton == ButtonState.Pressed;
		}

		public bool IsRightMouseButtonDown()
		{
			return mMouseState.RightButton == ButtonState.Pressed;
		}

		public Vector2 MousePosition
		{
			get
			{
				return mMouseState.Position.ToVector2();
			}
		}

		private KeyboardState mKeyboardState;
		private MouseState mMouseState;
	}
}
