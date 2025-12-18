using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedUIInteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject cubeDebug;
    // --- BUTTONS ---

    public void ButtonA()
    {
        cubeDebug.SetActive(!cubeDebug.activeSelf);
        // [ID] Mencetak pesan saat Button A ditekan
        // [EN] Prints a message when Button A is pressed
        print("Button A Pressed");
    }

    public void ButtonB()
    {
        cubeDebug.SetActive(!cubeDebug.activeSelf);
        // [ID] Mencetak pesan saat Button B ditekan
        // [EN] Prints a message when Button B is pressed
        print("Button B Pressed");
    }


    // --- TOGGLES ---

    /// <summary>
    /// [ID] Fungsi ini dipanggil otomatis ketika nilai Toggle A berubah (Dicentang/Tidak).
    /// [EN] This function is called automatically when Toggle A's value changes (Checked/Unchecked).
    /// </summary>
    /// <param name="isOn">[ID] Status baru toggle (True = Hidup, False = Mati) / [EN] New toggle state (True = On, False = Off)</param>
    public void ToggleA(bool isOn)
    {
        if (isOn)
        {
            // [ID] Logika jika Toggle A menyala (aktif)
            // [EN] Logic if Toggle A is turned on (active)
            print("Toggle A: ON (Active)");
        }
        else
        {
            // [ID] Logika jika Toggle A mati (tidak aktif)
            // [EN] Logic if Toggle A is turned off (inactive)
            print("Toggle A: OFF (Inactive)");
        }
    }

    /// <summary>
    /// [ID] Fungsi ini dipanggil otomatis ketika nilai Toggle B berubah.
    /// [EN] This function is called automatically when Toggle B's value changes.
    /// </summary>
    /// <param name="isOn">[ID] Status baru toggle / [EN] New toggle state</param>
    public void ToggleB(bool isOn)
    {
        if (isOn)
        {
            // [ID] Logika jika Toggle B menyala
            // [EN] Logic if Toggle B is turned on
            print("Toggle B: ON (Active)");
        }
        else
        {
            // [ID] Logika jika Toggle B mati
            // [EN] Logic if Toggle B is turned off
            print("Toggle B: OFF (Inactive)");
        }
    }


    // --- SLIDERS ---

    /// <summary>
    /// [ID] Dipanggil otomatis oleh Slider A ketika nilainya berubah.
    /// [EN] Automatically called by Slider A when its value changes.
    /// </summary>
    /// <param name="value">[ID] Nilai slider saat ini / [EN] Current slider value</param>
    public void SliderA(float value)
    {
        // [ID] Menampilkan nilai Slider A
        // [EN] Print Slider A's current value
        print("Slider A Value: " + value);
    }

    /// <summary>
    /// [ID] Dipanggil otomatis oleh Slider B ketika nilainya berubah.
    /// [EN] Automatically called by Slider B when its value changes.
    /// </summary>
    /// <param name="value">[ID] Nilai slider saat ini / [EN] Current slider value</param>
    public void SliderB(float value)
    {
        // [ID] Menampilkan nilai Slider B
        // [EN] Print Slider B's current value
        print("Slider B Value: " + value);
    }

    // --- SCROLLBARS ---

    /// <summary>
    /// [ID] Dipanggil otomatis oleh Scrollbar A ketika nilainya berubah.
    /// [EN] Automatically called by Scrollbar A when its value changes.
    /// </summary>
    /// <param name="value">[ID] Nilai scrollbar saat ini (0.0 - 1.0) / [EN] Current scrollbar value (0.0 - 1.0)</param>
    public void ScrollbarA(float value)
    {
        // [ID] Menampilkan nilai Scrollbar A
        // [EN] Print Scrollbar A's current value
        print("Scrollbar A Value: " + value);
    }

    /// <summary>
    /// [ID] Dipanggil otomatis oleh Scrollbar B ketika nilainya berubah.
    /// [EN] Automatically called by Scrollbar B when its value changes.
    /// </summary>
    /// <param name="value">[ID] Nilai scrollbar saat ini (0.0 - 1.0) / [EN] Current scrollbar value (0.0 - 1.0)</param>
    public void ScrollbarB(float value)
    {
        // [ID] Menampilkan nilai Scrollbar B
        // [EN] Print Scrollbar B's current value
        print("Scrollbar B Value: " + value);
    }
}