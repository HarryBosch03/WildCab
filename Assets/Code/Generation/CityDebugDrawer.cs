using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Generation
{
    public class CityDebugDrawer : MonoBehaviour
    {
        [SerializeField] bool draw;
        [SerializeField] float scale;
        [SerializeField] CityGenerationProfile profile;

        private void OnDrawGizmos()
        {
            if (!draw) return;
            if (!profile) return;

            var data = profile.Generate();

            for (int x = 0; x < data.Size.x; x++)
            {
                for (int y = 0; y < data.Size.y; y++)
                {
                    if (data.Roads[x, y])
                    {
                        Gizmos.color = Color.red;
                        Vector3 pos = new Vector3(x, 0.0f, y) * scale;
                        Gizmos.DrawCube(pos, new Vector3(scale, 0, scale));
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                        Vector3 pos = new Vector3(x, 0.5f, y) * scale;
                        Gizmos.DrawCube(pos, Vector3.one * scale);
                    }
                }
            }
        }
    }
}
