using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Player,
    Enemy,
    Skeleton,
}

public class Bullet : MonoBehaviour
{
    Vector2 dir;
    public bool isAttack = true;
    public int damage = 1;
    public TargetType targetType;
    public float shootTiming;
    Rigidbody2D rigid;
    public GameObject target;
    Vector2 newPos;
    public float speed;
    // Start is called before the first frame update
    void Awake()
    {
        dir = GameObject.Find("Player").GetComponent<Move>().target.transform.right;
        rigid = GetComponent<Rigidbody2D>();
        
        try
        {
            // 예외가 발생할 수 있는 코드
            target = GameObject.FindGameObjectWithTag(targetType.ToString());
            newPos = new Vector2(target.transform.position.x - transform.position.x, 0);
        }
        catch
        {
            // 예외 처리 코드
        }


        
    }

    // Update is called once per frame

    void Update()
    {

        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
            if (targetType == TargetType.Player)
            {
            Physics2D.IgnoreLayerCollision(8, 9, true);
                if (newPos.x < 0)
                {
                    rigid.AddForce(Vector2.left * speed);
                }
                else if (newPos.x > 0)
                {
                    rigid.AddForce(Vector2.right * speed);
                }

            }
            else if (targetType == TargetType.Enemy)
            {
                rigid.velocity = dir * speed;
            }
        
        yield return new WaitForSeconds(shootTiming);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(targetType.ToString()))
        {
            if(isAttack)
            {
                if(targetType == TargetType.Player)
                {
                    isAttack = false;
                    collision.gameObject.GetComponent<Move>().SetHp(damage);
                }
                else if (targetType == TargetType.Enemy && collision.gameObject.GetComponent<Enemy>())
                {
                    isAttack = false;
                    collision.gameObject.GetComponent<Enemy>().SetHp(damage);
                }
                else if (targetType == TargetType.Enemy && collision.gameObject.GetComponent<enemy_Skeleton>())
                {
                    isAttack = false;
                    collision.gameObject.GetComponent<enemy_Skeleton>().SetHp(damage);
                }
                else if (targetType == TargetType.Enemy && collision.gameObject.GetComponent<BossPattern>())
                {
                    isAttack = false;
                    collision.gameObject.gameObject.GetComponent<BossPattern>().SetHp(2);
                }


            }

            
           
            Destroy(this.gameObject);
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
