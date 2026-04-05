using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// [ID] Manajer utama untuk Keyboard 3D kustom di VR. Menangani logika ketikan dari Collider tombol 3D.
/// [EN] Main manager for the custom 3D Keyboard in VR. Handles typing logic from 3D button Colliders.
/// </summary>
public class KeyboardInputFieldManager : MonoBehaviour
{
    // ==========================================
    // DEFINISI STRUKTUR / STRUCT DEFINITIONS
    // ==========================================
    public enum KeyFunction
    {
        Character, Space, Backspace, Enter, Tab,
        CapsLock, Shift, Esc, ArrowLeft, ArrowRight,
        ArrowUp, ArrowDown, Ctrl, Alt
    }

    [System.Serializable]
    public struct KeyDefinition
    {
        [Tooltip("[ID] Collider 3D fisik untuk tombol ini.\n[EN] Physical 3D Collider for this key.")]
        public Collider keyCollider;

        [Tooltip("[ID] Fungsi dari tombol ini (huruf, spasi, hapus, dll).\n[EN] Function of this key (character, space, backspace, etc).")]
        public KeyFunction function;

        [Tooltip("[ID] Isi hurufnya (misal: a, 1, @). Kosongkan jika ini tombol fungsi.\n[EN] The character value (e.g., a, 1, @). Leave blank if this is a function key.")]
        public string characterValue;
    }

    // ==========================================
    // REFERENSI & VARIABEL / REFERENCES & VARIABLES
    // ==========================================
    [Header("Keyboard Visuals")]
    [Tooltip("[ID] Masukkan parent dari semua cube keyboard di sini agar bisa diaktifkan/dimatikan.\n[EN] Insert the parent of all keyboard cubes here so it can be toggled on/off.")]
    public GameObject keyboardVisualContainer;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor (biasanya di tangan kanan).\n[EN] Reference to the Ray Interactor (usually on the right hand).")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;

    [Tooltip("[ID] Input Action untuk trigger/klik.\n[EN] Input Action for trigger/click.")]
    public InputActionReference selectAction;

    [Header("Keyboard Keys Mapping")]
    [Tooltip("[ID] Daftar pemetaan collider ke fungsi tombol.\n[EN] List of collider to key function mappings.")]
    public List<KeyDefinition> keyboardKeys = new List<KeyDefinition>();

    // [ID] Target input field yang sedang aktif saat ini
    // [EN] Currently active target input field
    private TMP_InputField targetInputField;
    private bool isShiftActive = false;
    private bool isCapsActive = false;

    // ==========================================
    // LIFECYCLE METHODS
    // ==========================================
    private void Start()
    {
        // [ID] Pastikan keyboard tersembunyi saat game dimulai
        // [EN] Ensure the keyboard is hidden when the game starts
        CloseKeyboard();
    }

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR
        // [EN] Enable VR input listener
        if (selectAction != null) selectAction.action.started += OnSelect;
    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR
        // [EN] Remove VR listener
        if (selectAction != null) selectAction.action.started -= OnSelect;
    }

    // ==========================================
    // FUNGSI BUKA TUTUP / OPEN & CLOSE FUNCTIONS
    // ==========================================
    public void OpenKeyboard(TMP_InputField newTarget)
    {
        targetInputField = newTarget;

        // [ID] Reset status shift/caps setiap kali keyboard dibuka
        // [EN] Reset shift/caps status every time the keyboard is opened
        isShiftActive = false;
        isCapsActive = false;

        if (keyboardVisualContainer != null)
        {
            keyboardVisualContainer.SetActive(true);
        }
    }

    public void CloseKeyboard()
    {
        if (keyboardVisualContainer != null)
        {
            keyboardVisualContainer.SetActive(false);
        }

        if (targetInputField != null)
        {
            // [ID] Nonaktifkan kursor/fokus pada InputField
            // [EN] Deactivate cursor/focus on the InputField
            targetInputField.DeactivateInputField();
            targetInputField = null;
        }
    }

    // ==========================================
    // LOGIC UTAMA / MAIN LOGIC
    // ==========================================
    private void OnSelect(InputAction.CallbackContext ctx)
    {
        // [ID] Jangan proses ketikan jika keyboard sedang ditutup atau tidak ada target
        // [EN] Do not process typing if keyboard is closed or there is no target
        if (targetInputField == null || !keyboardVisualContainer.activeSelf) return;

        // [ID] Cek apakah raycast VR mengenai sesuatu
        // [EN] Check if the VR raycast hits something
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            foreach (var key in keyboardKeys)
            {
                if (key.keyCollider == hit.collider)
                {
                    ProcessKey(key);
                    break;
                }
            }
        }
    }

    private void ProcessKey(KeyDefinition key)
    {
        // [ID] Pastikan UI tetap fokus agar kursor berkedip
        // [EN] Ensure UI remains focused so the cursor blinks
        EventSystem.current.SetSelectedGameObject(targetInputField.gameObject);

        string currentText = targetInputField.text;
        int caret = targetInputField.caretPosition;

        switch (key.function)
        {
            case KeyFunction.Character:
                string charToType = key.characterValue;

                // [ID] Logika Uppercase (Jika Caps ON atau Shift ON)
                // [EN] Uppercase Logic (If Caps is ON or Shift is ON)
                bool isUppercase = isCapsActive ^ isShiftActive;
                charToType = isUppercase ? charToType.ToUpper() : charToType.ToLower();

                // [ID] Sisipkan huruf di posisi kursor saat ini
                // [EN] Insert the character at the current cursor position
                targetInputField.text = currentText.Insert(caret, charToType);
                targetInputField.caretPosition = caret + charToType.Length;

                // [ID] Matikan shift setelah satu kali ketik
                // [EN] Turn off shift after one keystroke
                if (isShiftActive) isShiftActive = false;
                break;

            case KeyFunction.Space:
                targetInputField.text = currentText.Insert(caret, " ");
                targetInputField.caretPosition = caret + 1;
                break;

            case KeyFunction.Backspace:
                if (currentText.Length > 0 && caret > 0)
                {
                    targetInputField.text = currentText.Remove(caret - 1, 1);
                    targetInputField.caretPosition = caret - 1;
                }
                break;

            case KeyFunction.Enter:
                if (targetInputField.lineType != TMP_InputField.LineType.SingleLine)
                {
                    targetInputField.text = currentText.Insert(caret, "\n");
                    targetInputField.caretPosition = caret + 1;
                }
                else
                {
                    // [ID] Eksekusi event Submit jika input field hanya 1 baris
                    // [EN] Execute the Submit event if the input field is only 1 line
                    targetInputField.onEndEdit?.Invoke(targetInputField.text);
                    CloseKeyboard();
                }
                break;

            case KeyFunction.Tab:
                targetInputField.text = currentText.Insert(caret, "    ");
                targetInputField.caretPosition = caret + 4;
                break;

            case KeyFunction.CapsLock:
                isCapsActive = !isCapsActive;
                break;

            case KeyFunction.Shift:
                isShiftActive = !isShiftActive;
                break;

            case KeyFunction.ArrowLeft:
                if (caret > 0) targetInputField.caretPosition = caret - 1;
                break;

            case KeyFunction.ArrowRight:
                if (caret < currentText.Length) targetInputField.caretPosition = caret + 1;
                break;

            case KeyFunction.Esc:
                // [ID] Tombol Esc mematikan UI Keyboard 3D
                // [EN] Esc button deactivates the 3D Keyboard UI
                CloseKeyboard();
                break;
        }

        // [ID] Paksa update visual input field agar terlihat di Render Texture
        // [EN] Force update the visual input field so it's visible in the Render Texture
        if (targetInputField != null)
        {
            targetInputField.ForceLabelUpdate();
        }
    }
}