using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR Toggle (Checkbox) menggunakan pendekatan "Physical Proxy".
    /// Memetakan Collider 3D fisik ke komponen Toggle UI Canvas untuk DUA TANGAN.
    /// 
    /// [EN] Handles VR Toggle (Checkbox) interaction using the "Physical Proxy" approach.
    /// Maps physical 3D Colliders to Canvas UI Toggle components for BOTH HANDS.
    /// </summary>
    public class CurvedPhysicalUIToggleHandler : MonoBehaviour
    {
        [Header("Mapping Configuration")]
        [Tooltip("[ID] Daftar Collider 3D untuk area Toggle.\n[EN] List of 3D Colliders for Toggle areas.")]
        public Collider[] toggleBoxColliders;

        [Tooltip("[ID] Daftar Toggle UI yang sesuai dengan urutan Collider di atas.\n[EN] List of UI Toggles corresponding to the order of Colliders above.")]
        public Toggle[] canvasToggles;

        [Header("VR Settings - Interactors")]
        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
        [SerializeField] private XRRayInteractor leftRayInteractor;

        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
        [SerializeField] private XRRayInteractor rightRayInteractor;

        [Header("VR Settings - Input Actions")]
        [Tooltip("[ID] Input Action untuk trigger/klik Kiri (XRI LeftHand Interaction/Select).\n[EN] Input Action for Left trigger/click.")]
        [SerializeField] private InputActionReference leftSelectAction;

        [Tooltip("[ID] Input Action untuk trigger/klik Kanan (XRI RightHand Interaction/Select).\n[EN] Input Action for Right trigger/click.")]
        [SerializeField] private InputActionReference rightSelectAction;

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
            // [ID] Aktifkan listener input VR untuk kedua tangan ketika object aktif
            // [EN] Enable VR input listeners for both hands when this object becomes active
            if (leftSelectAction != null)
                leftSelectAction.action.started += OnLeftTriggerPressed;

            if (rightSelectAction != null)
                rightSelectAction.action.started += OnRightTriggerPressed;
        }

        private void OnDisable()
        {
            // [ID] Lepaskan listener VR untuk mencegah memory leak
            // [EN] Remove VR listeners to avoid memory leaks
            if (leftSelectAction != null)
                leftSelectAction.action.started -= OnLeftTriggerPressed;

            if (rightSelectAction != null)
                rightSelectAction.action.started -= OnRightTriggerPressed;
        }

        // ==========================================
        // LOGIC UTAMA / MAIN LOGIC
        // ==========================================

        private void OnLeftTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (leftRayInteractor != null)
            {
                if (showDebug) Debug.Log("[PhysicalToggle] Left hand trigger pressed.");
                CheckAndToggle(leftRayInteractor);
            }
        }

        private void OnRightTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (rightRayInteractor != null)
            {
                if (showDebug) Debug.Log("[PhysicalToggle] Right hand trigger pressed.");
                CheckAndToggle(rightRayInteractor);
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
                            Debug.Log($"[PhysicalToggle] Hit Collider [{index}] via {interactor.name} -> Switching Toggle: '{canvasToggles[index].name}'");

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
}