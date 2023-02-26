using UnityEngine;

namespace BoschingMachine.Generation
{
    public class CityGenerationData
    {
        public Vector2Int Size { get; private set; }
        public bool[,] Roads { get; private set; }
        public float[,] Heights { get; private set; }

        public CityGenerationData(Vector2Int size)
        {
            Size = size;
            Roads = new bool[size.x, size.y];
            Heights = new float[size.x, size.y];
        }
    }
}
