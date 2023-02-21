using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Generation/City Generation Profile")]
    public sealed class CityGenerationProfile : ScriptableObject
    {
        [SerializeField] Vector2Int size;
        [SerializeField] int roadMinDistance;

        public CityGenerationData Generate ()
        {
            var data = new CityGenerationData(size.x, size.y);

            CollapseRandomCell(data.Roads);

            return data;
        }

        public bool CollapseRandomCell(bool[,] grid)
        {
            var validCells = GetAllValidCells(grid);

            if (validCells.Count == 0) return false;

            var c = validCells[Random.Range(0, validCells.Count)];
            grid[c.x, c.y] = true;

            return true;
        }

        public bool CanCollapseCell(bool[,] grid, int x, int y)
        {
            for (int i = -roadMinDistance; i <= roadMinDistance; i++)
            {
                for (int j = -roadMinDistance; j <= roadMinDistance; j++)
                {
                    if (x + i < 0) continue;
                    if (x + i >= grid.GetLength(0)) continue;
                    if (y + j < 0) continue;
                    if (y + j >= grid.GetLength(1)) continue;

                    if (grid[x + i, y + j]) return false;
                }
            }

            return true;
        }

        public List<Vector2Int> GetAllValidCells(bool[,] grid)
        {
            List<Vector2Int> cells = new List<Vector2Int>();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (CanCollapseCell(grid, x, y))
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }

            return cells;
        }
    }
}
