using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurvedUIScrollbarHandler : MonoBehaviour
{
    [Header("References")]
    private Camera mainCamera;        // [ID] Kamera utama yang digunakan untuk raycast
                                      // [EN] Main camera used for raycasting

    [SerializeField] private GameObject curvedMeshObj; // [ID] Mesh 3D yang menampilkan UI melengkung (harus memiliki MeshCollider)
                                                       // [EN] 3D mesh that displays the curved UI (must have a MeshCollider)

    [SerializeField] private Canvas sourceCanvas;      // [ID] Canvas asli yang dirender menjadi RenderTexture
                                                       // [EN] Original canvas rendered as a RenderTexture


    [Header("Settings")]
    [SerializeField] private LayerMask uiLayerMask;    // [ID] Layer tempat curved mesh berada agar raycast lebih terfokus
                                                       // [EN] Layer where the curved mesh is placed for accurate raycasting


    [Header("VR Input")]
    [SerializeField] private InputActionReference vrInput; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                           // [EN] VR Input used to detect click/trigger events inside VR

    // [ID] Variabel internal untuk melacak status drag
    // [EN] Internal variables to track drag state
    private Scrollbar activeScrollbar;
    private bool isDragging;

    // [ID] Komponen cached untuk performa
    // [EN] Cached components for performance
    private GraphicRaycaster canvasRaycaster;
    private EventSystem eventSystem;

    // ----------------------------------------------------
    // UNITY LIFECYCLE
    // ----------------------------------------------------
    private void OnEnable()
    {
        // [ID] Mengaktifkan listener input VR saat skrip aktif
        // [EN] Enable VR input listener when script is active
        if (vrInput != null)
        {
            vrInput.action.Enable();
            vrInput.action.started += OnPressed;
        }
    }

    private void OnDisable()
    {
        // [ID] Membersihkan listener saat skrip mati untuk mencegah memory leak
        // [EN] Cleanup listener when script is disabled to prevent memory leaks
        if (vrInput != null)
            vrInput.action.started -= OnPressed;
    }

    private void Start()
    {
        // [ID] Ambil komponen Raycaster dari Canvas
        // [EN] Cache the Canvas Raycaster
        canvasRaycaster = sourceCanvas.GetComponent<GraphicRaycaster>();

        // [ID] Ambil EventSystem aktif
        // [EN] Get the active EventSystem
        eventSystem = EventSystem.current;

        // [ID] Jika kamera belum di-assign, gunakan MainCamera
        // [EN] Auto-assign MainCamera if none provided
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        // [ID] Jika sedang drag dan ada scrollbar aktif, jalankan logika drag
        // [EN] If dragging and there is an active scrollbar, run drag logic
        if (isDragging && activeScrollbar != null)
        {
            DragScrollbar();
        }
    }

    // ----------------------------------------------------
    // INPUT CHECK
    // ----------------------------------------------------
    /// <summary>
    /// [ID] Memeriksa apakah tombol input (Mouse Kiri atau Tombol VR) sedang ditahan.
    /// [EN] Checks if input button (Left Mouse or VR Button) is currently held down.
    /// </summary>
    private bool IsInputPressed()
    {
        // 1. Cek Mouse / Check Mouse
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return true;

        // 2. Cek VR / Check VR
        if (vrInput != null && vrInput.action != null && vrInput.action.IsPressed())
            return true;

        return false;
    }

    // ----------------------------------------------------
    // PRESS HANDLER (DETECTION)
    // ----------------------------------------------------
    private void OnPressed(InputAction.CallbackContext ctx)
    {
        TryPress();
    }

    private void TryPress()
    {
        // [ID] Ambil posisi mouse saat ini
        // [EN] Get current mouse position
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        // [ID] Lakukan raycast ke layer UI
        // [EN] Perform raycast to UI layer
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, uiLayerMask))
            return;

        // [ID] Pastikan yang terkena adalah layar lengkung kita
        // [EN] Ensure the hit object is our curved screen
        if (hit.collider.gameObject != curvedMeshObj)
            return;

        // [ID] Lanjutkan ke simulasi tekan menggunakan koordinat UV
        // [EN] Proceed to simulate press using UV coordinates
        SimulatePress(hit.textureCoord);
    }

    // ----------------------------------------------------
    // INITIAL PRESS (FIND SCROLLBAR)
    // ----------------------------------------------------
    private void SimulatePress(Vector2 uv)
    {
        PointerEventData ped = new PointerEventData(eventSystem);
        RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();

        // [ID] Konversi UV (0-1) menjadi posisi Pixel Canvas (misal 1920x1080)
        // [EN] Convert UV (0-1) to Canvas Pixel position (e.g., 1920x1080)
        Vector2 canvasPos = new Vector2(
            uv.x * canvasRect.rect.width,
            uv.y * canvasRect.rect.height
        );

        ped.position = canvasPos;
        ped.pressPosition = canvasPos;
        ped.button = PointerEventData.InputButton.Left;

        // [ID] Raycast UI secara internal untuk mencari elemen di posisi tersebut
        // [EN] Internal UI Raycast to find elements at that position
        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(ped, results);

        foreach (RaycastResult r in results)
        {
            // [ID] Cari komponen Scrollbar di parent objek yang diklik
            // [EN] Find Scrollbar component in the parent of the clicked object
            Scrollbar sb = r.gameObject.GetComponentInParent<Scrollbar>();
            if (sb == null)
                continue;

            // [ID] Scrollbar ditemukan! Set status menjadi dragging
            // [EN] Scrollbar found! Set state to dragging
            activeScrollbar = sb;
            isDragging = true;

            Debug.Log("✅ Scrollbar Drag START: " + sb.name);
            return;
        }
    }

    // ----------------------------------------------------
    // DRAG LOGIC (CORE FIX)
    // ----------------------------------------------------
    private void DragScrollbar()
    {
        // [ID] Jika tombol dilepas, hentikan drag
        // [EN] If button is released, stop dragging
        if (!IsInputPressed())
        {
            EndDrag();
            return;
        }

        // [ID] Ambil posisi UV terbaru (Raycast ulang setiap frame)
        // [EN] Get latest UV position (Re-raycast every frame)
        if (!TryGetMouseUV(out Vector2 uv))
            return;

        RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();

        // [ID] Hitung posisi pixel absolut di Canvas
        // [EN] Calculate absolute pixel position on Canvas
        Vector2 canvasPos = new Vector2(
            uv.x * canvasRect.rect.width,
            uv.y * canvasRect.rect.height
        );

        float value = activeScrollbar.value;

        // ==========================================
        // [ID] PERHITUNGAN NILAI SCROLLBAR (SOLUSI UTAMA)
        // [EN] SCROLLBAR VALUE CALCULATION (CORE FIX)
        // ==========================================
        // [ID]
        // Alih-alih menggunakan delta (perubahan gerak), kita menghitung posisi Handle
        // secara absolut berdasarkan koordinat Canvas (0.0 sampai 1.0).
        //
        // [EN]
        // Instead of using delta (motion change), we calculate the Handle position
        // absolutely based on Canvas coordinates (0.0 to 1.0).

        switch (activeScrollbar.direction)
        {
            case Scrollbar.Direction.LeftToRight:
                // [ID] Normalisasi posisi X menjadi nilai 0-1
                // [EN] Normalize X position to 0-1 value
                value = Mathf.Clamp01(canvasPos.x / canvasRect.rect.width);
                break;

            case Scrollbar.Direction.RightToLeft:
                // [ID] Kebalikan dari LeftToRight (1 - X)
                // [EN] Inverse of LeftToRight (1 - X)
                value = 1f - Mathf.Clamp01(canvasPos.x / canvasRect.rect.width);
                break;

            case Scrollbar.Direction.BottomToTop:
                // [ID] Normalisasi posisi Y menjadi nilai 0-1
                // [EN] Normalize Y position to 0-1 value
                value = Mathf.Clamp01(canvasPos.y / canvasRect.rect.height);
                break;

            case Scrollbar.Direction.TopToBottom:
                // [ID] Kebalikan dari BottomToTop (1 - Y)
                // [EN] Inverse of BottomToTop (1 - Y)
                value = 1f - Mathf.Clamp01(canvasPos.y / canvasRect.rect.height);
                break;
        }

        // [ID] Terapkan nilai langsung ke Scrollbar
        // [EN] Apply value directly to the Scrollbar
        activeScrollbar.value = value;
    }

    private void EndDrag()
    {
        Debug.Log("🛑 Scrollbar Drag END");
        activeScrollbar = null;
        isDragging = false;
    }

    // ----------------------------------------------------
    // UTILITY
    // ----------------------------------------------------
    private bool TryGetMouseUV(out Vector2 uv)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        // [ID] Raycast fisik ke Mesh Lengkung
        // [EN] Physics raycast to Curved Mesh
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, uiLayerMask))
        {
            if (hit.collider.gameObject == curvedMeshObj)
            {
                uv = hit.textureCoord;
                return true;
            }
        }

        uv = Vector2.zero;
        return false;
    }
}