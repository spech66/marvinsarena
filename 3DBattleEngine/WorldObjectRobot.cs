#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine3D
{
	public class WorldObjectRobot : WorldObject
	{
		#region Fields

		// Shortcut references to the bones that we are going to animate.
		// We could just look these up inside the Draw method, but it is more
		// efficient to do the lookups while loading and cache the results.
		ModelBone leftBackWheelBone;
		ModelBone rightBackWheelBone;
		ModelBone leftFrontWheelBone;
		ModelBone rightFrontWheelBone;
		ModelBone leftSteerBone;
		ModelBone rightSteerBone;
		ModelBone turretBone;
		ModelBone cannonBone;
		ModelBone hatchBone;
		
		// Store the original transform matrix for each animating bone.
		Matrix leftBackWheelTransform;
		Matrix rightBackWheelTransform;
		Matrix leftFrontWheelTransform;
		Matrix rightFrontWheelTransform;
		Matrix leftSteerTransform;
		Matrix rightSteerTransform;
		Matrix turretTransform;
		Matrix cannonTransform;
		Matrix hatchTransform;
		
		// Array holding all the bone transform matrices for the entire model.
		// We could just allocate this locally inside the Draw method, but it
		// is more efficient to reuse a single array, as this avoids creating
		// unnecessary garbage.
		Matrix[] boneTransforms;

		private Vector3 color;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the wheel rotation amount.
		/// </summary>
		public float WheelRotation { get; set; }
		
		/// <summary>
		/// Gets or sets the steering rotation amount.
		/// </summary>
		public float SteerRotation { get; set; }
		
		/// <summary>
		/// Gets or sets the turret rotation amount.
		/// </summary>
		public float TurretRotation { get; set; }
		
		/// <summary>
		/// Gets or sets the cannon rotation amount.
		/// </summary>
		public float CannonRotation { get; set; }
		
		/// <summary>
		/// Gets or sets the entry hatch rotation amount.
		/// </summary>
		public float HatchRotation { get; set; }

		#endregion

		public WorldObjectRobot(GameObject gameObject)
		{
			GameObject = gameObject;
			Scale = 0.04f;

			System.Drawing.Color sysColor = ((GameObjectRobot)gameObject).TeamColor();
			color = new Vector3(sysColor.R, sysColor.G, sysColor.B);
		}

		public override void Update(double deltaTime)
		{
			base.Update(deltaTime);

			/*if(LastPositionX != PositionX || LastPositionY != PositionY || Rotation != LastRotation)
			{
			//TODO:
				//thisrobot.WheelRotation += (float)moveFactor;
			}*/

			SteerRotation = Rotation;
			//Negate for fixing the direction
			TurretRotation = -((float)((GameObjectRobot)GameObject).RotationGun - Rotation);
			HatchRotation = (float)((GameObjectRobot)GameObject).RotationRadar - Rotation;
		}

		/// <summary>
		/// Loads the robot model.
		/// </summary>
		public override void Load(ContentManager content)
		{
			// Load the robot model from the ContentManager.
			Model = content.Load<Model>("robot");

			// Look up shortcut references to the bones we are going to animate.
			leftBackWheelBone = Model.Bones["l_back_wheel_geo"];
			rightBackWheelBone = Model.Bones["r_back_wheel_geo"];
			leftFrontWheelBone = Model.Bones["l_front_wheel_geo"];
			rightFrontWheelBone = Model.Bones["r_front_wheel_geo"];
			leftSteerBone = Model.Bones["l_steer_geo"];
			rightSteerBone = Model.Bones["r_steer_geo"];
			turretBone = Model.Bones["turret_geo"];
			cannonBone = Model.Bones["canon_geo"];
			hatchBone = Model.Bones["hatch_geo"];

			// Store the original transform matrix for each animating bone.
			leftBackWheelTransform = leftBackWheelBone.Transform;
			rightBackWheelTransform = rightBackWheelBone.Transform;
			leftFrontWheelTransform = leftFrontWheelBone.Transform;
			rightFrontWheelTransform = rightFrontWheelBone.Transform;
			leftSteerTransform = leftSteerBone.Transform;
			rightSteerTransform = rightSteerBone.Transform;
			turretTransform = turretBone.Transform;
			cannonTransform = cannonBone.Transform;
			hatchTransform = hatchBone.Transform;

			// Allocate the transform matrix array.
			boneTransforms = new Matrix[Model.Bones.Count];

			base.Load(content);
		}

		/// <summary>
		/// Draws the robot model, using the current animation settings.
		/// </summary>
		public override void Draw(Matrix world, Matrix view, Matrix projection)
		{
			// Default Model rotation is not zero deg so add 270deg
			Matrix world2 = Matrix.CreateScale(Scale) * Matrix.CreateRotationY((MathHelper.PiOver2) - Rotation) * Matrix.CreateTranslation(PositionX, 0, PositionZ);
			Model.Root.Transform = world2;

			// Calculate matrices based on the current animation position.
			Matrix wheelRotation = Matrix.CreateRotationX(WheelRotation);
			Matrix steerRotation = Matrix.CreateRotationY(SteerRotation);
			Matrix turretRotation = Matrix.CreateRotationY(TurretRotation);
			Matrix cannonRotation = Matrix.CreateRotationX(CannonRotation);
			//Multiply with negativ turretRotation because hatch is attached to it
			Matrix hatchRotation = Matrix.CreateRotationX(0.9f) * Matrix.CreateRotationY((MathHelper.PiOver2) - HatchRotation) * Matrix.CreateRotationY(((MathHelper.PiOver2) - TurretRotation) * -1.0f);

			// Apply matrices to the relevant bones.
			leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
			rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
			leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
			rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
			leftSteerBone.Transform = steerRotation * leftSteerTransform;
			rightSteerBone.Transform = steerRotation * rightSteerTransform;
			turretBone.Transform = turretRotation * turretTransform;
			cannonBone.Transform = cannonRotation * cannonTransform;
			hatchBone.Transform = hatchRotation * hatchTransform;

			// Look up combined bone matrices for the entire model.
			Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

			// Draw the model.
			foreach(ModelMesh mesh in Model.Meshes)
			{
				foreach(BasicEffect effect in mesh.Effects)
				{
					effect.SpecularColor = color;
					effect.World = boneTransforms[mesh.ParentBone.Index];
					effect.View = view;
					effect.Projection = projection;

					effect.EnableDefaultLighting();
				}

				mesh.Draw();
			}
		}
	}
}
