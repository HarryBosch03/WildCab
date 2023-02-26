using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public static class Util
    {
        public static void Swap <T> (ref T a1, ref T b1)
        {
            ref T a2 = ref a1;
            ref T b2 = ref b1;

            a1 = b2;
            b1 = a2;
        }
    }
}
