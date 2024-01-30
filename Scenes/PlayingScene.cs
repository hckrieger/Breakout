using Breakout.Components;
using Breakout.Entities;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
		


		bool ballInBrickCollidingRegion = false;
		Entity score; 

		public PlayingScene(Game game) : base(game) 
		{
		}


		public override void Initialize()
		{
			inputManager = Game.Services.GetService<InputManager>();
			displayManager = Game.Services.GetService<DisplayManager>();
			collisionManager = Game.Services.GetService<CollisionManager>();
			sceneManager = Game.Services.GetService<SceneManager>();

			HUD = new Entity(Game);
			HUD.LoadRectangleComponents("HUD", displayManager.Width, 25, Color.LightCoral, Game, true);

			HUD.GetComponent<RectangleRenderer>().LayerDepth = .1f;
			AddEntity(HUD, RootEntity);

			GameWorld = new Entity(Game);


			var accountForTopBounds = HUD.GetComponent<BoxCollider2D>().Bounds.Height;
			GameWorld.LoadRectangleComponents("game world bounds", displayManager.Width, displayManager.Height - accountForTopBounds, null, Game, true);
			GameWorld.Transform.LocalPosition = new Vector2(0, accountForTopBounds);

			AddEntity(GameWorld, RootEntity);


			var gameWorldBounds = GameWorld.GetComponent<BoxCollider2D>();

			paddle = new Paddle(gameWorldBounds, Game);
			ball = new Ball(gameWorldBounds, Game);

			ball.BallPassedPaddle += PopUpWindow;


			AddEntity(ball, GameWorld);
			AddEntity(paddle, GameWorld);

			endGameWindow = new EndGameWindow(GameWorld, Game);
			AddEntity(endGameWindow, GameWorld);

			endGameWindow.GameReset += Reset;

			foreach (var entities in endGameWindow.WindowEntities)
			{
				AddEntity(entities);
			}

			paddle.AttachBall(ball);


			score = new Entity(Game);
			//score.GetComponent<TextRenderer>().LayerDepth = .75f;
			AddEntity(score, HUD);

			score.AddComponents(new TextRenderer("Fonts/Score", "Score: 0", Color.Black, Game, score));

			score.GetComponent<TextRenderer>().TextAlignment = TextRenderer.Alignment.Left;


			score.Transform.LocalPosition = Vector2.Zero;
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
					//	renderedBrick.LayerDepth = .1f;
					brickGrid[x, y].Transform.LocalPosition = new Vector2(x * (renderedBrick.TextureWidth + 1), y * (renderedBrick.TextureHeight + 1) + brickHeightPlacement);
					AddEntity(brickGrid[x, y], GameWorld);
				}
			}

			base.Initialize();
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			score.GetComponent<TextRenderer>().Text = $"Score: " + bricksHit.ToString();

			if (inputManager.KeyJustPressed(Keys.Back))
				sceneManager.ChangeScene(Game1.MAIN_MENU_SCREEN);

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


			if (ball.GetComponent<CircleCollider2D>().Bounds.Center.Y + ball.GetComponent<CircleCollider2D>().Bounds.Radius <= brickGrid[BRICK_COLS-1, BRICK_ROWS-1].GetComponent<BoxCollider2D>().Bounds.Bottom+50)
				ballInBrickCollidingRegion = true;
			else
				ballInBrickCollidingRegion = false;



			if (!ballInBrickCollidingRegion)
				return;

			


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

						bricksHit++;
					} 
				}
			}

		
			if (bricksHit == BRICK_COLS*BRICK_ROWS)
			{
				Reset();
			}

		}

		public void PopUpWindow()
		{
			endGameWindow.SetAllEndWindowEntitiesVisibility(true);
		}

		public override void Reset()
		{
			ball.Reset();
			

			//paddle.Transform.LocalPosition = displayManager.WindowCenter;

			paddle.AttachBall(ball);

			for (int y = 0; y < BRICK_ROWS; y++)
			{
				for (int x = 0; x < BRICK_COLS; x++)
				{
					brickGrid[x, y].Visible = true;
				}
			}

			bricksHit = 0;

			

			endGameWindow.SetAllEndWindowEntitiesVisibility(false);
			

		}

	}
}
