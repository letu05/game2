using UnityEngine;

public class SwingingLogo : MonoBehaviour
{
    public float speed = 2f;
    public float angle = 5f;

    void Update()
    {
        float rotationZ = Mathf.Sin(Time.time * speed) * angle;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}
