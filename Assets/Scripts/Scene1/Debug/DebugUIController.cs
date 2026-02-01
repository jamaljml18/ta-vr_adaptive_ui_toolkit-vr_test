using UnityEngine;
using UnityEngine.InputSystem;

public class DebugUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject curved_ui_button_panel;
    [SerializeField] private GameObject curved_ui_image_panel;
    [SerializeField] private GameObject curved_ui_toggle_panel;
    [SerializeField] private GameObject curved_ui_scrollview_panel;
    [SerializeField] private GameObject curved_ui_slider_panel;
    [SerializeField] private GameObject curved_ui_scrollbar_panel;
    [SerializeField] private GameObject curved_ui_dropdown_panel;
    // Hapus
    [SerializeField] private GameObject curved_ui_scrollview2_panel;

    [Header("VR Input")]
    [SerializeField] private InputActionReference vrInput; // [ID] Input VR yang akan dipakai untuk mendeteksi klik/trigger dalam VR
                                                           // [EN] VR Input used to detect click/trigger events inside VR

    [Header("Button, Toggle, Slider")]
    [SerializeField] private GameObject buttonA;
    [SerializeField] private GameObject buttonB;
    [SerializeField] private GameObject toggleA;
    [SerializeField] private GameObject toggleB;
    [SerializeField] private GameObject sliderA;
    [SerializeField] private GameObject sliderB;
    [SerializeField] private GameObject sliderC;
    [SerializeField] private GameObject sliderD;
    [SerializeField] private GameObject scrollbarA;
    [SerializeField] private GameObject scrollbarB;
    [SerializeField] private GameObject scrollbarC;
    [SerializeField] private GameObject scrollbarD;
    [SerializeField] private GameObject scrollviewA;
    [SerializeField] private GameObject scrollviewB;
    [SerializeField] private GameObject scrollviewC;
    [SerializeField] private GameObject scrollviewD;
    // Hapus
    [SerializeField] private GameObject scrollviewA2;
    [SerializeField] private GameObject scrollviewB2;

    private int index = 1;

    private void OnEnable()
    {
        // [ID] Aktifkan listener input VR ketika object aktif
        // [EN] Enable VR input listener when this object becomes active
        if (vrInput != null)
            vrInput.action.started += OnVRPressed;

    }

    private void OnDisable()
    {
        // [ID] Lepaskan listener VR untuk mencegah memory leak
        // [EN] Remove VR listener to avoid memory leaks
        if (vrInput != null)
            vrInput.action.started -= OnVRPressed;

    }

    private void Start()
    {
        curved_ui_button_panel.SetActive(true);
        curved_ui_image_panel.SetActive(false);
        curved_ui_toggle_panel.SetActive(false);
        curved_ui_scrollview_panel.SetActive(false);
        curved_ui_slider_panel.SetActive(false);
        curved_ui_scrollbar_panel.SetActive(false);
        curved_ui_dropdown_panel.SetActive(false);
        // Hapus
        curved_ui_scrollview2_panel.SetActive(false);

        buttonA.SetActive(true);
        buttonB.SetActive(true);
        toggleA.SetActive(false);
        toggleB.SetActive(false);
        sliderA.SetActive(false);
        sliderB.SetActive(false);
        sliderC.SetActive(false);
        sliderD.SetActive(false);
        scrollbarA.SetActive(false);
        scrollbarB.SetActive(false);
        scrollbarC.SetActive(false);
        scrollbarD.SetActive(false);
        scrollviewA.SetActive(false);
        scrollviewB.SetActive(false);
        scrollviewC.SetActive(false);
        scrollviewD.SetActive(false);
        // Hapus
        scrollviewA2.SetActive(false);
        scrollviewB2.SetActive(false);
    }

    private void OnVRPressed(InputAction.CallbackContext ctx)
    {
        // [ID] Dipanggil saat input VR utama (misalnya trigger) ditekan
        // [EN] Called when the primary VR input (e.g., trigger) is pressed

        Debug.Log("VR INPUT TRIGGERED! Find");

        // [ID] Tampilkan UI melengkung
        // [EN] Show the curved UI
        ShowUI();
    }

    private void ShowUI()
    {
        index++;
        //if (index > 8) index = 1;
        if (index > 9) index = 1;

        // Nonaktifkan semua panel dulu
        curved_ui_button_panel.SetActive(false);
        curved_ui_image_panel.SetActive(false);
        curved_ui_toggle_panel.SetActive(false);
        curved_ui_scrollview_panel.SetActive(false);
        curved_ui_slider_panel.SetActive(false);
        curved_ui_scrollbar_panel.SetActive(false);
        curved_ui_dropdown_panel.SetActive(false);
        // Hapus
        curved_ui_scrollview2_panel.SetActive(false);

        switch (index)
        {
            case 1:
                buttonA.SetActive(true);
                buttonB.SetActive(true);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_button_panel.SetActive(true);
                Debug.Log("Show Button Panel");
                break;

            case 2:
                curved_ui_image_panel.SetActive(true);
                Debug.Log("Show Image Panel");
                break;

            case 3:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(true);
                toggleB.SetActive(true);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_toggle_panel.SetActive(true);
                Debug.Log("Show Toggle Panel");
                break;

            case 4:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(true);
                scrollviewB.SetActive(true);
                scrollviewC.SetActive(true);
                //scrollviewD.SetActive(true);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_scrollview_panel.SetActive(true);
                Debug.Log("Show ScrollView Panel");
                break;

            case 5:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(true);
                sliderB.SetActive(true);
                sliderC.SetActive(true);
                sliderD.SetActive(true);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_slider_panel.SetActive(true);
                Debug.Log("Show Slider Panel");
                break;

            case 6:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(true);
                scrollbarB.SetActive(true);
                scrollbarC.SetActive(true);
                scrollbarD.SetActive(true);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_scrollbar_panel.SetActive(true);
                Debug.Log("Show Scrollbar Panel");
                break;

            case 7:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(false);
                scrollviewB2.SetActive(false);

                curved_ui_dropdown_panel.SetActive(true);
                Debug.Log("Show Dropdown Panel");
                break;

            case 8:
                buttonA.SetActive(false);
                buttonB.SetActive(false);
                toggleA.SetActive(false);
                toggleB.SetActive(false);
                sliderA.SetActive(false);
                sliderB.SetActive(false);
                sliderC.SetActive(false);
                sliderD.SetActive(false);
                scrollbarA.SetActive(false);
                scrollbarB.SetActive(false);
                scrollbarC.SetActive(false);
                scrollbarD.SetActive(false);
                //scrollviewA.SetActive(false);
                scrollviewB.SetActive(false);
                scrollviewC.SetActive(false);
                //scrollviewD.SetActive(false);
                // Hapus
                scrollviewA2.SetActive(true);
                scrollviewB2.SetActive(true);

                curved_ui_scrollview2_panel.SetActive(true);
                Debug.Log("Show Scrollbar Panel");
                break;
        }
    }

}
