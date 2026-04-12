using System;
using UnityEngine;

/// <summary>
/// [ID] Kelas statis untuk menghasilkan Mesh melengkung (silinder) secara prosedural.
/// Biasa digunakan untuk membuat kanvas UI melengkung di VR.
/// 
/// [EN] Static class for procedurally generating curved (cylindrical) Meshes.
/// Commonly used for creating curved UI canvases in VR.
/// </summary>
public class MeshGenerator
{
    /// <summary>
    /// [ID] Menghasilkan objek Mesh berdasarkan parameter silinder.
    /// [EN] Generates a Mesh object based on cylindrical parameters.
    /// </summary>
    public static Mesh GenerateMesh(int resolution, float radius, float height, float chordangle)
    {
        Mesh mesh = new Mesh();

        // [ID] Inisialisasi array untuk vertices dan triangles
        // [EN] Initialize arrays for vertices and triangles
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        // [ID] Siapkan UV map (gunakan yang ada atau buat baru)
        // [EN] Prepare UV map (use existing or create new)
        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

        // [ID] Hitung langkah sudut dan tinggi per segmen
        // [EN] Calculate angle step and height step per segment
        float angleStep = (float)((Math.PI / 180f) * chordangle) / resolution;
        float heightStep = height / resolution;

        // [ID] Loop vertikal (Y)
        // [EN] Vertical loop (Y)
        for (int y = 0; y < resolution; y++)
        {
            // [ID] Loop horizontal (X)
            // [EN] Horizontal loop (X)
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                float angle = x * angleStep;

                // [ID] Hitung persentase posisi untuk UV (0.0 s/d 1.0)
                // [EN] Calculate position percentage for UV (0.0 to 1.0)
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                // [ID] Konversi koordinat Polar (sudut/radius) ke Cartesian (X/Z) untuk efek melengkung
                // [EN] Convert Polar coordinates (angle/radius) to Cartesian (X/Z) for the curvature effect
                vertices[i] = new Vector3(radius * Mathf.Cos(angle), y * heightStep, radius * Mathf.Sin(angle));

                // [ID] Set UV. Kita kurangi 1 dengan percent.x untuk membalik (mirror) sumbu horizontal
                //      agar teks UI tidak terbalik saat dirender.
                // [EN] Set UV. We subtract percent.x from 1 to flip (mirror) the horizontal axis
                //      so UI text is not mirrored when rendered.
                uv[i] = new Vector2(1f - percent.x, percent.y);

                // [ID] Membuat segitiga (Triangles) untuk membentuk Quad, kecuali di baris/kolom terakhir
                // [EN] Construct triangles to form a Quad, except for the last row/column
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // [ID] Segitiga pertama
                    // [EN] First triangle
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    // [ID] Segitiga kedua
                    // [EN] Second triangle
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }

        // [ID] Bersihkan dan tetapkan data baru ke mesh
        // [EN] Clear and assign new data to the mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // [ID] Hitung ulang normal untuk pencahayaan yang benar
        // [EN] Recalculate normals for correct lighting
        mesh.RecalculateNormals();
        mesh.uv = uv;

        return mesh;
    }
}