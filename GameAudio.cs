using EC.Services.AssetManagers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
	internal class GameAudio
	{
		private AudioAssetManager audioAssetManager;

        public GameAudio(Game game)
        {
            audioAssetManager = game.Services.GetService<AudioAssetManager>();
        }

        public void PlayCollisionSound(CollisionEventArgs e)
        {
            if (e is BrickCollisionEventArgs brickCollisionEventArgs)
            {
                PlayBrickHitSound(brickCollisionEventArgs.HitRow);
            } else
            {
                switch (e.CollisionType)
                {
                    case "paddle":
                        PlayPaddleHitSound();
                        break;
                    case "boundary":
                        PlayWallHitSound();
                        break;
                    default:
                        throw new ArgumentException($"This sound from the identifier {e.CollisionType} does not exist");
                }
            }
        }

        private void PlayBrickHitSound(int hitRow)
        {
			audioAssetManager.PlaySoundEffect($"Audio/Sound/Bricks/brickHit{hitRow}");
		}

        private void PlayPaddleHitSound()
        {
            audioAssetManager.PlaySoundEffect("Audio/Sound/paddleHit");
        }


        private void PlayWallHitSound()
        {
            audioAssetManager.PlaySoundEffect("Audio/Sound/wallHit");
        }


    }
}
