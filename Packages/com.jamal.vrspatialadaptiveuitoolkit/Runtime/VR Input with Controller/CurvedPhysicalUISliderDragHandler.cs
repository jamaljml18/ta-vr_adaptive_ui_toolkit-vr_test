using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR Slider dengan fitur DRAG (Tahan dan Geser).
    /// Mendukung DUA TANGAN. Menggunakan Update loop untuk mengubah nilai slider secara real-time.
    /// 
    /// [EN] Handles VR Slider interaction with DRAG feature (Hold and Slide).
    /// Supports BOTH HANDS. Uses Update loop to change slider value in real-time.
    /// </summary>
    public class CurvedPhysicalUISliderDragHandler : MonoBehaviour
    {
        [Header("Mapping Configuration")]
        public BoxCollider[] sliderColliders;
        public Slider[] canvasSliders;

        [Header("VR Settings - Interactors")]
        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kiri.\n[EN] Reference to the Left Hand Ray Interactor.")]
        [SerializeField] private XRRayInteractor leftRayInteractor;

        [Tooltip("[ID] Referensi ke Ray Interactor di tangan kanan.\n[EN] Reference to the Right Hand Ray Interactor.")]
        [SerializeField] private XRRayInteractor rightRayInteractor;

        [Header("VR Settings - Input Actions")]
        [Tooltip("[ID] Input Action untuk trigger/klik Kiri.\n[EN] Input Action for Left trigger/click.")]
        [SerializeField] private InputActionReference leftSelectAction;

        [Tooltip("[ID] Input Action untuk trigger/klik Kanan.\n[EN] Input Action for Right trigger/click.")]
        [SerializeField] private InputActionReference rightSelectAction;

        [Header("Debug")]
        [SerializeField] private bool showDebug = false;

        // [ID] State untuk mengecek apakah user sedang menahan trigger
        // [EN] State to check if user is holding the trigger
        private bool isDragging = false;

        // [ID] Menyimpan index slider mana yang sedang digerakkan
        // [EN] Stores the index of the slider currently being moved
        private int activeSliderIndex = -1;

        // [ID] Menyimpan Interactor mana (kiri/kanan) yang sedang aktif melakukan drag
        // [EN] Stores which Interactor (left/right) is currently dragging
        private XRRayInteractor activeInteractor = null;

        private void Start()
        {
            if (sliderColliders.Length != canvasSliders.Length)
                Debug.LogError($"[PhysicalSliderDrag] Mismatch! Colliders: {sliderColliders.Length}, Sliders: {canvasSliders.Length}.");
        }

        private void OnEnable()
        {
            // [ID] Daftarkan event Started (Ditekan) dan Canceled (Dilepas) untuk kedua tangan
            // [EN] Register Started (Pressed) and Canceled (Released) events for both hands
            if (leftSelectAction != null)
            {
                leftSelectAction.action.started += OnLeftDragStarted;
                leftSelectAction.action.canceled += OnLeftDragEnded;
            }
            if (rightSelectAction != null)
            {
                rightSelectAction.action.started += OnRightDragStarted;
                rightSelectAction.action.canceled += OnRightDragEnded;
            }
        }

        private void OnDisable()
        {
            // [ID] Lepaskan listener VR untuk mencegah memory leak
            // [EN] Remove VR listener to avoid memory leaks
            if (leftSelectAction != null)
            {
                leftSelectAction.action.started -= OnLeftDragStarted;
                leftSelectAction.action.canceled -= OnLeftDragEnded;
            }
            if (rightSelectAction != null)
            {
                rightSelectAction.action.started -= OnRightDragStarted;
                rightSelectAction.action.canceled -= OnRightDragEnded;
            }
        }

        // ============================================================
        // INPUT EVENTS (STATE MACHINE)
        // ============================================================

        private void OnLeftDragStarted(InputAction.CallbackContext ctx) => TryStartDrag(leftRayInteractor);
        private void OnRightDragStarted(InputAction.CallbackContext ctx) => TryStartDrag(rightRayInteractor);

        private void TryStartDrag(XRRayInteractor interactor)
        {
            // [ID] Abaikan jika sedang melakukan drag dengan tangan yang lain
            // [EN] Ignore if already dragging with the other hand
            if (isDragging) return;

            if (interactor != null && interactor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                int index = Array.IndexOf(sliderColliders, hit.collider);

                if (index != -1 && index < canvasSliders.Length)
                {
                    isDragging = true;
                    activeSliderIndex = index;
                    activeInteractor = interactor; // Simpan referensi tangan yang sedang drag

                    UpdateSliderValue(hit.point);
                }
            }
        }

        private void OnLeftDragEnded(InputAction.CallbackContext ctx) => TryEndDrag(leftRayInteractor);
        private void OnRightDragEnded(InputAction.CallbackContext ctx) => TryEndDrag(rightRayInteractor);

        private void TryEndDrag(XRRayInteractor interactor)
        {
            // [ID] Hanya hentikan drag JIKA tangan yang dilepas adalah tangan yang sedang melakukan drag
            // [EN] Only stop drag IF the released hand is the one currently dragging
            if (isDragging && activeInteractor == interactor)
            {
                isDragging = false;
                activeSliderIndex = -1;
                activeInteractor = null; // Reset tangan yang aktif
            }
        }

        // ============================================================
        // UPDATE LOOP (REAL-TIME CALCULATION)
        // ============================================================

        private void Update()
        {
            // [ID] Gunakan activeInteractor untuk mendapatkan posisi raycast
            // [EN] Use activeInteractor to get the raycast position
            if (isDragging && activeSliderIndex != -1 && activeInteractor != null)
            {
                if (activeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                {
                    // [ID] PENTING: Pastikan raycast masih mengenai collider slider yang sama
                    // [EN] CRITICAL: Ensure raycast is still hitting the same slider collider
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
                    Debug.Log($"[SliderDrag] Val: {newValue:F2} via {activeInteractor.name}");
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
}