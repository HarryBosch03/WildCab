using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace BoschingMachine.Editor.Tools
{
    public static class Albert
    {
        [MenuItem("Tools/Fast Ragdoll")]
        public static void FastRagdoll()
        {
            var bodies = new List<Rigidbody>();
            
            foreach (var gameObject in Selection.gameObjects)
            {
                var self = gameObject.transform.GetOrAddComponent<Rigidbody>();
                
                var joint = gameObject.AddComponent<CharacterJoint>();
                var parent = gameObject.transform.parent.GetOrAddComponent<Rigidbody>();

                joint.connectedBody = parent;
                
                if (!bodies.Contains(self)) bodies.Add(self);
                if (!bodies.Contains(parent)) bodies.Add(parent);
            }

            const float totalMass = 80.0f;
            foreach (var body in bodies)
            {
                body.mass = totalMass / bodies.Count;
            }
        }
    }
}
