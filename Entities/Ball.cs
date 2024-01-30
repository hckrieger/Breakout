﻿using Breakout.Scenes;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities;
using EC.Utilities.Extensions;
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
		private Paddle paddle;
		private Velocity velocity;
		private float speedX, speedY, speed;
		private DisplayManager displayManager;
		private int circleRadius;

		private CollisionManager collisionManager;
		private InputManager inputManager;

		private BoxCollider2D gameWorldCollider;

		public event Action BallPassedPaddle;

		public Ball(BoxCollider2D gameWorldCollider, Game game) : base(game)
		{
			this.LoadCircleComponents("ball", 6, Color.Beige, game, true);


			this.gameWorldCollider = gameWorldCollider;


		}

		public override void Initialize()
		{
			base.Initialize();

			velocity = new Velocity(GetComponent<Transform>(), this);
			AddComponent(velocity);

			displayManager = Game.Services.GetService<DisplayManager>();

			speed = 450;

			collisionManager = Game.Services.GetService<CollisionManager>();
			inputManager = Game.Services.GetService<InputManager>();

			circleRadius = GetComponent<CircleRenderer>().TextureWidth / 2;
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
				Transform.Parent = null;
			} else
			{
				Transform.Parent = paddle.GetComponent<Transform>();
			}
		}

		public void ReflectOffEdges()
		{

			var collider = GetComponent<CircleCollider2D>();

			if ((velocity.Value.X >= 0 && (Transform.Position.X + circleRadius) > gameWorldCollider.Bounds.Right) || 
				(velocity.Value.X < 0 && (Transform.Position.X - circleRadius) < gameWorldCollider.Bounds.Left))
				speedX = -speedX;


			if (velocity.Value.Y < 0 && (Transform.Position.Y - circleRadius) < gameWorldCollider.Bounds.Top + 3)
				speedY = -speedY;

			if (Transform.Position.Y + circleRadius > gameWorldCollider.Bounds.Bottom + 100)
			{
				
			    BallPassedPaddle?.Invoke();
			}
				
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

		public override void Reset()
		{
			speedX = 0;
			speedY = 0;

			if (velocity != null)
				velocity.Value = Vector2.Zero;

			SetParent(paddle); // if the paddle is always the starting position
			Transform.LocalPosition = new Vector2(0, -18);
		}
	}

	
}
