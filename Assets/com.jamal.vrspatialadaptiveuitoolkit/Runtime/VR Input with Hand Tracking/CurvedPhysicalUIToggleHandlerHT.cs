using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR Toggle (Checkbox) menggunakan pendekatan "Physical Proxy" untuk Hand Tracking.
    /// Memetakan Collider 3D fisik ke komponen Toggle UI Canvas untuk DUA TANGAN.
    /// Kompatibel dengan arsitektur XR Interaction Toolkit (XRI) 3.0+.
    /// 
    /// [EN] Handles VR Toggle (Checkbox) interaction using the "Physical Proxy" approach for Hand Tracking.
    /// Maps physical 3D Colliders to Canvas UI Toggle components for BOTH HANDS.
    /// Compatible with XR Interaction Toolkit (XRI) 3.0+ architecture.
    /// </summary>
    public class CurvedPhysicalUIToggleHandlerHT : MonoBehaviour
    {
        [Header("Mapping Configuration")]
        [Tooltip("[ID] Daftar Collider 3D transparan untuk area Toggle.\n[EN] List of transparent 3D Colliders for Toggle areas.")]
        public Collider[] toggleBoxColliders;

        [Tooltip("[ID] Daftar Toggle UI yang sesuai dengan urutan Collider di atas.\n[EN] List of UI Toggles corresponding to the order of Colliders above.")]
        public Toggle[] canvasToggles;

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
        [SerializeField] private bool showDebug = true;

        private void Start()
        {
            // [ID] Validasi panjang array: Pastikan jumlah collider dan toggle sama
            // [EN] Validate array length: Ensure the count of colliders and toggles matches
            if (toggleBoxColliders.Length != canvasToggles.Length)
            {
                Debug.LogError($"[PhysicalToggle] Mismatch! Colliders: {toggleBoxColliders.Length}, Toggles: {canvasToggles.Length}. Please fix in Inspector.");
            }
        }

        private void OnEnable()
        {
            // [ID] Daftarkan dan aktifkan listener untuk cubitan Kiri
            // [EN] Register and enable listener for Left pinch
            if (leftSelectAction != null && leftSelectAction.action != null)
            {
                // [ID] Wajib di XRI 3.0+ agar input selalu aktif membaca Hand Tracking
                // [EN] Mandatory in XRI 3.0+ to ensure input actively reads Hand Tracking
                leftSelectAction.action.Enable();
                leftSelectAction.action.started += OnLeftTriggerPressed;
            }

            // [ID] Daftarkan dan aktifkan listener untuk cubitan Kanan
            // [EN] Register and enable listener for Right pinch
            if (rightSelectAction != null && rightSelectAction.action != null)
            {
                rightSelectAction.action.Enable();
                rightSelectAction.action.started += OnRightTriggerPressed;
            }
        }

        private void OnDisable()
        {
            // [ID] Lepaskan listener VR untuk mencegah memory leak saat script mati
            // [EN] Remove VR listeners to prevent memory leaks when script is disabled
            if (leftSelectAction != null && leftSelectAction.action != null)
            {
                leftSelectAction.action.started -= OnLeftTriggerPressed;
            }

            if (rightSelectAction != null && rightSelectAction.action != null)
            {
                rightSelectAction.action.started -= OnRightTriggerPressed;
            }
        }

        // ==========================================
        // LOGIC UTAMA / MAIN LOGIC
        // ==========================================

        private void OnLeftTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (leftRayInteractor != null)
            {
                if (showDebug) Debug.Log("[PhysicalToggle] Tangan Kiri mencubit! / Left hand pinched!");
                CheckAndToggle(leftRayInteractor);
            }
        }

        private void OnRightTriggerPressed(InputAction.CallbackContext ctx)
        {
            if (rightRayInteractor != null)
            {
                if (showDebug) Debug.Log("[PhysicalToggle] Tangan Kanan mencubit! / Right hand pinched!");
                CheckAndToggle(rightRayInteractor);
            }
        }

        private void CheckAndToggle(XRRayInteractor interactor)
        {
            // [ID] 1. Minta data Raycast Hit dari Interactor saat terjadi cubitan
            // [EN] 1. Request Raycast Hit data from the Interactor when a pinch occurs
            if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // [ID] 2. Cari index collider yang tertabrak di dalam array
                // [EN] 2. Find the index of the hit collider within the array
                int index = Array.IndexOf(toggleBoxColliders, hit.collider);

                // [ID] Jika index ditemukan (bukan -1)
                // [EN] If index is found (not -1)
                if (index != -1)
                {
                    // [ID] 3. Pastikan Toggle UI di index tersebut ada/valid
                    // [EN] 3. Ensure the UI Toggle at that index is present/valid
                    if (index < canvasToggles.Length && canvasToggles[index] != null)
                    {
                        if (showDebug)
                            Debug.Log($"[PhysicalToggle] Kena Collider / Hit Collider [{index}] -> Switching Toggle: '{canvasToggles[index].name}'");

                        // [ID] 4. Ubah status Toggle (True jadi False, False jadi True)
                        // [EN] 4. Flip the Toggle state (True to False, False to True)
                        canvasToggles[index].isOn = !canvasToggles[index].isOn;

                        // [NOTE/ID] Mengubah .isOn secara code akan otomatis memanggil event 'OnValueChanged' pada Toggle
                        // [NOTE/EN] Changing .isOn via code automatically invokes the 'OnValueChanged' event on the Toggle
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

            Gizmos.color = Color.yellow; // [ID] Warna Kuning untuk penanda Toggle / [EN] Yellow color for Toggle indicator
            for (int i = 0; i < toggleBoxColliders.Length; i++)
            {
                if (i < canvasToggles.Length && toggleBoxColliders[i] != null && canvasToggles[i] != null)
                {
                    // [ID] Gambar garis penghubung visual antara Collider fisik dan Tombol UI
                    // [EN] Draw a visual connecting line between physical Collider and UI Button
                    Gizmos.DrawLine(toggleBoxColliders[i].transform.position, canvasToggles[i].transform.position);
                }
            }
        }
    }
}