using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public class Hitstop : MonoBehaviour
    {
        [SerializeField] float duration;

        static int Count;

        IEnumerator Start ()
        {
            Time.timeScale = 0.02f;
            Time.fixedDeltaTime = Time.timeScale / 50.0f;

            Count++;

            yield return new WaitForSecondsRealtime(duration / 60.0f);

            Count--;

            if (Count <= 0)
            {
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }
}
