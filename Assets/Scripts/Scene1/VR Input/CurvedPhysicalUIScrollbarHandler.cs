using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// [ID] Menangani interaksi VR Scrollbar dengan fitur DRAG.
/// Menggunakan logika sumbu X Lokal. Jika Scrollbar vertikal, putar GameObject Collider-nya 90 derajat.
/// 
/// [EN] Handles VR Scrollbar interaction with DRAG feature.
/// Uses Local X-axis logic. If Scrollbar is vertical, rotate the Collider GameObject 90 degrees.
/// </summary>
public class CurvedPhysicalUIScrollbarHandler : MonoBehaviour
{
    [Header("Mapping Configuration")]
    [Tooltip("[ID] Box Collider sepanjang track Scrollbar.\n[EN] Box Collider along the Scrollbar track.")]
    public BoxCollider[] scrollbarColliders;

    [Tooltip("[ID] UI Scrollbar di Canvas.\n[EN] UI Scrollbar on Canvas.")]
    public Scrollbar[] canvasScrollbars;

    [Header("VR Settings")]
    [Tooltip("[ID] Referensi ke Ray Interactor.\n[EN] Reference to the Ray Interactor.")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input Action")]
    [SerializeField] private InputActionReference selectAction;

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;

    // State Variables
    private bool isDragging = false;
    private int activeIndex = -1;

    private void Start()
    {
        if (scrollbarColliders.Length != canvasScrollbars.Length)
            Debug.LogError($"[PhysicalScrollbar] Mismatch! Colliders: {scrollbarColliders.Length}, Scrollbars: {canvasScrollbars.Length}.");
    }

    private void OnEnable()
    {
        if (selectAction != null)
        {
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
    // INPUT EVENTS
    // ============================================================
    private void OnDragStarted(InputAction.CallbackContext ctx)
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            int index = Array.IndexOf(scrollbarColliders, hit.collider);

            if (index != -1 && index < canvasScrollbars.Length)
            {
                isDragging = true;
                activeIndex = index;
                UpdateScrollbarValue(hit.point); // Update instan saat klik pertama
            }
        }
    }

    private void OnDragEnded(InputAction.CallbackContext ctx)
    {
        isDragging = false;
        activeIndex = -1;
    }

    // ============================================================
    // UPDATE LOOP
    // ============================================================
    private void Update()
    {
        if (isDragging && activeIndex != -1 && rayInteractor != null)
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
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
                Debug.Log($"[Scrollbar] Val: {normalizedValue:F2}");
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