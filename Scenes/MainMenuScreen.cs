using EC;
using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.UI;
using EC.CoreSystem;
using EC.Services;
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
		public MainMenuScreen(Game1 game) : base(game)
		{
			Entity textButton = new Entity(game);

			Transform transform = new Transform(textButton);
			transform.LocalPosition = new Vector2(300, 200);
			textButton.AddComponent(transform);

			RectangleRenderer rectangleRenderer = new RectangleRenderer("button image", 100, 75, Color.LightBlue, game, textButton);

			textButton.AddComponent(rectangleRenderer);

			BoxCollider2D boxCollider = new BoxCollider2D(0, 0, rectangleRenderer.TextureWidth, rectangleRenderer.TextureHeight, textButton);
			textButton.AddComponent(boxCollider);

			Button button = new Button(game, textButton);
			textButton.AddComponent(button);






			AddEntity(textButton);

			button.Clicked += () => sceneManager.ChangeScene(Game1.PLAYING_SCENE);


			sceneManager = game.Services.GetService<SceneManager>();	

		}

	
	}
}
