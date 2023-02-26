using UnityEngine;

namespace BoschingMachine
{
    public class Squirrel3
    {
        const uint BitNoise1 = 0xB5297A4D;
        const uint BitNoise2 = 0x68E31DA4;
        const uint BitNoise3 = 0x1B56C4E9;
        const uint Prime1 = 198491317;
        const uint Prime2 = 6542989;

        public LerpFunction Lerp { get; set; } = Mathf.Lerp;

        public uint Get (uint pos, uint seed = 0)
        {
            pos *= BitNoise1;
            pos += seed;
            pos ^= (pos >> 8);
            pos += BitNoise2;
            pos ^= (pos << 8);
            pos *= BitNoise3;
            pos ^= (pos >> 8);

            return pos;
        }
        public uint Get(int pos, uint seed = 0) => Get((uint)pos, seed);
        public uint Get(int x, int y, uint seed = 0) => Get((int)(x + y * Prime1), seed);
        public uint Get(int x, int y, int z, uint seed = 0) => Get((int)(x + y * Prime1 + z * Prime2), seed);

        public float Get(float pos, uint seed = 0)
        {
            int i = (int)pos;
            int j = i + 1;
            float p = pos - i;

            float a = Get(i, seed) / uint.MaxValue;
            float b = Get(j, seed) / uint.MaxValue;
            return Lerp(a, b, p);
        }

        public float Get(Vector2 pos, uint seed = 0)
        {
            var corners = new Vector2Int[]
            {
                Vector2Int.FloorToInt(pos),
                Vector2Int.FloorToInt(pos) + Vector2Int.right,
                Vector2Int.FloorToInt(pos) + Vector2Int.up,
                Vector2Int.FloorToInt(pos) + Vector2Int.one,
            };

            var directions = new Vector2[4];

            for (int i = 0; i < 4; i++)
            {
                var a = Get(corners[i].x, corners[i].y, seed) / (float)uint.MaxValue;
                a *= Mathf.PI * 2.0f;
                directions[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            }

            Vector2 percent = pos - corners[0];

            var dots = new float[4];
            for (int i = 0; i < 4; i++)
            {
                dots[i] = Vector2.Dot(percent, directions[i]);
            }

            float x1 = Lerp(dots[0], dots[1], percent.x);
            float x2 = Lerp(dots[2], dots[3], percent.x);

            return Lerp(x1, x2, percent.y);
        }
        
        public float Get(Vector3 pos, uint seed = 0)
        {
            var corners = new Vector3Int[]
            {
                Vector3Int.FloorToInt(pos),
                Vector3Int.FloorToInt(pos) + Vector3Int.right,
                Vector3Int.FloorToInt(pos) + Vector3Int.up,
                Vector3Int.FloorToInt(pos) + Vector3Int.right * Vector3Int.up,

                Vector3Int.FloorToInt(pos) + Vector3Int.forward,
                Vector3Int.FloorToInt(pos) + Vector3Int.right + Vector3Int.forward,
                Vector3Int.FloorToInt(pos) + Vector3Int.up + Vector3Int.forward,
                Vector3Int.FloorToInt(pos) + Vector3Int.one,
            };

            var directions = new Vector3[8];

            for (int i = 0; i < 8; i++)
            {
                var a1 = Get(corners[i].x, corners[i].y, corners[i].z, seed) / (float)uint.MaxValue;
                var a2 = Get(corners[i].x, corners[i].y, corners[i].z, seed + 1) / (float)uint.MaxValue;

                a1 *= Mathf.PI * 2.0f;
                a2 *= Mathf.PI * 2.0f;

                directions[i] = new Vector3(Mathf.Cos(a1), Mathf.Sin(a1)) * Mathf.Cos(a2) + Vector3.forward * Mathf.Sin(a2);
            }

            Vector3 percent = pos - corners[0];

            var dots = new float[8];
            for (int i = 0; i < 8; i++)
            {
                dots[i] = Vector3.Dot(percent, directions[i]);
            }

            float x1 = Lerp(dots[0], dots[1], percent.x);
            float x2 = Lerp(dots[2], dots[3], percent.x);
            float x3 = Lerp(dots[4], dots[5], percent.x);
            float x4 = Lerp(dots[6], dots[7], percent.x);

            float y1 = Lerp(x1, x2, percent.y);
            float y2 = Lerp(x3, x4, percent.y);

            return Lerp(y1, y2, percent.y);
        }

        public static float Smoothdamp(float a, float b, float t) => Mathf.Lerp(a, b, t * t * t * (t * (t * 6 - 15) + 10));

        public delegate float LerpFunction(float a, float b, float t);
    }
}
