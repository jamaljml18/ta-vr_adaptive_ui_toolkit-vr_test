using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// [ID] Menangani interaksi VR untuk Input Field menggunakan Keyboard 3D Kustom.
/// Mendukung DUA TANGAN.
/// 
/// [EN] Handles VR interaction for the Input Field using a Custom 3D Keyboard.
/// Supports BOTH HANDS.
/// </summary>
public class CurvedPhysicalUIInputFieldHandler : MonoBehaviour
{
    // ==========================================
    // REFERENSI / REFERENCES
    // ==========================================
    [Header("References")]
    [Tooltip("[ID] Komponen TMP_InputField target.\n[EN] The target TMP_InputField component.")]
    public TMP_InputField inputField;

    [Tooltip("[ID] Collider fisik yang mewakili area Input Field.\n[EN] Physical Collider representing the Input Field area.")]
    public BoxCollider inputCollider;

    [Header("VR Settings - Interactors")]
    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor leftRayInteractor;

    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rightRayInteractor;

    [Header("VR Settings - Input Actions")]
    [Tooltip("[ID] Action trigger dari VR Controller Kiri.\n[EN] Trigger action from the Left VR Controller.")]
    public InputActionReference leftSelectAction;

    [Tooltip("[ID] Action trigger dari VR Controller Kanan.\n[EN] Trigger action from the Right VR Controller.")]
    public InputActionReference rightSelectAction;

    [Header("Custom Keyboard Link")]
    [Tooltip("[ID] Masukkan GameObject yang memiliki script KeyboardInputFieldManager.\n[EN] Insert the GameObject containing the KeyboardInputFieldManager script.")]
    public KeyboardInputFieldManager customKeyboardManager;

    private void Start()
    {
        if (inputField != null)
        {
            // [ID] Matikan input mobile bawaan agar tidak konflik
            // [EN] Disable native mobile input to prevent conflicts
            inputField.shouldHideMobileInput = true;

            // [ID] [KUNCI PERBAIKAN BUG] Jadikan Read Only agar keyboard OS (Pico/Android) tidak terpancing keluar.
            // [EN] [BUG FIX KEY] Make it Read Only so the OS keyboard (Pico/Android) is not triggered.
            inputField.readOnly = true;
        }
    }

    private void OnEnable()
    {
        // [ID] Daftarkan listener untuk kedua tangan
        // [EN] Register listeners for both hands
        if (leftSelectAction != null) leftSelectAction.action.started += OnLeftSelect;
        if (rightSelectAction != null) rightSelectAction.action.started += OnRightSelect;
    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener untuk kedua tangan
        // [EN] Remove listeners for both hands
        if (leftSelectAction != null) leftSelectAction.action.started -= OnLeftSelect;
        if (rightSelectAction != null) rightSelectAction.action.started -= OnRightSelect;
    }

    // ==========================================
    // LOGIC UTAMA / MAIN LOGIC
    // ==========================================

    private void OnLeftSelect(InputAction.CallbackContext ctx) => ProcessSelection(leftRayInteractor);
    private void OnRightSelect(InputAction.CallbackContext ctx) => ProcessSelection(rightRayInteractor);

    private void ProcessSelection(UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor)
    {
        if (interactor != null && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider == inputCollider)
            {
                // [ID] Fokuskan UI secara visual (kursor tetap bisa berkedip meski Read Only)
                // [EN] Visually focus the UI (cursor can still blink even if Read Only)
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                inputField.ActivateInputField();

                // [ID] Panggil keyboard manual kita
                // [EN] Call our manual keyboard
                if (customKeyboardManager != null)
                {
                    customKeyboardManager.OpenKeyboard(inputField);
                }
                else
                {
                    Debug.LogWarning("[PhysicalUI] Custom Keyboard Manager belum di-assign di Inspector!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (inputCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(inputCollider.transform.position, inputCollider.size);
        }
    }
}