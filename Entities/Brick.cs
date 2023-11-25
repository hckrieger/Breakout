using EC.Components;
using EC.Components.Colliders;
using EC.CoreSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Entities
{
	internal class Brick : Entity
	{
		private Transform transform;
		private RectangleRenderer rectangleRenderer;
		private BoxCollider2D boxCollider;
		private Point rectangleSize;

		public Brick(Game game, Color color, string identifier) : base(game)
		{
			rectangleSize = new Point(74, 30);
			transform = new Transform(this);
			AddComponent(transform);
			rectangleRenderer = new RectangleRenderer(identifier, rectangleSize.X, rectangleSize.Y, color, game, this);
			boxCollider = new BoxCollider2D(0, 0, rectangleSize.X, rectangleSize.Y, this);

			AddComponents(rectangleRenderer, boxCollider);

		}
	}
}
