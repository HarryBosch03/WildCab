using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.TryGetComponent(out T result) ? result : component.gameObject.AddComponent<T>();
        }

        public static T Best<T> (this IEnumerable<T> enumerator, System.Func<T, float> getScore)
        {
            T best = default;

            foreach (var element in enumerator)
            {
                if (best == null)
                {
                    best = element;
                }
                else if (getScore(element) < getScore(best))
                {
                    best = element;
                }
            }

            return best;
        }
    }
}
