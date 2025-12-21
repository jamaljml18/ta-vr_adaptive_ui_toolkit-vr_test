using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// [ID] Menangani interaksi UI pada Mesh Melengkung (Curved UI) dengan fokus khusus pada perbaikan Bug Dropdown (TMP).
/// [EN] Handles UI interactions on Curved Meshes with a specific focus on fixing Dropdown (TMP) interaction bugs.
/// </summary>
public class CurvedUIDropdownHandler : MonoBehaviour
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

    #region CALIBRATION
    [Header("Calibration")]
    [Tooltip("Adjust Y position if click hits above/below target.")]
    // [ID] Offset Y: Gunakan ini jika titik klik terasa meleset ke atas/bawah
    // [EN] Y Offset: Use this if the click point feels offset vertically
    [SerializeField] private float yOffset = 0f;

    [Tooltip("Adjust X position if click hits left/right target.")]
    // [ID] Offset X: Gunakan ini jika titik klik terasa meleset ke kiri/kanan
    // [EN] X Offset: Use this if the click point feels offset horizontally
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
    private TMP_Dropdown activeTMPDropdown = null; // [ID] Melacak dropdown yang sedang terbuka / [EN] Tracks currently open dropdown
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
    /// [ID] Melakukan Raycast fisik dari Kamera ke Mesh Lengkung untuk mendapatkan koordinat UV.
    /// [EN] Performs a physical Raycast from Camera to the Curved Mesh to retrieve UV coordinates.
    /// </summary>
    private void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, uiLayerMask))
        {
            // [ID] Pastikan kita mengenai layar lengkung yang benar
            // [EN] Ensure we hit the correct curved screen
            if (hit.collider.gameObject == curvedMeshObj)
            {
                // [ID] Kirim koordinat tekstur (UV) ke simulasi UI
                // [EN] Send texture coordinates (UV) to UI simulation
                SimulateUIClick(hit.textureCoord);
            }
        }
    }
    #endregion

    #region UI SIMULATION CORE
    /// <summary>
    /// [ID] Inti logika: Mengubah UV menjadi klik UI Pixel, menangani logika Dropdown yang rumit.
    /// [EN] Core logic: Converts UV to UI Pixel click, handles complex Dropdown logic.
    /// </summary>
    private void SimulateUIClick(Vector2 uvCoords)
    {
        // ---------------------------------------------------------
        // 1. SETUP & CALCULATION (UV -> PIXELS)
        // ---------------------------------------------------------
        if (eventSystem == null) eventSystem = EventSystem.current;
        if (sourceCanvas == null) return;

        pointerEventData = new PointerEventData(eventSystem);
        RectTransform canvasRect = sourceCanvas.GetComponent<RectTransform>();

        // [ID] Hitung posisi pixel absolut pada Canvas berdasarkan UV (0-1)
        // [EN] Calculate absolute pixel position on Canvas based on UV (0-1)
        float finalX = (uvCoords.x * canvasRect.rect.width);
        float finalY = (uvCoords.y * canvasRect.rect.height);

        // [ID] Terapkan kalibrasi manual
        // [EN] Apply manual calibration
        finalX += xOffset;
        finalY += yOffset;

        pointerEventData.position = new Vector2(finalX, finalY);
        pointerEventData.delta = Vector2.zero;
        pointerEventData.button = PointerEventData.InputButton.Left;
        pointerEventData.clickCount = 1; // [ID] Penting untuk deteksi klik valid / [EN] Important for valid click detection

        // ---------------------------------------------------------
        // 2. MULTI-RAYCAST STRATEGY (MAIN CANVAS + DROPDOWN LAYER)
        // ---------------------------------------------------------
        List<RaycastResult> finalResults = new List<RaycastResult>();

        // A. Raycast Main Canvas (Normal UI)
        mainCanvasRaycaster.Raycast(pointerEventData, finalResults);

        // B. Raycast Dropdown List (Special Runtime Object)
        // [ID]
        // Unity membuat objek bernama "Dropdown List" di root scene saat dropdown dibuka.
        // Objek ini seringkali tidak terdeteksi oleh Raycaster canvas utama karena perbedaan sorting layer.
        // Kita harus mencarinya secara manual dan melakukan raycast terpisah.
        //
        // [EN]
        // Unity creates an object named "Dropdown List" at scene root when opened.
        // This object is often missed by the main canvas Raycaster due to sorting layer differences.
        // We must find it manually and raycast it separately.

        GameObject dropdownListObj = GameObject.Find("Dropdown List");
        if (dropdownListObj != null)
        {
            GraphicRaycaster dropdownRaycaster = dropdownListObj.GetComponent<GraphicRaycaster>();
            if (dropdownRaycaster != null)
            {
                List<RaycastResult> dropdownResults = new List<RaycastResult>();
                dropdownRaycaster.Raycast(pointerEventData, dropdownResults);

                // [ID] Jika kena Dropdown List, masukkan ke prioritas utama (index 0)
                // [EN] If Dropdown List is hit, insert as top priority (index 0)
                if (dropdownResults.Count > 0)
                {
                    finalResults.InsertRange(0, dropdownResults);
                }
            }
        }

        if (finalResults.Count == 0) return;

        bool inputHandled = false;

        // ---------------------------------------------------------
        // 3. LOGIC: DROPDOWN ITEM SELECTION
        // ---------------------------------------------------------
        foreach (RaycastResult result in finalResults)
        {
            GameObject hitObj = result.gameObject;

            // [ID] Abaikan Blocker (objek transparan penutup layar Unity)
            // [EN] Ignore Blocker (Unity's transparent screen-covering object)
            if (hitObj == null || hitObj.name == "Blocker") continue;

            // [ID] Deteksi apakah yang diklik adalah Item dalam Dropdown
            // [EN] Detect if the clicked object is an Item inside a Dropdown
            Toggle itemToggle = hitObj.GetComponentInParent<Toggle>();
            bool isItemName = hitObj.name.Contains("Item");

            if (activeTMPDropdown != null && (itemToggle != null || isItemName))
            {
                // [ID] Ambil komponen Text untuk mencocokkan konten
                // [EN] Get Text component to match content
                TMP_Text itemText = hitObj.GetComponentInChildren<TMP_Text>();
                if (itemText == null && itemToggle != null) itemText = itemToggle.GetComponentInChildren<TMP_Text>();

                if (itemText != null)
                {
                    // [ID]
                    // PENCARIAN MANUAL: Kita cari text item yang diklik di dalam daftar opsi Dropdown.
                    // Ini diperlukan karena event onClick Unity sering gagal pada Render Texture.
                    //
                    // [EN]
                    // MANUAL SEARCH: We find the clicked item text within the Dropdown options list.
                    // This is required because Unity's onClick events often fail on Render Textures.

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
                        // [ID] Update nilai dropdown secara paksa
                        // [EN] Force update dropdown value
                        activeTMPDropdown.value = index;
                        activeTMPDropdown.onValueChanged.Invoke(index);
                        activeTMPDropdown.RefreshShownValue();

                        // [ID] Tutup dropdown dan tandai input selesai
                        // [EN] Close dropdown and mark input as handled
                        activeTMPDropdown.Hide();
                        activeTMPDropdown = null;
                        inputHandled = true;
                        return;
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // 4. LOGIC: HEADER & OUTSIDE CLICK
        // ---------------------------------------------------------
        if (!inputHandled)
        {
            foreach (RaycastResult result in finalResults)
            {
                TMP_Dropdown dropdown = result.gameObject.GetComponentInParent<TMP_Dropdown>();

                // [ID] Cek Interaksi Header Dropdown (Membuka/Menutup)
                // [EN] Check Dropdown Header Interaction (Opening/Closing)
                if (dropdown != null && !IsPartOfDropdownList(result.gameObject))
                {
                    if (activeTMPDropdown == dropdown)
                    {
                        // [ID] Klik header saat terbuka -> Tutup
                        // [EN] Click header while open -> Close
                        dropdown.Hide();
                        activeTMPDropdown = null;
                    }
                    else
                    {
                        // [ID] Klik header baru -> Buka (tutup yang lama jika ada)
                        // [EN] Click new header -> Open (close old one if exists)
                        if (activeTMPDropdown != null) activeTMPDropdown.Hide();

                        ExecuteEvents.Execute(dropdown.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                        activeTMPDropdown = dropdown;
                    }
                    return; // [ID] Selesai, jangan proses klik lain / [EN] Done, don't process other clicks
                }
            }

            // [ID] Logika Klik di Luar: Tutup dropdown jika klik sembarang tempat
            // [EN] Outside Click Logic: Close dropdown if clicking elsewhere
            if (activeTMPDropdown != null)
            {
                bool hitListPart = false;
                foreach (var r in finalResults)
                {
                    // [ID] Cek apakah kita masih mengklik area scrollbar/list milik dropdown
                    // [EN] Check if we are still clicking the dropdown's scrollbar/list area
                    if (IsPartOfDropdownList(r.gameObject) || r.gameObject.name.Contains("Scroll"))
                        hitListPart = true;
                }

                if (!hitListPart)
                {
                    activeTMPDropdown.Hide();
                    activeTMPDropdown = null;
                }
            }
        }

        // ---------------------------------------------------------
        // 5. GENERAL UI (BUTTONS, TOGGLES, SLIDERS)
        // ---------------------------------------------------------
        // [ID] Jika bukan interaksi dropdown khusus, jalankan klik UI standar Unity
        // [EN] If not specific dropdown interaction, execute standard Unity UI click
        if (!inputHandled && finalResults.Count > 0)
        {
            GameObject go = finalResults[0].gameObject;
            if (go != null && go.name != "Blocker")
            {
                ExecuteEvents.Execute(go, pointerEventData, ExecuteEvents.pointerClickHandler);
                // [ID] Debug opsional untuk melihat apa yang diklik
                // Debug.Log("Clicked General UI: " + go.name);
            }
        }
    }
    #endregion

    #region HELPER METHODS
    /// <summary>
    /// [ID] Memeriksa apakah objek adalah bagian dari "Dropdown List" yang digenerate Unity.
    /// [EN] Checks if an object is part of the Unity-generated "Dropdown List".
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