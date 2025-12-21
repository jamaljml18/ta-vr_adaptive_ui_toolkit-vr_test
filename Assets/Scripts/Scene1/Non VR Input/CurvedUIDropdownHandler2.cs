using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// [ID] Menangani interaksi UI pada Mesh Melengkung (Curved UI) menggunakan Render Texture di VR.
/// [EN] Handles UI interactions on Curved Meshes using Render Textures in VR.
/// </summary>
public class CurvedUIDropdownHandler2 : MonoBehaviour
{
    #region REFERENCES
    [Header("References")]
    private Camera mainCamera;        // [ID] Kamera utama yang digunakan untuk raycast
                                      // [EN] Main camera used for raycasting

    [SerializeField] private GameObject curvedMeshObj; // [ID] Mesh 3D yang menampilkan UI melengkung (harus memiliki MeshCollider)
                                                       // [EN] 3D mesh that displays the curved UI (must have a MeshCollider)

    [SerializeField] private Canvas sourceCanvas;      // [ID] Canvas asli yang dirender menjadi RenderTexture
                                                       // [EN] Original canvas rendered as a RenderTexture
    #endregion

    #region DEBUG & CALIBRATION
    [Header("Debug Visual")]
    // [ID] Prefab bola merah untuk melihat posisi klik raycast (Sangat berguna untuk debugging).
    // [EN] Red sphere prefab to visualize raycast click position (Very useful for debugging).
    [SerializeField] private GameObject debugSphere;

    [Header("Calibration")]
    [Tooltip("Adjust Y position if click hits above/below target.")]
    // [ID] Offset Y: Ubah nilai ini jika klik selalu meleset ke Atas/Bawah.
    // [EN] Y Offset: Change this value if clicks always miss Up/Down.
    [SerializeField] private float yOffset = 0f;

    [Tooltip("Adjust X position if click hits left/right target.")]
    // [ID] Offset X: Ubah nilai ini jika klik selalu meleset ke Kiri/Kanan.
    // [EN] X Offset: Change this value if clicks always miss Left/Right.
    [SerializeField] private float xOffset = 0f;
    #endregion

    #region SETTINGS & INPUT
    [Header("Settings")]
    [SerializeField] private LayerMask uiLayerMask;    // [ID] Layer tempat curved mesh berada agar raycast lebih terfokus
                                                       // [EN] Layer where the curved mesh is placed for accurate raycasting


    [Header("VR Input")]
    [SerializeField] private InputActionReference vrInput; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                           // [EN] VR Input used to detect click/trigger events inside VR
    #endregion

    // Private State Variables
    private TMP_Dropdown activeTMPDropdown = null;
    private GraphicRaycaster mainCanvasRaycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    #region UNITY LIFECYCLE
    private void OnEnable()
    {
        if (vrInput != null) vrInput.action.started += OnVRPressed;
    }

    private void OnDisable()
    {
        if (vrInput != null) vrInput.action.started -= OnVRPressed;
    }

    private void Start()
    {
        mainCanvasRaycaster = sourceCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (mainCamera == null) mainCamera = Camera.main;
    }
    #endregion

    #region INPUT HANDLING
    private void OnVRPressed(InputAction.CallbackContext ctx)
    {
        HandleClick();
    }

    /// <summary>
    /// [ID] Melakukan Raycast Fisika ke Mesh 3D untuk mendapatkan koordinat UV.
    /// [EN] Performs Physics Raycast to the 3D Mesh to get UV coordinates.
    /// </summary>
    private void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // [ID] Cek apakah raycast mengenai layer UI 3D
        // [EN] Check if raycast hits the 3D UI layer
        if (Physics.Raycast(ray, out hit, 100f, uiLayerMask))
        {
            if (hit.collider.gameObject == curvedMeshObj)
            {
                // [ID] Visualisasi Debug: Pindahkan bola ke titik sentuh
                // [EN] Debug Visualization: Move sphere to hit point
                if (debugSphere != null)
                {
                    debugSphere.transform.position = hit.point;
                    debugSphere.SetActive(true);
                }

                // [ID] Lanjutkan ke simulasi klik UI 2D
                // [EN] Proceed to simulate 2D UI click
                SimulateUIClick(hit.textureCoord);
            }
        }
    }
    #endregion

    #region UI SIMULATION CORE
    /// <summary>
    /// [ID] Menerjemahkan UV Mesh menjadi posisi Pixel Canvas dan melakukan Raycast UI.
    /// [EN] Translates Mesh UVs into Canvas Pixel positions and performs UI Raycast.
    /// </summary>
    private void SimulateUIClick(Vector2 uvCoords)
    {
        // --------------------------------------------------------------------
        // 1. SETUP & COORDINATE CALCULATION
        // --------------------------------------------------------------------
        if (eventSystem == null) eventSystem = EventSystem.current;
        if (sourceCanvas == null) return;

        pointerEventData = new PointerEventData(eventSystem);
        RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();

        // [ID] Hitung posisi X/Y berdasarkan lebar/tinggi Canvas
        // [EN] Calculate X/Y position based on Canvas width/height
        float finalX = (uvCoords.x * canvasRect.rect.width);
        float finalY = (uvCoords.y * canvasRect.rect.height);

        // [ID] Terapkan Offset Kalibrasi
        // [EN] Apply Calibration Offset
        finalX += xOffset;
        finalY += yOffset;

        pointerEventData.position = new Vector2(finalX, finalY);
        pointerEventData.delta = Vector2.zero;
        pointerEventData.button = PointerEventData.InputButton.Left;
        pointerEventData.clickCount = 1;


        // --------------------------------------------------------------------
        // 2. MULTI-RAYCAST STRATEGY (CRITICAL FIX)
        // --------------------------------------------------------------------
        // [ID] Kita perlu melakukan Raycast ke Canvas Utama DAN ke Dropdown List (jika ada).
        // [EN] We need to Raycast to the Main Canvas AND the Dropdown List (if it exists).

        List<RaycastResult> finalResults = new List<RaycastResult>();

        // A. Raycast Main Canvas
        mainCanvasRaycaster.Raycast(pointerEventData, finalResults);

        // B. Raycast Dropdown List (Runtime Object)
        // [ID] Dropdown List punya Raycaster sendiri karena Override Sorting.
        // [EN] Dropdown List has its own Raycaster due to Override Sorting.
        GameObject dropdownListObj = GameObject.Find("Dropdown List");
        if (dropdownListObj != null)
        {
            GraphicRaycaster dropdownRaycaster = dropdownListObj.GetComponent<GraphicRaycaster>();
            if (dropdownRaycaster != null)
            {
                List<RaycastResult> dropdownResults = new List<RaycastResult>();
                dropdownRaycaster.Raycast(pointerEventData, dropdownResults);

                // [ID] Masukkan hasil dropdown di urutan teratas (Prioritas)
                // [EN] Insert dropdown results at the top (Priority)
                if (dropdownResults.Count > 0)
                {
                    finalResults.InsertRange(0, dropdownResults);
                }
            }
        }

        if (finalResults.Count == 0) return; // [ID] Tidak kena UI apapun / [EN] No UI hit

        bool inputHandled = false;


        // --------------------------------------------------------------------
        // 3. LOGIC: ITEM DETECTION & FORCE UPDATE
        // --------------------------------------------------------------------
        foreach (RaycastResult result in finalResults)
        {
            GameObject hitObj = result.gameObject;
            if (hitObj == null || hitObj.name == "Blocker") continue;

            // [ID] Cek apakah objek ini bagian dari Item Dropdown (Toggle/Text)
            // [EN] Check if object is part of a Dropdown Item (Toggle/Text)
            Toggle itemToggle = hitObj.GetComponentInParent<Toggle>();
            bool isItemName = hitObj.name.Contains("Item");

            if (activeTMPDropdown != null && (itemToggle != null || isItemName))
            {
                // [ID] Ambil komponen Text untuk identifikasi Item
                // [EN] Get Text component to identify the Item
                TMP_Text itemText = hitObj.GetComponentInChildren<TMP_Text>();
                if (itemText == null && itemToggle != null) itemText = itemToggle.GetComponentInChildren<TMP_Text>();

                if (itemText != null)
                {
                    Debug.Log($"[CurvedUI] 🎯 Target Text Found: {itemText.text}");

                    // [ID] Cari Index opsi yang cocok dengan teks
                    // [EN] Find Option Index matching the text
                    int index = -1;
                    for (int i = 0; i < activeTMPDropdown.options.Count; i++)
                    {
                        if (activeTMPDropdown.options[i].text == itemText.text)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        // [ID] PAKSA UPDATE VALUE (Force Update)
                        // [EN] FORCE UPDATE VALUE
                        activeTMPDropdown.value = index;
                        activeTMPDropdown.onValueChanged.Invoke(index);
                        activeTMPDropdown.RefreshShownValue();

                        Debug.Log($"[CurvedUI] ✅ Success: Value changed to {itemText.text}");

                        // [ID] Tutup Dropdown & Selesai
                        // [EN] Close Dropdown & Finish
                        activeTMPDropdown.Hide();
                        activeTMPDropdown = null;
                        inputHandled = true;
                        return;
                    }
                }
            }
        }


        // --------------------------------------------------------------------
        // 4. LOGIC: HEADER INTERACTION
        // --------------------------------------------------------------------
        if (!inputHandled)
        {
            foreach (RaycastResult result in finalResults)
            {
                TMP_Dropdown dropdown = result.gameObject.GetComponentInParent<TMP_Dropdown>();

                // [ID] Pastikan kita mengklik Header, bukan isi list
                // [EN] Ensure we clicked the Header, not the list content
                if (dropdown != null && !IsPartOfDropdownList(result.gameObject))
                {
                    if (activeTMPDropdown == dropdown)
                    {
                        // [ID] Klik Header lagi -> Tutup
                        // [EN] Click Header again -> Close
                        dropdown.Hide();
                        activeTMPDropdown = null;
                    }
                    else
                    {
                        // [ID] Klik Header -> Buka
                        // [EN] Click Header -> Open
                        if (activeTMPDropdown != null) activeTMPDropdown.Hide();

                        ExecuteEvents.Execute(dropdown.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                        activeTMPDropdown = dropdown;
                        Debug.Log("[CurvedUI] 🔼 Dropdown Opened");
                    }
                    return;
                }
            }

            // --------------------------------------------------------------------
            // 5. LOGIC: OUTSIDE CLICK (CLOSE)
            // --------------------------------------------------------------------
            if (activeTMPDropdown != null)
            {
                bool hitListPart = false;
                foreach (var r in finalResults)
                {
                    // [ID] Cek jika kita mengenai Scrollbar atau background list -> Jangan tutup
                    // [EN] Check if we hit Scrollbar or list background -> Do not close
                    if (IsPartOfDropdownList(r.gameObject) || r.gameObject.name.Contains("Scroll"))
                        hitListPart = true;
                }

                if (!hitListPart)
                {
                    // [ID] Benar-benar klik di luar -> Tutup
                    // [EN] Valid outside click -> Close
                    activeTMPDropdown.Hide();
                    activeTMPDropdown = null;
                    Debug.Log("[CurvedUI] ❌ Closed (Outside Click)");
                }
            }
        }


        // --------------------------------------------------------------------
        // 6. LOGIC: GENERAL UI (BUTTONS/SLIDERS)
        // --------------------------------------------------------------------
        if (!inputHandled && finalResults.Count > 0)
        {
            GameObject go = finalResults[0].gameObject;
            if (go != null && go.name != "Blocker")
            {
                ExecuteEvents.Execute(go, pointerEventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
    #endregion

    #region HELPER METHODS
    /// <summary>
    /// [ID] Mengecek secara rekursif apakah objek adalah bagian dari "Dropdown List".
    /// [EN] Recursively checks if the object is part of the "Dropdown List".
    /// </summary>
    private bool IsPartOfDropdownList(GameObject obj)
    {
        if (obj == null) return false;
        Transform t = obj.transform;
        while (t != null)
        {
            if (t.name == "Dropdown List") return true;
            t = t.parent;
        }
        return false;
    }
    #endregion
}