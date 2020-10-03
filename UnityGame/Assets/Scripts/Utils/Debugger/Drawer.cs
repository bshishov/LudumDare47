using System.Collections.Generic;
using UnityEngine;

namespace Utils.Debugger
{
    public class Drawer
    {
        private readonly Mesh _circleMesh;
        private readonly Mesh _lineMesh;

        private readonly Material _material;
        private readonly Dictionary<Color, Material> _materials = new Dictionary<Color, Material>();
        private readonly List<RenderData> _queuedMeshes = new List<RenderData>();

        public Drawer()
        {
            _lineMesh = MeshUtils.LineMesh(Vector3.zero, Vector3.forward);
            _circleMesh = MeshUtils.CircleMesh(1f);
            _material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        }

        public Material GetMaterial(Color col)
        {
            // TODO: FIX! Way to inefficient. Use MaterialPropertyBlock instead
            if (_materials.ContainsKey(col))
                return _materials[col];

            var mat = Object.Instantiate(_material);
            mat.color = col;
            _materials.Add(col, mat);
            return mat;
        }

        public void DrawLine(Vector3 from, Vector3 to, Color col, float duration)
        {
            _material.color = col;
            QueueRenderData(_lineMesh, GetLineTransform(from, to), GetMaterial(col), duration);
        }

        public void DrawCircle(Vector3 center, Vector3 normal, float radius, Color col, float duration)
        {
            _material.color = col;

            var transform = Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
            QueueRenderData(_circleMesh, transform, GetMaterial(col), duration);
        }

        public void DrawCircleSphere(Vector3 center, float radius, Color col, float duration)
        {
            DrawCircle(center, Vector3.up, radius, col, duration);
            DrawCircle(center, Vector3.right, radius, col, duration);
            DrawCircle(center, Vector3.forward, radius, col, duration);
        }

        public static Matrix4x4 GetLineTransform(Vector3 from, Vector3 to)
        {
            var direction = to - from;
            return Matrix4x4.TRS(from, Quaternion.LookRotation(direction), Vector3.one * direction.magnitude);
        }

        private void QueueRenderData(Mesh mesh, Matrix4x4 transform, Material material, float duration)
        {
            _queuedMeshes.Add(new RenderData
            {
                Mesh = mesh,
                Material = material,
                Transform = transform,
                Until = Time.time + duration
            });
        }

        public void DrawQueuedMeshes()
        {
            foreach (var data in _queuedMeshes) Graphics.DrawMesh(data.Mesh, data.Transform, data.Material, 0, null);
        }

        public void ProcessQueue()
        {
            _queuedMeshes.RemoveAll(data => data.Until < Time.time);
        }

        public void DrawCone(Vector3 origin, Vector3 direction, float sphereSize, float angle, Color color,
            float duration)
        {
            direction.Normalize();

            Debugger.Default.DrawCircle(origin, Vector3.up, sphereSize, color, duration);
            Debugger.Default.DrawRay(new Ray(origin, direction), color, sphereSize, duration);
            Debugger.Default.DrawRay(new Ray(origin, Quaternion.Euler(0, -angle * 0.5f, 0) * direction), color,
                sphereSize, duration);
            Debugger.Default.DrawRay(new Ray(origin, Quaternion.Euler(0, +angle * 0.5f, 0) * direction), color,
                sphereSize, duration);
        }

        private struct RenderData
        {
            public Mesh Mesh;
            public Material Material;
            public Matrix4x4 Transform;
            public float Until;
        }
    }

    public static class MeshUtils
    {
        public static Mesh LineMesh(Vector3 from, Vector3 to)
        {
            var mesh = new Mesh();
            mesh.SetVertices(new List<Vector3> {from, to});
            mesh.SetNormals(new List<Vector3> {Vector3.up, Vector3.up});
            mesh.SetUVs(0, new List<Vector3> {Vector3.zero, Vector3.one});
            mesh.SetIndices(new[] {0, 1}, MeshTopology.Lines, 0, true);
            return mesh;
        }

        public static Mesh CircleMesh(float radius, int segments = 50)
        {
            var mesh = new Mesh();
            var vertices = new Vector3[segments];
            for (var i = 0; i < vertices.Length; i++)
            {
                var x = radius * Mathf.Sin(2 * Mathf.PI * i / segments);
                var y = radius * Mathf.Cos(2 * Mathf.PI * i / segments);
                vertices[i] = new Vector3(x, y, 0f);
            }

            var tris = new int[2 * segments];
            var idx = 0;
            for (var i = 0; i < segments; i++)
            {
                tris[idx++] = i;
                tris[idx++] = (i + 1) % segments;
            }

            var normals = new Vector3[vertices.Length];
            for (var i = 0; i < normals.Length; i++) normals[i] = -Vector3.forward;

            mesh.vertices = vertices;
            mesh.SetIndices(tris, MeshTopology.Lines, 0);
            mesh.normals = normals;

            return mesh;
        }
    }
}