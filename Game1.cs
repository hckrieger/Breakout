using Breakout.Scenes;
using EC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breakout
{
	public class Game1 : ExtendedGame
	{
		public static string PLAYING_SCENE = "PLAYING_SCENE";
		public Game1()
		{
			IsMouseVisible = true;
			WindowWidth = 900;
			WindowHeight = 600;
		}

		protected override void Initialize()
		{



			base.Initialize();

			// TODO: Add your initialization logic here
			SceneManager.AddScene(PLAYING_SCENE, new PlayingScene(this));
			SceneManager.ChangeScene(PLAYING_SCENE);

			
		}


	}
}