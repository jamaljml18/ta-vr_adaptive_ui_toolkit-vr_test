using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Wajib untuk Input Action
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CurvedPhysicalUIButtonHandler : MonoBehaviour
{
    [Header("Mapping Config")]
    [Tooltip("Masukkan Collider 3D yang Anda buat manual di sini")]
    public Collider[] buttonBoxColliders;

    [Tooltip("Masukkan Tombol UI Canvas sesuai urutan Collider di atas")]
    public Button[] canvasButtons;

    [Header("VR Settings")]
    [SerializeField] private XRRayInteractor leftRayInteractor;
    // [SerializeField] private XRRayInteractor rightRayInteractor; // Jika ingin support 2 tangan

    [Header("Input Action")]
    // Masukkan Reference: XRI LeftHand Interaction/Select (Tombol Trigger)
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private void OnEnable()
    {
        if (selectAction != null)
            selectAction.action.started += OnTriggerPressed;
    }

    private void OnDisable()
    {
        if (selectAction != null)
            selectAction.action.started -= OnTriggerPressed;
    }

    // ==========================================
    // LOGIC UTAMA
    // ==========================================
    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        // Cek Interactor mana yang sedang aktif/valid
        if (leftRayInteractor != null)
        {
            CheckAndClick(leftRayInteractor);
        }

        // Anda bisa menambahkan logika untuk tangan kanan di sini jika perlu
    }

    private void CheckAndClick(XRRayInteractor interactor)
    {
        // 1. Minta data Raycast Hit dari Interactor
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // 2. Cek apakah objek yang tertabrak ada di dalam daftar Array Collider kita?
            // Fungsi Array.IndexOf akan mencari nomor index collider yang tertabrak
            // Jika tidak ketemu, hasilnya -1.
            int index = Array.IndexOf(buttonBoxColliders, hit.collider);

            if (index != -1) // Artinya KETEMU!
            {
                // 3. Pastikan ada tombol pasangan di index yang sama
                if (index < canvasButtons.Length && canvasButtons[index] != null)
                {
                    if (showDebug)
                        Debug.Log($"[PhysicalUI] Collider '{hit.collider.name}' hit! Clicking Button '{canvasButtons[index].name}'");

                    // 4. KLIK TOMBOLNYA
                    canvasButtons[index].onClick.Invoke();
                }
                else
                {
                    Debug.LogWarning($"[PhysicalUI] Collider ketemu di index {index}, tapi Array Button kosong/null di index itu!");
                }
            }
            else
            {
                if (showDebug) Debug.Log($"[PhysicalUI] Raycast kena '{hit.collider.name}', tapi bukan bagian dari tombol UI.");
            }
        }
    }

    // ==========================================
    // VISUALISASI DI EDITOR (GIZMOS)
    // ==========================================
    // Ini membantu Anda melihat garis penghubung antara Collider dan Tombol di Scene View
    private void OnDrawGizmosSelected()
    {
        if (buttonBoxColliders == null || canvasButtons == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < buttonBoxColliders.Length; i++)
        {
            if (i < canvasButtons.Length && buttonBoxColliders[i] != null && canvasButtons[i] != null)
            {
                // Gambar garis dari collider 3D ke posisi canvas (hanya visualisasi editor)
                Gizmos.DrawLine(buttonBoxColliders[i].transform.position, canvasButtons[i].transform.position);
            }
        }
    }
}