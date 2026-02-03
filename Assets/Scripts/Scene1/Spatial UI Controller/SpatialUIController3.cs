using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpatialUIController3 : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform cameraTransform;

    [Header("Distance Settings (Manual Offset)")]
    [Tooltip("Jika diisi 0, akan otomatis mengambil jarak saat ini terhadap kamera di Start.")]
    [SerializeField] private float overrideDistanceZ = 2.0f;

    private Vector3 relativeOffset; // Menyimpan posisi relatif awal

    [Header("Axis Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;

    [Header("Movement Smoothness")]
    [SerializeField] private float smoothTime = 0.1f;

    [Header("UI Reference")]
    [SerializeField] private GameObject ui;

    [Header("VR Input")]
    [SerializeField] private InputActionReference displaysUIInput;
    [SerializeField] private InputActionReference hideUIInput;
    [SerializeField] private InputActionReference recenterUIInput;

    private Vector3 velocity = Vector3.zero;
    private bool alwaysFaceCamera = true;

    private void OnEnable()
    {
        if (displaysUIInput != null) displaysUIInput.action.started += DisplaysUIInputPressed;
        if (hideUIInput != null) hideUIInput.action.started += HideUIInputPressed;
        if (recenterUIInput != null) recenterUIInput.action.started += RecenterUIInputPressed;
    }

    private void OnDisable()
    {
        if (displaysUIInput != null) displaysUIInput.action.started -= DisplaysUIInputPressed;
        if (hideUIInput != null) hideUIInput.action.started -= HideUIInputPressed;
        if (recenterUIInput != null) recenterUIInput.action.started -= RecenterUIInputPressed;
    }

    private void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // [PENTING] Menghitung posisi UI relatif terhadap 'ruang lokal' kamera
        // Ini memastikan UI tetap di posisi yang sama terhadap pandangan mata
        relativeOffset = cameraTransform.InverseTransformPoint(transform.position);

        // Jika Anda ingin memaksa jarak tertentu (misal 2 meter ke depan)
        if (overrideDistanceZ > 0)
        {
            relativeOffset = new Vector3(0, 0, overrideDistanceZ);
        }

        UpdatePosition(true);
    }

    private void LateUpdate()
    {
        UpdatePosition(false);
    }

    void UpdatePosition(bool instant)
    {
        if (cameraTransform == null) return;

        // 1. Hitung Target Position berdasarkan rotasi kamera saat ini
        // Kita mengubah offset relatif kembali ke koordinat dunia
        Vector3 targetPosition = cameraTransform.TransformPoint(relativeOffset);

        // 2. Axis Locking
        Vector3 currentPos = transform.position;
        Vector3 finalPosition = new Vector3(
            followX ? targetPosition.x : currentPos.x,
            followY ? targetPosition.y : currentPos.y,
            followZ ? targetPosition.z : currentPos.z
        );

        // 3. Smooth Movement
        if (instant)
            transform.position = finalPosition;
        else
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref velocity, smoothTime);

        // 4. Face Camera (Mata ke Mata)
        if (alwaysFaceCamera)
        {
            // Opsi A: UI sejajar sempurna dengan kemiringan kepala (Paling nyaman untuk HUD)
            transform.rotation = cameraTransform.rotation;

            /* // Opsi B: Gunakan ini jika ingin UI tegak lurus tapi tetap menghadap user
            Vector3 lookPos = cameraTransform.position - transform.position;
            lookPos.y = 0; // Kunci sumbu Y agar tidak miring-miring saat menoleh atas/bawah
            if (lookPos != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(-lookPos); 
            */
        }
    }

    private void DisplaysUIInputPressed(InputAction.CallbackContext ctx) => DisplayUI();
    private void HideUIInputPressed(InputAction.CallbackContext ctx) => HideUI();
    private void RecenterUIInputPressed(InputAction.CallbackContext ctx) => RecenterUI();

    public void DisplayUI()
    {
        if (ui != null) ui.SetActive(true);
        RecenterUI();
    }

    public void HideUI()
    {
        if (ui != null) ui.SetActive(false);
    }

    public void RecenterUI()
    {
        // Memaksa UI ke depan mata lagi
        UpdatePosition(true);
    }
}