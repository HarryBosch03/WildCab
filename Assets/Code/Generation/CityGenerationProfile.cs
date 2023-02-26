using UnityEngine;

namespace BoschingMachine.Generation
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Generation/City Generation Profile")]
    public class CityGenerationProfile : ScriptableObject
    {
        [SerializeField] Vector2Int cells;
        [SerializeField] int blockSize;
        [SerializeField] int roadWidth;

        [Space]
        [SerializeField] int halfBlocks;
        [SerializeField] int thirdBlocks;
        
        public CityGenerationData Generate ()
        {
            var data = new CityGenerationData(GetGridSize());

            MarkGrid(data);
            MarkBlocks(data, halfBlocks, 2);
            MarkBlocks(data, halfBlocks, 3);

            return data;
        }

        private void MarkBlocks(CityGenerationData data, int halfBlocks, int d)
        {
            for (int i = 0; i < halfBlocks; i++)
            {
                Vector2Int pos = new Vector2Int
                    (
                    Random.Range(0, data.Size.x), 
                    Random.Range(0, data.Size.y)
                    );

                pos = GetClosestBlockCorner(pos);

                MarkBlock(pos, d);
            }
        }

        private void MarkBlock (CityGenerationData data, Vector2Int origin, int d)
        {
            System.Func<Vector2Int, Vector2Int> swap = Random.value > 0.5f ?
                v => v :
                v => new Vector2Int(v.y, v.x);

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

        public bool IsRoadEdge (int x)
        {
            return (x % (blockSize + roadWidth)) < roadWidth; 
        }

        private Vector2Int GetGridSize()
        {
            int Edge(System.Func<Vector2Int, int> c)
            {
                return c(cells) * blockSize + (1 + c(cells)) * roadWidth;
            }

            return new Vector2Int(Edge(v => v.x), Edge(v => v.y));
        }
        
        private Vector2Int GetClosestBlockCorner (Vector2Int p)
        {
            int Edge (System.Func<Vector2Int, int> c)
            {
                return c(p) / (blockSize + roadWidth) + roadWidth;
            }

            return new Vector2Int(Edge(v => v.x), Edge(v => v.y));
        }
    }
}
