using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitBox : MonoBehaviour
{
    public int damage = 1; // 항상 1 데미지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Move player = collision.GetComponent<Move>();
            if (player != null)
            {
                player.SetHp(damage);
                
            }
        }
    }
}
