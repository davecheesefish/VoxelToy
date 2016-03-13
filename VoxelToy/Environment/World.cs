using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelToy.Environment
{
    class World
    {
        private int width;
        private int height;
        private int length;
        private Block[,,] blocks;
        private BasicEffect effect;

        public World(int width, int height, int length)
        {
            this.width = width;
            this.height = height;
            this.length = length;

            blocks = new Block[width, height, length];
            // Initialise blocks array
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        if (y <= height * (Math.Cos(0.1 * (x + z)) + 1.0) / 2.0)
                        {
                            blocks[x, y, z] = new Block(new Vector3(x, y, z), new Color(80, GameServices.Random.Next(256), 80));
                        }
                        else
                        {
                            blocks[x, y, z] = null;
                        }
                    }
                }
            }

            effect = new BasicEffect(GameServices.GraphicsDevice);
            effect.VertexColorEnabled = true;
        }

        public void Draw(Camera camera)
        {
            effect.View = camera.LookAtMatrix;
            effect.Projection = GameSettings.PfovMatrix;

            // Draw blocks
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    for (int z = 0; z < length; ++z)
                    {
                        if (blocks[x, y, z] != null)
                        {
                            blocks[x, y, z].Draw(camera, effect);
                        }
                    }
                }
            }
        }
    }
}
