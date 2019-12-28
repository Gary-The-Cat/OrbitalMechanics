using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid_SFML.CrateTools
{
    public class CrateManager
    {
        public readonly int VerticalPixelOffset = 100;
        public int CrateSize = 128;

        private bool[,] crates;

        public CrateManager()
        {
            crates = new bool[15, 8];
        }

        public Vector2i GetCrateIndexFromPosition(Vector2f position)
        {
            var x = position.X;
            var y = position.Y - VerticalPixelOffset;

            return new Vector2i((int)(x / CrateSize), (int)(y / CrateSize));
        }

        public Vector2f GetCrateCentreFromIndex(Vector2i index)
        {
            var x = index.X * CrateSize + CrateSize / 2;
            var y = VerticalPixelOffset + index.Y * CrateSize + CrateSize / 2;

            return new Vector2f(x, y);
        }

        public IEnumerable<Vector2f> GetCratePositions()
        {
            for(int x = 0; x < crates.GetLength(0); x++)
            {
                for (int y = 0; y < crates.GetLength(1); y++)
                {
                    if(crates[x, y])
                    {
                        yield return GetCrateCentreFromIndex(new Vector2i(x, y));
                    }
                }
            }

            yield break;
        }

        public void AddCrate(Vector2f cratePosition)
        {
            var crateIndex = GetCrateIndexFromPosition(cratePosition);
            crates[crateIndex.X, crateIndex.Y] = true;
        }
    }
}
