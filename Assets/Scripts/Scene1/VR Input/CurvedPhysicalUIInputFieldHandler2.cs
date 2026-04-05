using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// [ID] Menangani interaksi VR untuk Input Field menggunakan Keyboard Bawaan OS (Pico/Android).
/// [EN] Handles VR interaction for the Input Field using the Native OS Keyboard (Pico/Android).
/// </summary>
public class CurvedPhysicalUIInputFieldHandler2 : MonoBehaviour
{
    // ==========================================
    // REFERENSI / REFERENCES
    // ==========================================
    [Header("References")]
    [Tooltip("[ID] Komponen TMP_InputField target.\n[EN] The target TMP_InputField component.")]
    public TMP_InputField inputField;

    [Tooltip("[ID] Collider fisik yang mewakili area Input Field.\n[EN] Physical Collider representing the Input Field area.")]
    public BoxCollider inputCollider;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor VR.\n[EN] Reference to the VR Ray Interactor.")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    [Tooltip("[ID] Action trigger dari VR Controller.\n[EN] Trigger action from the VR Controller.")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Keyboard Settings")]
    [Tooltip("[ID] Izinkan membuka keyboard OS virtual.\n[EN] Allow opening the virtual OS keyboard.")]
    public bool openTouchKeyboard = true;

    [Tooltip("[ID] Tipe keyboard, ubah ke ASCIICapable jika Default tidak muncul di Pico.\n[EN] Keyboard type, change to ASCIICapable if Default doesn't show up on Pico.")]
    public TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.ASCIICapable;

    private TouchScreenKeyboard virtualKeyboard;

    // [ID] Mencegah coroutine dipanggil berkali-kali dalam waktu bersamaan
    // [EN] Prevents the coroutine from being called multiple times simultaneously
    private bool isKeyboardOpening = false;

    private void Start()
    {
        if (inputField != null)
        {
            // [ID] Biarkan true agar TMP tidak bentrok dengan pemanggilan TouchScreenKeyboard manual kita
            // [EN] Keep it true so TMP doesn't conflict with our manual TouchScreenKeyboard call
            inputField.shouldHideMobileInput = true;
        }
    }

    private void OnEnable()
    {
        // [ID] Hanya deteksi saat tombol mulai ditekan (bukan saat dilepas/canceled)
        // [EN] Only detect when the button starts being pressed (not when released/canceled)
        if (selectAction != null) selectAction.action.started += OnSelect;
    }

    private void OnDisable()
    {
        if (selectAction != null) selectAction.action.started -= OnSelect;
    }

    // ==========================================
    // LOGIC UTAMA / MAIN LOGIC
    // ==========================================
    private void OnSelect(InputAction.CallbackContext ctx)
    {
        // [ID] Jangan lakukan apapun jika keyboard sedang proses buka atau sudah terbuka
        // [EN] Do nothing if the keyboard is currently opening or is already open
        if (isKeyboardOpening) return;
        if (virtualKeyboard != null && (virtualKeyboard.active || virtualKeyboard.status == TouchScreenKeyboard.Status.Visible)) return;

        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (hit.collider == inputCollider)
            {
                StartCoroutine(ForceOpenKeyboard());
            }
        }
    }

    private IEnumerator ForceOpenKeyboard()
    {
        if (inputField == null) yield break;

        isKeyboardOpening = true;

        // [ID] 1. Amankan fokus UI
        // [EN] 1. Secure UI focus
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();

        // [ID] Tunggu sedikit agar EventSystem Unity selesai memproses seleksi UI
        // [EN] Wait slightly so Unity's EventSystem finishes processing UI selection
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // [ID] 2. Buka Keyboard Bawaan OS
        // [EN] 2. Open Native OS Keyboard
        if (openTouchKeyboard)
        {
            // [ID] Tidak perlu mematikan keyboard lama, cukup buat instance baru dengan parameter aman
            // [EN] No need to disable the old keyboard, just create a new instance with safe parameters
            string currentText = inputField.text != null ? inputField.text : "";
            string placeholderText = inputField.placeholder != null ? inputField.placeholder.GetComponent<TMP_Text>().text : "";

            virtualKeyboard = TouchScreenKeyboard.Open(
                currentText,
                keyboardType,
                false, // autocorrection
                inputField.lineType != TMP_InputField.LineType.SingleLine, // multiline
                inputField.contentType == TMP_InputField.ContentType.Password, // secure
                false, // alert
                placeholderText
            );

            Debug.Log("[VR Keyboard] Membuka native keyboard berhasil. / Native keyboard opened successfully.");
        }

        // [ID] Beri jeda sejenak sebelum mengizinkan klik lagi
        // [EN] Give a brief pause before allowing clicks again
        yield return new WaitForSeconds(0.5f);
        isKeyboardOpening = false;
    }

    private void Update()
    {
        // [ID] Sinkronisasi ketikan dari OS Keyboard ke Text Unity
        // [EN] Synchronize typing from OS Keyboard to Unity Text
        if (virtualKeyboard != null && virtualKeyboard.active)
        {
            if (inputField.text != virtualKeyboard.text)
            {
                inputField.text = virtualKeyboard.text;
                inputField.ForceLabelUpdate();
            }
        }

        // [ID] Menutup dan membersihkan referensi jika user menekan Enter/Selesai di OS Keyboard
        // [EN] Close and clear reference if the user presses Enter/Done on the OS Keyboard
        if (virtualKeyboard != null)
        {
            if (virtualKeyboard.status == TouchScreenKeyboard.Status.Done ||
                virtualKeyboard.status == TouchScreenKeyboard.Status.Canceled ||
                virtualKeyboard.status == TouchScreenKeyboard.Status.LostFocus)
            {
                inputField.DeactivateInputField();
                virtualKeyboard = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (inputCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(inputCollider.transform.position, inputCollider.size);
        }
    }
}