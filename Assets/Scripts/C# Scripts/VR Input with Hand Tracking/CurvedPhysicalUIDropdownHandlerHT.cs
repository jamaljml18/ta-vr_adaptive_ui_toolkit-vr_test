using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

/// <summary>
/// [ID] Menangani interaksi VR Dropdown menggunakan pendekatan "Physical Proxy" untuk Hand Tracking.
/// Mendukung DUA TANGAN. Menghitung indeks item berdasarkan posisi Y lokal pada List Collider saat dicubit.
/// Kompatibel dengan arsitektur XR Interaction Toolkit (XRI) 3.0+.
/// 
/// [EN] Handles VR Dropdown interaction using the "Physical Proxy" approach for Hand Tracking.
/// Supports BOTH HANDS. Calculates item index based on the local Y position on the List Collider upon pinch.
/// Compatible with XR Interaction Toolkit (XRI) 3.0+ architecture.
/// </summary>
public class CurvedPhysicalUIDropdownHandlerHT : MonoBehaviour
{
    [Header("References")]
    [Tooltip("[ID] Referensi ke komponen TMP Dropdown.\n[EN] Reference to the TMP Dropdown component.")]
    public TMP_Dropdown dropdown;

    [Tooltip("[ID] Collider pada tombol utama Dropdown.\n[EN] Collider on the main Dropdown button.")]
    public BoxCollider headerCollider;

    [Tooltip("[ID] Collider besar yang menutupi area daftar (Template).\n[EN] Large collider covering the list area (Template).")]
    public BoxCollider listCollider;

    [Header("VR Settings - Interactors")]
    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
    [SerializeField] private XRRayInteractor leftRayInteractor;

    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
    [SerializeField] private XRRayInteractor rightRayInteractor;

    [Header("VR Settings - Input Actions (XRI 3.0+)")]
    [Tooltip("[ID] Input Action untuk cubitan kiri (Contoh: XRI LeftHand Interaction/Select).\n[EN] Input Action for Left pinch (Example: XRI LeftHand Interaction/Select).")]
    [SerializeField] private InputActionReference leftSelectAction;

    [Tooltip("[ID] Input Action untuk cubitan kanan (Contoh: XRI RightHand Interaction/Select).\n[EN] Input Action for Right pinch (Example: XRI RightHand Interaction/Select).")]
    [SerializeField] private InputActionReference rightSelectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    private void OnEnable()
    {
        // [ID] Daftarkan dan aktifkan listener untuk cubitan Kiri
        // [EN] Register and enable listener for Left pinch
        if (leftSelectAction != null && leftSelectAction.action != null)
        {
            // [ID] Wajib di XRI 3.0+ agar input selalu aktif membaca Hand Tracking
            // [EN] Mandatory in XRI 3.0+ to ensure input actively reads Hand Tracking
            leftSelectAction.action.Enable();
            leftSelectAction.action.started += OnLeftSelect;
        }

        // [ID] Daftarkan dan aktifkan listener untuk cubitan Kanan
        // [EN] Register and enable listener for Right pinch
        if (rightSelectAction != null && rightSelectAction.action != null)
        {
            rightSelectAction.action.Enable();
            rightSelectAction.action.started += OnRightSelect;
        }

        // [ID] Sembunyikan collider list saat awal agar tidak menghalangi raycast lain.
        // [EN] Hide the list collider at start so it doesn't block other raycasts.
        if (listCollider != null)
            listCollider.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listeners to prevent memory leaks
        if (leftSelectAction != null && leftSelectAction.action != null)
            leftSelectAction.action.started -= OnLeftSelect;

        if (rightSelectAction != null && rightSelectAction.action != null)
            rightSelectAction.action.started -= OnRightSelect;
    }

    // ============================================================
    // INPUT EVENTS
    // ============================================================
    private void OnLeftSelect(InputAction.CallbackContext ctx) => ProcessSelect(leftRayInteractor);
    private void OnRightSelect(InputAction.CallbackContext ctx) => ProcessSelect(rightRayInteractor);

    private void ProcessSelect(XRRayInteractor interactor)
    {
        if (interactor != null && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] 1. Deteksi cubitan pada Header (Buka/Tutup Dropdown).
            // [EN] 1. Detect pinch on Header (Open/Close Dropdown).
            if (hit.collider == headerCollider)
            {
                if (showDebug) Debug.Log($"[DropdownHT] Header pinched via {interactor.name}");

                if (!listCollider.gameObject.activeSelf)
                    OpenDropdown();
                else
                    CloseDropdown();
            }
            // [ID] 2. Deteksi cubitan pada area List (Pilih Item).
            // [EN] 2. Detect pinch on List area (Select Item).
            else if (hit.collider == listCollider)
            {
                if (showDebug) Debug.Log($"[DropdownHT] List item pinched via {interactor.name}");
                ProcessListClick(hit.point);
            }
        }
    }

    // ============================================================
    // DROPDOWN ACTIONS
    // ============================================================
    private void OpenDropdown()
    {
        if (dropdown != null)
        {
            dropdown.Show();
            if (listCollider != null) listCollider.gameObject.SetActive(true);

            if (showDebug) Debug.Log("[DropdownHT] List Opened");
        }
    }

    private void CloseDropdown()
    {
        if (dropdown != null)
        {
            dropdown.Hide();
            if (listCollider != null) listCollider.gameObject.SetActive(false);

            if (showDebug) Debug.Log("[DropdownHT] List Closed");
        }
    }

    private void ProcessListClick(Vector3 worldPoint)
    {
        if (dropdown == null || listCollider == null) return;

        // [ID] 1. Konversi World ke Local (Mendapatkan posisi relatif terhadap collider).
        // [EN] 1. Convert World to Local (Get position relative to the collider).
        Vector3 localPoint = listCollider.transform.InverseTransformPoint(worldPoint);
        float height = listCollider.size.y;

        // [ID] 2. Normalisasi posisi Y (0.0 di bawah, 1.0 di atas).
        // [EN] 2. Normalize Y position (0.0 at bottom, 1.0 at top).
        float normalizedY = Mathf.Clamp01((localPoint.y + (height / 2f)) / height);

        // [ID] 3. Invert nilai karena index 0 dropdown Unity dimulai dari paling ATAS.
        // [EN] 3. Invert value because Unity dropdown index 0 starts at the TOP.
        float invertedY = 1f - normalizedY;

        // [ID] 4. Hitung indeks berdasarkan jumlah opsi yang tersedia di Dropdown.
        // [EN] 4. Calculate index based on the number of options available in the Dropdown.
        int itemCount = dropdown.options.Count;
        int selectedIndex = Mathf.FloorToInt(invertedY * itemCount);

        // [ID] Pastikan indeks tidak keluar batas (misal: jika pengguna mencubit tepat di garis bawah).
        // [EN] Ensure index is within bounds (e.g., if the user pinches exactly on the bottom line).
        selectedIndex = Mathf.Clamp(selectedIndex, 0, itemCount - 1);

        // [ID] 5. Terapkan nilai dan tutup dropdown secara terprogram.
        // [EN] 5. Apply value and close dropdown programmatically.
        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue(); // Memastikan teks di header berubah sesuai pilihan
        CloseDropdown();

        if (showDebug)
            Debug.Log($"[DropdownHT] Selected Index: {selectedIndex} ({dropdown.options[selectedIndex].text})");
    }

    // ==========================================
    // GIZMOS
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (headerCollider != null)
        {
            Gizmos.color = Color.yellow; // [ID] Penanda untuk Header / [EN] Indicator for Header
            Gizmos.DrawWireCube(headerCollider.transform.position, headerCollider.size);
        }

        if (listCollider != null && listCollider.gameObject.activeSelf)
        {
            Gizmos.color = Color.cyan; // [ID] Penanda untuk List Area / [EN] Indicator for List Area
            Gizmos.DrawWireCube(listCollider.transform.position, listCollider.size);
        }
    }
}