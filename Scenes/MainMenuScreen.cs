using EC;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.UI;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Scenes
{
	internal class MainMenuScreen : Scene
	{
		private SceneManager sceneManager;
		private DisplayManager displayManager;

		private Entity playButtonBox;
		private Entity exitButtonBox;

		private InputManager inputManager;

		public MainMenuScreen(Game1 game) : base(game)
		{
			inputManager = game.Services.GetService<InputManager>();
		}

		public override void Initialize()
		{
			base.Initialize();

			displayManager = Game.Services.GetService<DisplayManager>();
			sceneManager = Game.Services.GetService<SceneManager>();
			InitializeTitle(Game);


			playButtonBox = new Entity(Game);
			playButtonBox.CreateButton("play button", new Vector2(100, 33), true, Color.MonoGameOrange, Game, () => sceneManager.ChangeScene(Game1.PLAYING_SCENE));
			playButtonBox.Transform.LocalPosition = new Vector2(displayManager.InternalResolution.X / 2, 250);
			playButtonBox.AddButtonText("Fonts/Score", "Play", Color.Black, Game, AddEntity);
			AddEntity(playButtonBox);



			exitButtonBox = new Entity(Game);
			exitButtonBox.CreateButton("exit button", new Vector2(100, 33), true, new Color(40, 100, 40, 1), Game, () => Game.Exit());
			exitButtonBox.Transform.LocalPosition = new Vector2(displayManager.InternalResolution.X / 2, 325);
			exitButtonBox.AddButtonText("Fonts/Score", "Exit", Color.Black, Game, AddEntity);
			AddEntity(exitButtonBox);

		}


		private void InitializeTitle(Game game)
		{
			Entity titleEntity = new Entity(Game);
			titleEntity.LoadTextComponents("Fonts/Title", "Breakout", Color.MonoGameOrange, Game, TextRenderer.Alignment.Center);
			titleEntity.Transform.LocalPosition = new Vector2(displayManager.InternalResolution.X / 2, 100);
			AddEntity(titleEntity);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);


			Debug.WriteLine($"Mouse Position: {inputManager.MousePosition()}");
		}

	}
}
