
using UnityEngine;

public class LookAndStretch : MonoBehaviour
{
    public Transform t2;
    private void Update()
    {
        var dist = Vector3.Distance(transform.position, t2.position) * 0.25f;
        var transform1 = transform;
        var transformLocalScale = transform1.localScale;
        transformLocalScale.x = dist;
        transform1.localScale = transformLocalScale;
    }
}
