using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR Toggle (Checkbox) menggunakan pendekatan "Physical Proxy".
/// Memetakan Collider 3D fisik ke komponen Toggle UI Canvas.
/// 
/// [EN] Handles VR Toggle (Checkbox) interaction using the "Physical Proxy" approach.
/// Maps physical 3D Colliders to Canvas UI Toggle components.
/// </summary>
public class CurvedPhysicalUIToggleHandler : MonoBehaviour
{
    [Header("Mapping Configuration")]
    [Tooltip("[ID] Daftar Collider 3D untuk area Toggle.\n[EN] List of 3D Colliders for Toggle areas.")]
    public Collider[] toggleBoxColliders;

    [Tooltip("[ID] Daftar Toggle UI yang sesuai dengan urutan Collider di atas.\n[EN] List of UI Toggles corresponding to the order of Colliders above.")]
    public Toggle[] canvasToggles;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor.\n[EN] Reference to the Ray Interactor.")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [Tooltip("[ID] Input Action untuk trigger/klik.\n[EN] Input Action for trigger/click.")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private void Start()
    {
        // [ID] Validasi panjang array
        // [EN] Validate array length
        if (toggleBoxColliders.Length != canvasToggles.Length)
        {
            Debug.LogError($"[PhysicalToggle] Mismatch! Colliders: {toggleBoxColliders.Length}, Toggles: {canvasToggles.Length}.");
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

    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null)
        {
            CheckAndToggle(rayInteractor);
        }
    }

    private void CheckAndToggle(XRRayInteractor interactor)
    {
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] Cari index collider yang tertabrak
            // [EN] Find the index of the hit collider
            int index = Array.IndexOf(toggleBoxColliders, hit.collider);

            if (index != -1)
            {
                if (index < canvasToggles.Length && canvasToggles[index] != null)
                {
                    if (showDebug)
                        Debug.Log($"[PhysicalToggle] Hit Collider [{index}] -> Switching Toggle: '{canvasToggles[index].name}'");

                    // [ID] Ubah status Toggle (True jadi False, False jadi True)
                    // [EN] Flip the Toggle state (True to False, False to True)
                    canvasToggles[index].isOn = !canvasToggles[index].isOn;

                    // [NOTE] Mengubah .isOn secara code akan otomatis memanggil event 'OnValueChanged' pada Toggle
                }
                else
                {
                    Debug.LogWarning($"[PhysicalToggle] Index {index} found, but Toggle is null!");
                }
            }
        }
    }

    // ==========================================
    // VISUALISASI / VISUALIZATION
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (toggleBoxColliders == null || canvasToggles == null) return;

        Gizmos.color = Color.yellow; // Warna Kuning untuk Toggle
        for (int i = 0; i < toggleBoxColliders.Length; i++)
        {
            if (i < canvasToggles.Length && toggleBoxColliders[i] != null && canvasToggles[i] != null)
            {
                Gizmos.DrawLine(toggleBoxColliders[i].transform.position, canvasToggles[i].transform.position);
            }
        }
    }
}