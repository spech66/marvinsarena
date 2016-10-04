#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngineCommon
{
	public enum GameObjectsType
	{
		Robot,
		Bullet,
		Missile,
		Wall
	}

	public class Hud
	{
		private static string systemId = "SYSTEM:";

		private SpriteFont fontSmall;
		private SpriteFont font;
		private Vector2 fontCenterScreenPosition;

		private DateTime lastAdd;

		private Vector2 positionRadar;
		private Texture2D textureRadar;
		private Vector2 positionRadarBar;
		private Texture2D textureRadarBar;
		private Texture2D textureRadarRobot;
		private Texture2D textureRadarWall;

		private Vector2 positionMissileFont;
		private Vector2 positionStaus;
		private Texture2D textureOuter;
		private Texture2D textureMissile;
		private Texture2D textureHitpoints;
		private Texture2D textureHeat;

		private int messageStartPosition;
		private List<string> messages;
		private string statusMessage;

		public bool StatusMessageActive { get; set; }
		public string StatusMessage
		{
			get { return statusMessage; }
			set
			{
				statusMessage = value;
				StatusMessageActive = true;
			}
		}

		public SpriteFont FontSmall { get { return fontSmall; } }

		public Hud()
		{
			messages = new List<string>();
			lastAdd = DateTime.Now;
			StatusMessageActive = false;
		}

		/// <summary>
		/// Remove all entries
		/// </summary>
		public void Clear()
		{
			messages.Clear();
		}

		public void AddMessage(string message, bool system)
		{
			lastAdd = DateTime.Now;

			string[] messageList = message.Split(new char[] { '\n' });
			foreach (string msg in messageList)
			{
				if (system)
				{
					messages.Add(String.Format("{0}{1}", systemId, msg));
				}
				else
				{
					messages.Add(msg);
				}
			}
		}

		public void AddMessage(string message)
		{
			AddMessage(message, false);
		}

		public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
		{
			messageStartPosition = graphicsDevice.Viewport.Height - 11 * 14; // 10 + 1 entries, 14 pixel each

			//LoadTexture for radar
			textureRadar = content.Load<Texture2D>("Hud/Radar");
			positionRadar = new Vector2(graphicsDevice.Viewport.Width - 128, 0);
			textureRadarBar = content.Load<Texture2D>("Hud/RadarBar");
			positionRadarBar = new Vector2(positionRadar.X + textureRadarBar.Width / 2.0f,
				positionRadar.Y + textureRadarBar.Height / 2.0f);
			//Load texture for radar objects
			textureRadarRobot = content.Load<Texture2D>("Hud/RadarTarget_Robot");
			textureRadarWall = content.Load<Texture2D>("Hud/RadarTarget_Wall");

			positionMissileFont = new Vector2(64, 64);
			positionStaus = new Vector2(0, 0);
			textureOuter = content.Load<Texture2D>("Hud/Outer");
			textureMissile = content.Load<Texture2D>("Hud/Missiles");
			textureHitpoints = content.Load<Texture2D>("Hud/Hitpoints");
			textureHeat = content.Load<Texture2D>("Hud/Heat");

			// Load Font
			fontSmall = content.Load<SpriteFont>("Hud/smallfont");
			font = content.Load<SpriteFont>("menufont");
			fontCenterScreenPosition = new Vector2(graphicsDevice.Viewport.Width / 2,
				graphicsDevice.Viewport.Height / 2);
		}

		public void Update()
		{
			if (messages.Count > 0 && (DateTime.Now - lastAdd).TotalSeconds > 4)
			{
				lastAdd = DateTime.Now;
				messages.RemoveAt(0);
			}

			while (messages.Count > 10)
				messages.RemoveAt(0);
		}

		public void DrawMessages(SpriteBatch spriteBatch)
		{
			int i = messageStartPosition + (10 - messages.Count) * 14;
			foreach (string entry in messages)
			{
				if (entry.Contains(systemId))
				{
					string newentry = entry.Replace(systemId, "");
					spriteBatch.DrawString(fontSmall, newentry, new Vector2(10, i), Color.Red,
						0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);
				}
				else
				{
					spriteBatch.DrawString(fontSmall, entry, new Vector2(10, i), Color.White,
						0, new Vector2(), 1.0f, SpriteEffects.None, 0.5f);
				}
				i += 14;
			}

			// Status message
			if (StatusMessageActive)
			{
				Vector2 fontOriginStatusMessage = font.MeasureString(statusMessage) / 2;
				spriteBatch.DrawString(font, statusMessage, fontCenterScreenPosition, Color.White,
					0, fontOriginStatusMessage, 1.0f, SpriteEffects.None, 0.5f);
			}
		}

		public void Draw(SpriteBatch spriteBatch, GameObjectRobot robot, TournamentRules rules)
		{
			spriteBatch.Draw(textureRadar, positionRadar, Color.White);
			spriteBatch.Draw(textureRadarBar, positionRadarBar, null, Color.White, (float)robot.RotationRadar,
				new Vector2(textureRadarBar.Width / 2.0f, textureRadarBar.Height / 2.0f),
				1.0f, SpriteEffects.None, 0.5f);
			spriteBatch.Draw(textureMissile, positionStaus, Color.White);

			//spriteBatch.Draw(textureHitpoints, positionStaus, Color.White);
			int hpheight = (int)(robot.Hitpoints * 1.0f / rules.Hitpoints * textureHitpoints.Height);
			spriteBatch.Draw(textureHitpoints,
				new Rectangle(0, textureHitpoints.Height - hpheight, textureHitpoints.Width, hpheight),
				new Rectangle(0, textureHitpoints.Height - hpheight, textureHitpoints.Width, hpheight), Color.White);

			//spriteBatch.Draw(textureHeat, positionStaus, Color.White);
			int h = (int)(robot.Heat * 1.0f / rules.MaxHeat * textureHeat.Height);
			spriteBatch.Draw(textureHeat,
				new Rectangle(0, textureHeat.Height - h, textureHeat.Width, h),
				new Rectangle(0, textureHeat.Height - h, textureHeat.Width, h), Color.White);

			string missileCount = robot.Missiles.ToString(CultureInfo.InvariantCulture);
			Vector2 fontOriginMissile = font.MeasureString(missileCount) / 2;
			spriteBatch.DrawString(font, missileCount, positionMissileFont, Color.White,
				0, fontOriginMissile, 1.0f, SpriteEffects.None, 0.5f);
		}


		public void DrawObjectsOnRadar(SpriteBatch spriteBatch, GameObjectRobot scanner, Vector2 target, float mapScale, GameObjectsType objectType)
		{
			Texture2D drawingTexture = null;
			//Set drawing texture and map accuracy
			switch (objectType)
			{
				case GameObjectsType.Robot:
					drawingTexture = textureRadarRobot; 
					target.X = (float)(Math.Ceiling(target.X / mapScale) * mapScale);
					target.Y = (float)(Math.Ceiling(target.Y / mapScale) * mapScale);
					break;
				case GameObjectsType.Bullet:
					break;
				case GameObjectsType.Missile:
					break;
				case GameObjectsType.Wall:
					drawingTexture = textureRadarWall;
					target.X *= mapScale;
					target.Y *= mapScale;
					break;
				default:
					break;
			}

			double angleBetweenObjects = Math.Atan2(target.Y - scanner.PositionY, target.X - scanner.PositionX);
			double destinationToObject = Math.Sqrt(Math.Pow(target.Y - scanner.PositionY, 2) + Math.Pow(target.X - scanner.PositionX, 2));

			//Zone radar vision
			float radarVision = 7 * mapScale;
			double CofScaleRadar = (textureRadar.Height / 2.0f) / radarVision;
			float sizeObjectOnRadar = (float)(mapScale * CofScaleRadar);


			 if (radarVision > Math.Floor(destinationToObject + (sizeObjectOnRadar / CofScaleRadar)/2))
			{

				destinationToObject *= CofScaleRadar;

				Vector2 positionOnRadar = new Vector2();


				positionOnRadar.X = positionRadar.X + textureRadar.Width / 2 - sizeObjectOnRadar + (float)(destinationToObject * Math.Cos(angleBetweenObjects));
				positionOnRadar.Y = positionRadar.Y + textureRadar.Height / 2 - sizeObjectOnRadar + (float)(destinationToObject * Math.Sin(angleBetweenObjects));

				Rectangle ScaleObject = new Rectangle(Convert.ToInt32(positionOnRadar.X), Convert.ToInt32(positionOnRadar.Y),
					Convert.ToInt32(sizeObjectOnRadar), Convert.ToInt32(sizeObjectOnRadar));

				spriteBatch.Draw(drawingTexture, ScaleObject, Color.White);
				spriteBatch.Draw(textureOuter, positionRadar, Color.White);
			}

		}


	}
}
