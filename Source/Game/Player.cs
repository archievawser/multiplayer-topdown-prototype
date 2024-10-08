using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Rabid.Netcode.Steam;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rabid
{
	[RegisterEntityType(name: "Player")]
	class Player : Pawn
	{
		public override void Prepare()
		{
			Trace.WriteLine("Player created");
			Sprite = new SpriteComponent(new GridCoord(0, 0), new GridCoord(1, 2));
			Transform = new TransformComponent();

			AddComponent(Sprite);
			AddComponent(Transform);

			base.Prepare();
		}

		public override void Update(float dt)
		{
			if (!IsLocallyOwned)
				return;

			Vector2 direction;
			direction.X = (Input.IsKeyDown(Keys.D) ? 1 : 0) - (Input.IsKeyDown(Keys.A) ? 1 : 0);
			direction.Y = (Input.IsKeyDown(Keys.W) ? 1 : 0) - (Input.IsKeyDown(Keys.S) ? 1 : 0);

			float dirLengthSqr = direction.LengthSquared();

			if(dirLengthSqr != 0)
			{
				direction /= (float)Math.Sqrt(dirLengthSqr);
			}

			if(direction.X > 0)
			{
				Sprite.SetTexture(Tileset.Get(0).Elements["PlayerIdleRight"]);
			}
			else if(direction.X < 0)
			{
				Sprite.SetTexture(Tileset.Get(0).Elements["PlayerIdleLeft"]);
			} 
			else if(direction.Y > 0)
			{
				Sprite.SetTexture(Tileset.Get(0).Elements["PlayerIdleUp"]);
			}
			else if(direction.Y < 0)
			{
				Sprite.SetTexture(Tileset.Get(0).Elements["PlayerIdleDown"]);
			}
			
			Transform.Position += direction * dt * 50f;
			Camera.Current.Transform.Translation = Vector3.Lerp(Camera.Current.Transform.Translation, -new Vector3(Transform.Position, 0), 10f * dt);

			if(World.Instance.Networker.Role == NetRole.Host)
			{
				foreach (var v in World.Instance.Networker.Connections.Keys)
				{
					BroadcastPosition(v, Transform.Position);
				}
			}
			else
			{
				SetServerPosition(Transform.Position);
			}

			base.Update(dt);
		}

		[RunOnServer]
		public void SetServerPosition(Vector2 position) { }
		public void SetServerPosition_Impl(SteamId sender, NetBinaryReader data)
		{
			Transform.Position = data.ReadVector2();

			foreach(var v in World.Instance.Networker.Connections.Keys)
			{
				if (v == sender)
					continue;

				BroadcastPosition(v, Transform.Position);
			}
			//BroadcastPosition(Transform.Position);
		}

		[RunOnClient]
		public void BroadcastPosition(SteamId target, Vector2 position) { }
		public void BroadcastPosition_Impl(NetBinaryReader data) 
		{
			Transform.Position = data.ReadVector2();
		}


		
		public TransformComponent Transform;
		public SpriteComponent Sprite;
	}
}
