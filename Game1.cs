using Breakout.Scenes;
using EC;
using Microsoft.Xna.Framework;


namespace Breakout
{
	public class Game1 : ExtendedGame
	{
		public static string PLAYING_SCENE = "PLAYING_SCENE";
		public static string MAIN_MENU_SCREEN = "MAIN_MENU";
		public Game1()
		{
			IsMouseVisible = true;
			
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetWindowSize(900, 600);
			IsFullScreen = true;

			// TODO: Add your initialization logic here
			SceneManager.AddScene(MAIN_MENU_SCREEN, new MainMenuScreen(this));
			SceneManager.AddScene(PLAYING_SCENE, new PlayingScene(this));
			SceneManager.ChangeScene(MAIN_MENU_SCREEN);

			
		}


	}
}