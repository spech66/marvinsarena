using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MarvinsArena.Core;
using BattleEngineCommon;

namespace BattleEngine3D
{
	/// <summary>
	/// This is the main MsgExchangeType for Marvins Arena
	/// </summary>
	public class BattleEngine : Game
	{
		#region Fields
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private DepthStencilState depthStencilState;
		private BlendState blendState;

		private bool leftMouseButtonDown;
		private int cameraTargetIndex, cameraOverviewScaleFactor;
		private ArcBallCamera camera;
		private Matrix world, view, projection;
		
		private List<WorldObject> robots = new List<WorldObject>();
		private List<WorldObject> projectiles = new List<WorldObject>();
		private Theme theme;
		private Sky sky;
		private Map map;
		private Hud hud;
		private int round;
		private Scorekeeper scoreKeeper;
		private ViewSelectionMenu viewSelectionMenu;
		#endregion

		/// <summary>
		/// The main entry point for the Marvins Arena game
		/// </summary>
		public BattleEngine()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = Properties.Settings.Default.PreferredBackBufferWidth;
			graphics.PreferredBackBufferHeight = Properties.Settings.Default.PreferredBackBufferHeight;
			graphics.IsFullScreen = Properties.Settings.Default.IsFullScreen;
			graphics.PreferMultiSampling = Properties.Settings.Default.PreferMultiSampling;
			graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
			graphics.ApplyChanges();

			depthStencilState = GraphicsDevice.DepthStencilState;
			blendState = GraphicsDevice.BlendState;

			cameraTargetIndex = -1;
		}

		/// <summary>
		/// Prepare device settings. Check for multi sampling
		/// </summary>
		/// <param name="sender">Event source</param>
		/// <param name="e">Event arguments</param>
		private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			bool notFullscreen = !graphics.IsFullScreen;			
			graphics.PreferMultiSampling = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Randomly select theme from enumeration
		/// </summary>
		/// <returns></returns>
		private void SelectTheme()
		{
			Random rand = new Random();
			int elements = Enum.GetValues(typeof(Theme)).Length;
			theme = (Theme)rand.Next(0, elements);
		}

		private void InitializeCamera()
		{
			float aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
			float fov = MathHelper.PiOver4 * aspectRatio * 3 / 4;
			projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, .1f, 2000f);
			world = Matrix.Identity;

			camera = new ArcBallCamera(ArcBallCameraMode.RollConstrained);
			camera.Distance = 450;
			//orbit the camera so we're looking down the z=-1 axis
			//the acr-ball camera is traditionally oriented to look
			//at the "front" of an object
			camera.OrbitRight(MathHelper.Pi);
			camera.OrbitUp(1.0f);
			camera.InputDistanceRate = 100;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			InitializeCamera();

			// Select Theme
			SelectTheme();

			// Create map from tournament
			map = new Map(Program.Tournament.Map);
			map.LoadGraphicsContent(GraphicsDevice, Content, theme);
			map.ProjectionMatrix = projection;
			map.WorldMatrix = Matrix.Identity;

			// Laod sky
			sky = new Sky();
			sky.Load(Content, theme);
			
			// Laod hud
			hud = new Hud();
			hud.LoadContent(Content, GraphicsDevice);

			// Initialize ScoreKeeper
			scoreKeeper = new Scorekeeper();

			// Initialize MenuManager
			viewSelectionMenu = new ViewSelectionMenu();
			viewSelectionMenu.LoadContent(Content, GraphicsDevice);

			// Set up first round
			InitializeRound();
		}

		/// <summary>
		/// Set everything required for a new round
		/// </summary>
		private void InitializeRound()
		{
			round++;

			// Clear lists
			projectiles.Clear();
			robots.Clear();
			//hud.Clear();
			hud.AddMessage("New Round: " + round, true);
			foreach (KeyValuePair<string, int> score in scoreKeeper.GetAllScores())
			{
				hud.AddMessage(String.Format("Team {0} has {1,3} Point{2}", score.Key, score.Value, score.Value != 1 ? "s" : ""));
			}

			// Get all game objects from manager
			Collection<GameObjectRobot> gameObjectRobots = Program.Tournament.RobotManager.CreateGameObjects(Program.Tournament);
			foreach (GameObjectRobot gorobot in gameObjectRobots)
			{
				// Set position to free map area
				map.PlaceRobot(gorobot);
				// Set round values after positioning the object so positions are correct for notification event
				gorobot.SetRoundValues(Program.Tournament.Rules);
				// create world object and load model
				WorldObjectRobot worldObject = new WorldObjectRobot(gorobot);
				worldObject.Load(Content);
				// Assign all event handler
				((GameObjectRobot)worldObject.GameObject).HandleRobotEvents += RobotCore_HandleRobotEvents;
				// Add robot to the list
				robots.Add(worldObject);
			}

			// Place camera to see whole map
			cameraTargetIndex = -1;
			if (map.MapHeight > map.MapWidth)
				cameraOverviewScaleFactor = map.MapHeight;
			else
				cameraOverviewScaleFactor = map.MapWidth;

			if(round == 1)
				hud.StatusMessage = String.Format("Ready...\n\nRound {0} of {1}", round, Program.Tournament.Rules.Rounds);
			else
				hud.StatusMessage = String.Format("{0}\n\nRound {1} of {2}", hud.StatusMessage, round, Program.Tournament.Rules.Rounds);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			Content.Unload();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			double time = gameTime.ElapsedGameTime.TotalMilliseconds;
			KeyboardState currentKeyboardState = Keyboard.GetState();
			MouseState currentMouseState = Mouse.GetState();

			if(currentKeyboardState.IsKeyDown(Keys.Escape))
				Exit();

			if(hud.StatusMessageActive && (currentKeyboardState.IsKeyDown(Keys.Space) || currentKeyboardState.IsKeyDown(Keys.Enter)))
			{
				hud.StatusMessageActive = false;

				var teamsExit = from robot in robots
								group robot by ((GameObjectRobot)robot.GameObject).Team into robotGroup
								select robotGroup.Key;
				int teamsCountExit = teamsExit.Count();
				if (round == Program.Tournament.Rules.Rounds && (robots.Count < 2 || teamsCountExit < 2))
					Exit();
			}

			if(!IsActive || hud.StatusMessageActive)
				return;

			// -- Game is active --------------------------------------------------------
			if(currentMouseState.LeftButton == ButtonState.Released)
				leftMouseButtonDown = false;

			if(currentMouseState.LeftButton == ButtonState.Pressed && leftMouseButtonDown == false)
			{
				cameraTargetIndex++;
				if(cameraTargetIndex >= robots.Count)
					cameraTargetIndex = -1;

				leftMouseButtonDown = true;
			}

			camera.HandleDefaultKeyboardControls(currentKeyboardState, gameTime);

			// Set view matrix
			view = camera.ViewMatrix;
			map.ViewMatrix = camera.ViewMatrix;

			#region Move objects
			// Move robots and execute plugin
			foreach(WorldObject obj in robots)
			{
				obj.Update(time);
			}

			// Move missile and bullets
			foreach(WorldObject obj in projectiles)
			{
				if(obj is WorldObjectMissile)
				{
					obj.Update(time);
				} else if(obj is WorldObjectBullet)
				{
					obj.Update(time);
				}
			}
			#endregion

			#region Assign camera to target
			if (cameraTargetIndex != -1 && cameraTargetIndex < robots.Count)
			{
				Vector3 tempcv = camera.Target;
				tempcv.X = robots[cameraTargetIndex].PositionX;
				tempcv.Y = robots[cameraTargetIndex].PositionY;
				tempcv.Z = robots[cameraTargetIndex].PositionZ;
				camera.Target = tempcv;
				camera.Distance = 200;
			} else {
				cameraTargetIndex = -1;
				camera.Target = new Vector3((map.MapWidth + 1) / 2 * map.MapScale, 0, (map.MapHeight + 1) / 2 * map.MapScale);
				camera.Distance = map.MapScale * cameraOverviewScaleFactor * 1.3f;
			}
			#endregion

			#region Collision detection
			// Collision: robot vs Walls
			foreach(WorldObject robot1 in robots)
			{
				if(map.CheckCollision(robot1))
				{
					MarvinsArena.Robot.Core.Collision collision1 =
						new MarvinsArena.Robot.Core.Collision("Wall", 1,
							MarvinsArena.Robot.Core.MessageExchangeType.Wall
						);
					((GameObjectRobot)robot1.GameObject).NotifyCollision(collision1);
					// Add status message to HUD
					hud.AddMessage(String.Format("{0} hit a wall.", robot1.GameObject.FullName));
				}
			}

			// Collision: Projectile vs Walls
			foreach(WorldObject projectile1 in projectiles)
			{
				if(map.CheckCollision(projectile1))
				{
					projectile1.Dead = true;
				}
			}

			// Collision: robot vs robot and SCAN
			foreach(WorldObject robot1 in robots)
			{
				foreach(WorldObject robot2 in robots)
				{
					if(robot1 == robot2)
						continue;

					if(robot1.CheckForCollisions(robot2))
					{
						MarvinsArena.Robot.Core.Collision collision1 = 
							new MarvinsArena.Robot.Core.Collision(
								robot2.GameObject.FullName, 1,
								MarvinsArena.Robot.Core.MessageExchangeType.Robot
							);
						((GameObjectRobot)robot1.GameObject).NotifyCollision(collision1);

						MarvinsArena.Robot.Core.Collision collision2 =
							new MarvinsArena.Robot.Core.Collision(
								robot1.GameObject.FullName, 1,
								MarvinsArena.Robot.Core.MessageExchangeType.Robot
							);
						((GameObjectRobot)robot2.GameObject).NotifyCollision(collision2);

						// Add status message to HUD
						hud.AddMessage(robot1.GameObject.FullName + " bumped " + robot2.GameObject.FullName);
					}

					if(GameObject.ObjectInScannerRange(((GameObjectRobot)robot1.GameObject), robot2.GameObject))
					{
						MarvinsArena.Robot.Core.ScannerTarget target =
							new MarvinsArena.Robot.Core.ScannerTarget(
								robot2.PositionX, robot2.PositionZ, robot2.GameObject.FullName,
								MarvinsArena.Robot.Core.MessageExchangeType.Robot
							);
						((GameObjectRobot)robot1.GameObject).NotifyTarget(target);
					}
				}
			}

			// Collision: robot vs Projectile
			foreach(WorldObject robot1 in robots)
			{
				foreach(WorldObject projectile1 in projectiles)
				{
					if(!projectile1.Dead && robot1.CheckForCollisions(projectile1))
					{
						MarvinsArena.Robot.Core.Collision collision;
						if(projectile1 is WorldObjectBullet)
						{
							collision = new MarvinsArena.Robot.Core.Collision(
								projectile1.GameObject.Name,
								((GameObjectBullet)projectile1.GameObject).Power,
								MarvinsArena.Robot.Core.MessageExchangeType.Bullet
							);
						} else //if (projectile1 is WorldObjectMissile)
						{
							collision = new MarvinsArena.Robot.Core.Collision(
								projectile1.GameObject.Name, 
								((GameObjectMissile)projectile1.GameObject).Power,
								MarvinsArena.Robot.Core.MessageExchangeType.Missile
							);
						}
						((GameObjectRobot)robot1.GameObject).NotifyCollision(collision);
						projectile1.Dead = true;

						// Add status message to HUD
						hud.AddMessage(robot1.GameObject.FullName + " was hit by a " + 
							collision.Source + " and got " + collision.Damage + " damage");
					}
				}
			}

			// Collision: Projectile vs Projectile
			foreach(WorldObject projectile1 in projectiles)
			{
				foreach(WorldObject projectile2 in projectiles)
				{
					if(projectile1 == projectile2 || projectile1.Dead || projectile2.Dead)
						continue;

					if(projectile1.CheckForCollisions(projectile2))
					{
						projectile1.Dead = true;
						projectile2.Dead = true;
					}
				}
			}

			projectiles.RemoveAll(obj => obj.Dead == true);

			foreach(WorldObject robot in robots)
			{
				((GameObjectRobot)robot.GameObject).Notify();
			}
			
			var deadrobotlist = from robot in robots
								where ((GameObjectRobot)robot.GameObject).Hitpoints < 1
								select robot;
			foreach(WorldObjectRobot t in deadrobotlist)
			{
				hud.AddMessage(String.Format("{0} died.", t.GameObject.FullName), true);
			}
			robots.RemoveAll(obj => ((GameObjectRobot)obj.GameObject).Hitpoints < 1);

			var disabledrobotlist = from robot in robots
									where ((GameObjectRobot)robot.GameObject).IsActive == false
									select robot;
			foreach(WorldObject t in disabledrobotlist)
			{
				hud.AddMessage(t.GameObject.FullName + " disabled: " + ((GameObjectRobot)t.GameObject).DisabledMessage, true);
			}
			robots.RemoveAll(obj => ((GameObjectRobot)obj.GameObject).IsActive == false);
			#endregion

			#region Determine winner, save tournament and inform hud
			var teams = from robot in robots
						group robot by ((GameObjectRobot)robot.GameObject).Team into robotGroup
						select robotGroup.Key;
			int teamsCount = teams.Count();

			if (robots.Count == 0 || teamsCount == 0)
			{
				hud.StatusMessage = "Draw!";
			}
			else if (robots.Count == 1 || teamsCount == 1)
			{
				hud.StatusMessage = robots[0].GameObject.FullName + " won the round!";
				scoreKeeper.AddScore(((GameObjectRobot)robots[0].GameObject).Team.ToString());				
			}

			// Check for next round or game ist over
			if (robots.Count < 2 || teamsCount < 2)
			{
				if (round == Program.Tournament.Rules.Rounds)
				{
					List<string> names = scoreKeeper.GetHighscoreName().ToList();
					if (names.Count() > 1)
					{
						hud.StatusMessage = "Game ended with Draw!";
					} else
					{
						int winnerTeam = Convert.ToInt32(names[0]);
						hud.StatusMessage = "Team " + winnerTeam + " won the game!";
						Program.Tournament.Bracket.SetWinner(winnerTeam);
						Program.Tournament.SaveToXml();
					}
				} else
				{
					InitializeRound(); // Start new round
				}
			}
			#endregion

			// Update hud
			hud.Update();

			// Base update
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			ResetStatesFor3D();

			map.Draw();

			foreach(WorldObjectRobot obj in robots)
			{
				obj.Draw(world, view, projection);
			}

			foreach(WorldObject obj in projectiles)
			{
				obj.Draw(world, view, projection);			
			}

			/*foreach(WorldObject obj in robots)
			{
				obj.RenderBoundingSphere(GraphicsDevice, view, projection, Color.Green);
			}
			foreach(WorldObject obj in projectiles)
			{
				obj.RenderBoundingSphere(GraphicsDevice, view, projection, Color.Green);
			}*/

			sky.Draw(graphics.GraphicsDevice, camera, projection);

			spriteBatch.Begin();
			// Draw robot names
			foreach(WorldObjectRobot obj in robots)
			{
				hud.DrawrobotNames(spriteBatch, obj, GraphicsDevice, camera, projection);
			}

			// Only show hud if robot is selected
			if(cameraTargetIndex != -1 && cameraTargetIndex < robots.Count)
			{
				GameObjectRobot robot = (GameObjectRobot)robots[cameraTargetIndex].GameObject;
				hud.Draw(spriteBatch, robot, Program.Tournament.Rules);

				foreach (WorldObjectRobot obj in robots)
				{
					hud.DrawrobotNames(spriteBatch, obj, GraphicsDevice, camera, projection);

					//Show walls on radar
					for (int x = 0; x < map.Heightmap.GetLength(0); x++)
					{
						for (int y = 0; y < map.Heightmap.GetLength(1); y++)
						{
							if (map.Heightmap[x, y] > 0)
							{
								hud.DrawObjectsOnRadar(spriteBatch, robot, new Vector2((float)x , (float)y), map.MapScale, GameObjectsType.Wall);
							}
						}
					}

					//Show robots on radar
					foreach (WorldObject robot2 in robots)
					{
						if (robots[cameraTargetIndex] == robot2)
							continue;

						if (GameObject.ObjectInScannerRange(robot, robot2.GameObject))
						{
							hud.DrawObjectsOnRadar(spriteBatch, robot, new Vector2((float)robot2.GameObject.PositionX, (float)robot2.GameObject.PositionY),
								map.MapScale, GameObjectsType.Robot);
						}
					}
				}
			}

			hud.DrawMessages(spriteBatch);
			//viewSelectionMenu.Draw(spriteBatch, from robot in robots select robot.GameObject as GameObjectRobot);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Set standard 3D settings
		/// From: http://blogs.msdn.com/shawnhar/archive/2006/11/13/spritebatch-and-renderstates.aspx
		/// </summary>
		private void ResetStatesFor3D()
		{
			// Mostly fine for 2D and 3D:
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.DepthStencilState = depthStencilState;

			GraphicsDevice.BlendState = blendState;

			/*GraphicsDevice.RenderState.AlphaTestEnable = true;
			GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
			GraphicsDevice.RenderState.ReferenceAlpha = 0;*/

			/*GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
			GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Clamp;*/

			/*GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Linear;
			GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Linear;
			GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Linear;*/

			/*GraphicsDevice.SamplerStates[0].MipMapLevelOfDetailBias = 0.0f;
			GraphicsDevice.SamplerStates[0].MaxMipLevel = 0;*/

			// 3D
			/*GraphicsDevice.RenderState.DepthBufferEnable = true;
			GraphicsDevice.RenderState.AlphaBlendEnable = false;
			GraphicsDevice.RenderState.AlphaTestEnable = false;*/

			// Content depending
			/*GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;*/
		}

		/// <summary>
		/// Event from robot
		/// </summary>
		/// <param name="sender">The robot raising the event</param>
		/// <param name="e">The event arguments</param>
		void RobotCore_HandleRobotEvents(object sender, MarvinsArena.Robot.Core.MessageExchangeFromRobotEventArgs e)
		{
			GameObjectRobot senderRobot = (GameObjectRobot)sender;

			if(e.FireProjectile != null)
			{
				WorldObject projectile;
				if(e.FireProjectile.FireType == MarvinsArena.Robot.Core.ProjectileType.Bullet)
				{
					projectile = new WorldObjectBullet(e.FireProjectile.Power);
				} else
				{
					projectile = new WorldObjectMissile();
				}

				projectile.Load(Content);
				projectile.PositionX = (float)senderRobot.PositionX;
				projectile.PositionZ = (float)senderRobot.PositionY;
				projectile.Rotation = (float)senderRobot.RotationGun;
				// Move away from spawn position immediately or this would collide with the spawning robot
				projectile.PositionX += 30.0f * (float)Math.Cos(projectile.Rotation);
				projectile.PositionZ += 30.0f * (float)Math.Sin(projectile.Rotation);
				projectiles.Add(projectile);
			}
			
			foreach(string msg in e.PrintMessage)
			{
				hud.AddMessage(String.Format("{0} says: {1}", senderRobot.FullName, msg));
			}

			foreach(MarvinsArena.Robot.Core.CoreMessageReceivedFromTeammateEventArgs tmsg in e.TeamMessage)
			{
				foreach(WorldObjectRobot worrobot in robots)
				{
					GameObjectRobot robot = (GameObjectRobot)worrobot.GameObject;
					if(robot.Team == tmsg.Team && robot.SquadNumber != tmsg.SquadNumber)
						robot.NotifyMessageToTeammate(tmsg);
				}
			}

			if(CoreMain.Instance.Debug)
			{
				foreach(string msg in e.DebugMessage)
				{
					hud.AddMessage(String.Format("DEBUG ({0}): {1}", senderRobot.FullName, msg));
				}
			}
		}
	}
}
