using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [ID] Komponen MonoBehaviour untuk menampilkan Mesh UI melengkung di scene.
/// Mengupdate mesh secara real-time saat nilai diubah di Editor.
/// 
/// [EN] MonoBehaviour component to display Curved UI Mesh in the scene.
/// Updates the mesh in real-time when values are changed in the Editor.
/// </summary>
public class CurvedUI : MonoBehaviour
{
    [Header("Mesh Configuration")]
    [Range(2, 512)]
    [Tooltip("[ID] Resolusi grid mesh (kepadatan vertex). Semakin tinggi semakin halus lengkungannya.\n[EN] Mesh grid resolution (vertex density). Higher values mean smoother curvature.")]
    public int meshresolution = 10;

    [Range(5, 100)]
    [Tooltip("[ID] Jari-jari lengkungan. Nilai kecil = lengkungan tajam.\n[EN] Curvature radius. Small values = sharp curve.")]
    public float curveradius = 20f;

    [Range(5, 100)]
    [Tooltip("[ID] Tinggi vertikal dari mesh UI.\n[EN] Vertical height of the UI mesh.")]
    public float height = 30f;

    [Range(30, 270)]
    [Tooltip("[ID] Sudut bentangan busur (arc). Menentukan seberapa lebar UI melingkar.\n[EN] Arc chord angle. Determines how wide the UI wraps around.")]
    public float chordangle = 180f;

    [SerializeField, HideInInspector]
    MeshFilter meshFilter;

    // [ID] Referensi ke generator (meskipun statis, variabel ini untuk kejelasan struktur)
    // [EN] Reference to generator (though static, this variable keeps structure clear)
    private MeshGenerator face;

    // [ID] Dipanggil otomatis saat nilai variabel diubah di Inspector (Editor only)
    // [EN] Called automatically when variable values are changed in Inspector (Editor only)
    private void OnValidate()
    {
        Initialize();
    }

    // [ID] Fungsi inisialisasi utama untuk setup komponen dan generate mesh
    // [EN] Main initialization function to setup components and generate mesh
    void Initialize()
    {
        // [ID] Tambahkan MeshRenderer dan Material default jika belum ada
        // [EN] Add MeshRenderer and default Material if missing
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load("Materials/CurvedUIMaterial", typeof(Material)) as Material;
        }

        // [ID] Tambahkan MeshFilter jika belum ada
        // [EN] Add MeshFilter if missing
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // [ID] Generate mesh baru berdasarkan parameter yang diset
        // [EN] Generate new mesh based on set parameters
        meshFilter.sharedMesh = MeshGenerator.GenerateMesh(meshresolution, curveradius, height, chordangle);
    }
}