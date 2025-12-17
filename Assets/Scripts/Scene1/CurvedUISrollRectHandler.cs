using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CurvedUIScrollRectHandler : MonoBehaviour
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

    // [ID] Menyimpan ScrollRect yang sedang digunakan untuk drag/scroll.
    //      Diset ketika user pertama kali melakukan klik pada area ScrollRect.
    // [EN] Stores the ScrollRect currently being dragged.
    //      Set when the user first clicks on a ScrollRect area.
    private ScrollRect activeScroll;

    // [ID] Menandai apakah proses drag sedang berlangsung.
    //      True = drag sedang berjalan, false = drag selesai.
    // [EN] Indicates whether a drag operation is active.
    //      True = dragging is in progress, false = drag has ended.
    private bool isDraggingScroll = false;

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
        // [ID] Jika sedang drag ScrollRect → teruskan update drag
        // [EN] If a ScrollRect drag is active → update the drag
        if (isDraggingScroll && activeScroll != null)
        {
            DragScroll();
            return;
        }
    }

    private void DragScroll()
    {
        // [ID] Ambil posisi mouse terbaru untuk menghitung gerakan drag
        // [EN] Get the latest mouse position to calculate drag movement
        Vector2 currentMousePos = Mouse.current.position.ReadValue();

        // [ID] Hitung perubahan posisi (delta)
        // [EN] Compute movement delta
        pointerEventData.delta = currentMousePos - pointerEventData.position;
        pointerEventData.position = currentMousePos;

        // [ID] Kirim event drag ke ScrollRect
        // [EN] Send drag event to the ScrollRect
        ExecuteEvents.Execute(activeScroll.gameObject, pointerEventData, ExecuteEvents.dragHandler);

        // [ID] Jika mouse dilepas → akhiri drag
        // [EN] If the mouse button is released → end the drag
        if (!Mouse.current.leftButton.isPressed)
        {
            ExecuteEvents.Execute(activeScroll.gameObject, pointerEventData, ExecuteEvents.endDragHandler);
            ExecuteEvents.Execute(activeScroll.gameObject, pointerEventData, ExecuteEvents.pointerUpHandler);

            // [ID] Reset status drag ScrollRect
            // [EN] Reset ScrollRect drag state
            isDraggingScroll = false;
            activeScroll = null;
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
            // --- SCROLLRECT HANDLING ---
            ScrollRect scrollRect = result.gameObject.GetComponentInParent<ScrollRect>();

            if (scrollRect)
            {
                // [ID] Simpan ScrollRect aktif untuk proses drag di Update()
                // [EN] Store active ScrollRect for dragging in Update()
                activeScroll = scrollRect;
                isDraggingScroll = true;

                // [ID] Buat PointerEventData baru khusus untuk drag
                // [EN] Create new PointerEventData specifically for drag actions
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.button = PointerEventData.InputButton.Left;

                // [ID] Set posisi awal drag menggunakan posisi mouse saat ini
                // [EN] Set initial drag position using current mouse position
                Vector2 mousePos = Mouse.current.position.ReadValue();
                pointerEventData.pressPosition = mousePos;
                pointerEventData.position = mousePos;

                // [ID] Simpan informasi raycast yang diklik sebagai referensi selama drag
                // [EN] Store raycast info from the clicked element for drag reference
                pointerEventData.pointerPressRaycast = result;
                pointerEventData.pointerCurrentRaycast = result;
                pointerEventData.pointerDrag = scrollRect.gameObject;

                // [ID] Kirim event pointerDown → mulai menahan scroll area
                // [EN] Send pointerDown → beginning of scroll hold
                ExecuteEvents.Execute(scrollRect.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);

                // [ID] Kirim event beginDrag → Unity menyiapkan proses drag
                // [EN] Send beginDrag → Unity initializes drag behavior
                ExecuteEvents.Execute(scrollRect.gameObject, pointerEventData, ExecuteEvents.beginDragHandler);

                // [ID] Keluar dari fungsi scroll (tidak menjalankan UI lain)
                // [EN] Exit now because scroll action has begun
                return;
            }
        }
    }
}
