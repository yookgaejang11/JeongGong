using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_item : MonoBehaviour
{

    public bool canHeal = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (canHeal)
            {
                canHeal = false;
                if(collision.gameObject.GetComponent<Move>().currentHp < 3)
                {
                    collision.gameObject.GetComponent<Move>().currentHp += 1;
                    collision.gameObject.GetComponent<Move>().HpUpdate();
                }
            }
            Destroy(this.gameObject);
            
        }
    }
}
