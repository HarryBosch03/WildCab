using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoschingMachine.Utility
{
    public class MeshUtilities
    {
        public Matrix4x4 Space { get; set; }
        public List<Vector3> Vertices { get; private set; }
        public List<int> Triangles { get; private set; }

        public MeshUtilities () : this(new List<Vector3>(), new List<int>())
        {

        }

        public MeshUtilities (List<Vector3> verts, List<int> tris)
        {
            Space = Matrix4x4.identity;
            Vertices = verts;
            Triangles = tris;
        }

        public MeshUtilities AppendBox (Vector3 size, Matrix4x4 subSpace)
        {
            Vector3 right = Vector3.right * size.x * 0.5f;
            Vector3 up = Vector3.up * size.y * 0.5f;
            Vector3 forward = Vector3.forward * size.z * 0.5f;
            
            AppendQuad(right, forward, up, subSpace);
            AppendQuad(-right, -forward, up, subSpace);
            
            AppendQuad(forward, up, right, subSpace);
            AppendQuad(-forward, up, -right, subSpace);
            
            AppendQuad(up, right, forward, subSpace);
            AppendQuad(-up, right, -forward, subSpace);
            
            return this;
        }

        public MeshUtilities AppendQuad (Vector3 point, Vector3 width, Vector3 height, Matrix4x4 subSpace)
        {
            Vector2 size = new Vector2(width.magnitude, height.magnitude);
            Vector3 forward = Vector3.Cross(height, width);

            AppendQuad(size, subSpace * Matrix4x4.LookAt(point, point + forward, height));

            return this;
        }

        public MeshUtilities AppendQuad (Vector2 size, Matrix4x4 subSpace)
        {
            Vector3 a = new Vector3( size.x,  size.y, 0.0f);
            Vector3 b = new Vector3(-size.x,  size.y, 0.0f);
            Vector3 c = new Vector3(-size.x, -size.y, 0.0f);
            Vector3 d = new Vector3( size.x, -size.y, 0.0f);

            AddTriangle(a, b, c, subSpace);
            AddTriangle(a, c, d, subSpace);

            return this;
        }

        public MeshUtilities AddTriangle (Vector3 a, Vector3 b, Vector3 c, Matrix4x4 subSpace)
        {
            var space = Space * subSpace;

            Vertices.Add(space * Point(a));
            Vertices.Add(space * Point(b));
            Vertices.Add(space * Point(c));

            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 1);

            return this;
        }

        public Vector4 Point(Vector3 p) => new Vector4(p.x, p.y, p.z, 1.0f);

        public Mesh Build () => Build(new Mesh());
        public Mesh Build (Mesh mesh)
        {
            mesh.vertices = Vertices.ToArray();
            mesh.triangles = Triangles.ToArray();

            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
 