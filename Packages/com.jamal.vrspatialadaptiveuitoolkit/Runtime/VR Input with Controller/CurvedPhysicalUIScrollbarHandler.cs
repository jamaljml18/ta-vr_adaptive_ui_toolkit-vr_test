using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR Scrollbar dengan fitur DRAG.
    /// Mendukung DUA TANGAN. Menggunakan logika sumbu X Lokal. 
    /// Jika Scrollbar vertikal, putar GameObject Collider-nya 90 derajat.
    /// 
    /// [EN] Handles VR Scrollbar interaction with DRAG feature.
    /// Supports BOTH HANDS. Uses Local X-axis logic. 
    /// If Scrollbar is vertical, rotate the Collider GameObject 90 degrees.
    /// </summary>
    public class CurvedPhysicalUIScrollbarHandler : MonoBehaviour
    {
        [Header("Mapping Configuration")]
        [Tooltip("[ID] Box Collider sepanjang track Scrollbar.\n[EN] Box Collider along the Scrollbar track.")]
        public BoxCollider[] scrollbarColliders;

        [Tooltip("[ID] UI Scrollbar di Canvas.\n[EN] UI Scrollbar on Canvas.")]
        public Scrollbar[] canvasScrollbars;

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

        // State Variables
        private bool isDragging = false;
        private int activeIndex = -1;

        // [ID] Menyimpan Interactor mana (kiri/kanan) yang sedang aktif melakukan drag
        // [EN] Stores which Interactor (left/right) is currently dragging
        private XRRayInteractor activeInteractor = null;

        private void Start()
        {
            if (scrollbarColliders.Length != canvasScrollbars.Length)
                Debug.LogError($"[PhysicalScrollbar] Mismatch! Colliders: {scrollbarColliders.Length}, Scrollbars: {canvasScrollbars.Length}.");
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
            // [ID] Lepaskan listener VR
            // [EN] Remove VR listeners
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
        // INPUT EVENTS
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
                int index = Array.IndexOf(scrollbarColliders, hit.collider);

                if (index != -1 && index < canvasScrollbars.Length)
                {
                    isDragging = true;
                    activeIndex = index;
                    activeInteractor = interactor; // Simpan tangan yang aktif
                    UpdateScrollbarValue(hit.point); // Update instan saat klik pertama
                }
            }
        }

        private void OnLeftDragEnded(InputAction.CallbackContext ctx) => TryEndDrag(leftRayInteractor);
        private void OnRightDragEnded(InputAction.CallbackContext ctx) => TryEndDrag(rightRayInteractor);

        private void TryEndDrag(XRRayInteractor interactor)
        {
            // [ID] Hanya hentikan drag JIKA tangan yang dilepas adalah tangan yang sedang drag
            // [EN] Only stop drag IF the released hand is the one currently dragging
            if (isDragging && activeInteractor == interactor)
            {
                isDragging = false;
                activeIndex = -1;
                activeInteractor = null;
            }
        }

        // ============================================================
        // UPDATE LOOP
        // ============================================================
        private void Update()
        {
            // [ID] Gunakan activeInteractor untuk melacak posisi tangan yang benar
            // [EN] Use activeInteractor to track the correct hand's position
            if (isDragging && activeIndex != -1 && activeInteractor != null)
            {
                if (activeInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                {
                    // [ID] Pastikan masih kena collider yang sama agar tidak lompat
                    // [EN] Ensure it still hits the same collider to prevent jumping
                    if (hit.collider == scrollbarColliders[activeIndex])
                    {
                        UpdateScrollbarValue(hit.point);
                    }
                }
            }
        }

        private void UpdateScrollbarValue(Vector3 worldHitPoint)
        {
            BoxCollider col = scrollbarColliders[activeIndex];
            Scrollbar uiScrollbar = canvasScrollbars[activeIndex];

            if (col != null && uiScrollbar != null)
            {
                // [ID] 1. Konversi World ke Local (Otomatis handle rotasi)
                // [EN] 1. Convert World to Local (Automatically handles rotation)
                Vector3 localHitPoint = col.transform.InverseTransformPoint(worldHitPoint);

                // [ID] 2. Hitung posisi relatif X (0.0 sampai 1.0)
                // [EN] 2. Calculate relative X position (0.0 to 1.0)
                float colliderWidth = col.size.x;
                float adjustedX = localHitPoint.x + (colliderWidth / 2f);
                float normalizedValue = Mathf.Clamp01(adjustedX / colliderWidth);

                // [ID] 3. Terapkan ke Scrollbar
                // [EN] 3. Apply to Scrollbar
                uiScrollbar.value = normalizedValue;

                if (showDebug)
                    Debug.Log($"[Scrollbar] Val: {normalizedValue:F2} via {activeInteractor.name}");
            }
        }

        // ==========================================
        // GIZMOS
        // ==========================================
        private void OnDrawGizmosSelected()
        {
            if (scrollbarColliders == null || canvasScrollbars == null) return;

            Gizmos.color = Color.blue; // Warna Biru untuk Scrollbar
            for (int i = 0; i < scrollbarColliders.Length; i++)
            {
                if (i < canvasScrollbars.Length && scrollbarColliders[i] != null && canvasScrollbars[i] != null)
                {
                    Gizmos.DrawLine(scrollbarColliders[i].transform.position, canvasScrollbars[i].transform.position);

                    // Visualisasi Sumbu X Lokal (Arah Gerak)
                    Gizmos.color = Color.red;
                    Vector3 left = scrollbarColliders[i].transform.TransformPoint(new Vector3(-scrollbarColliders[i].size.x / 2, 0, 0));
                    Vector3 right = scrollbarColliders[i].transform.TransformPoint(new Vector3(scrollbarColliders[i].size.x / 2, 0, 0));
                    Gizmos.DrawLine(left, right);
                    Gizmos.color = Color.blue;
                }
            }
        }
    }
}