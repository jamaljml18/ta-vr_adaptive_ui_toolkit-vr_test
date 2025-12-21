using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR Slider dengan fitur DRAG (Tahan dan Geser).
/// Menggunakan Update loop untuk mengubah nilai slider secara real-time saat trigger ditahan.
/// 
/// [EN] Handles VR Slider interaction with DRAG feature (Hold and Slide).
/// Uses Update loop to change slider value in real-time while trigger is held.
/// </summary>
public class CurvedPhysicalUISliderDragHandler : MonoBehaviour
{
    [Header("Mapping Configuration")]
    public BoxCollider[] sliderColliders;
    public Slider[] canvasSliders;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor.\n[EN] Reference to the Ray Interactor.")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    // [ID] State untuk mengecek apakah user sedang menahan trigger
    // [EN] State to check if user is holding the trigger
    private bool isDragging = false;

    // [ID] Menyimpan index slider mana yang sedang digerakkan
    // [EN] Stores the index of the slider currently being moved
    private int activeSliderIndex = -1;

    private void Start()
    {
        if (sliderColliders.Length != canvasSliders.Length)
            Debug.LogError($"[PhysicalSliderDrag] Mismatch! Colliders: {sliderColliders.Length}, Sliders: {canvasSliders.Length}.");
    }

    private void OnEnable()
    {
        if (selectAction != null)
        {
            // [ID] Kita butuh dua event: Saat ditekan (Mulai) dan Saat dilepas (Berhenti)
            // [EN] We need two events: When pressed (Start) and When released (Stop)
            selectAction.action.started += OnDragStarted;
            selectAction.action.canceled += OnDragEnded;
        }
    }

    private void OnDisable()
    {
        if (selectAction != null)
        {
            selectAction.action.started -= OnDragStarted;
            selectAction.action.canceled -= OnDragEnded;
        }
    }

    // ============================================================
    // INPUT EVENTS (STATE MACHINE)
    // ============================================================

    // [ID] 1. Saat Trigger Ditekan: Cek apakah kena slider? Jika ya, mulai Dragging.
    // [EN] 1. When Trigger Pressed: Check if hitting slider? If yes, start Dragging.
    private void OnDragStarted(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Cek apakah yang kena adalah salah satu collider slider kita
            int index = Array.IndexOf(sliderColliders, hit.collider);

            if (index != -1 && index < canvasSliders.Length)
            {
                isDragging = true;
                activeSliderIndex = index;

                // [ID] Opsional: Update nilai sekali saat pertama ditekan (agar responsif)
                // [EN] Optional: Update value once on first press (to be responsive)
                UpdateSliderValue(hit.point);
            }
        }
    }

    // [ID] 2. Saat Trigger Dilepas: Hentikan Dragging.
    // [EN] 2. When Trigger Released: Stop Dragging.
    private void OnDragEnded(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        activeSliderIndex = -1;
    }

    // ============================================================
    // UPDATE LOOP (REAL-TIME CALCULATION)
    // ============================================================

    private void Update()
    {
        // [ID] Jika sedang dragging DAN ada slider yang aktif
        // [EN] If currently dragging AND there is an active slider
        if (isDragging && activeSliderIndex != -1 && rayInteractor != null)
        {
            // [ID] Dapatkan posisi raycast terbaru setiap frame
            // [EN] Get the latest raycast position every frame
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // [ID] PENTING: Pastikan raycast masih mengenai collider slider yang sama
                // Jika laser keluar dari kotak slider, kita berhenti update agar tidak error/lompat
                // [EN] CRITICAL: Ensure raycast is still hitting the same slider collider
                // If laser exits the slider box, we stop updating to prevent errors/jumping
                if (hit.collider == sliderColliders[activeSliderIndex])
                {
                    UpdateSliderValue(hit.point);
                }
            }
        }
    }

    // [ID] Logika Matematika (Sama seperti sebelumnya)
    // [EN] Math Logic (Same as before)
    private void UpdateSliderValue(Vector3 worldHitPoint)
    {
        BoxCollider col = sliderColliders[activeSliderIndex];
        Slider uiSlider = canvasSliders[activeSliderIndex];

        if (col != null && uiSlider != null)
        {
            // 1. World to Local
            Vector3 localHitPoint = col.transform.InverseTransformPoint(worldHitPoint);

            // 2. Calculate Normalized Value (0 to 1)
            float colliderWidth = col.size.x;

            // Shift range from [-Width/2, +Width/2] to [0, Width]
            float adjustedX = localHitPoint.x + (colliderWidth / 2f);
            float normalizedValue = Mathf.Clamp01(adjustedX / colliderWidth);

            // 3. Apply to UI
            float newValue = uiSlider.minValue + (normalizedValue * (uiSlider.maxValue - uiSlider.minValue));
            uiSlider.value = newValue;

            if (showDebug)
                Debug.Log($"[SliderDrag] Val: {newValue:F2}");
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

                Gizmos.color = Color.red;
                Vector3 left = sliderColliders[i].transform.TransformPoint(new Vector3(-sliderColliders[i].size.x / 2, 0, 0));
                Vector3 right = sliderColliders[i].transform.TransformPoint(new Vector3(sliderColliders[i].size.x / 2, 0, 0));
                Gizmos.DrawLine(left, right);
                Gizmos.color = Color.magenta;
            }
        }
    }
}