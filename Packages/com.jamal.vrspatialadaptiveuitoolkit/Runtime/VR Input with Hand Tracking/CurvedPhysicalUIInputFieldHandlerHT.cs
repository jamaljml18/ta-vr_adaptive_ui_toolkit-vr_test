using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR untuk Input Field menggunakan Keyboard 3D Kustom untuk Hand Tracking.
    /// Mendukung DUA TANGAN melalui gestur cubitan. Kompatibel dengan arsitektur XR Interaction Toolkit (XRI) 3.0+.
    /// 
    /// [EN] Handles VR interaction for the Input Field using a Custom 3D Keyboard for Hand Tracking.
    /// Supports BOTH HANDS via pinch gestures. Compatible with XR Interaction Toolkit (XRI) 3.0+ architecture.
    /// </summary>
    public class CurvedPhysicalUIInputFieldHandlerHT : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("[ID] Komponen TMP_InputField target.\n[EN] The target TMP_InputField component.")]
        public TMP_InputField inputField;

        [Tooltip("[ID] Collider fisik yang mewakili area Input Field.\n[EN] Physical Collider representing the Input Field area.")]
        public BoxCollider inputCollider;

        [Header("VR Settings - Interactors")]
        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
        public XRRayInteractor leftRayInteractor;

        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
        public XRRayInteractor rightRayInteractor;

        [Header("VR Settings - Input Actions (XRI 3.0+)")]
        [Tooltip("[ID] Input Action untuk cubitan kiri (Contoh: XRI LeftHand Interaction/Select).\n[EN] Input Action for Left pinch (Example: XRI LeftHand Interaction/Select).")]
        public InputActionReference leftSelectAction;

        [Tooltip("[ID] Input Action untuk cubitan kanan (Contoh: XRI RightHand Interaction/Select).\n[EN] Input Action for Right pinch (Example: XRI RightHand Interaction/Select).")]
        public InputActionReference rightSelectAction;

        [Header("Custom Keyboard Link")]
        [Tooltip("[ID] Masukkan GameObject yang memiliki script KeyboardInputFieldManager.\n[EN] Insert the GameObject containing the KeyboardInputFieldManager script.")]
        public KeyboardInputFieldManagerHT customKeyboardManager;

        private void Start()
        {
            if (inputField != null)
            {
                // [ID] Matikan input mobile bawaan agar tidak konflik
                // [EN] Disable native mobile input to prevent conflicts
                inputField.shouldHideMobileInput = true;

                // [ID] [KUNCI PERBAIKAN BUG] Jadikan Read Only agar keyboard OS (seperti Meta Quest/Pico UI) tidak terpancing keluar.
                // [EN] [BUG FIX KEY] Make it Read Only so the OS keyboard (like Meta Quest/Pico UI) is not triggered.
                inputField.readOnly = true;
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
                leftSelectAction.action.started += OnLeftSelect;
            }

            // [ID] Daftarkan dan aktifkan listener untuk cubitan Kanan
            // [EN] Register and enable listener for Right pinch
            if (rightSelectAction != null && rightSelectAction.action != null)
            {
                rightSelectAction.action.Enable();
                rightSelectAction.action.started += OnRightSelect;
            }
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

        // ==========================================
        // LOGIC UTAMA / MAIN LOGIC
        // ==========================================

        private void OnLeftSelect(InputAction.CallbackContext ctx) => ProcessSelection(leftRayInteractor);
        private void OnRightSelect(InputAction.CallbackContext ctx) => ProcessSelection(rightRayInteractor);

        private void ProcessSelection(XRRayInteractor interactor)
        {
            // [ID] Mengecek apakah ada objek yang tersorot laser saat gestur cubitan terjadi
            // [EN] Checks if an object is hit by the laser when the pinch gesture occurs
            if (interactor != null && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                if (hit.collider == inputCollider)
                {
                    // [ID] Fokuskan UI secara visual (kursor tetap bisa berkedip meski Read Only)
                    // [EN] Visually focus the UI (cursor can still blink even if Read Only)
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                    inputField.ActivateInputField();

                    // [ID] Panggil custom 3D keyboard kita
                    // [EN] Call our custom 3D keyboard
                    if (customKeyboardManager != null)
                    {
                        Debug.Log($"[InputFieldHT] Area InputField dicubit melalui {interactor.name}. Membuka Keyboard.");
                        customKeyboardManager.OpenKeyboard(inputField);
                    }
                    else
                    {
                        Debug.LogWarning("[InputFieldHT] Custom Keyboard Manager belum di-assign di Inspector!");
                    }
                }
            }
        }

        // ==========================================
        // VISUALISASI / VISUALIZATION
        // ==========================================
        private void OnDrawGizmosSelected()
        {
            if (inputCollider != null)
            {
                Gizmos.color = Color.magenta; // [ID] Warna Magenta untuk area Input Field / [EN] Magenta color for Input Field area
                Gizmos.DrawWireCube(inputCollider.transform.position, inputCollider.size);
            }
        }
    }
}