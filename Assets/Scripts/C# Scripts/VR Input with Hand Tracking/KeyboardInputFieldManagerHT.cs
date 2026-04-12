using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Manajer utama untuk Keyboard 3D kustom di antarmuka VR, dioptimalkan untuk Hand Tracking.
/// Mendukung DUA TANGAN (Gestur Cubit). Menangani logika pengetikan menggunakan raycast pada Collider tombol 3D.
/// Kompatibel dengan arsitektur XR Interaction Toolkit (XRI) 3.0+.
/// 
/// [EN] Main manager for the custom 3D Keyboard in VR interface, optimized for Hand Tracking.
/// Supports BOTH HANDS (Pinch Gesture). Handles typing logic using raycasts on 3D button Colliders.
/// Compatible with XR Interaction Toolkit (XRI) 3.0+ architecture.
/// </summary>
public class KeyboardInputFieldManagerHT : MonoBehaviour
{
    // ==========================================
    // DEFINISI STRUKTUR / STRUCT DEFINITIONS
    // ==========================================

    /// <summary>
    /// [ID] Kumpulan fungsi yang dapat dijalankan oleh tombol keyboard.
    /// [EN] The collection of functions that a keyboard key can perform.
    /// </summary>
    public enum KeyFunction
    {
        Character, Space, Backspace, Delete, Enter, Tab,
        CapsLock, Shift, Esc, ArrowLeft, ArrowRight,
        ArrowUp, ArrowDown, Ctrl, Alt
    }

    /// <summary>
    /// [ID] Struktur untuk memetakan Collider fisik 3D ke fungsi dan nilai karakter spesifik.
    /// [EN] Structure to map a physical 3D Collider to a specific key function and character value.
    /// </summary>
    [System.Serializable]
    public struct KeyDefinition
    {
        [Tooltip("[ID] Collider 3D fisik untuk tombol ini.\n[EN] Physical 3D Collider for this key.")]
        public Collider keyCollider;

        [Tooltip("[ID] Fungsi dari tombol ini (huruf, spasi, hapus, dll).\n[EN] Function of this key (character, space, backspace, etc).")]
        public KeyFunction function;

        [Tooltip("[ID] Isi karakter (misal: a, 1, [, /). Kosongkan jika ini adalah tombol fungsi (seperti Enter/Shift).\n[EN] Character value (e.g., a, 1, [, /). Leave blank if this is a function key (like Enter/Shift).")]
        public string characterValue;
    }

    // ==========================================
    // REFERENSI & VARIABEL / REFERENCES & VARIABLES
    // ==========================================

    [Header("Keyboard Visuals")]
    [Tooltip("[ID] Masukkan parent (Wadah utama) dari seluruh visual keyboard agar bisa diaktifkan/dinonaktifkan.\n[EN] Insert the parent container of all keyboard visuals so it can be toggled on/off.")]
    public GameObject keyboardVisualContainer;

    [Header("VR Settings - Interactors")]
    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
    public XRRayInteractor leftRayInteractor;

    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
    public XRRayInteractor rightRayInteractor;

    [Header("VR Settings - Input Actions (XRI 3.0+)")]
    [Tooltip("[ID] Input Action dari tangan Kiri untuk melakukan cubitan (Pinch).\n[EN] Left hand Input Action for pinching.")]
    public InputActionReference leftSelectAction;

    [Tooltip("[ID] Input Action dari tangan Kanan untuk melakukan cubitan (Pinch).\n[EN] Right hand Input Action for pinching.")]
    public InputActionReference rightSelectAction;

    [Header("Keyboard Keys Mapping")]
    [Tooltip("[ID] Daftar pemetaan setiap collider tombol fisik ke fungsinya masing-masing.\n[EN] List mapping each physical key collider to its respective function.")]
    public List<KeyDefinition> keyboardKeys = new List<KeyDefinition>();

    // [ID] Menyimpan referensi Input Field yang saat ini sedang aktif dan menerima ketikan.
    // [EN] Stores the reference to the currently active Input Field receiving input.
    private TMP_InputField targetInputField;

    // [ID] Status modifier keyboard.
    // [EN] Keyboard modifier states.
    private bool isShiftActive = false;
    private bool isCapsActive = false;

    // ==========================================
    // METODE SIKLUS HIDUP / LIFECYCLE METHODS
    // ==========================================

    private void Start()
    {
        // [ID] Pastikan visual keyboard disembunyikan saat aplikasi pertama kali berjalan.
        // [EN] Ensure the keyboard visuals are hidden when the application first starts.
        CloseKeyboard();
    }

    private void OnEnable()
    {
        // [ID] Daftarkan event listener untuk menangkap gestur cubitan (pinch) dari KEDUA tangan.
        // [EN] Register the event listeners to catch pinch gestures from BOTH hands.
        if (leftSelectAction != null && leftSelectAction.action != null)
        {
            leftSelectAction.action.Enable(); // Wajib di XRI 3.0+
            leftSelectAction.action.started += OnLeftSelect;
        }

        if (rightSelectAction != null && rightSelectAction.action != null)
        {
            rightSelectAction.action.Enable(); // Wajib di XRI 3.0+
            rightSelectAction.action.started += OnRightSelect;
        }
    }

    private void OnDisable()
    {
        // [ID] Lepaskan event listener saat script dinonaktifkan untuk mencegah memory leak.
        // [EN] Unregister the event listeners when the script is disabled to prevent memory leaks.
        if (leftSelectAction != null && leftSelectAction.action != null)
            leftSelectAction.action.started -= OnLeftSelect;

        if (rightSelectAction != null && rightSelectAction.action != null)
            rightSelectAction.action.started -= OnRightSelect;
    }

    // ==========================================
    // FUNGSI BUKA TUTUP / OPEN & CLOSE FUNCTIONS
    // ==========================================

    public void OpenKeyboard(TMP_InputField newTarget)
    {
        targetInputField = newTarget;

        // [ID] Reset status modifier setiap kali keyboard baru dibuka.
        // [EN] Reset modifier states every time a new keyboard is opened.
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
            // [ID] Nonaktifkan Input Field agar kursor (caret) berhenti berkedip.
            // [EN] Deactivate the Input Field so the blinking cursor (caret) stops.
            targetInputField.DeactivateInputField();
            targetInputField = null;
        }
    }

    // ==========================================
    // LOGIKA UTAMA / MAIN LOGIC
    // ==========================================

    private void OnLeftSelect(InputAction.CallbackContext ctx) => ProcessSelection(leftRayInteractor);
    private void OnRightSelect(InputAction.CallbackContext ctx) => ProcessSelection(rightRayInteractor);

    /// <summary>
    /// [ID] Dipanggil saat pemain melakukan gestur cubitan (pinch) dengan tangannya.
    /// [EN] Called when the player performs a pinch gesture with their hands.
    /// </summary>
    private void ProcessSelection(XRRayInteractor interactor)
    {
        // [ID] Abaikan input jika tidak ada target InputField atau jika keyboard sedang tertutup.
        // [EN] Ignore input if there is no target InputField or if the keyboard is currently closed.
        if (targetInputField == null || !keyboardVisualContainer.activeSelf) return;

        // [ID] Periksa apakah tembakan laser tangan (raycast) VR mengenai suatu objek 3D.
        // [EN] Check if the VR hand raycast hits a 3D object.
        if (interactor != null && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // [ID] Cari tombol mana yang cocok dengan objek yang terkena raycast.
            // [EN] Find which key matches the object hit by the raycast.
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

    /// <summary>
    /// [ID] Memproses logika pengetikan berdasarkan tipe tombol yang ditekan (dicubit).
    /// [EN] Processes the typing logic based on the type of key pressed (pinched).
    /// </summary>
    private void ProcessKey(KeyDefinition key)
    {
        // [ID] Paksa EventSystem untuk tetap fokus pada InputField agar kursor tetap aktif saat mengetik.
        // [EN] Force the EventSystem to keep focus on the InputField so the cursor remains active while typing.
        EventSystem.current.SetSelectedGameObject(targetInputField.gameObject);

        string currentText = targetInputField.text;
        int caret = targetInputField.caretPosition;

        switch (key.function)
        {
            case KeyFunction.Character:
                string charToType = key.characterValue;

                // [ID] Periksa apakah karakter yang diketik adalah abjad (A-Z).
                // [EN] Check if the typed character is an alphabet letter (A-Z).
                bool isLetter = charToType.Length == 1 && char.IsLetter(charToType[0]);

                if (isLetter)
                {
                    // [ID] Logika huruf besar: aktif jika CapsLock ATAU Shift menyala, tapi tidak keduanya (XOR).
                    // [EN] Uppercase logic: active if CapsLock OR Shift is on, but not both (XOR).
                    bool isUppercase = isCapsActive ^ isShiftActive;
                    charToType = isUppercase ? charToType.ToUpper() : charToType.ToLower();
                }
                else
                {
                    // [ID] Logika simbol: angka dan tanda baca hanya dipengaruhi oleh tombol Shift.
                    // [EN] Symbol logic: numbers and punctuation are only affected by the Shift key.
                    if (isShiftActive)
                    {
                        charToType = GetShiftedSymbol(charToType);
                    }
                }

                // [ID] Sisipkan karakter ke dalam teks pada posisi kursor saat ini.
                // [EN] Insert the character into the text at the current cursor position.
                targetInputField.text = currentText.Insert(caret, charToType);
                targetInputField.caretPosition = caret + charToType.Length;

                // [ID] Matikan Shift secara otomatis setelah satu karakter diketik.
                // [EN] Turn off Shift automatically after one character is typed.
                if (isShiftActive) isShiftActive = false;
                break;

            case KeyFunction.Space:
                targetInputField.text = currentText.Insert(caret, " ");
                targetInputField.caretPosition = caret + 1;
                break;

            case KeyFunction.Backspace:
                // [ID] Hapus 1 karakter di sebelah KIRI kursor.
                // [EN] Delete 1 character to the LEFT of the cursor.
                if (currentText.Length > 0 && caret > 0)
                {
                    targetInputField.text = currentText.Remove(caret - 1, 1);
                    targetInputField.caretPosition = caret - 1;
                }
                break;

            case KeyFunction.Delete:
                // [ID] Hapus 1 karakter di sebelah KANAN kursor.
                // [EN] Delete 1 character to the RIGHT of the cursor.
                if (caret >= 0 && caret < currentText.Length)
                {
                    targetInputField.text = currentText.Remove(caret, 1);
                    // [ID] Posisi kursor tetap, karena teks bergeser ke kiri menutupi celah.
                    // [EN] Cursor position remains the same, as the text shifts left to fill the gap.
                }
                break;

            case KeyFunction.Enter:
                if (targetInputField.lineType != TMP_InputField.LineType.SingleLine)
                {
                    // [ID] Tambahkan baris baru jika mode teks adalah MultiLine.
                    // [EN] Add a new line if the text mode is MultiLine.
                    targetInputField.text = currentText.Insert(caret, "\n");
                    targetInputField.caretPosition = caret + 1;
                }
                else
                {
                    // [ID] Eksekusi event Submit (selesai mengetik) lalu tutup keyboard.
                    // [EN] Execute the Submit event (finished typing) then close the keyboard.
                    targetInputField.onEndEdit?.Invoke(targetInputField.text);
                    CloseKeyboard();
                }
                break;

            case KeyFunction.Tab:
                // [ID] Sisipkan 4 spasi sebagai representasi Tab.
                // [EN] Insert 4 spaces to represent a Tab.
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
                // [ID] Menutup keyboard saat tombol Escape ditekan.
                // [EN] Closes the keyboard when the Escape key is pressed.
                CloseKeyboard();
                break;
        }

        // [ID] Perbarui visual Input Field secara instan agar perubahan terlihat di VR Render Texture.
        // [EN] Instantly update the Input Field visuals so changes appear on the VR Render Texture.
        if (targetInputField != null)
        {
            targetInputField.ForceLabelUpdate();
        }
    }

    // ==========================================
    // FUNGSI PENDUKUNG / HELPER FUNCTIONS
    // ==========================================

    /// <summary>
    /// [ID] Menerjemahkan karakter dasar ke simbol alternatifnya saat Shift ditekan (Berdasarkan standar US Keyboard).
    /// [EN] Translates a base character to its alternate symbol when Shift is pressed (Based on US Keyboard standards).
    /// </summary>
    private string GetShiftedSymbol(string baseChar)
    {
        switch (baseChar)
        {
            case "1": return "!";
            case "2": return "@";
            case "3": return "#";
            case "4": return "$";
            case "5": return "%";
            case "6": return "^";
            case "7": return "&";
            case "8": return "*";
            case "9": return "(";
            case "0": return ")";
            case "-": return "_";
            case "=": return "+";
            case "[": return "{";
            case "]": return "}";
            case "\\": return "|";
            case ";": return ":";
            case "'": return "\"";
            case ",": return "<";
            case ".": return ">";
            case "/": return "?";
            case "`": return "~";
            default: return baseChar;
        }
    }
}