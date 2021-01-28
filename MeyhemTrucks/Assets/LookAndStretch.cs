
using UnityEngine;

public class LookAndStretch : MonoBehaviour
{

    public SpringJoint2D spJ;
    public Transform t2;
    private void Update()
    {
        var dist = Vector3.Distance(transform.position, t2.position);
        var transform1 = transform;
        var transformLocalScale = transform1.localScale;
        transformLocalScale.x = dist;
        transform1.localScale = transformLocalScale;
    }
}
