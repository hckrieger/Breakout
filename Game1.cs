using Breakout.Scenes;
using EC;


namespace Breakout
{
	public class Game1 : ExtendedGame
	{
		public static string PLAYING_SCENE = "PLAYING_SCENE";
		public static string MAIN_MENU_SCREEN = "MAIN_MENU";
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
			SceneManager.AddScene(MAIN_MENU_SCREEN, new MainMenuScreen(this));
			SceneManager.AddScene(PLAYING_SCENE, new PlayingScene(this));
			SceneManager.ChangeScene(MAIN_MENU_SCREEN);

			
		}


	}
}