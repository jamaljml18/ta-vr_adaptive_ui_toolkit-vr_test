using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR Slider dengan Hand Tracking (Auto-Hover).
/// Mendukung DUA TANGAN. Nilai slider berubah instan saat laser menyentuh collider tanpa perlu dicubit.
/// 
/// [EN] Handles VR Slider interaction with Hand Tracking (Auto-Hover).
/// Supports BOTH HANDS. Slider value changes instantly when the laser hits the collider, no pinch required.
/// </summary>
public class CurvedPhysicalUISliderDragHandlerHT : MonoBehaviour
{
    [Header("Mapping Configuration")]
    [Tooltip("[ID] Box Collider sepanjang track Slider.\n[EN] Box Collider along the Slider track.")]
    public BoxCollider[] sliderColliders;

    [Tooltip("[ID] UI Slider di Canvas.\n[EN] UI Slider on Canvas.")]
    public Slider[] canvasSliders;

    [Header("VR Settings - Interactors")]
    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
    [SerializeField] private XRRayInteractor leftRayInteractor;

    [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
    [SerializeField] private XRRayInteractor rightRayInteractor;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    private void Start()
    {
        if (sliderColliders.Length != canvasSliders.Length)
            Debug.LogError($"[PhysicalSliderHT] Mismatch! Colliders: {sliderColliders.Length}, Sliders: {canvasSliders.Length}.");
    }

    // ============================================================
    // UPDATE LOOP (HOVER DETECTION)
    // ============================================================
    private void Update()
    {
        // [ID] Cek tangan Kiri terlebih dahulu
        // [EN] Check Left hand first
        if (leftRayInteractor != null && leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit leftHit))
        {
            ProcessHover(leftHit, leftRayInteractor.name);
        }

        // [ID] Cek tangan Kanan (Bisa menimpa input kiri jika mengenai slider lain)
        // [EN] Check Right hand (Can override left input if it hits another slider)
        if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit rightHit))
        {
            ProcessHover(rightHit, rightRayInteractor.name);
        }
    }

    private void ProcessHover(RaycastHit hit, string interactorName)
    {
        // [ID] Cari apakah objek yang tersorot laser adalah salah satu collider slider kita
        // [EN] Check if the object hit by the laser is one of our slider colliders
        int index = Array.IndexOf(sliderColliders, hit.collider);

        // [ID] Jika kena, langsung update nilainya
        // [EN] If hit, update the value immediately
        if (index != -1 && index < canvasSliders.Length)
        {
            UpdateSliderValue(index, hit.point, interactorName);
        }
    }

    private void UpdateSliderValue(int index, Vector3 worldHitPoint, string interactorName)
    {
        BoxCollider col = sliderColliders[index];
        Slider uiSlider = canvasSliders[index];

        if (col != null && uiSlider != null)
        {
            // [ID] 1. Konversi World ke Local (Otomatis menangani rotasi dan kurva)
            // [EN] 1. Convert World to Local (Automatically handles rotation and curves)
            Vector3 localHitPoint = col.transform.InverseTransformPoint(worldHitPoint);

            // [ID] 2. Hitung posisi relatif X (0.0 sampai 1.0)
            // [EN] 2. Calculate relative X position (0.0 to 1.0)
            float colliderWidth = col.size.x;

            // [ID] Geser rentang dari [-Width/2, +Width/2] menjadi [0, Width]
            // [EN] Shift range from [-Width/2, +Width/2] to [0, Width]
            float adjustedX = localHitPoint.x + (colliderWidth / 2f);
            float normalizedValue = Mathf.Clamp01(adjustedX / colliderWidth);

            // [ID] 3. Terapkan nilai ke UI Slider berdasarkan min/max-nya secara instan
            // [EN] 3. Apply value to the UI Slider based on its min/max instantly
            float newValue = uiSlider.minValue + (normalizedValue * (uiSlider.maxValue - uiSlider.minValue));
            uiSlider.value = newValue;

            if (showDebug)
                Debug.Log($"[SliderHT] Val: {newValue:F2} via {interactorName} (Auto-Hover)");
        }
    }

    // ==========================================
    // VISUALISASI EDITOR
    // ==========================================
    private void OnDrawGizmosSelected()
    {
        if (sliderColliders == null || canvasSliders == null) return;

        Gizmos.color = Color.magenta;
        for (int i = 0; i < sliderColliders.Length; i++)
        {
            if (i < canvasSliders.Length && sliderColliders[i] != null && canvasSliders[i] != null)
            {
                Gizmos.DrawLine(sliderColliders[i].transform.position, canvasSliders[i].transform.position);

                // Visualisasi Sumbu X Lokal (Arah Gerak)
                Gizmos.color = Color.red;
                Vector3 left = sliderColliders[i].transform.TransformPoint(new Vector3(-sliderColliders[i].size.x / 2, 0, 0));
                Vector3 right = sliderColliders[i].transform.TransformPoint(new Vector3(sliderColliders[i].size.x / 2, 0, 0));
                Gizmos.DrawLine(left, right);
                Gizmos.color = Color.magenta;
            }
        }
    }
}