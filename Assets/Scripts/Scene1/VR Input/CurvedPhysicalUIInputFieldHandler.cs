using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class CurvedPhysicalUIInputFieldHandler : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField inputField;
    public BoxCollider inputCollider;

    [Header("VR Settings")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Keyboard Settings")]
    public bool openTouchKeyboard = true;

    // [ID] Coba ganti ke ASCIICapable jika Default tidak muncul di Pico
    public TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.ASCIICapable;

    private TouchScreenKeyboard virtualKeyboard;

    private void Start()
    {
        if (inputField != null)
        {
            // [ID] PENTING: Matikan input mobile bawaan TMP sepenuhnya
            inputField.shouldHideMobileInput = true;
        }
    }

    private void OnEnable()
    {
        if (selectAction != null) selectAction.action.started += OnSelect;
    }

    private void OnDisable()
    {
        if (selectAction != null) selectAction.action.started -= OnSelect;
    }

    private void OnSelect(InputAction.CallbackContext ctx)
    {
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

        // [ID] 1. Visual Focus (Agar kursor berkedip di Render Texture)
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField(); // Visual cursor blink

        yield return new WaitForEndOfFrame(); // Tunggu frame selesai

        // [ID] 2. Buka Keyboard Secara Paksa dengan Parameter Lengkap
        if (openTouchKeyboard)
        {
            // Reset keyboard jika sudah terbuka
            if (virtualKeyboard != null)
            {
                virtualKeyboard.active = false;
                virtualKeyboard = null;
            }

            // [PENTING UNTUK PICO/ANDROID]
            // Parameter: (text, type, autocorrection, multiline, secure, alert, placeholder)
            // Kadang Pico butuh parameter ini didefinisikan eksplisit.
            virtualKeyboard = TouchScreenKeyboard.Open(
                inputField.text,
                keyboardType,
                false, // autocorrection
                inputField.lineType != TMP_InputField.LineType.SingleLine, // multiline support
                inputField.contentType == TMP_InputField.ContentType.Password, // secure/password
                false, // alert
                inputField.placeholder.GetComponent<TMP_Text>().text // placeholder
            );

            Debug.Log($"[Pico Fix] Request Open Keyboard. Supported: {TouchScreenKeyboard.isSupported}");
        }
    }

    private void Update()
    {
        // [ID] Logic sinkronisasi (Sama seperti sebelumnya, ini sudah benar)
        if (virtualKeyboard != null && virtualKeyboard.active)
        {
            if (inputField.text != virtualKeyboard.text)
            {
                inputField.text = virtualKeyboard.text;

                // [ID] Paksa update visual input field agar terlihat di Render Texture
                inputField.ForceLabelUpdate();
            }
        }

        if (virtualKeyboard != null && virtualKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            inputField.DeactivateInputField();
            virtualKeyboard = null;
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