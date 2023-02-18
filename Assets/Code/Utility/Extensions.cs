using System.Collections;
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
    }
}
