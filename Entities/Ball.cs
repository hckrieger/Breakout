using Breakout.Scenes;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Breakout.Entities.Paddle;
using static EC.Services.CollisionManager;

namespace Breakout.Entities
{
    internal class Ball : Entity
	{
		private Transform transform;
		private CircleRenderer circleRenderer;
		private CircleCollider2D circleCollider;
		private Paddle paddle;
		private Velocity velocity;
		private float speedX, speedY, speed;
		private DisplayManager displayManager;
		private int circleRadius;

		private CollisionManager collisionManager;
		private InputManager inputManager; 

		public Ball(Game game) : base(game)
		{
			transform = new Transform(this);
			circleRenderer = new CircleRenderer("ball", 6, Color.Beige, game, this);

			Origin origin = new Origin(new Vector2(circleRenderer.TextureWidth / 2, circleRenderer.TextureHeight / 2), this);
			AddComponent(origin);
			circleRadius = circleRenderer.TextureWidth / 2;
			AddComponent(transform);
			circleCollider = new CircleCollider2D(new Circle(new Vector2(transform.Position.X + origin.Value.X, transform.Position.Y + origin.Value.Y), circleRadius), this);

			velocity = new Velocity(transform, this);

			displayManager = game.Services.GetService<DisplayManager>();

			speed = 450;
			AddComponents(circleRenderer, circleCollider, velocity);

			//transform.LocalPosition = new Vector2(400, 400);

			collisionManager = Game.Services.GetService<CollisionManager>();
			inputManager = Game.Services.GetService<InputManager>();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			velocity.Value = new Vector2(speedX, speedY);

	

			ReflectOffEdges();
		}

		public void SetParent(Paddle newParent)
		{
			paddle = newParent;
			if (paddle == null)
			{
				transform.Parent = null;
			} else
			{
				transform.Parent = paddle.GetComponent<Transform>();
			}
		}

		public void ReflectOffEdges()
		{
			if ((velocity.Value.X >= 0 && (transform.Position.X + circleRadius) > displayManager.Width) || 
				(velocity.Value.X < 0 && (transform.Position.X - circleRadius) < 0))
				speedX = -speedX;

			if (velocity.Value.Y < 0 && (transform.Position.Y - circleRadius) < 0)
				speedY = -speedY;
		}


		public void ReflectBallFromBrick(CollisionSide collisionSide)
		{

			if (collisionSide == CollisionSide.Left || collisionSide == CollisionSide.Right)
				speedX = -speedX;

			if (collisionSide == CollisionSide.Top || collisionSide == CollisionSide.Bottom)
				speedY = -speedY;
		}


		public void ReflectBallFromSection(LaunchSection LaunchSection, Paddle paddle)
		{
			var velocityVector = Vector2.Zero;

			switch (LaunchSection)
			{

				case LaunchSection.OuterLeft:
					velocityVector = MathUtils.VelocityFromDegrees(150, speed);
					break;
				case LaunchSection.MiddleLeft:
					velocityVector = MathUtils.VelocityFromDegrees(130, speed);
					break;
				case LaunchSection.Center:

					
					if (velocity.Value.X < 0)
						velocityVector = MathUtils.VelocityFromDegrees(110, speed);
					else
						velocityVector = MathUtils.VelocityFromDegrees(80, speed);
					


					break;
				case LaunchSection.MiddleRight:
					velocityVector = MathUtils.VelocityFromDegrees(60, speed);
					break;
				case LaunchSection.OuterRight:
					velocityVector = MathUtils.VelocityFromDegrees(40, speed);
					break;
			}

			speedX = velocityVector.X;
			speedY = velocityVector.Y;
		}
	}
}
