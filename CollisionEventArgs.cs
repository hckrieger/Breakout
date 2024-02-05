using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
	internal class CollisionEventArgs : EventArgs
	{
		public string CollisionType { get; }

        public CollisionEventArgs(string collisionType)
        {
                CollisionType = collisionType;
        }


    }

    internal class BrickCollisionEventArgs : CollisionEventArgs
    {
        public int HitRow { get; }

        public BrickCollisionEventArgs(int hitRow) : base("brick")
        {
            HitRow = hitRow;
        }
    }
}
