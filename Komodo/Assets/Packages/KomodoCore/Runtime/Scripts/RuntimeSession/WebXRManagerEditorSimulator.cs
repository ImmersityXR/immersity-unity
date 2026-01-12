//#define TESTING_BEFORE_BUILDING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using WebXR;

public class WebXRManagerEditorSimulator : MonoBehaviour
{
    public delegate void XRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect);

    public static event XRChange OnXRChange;

    public bool showEnterVRButton;
    [SerializeField] private Button enterVRButton;
    private bool isVRButtonActivated;
    private bool previousIsVRActive = false;
    private bool isInitialized;

#if UNITY_WEBGL && !UNITY_EDITOR || TESTING_BEFORE_BUILDING
//do nothing
#else

    void Start()
    {
        enterVRButton = GameObject.FindGameObjectWithTag("EnterVRButton").GetComponent<Button>();
        enterVRButton.onClick.AddListener(() =>
        {
            isVRButtonActivated = !isVRButtonActivated;
            if (isVRButtonActivated)
            {
                EnterVR();
            }
            else
            {
                ExitVR();
            }
        });
    }
    
    void Update () {
        bool isVRActive = showEnterVRButton 
                            ? isVRButtonActivated 
                            : XRSettings.isDeviceActive; //tells us whether the device is attached (not necessarily if it is being worn or used.)

        if (isVRActive && (!previousIsVRActive || !isInitialized))
        {
            EnterVR();
            isInitialized = true;
        }

        if (!isVRActive && (previousIsVRActive || !isInitialized))
        {
            ExitVR();
            isInitialized = true;
        }
        
        previousIsVRActive = isVRActive;
    }

    [ContextMenu("Enter VR")]
    private void EnterVR()
    {
        OnXRChange?.Invoke(WebXRState.VR, 2, new Rect(), new Rect());
        enterVRButton.GetComponentInChildren<Text>().text = "Exit VR";
    }

    [ContextMenu("Exit VR")]
    private void ExitVR()
    {
        OnXRChange?.Invoke(WebXRState.NORMAL, 1, new Rect(), new Rect());
        enterVRButton.GetComponentInChildren<Text>().text = "Enter VR";
    }
#endif
}