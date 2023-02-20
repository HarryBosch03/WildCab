using UnityEngine;
using UnityEngine.AI;

namespace BoschingMachine.AI.Modules
{
    [System.Serializable]
    public class NPCBehaviour
    {
        [SerializeField] float maxWanderDistance;
        [SerializeField] float maxWaitTime;

        float waitTimer;
        NavMeshPath path = null;
        int cornerIndex = 0;

        public void Update (NPC npc)
        {
            if (path == null)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer > maxWaitTime)
                {
                    GetNewPath(npc);
                }
            }
            else
            {
                while((path.corners[cornerIndex] - npc.transform.position).sqrMagnitude < 1.0f)
                {
                    cornerIndex++;
                    if (cornerIndex >= path.corners.Length) 
                    {
                        path = null;
                        Update(npc);
                        return;
                    }
                }
                var corner = path.corners[cornerIndex];
                npc.MoveAndFacePoint(corner);

                waitTimer = 0.0f;
            }
        }

        public void GetNewPath (NPC npc)
        {
            path = null;
            var from = npc.transform.position;
            var to = npc.transform.position + Random.insideUnitSphere * maxWanderDistance;

            if (!NavMesh.SamplePosition(from, out var fromHit, maxWanderDistance, ~0)) return;
            if (!NavMesh.SamplePosition(to, out var toHit, maxWanderDistance, ~0)) return;

            from = fromHit.position;
            to = toHit.position;

            var newPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(from, to, ~0, newPath)) return;

            path = newPath;
            cornerIndex = 0;
        }
    }
}
