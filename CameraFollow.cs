using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTransform;
    public Vector2 xyOffset;

    public float smoothTime;
    private float interpPoint;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LerpTransform();
    }

    private void LerpTransform()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(followTransform.position.x + xyOffset.x, followTransform.position.y + xyOffset.y, this.transform.position.z), Time.deltaTime * smoothTime);
    }
}
