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

		public MainMenuScreen(Game1 game) : base(game)
		{


			displayManager = Game.Services.GetService<DisplayManager>();


			Entity titleEntity = new Entity(game);
			titleEntity.LoadTextComponents("Fonts/Title", "Breakout", Color.MonoGameOrange, Game, TextRenderer.Alignment.Center);
			titleEntity.Transform.LocalPosition = new Vector2(displayManager.WindowCenter.X, 100);
			AddEntity(titleEntity);

			playButtonBox = new Entity(game);
			playButtonBox.LoadRectangleComponents("play button", 100, 33, Color.MonoGameOrange, game, true);
			//playButtonBox.AddComponent(new Origin(playButtonBox.GetComponent<RectangleRenderer>().TextureCenter, playButtonBox));
		    playButtonBox.GetComponent<Origin>().Value = playButtonBox.GetComponent<RectangleRenderer>().TextureCenter;
			playButtonBox.Transform.LocalPosition = new Vector2(displayManager.WindowCenter.X, 250);
			
			playButtonBox.AddComponent(new Button(game, playButtonBox));
			AddEntity(playButtonBox);

			

			sceneManager = game.Services.GetService<SceneManager>();

			playButtonBox.GetComponent<Button>().Clicked += () => sceneManager.ChangeScene(Game1.PLAYING_SCENE);

			Entity playText = new Entity(game);
			playText.LoadTextComponents("Fonts/Score", "Play", Color.Black, game, TextRenderer.Alignment.Center);
			playText.Transform.Parent = playButtonBox.Transform;
			playText.GetComponent<TextRenderer>().LayerDepth = playButtonBox.GetComponent<RectangleRenderer>().LayerDepth + .1f;
			playText.Transform.LocalPosition = Vector2.Zero;

			AddEntity(playText);


			exitButtonBox = new Entity(game);
			exitButtonBox.LoadRectangleComponents("exit button", 100, 33, new Color(40, 100, 40, 1), game, true);
			exitButtonBox.Transform.LocalPosition = new Vector2(displayManager.WindowCenter.X, 325);
			//exitButtonBox.AddComponent(new Origin(exitButtonBox.GetComponent<RectangleRenderer>().TextureCenter, exitButtonBox));
			exitButtonBox.GetComponent<Origin>().Value = exitButtonBox.GetComponent<RectangleRenderer>().TextureCenter;
			exitButtonBox.AddComponent(new Button(game, exitButtonBox));
			AddEntity(exitButtonBox);


			Entity exitText = new Entity(game);
			exitText.LoadTextComponents("Fonts/Score", "Exit", Color.Black, game, TextRenderer.Alignment.Center);
			exitText.Transform.Parent = exitButtonBox.Transform;
			exitText.GetComponent<TextRenderer>().LayerDepth = exitButtonBox.GetComponent<RectangleRenderer>().LayerDepth + .1f;
			exitText.Transform.LocalPosition = Vector2.Zero;

			exitButtonBox.GetComponent<Button>().Clicked += () => game.Exit();

			AddEntity(exitText);




		}

		public override void Initialize()
		{
			base.Initialize();


		}




	}
}
