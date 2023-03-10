using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Utility;

namespace BoschingMachine.Generation
{
    public class CityMeshGenerator : MonoBehaviour
    {
        [SerializeField] CityGenerationProfile profile;
        [SerializeField] float subdivideRate;
        [SerializeField] float minSize;
        [SerializeField] float gridScale;
        [SerializeField] float floorHeight;
        [SerializeField] Gradient buildingPalette;
        [SerializeField] AnimationCurve heightCurve;

        List<Building> buildings;

        private void Awake()
        {
            Build();
        }

        [ContextMenu("Build")]
        private void Build()
        {
            if (!profile) return;

            var data = profile.Generate();
            buildings = new();
            var closed = new HashSet<Vector2Int>();

            for (int x = 0; x < data.Size.x; x++)
            {
                for (int y = 0; y < data.Size.y; y++)
                {
                    Vector2Int p = new Vector2Int(x, y);
                    TryAddBuilding(data, p, buildings, closed);
                }
            }

            var subBuildings = new List<Building>();

            for (int i = 0; i < buildings.Count * subdivideRate; i++)
            {
                if (buildings.Count == 0) break;

                var building = buildings[Random.Range(0, buildings.Count)];
                buildings.Remove(building);

                if (building.size.x < minSize || building.size.y < minSize)
                {
                    subBuildings.Add(building);
                    i--;
                    continue;
                }

                subBuildings.AddRange(building.Subdivide());
            }

            buildings.AddRange(subBuildings);

            BakeMesh(buildings);
        }

        private void BakeMesh(List<Building> buildings)
        {
            var builder = new MeshBuilder();
            builder.Space = Matrix4x4.Scale(Vector3.one);

            Building.FloorHeight = floorHeight;
            buildings.ForEach(b =>
            {
                b.floors = (int)heightCurve.Evaluate(Random.value);
                b.color = buildingPalette.Evaluate(Random.value);
                b.AppendMeshData(builder, gridScale);
            });

            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            builder.Build(mesh);
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        private void TryAddBuilding(CityGenerationData data, Vector2Int p, List<Building> buildings, HashSet<Vector2Int> closed)
        {
            bool AxisValid(Vector2Int p, Axis axis) => axis(p) >= 0 && axis(p) < axis(data.Size);

            bool Valid(Vector2Int p)
            {
                if (closed.Contains(p)) return false;
                if (!AxisValid(p, p => p.x)) return false;
                if (!AxisValid(p, p => p.y)) return false;
                if (data.Roads[p.x, p.y]) return false;

                return true;
            }

            if (!Valid(p)) return;

            Vector2Int other = p;
            bool valid;
            do
            {
                valid = true;
                for (int x = p.x; x < other.x + 1 && valid; x++)
                {
                    if (!Valid(new Vector2Int(x, other.y + 1)))
                    {
                        valid = false;
                    }
                }

                for (int y = p.y; y < other.y + 1 && valid; y++)
                {
                    if (!Valid(new Vector2Int(other.x + 1, y)))
                    {
                        valid = false;
                    }
                }

                other += Vector2Int.one;
            }
            while (valid);

            var building = new Building(((Vector2)other + p) / 2.0f, other - p);
            buildings.Add(building);

            for (int x = p.x; x < other.x; x++)
            {
                for (int y = p.y; y < other.y; y++)
                {
                    closed.Add(new Vector2Int(x, y));
                }
            }
        }

        class Building
        {
            public Vector2 position;
            public Vector2 size;
            public int floors;
            public Color color = Color.white;
            public static float FloorHeight { get; set; }

            public Building(Vector2 position, Vector2 size)
            {
                this.position = position;
                this.size = size;
                floors = 1;
            }

            public Building[] Subdivide()
            {
                return new Building[]
                {
                    new Building(position + new Vector2(size.x, size.y) / 4.0f, size / 2.0f),
                    new Building(position + new Vector2(size.x, -size.y) / 4.0f, size / 2.0f),
                    new Building(position + new Vector2(-size.x, -size.y) / 4.0f, size / 2.0f),
                    new Building(position + new Vector2(-size.x, size.y) / 4.0f, size / 2.0f),
                };
            }

            public void AppendMeshData(MeshBuilder builder, float scale = 1.0f)
            {
                builder.Color = color;

                for (int i = 0; i < floors; i++)
                {
                    var position = new Vector3(this.position.x * scale, i * FloorHeight, this.position.y * scale);
                    var size = new Vector3(this.size.x * scale, FloorHeight, this.size.y * scale);

                    builder.AppendBox(size - new Vector3(0.1f, 0.2f, 0.1f), Matrix4x4.Translate(position + Vector3.up * (FloorHeight * 0.5f - 0.1f)));
                    builder.AppendBox(new Vector3(size.x, 0.2f, size.z), Matrix4x4.Translate(position + Vector3.up * (FloorHeight - 0.1f)));
                }

            }
        }

        public delegate int Axis(Vector2Int v);
    }
}
