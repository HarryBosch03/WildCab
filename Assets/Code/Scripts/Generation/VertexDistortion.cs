using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class VertexDistortion
    {
        [SerializeField] float noiseFrequency;
        [SerializeField] float noiseAmplitude;
        [SerializeField] Vector3 preScale;
        [SerializeField] Vector3 postScale;

        Squirrel3 noise = new Squirrel3();

        public void Apply(Vector3[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Vector3 v = list[i];
                Apply(ref v);
                list[i] = v;
            }
        }

        public void Apply(IList<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Vector3 v = list[i];
                Apply(ref v);
                list[i] = v;
            }
        }

        public void Apply(ref Vector3 v)
        {
            Vector3 input = Vector3.Scale(v, preScale);

            float a1 = noise.Get(input * noiseFrequency, 0) * Mathf.PI * 2.0f;
            float a2 = noise.Get(input * noiseFrequency, 1) * Mathf.PI * 2.0f;

            Vector3 dir = new Vector3(Mathf.Cos(a1), Mathf.Sin(a1)) * Mathf.Cos(a2) + Vector3.forward * Mathf.Sin(a2);
            v += Vector3.Scale(dir * noiseAmplitude, postScale);
        }
    }
}
