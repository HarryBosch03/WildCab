
namespace BoschingMachine
{
    public sealed class CityGenerationData
    {
        public bool[,] Roads { get; private set; }

        public CityGenerationData (int width, int height)
        {
            Roads = new bool[width, height];
        }
    }
}
