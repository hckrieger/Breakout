using EC.Components.Colliders;
using EC.CoreSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Components
{
	internal class PaddleColliders : Component
	{
		private BoxCollider2D[] sections;

		public PaddleColliders(Entity entity, int sectionCount, Vector2 paddleSize) : base(entity)
		{
			sections = new BoxCollider2D[sectionCount];
			int sectionWidth = (int)paddleSize.X / sectionCount;
			int sectionHeight = (int)paddleSize.Y/3;

			for (int i = 0; i < sectionCount; i++)
				sections[i] = new BoxCollider2D(new Rectangle(i * sectionWidth, 0, sectionWidth, sectionHeight), entity); 
			
		}

		public int SectionCount => sections.Length;


		public BoxCollider2D GetSection(int index)
		{
			if (index < 0 || index >= sections.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
			}
			return sections[index];
		}
	}
}
