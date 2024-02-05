using Breakout.Components;
using Breakout.Entities;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
using EC.Services.AssetManagers;
using EC.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Breakout.Scenes
{
    internal class PlayingScene : Scene
	{
		private Paddle paddle;
		private Ball ball;
		private InputManager inputManager;
		private DisplayManager displayManager;
		private CollisionManager collisionManager;
		private SceneManager sceneManager;


		private const int BRICK_COLS = 12;
		private const int BRICK_ROWS = 6;

		private Entity[,] brickGrid;

		private float brickHeightPlacement = 60;
		private Entity HUD, GameWorld;

		private int bricksHit = 0;

		private EndGameWindow endGameWindow;

		private AudioAssetManager audioAssetManager;

		bool ballInBrickCollidingRegion = false;
		Entity score; 

		public PlayingScene(Game game) : base(game) 
		{

		}


		public override void Initialize()
		{
			SetupServices();

			SetupGameZones();

			SetUpEntities();


			base.Initialize();
		}

		private void SetupServices()
		{
			inputManager = Game.Services.GetService<InputManager>();
			displayManager = Game.Services.GetService<DisplayManager>();
			collisionManager = Game.Services.GetService<CollisionManager>();
			sceneManager = Game.Services.GetService<SceneManager>();
			audioAssetManager = Game.Services.GetService<AudioAssetManager>();
		}

		public void SetupGameZones()
		{
			HUD = new Entity(Game);
			HUD.LoadRectangleComponents("HUD", displayManager.Width, 25, Color.LightCoral, Game, true);
			HUD.GetComponent<RectangleRenderer>().LayerDepth = .1f;
			AddEntity(HUD, RootEntity);

			GameWorld = new Entity(Game);
			var accountForTopBounds = HUD.GetComponent<BoxCollider2D>().Bounds.Height;
			GameWorld.LoadRectangleComponents("game world bounds", displayManager.Width, displayManager.Height - accountForTopBounds, null, Game, true);
			GameWorld.Transform.LocalPosition = new Vector2(0, accountForTopBounds);

			AddEntity(GameWorld, RootEntity);
		}

		public void SetUpEntities()
		{
			var gameWorldBounds = GameWorld.GetComponent<BoxCollider2D>();

			//add paddle entity
			paddle = new Paddle(gameWorldBounds, Game);
			AddEntity(paddle, GameWorld);


			//add ball entity
			ball = new Ball(gameWorldBounds, Game);
			ball.BallPassedPaddle += ShowEndGameWindow;

			var gameAudio = new GameAudio(Game);
			ball.OnCollision += (sender, e) => gameAudio.PlayCollisionSound(e);
			AddEntity(ball, GameWorld);

			paddle.AttachBall(ball);

			//add game window entity and it's children
			endGameWindow = new EndGameWindow(GameWorld, Game);
			endGameWindow.GameReset += Reset;
			AddEntity(endGameWindow, GameWorld);
			foreach (var entities in endGameWindow.WindowEntities)
				AddEntity(entities);
			

			//add score entity
			score = new Entity(Game);
			score.AddComponents(new TextRenderer("Fonts/Score", "Score: 0", Color.Black, Game, score));
			AddEntity(score, HUD);

			//add entity for each of the bricks
			var rectangleSize = new Point(74, 30);
			brickGrid = new Entity[BRICK_COLS, BRICK_ROWS];

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


					brickGrid[x, y] = new Entity(Game);
					brickGrid[x, y].LoadRectangleComponents($"brick-{x}x{y}", rectangleSize.X, rectangleSize.Y, color, Game, true);
					var renderedBrick = brickGrid[x, y].GetComponent<RectangleRenderer>();
					renderedBrick.LayerDepth = .1f;
					brickGrid[x, y].Transform.LocalPosition = new Vector2(x * (renderedBrick.TextureWidth + 1), y * (renderedBrick.TextureHeight + 1) + brickHeightPlacement);
					AddEntity(brickGrid[x, y], GameWorld);
				}
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			//Show and update score
			score.GetComponent<TextRenderer>().Text = $"Score: " + bricksHit.ToString();


			//Change scene to main menu upon key press
			if (inputManager.KeyJustPressed(Keys.Back))
				sceneManager.ChangeScene(Game1.MAIN_MENU_SCREEN);


			//if the ball is the child of the paddle....
			if (ball.GetComponent<Transform>().Parent == paddle.GetComponent<Transform>())
			{
				//....then launch the ball at a given angle upon mouseclick. 
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
				//Launch the ball at a given angle depends on what section of the paddle it hits
				paddle.PaddleSectionCollision(ball);

				//if (collisionManager.ShapesIntersect(paddle.GetComponent<BoxCollider2D>(), ball.GetComponent<CircleCollider2D>()))
				//	audioAssetManager.PlaySoundEffect("Audio/Sound/wallCollisionSound");
			}



			if (ball.GetComponent<CircleCollider2D>().Bounds.Center.Y + ball.GetComponent<CircleCollider2D>().Bounds.Radius <= brickGrid[BRICK_COLS-1, BRICK_ROWS-1].GetComponent<BoxCollider2D>().Bounds.Bottom+50)
				ballInBrickCollidingRegion = true;
			else
				ballInBrickCollidingRegion = false;


			//return the update to avoid iterating through all the bricks if the ball is below a certain section of the screen
			if (!ballInBrickCollidingRegion)
				return;

			

			//Iterate through all the bricks to detect a collision and make the bricks invisible upon collision. 
			for (int y = 0; y < BRICK_ROWS; y++)
			{
				for (int x = 0; x < BRICK_COLS; x++)
				{

					var ballCollider = ball.GetComponent<CircleCollider2D>();
					var brickCollider = brickGrid[x, y].GetComponent<BoxCollider2D>();

					if (collisionManager.ShapesIntersect(ballCollider, brickCollider) && brickGrid[x, y].Visible)
					{
						ball.RaiseBrickCollisionEvent(y);
						ball.ReflectBallFromBrick(collisionManager.GetCollisionSide(brickCollider, ballCollider));
							
						brickGrid[x, y].Visible = false;

						bricksHit++;
					}

					

				}
			}

			//If you hit all the bricks then show an end game window
			if (bricksHit == BRICK_COLS*BRICK_ROWS)
			{
				ShowEndGameWindow();
				
			}

		}


		private void ShowEndGameWindow()
		{
			//set the end game window and it's children to being visible 
			endGameWindow.SetAllEndWindowEntitiesVisibility(true);

			//If the ball is below the bottom of the screen upon the window popping up....
			//...then show a game over message; if it's above that show a win message
			if (ball.Transform.Position.Y > displayManager.Height)
				endGameWindow.Message = "Try again!";
			else
				endGameWindow.Message = "You win!";

			//disable the ball and paddle to prevent them from moving upon the game being over
			EnableBallAndPaddle(false);
			
		}

		public override void Reset()
		{


			
			ball.Reset();
			paddle.AttachBall(ball);
			EnableBallAndPaddle();
			for (int y = 0; y < BRICK_ROWS; y++)
			{
				for (int x = 0; x < BRICK_COLS; x++)
				{
					brickGrid[x, y].Visible = true;
				}
			}

			bricksHit = 0;

			Enabled = true;

			endGameWindow.SetAllEndWindowEntitiesVisibility(false);
			

		}


		public void EnableBallAndPaddle(bool enable = true)
		{
			ball.Enabled = enable;
			paddle.Enabled = enable;
		}

	}
}
