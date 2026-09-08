using UnityEngine;


public class RecursosAdicionales : MonoBehaviour
{
    public string resourceURL;
    public string resourceName;

    public void OpenResource()
    {
        Debug.Log(
            "Opening: " + resourceName);
        Application.OpenURL(resourceURL);
    }
}