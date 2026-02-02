using UnityEditor;
using UnityEngine;

namespace Komodo.Runtime
{
    public class DevelopmentManager : MonoBehaviour
    {
        public string AppVersion => Application.version;

        public string UnityVersion => Application.unityVersion;

        void Awake()
        {
            Debug.Log("Application version " + AppVersion + " " + UnityVersion);
        }
    }
}