// #define TESTING_BEFORE_BUILDING

using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using WebXR;

public class WebXRManagerEditorSimulator : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR || TESTING_BEFORE_BUILDING
//do nothing
#else
    public delegate void XRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect);

    public static event XRChange OnXRChange;
    
    [Tooltip("Manually control VR session, to mimic how the WebGL Template provides a VR button. Disables WebXR Manager if enabled, and vice-versa.")]
    public bool showEnterVRButton;
    public Button enterVRButton;
    private bool isVRButtonActivated;
    private bool previousIsVRActive = false;
    private bool isInitialized;

    public void OnValidate()
    { 
        GetComponent<WebXRManager>().enabled = !showEnterVRButton;
    }

    void Start()
    {
        if (!showEnterVRButton)
        {
            StartCoroutine(EnterVRCoroutine());
            return;
        }
        
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

        ExitVR();
    }
    
    void Update () {
        bool isVRActive = showEnterVRButton 
                            ? isVRButtonActivated 
                            : XRSettings.isDeviceActive; //tells us whether the device is attached (not necessarily if it is being worn or used.)

        if (isVRActive && (!previousIsVRActive || !isInitialized))
        {
            isInitialized = true;
            EnterVR();
        }

        if (!isVRActive && (previousIsVRActive || !isInitialized))
        {
            isInitialized = true;
            ExitVR();
        }
        
        previousIsVRActive = isVRActive;
    }

    private IEnumerator EnterVRCoroutine()
    {
        if (XRGeneralSettings.Instance.Manager 
            && XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            yield break;
        }
        
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            
        if (!XRGeneralSettings.Instance.Manager.activeLoader) 
        {
            Debug.Log("No XR Loader was selected.");
        }
        else
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems(); 
        }
    }


    [ContextMenu("Enter VR")]
    private void EnterVR()
    {
        if (showEnterVRButton)
        {
            StartCoroutine(EnterVRCoroutine());
        }
        OnXRChange?.Invoke(WebXRState.VR, 2, new Rect(), new Rect());
        enterVRButton.GetComponentInChildren<Text>().text = "Exit VR";
    }

    private IEnumerator ExitVRCoroutine()
    {
        yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager.isInitializationComplete);
        if (XRGeneralSettings.Instance.Manager
            && XRGeneralSettings.Instance.Manager.activeLoader)
        {
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    [ContextMenu("Exit VR")]
    private void ExitVR()
    {
        if (showEnterVRButton)
        {
            if (XRGeneralSettings.Instance.Manager 
                && XRGeneralSettings.Instance.Manager.activeLoader)
            {
                StartCoroutine(ExitVRCoroutine());
            }
        }
        OnXRChange?.Invoke(WebXRState.NORMAL, 1, new Rect(), new Rect());
        enterVRButton.GetComponentInChildren<Text>().text = "Enter VR";
    }

    public void OnApplicationQuit() 
    {
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
    }
#endif
}