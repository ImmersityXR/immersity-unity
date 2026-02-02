//#define TESTING_BEFORE_BUILDING

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;
using UnityEngine.XR;
using WebXR;

namespace Komodo.Runtime
{


    public class KomodoWebXRCamera : MonoBehaviour
    {
        public enum CameraID
        {
            Main,
            LeftVR,
            RightVR,
            LeftAR,
            RightAR
        }

        [FormerlySerializedAs("cameraMain")] public Camera cameraSpectator;
        public Camera cameraMainEditor, cameraL, cameraR, cameraARL, cameraARR;
        private WebXRState xrState = WebXRState.NORMAL;
        private Rect leftRect, rightRect;
        private int viewsCount = 1;

        void OnEnable()
        {

#if UNITY_WEBGL && !UNITY_EDITOR || TESTING_BEFORE_BUILDING
            WebXRManager.OnXRChange += onVRChange;
#else 
            WebXRManagerEditorSimulator.OnXRChange += onVRChange;
#endif

            WebXRManager.OnHeadsetUpdate += onHeadsetUpdate;
        }

        private void OnDisable()
        {

        }

        public void Start()
        {
            SetToDesktop();
        }
        
        public void OnDestroy()
        {
            WebXRManager.OnXRChange -= onVRChange;
            WebXRManager.OnHeadsetUpdate -= onHeadsetUpdate;
        }

        public Camera GetCamera(CameraID cameraID)
        {
            switch (cameraID)
            {
                case CameraID.LeftVR:
                    return cameraL;
                case CameraID.RightVR:
                    return cameraR;
                case CameraID.LeftAR:
                    return cameraARL;
                case CameraID.RightAR:
                    return cameraARR;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            return cameraSpectator;
#else 
            return cameraMainEditor;
#endif
        }

        private void onVRChange(WebXRState state, int viewsCount, Rect leftRect, Rect rightRect)
        {
            xrState = state;
            this.viewsCount = viewsCount;
            this.leftRect = leftRect;
            this.rightRect = rightRect;

            if (xrState == WebXRState.VR)
            {
                SetToVR(viewsCount, leftRect, rightRect);
            }
            else if (xrState == WebXRState.AR)
            {
                SetToAR(viewsCount, leftRect, rightRect);
            }
            else if (xrState == WebXRState.NORMAL)
            {
                SetToDesktop();
            }
        }

        private void SetToDesktop()
        {
            cameraMainEditor.gameObject.SetActive(false);
            cameraSpectator.gameObject.SetActive(true);

            cameraL.enabled = false;
            cameraR.enabled = false;

            cameraARL.enabled = false;
            cameraARR.enabled = false;
        }

        private void SetToAR(int viewsCount, Rect leftRect, Rect rightRect)
        {
            cameraSpectator.gameObject.SetActive(false);

            cameraL.enabled = false;
            cameraR.enabled = false;

            cameraARL.enabled = viewsCount > 0;
            cameraARL.rect = leftRect;
            cameraARR.enabled = viewsCount > 1;
            cameraARR.rect = rightRect;
        }

        private void SetToVR(int viewsCount, Rect leftRect, Rect rightRect)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
                //set complete camera gameobject to false to prevent update calls from freeflight controller
                cameraMainEditor.gameObject.SetActive(false);
                cameraSpectator.gameObject.SetActive(false);
#else 
            cameraMainEditor.gameObject.SetActive(true);
            cameraSpectator.gameObject.SetActive(false);
#endif

            cameraL.enabled = viewsCount > 0;
            cameraL.rect = leftRect;
            cameraR.enabled = viewsCount > 1;
            cameraR.rect = rightRect;

            cameraARL.enabled = false;
            cameraARR.enabled = false;
        }

        private void onHeadsetUpdate(
            Matrix4x4 leftProjectionMatrix,
            Matrix4x4 rightProjectionMatrix,
            Matrix4x4 leftViewMatrix,
            Matrix4x4 rightViewMatrix,
            Matrix4x4 sitStandMatrix)
        {
            if (xrState == WebXRState.VR)
            {
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraL.transform, leftViewMatrix * sitStandMatrix.inverse);

                cameraL.projectionMatrix = leftProjectionMatrix;

                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraR.transform, rightViewMatrix * sitStandMatrix.inverse);

                cameraR.projectionMatrix = rightProjectionMatrix;

            }
            else if (xrState == WebXRState.AR)
            {
                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraARL.transform, leftViewMatrix * sitStandMatrix.inverse);

                cameraARL.projectionMatrix = leftProjectionMatrix;

                WebXRMatrixUtil.SetTransformFromViewMatrix(cameraARR.transform, rightViewMatrix * sitStandMatrix.inverse);

                cameraARR.projectionMatrix = rightProjectionMatrix;
            }
        }
    }
}