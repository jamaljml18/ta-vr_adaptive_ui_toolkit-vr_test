using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpatialUIController : MonoBehaviour
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

    // [Header("Rotation Settings (Pengaturan Rotasi)")]
    // [Tooltip("Jika aktif, UI akan selalu menghadap kamera.\nIf enabled, the UI will always face the camera.")]
    private bool alwaysFaceCamera;

    [Header("UI")]
    [Tooltip("UI yang akan ditampilkan atau disembunyikan. \nUI that will be shown or hidden.")]
    [SerializeField] private GameObject ui;

    [Header("VR Input")]
    [Tooltip("Opsional, dapat dikosongi jika followX, followY, atau followZ tidak kosong, tombol akan digunakan untuk menampilkan ui. " +
        "\nOptionally, can be left blank if followX, followY, or followZ are not blank, the button will be used to display the ui.")]
    [SerializeField] private InputActionReference displaysUIInput;

    [Tooltip("Opsional, dapat dikosongi jika followX, followY, atau followZ tidak kosong, tombol akan digunakan untuk menyembunyikan ui. " +
        "\nOptionally, can be left blank if followX, followY, or followZ are not blank, the button will be used to hide the ui.")]
    [SerializeField] private InputActionReference hideUIInput;

    [Tooltip("Opsional, dapat dikosongi, tombol akan digunakan untuk memastikan UI berada di depan tampilan pengguna. \n" +
        "Optional, can be left blank, button will be used to ensure the UI is in front of the user's view.")]
    [SerializeField] private InputActionReference recenterUIInput;

    [SerializeField] private InputActionReference debugInput;


    // [ID] Velocity digunakan untuk SmoothDamp
    // [EN] Velocity used internally by SmoothDamp
    private Vector3 velocity = Vector3.zero;

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        displaysUIInput.action.started += DisplaysUIInputPressed;
        hideUIInput.action.started += HideUIInputPressed;
        recenterUIInput.action.started += RecenterUIInputPressed;
        debugInput.action.started += DebugInputPressed;

    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listener to avoid memory leaks
        displaysUIInput.action.started -= DisplaysUIInputPressed;
        hideUIInput.action.started -= HideUIInputPressed;
        recenterUIInput.action.started -= RecenterUIInputPressed;
        debugInput.action.started -= DebugInputPressed;

    }

    //private void Start()
    //{
    //    // [ID] Jika kamera tidak diassign, gunakan Main Camera
    //    // [EN] If cameraTransform is not assigned, automatically use Main Camera
    //    if (cameraTransform == null)
    //    {
    //        cameraTransform = Camera.main.transform;
    //    }
    //    // [ID] Simpan offset awal UI berdasarkan posisi dunia (world position).
    //    // [EN] This stores the initial UI offset based on its current world position.
    //    distanceX = transform.position.x;
    //    distanceY = transform.position.y;
    //    distanceZ = transform.position.z;
    //    // [ID] Set posisi awal secara instan agar tidak melompat dari jauh
    //    // [EN] Set initial position instantly to avoid visual popping
    //    UpdatePosition(true);
    //}

    private void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // [PENTING] Ini adalah cara melakukan 'pengurangan' yang benar:
        // Menghitung posisi UI relatif terhadap kamera (Local Offset).
        Vector3 relativeVector = cameraTransform.InverseTransformPoint(transform.position);

        // Sekarang kita simpan hasil 'pengurangan' tersebut ke variabel distance
        distanceX = relativeVector.x;
        distanceY = relativeVector.y;
        distanceZ = relativeVector.z;

        UpdatePosition(true);
    }

    private void DisplaysUIInputPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat displaysUIInput ditekan
        // [EN] Called when displaysUIInput is pressed

        Debug.Log("DisplaysUIInputPressed");

        // [ID] Tampilkan UI
        // [EN] Show the UI
        DisplayUI();
    }

    private void HideUIInputPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat hideUIInput ditekan
        // [EN] Called when hideUIInput is pressed

        Debug.Log("HideUIInputPressed");

        // [ID] Sembunyikan UI
        // [EN] Hide the UI
        HideUI();
    }

    private void RecenterUIInputPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat recenterUIInput ditekan
        // [EN] Called when recenterUIInput is pressed

        Debug.Log("RecenterUIInputPressed");

        // [ID] Mencenterkan UI di depan pengguna
        // [EN] Centering the UI in front of the user
        RecenterUI();
    }

    private void DebugInputPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat DebugInputPressed ditekan
        // [EN] Called when DebugInputPressed is pressed

        Debug.Log("VR INPUT TRIGGERED! Change");

        // [ID] Mengubah followX, followY, dan followZ
        // [EN] Change followX, followY, and followZ
        ChangeFollowXYZ();
    }
    private void DisplayUI()
    {
        // [ID] Jika UI sedang mengikuti posisi kamera pada sumbu tertentu,
        //      maka fungsi ini tidak dijalankan untuk mencegah konflik transform
        // [EN] If the UI is following the camera on certain axes,
        //      abort this function to avoid transform conflicts
        if (followX || followY || followZ)
        {
            return;
        }

        // [ID] Aktifkan UI jika saat ini tidak aktif
        // [EN] Activate the UI if it is currently inactive
        if (ui.gameObject.activeSelf == false)
        {
            ui.gameObject.SetActive(true);
        }

        // [ID] Paksa UI untuk menghadap kamera dan mengikuti posisi kamera
        // [EN] Force UI to face the camera and follow camera position
        alwaysFaceCamera = true;
        followX = true;
        followY = true;
        followZ = true;

        // [ID] Snap posisi secara instan ke depan kamera
        // [EN] Instantly snap position to the front of the camera
        UpdatePosition(true);

        // [ID] Mulai coroutine untuk melepas kuncian posisi setelah jeda
        // [EN] Start coroutine to release position lock after a delay
        StartCoroutine(BackToFrontCamera());
    }

    private void RecenterUI()
    {
        // [ID] Paksa UI untuk menghadap kamera dan mengikuti posisi kamera
        // [EN] Force UI to face the camera and follow camera position
        alwaysFaceCamera = true;
        followX = true;
        followY = true;
        followZ = true;

        // [ID] Snap posisi secara instan ke depan kamera
        // [EN] Instantly snap position to the front of the camera
        UpdatePosition(true);
    }

    private IEnumerator BackToFrontCamera()
    {
        // [ID] Tunggu sebentar sebelum melepas UI agar diam di tempat
        // [EN] Wait briefly before releasing the UI to stay in place
        yield return new WaitForSeconds(.5f);

        print("Follow X Y Z kembali false / released");

        // [ID] Nonaktifkan fitur mengikuti kamera agar UI menjadi statis di dunia
        // [EN] Disable camera following so the UI becomes static in the world
        alwaysFaceCamera = false;
        followX = false;
        followY = false;
        followZ = false;
    }

    private void HideUI()
    {
        // [ID] Jika UI sedang mengikuti posisi kamera pada sumbu tertentu,
        //      maka fungsi ini tidak dijalankan untuk mencegah konflik transform
        // [EN] If the UI is following the camera on certain axes,
        //      abort this function to avoid transform conflicts
        if (followX || followY || followZ)
        {
            return;
        }

        // [ID] Nonaktifkan UI jika saat ini masih aktif
        // [EN] Deactivate the UI if it is currently active
        if (ui.gameObject.activeSelf == true)
        {
            ui.gameObject.SetActive(false);
        }
    }

    // [ID] Fungsi ChangeFollowXYZ() hanya digunakan untuk melakukan debugging.
    // [EN] The ChangeFollowXYZ() function is only used for debugging.
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
