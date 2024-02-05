using EC.Components;
using EC.Components.Colliders;
using EC.Components.Render;
using EC.Components.Renderers;
using EC.Components.UI;
using EC.CoreSystem;
using EC.Services;
using EC.Utilities.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Entities
{
	internal class EndGameWindow : Entity
	{
		private Entity[] windowEntities; 
		public Entity[] WindowEntities => windowEntities;

		public event Action GameReset;

		private Entity textMessage;

		public string Message { get; set; }

		public EndGameWindow(Entity gameWorld, Game game) : base(game)
		{
			this.LoadRectangleComponents("end game window", 250, 200, Color.LightSteelBlue, game, false);

			var rectangleRenderer = GetComponent<RectangleRenderer>();


			AddComponent(new Origin(rectangleRenderer.TextureCenter, this));

			var centerPoint = gameWorld.GetComponent<BoxCollider2D>().Bounds;
			Transform.LocalPosition = new Vector2(centerPoint.Width / 2, centerPoint.Height / 2);



			textMessage = new Entity(Game);
			textMessage.LoadTextComponents("Fonts/PopUpMessage", "", Color.Black, Game, TextRenderer.Alignment.Center);
			textMessage.Transform.Parent = Transform;
			textMessage.Transform.LocalPosition = new Vector2(0, -33);

			

			Entity playAgainButton = new Entity(Game);
			playAgainButton.Transform.Parent = Transform;
			playAgainButton.LoadRectangleComponents("button rectangle", 70, 33, Color.Green, Game, true);
			playAgainButton.Transform.LocalPosition = new Vector2(0, 50);
			var buttonRenderer = playAgainButton.GetComponent<RectangleRenderer>();
			
			
			playAgainButton.AddComponent(new Button(game, playAgainButton));
			playAgainButton.GetComponent<Origin>().Value = buttonRenderer.TextureCenter;

			playAgainButton.GetComponent<Button>().Clicked += () => GameReset?.Invoke();

			Entity restartText = new Entity(Game);
			restartText.LoadTextComponents("Fonts/Score", "Restart", Color.Black, game, TextRenderer.Alignment.Center);
			restartText.Transform.Parent = playAgainButton.Transform;



			windowEntities = new Entity[] { this, playAgainButton, textMessage, restartText };

			foreach (var entity in windowEntities)
			{
				var renderer = entity.GetComponent<Renderer>();
				if (renderer != null)
				{
					var parentRenderer = entity.Transform.Parent?.Entity.GetComponent<Renderer>();
					if (parentRenderer != null)
					{
						renderer.LayerDepth = parentRenderer.LayerDepth + .05f;
					}
				}
			}

			foreach (var entity in windowEntities)
			{
				entity.IntendedVisible = false;
				entity.IntendedEnable = false;
			}

			SetAllEndWindowEntitiesVisibility(false);
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			textMessage.GetComponent<TextRenderer>().Text = Message;
		}


		public void SetAllEndWindowEntitiesVisibility(bool visible)
		{
			foreach (var entity in windowEntities)
			{
				entity.IntendedVisible = visible;
				entity.IntendedEnable = visible;


				entity.Visible = entity.IntendedVisible;
				entity.Enabled = entity.IntendedEnable;
			}

		}



	}
}
