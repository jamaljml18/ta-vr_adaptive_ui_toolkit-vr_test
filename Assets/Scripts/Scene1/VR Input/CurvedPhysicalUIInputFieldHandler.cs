using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR InputField menggunakan Box Collider.
/// Memicu fokus input dan membuka keyboard sistem (TouchScreenKeyboard) saat diklik.
/// 
/// [EN] Handles VR InputField interaction using a Box Collider.
/// Triggers input focus and opens the system keyboard (TouchScreenKeyboard) when clicked.
/// </summary>
public class CurvedPhysicalUIInputFieldHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("[ID] Referensi ke komponen TMP InputField.\n[EN] Reference to the TMP InputField component.")]
    public TMP_InputField inputField;

    [Tooltip("[ID] Collider yang menutupi area Input Field.\n[EN] Collider covering the Input Field area.")]
    public BoxCollider inputCollider;

    [Header("VR Settings")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Keyboard Settings")]
    [Tooltip("[ID] Apakah ingin membuka keyboard virtual secara otomatis (Android/Oculus).\n[EN] Whether to open the virtual keyboard automatically (Android/Oculus).")]
    public bool openTouchKeyboard = true;

    private TouchScreenKeyboard virtualKeyboard;

    private void OnEnable()
    {
        if (selectAction != null)
            selectAction.action.started += OnSelect;
    }

    private void OnDisable()
    {
        if (selectAction != null)
            selectAction.action.started -= OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] Cek apakah collider yang diklik adalah milik InputField ini.
            // [EN] Check if the clicked collider belongs to this InputField.
            if (hit.collider == inputCollider)
            {
                ActivateInput();
            }
        }
    }

    private void ActivateInput()
    {
        if (inputField == null) return;

        // [ID] 1. Berikan fokus secara programatik ke InputField.
        // [EN] 1. Programmatically give focus to the InputField.
        inputField.ActivateInputField();
        inputField.Select();

        // [ID] 2. Tangani pemunculan keyboard.
        // [EN] 2. Handle keyboard appearance.
        if (openTouchKeyboard)
        {
            // [ID] TouchScreenKeyboard.Open berfungsi untuk memicu keyboard bawaan (Meta Quest/Pico/Android).
            // [EN] TouchScreenKeyboard.Open triggers the native keyboard (Meta Quest/Pico/Android).
            virtualKeyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
        }

        Debug.Log($"[InputField] {inputField.name} Activated");
    }

    private void Update()
    {
        // [ID] Update teks InputField secara real-time dari keyboard virtual jika sedang aktif.
        // [EN] Update InputField text in real-time from the virtual keyboard if active.
        if (virtualKeyboard != null && virtualKeyboard.active)
        {
            inputField.text = virtualKeyboard.text;
        }
    }

    // ==========================================
    // GIZMOS
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (inputCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(inputCollider.transform.position, inputCollider.size);
        }
    }
}