using Breakout.Components;
using Breakout.Scenes;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Breakout.Scenes.PlayingScene;

namespace Breakout.Entities
{
    internal class Paddle : Entity
	{
		private Vector2 rectangleSize;
		private InputManager inputManager;
		private PaddleColliders paddleColliders;
		private CollisionManager collisionManager;
		private BoxCollider2D gameWorldBounds;

		public enum LaunchSection
		{
			OuterLeft,
			MiddleLeft,
			Center,
			MiddleRight,
			OuterRight
		}


		public Paddle(BoxCollider2D gameWorldBounds, Game game) : base(game)
		{

			rectangleSize = new Vector2(90, 20);

			this.LoadRectangleComponents("paddle", (int)rectangleSize.X, (int)rectangleSize.Y, Color.Azure, game, true);


			this.gameWorldBounds = gameWorldBounds;

		}

		public override void Initialize()
		{
			base.Initialize();


			GetComponent<Origin>().Value = GetComponent<RectangleRenderer>().TextureCenter;
			//AddComponent(new Origin(GetComponent<RectangleRenderer>().TextureCenter, this));


			paddleColliders = new PaddleColliders(this, 5, rectangleSize);
			AddComponent(paddleColliders);



			inputManager = Game.Services.GetService<InputManager>();


			collisionManager = Game.Services.GetService<CollisionManager>();

			TrackPosition();
		}



		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			TrackPosition();


		}

		public void TrackPosition()
		{
			var mousePosition = inputManager.MousePosition(); //Get the mouse position

			//Keep the padding on screen even when the curor goes out of screen
			var clampedXAxis = Math.Clamp(mousePosition.X, rectangleSize.X / 1.9f, gameWorldBounds.Bounds.Width - rectangleSize.X / 1.9f);


			//Keep the paddle on the bottom of the screen
			var constrainedYAxis = gameWorldBounds.Bounds.Height - rectangleSize.Y / 1.25f;

			Transform.LocalPosition = new Vector2(clampedXAxis, constrainedYAxis);
		}

		public void AttachBall(Ball ball)
		{
			ball.SetParent(this);
			ball.Transform.LocalPosition = new Vector2(0, -18);
		}

		public void DetachBall(Ball ball, LaunchSection launchSection)
		{
			if (ball != null)
			{
				ball.SetParent(null);

				ball.ReflectBallFromSection(launchSection, this);
			}
		}

		public void PaddleSectionCollision(Ball ball)
		{
			LaunchSection[] enumValues = (LaunchSection[])Enum.GetValues(typeof(LaunchSection));

			for (int i = 0; i < paddleColliders.SectionCount; i++)
			{
				if (collisionManager.ShapesIntersect(ball.GetComponent<CircleCollider2D>(), paddleColliders.GetSection(i)))
				{
					ball.ReflectBallFromSection(enumValues[i], this);
					//break;
				}
			}

			if (enumValues.Length != paddleColliders.SectionCount)
			{
				throw new Exception("quanity and sequence of enum elements needs to correspond with the quanity and sequence of paddle colliders");
			}

		}

		public override void Reset()
		{
			base.Reset();

			TrackPosition();
		}
	}
}
