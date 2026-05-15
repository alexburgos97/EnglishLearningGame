using UnityEngine;
using SpatialSys.UnitySDK;

public class RecursosAdicionales : MonoBehaviour
{
    public string resourceURL;
    public string resourceName;

    public void OpenResource()
    {
        SpatialBridge.coreGUIService.DisplayToastMessage(
            "Opening: " + resourceName);
        SpatialBridge.spaceService.OpenURL(resourceURL);
    }
}