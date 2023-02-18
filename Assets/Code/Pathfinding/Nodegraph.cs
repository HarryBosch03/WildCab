using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Pathfinding
{
    public class Nodegraph : MonoBehaviour
    {
        private List<Transform> points = new();
        private List<Edge> edges = new();

        [ContextMenu("Get Points")]
        public void GetPoints()
        {
            points.Clear();
            foreach (Transform p in transform)
            {
                points.Add(p);
            }
        }
        
        private void OnDrawGizmos()
        {
            foreach (var point in points)
            {
                Gizmos.DrawSphere(point.position, 0.1f);
            }
            
            foreach (var edge in edges)
            {
                var a = edge.a.position;
                var b = edge.b.position;
                
                Gizmos.DrawLine(a, b);
            }
        }

        class Edge
        {
            public Transform a;
            public Transform b;
            
            public Edge(Transform a, Transform b)
            {
                this.a = a;
                this.b = b;
            }
        }
    }
}
