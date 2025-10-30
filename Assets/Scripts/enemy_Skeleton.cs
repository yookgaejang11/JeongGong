using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemy_Skeleton : MonoBehaviour
{
    int awakeCount = 0;
    public bool isDie = false;
    Animator animator;
    public int maxHp;
    public float currentHp;
    bool isShoot = true;
    public GameObject bullet;
    public Vector3 pos;
    Rigidbody2D rigid;
    Vector2 movePos;
    public bool isFInd = false;
    public bool isStop = false;
    public float speed;
    public float range;
    public float shootRange;
    BoxCollider2D BoxCollider2D;
    // Start is called before the first frame update
    private void Awake()
    {
        BoxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
    }

    public void SetHp(float damage)
    {
        if (animator.GetBool("isSpawn"))
        {
            if (GameManager.Instance.onePunch)
            {
                currentHp -= 9999;
            }
            currentHp -= damage;
            animator.SetTrigger("isHit");
            StartCoroutine(Hit());
            if (currentHp <= 0 && !isDie)
            {
                StartCoroutine(Death());
                
            }
        }
       
    }
    IEnumerator Hit()
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

        
    }

    IEnumerator Death()
    {
        currentHp = 0;
        animator.SetBool("isDeath", true);
        AudioSetting.Instance.PlaySFX(SFXType.skeleton_die);
        isDie = true;
        AudioSetting.Instance.PlaySFX(SFXType.skeleton_die);
        GameManager.Instance.score += 200;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    IEnumerator WalkSound()
    {
        while (true)
        {
            if (animator.GetBool("isWalk"))
            {
                yield return new WaitForSeconds(0.2f);
                AudioSetting.Instance.PlaySFX(SFXType.SkeletonWalk);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDie)
        {
            pos = GameObject.Find("Player").transform.position - transform.position;
            Move();

            if(isFInd)
            {
                if (!animator.GetBool("isSpawn"))
                {
                    BoxCollider2D.size = new Vector2(0.446747f, 0.9f);
                }
                awakeCount += 1;
                if(awakeCount ==1)
                {
                    AudioSetting.Instance.PlaySFX(SFXType.skeletonAwake);
                }
                animator.SetBool("isSpawn", true);
                animator.SetBool("isWalk", true);
                
            }
            else if(!isFInd)
            {
                animator.SetBool("isWalk",false);
            }
            float distance = Vector2.Distance(transform.position, GameObject.Find("Player").transform.position);
            if (distance < shootRange)
            {
                isStop = true;
                isFInd = false;
            }
            else if (distance <= range)
            {
                isFInd = true;
                isStop = false;
            }
            else
            {
                isFInd = false;
                isStop = false;
            }

            if (isFInd || isStop)
            {
                if (pos.x < 0)
                {
                    this.GetComponent<SpriteRenderer>().flipX = true;
                    transform.GetChild(0).transform.localPosition = new Vector2(-1.254f, -0.593f);
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().flipX = false;
                    transform.GetChild(0).transform.localPosition = new Vector2(1.254f, -0.593f);
                }
            }
            transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
            transform.GetChild(1).transform.localPosition = new Vector3(0, 0, 0);
        }
        

    }

    private void Move()
    {
       
        if (isFInd)
        {
            pos = GameObject.Find("Player").transform.position - transform.position;
            rigid.velocity = new Vector3(pos.x * speed, 0, 0);
            /*else//감지 안됬을때 움직이는 코드임
            {
                rigid.AddForce(Vector2.right,ForceMode2D.Impulse);
            }*/
        }
        else if (isStop)
        {
            StartCoroutine(ShootBullet());

        }


        rigid.velocity = new Vector2(rigid.velocity.x * 0.99f, 0);
    }


    IEnumerator ShootBullet()
    {
        if (isShoot)
        {
            animator.SetTrigger("isAtatck");
            isShoot = false;
            pos = GameObject.Find("Player").transform.position - transform.position;
            if(GetComponent<SpriteRenderer>().flipX)
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
            }
            if (!GetComponent<SpriteRenderer>().flipX)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.75f);
            if (GetComponent<SpriteRenderer>().flipX)
            {
                this.transform.GetChild(1).gameObject.SetActive(false);
            }
            if (!GetComponent<SpriteRenderer>().flipX)
            {
                this.transform.GetChild(0).gameObject.SetActive(false);
            }
            AudioSetting.Instance.PlaySFX(SFXType.punch);
            isShoot = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("trap"))
        {
            SetHp(99);
        }
    }
}
