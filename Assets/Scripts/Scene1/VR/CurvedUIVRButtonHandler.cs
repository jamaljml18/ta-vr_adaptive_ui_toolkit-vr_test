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
        // [ID] Mendaftarkan event listener saat tombol Trigger (Select) ditekan
        // [EN] Register event listener when Trigger (Select) button is pressed
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

    // ============================================================
    // [ID] LOGIKA SIMULASI KLIK UI
    // [EN] UI CLICK SIMULATION LOGIC
    // ============================================================
    private void SimulateUIClick(Vector2 uvCoords)
    {
        pointerEventData = new PointerEventData(eventSystem);

        // [ID] Ambil kamera yang digunakan untuk me-render Canvas ini
        // [EN] Get the camera used to render this Canvas
        Camera uiCam = sourceCanvas.worldCamera;

        if (uiCam != null)
        {
            // [ID] PENTING: Gunakan resolusi Pixel Kamera (Render Texture) untuk akurasi
            // Ini memastikan klik jatuh di titik yang benar meskipun Canvas di-scale
            // [EN] CRITICAL: Use Camera Pixel resolution (Render Texture) for accuracy
            // This ensures the click lands correctly even if the Canvas is scaled
            pointerEventData.position = new Vector2(
                uvCoords.x * uiCam.pixelWidth,
                uvCoords.y * uiCam.pixelHeight
            );
        }
        else
        {
            // [ID] Fallback: Gunakan ukuran Rect Canvas jika kamera hilang (kurang akurat untuk Render Texture)
            // [EN] Fallback: Use Canvas Rect size if camera is missing (less accurate for Render Texture)
            RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();
            pointerEventData.position = new Vector2(
                uvCoords.x * canvasRect.rect.width,
                uvCoords.y * canvasRect.rect.height
            );
            Debug.LogWarning("[CurvedUIVR] Using Canvas Rect fallback. Assign Render Camera to Canvas for better accuracy.");
        }

        if (showDebugLogs)
            Debug.Log($"[CurvedUIVR] Mapping UV {uvCoords} to Pixel Position: {pointerEventData.position}");

        // [ID] Lakukan Raycast UI Virtual pada posisi pixel yang sudah dihitung
        // [EN] Perform Virtual UI Raycast at the calculated pixel position
        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        // [ID] Loop hasil raycast untuk mencari tombol
        // [EN] Loop through raycast results to find a button
        foreach (RaycastResult result in results)
        {
            // [ID] Cek apakah objek memiliki komponen Button
            // [EN] Check if object has a Button component
            Button btn = result.gameObject.GetComponent<Button>();

            if (btn != null)
            {
                // [ID] Visual feedback kedua: Menandakan tombol UI berhasil ditemukan dan diklik
                // [EN] Second visual feedback: Indicates UI button was found and clicked
                if (cubeDebug2 != null) cubeDebug2.SetActive(!cubeDebug2.activeSelf);

                // [ID] Panggil fungsi OnClick tombol tersebut secara virtual
                // [EN] Invoke the button's OnClick function virtually
                btn.onClick.Invoke();

                if (showDebugLogs) Debug.Log("[CurvedUIVR] BUTTON CLICKED: " + result.gameObject.name);

                // [ID] Berhenti setelah menekan tombol teratas (agar tidak menembus ke tombol di belakangnya)
                // [EN] Break after clicking the topmost button (to avoid clicking buttons behind it)
                break;
            }

            // [ID] Catatan: Anda bisa menambahkan logika untuk Toggle, Slider, atau InputField di sini jika perlu
            // [EN] Note: You can add logic for Toggles, Sliders, or InputFields here if needed
        }
    }
}