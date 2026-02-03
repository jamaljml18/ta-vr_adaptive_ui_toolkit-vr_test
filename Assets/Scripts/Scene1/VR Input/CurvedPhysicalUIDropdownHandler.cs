using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

/// <summary>
/// [ID] Menangani interaksi VR Dropdown menggunakan dua collider (Header dan List).
/// Menghitung indeks item berdasarkan posisi Y lokal pada List Collider.
/// 
/// [EN] Handles VR Dropdown interaction using two colliders (Header and List).
/// Calculates item index based on the local Y position on the List Collider.
/// </summary>
public class CurvedPhysicalUIDropdownHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("[ID] Referensi ke komponen TMP Dropdown.\n[EN] Reference to the TMP Dropdown component.")]
    public TMP_Dropdown dropdown;

    [Tooltip("[ID] Collider pada tombol utama Dropdown.\n[EN] Collider on the main Dropdown button.")]
    public BoxCollider headerCollider;

    [Tooltip("[ID] Collider besar yang menutupi area daftar (Template).\n[EN] Large collider covering the list area (Template).")]
    public BoxCollider listCollider;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor.\n[EN] Reference to the Ray Interactor.")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [Tooltip("[ID] Input Action untuk klik/pilih.\n[EN] Input Action for click/select.")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    private void OnEnable()
    {
        if (selectAction != null)
        {
            selectAction.action.started += OnSelect;
        }

        // [ID] Sembunyikan collider list saat awal agar tidak menghalangi raycast lain.
        // [EN] Hide the list collider at start so it doesn't block other raycasts.
        if (listCollider != null)
            listCollider.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (selectAction != null)
        {
            selectAction.action.started -= OnSelect;
        }
    }

    // ============================================================
    // INPUT EVENTS
    // ============================================================
    private void OnSelect(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] 1. Deteksi klik pada Header (Buka/Tutup Dropdown).
            // [EN] 1. Detect click on Header (Open/Close Dropdown).
            if (hit.collider == headerCollider)
            {
                if (!listCollider.gameObject.activeSelf)
                    OpenDropdown();
                else
                    CloseDropdown();
            }
            // [ID] 2. Deteksi klik pada area List (Pilih Item).
            // [EN] 2. Detect click on List area (Select Item).
            else if (hit.collider == listCollider)
            {
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

            if (showDebug) Debug.Log("[Dropdown] List Opened");
        }
    }

    private void CloseDropdown()
    {
        if (dropdown != null)
        {
            dropdown.Hide();
            if (listCollider != null) listCollider.gameObject.SetActive(false);

            if (showDebug) Debug.Log("[Dropdown] List Closed");
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

        // [ID] 4. Hitung indeks berdasarkan jumlah opsi.
        // [EN] 4. Calculate index based on the number of options.
        int itemCount = dropdown.options.Count;
        int selectedIndex = Mathf.FloorToInt(invertedY * itemCount);

        // [ID] Pastikan indeks tidak keluar batas (misal: tepat di garis bawah).
        // [EN] Ensure index is within bounds (e.g., exactly on the bottom line).
        selectedIndex = Mathf.Clamp(selectedIndex, 0, itemCount - 1);

        // [ID] 5. Terapkan nilai dan tutup dropdown.
        // [EN] 5. Apply value and close dropdown.
        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue(); // Memastikan teks di header berubah
        CloseDropdown();

        if (showDebug)
            Debug.Log($"[Dropdown] Selected Index: {selectedIndex} ({dropdown.options[selectedIndex].text})");
    }

    // ==========================================
    // GIZMOS
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (headerCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(headerCollider.transform.position, headerCollider.size);
        }

        if (listCollider != null && listCollider.gameObject.activeSelf)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(listCollider.transform.position, listCollider.size);
        }
    }
}