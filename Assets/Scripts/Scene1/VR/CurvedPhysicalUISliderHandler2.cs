using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR Slider menggunakan pendekatan "Physical Proxy".
/// Menghitung posisi relatif raycast pada Box Collider untuk menentukan nilai Slider.
/// 
/// [EN] Handles VR Slider interaction using the "Physical Proxy" approach.
/// Calculates the relative raycast position on the Box Collider to determine Slider value.
/// </summary>
public class CurvedPhysicalUISliderHandler2 : MonoBehaviour
{
    [Header("Mapping Configuration")]
    [Tooltip("[ID] Daftar Box Collider yang menutupi area batang/track Slider.\n[EN] List of Box Colliders covering the Slider track area.")]
    public BoxCollider[] sliderColliders;

    [Tooltip("[ID] Daftar UI Slider yang sesuai dengan urutan Collider di atas.\n[EN] List of UI Sliders corresponding to the order of Colliders above.")]
    public Slider[] canvasSliders;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor.\n[EN] Reference to the Ray Interactor.")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private void Start()
    {
        // [ID] Validasi panjang array
        if (sliderColliders.Length != canvasSliders.Length)
            Debug.LogError($"[PhysicalSlider] Mismatch! Colliders: {sliderColliders.Length}, Sliders: {canvasSliders.Length}.");
    }

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

    // [ID] Event saat trigger ditekan (Sekali klik)
    // [EN] Event when trigger is pressed (Single click)
    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null)
        {
            CheckAndSlide(rayInteractor);
        }
    }

    // [ID] Fungsi utama perhitungan Slider
    // [EN] Main Slider calculation function
    private void CheckAndSlide(XRRayInteractor interactor)
    {
        if (interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // 1. Cari index collider
            int index = Array.IndexOf(sliderColliders, hit.collider);

            if (index != -1 && index < canvasSliders.Length)
            {
                // [ID] 2. Ambil Collider dan Slider yang cocok
                BoxCollider col = sliderColliders[index];
                Slider uiSlider = canvasSliders[index];

                if (col != null && uiSlider != null)
                {
                    // ==========================================================
                    // MATEMATIKA LOKAL (LOCAL SPACE MATH)
                    // ==========================================================

                    // [ID] Ubah posisi tabrak dunia (World) ke posisi lokal Collider
                    // Ini penting agar rotasi melengkung tidak merusak perhitungan
                    // [EN] Convert World hit position to Collider's Local position
                    // This is crucial so curved rotation doesn't break calculation
                    Vector3 localHitPoint = col.transform.InverseTransformPoint(hit.point);

                    // [ID] Ukuran lebar collider (X-Axis)
                    // [EN] Collider width size (X-Axis)
                    float colliderWidth = col.size.x;

                    // [ID] Posisi X lokal biasanya range dari -(Width/2) sampai +(Width/2)
                    // Kita geser agar mulainya dari 0 sampai Width
                    // [EN] Local X position usually ranges from -(Width/2) to +(Width/2)
                    // We shift it so it starts from 0 to Width
                    float adjustedX = localHitPoint.x + (colliderWidth / 2f);

                    // [ID] Hitung persentase (0.0 sampai 1.0)
                    // [EN] Calculate percentage (0.0 to 1.0)
                    float normalizedValue = Mathf.Clamp01(adjustedX / colliderWidth);

                    // [ID] Aplikasikan ke Slider UI
                    // Rumus: MinValue + (Persentase * (Max - Min))
                    // [EN] Apply to UI Slider
                    // Formula: MinValue + (Percentage * (Max - Min))
                    float newValue = uiSlider.minValue + (normalizedValue * (uiSlider.maxValue - uiSlider.minValue));

                    uiSlider.value = newValue;

                    if (showDebug)
                        Debug.Log($"[PhysicalSlider] Hit Local X: {localHitPoint.x:F2} -> Norm: {normalizedValue:P0} -> Value: {newValue}");
                }
            }
        }
    }

    // ==========================================
    // VISUALISASI EDITOR
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (sliderColliders == null || canvasSliders == null) return;

        Gizmos.color = Color.magenta; // Warna Magenta untuk Slider
        for (int i = 0; i < sliderColliders.Length; i++)
        {
            if (i < canvasSliders.Length && sliderColliders[i] != null && canvasSliders[i] != null)
            {
                Gizmos.DrawLine(sliderColliders[i].transform.position, canvasSliders[i].transform.position);

                // [ID] Visualisasi arah slider (Sumbu X merah)
                // [EN] Visualize slider direction (Red X-Axis)
                Gizmos.color = Color.red;
                Vector3 left = sliderColliders[i].transform.TransformPoint(new Vector3(-sliderColliders[i].size.x / 2, 0, 0));
                Vector3 right = sliderColliders[i].transform.TransformPoint(new Vector3(sliderColliders[i].size.x / 2, 0, 0));
                Gizmos.DrawLine(left, right);
                Gizmos.color = Color.magenta;
            }
        }
    }
}

// Jika Slider Anda vertikal, putar Game Object Box Collider-nya 90 derajat (Z axis),
// jangan ubah Size Y-nya, tapi tetap gunakan Size X sebagai panjang slider.
// Script ini mengasumsikan "Panjang Slider = Size X Collider".