using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurvedUISliderHandler : MonoBehaviour
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

    // [ID] Menyimpan Slider yang sedang digunakan untuk drag/scroll.
    //      Diset ketika user pertama kali melakukan klik pada area Slider.
    // [EN] Stores the Slider currently being dragged.
    //      Set when the user first clicks on a Slider area.
    private Slider activeSlider;

    // [ID] Menandai apakah proses drag sedang berlangsung.
    //      True = drag sedang berjalan, false = drag selesai.
    // [EN] Indicates whether a drag operation is active.
    //      True = dragging is in progress, false = drag has ended.
    private bool isDraggingSlider = false;

    // Cached components
    private GraphicRaycaster canvasRaycaster; // [ID] Untuk melakukan raycast ke elemen UI di Canvas
                                              // [EN] Used to raycast against UI elements in the Canvas

    private PointerEventData pointerEventData; // [ID] Data pointer palsu untuk mensimulasikan klik UI
                                               // [EN] Fake pointer event data to simulate UI clicks

    private EventSystem eventSystem; // [ID] Event System utama Unity
                                     // [EN] Unity's main Event System


    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        if (vrInput != null)
            vrInput.action.started += OnVRPressed;
    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listener to avoid memory leaks
        if (vrInput != null)
            vrInput.action.started -= OnVRPressed;
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
        // [ID] Jika sedang drag Slider → teruskan update drag
        // [EN] If a Slider drag is active → update the drag
        if (isDraggingSlider && activeSlider != null)
        {
            DragSlider();
            return;
        }
    }

    private void DragSlider()
    {
        // [ID]
        // Fungsi ini dipanggil setiap frame ketika slider sedang di-drag.
        // Proses:
        //   1. Ambil UV posisi mouse pada mesh UI
        //   2. Konversi UV → posisi pixel canvas
        //   3. Update pointerEventData
        //   4. Kirim event drag agar handle slider bergerak
        //
        // [EN]
        // This function runs every frame while a slider drag is active.
        // Steps:
        //   1. Get mouse UV on the curved mesh
        //   2. Convert UV → canvas pixel coordinates
        //   3. Update pointerEventData
        //   4. Send drag event to move the slider handle
        if (isDraggingSlider && activeSlider != null)
        {
            // [ID] Ambil UV dari raycast mouse
            // [EN] Retrieve UV from mouse raycast
            if (TryGetMouseUV(out Vector2 uv))
            {
                // [ID] Konversi ke posisi pixel canvas
                // [EN] Convert to canvas pixel coordinates
                Vector2 canvasPos = UVToCanvasPixels(uv);

                // [ID] Update data drag pointer
                // [EN] Update pointer drag data
                pointerEventData.delta = canvasPos - pointerEventData.position;
                pointerEventData.position = canvasPos;

                // [ID] Jalankan event drag pada slider
                // [EN] Send drag event to the slider
                ExecuteEvents.Execute(activeSlider.gameObject, pointerEventData, ExecuteEvents.dragHandler);
            }

            // [ID] Kalau mouse dilepas → hentikan drag slider
            // [EN] If mouse is released → end slider drag
            if (!Mouse.current.leftButton.isPressed)
            {
                ExecuteEvents.Execute(activeSlider.gameObject, pointerEventData, ExecuteEvents.endDragHandler);
                ExecuteEvents.Execute(activeSlider.gameObject, pointerEventData, ExecuteEvents.pointerUpHandler);

                // [ID] Reset status slider
                // [EN] Reset slider state
                isDraggingSlider = false;
                activeSlider = null;
            }
        }
    }

    /// <summary>
    /// [ID]
    /// Mengambil koordinat UV berdasarkan posisi mouse menggunakan raycast ke curved mesh.
    /// UV inilah yang digunakan untuk menentukan posisi pointer di canvas UI.
    /// Mengembalikan true jika ray mengenai curvedMeshObj, false jika tidak.
    ///
    /// [EN]
    /// Gets the UV coordinates based on the mouse position by raycasting onto the curved mesh.
    /// These UVs are used to determine the pointer position on the UI canvas.
    /// Returns true if the ray hits the curvedMeshObj, otherwise false.
    /// </summary>
    private bool TryGetMouseUV(out Vector2 uv)
    {
        uv = Vector2.zero;

        // [ID] Ambil posisi mouse saat ini
        // [EN] Get current mouse position
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // [ID] Ray dari kamera menuju posisi mouse
        // [EN] Ray from camera toward mouse position
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        // [ID] Raycast hanya ke layer UI melengkung
        // [EN] Raycast only against the curved UI layer
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, uiLayerMask))
        {
            // [ID] Pastikan objek yang terkena adalah curvedMeshObj
            // [EN] Ensure the hit object is the curvedMeshObj
            if (hit.collider != null && hit.collider.gameObject == curvedMeshObj)
            {
                // [ID] Ambil koordinat UV dari hasil ray
                // [EN] Get UV coordinates from raycast hit
                uv = hit.textureCoord;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// [ID]
    /// Mengubah koordinat UV (0–1) menjadi posisi pixel yang sesuai pada Canvas.
    /// Jika Canvas menggunakan RenderTexture (mode worldCamera), gunakan ukuran RT.
    /// Jika tidak, gunakan pixelRect Canvas (fallback).
    ///
    /// [EN]
    /// Converts UV coordinates (0–1) into actual pixel positions on the Canvas.
    /// If the Canvas uses a RenderTexture (worldCamera mode), its size is used.
    /// Otherwise, Canvas pixelRect is used as a fallback.
    /// </summary>
    private Vector2 UVToCanvasPixels(Vector2 uv)
    {
        Canvas canvas = canvasRaycaster.GetComponent<Canvas>();
        RenderTexture rt = null;

        // [ID] Jika Canvas memakai worldCamera, ambil targetTexture-nya
        // [EN] If Canvas uses worldCamera, get its target RenderTexture
        if (canvas.worldCamera != null)
            rt = canvas.worldCamera.targetTexture;

        // [ID] Jika RenderTexture tersedia → gunakan ukuran RT
        // [EN] If RenderTexture exists → use RT resolution
        if (rt != null)
        {
            return new Vector2(uv.x * rt.width, uv.y * rt.height);
        }
        else
        {
            // [ID] Fallback: gunakan ukuran pixelRect Canvas (mode overlay)
            // [EN] Fallback: use Canvas pixelRect size (overlay mode)
            return new Vector2(uv.x * canvas.pixelRect.width, uv.y * canvas.pixelRect.height);
        }
    }

    // ============================================================
    // [ID] EVENT INPUT VR (trigger/klik VR)
    // [EN] VR INPUT EVENT (trigger/click)
    // ============================================================
    private void OnVRPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Panggil fungsi utama klik sama seperti input mouse
        // [EN] Call the click handler same as mouse version
        Debug.Log("VR INPUT TRIGGERED!");
        HandleClick();
    }

    /// <summary>
    /// [ID] Menangani klik dan melakukan raycast ke curved mesh.
    /// [EN] Handles mouse click and performs a raycast to the curved mesh.
    /// </summary>
    private void HandleClick()
    {
        // [ID] Membaca posisi mouse
        // [EN] Read mouse position
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // [ID] Membuat ray dari kamera ke arah posisi mouse
        // [EN] Create ray from camera toward the mouse position
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // [ID] Raycast hanya ke layer UI melengkung
        // [EN] Raycast only against the curved UI layer
        if (Physics.Raycast(ray, out hit, 100f, uiLayerMask))
        {
            // [ID] Validasi bahwa mesh yang terkena adalah curvedMeshObj
            // [EN] Check if the hit object is indeed the curvedMeshObj
            if (hit.collider.gameObject == curvedMeshObj)
            {
                // [ID] Mengambil koordinat UV dari titik benturan
                // [EN] Retrieve UV coordinates from the hit point
                Vector2 uvHit = hit.textureCoord;

                // [ID] Simulasikan klik UI berdasarkan UV
                // [EN] Simulate UI click based on UV hit
                SimulateUIClick(uvHit);
            }
        }
    }

    /// <summary>
    /// [ID] Mensimulasikan klik pada UI Canvas menggunakan koordinat UV dari curved mesh.
    /// [EN] Simulates a click on the Canvas UI using UV coordinates from the curved mesh.
    /// </summary>
    private void SimulateUIClick(Vector2 uvCoords)
    {
        // [ID] Buat event pointer baru
        // [EN] Create a new pointer event
        pointerEventData = new PointerEventData(eventSystem);

        // [ID] Hitung posisi pointer berdasarkan UV * pixelRect Canvas
        // [EN] Calculate pointer position based on UV * Canvas pixelRect
        pointerEventData.position = new Vector2(
            uvCoords.x * canvasRaycaster.GetComponent<Canvas>().pixelRect.width,
            uvCoords.y * canvasRaycaster.GetComponent<Canvas>().pixelRect.height
        );

        // [ID] Simpan hasil raycast UI
        // [EN] Store UI raycast results
        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        // [ID] Eksekusi klik pada elemen UI teratas yang valid
        // [EN] Execute click on the first valid UI element
        foreach (RaycastResult result in results)
        {
            // --- SLIDER HANDLING ---
            // [ID]
            // Gunakan GetComponentInParent<Slider>() karena raycast biasanya mengenai objek anak
            // (handle / fill) dan bukan komponen Slider utama.
            // Jika slider terdeteksi:
            //   1. Simpan slider sebagai slider aktif (activeSlider)
            //   2. Aktifkan mode drag slider
            //   3. Siapkan pointerEventData agar Unity mengenali proses drag
            //   4. Kirim event pointerDown & beginDrag pada HANDLE agar slider merespon secara normal
            //
            // [EN]
            // Use GetComponentInParent<Slider>() because raycasts often hit child objects
            // (handle / fill) instead of the main Slider component.
            // When a slider is detected:
            //   1. Store the slider as the active slider
            //   2. Enable slider drag mode
            //   3. Prepare pointerEventData for Unity's drag system
            //   4. Send pointerDown & beginDrag events to the HANDLE so the slider behaves naturally
            Slider slider = result.gameObject.GetComponentInParent<Slider>();
            if (slider)
            {
                activeSlider = slider;
                isDraggingSlider = true;

                // [ID] Ambil objek handle (bagian yang benar-benar di-drag)
                // [EN] Get the handle object (the real draggable part)
                GameObject handleObj = slider.handleRect.gameObject;

                // [ID] Siapkan pointerEventData untuk memulai drag
                // [EN] Prepare pointerEventData for drag initialization
                pointerEventData.pointerPressRaycast = result;
                pointerEventData.pointerCurrentRaycast = result;
                pointerEventData.pointerDrag = handleObj;

                // [ID] Wajib: pointerDown pada handle agar drag dapat dimulai
                // [EN] Required: pointerDown on handle so drag can begin
                ExecuteEvents.Execute(handleObj, pointerEventData, ExecuteEvents.pointerDownHandler);

                // [ID] Memasuki mode beginDrag
                // [EN] Enter beginDrag mode
                ExecuteEvents.Execute(handleObj, pointerEventData, ExecuteEvents.beginDragHandler);

                Debug.Log("Slider drag started on HANDLE: " + handleObj.name);

                return;
            }

        }
    }
}
