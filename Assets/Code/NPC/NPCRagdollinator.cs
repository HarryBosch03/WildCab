using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoschingMachine.AI.Modules
{
    [System.Serializable]
    public class NPCRagdollinator
    {
        [SerializeField] GameObject ragdoll;
        [SerializeField] Transform ragdollRoot;
        [SerializeField] GameObject notRagdoll;
        [SerializeField] Transform notRagdollRoot;

        public bool Ragdolled { get; private set; }
        List<Rigidbody> bodies;

        public IReadOnlyList<Rigidbody> Bodies => bodies;

        public void Ragdoll(Transform transform, Rigidbody rigidbody, System.Action<bool> setCollision)
        {
            if (Ragdolled) return;
            Ragdolled = true;

            LazyInitalize();

            TransposeTransformTree(notRagdollRoot, ragdollRoot);
            ragdoll.SetActive(true);
            notRagdoll.SetActive(false);

            bodies.ForEach(e => e.velocity = rigidbody.velocity);

            rigidbody.isKinematic = true;
            setCollision(false);
        }

        public void Resume(Transform transform, Rigidbody rigidbody, System.Action<bool> setCollision)
        {
            if (!Ragdolled) return;
            Ragdolled = false;

            LazyInitalize();

            Recenter(transform);

            ragdoll.SetActive(false);
            notRagdoll.SetActive(true);

            rigidbody.isKinematic = false;
            setCollision(true);
        }

        public void Recenter (Transform transform)
        {
            var center = Vector3.zero;
            bodies.ForEach(e => center += e.position);
            center *= 1.0f / bodies.Count;

            transform.position = center;
        }

        public bool IsRagdollStill ()
        {
            LazyInitalize();

            var meanVelocity = Vector3.zero;
            bodies.ForEach(e => meanVelocity += e.velocity);
            meanVelocity *= 1.0f / bodies.Count;

            return meanVelocity.sqrMagnitude < 0.25f;
        }

        public void LazyInitalize ()
        {
            if (bodies == null) bodies = new(ragdoll.GetComponentsInChildren<Rigidbody>(true));
        }

        public static void TransposeTransformTree(Transform from, Transform to)
        {
            var fromList = new List<Transform>();
            var toList = new List<Transform>();

            AppendChildrenToList(from, fromList);
            AppendChildrenToList(to, toList);

            if (fromList.Count != toList.Count) throw new System.Exception("From and To Trees are not compatable");

            for (int i = 0; i < fromList.Count; i++)
            {
                toList[i].position = fromList[i].position;
                toList[i].rotation = fromList[i].rotation;
                toList[i].localScale = fromList[i].localScale;
            }
        }

        private static void AppendChildrenToList(Transform t, IList<Transform> list)
        {
            list.Add(t);
            foreach (Transform child in t)
            {
                AppendChildrenToList(child, list);
            }
        }
    }
}