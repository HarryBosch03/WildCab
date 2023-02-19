using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoschingMachine .AI.Modules
{
    [System.Serializable]
    public class NPCRagdollinator
    {
        [SerializeField] private GameObject ragdollPrefab;
        [SerializeField] private string rootName;
        
        public void Ragdoll(NPC npc)
        {
            var ragdoll = Object.Instantiate(ragdollPrefab, npc.transform.position, npc.transform.rotation);
            
            var rootFrom = npc.transform.Find(rootName);
            var rootTo = npc.transform.Find(rootName);

            var fromList = new List<Transform>();
            var toList = new List<Transform>();
            
            AppendChildrenToList(rootFrom, fromList);
            AppendChildrenToList(rootTo, toList);

            if (fromList.Count != toList.Count) throw new System.Exception($"Ragdoll \"{ragdoll}\" is Invalid");
            
            for (var i = 0; i < fromList.Count; i++)
            {
                var from = fromList[i];
                var to = toList[i];

                to.position = from.position;
                to.rotation = from.rotation;
            }

            var fromRigidbody = npc.GetComponent<Rigidbody>();
            var toRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();

            foreach (var body in toRigidbodies)
            {
                body.velocity = fromRigidbody.velocity;
            }
        }

        private void AppendChildrenToList(Transform transform, List<Transform> list)
        {
            list.Add(transform);
            foreach (Transform child in transform)
            {
                AppendChildrenToList(child, list);
            }
        }
    }
}
