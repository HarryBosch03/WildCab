using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace BoschingMachine.Generation
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Generation/City Generation Profile")]
    public class CityGenerationProfile : ScriptableObject
    {
        [SerializeField] Vector2Int cells;
        [SerializeField] int blockSize;
        [SerializeField] int roadWidth;

        [Space]
        [SerializeField] List<float> splitWeights;

        [Space]
        [SerializeField] bool regenerate;

        CityGenerationData last;

        public CityGenerationData Generate()
        {
            if (!regenerate && last != null) return last;

            var data = new CityGenerationData(GetGridSize());

            MarkGrid(data);
            MarkBlocks(data, true);
            MarkBlocks(data, false);

            OuterEdges(data);

            regenerate = false;
            last = data;
            return data;
        }

        private void OuterEdges(CityGenerationData data)
        {
            bool[,] old = new bool[data.Size.x - roadWidth - blockSize, data.Size.y - roadWidth - blockSize];
            for (int x = 0; x < old.GetLength(0); x++)
            {
                for (int y = 0; y < old.GetLength(1); y++)
                {
                    old[x, y] = data.Roads[x, y];
                }
            }

            for (int x = 0; x < data.Size.x; x++)
            {
                for (int y = 0; y < data.Size.y; y++)
                {
                    data.Roads[x, y] = false;
                }
            }

            int o = (blockSize + roadWidth) / 2;
            for (int x = 0; x < old.GetLength(0); x++)
            {
                for (int y = 0; y < old.GetLength(1); y++)
                {
                    data.Roads[x + o, y + o] = old[x, y];
                }
            }            
        }

        private void MarkBlocks(CityGenerationData data, bool swap)
        {
            float totalWeight = 0.0f;
            splitWeights.ForEach(v => totalWeight += v);

            int GetDivision ()
            {
                float w = Random.value * totalWeight;
                int i = 1;
                foreach (float split in splitWeights)
                {
                    if (w < split) return i;
                    
                    w -= split;
                    i++;
                }

                return splitWeights.Count;
            }

            int xMax = GetGridEdge(v => v.x);
            int yMax = GetGridEdge(v => v.y);
            for (int x = roadWidth; x < xMax; x += (blockSize + roadWidth))
                for (int y = roadWidth; y < yMax; y += (blockSize + roadWidth))
                {
                    MarkBlock(data, new Vector2Int(x, y), GetDivision(), swap);
                }
        }

        private void MarkBlock(CityGenerationData data, Vector2Int origin, int d, bool doSwap)
        {
            if (d < 2) return;

            System.Func<Vector2Int, Vector2Int> swap = doSwap ?
                v => new Vector2Int(v.y, v.x) :
                v => v;

            for (int i = 0; i < d - 1; i++)
            {
                float p = (i + 1.0f) / d;
                int c = (int)(p * blockSize);
                for (int r = 0; r < blockSize; r++)
                {
                    Vector2Int offset = new Vector2Int(c, r);
                    Vector2Int pos = origin + swap(offset);
                    data.Roads[pos.x, pos.y] = true;
                }
            }
        }

        private void MarkGrid(CityGenerationData data)
        {
            for (int x = 0; x < data.Size.x; x++)
            {
                for (int y = 0; y < data.Size.y; y++)
                {
                    if (IsRoadEdge(x) || IsRoadEdge(y))
                    {
                        data.Roads[x, y] = true;
                    }
                }
            }
        }

        public bool IsRoadEdge(int x)
        {
            return (x % (blockSize + roadWidth)) < roadWidth;
        }

        private int GetGridEdge(System.Func<Vector2Int, int> c)
        {
            return c(cells) * blockSize + (1 + c(cells)) * roadWidth;
        }

        private Vector2Int GetGridSize()
        {
            return new Vector2Int(GetGridEdge(v => v.x), GetGridEdge(v => v.y));
        }

        private Vector2Int Swap (Vector2Int v) => new Vector2Int(v.y, v.x);

        #if UNITY_EDITOR
        private void OnValidate ()
        {
            regenerate = true;
        }
        #endif
    }
}
