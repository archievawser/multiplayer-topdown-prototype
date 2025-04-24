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

			Sprite.SetTexture("PlayerIdleDown");

			base.Prepare();
		}

		public override void Update(float dt)
		{
			// pushed to top so that components update properly
			base.Update(dt);

			if (!IsLocallyOwned)
				return;

			Vector2 direction;
			direction.X = (Input.IsKeyDown(Keys.D) ? 1 : 0) - (Input.IsKeyDown(Keys.A) ? 1 : 0);
			direction.Y = (Input.IsKeyDown(Keys.W) ? 1 : 0) - (Input.IsKeyDown(Keys.S) ? 1 : 0);

			float dirLengthSqr = direction.LengthSquared();

			if(dirLengthSqr != 0)
			{
				direction /= (float)Math.Sqrt(dirLengthSqr);
				mLastNonZeroDirection = direction;
			}

			if(direction.X != 0)
			{
				Sprite.SetAnimation(direction.X > 0 ? "PlayerWalkRight" : "PlayerWalkLeft", false);
				ReplicateAppearance((byte)AppearanceTarget.Animation, direction.X > 0 ? "PlayerWalkRight" : "PlayerWalkLeft");
			}
			else
			{
				if(direction.Y == 0)
				{
					Sprite.SetTexture(mLastNonZeroDirection.X > 0 ? "PlayerIdleRight" : "PlayerIdleLeft");
					ReplicateAppearance((byte)AppearanceTarget.Texture, mLastNonZeroDirection.X > 0 ? "PlayerIdleRight" : "PlayerIdleLeft");
				}
			}

			if (direction.Y != 0)
			{
				Sprite.SetAnimation(direction.Y > 0 ? "PlayerWalkUp" : "PlayerWalkDown", false);
				ReplicateAppearance((byte)AppearanceTarget.Animation, direction.Y > 0 ? "PlayerWalkUp" : "PlayerWalkDown");
			}
			else
			{
				if(mLastNonZeroDirection.X == 0)
				{
					Sprite.SetTexture(mLastNonZeroDirection.Y > 0 ? "PlayerIdleUp" : "PlayerIdleDown");
					ReplicateAppearance((byte)AppearanceTarget.Texture, mLastNonZeroDirection.Y > 0 ? "PlayerIdleUp" : "PlayerIdleDown");
				}
			}
			
			Transform.Position += direction * dt * 80f;
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
		}

		[RunOnClient]
		public void BroadcastPosition(SteamId target, Vector2 position) { }
		public void BroadcastPosition_Impl(NetBinaryReader data) 
		{
			Transform.Position = data.ReadVector2();
		}

		public enum AppearanceTarget : byte
		{ 
			Animation, Texture 
		};


		public void ReplicateAppearance(byte target, string value)
		{
			// If we're already on the server, just replicate straight to clients.
			if(World.Instance.Networker.Role != NetRole.Client)
			{
				foreach (var v in World.Instance.Networker.Connections.Keys)
				{
					if (v == SteamClient.SteamId)
						continue;

					SetClientAppearance(v, (byte)target, value);
				}

				return;
			}

			SetServerAppearance(target, value);
		}

		[RunOnServer]
		public void SetServerAppearance(byte target, string value) { }
		public void SetServerAppearance_Impl(SteamId sender, NetBinaryReader data)
		{
			AppearanceTarget target = (AppearanceTarget)data.ReadByte();
			string animation = data.ReadString();

			if (target == AppearanceTarget.Animation)
			{
				Sprite.SetAnimation(animation, false);
			}
			else
			{
				Sprite.SetTexture(animation);
			}

			foreach (var v in World.Instance.Networker.Connections.Keys)
			{
				if (v == sender)
					continue;

				SetClientAppearance(v, (byte)target, animation);
			}
		}

		[RunOnClient]
		public void SetClientAppearance(SteamId target, byte _target, string animation) { }
		public void SetClientAppearance_Impl(NetBinaryReader data)
		{
			AppearanceTarget _target = (AppearanceTarget)data.ReadByte();
			string animation = data.ReadString();

			if (_target == AppearanceTarget.Animation)
			{
				Sprite.SetAnimation(animation, false);
			}
			else
			{
				Sprite.SetTexture(animation);
			}
		}

		public TransformComponent Transform;
		public SpriteComponent Sprite;
		private Vector2 mLastNonZeroDirection;
	}
}
