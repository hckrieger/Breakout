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
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Entities
{
	internal class EndGameWindow : Entity
	{
		private Entity[] windowEntities; 
		public Entity[] WindowEntities => windowEntities;

		public event Action GameReset;

		Entity playAgainButton;

		public EndGameWindow(Entity gameWorld, Game game) : base(game)
		{
			this.LoadRectangleComponents("end game window", 250, 200, Color.LightSteelBlue, game, false);

			var rectangleRenderer = GetComponent<RectangleRenderer>();
			rectangleRenderer.LayerDepth = .75f;


			AddComponent(new Origin(rectangleRenderer.TextureCenter, this));

			var centerPoint = gameWorld.GetComponent<BoxCollider2D>().Bounds;
			Transform.LocalPosition = new Vector2(centerPoint.Width / 2, centerPoint.Height / 2);

			

			playAgainButton = new Entity(Game);
			playAgainButton.Transform.Parent = Transform;
			playAgainButton.LoadRectangleComponents("button rectangle", 50, 33, Color.Green, Game, true);
			playAgainButton.Transform.LocalPosition = new Vector2(0, 50);
			var buttonRenderer = playAgainButton.GetComponent<RectangleRenderer>();
			
			
			buttonRenderer.LayerDepth = .85f;
			playAgainButton.AddComponent(new Button(game, playAgainButton));
			playAgainButton.GetComponent<Origin>().Value = buttonRenderer.TextureCenter;

			playAgainButton.GetComponent<Button>().Clicked += () => GameReset?.Invoke();

			windowEntities = new Entity[] { this, playAgainButton };

			SetAllEndWindowEntitiesVisibility(false);
		}

	


		public void SetAllEndWindowEntitiesVisibility(bool visible)
		{
			foreach (var entity in windowEntities)
			{
				entity.Visible = visible;	
			}

		}



	}
}
