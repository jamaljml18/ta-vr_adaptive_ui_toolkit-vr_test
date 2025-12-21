using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// [ID] Menangani interaksi VR pada UI melengkung yang menggunakan Render Texture.
/// Mengubah input Raycast VR menjadi klik tombol pada Canvas 2D tersembunyi.
/// 
/// [EN] Handles VR interaction on curved UI using Render Texture.
/// Converts VR Raycast input into button clicks on a hidden 2D Canvas.
/// </summary>
public class CurvedUIVRButtonHandler : MonoBehaviour
{
    [Header("References")]
    // [ID] Komponen interactable pada mesh 3D yang akan menerima raycast
    // [EN] The interactable component on the 3D mesh receiving the raycast
    [SerializeField] private XRSimpleInteractable interactable;

    // [ID] Canvas asli yang dirender menjadi RenderTexture (Render Mode harus Screen Space - Camera)
    // [EN] Original canvas rendered as a RenderTexture (Render Mode must be Screen Space - Camera)
    [SerializeField] private Canvas sourceCanvas;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    // [ID] Objek debug visual (misal: Cube) untuk menandakan trigger terdeteksi
    // [EN] Visual debug object (e.g., Cube) to indicate trigger detection
    [SerializeField] private GameObject cubeDebug;

    // [ID] Objek debug visual kedua untuk menandakan tombol UI berhasil diklik
    // [EN] Second visual debug object to indicate UI button was successfully clicked
    [SerializeField] private GameObject cubeDebug2;

    // Cached components
    private GraphicRaycaster canvasRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Awake()
    {
        // [ID] Jika interactable tidak di-assign manual, cari di object ini
        // [EN] If interactable is not manually assigned, look for it on this object
        if (interactable == null)
            interactable = GetComponent<XRSimpleInteractable>();
    }

    private void Start()
    {
        canvasRaycaster = sourceCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        // [ID] Validasi komponen penting
        // [EN] Validate essential components
        if (canvasRaycaster == null)
            Debug.LogError("[CurvedUIVR] GraphicRaycaster not found on Source Canvas!");

        if (interactable == null)
            Debug.LogError("[CurvedUIVR] XRSimpleInteractable not found!");

        if (sourceCanvas.worldCamera == null)
            Debug.LogError("[CurvedUIVR] Source Canvas is missing Event Camera! Please assign the UI Camera.");
    }

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnVRTriggerPressed);
        }
    }

    private void OnDisable()
    {
        // [ID] Membersihkan event listener untuk mencegah memory leak
        // [EN] Cleanup event listener to prevent memory leaks
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnVRTriggerPressed);
        }
    }

    // ============================================================
    // [ID] EVENT SAAT TRIGGER DITEKAN
    // [EN] EVENT WHEN TRIGGER IS PRESSED
    // ============================================================
    private void OnVRTriggerPressed(SelectEnterEventArgs args)
    {
        // [ID] Visual feedback: Toggle cubeDebug saat trigger ditekan pada mesh
        // [EN] Visual feedback: Toggle cubeDebug when trigger is pressed on the mesh
        if (cubeDebug != null) cubeDebug.SetActive(!cubeDebug.activeSelf);

        // [ID] Cek apakah interactor yang menekan adalah Ray Interactor (Laser Pointer)
        // [EN] Check if the interactor pressing is a Ray Interactor (Laser Pointer)
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            // [ID] Mencoba mendapatkan informasi Raycast Hit 3D dari interactor
            // [EN] Try to get 3D Raycast Hit information from the interactor
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // [ID] Mengambil koordinat UV dari titik benturan pada Mesh Collider
                // [EN] Retrieve UV coordinates from the hit point on the Mesh Collider
                Vector2 uvHit = hit.textureCoord;

                if (showDebugLogs) Debug.Log($"[CurvedUIVR] VR Click detected at UV: {uvHit}");

                // [ID] Jalankan logika simulasi klik UI
                // [EN] Execute UI click simulation logic
                SimulateUIClick(uvHit);
            }
            else
            {
                if (showDebugLogs) Debug.LogWarning("[CurvedUIVR] Ray Interactor active but no 3D RaycastHit data found.");
            }
        }
    }

    private void SimulateUIClick(Vector2 uvCoords)
    {
        if (eventSystem == null || canvasRaycaster == null) return;

        pointerEventData = new PointerEventData(eventSystem);

        // ✅ PERBAIKAN WAJIB UNTUK VR RENDER TEXTURE
        Camera uiCam = sourceCanvas.worldCamera;

        if (uiCam != null)
        {
            // Gunakan resolusi kamera/texture (1024), BUKAN resolusi headset (2000+)
            pointerEventData.position = new Vector2(
                uvCoords.x * uiCam.pixelWidth,
                uvCoords.y * uiCam.pixelHeight
            );
        }
        else
        {
            // Fallback
            RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();
            pointerEventData.position = new Vector2(
               uvCoords.x * canvasRect.rect.width,
               uvCoords.y * canvasRect.rect.height
           );
        }

        // ... Lanjutkan raycast seperti biasa ...
        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null)
            {
                if (cubeDebug2) cubeDebug2.SetActive(!cubeDebug2.activeSelf); // Debug visual
                btn.onClick.Invoke();
                break;
            }
        }
    }
}