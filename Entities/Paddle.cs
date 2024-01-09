using Breakout.Components;
using Breakout.Scenes;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
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
		private Transform transform;
		private Vector2 rectangleSize;
		private DisplayManager displayManager;
		private InputManager inputManager;
		private PaddleColliders paddleColliders;
		private CollisionManager collisionManager;
		private RectangleRenderer rectangleRenderer;

		public enum LaunchSection
		{
			OuterLeft,
			MiddleLeft,
			Center,
			MiddleRight,
			OuterRight
		}


		int windowWidth, windowHeight;
		public Paddle(Game game) : base(game)
		{

			displayManager = game.Services.GetService<DisplayManager>();

			rectangleSize = new Vector2(90, 20);

			transform = new Transform(this);
			transform.LocalPosition = new Vector2(displayManager.WindowCenter.X, displayManager.Height - rectangleSize.Y);
			AddComponent(transform);

			Origin origin = new Origin(rectangleSize / 2, this);
			AddComponent(origin);

			rectangleRenderer = new RectangleRenderer("paddle", (int)rectangleSize.X, (int)rectangleSize.Y, Color.Azure, game, this);

			paddleColliders = new PaddleColliders(this, 5, rectangleSize);
			

			AddComponents(rectangleRenderer, paddleColliders);

			
			windowWidth = displayManager.Width;
			windowHeight = displayManager.Height;

			inputManager = game.Services.GetService<InputManager>();

			rectangleRenderer.LayerDepth = .6f;

			collisionManager = game.Services.GetService<CollisionManager>();
		}



		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			var mousePosition = inputManager.MousePosition(); //Get the mouse position

			//Keep the padding on screen even when the curor goes out of screen
			var clampedXAxis = Math.Clamp(mousePosition.X, rectangleSize.X / 1.9f, windowWidth - rectangleSize.X / 1.9f);


			//Keep the paddle on the bottom of the screen
			var constrainedYAxis = windowHeight - rectangleSize.Y / 1.25f;

			transform.LocalPosition = new Vector2(clampedXAxis, constrainedYAxis);


		}

		public void AttachBall(Ball ball)
		{
			ball.SetParent(this);
			ball.GetComponent<Transform>().LocalPosition = new Vector2(0, -18);
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

	}
}
