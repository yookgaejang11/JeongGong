using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HidingPlatform : MonoBehaviour
{
    public float droptime;

    void Start()
    {
    }

    void Update()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke("drop", droptime);

        }
        if (collision.gameObject.CompareTag("trap"))
        {
            Destroy(this.gameObject);
        }
    }

    void drop()
    {
        gameObject.AddComponent<Rigidbody2D>();
    }
}
