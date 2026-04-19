using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Jamal.VRSpatialAdaptiveUIToolkit
{
    /// <summary>
    /// [ID] Menangani interaksi VR Scroll View (Scrollbar) dengan Hand Tracking (Auto-Hover).
    /// Mendukung DUA TANGAN. Logika berjalan otomatis saat laser menyentuh collider, TANPA perlu dicubit.
    /// Jika Scrollbar vertikal, putar GameObject Collider-nya 90 derajat.
    /// 
    /// [EN] Handles VR Scroll View (Scrollbar) interaction with Hand Tracking (Auto-Hover).
    /// Supports BOTH HANDS. Logic runs automatically when the laser hits the collider, NO pinch required.
    /// If Scrollbar is vertical, rotate the Collider GameObject 90 degrees.
    /// </summary>
    public class CurvedPhysicalUIScrollviewHandlerHT : MonoBehaviour
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

        [Header("Debug")]
        [SerializeField] private bool showDebug = false;

        private void Start()
        {
            if (scrollbarColliders.Length != canvasScrollbars.Length)
                Debug.LogError($"[PhysicalScrollviewHT] Mismatch! Colliders: {scrollbarColliders.Length}, Scrollbars: {canvasScrollbars.Length}.");
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

            // [ID] Cek tangan Kanan (Bisa menimpa input kiri jika mengenai scrollview lain)
            // [EN] Check Right hand (Can override left input if it hits another scrollview)
            if (rightRayInteractor != null && rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit rightHit))
            {
                ProcessHover(rightHit, rightRayInteractor.name);
            }
        }

        private void ProcessHover(RaycastHit hit, string interactorName)
        {
            // [ID] Cari apakah objek yang tersorot laser adalah salah satu collider scrollview kita
            // [EN] Check if the object hit by the laser is one of our scrollview colliders
            int index = Array.IndexOf(scrollbarColliders, hit.collider);

            // [ID] Jika kena, langsung update nilainya secara instan
            // [EN] If hit, update the value instantly
            if (index != -1 && index < canvasScrollbars.Length)
            {
                UpdateScrollviewValue(index, hit.point, interactorName);
            }
        }

        private void UpdateScrollviewValue(int index, Vector3 worldHitPoint, string interactorName)
        {
            BoxCollider col = scrollbarColliders[index];
            Scrollbar uiScrollbar = canvasScrollbars[index];

            if (col != null && uiScrollbar != null)
            {
                // [ID] 1. Konversi World ke Local (Otomatis menangani rotasi)
                // [EN] 1. Convert World to Local (Automatically handles rotation)
                Vector3 localHitPoint = col.transform.InverseTransformPoint(worldHitPoint);

                // [ID] 2. Hitung posisi relatif X (0.0 sampai 1.0)
                // [EN] 2. Calculate relative X position (0.0 to 1.0)
                float colliderWidth = col.size.x;
                float adjustedX = localHitPoint.x + (colliderWidth / 2f);
                float normalizedValue = Mathf.Clamp01(adjustedX / colliderWidth);

                // [ID] 3. Terapkan nilai ke Scrollbar secara instan
                // [EN] 3. Apply value to the Scrollbar instantly
                uiScrollbar.value = normalizedValue;

                if (showDebug)
                    Debug.Log($"[ScrollviewHT] Val: {normalizedValue:F2} via {interactorName} (Auto-Hover)");
            }
        }

        // ==========================================
        // GIZMOS
        // ==========================================
        private void OnDrawGizmosSelected()
        {
            if (scrollbarColliders == null || canvasScrollbars == null) return;

            Gizmos.color = Color.blue; // Warna Biru untuk Scrollview
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