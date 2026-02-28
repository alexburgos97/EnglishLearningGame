using UnityEngine;
using TMPro;
using SpatialSys.UnitySDK;

public class VolcanoRock : MonoBehaviour
{
    public TextMeshProUGUI pronounText;
    public Transform puntoLanzamiento;

    private Rigidbody rb;
    private bool isLaunched = false;
    private bool hasLanded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    public void Launch(string pronoun, Transform targetPoint)
    {
        if (targetPoint == null)
        {
            SpatialBridge.coreGUIService.DisplayToastMessage("ERROR: targetPoint null!");
            return;
        }

        hasLanded = false;
        isLaunched = true;
        pronounText.text = pronoun;
        gameObject.SetActive(true);

        transform.position = puntoLanzamiento.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float height = 15f;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float displacementY = targetPoint.position.y - puntoLanzamiento.position.y;
        Vector3 displacementXZ = new Vector3(
            targetPoint.position.x - puntoLanzamiento.position.x, 0,
            targetPoint.position.z - puntoLanzamiento.position.z);

        float time = Mathf.Sqrt(-2 * height / -gravity) +
            Mathf.Sqrt(2 * (displacementY - height) / -gravity);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * -gravity * height);
        Vector3 velocityXZ = displacementXZ / time;

        rb.velocity = velocityXZ + velocityY;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLaunched) return;
        if (hasLanded) return;

        hasLanded = true;
        isLaunched = false;
        gameObject.SetActive(false);
        VolcanoQuizManager.Instance.OnRockLanded();
    }
}