using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform player; //타겟
    public float smoothSpeed = 0.125f;//속도 보간
    public Vector3 offset; //카메라와 플레이어 사이의 거리

    void Update()
    {
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x,0, offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(new Vector3(transform.position.x,0,transform.position.z), new Vector3(desiredPosition.x,0,desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
