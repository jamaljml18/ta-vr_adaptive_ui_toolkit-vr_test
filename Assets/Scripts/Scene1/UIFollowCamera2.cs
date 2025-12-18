using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIFollowCamera2 : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Transform kamera yang akan diikuti oleh UI.\nCamera transform that the UI will follow.")]
    private Transform cameraTransform;

    [Header("Distance Settings (Offset dari Kamera / Offset from Camera)")]
    [Tooltip("Offset horizontal relatif terhadap arah kanan kamera.\nHorizontal offset relative to camera's right direction.")]
    private float distanceX;

    [Tooltip("Offset vertikal relatif terhadap arah atas kamera.\nVertical offset relative to camera's up direction.")]
    private float distanceY;

    [Tooltip("Offset ke depan relatif terhadap arah depan kamera.\nForward offset relative to camera's forward direction.")]
    private float distanceZ;

    [Header("Axis Settings (Check to follow axis / Centang untuk mengikuti sumbu)")]
    [Tooltip("Jika aktif, UI mengikuti posisi kamera pada sumbu X.\nIf enabled, UI follows the camera's X-axis movement.")]
    [SerializeField] private bool followX;

    [Tooltip("Jika aktif, UI mengikuti posisi kamera pada sumbu Y.\nIf enabled, UI follows the camera's Y-axis movement.")]
    [SerializeField] private bool followY;

    [Tooltip("Jika aktif, UI mengikuti posisi kamera pada sumbu Z.\nIf enabled, UI follows the camera's Z-axis movement.")]
    [SerializeField] private bool followZ;

    [Header("Movement Smoothness (Kelembutan Gerakan)")]
    [Tooltip("Semakin besar nilai, semakin lambat/halus pergerakannya (efek lag).\nHigher values result in slower and smoother movement (lag effect).")]
    [SerializeField] private float smoothTime;

    [Header("Rotation Settings (Pengaturan Rotasi)")]
    [Tooltip("Jika aktif, UI akan selalu menghadap kamera.\nIf enabled, the UI will always face the camera.")]
    [SerializeField] private bool alwaysFaceCamera;

    [Header("Curved Mesh")]
    [SerializeField] private GameObject curvedMesh;
    // [ID] GameObject UI melengkung (Curved UI) yang akan ditampilkan atau disembunyikan
    // [EN] Curved UI GameObject that will be shown or hidden

    [Header("VR Input")]
    [SerializeField] private InputActionReference vrInput; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                           // [EN] VR Input used to detect click/trigger events inside VR
    [SerializeField] private InputActionReference vrInput2; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                            // [EN] VR Input used to detect click/trigger events inside VR
    [SerializeField] private InputActionReference vrInput3; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                            // [EN] VR Input used to detect click/trigger events inside VR

    // [ID] Velocity digunakan untuk SmoothDamp
    // [EN] Velocity used internally by SmoothDamp
    private Vector3 velocity = Vector3.zero;

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        if (vrInput != null)
            vrInput.action.started += OnVRPressed;
        vrInput2.action.started += OnVRPressed2;
        vrInput3.action.started += OnVRPressed3;

    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listener to avoid memory leaks
        if (vrInput != null)
            vrInput.action.started -= OnVRPressed;
        vrInput2.action.started -= OnVRPressed2;
        vrInput3.action.started -= OnVRPressed3;

    }

    private void Start()
    {
        // [ID] Jika kamera tidak diassign, gunakan Main Camera
        // [EN] If cameraTransform is not assigned, automatically use Main Camera
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        // [ID] Simpan offset awal UI berdasarkan posisi dunia (world position).
        // [EN] This stores the initial UI offset based on its current world position.
        distanceX = transform.position.x;
        distanceY = transform.position.y;
        distanceZ = transform.position.z;
        // [ID] Set posisi awal secara instan agar tidak melompat dari jauh
        // [EN] Set initial position instantly to avoid visual popping
        UpdatePosition(true);
    }

    private void OnVRPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat input VR utama (misalnya trigger) ditekan
        // [EN] Called when the primary VR input (e.g., trigger) is pressed

        Debug.Log("VR INPUT TRIGGERED! Find");

        // [ID] Tampilkan UI melengkung
        // [EN] Show the curved UI
        FindUI();
    }

    private void OnVRPressed2(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat input VR sekunder ditekan
        // [EN] Called when the secondary VR input is pressed

        Debug.Log("VR INPUT TRIGGERED! Hidden");

        // [ID] Sembunyikan UI melengkung
        // [EN] Hide the curved UI
        HiddenUI();
    }

    private void OnVRPressed3(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat input VR utama (misalnya trigger) ditekan
        // [EN] Called when the primary VR input (e.g., trigger) is pressed

        Debug.Log("VR INPUT TRIGGERED! Change");

        // [ID] Mengubah followX, followY, dan followZ
        // [EN] Change followX, followY, and followZ
        ChangeFollowXYZ();
    }
    private void FindUI()
    {
        // [ID] Jika UI sedang mengikuti posisi kamera pada sumbu tertentu,
        //      maka fungsi ini tidak dijalankan untuk mencegah konflik transform
        // [EN] If the UI is following the camera on certain axes,
        //      abort this function to avoid transform conflicts
        if (followX || followY || followZ)
        {
            return;
        }

        // [ID] Aktifkan Curved UI jika saat ini tidak aktif
        // [EN] Activate the curved UI if it is currently inactive
        if (curvedMesh.gameObject.activeSelf == false)
        {
            curvedMesh.gameObject.SetActive(true);
        }

        alwaysFaceCamera = true;
        followX = true;
        followY = true;
        followZ = true;
        UpdatePosition(true);
        StartCoroutine(BackToFrontCamera());
    }

    private IEnumerator BackToFrontCamera() { 
        yield return new WaitForSeconds(.5f);
        print("Follow X Y Z kembali false");
        alwaysFaceCamera = false;
        followX = false;
        followY = false;
        followZ = false;
    }

    private void HiddenUI()
    {
        // [ID] Jika UI sedang mengikuti posisi kamera pada sumbu tertentu,
        //      maka fungsi ini tidak dijalankan untuk mencegah konflik transform
        // [EN] If the UI is following the camera on certain axes,
        //      abort this function to avoid transform conflicts
        if (followX || followY || followZ)
        {
            return;
        }

        // [ID] Nonaktifkan Curved UI jika saat ini masih aktif
        // [EN] Deactivate the curved UI if it is currently active
        if (curvedMesh.gameObject.activeSelf == true)
        {
            curvedMesh.gameObject.SetActive(false);
        }
    }

    private void ChangeFollowXYZ()
    {
        alwaysFaceCamera = !alwaysFaceCamera;
        followX = !followX;
        followY = !followY;
        followZ = !followZ;
    }

    private void LateUpdate()
    {
        // [ID] Update posisi setiap frame setelah semua object bergerak
        // [EN] Update the position after all scene objects have moved
        UpdatePosition(false);
    }


    /// <summary>
    /// [ID] Memperbarui posisi UI berdasarkan posisi kamera dan offset.
    /// [EN] Update UI position based on camera movement and offsets.
    /// </summary>
    /// <param name="instant">
    /// [ID] Jika true, memindahkan UI secara langsung tanpa smoothing.
    /// [EN] If true, moves the UI instantly without smoothing.
    /// </param>
    void UpdatePosition(bool instant)
    {
        if (cameraTransform == null) return;

        // =====================================================
        // 1. [ID] Hitung target position dengan distance XYZ
        //    [EN] Calculate target position based on XYZ offsets
        // =====================================================

        // [ID] Arah orientasi kamera
        // [EN] Camera orientation vectors
        Vector3 cameraRight = cameraTransform.right;
        Vector3 cameraUp = cameraTransform.up;
        Vector3 cameraForward = cameraTransform.forward;

        // [ID] Posisi target adalah posisi kamera + offset pada 3 arah
        // [EN] Target position = camera position + offsets along three axes
        Vector3 targetPosition =
            cameraTransform.position +
            (cameraRight * distanceX) +
            (cameraUp * distanceY) +
            (cameraForward * distanceZ);


        // =====================================================
        // 2. [ID] Axis Locking: menentukan sumbu mana yang mengikuti
        //    [EN] Axis Locking: determine which axes should follow
        // =====================================================

        Vector3 finalPosition = transform.position;

        if (followX) finalPosition.x = targetPosition.x;
        if (followY) finalPosition.y = targetPosition.y;
        if (followZ) finalPosition.z = targetPosition.z;

        // =====================================================
        // 3. [ID] Smooth Movement (Interpolasi halus)
        //    [EN] Smooth Movement (Soft interpolation)
        // =====================================================

        if (instant)
        {
            // [ID] Pindah langsung tanpa smoothing
            // [EN] Move instantly without smoothing
            transform.position = finalPosition;
        }
        else
        {
            // [ID] SmoothDamp membuat gerakan UI lebih natural
            // [EN] SmoothDamp results in natural, dampened movement
            transform.position = Vector3.SmoothDamp(
                transform.position,
                finalPosition,
                ref velocity,
                smoothTime
            );
        }


        // =====================================================
        // 4. [ID] Rotasi agar UI selalu menghadap kamera
        //    [EN] Rotation: make UI always face the camera
        // =====================================================

        if (alwaysFaceCamera)
        {
            // [ID] LookAt dengan orientasi kamera memastikan UI menghadap pengguna
            // [EN] LookAt using camera rotation ensures UI faces the user correctly
            transform.LookAt(
                transform.position + cameraTransform.rotation * Vector3.forward,
                cameraTransform.rotation * Vector3.up
            );
        }
    }
}
