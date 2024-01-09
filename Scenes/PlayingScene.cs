using Breakout.Components;
using Breakout.Entities;
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
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EC.Services.CollisionManager;

namespace Breakout.Scenes
{
    internal class PlayingScene : Scene
	{
		private Paddle paddle;
		private Ball ball;
		private InputManager inputManager;
		private DisplayManager displayManager;
		private CollisionManager collisionManager;

		private SpriteRenderer charizard; 

		private const int BRICK_COLS = 12;
		private const int BRICK_ROWS = 6;

		private Brick[,] brickGrid;

		private float brickHeightPlacement = 60;

		bool ballInBrickCollidingRegion = false;


		Entity score; 

		public PlayingScene(Game game) : base(game) 
		{
			paddle = new Paddle(game);
			ball = new Ball(game);
			
			AddEntities(paddle, ball);

			paddle.AttachBall(ball);

			score = new Entity(game);
			AddEntity(score);
			Transform transform = new Transform(score);
			score.AddComponent(transform);
			
			score.AddComponents(new TextRenderer("Fonts/Score", "Score: 0", Color.Black, game, score));
			score.GetComponent<TextRenderer>().TextAlignment = TextRenderer.Alignment.Center;
			

			inputManager = game.Services.GetService<InputManager>();
			displayManager = game.Services.GetService<DisplayManager>();
			collisionManager = game.Services.GetService<CollisionManager>();

			brickGrid = new Brick[BRICK_COLS, BRICK_ROWS];

			for (int y = 0; y < BRICK_ROWS; y++)
			{
				for (int x = 0; x < BRICK_COLS; x++)
				{
					var color = Color.White;
					switch (y)
					{
						case 0:
							color = Color.Purple; break;
						case 1:
							color = Color.Blue; break;
						case 2:
							color = Color.Green; break;
						case 3:
							color = Color.Yellow; break;
						case 4:
							color = Color.OrangeRed; break;
						case 5:
							color = Color.DarkRed; break;
					}

					brickGrid[x, y] = new Brick(game, color, $"brick-{x}x{y}");
					var renderedBrick = brickGrid[x, y].GetComponent<RectangleRenderer>();
					renderedBrick.LayerDepth = .1f;
					brickGrid[x, y].GetComponent<Transform>().LocalPosition = new Vector2(x * (renderedBrick.TextureWidth + 1), y * (renderedBrick.TextureHeight + 1) + brickHeightPlacement);

					AddEntity(brickGrid[x, y]);
				}
			}

		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (ball.GetComponent<Transform>().Parent == paddle.GetComponent<Transform>())
			{
				if (inputManager.MouseJustPressed() && inputManager.MouseOnScreen())
				{
					if (paddle.GetComponent<Transform>().Position.X >= displayManager.WindowCenter.X)
						paddle.DetachBall(ball, Paddle.LaunchSection.MiddleLeft);
					else
						paddle.DetachBall(ball, Paddle.LaunchSection.MiddleRight);
				}
			}
			else
			{
				paddle.PaddleSectionCollision(ball);
			}


			if (ball.GetComponent<CircleCollider2D>().Bounds.Center.Y + ball.GetComponent<CircleCollider2D>().Bounds.Radius <= brickGrid[BRICK_COLS-1, BRICK_ROWS-1].GetComponent<BoxCollider2D>().Bounds.Bottom+20)
				ballInBrickCollidingRegion = true;
			else
				ballInBrickCollidingRegion = false;



			if (!ballInBrickCollidingRegion)
				return;

			score.GetComponent<Transform>().LocalPosition = new Vector2(300, 300);


			for (int y = 0; y < BRICK_ROWS; y++)
			{
				for (int x = 0; x < BRICK_COLS; x++)
				{
					var ballCollider = ball.GetComponent<CircleCollider2D>();
					var brickCollider = brickGrid[x, y].GetComponent<BoxCollider2D>();

					if (collisionManager.ShapesIntersect(ballCollider, brickCollider) && brickGrid[x, y].Visible)
					{
						ball.ReflectBallFromBrick(collisionManager.GetCollisionSide(brickCollider, ballCollider));
							
						brickGrid[x, y].Visible = false;
						break;

					} 
				}
			}



		}

	}
}
