using Komodo.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class VersionTextController : MonoBehaviour
{
    [SerializeField]
    private DevelopmentManager developmentManager;
    [SerializeField]
    private Text textObject;
    
    void Start()
    {
        textObject.text = "v" + developmentManager.AppVersion;
    }
}
