using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpatialUIController4 : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Transform kamera (VR Headset).")]
    [SerializeField] private Transform cameraTransform;

    // Kita tidak lagi menggunakan distanceX, Y, Z manual di inspector
    // melainkan menyimpannya secara internal.
    private Vector3 initialRelativeOffset;

    [Header("Axis Settings (Check to follow axis)")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;

    [Header("Movement Smoothness")]
    [Tooltip("Semakin besar nilai, semakin lambat/halus pergerakannya.")]
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Rotation")]
    [SerializeField] private bool alwaysFaceCamera = true;

    [Header("UI Reference")]
    [SerializeField] private GameObject ui;

    [Header("VR Input")]
    [SerializeField] private InputActionReference displaysUIInput;
    [SerializeField] private InputActionReference hideUIInput;
    [SerializeField] private InputActionReference recenterUIInput;

    private Vector3 velocity = Vector3.zero;

    private void OnEnable()
    {
        if (displaysUIInput != null) displaysUIInput.action.started += (ctx) => ToggleUI(true);
        if (hideUIInput != null) hideUIInput.action.started += (ctx) => ToggleUI(false);
        if (recenterUIInput != null) recenterUIInput.action.started += (ctx) => RecenterUI();
    }

    private void OnDisable()
    {
        // Unsubscribe untuk mencegah memory leak
        if (displaysUIInput != null) displaysUIInput.action.started -= (ctx) => ToggleUI(true);
        if (hideUIInput != null) hideUIInput.action.started -= (ctx) => ToggleUI(false);
        if (recenterUIInput != null) recenterUIInput.action.started -= (ctx) => RecenterUI();
    }

    private void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // [LOGIKA KRUSIAL]
        // Mengubah posisi dunia UI saat ini menjadi posisi 'lokal' terhadap kamera.
        // Ini merekam: "Berapa meter UI ini di depan/samping/atas kamera saat ini".
        initialRelativeOffset = cameraTransform.InverseTransformPoint(transform.position);

        UpdatePosition(true);
    }

    private void LateUpdate()
    {
        UpdatePosition(false);
    }

    void UpdatePosition(bool instant)
    {
        if (cameraTransform == null) return;

        // 1. Hitung di mana seharusnya UI berada berdasarkan posisi & rotasi kamera SEKARANG
        // menggunakan patokan offset yang kita ambil di Start tadi.
        Vector3 targetWorldPosition = cameraTransform.TransformPoint(initialRelativeOffset);

        // 2. Terapkan Axis Locking
        Vector3 currentPos = transform.position;
        Vector3 finalPosition = new Vector3(
            followX ? targetWorldPosition.x : currentPos.x,
            followY ? targetWorldPosition.y : currentPos.y,
            followZ ? targetWorldPosition.z : currentPos.z
        );

        // 3. Eksekusi Pergerakan (Smooth atau Instant)
        if (instant)
        {
            transform.position = finalPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothTime);
        }

        // 4. Perbaikan Rotasi (Mata ke Mata)
        if (alwaysFaceCamera)
        {
            // Menyamakan rotasi UI dengan rotasi kamera adalah cara paling stabil di VR
            // agar UI selalu tegak lurus di depan mata.
            transform.rotation = cameraTransform.rotation;
        }
    }

    private void ToggleUI(bool show)
    {
        if (ui != null) ui.SetActive(show);
        if (show) RecenterUI();
    }

    public void RecenterUI()
    {
        // Paksa UI kembali ke posisi offset awal di depan mata secara instan
        UpdatePosition(true);
    }
}