using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum weapon_type
{
    shotgun,
    rebolber
}

public class Move : MonoBehaviour
{

    public GameObject rebolberBullet;
    public weapon_type weapon;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    bool isHit = false;
    public Vector2 moveDirection;                                      //이동벡터
    public float MoveX = 0;
    public float currentspeed;
    public GameObject bullet;                                    //총알 obj
    bool isDie = false;                                         //게임오버 판정
    public int MaxHp = 3;                                       //최대 hp        
    public int currentHp = 3;                                   //현재 hp
    Animator animator;
    bool isWalk;                                                //걸음 판정
    public TextMeshProUGUI bulletText;                          //총알 txt
    public GameObject gunObj;                                   //총(돌리기용)
    public int maxBullet = 2;                                   //공중에서 발사 할 수 있는 최대 탄약
    public int currentBullet;                                   //현재 탄약
    public List<GameObject> hp = new List<GameObject>();   //탄약 이미지
    bool isShoot = false;                                       //발사했는가 체크
    public float speed;                                         //이동 속도
    public float angle;                                         //발사 각도(쓸일 생김)
    public GameObject target;                                   //날라갈 때 사용할 오브젝트(shotgun)의 방향
    public GameObject rebolber1;
    public GameObject rebolber2;

    public GameObject jumpdir;                                  //실제로 날아갈 방향
    Vector2 mouse;                                              //마우스 방향
    Rigidbody2D rigid;                                          //rigidbody2D
    public float GunPower = 5;                                  //총 파워
    public bool isGround;                                       //땅에 닿았는가 판정
    public float maxSpeedx;
    public float maxSpeedy;

    public GameObject anchor1;
    public GameObject anchor2;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentBullet = Mathf.Max(currentBullet, 0);
    }
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(7, 8, false);
        Physics2D.IgnoreLayerCollision(7, 9, false);
    }
    private void Update()
    {
        if(weapon == weapon_type.shotgun)
        {
            target.SetActive(true);
            rebolber1.SetActive(false);
            rebolber2.SetActive(false);
        }
        else if(weapon == weapon_type.rebolber)
        {
            target.SetActive(false);
            rebolber1.SetActive(true);
            rebolber2.SetActive(true);
        }
        

          
        if (!isDie)
        {
            currentspeed = rigid.velocity.magnitude;
           

            if (Input.GetButtonUp("Horizontal") && isGround)
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 0.7f, rigid.velocity.y);
            }

            if (angle > 90 || angle < -90)
            {
                gunObj.GetComponent<SpriteRenderer>().flipY = true;
            }
            else if (angle <= 90 || angle >= -90)
            {
                gunObj.GetComponent<SpriteRenderer>().flipY = false;
            }

            if (isWalk)
            {
                animator.SetBool("isMove", true);
            }
            else
            {
                animator.SetBool("isMove", false);
            }
            PlayerMove();
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentBullet > 0)
            {
                StartCoroutine(Shootgun());
            }
            
            /*if (isGround)
            {
                speed = 5;
            }
            else if(!isGround)
            {
                speed = 3;
            }*/
           

            if (isGround && Mathf.Abs(rigid.velocity.x) > 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 0.99f, rigid.velocity.y);
            }
            

            /*if (!isGround)
            {
                rigid.velocity = new Vector2(rigid.velocity.x * 0.98f, rigid.velocity.y);
            }*/
        }

        if(isGround)
        {
            currentBullet = maxBullet;
        }

       
        bulletText.text = "x" + currentBullet;

    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    void CheckGround()
    {
        // 바닥 판정
        isGround = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        Debug.Log("isGround: " + isGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    private void HpUpdate()
    {
        
        for (int i = 0; i < MaxHp; i++)
        {
            hp[i].gameObject.SetActive(false);
        }

        for(int i = 0; i < currentHp; i++)
        {
            hp[i].gameObject.SetActive(true);
        }
    }

    public void SetHp(float damage)
    {
        if (!isHit)
        {
            currentHp -= (int)damage;
            StartCoroutine(Invisible());
            
            if (currentHp <= 0 && !isDie)
            {
                Debug.Log("dfadf");
                currentHp = 0;
                isDie = true;
                
                UIManager.Instance.fullText = "Game\nOver";
                UIManager.Instance.ShowGameOver();
                GameManager.Instance.StageOver = true;
                AudioManager.instance.PlaySfx(Sfx.GameOver);
                AudioManager.instance.PlayBgm(false);
                AudioManager.instance.sfxVolume = 0;
                AudioManager.instance.bgmVolume = 0;
                AudioManager.instance.VolumReset();
            }
            else
            {
                AudioManager.instance.PlaySfx(Sfx.Hit1);
            }
            HpUpdate();
        }
        
    }

    IEnumerator Invisible()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        Physics2D.IgnoreLayerCollision(7,9,true);
        isHit = true;
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,1f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        Physics2D.IgnoreLayerCollision(7, 8, false);
        Physics2D.IgnoreLayerCollision(7, 9, false);
        isHit = false;
    }
    public void PlayerMove()
    {
        MoveX = Input.GetAxis("Horizontal");
        rigid.AddForce(new Vector2(MoveX * speed, 0), ForceMode2D.Force);
        if (Input.GetAxis("Horizontal") != 0)
        {
            isWalk = true;
        }
        else
        {
            isWalk = false;
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (rigid.velocity.x >= maxSpeedx)
        {
            rigid.velocity = new Vector2(maxSpeedx, rigid.velocity.y);
        }
        else if (rigid.velocity.x <= -maxSpeedx)
        {
            rigid.velocity = new Vector2(-maxSpeedx, rigid.velocity.y);
        }
        if (rigid.velocity.y >= maxSpeedy)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, maxSpeedy);
        }

        float moveX = Input.GetAxis("Horizontal");
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - target.transform.position.y, mouse.x - target.transform.position.x) * Mathf.Rad2Deg;
        if(weapon == weapon_type.shotgun)
        {
            target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if(weapon == weapon_type.rebolber)
        {
            rebolber1.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rebolber2.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    

    IEnumerator Shootgun()
    {
        if(currentBullet <= 0)
        {
            yield break;
        }

        jumpdir.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        if(weapon == weapon_type.shotgun)
        {
            maxSpeedx = 22;
            GameObject shootObj = Instantiate(bullet, target.transform.position, target.transform.rotation);
            moveDirection = -jumpdir.transform.up * GunPower;
            target.GetComponent<Animator>().SetTrigger("isAttack");
            Debug.Log("dd");
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y) + moveDirection;
            yield return new WaitForSeconds(0.1f);
            if (!isGround)
            {
                currentBullet -= 1;

            }
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx = Sfx.Playershot);
            yield return new WaitForSeconds(0.05f);
            for (float i = maxSpeedx; i > 10; i--)
            {
                maxSpeedx -= 0.5f;
                i = maxSpeedx;
                yield return new WaitForSeconds(0.05f);
            }
            maxSpeedx = 10;
        }
        else if(weapon == weapon_type.rebolber)
        {
            maxSpeedx = 11;
            moveDirection = -jumpdir.transform.up * GunPower/5.9f;
            
           
            //target.GetComponent<Animator>().SetTrigger("isAttack");
            Debug.Log("dd");
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y) + moveDirection;
            yield return new WaitForSeconds(0.1f);
            if (!isGround)
            {
                currentBullet -= 1;

            }
            GameObject shootObj = Instantiate(rebolberBullet, anchor1.transform.position, rebolber1.transform.rotation);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx = Sfx.Playershot);

            yield return new WaitForSeconds(0.1f);

            //target.GetComponent<Animator>().SetTrigger("isAttack");
            Debug.Log("dd");
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y) + moveDirection;
            GameObject shootObj1 = Instantiate(rebolberBullet, anchor2.transform.position, rebolber2.transform.rotation);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfx = Sfx.Playershot);

            
            
            yield return new WaitForSeconds(0.05f);
            for (float i = maxSpeedx; i > 10; i--)
            {
                maxSpeedx -= 0.5f;
                i = maxSpeedx;
                yield return new WaitForSeconds(0.05f);
            }
            maxSpeedx = 10;
        }
        
       if(currentBullet < 0)
        {
            Debug.LogError("총알 수 -");
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if((collision.gameObject.GetComponent<BossPattern>() != null && !collision.gameObject.GetComponent<BossPattern>().isDie) || (collision.gameObject.GetComponent<enemy_Skeleton>() != null && !collision.gameObject.GetComponent<enemy_Skeleton>().isDie))
            {
                SetHp(1);
            }

        }

        if(collision.gameObject.name == "traps")
        {
            SetHp(3);
        }
        

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Clear"))
        {
            UIManager.Instance.fullText = "Game\nClear";
            UIManager.Instance.ShowGameClear();
            GameManager.Instance.StageClear = true;
            AudioManager.instance.PlaySfx(Sfx.GameOver);
            AudioManager.instance.PlayBgm(false);
            AudioManager.instance.sfxVolume = 0;
            AudioManager.instance.bgmVolume = 0;
            AudioManager.instance.VolumReset();
        }


        if (collision.gameObject.CompareTag("ChinemachineZone"))
        {
            UIManager.Instance.ActiveTimeLine();
        }
    }

}
