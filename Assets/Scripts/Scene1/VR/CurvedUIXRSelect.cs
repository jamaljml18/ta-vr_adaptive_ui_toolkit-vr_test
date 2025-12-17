using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CurvedUIXRSelect : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRRayInteractor xrRay;

    [Header("Curved UI")]
    [SerializeField] private GameObject curvedMeshObj;
    [SerializeField] private Canvas sourceCanvas;

    [Header("Debug")]
    [SerializeField] private GameObject debugCube;
    [SerializeField] private GameObject debugCube2;

    private GraphicRaycaster canvasRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;

    private void Awake()
    {
        canvasRaycaster = sourceCanvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    //private void OnEnable()
    //{
    //    // ✅ KLIK / TRIGGER
    //    xrRay.selectEntered.AddListener(OnSelectEntered);
    //}

    //private void OnDisable()
    //{
    //    xrRay.selectEntered.RemoveListener(OnSelectEntered);
    //}

    // =====================================================
    // SELECT ENTERED = KLIK
    // =====================================================
    //private void OnSelectEntered(SelectEnterEventArgs args)
    //{
    //    debugCube2.SetActive(false);
    //    RaycastHit hit;

    //    if (!xrRay.TryGetCurrent3DRaycastHit(out hit))
    //        return;

    //    if (hit.collider.gameObject != curvedMeshObj)
    //        return;

    //    Vector2 uv = hit.textureCoord;
    //    SimulateUIClick(uv);
    //}

    // =====================================================
    // MANUAL CALL FROM INSPECTOR (XR Select)
    // =====================================================
    public void OnXRSelectManual()
    {
        Debug.Log("🔥 OnXRSelectManual DIPANGGIL");
        Debug.Log("XR Select Manual Triggered");
        debugCube2.SetActive(false);

        RaycastHit hit;

        if (!xrRay.TryGetCurrent3DRaycastHit(out hit))
            return;

        if (hit.collider.gameObject != curvedMeshObj)
            return;

        Vector2 uv = hit.textureCoord;
        SimulateUIClick(uv);
    }

    // =====================================================
    // SIMULATE UI CLICK
    // =====================================================
    private void SimulateUIClick(Vector2 uv)
    {
        if (canvasRaycaster == null || eventSystem == null)
            return;

        pointerEventData = new PointerEventData(eventSystem);

        Rect pixelRect = sourceCanvas.pixelRect;

        pointerEventData.position = new Vector2(
            uv.x * pixelRect.width,
            uv.y * pixelRect.height
        );

        List<RaycastResult> results = new List<RaycastResult>();
        canvasRaycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.Invoke();
                Debug.Log("UI Clicked: " + result.gameObject.name);
                ToggleDebugCube();
                break;
            }
        }
    }

    private void ToggleDebugCube()
    {
        if (debugCube == null) return;
        debugCube.SetActive(!debugCube.activeSelf);
    }
}
