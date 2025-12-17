using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CurvedUIVRButtonHandler : MonoBehaviour
{
    [Header("XR References")]
    [SerializeField] private XRRayInteractor xrRay;
    // [ID] Ray interactor dari controller VR
    // [EN] XR Ray Interactor from VR controller

    [SerializeField] private InputActionReference triggerInput;
    // [ID] Input trigger controller VR
    // [EN] VR controller trigger input

    [Header("Curved UI References")]
    [SerializeField] private GameObject curvedMeshObj;
    // [ID] Mesh 3D curved UI (HARUS ada MeshCollider)
    // [EN] Curved 3D UI mesh (MUST have MeshCollider)

    [SerializeField] private Canvas sourceCanvas;
    // [ID] Canvas asli yang dirender ke RenderTexture
    // [EN] Original Canvas rendered to RenderTexture

    [Header("Layer Mask")]
    [SerializeField] private LayerMask uiLayerMask;
    // [ID] Layer khusus Curved UI
    // [EN] Layer for Curved UI

    [Header("Debug")]
    [SerializeField] private GameObject cube;

    // Internal
    private GraphicRaycaster canvasRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    private void OnEnable()
    {
        if (triggerInput != null)
            triggerInput.action.started += OnTriggerPressed;
    }

    private void OnDisable()
    {
        if (triggerInput != null)
            triggerInput.action.started -= OnTriggerPressed;
    }

    private void Start()
    {
        // Cache GraphicRaycaster
        canvasRaycaster = sourceCanvas.GetComponent<GraphicRaycaster>();

        if (canvasRaycaster == null)
        {
            Debug.LogError("GraphicRaycaster tidak ditemukan di Canvas!");
        }

        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            Debug.LogError("EventSystem tidak ditemukan di scene!");
        }
    }

    private void Update()
    {
        Ray ray = new Ray(xrRay.transform.position, xrRay.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.green);
    }


    // =====================================================
    // VR TRIGGER EVENT
    // =====================================================
    private void OnTriggerPressed(InputAction.CallbackContext ctx)
    {
        HandleXRClick();
    }

    // =====================================================
    // HANDLE XR RAY CLICK
    // =====================================================
    private void HandleXRClick()
    {
        if (xrRay == null) return;

        RaycastHit hit;

        // Ambil hasil raycast dari XR Ray Interactor
        if (xrRay.TryGetCurrent3DRaycastHit(out hit))
        {
            // Pastikan kena mesh curved UI
            if (hit.collider.gameObject == curvedMeshObj)
            {
                Vector2 uv = hit.textureCoord;
                SimulateUIClick(uv);
            }
        }
    }

    // =====================================================
    // SIMULATE UI CLICK
    // =====================================================
    private void SimulateUIClick(Vector2 uv)
    {
        if (canvasRaycaster == null || eventSystem == null) return;

        pointerEventData = new PointerEventData(eventSystem);

        Rect pixelRect = sourceCanvas.pixelRect;

        // Konversi UV ke posisi pixel Canvas
        pointerEventData.position = new Vector2(
            uv.x * pixelRect.width,
            uv.y * pixelRect.height
        );

        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            // BUTTON
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.Invoke();
                Debug.Log("Clicked UI Button: " + result.gameObject.name);
                DebugWithCube();
                break;
            }

            // TOGGLE (opsional)
            Toggle toggle = result.gameObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = !toggle.isOn;
                break;
            }
        }
    }

    private void DebugWithCube()
    {
        if (cube.gameObject.activeSelf == true) cube.gameObject.SetActive(false);
        if (cube.gameObject.activeSelf == false) cube.gameObject.SetActive(true);
    }
}