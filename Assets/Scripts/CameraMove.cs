using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform player; //Ÿ��
    public float smoothSpeed = 0.125f;//�ӵ� ����
    public Vector3 offset; //ī�޶�� �÷��̾� ������ �Ÿ�

    void Update()
    {
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x,0, offset.z);
        Vector3 smoothedPosition = Vector3.Lerp(new Vector3(transform.position.x,0,transform.position.z), new Vector3(desiredPosition.x,0,desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
