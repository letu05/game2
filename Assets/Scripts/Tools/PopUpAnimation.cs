using UnityEngine;

public class PopUpAnimation : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 _targetScale = Vector3.one;

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * speed);
    }
}
