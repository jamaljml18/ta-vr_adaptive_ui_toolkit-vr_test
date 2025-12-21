using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR tombol menggunakan pendekatan "Physical Proxy".
/// Memetakan Collider 3D fisik ke Tombol UI Canvas secara manual.
/// 
/// [EN] Handles VR button interaction using the "Physical Proxy" approach.
/// Maps physical 3D Colliders to Canvas UI Buttons manually.
/// </summary>
public class CurvedPhysicalUIButtonHandler : MonoBehaviour
{
    [Header("Mapping Configuration")]
    [Tooltip("[ID] Daftar Collider 3D transparan yang dipasang di atas mesh UI.\n[EN] List of transparent 3D Colliders placed over the UI mesh.")]
    public Collider[] buttonBoxColliders;

    [Tooltip("[ID] Daftar Button UI yang sesuai dengan urutan Collider di atas.\n[EN] List of UI Buttons corresponding to the order of Colliders above.")]
    public Button[] canvasButtons;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor (biasanya di tangan kiri/kanan).\n[EN] Reference to the Ray Interactor (usually on left/right hand).")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [Tooltip("[ID] Input Action untuk trigger/klik (contoh: XRI LeftHand Interaction/Select).\n[EN] Input Action for trigger/click (e.g., XRI LeftHand Interaction/Select).")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private void Start()
    {
        // [ID] Validasi: Pastikan jumlah collider dan tombol sama
        // [EN] Validation: Ensure the count of colliders and buttons matches
        if (buttonBoxColliders.Length != canvasButtons.Length)
        {
            Debug.LogError($"[PhysicalUI] Mismatch! Colliders: {buttonBoxColliders.Length}, Buttons: {canvasButtons.Length}. Please fix in Inspector.");
        }
    }

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        if (selectAction != null)
            selectAction.action.started += OnTriggerPressed;
    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listener to avoid memory leaks
        if (selectAction != null)
            selectAction.action.started -= OnTriggerPressed;
    }

    // ==========================================
    // LOGIC UTAMA / MAIN LOGIC
    // ==========================================
    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null)
        {
            CheckAndClick(rayInteractor);
        }
    }

    private void CheckAndClick(XRRayInteractor interactor)
    {
        // [ID] 1. Minta data Raycast Hit dari Interactor
        // [EN] 1. Request Raycast Hit data from the Interactor
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] 2. Cari index collider yang tertabrak di dalam array
            // [EN] 2. Find the index of the hit collider within the array
            int index = Array.IndexOf(buttonBoxColliders, hit.collider);

            // [ID] Jika index ditemukan (bukan -1)
            // [EN] If index is found (not -1)
            if (index != -1)
            {
                // [ID] 3. Pastikan tombol UI di index tersebut ada/valid
                // [EN] 3. Ensure the UI button at that index is present/valid
                if (index < canvasButtons.Length && canvasButtons[index] != null)
                {
                    if (showDebug)
                        Debug.Log($"[PhysicalUI] Hit Collider [{index}]: '{hit.collider.name}' -> Clicking Button: '{canvasButtons[index].name}'");

                    // [ID] 4. Eksekusi fungsi OnClick pada tombol
                    // [EN] 4. Execute the OnClick function on the button
                    canvasButtons[index].onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning($"[PhysicalUI] Collider found at index {index}, but Button is missing!");
                }
            }
        }
    }

    // ==========================================
    // VISUALISASI / VISUALIZATION
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (buttonBoxColliders == null || canvasButtons == null) return;

        Gizmos.color = Color.cyan; // Warna Cyan untuk Button
        for (int i = 0; i < buttonBoxColliders.Length; i++)
        {
            if (i < canvasButtons.Length && buttonBoxColliders[i] != null && canvasButtons[i] != null)
            {
                // [ID] Gambar garis penghubung visual antara Collider fisik dan Tombol UI
                // [EN] Draw a visual connecting line between physical Collider and UI Button
                Gizmos.DrawLine(buttonBoxColliders[i].transform.position, canvasButtons[i].transform.position);
            }
        }
    }
}