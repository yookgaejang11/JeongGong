using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFlatform : MonoBehaviour
{
    Vector3 previousPosition;
    Vector3 startPos;

    public float delta = 2.0f;
    public float speed = 3.0f;

    private List<Rigidbody2D> riders = new List<Rigidbody2D>();

    void Start()
    {
        startPos = transform.position;
        previousPosition = startPos;
    }

    void Update()
    {
        // �÷��� �̵�
        Vector3 newPos = startPos;
        newPos.x += delta * Mathf.Sin(Time.time * speed);
        transform.position = newPos;

        // �̵��� ��ŭ ���
        Vector3 movement = transform.position - previousPosition;

        // �÷��� �� �÷��̾� �̵�
        foreach (Rigidbody2D rb in riders)
        {
            rb.transform.position += movement;
        }

        previousPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null && !riders.Contains(rb))
            {
                riders.Add(rb);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null && riders.Contains(rb))
            {
                riders.Remove(rb);
            }
        }
    }
}
