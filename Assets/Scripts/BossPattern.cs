using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossPattern : MonoBehaviour
{
    public bool isActive = false;
    public Slider slider;
    public Animator animator;
    public Rigidbody2D rb;
    public Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    [SerializeField] ParticleSystem kungParticle;
    public GameObject stingHitBox;
    public GameObject slashHitBox;
    public GameObject kungHitBox;
    public GameObject spinHitBox;
    public GameObject rushHitBox;

    public float patternWaitTime = 0.5f;
    public float detectionRange;
    public float attackRange;
    public float rushMinDistance;
    public float moveSpeed;

    public float maxHp = 100;
    public float currentHp = 100;
    private bool isAttacking = false;
    private bool isRushing = false;
    
    public bool isDie = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        currentHp = maxHp;
        slider.maxValue = maxHp;
        slider.value = maxHp;
        DisableAllHitBoxes();
        StartCoroutine(PatternLoop());
    }

    private void Update()
    {
        if(isActive)
        {
            if(isDie)
            {
                return;
            }
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 방향 전환
            if (playerTransform.position.x < transform.position.x)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;

            // 따라가기
            if (distance <= detectionRange && distance > attackRange && !isAttacking && !isRushing)
            {
                FollowPlayer();
                animator.SetBool("walk", true);
            }
            else
            {
                animator.SetBool("walk", false);
            }
        }
        
    }

    private void FollowPlayer()
    {
        if(isDie)
        {
            return;
        }
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
    }

    public void LoopPattern()
    {
        StartCoroutine(PatternLoop());
    }

    IEnumerator PatternLoop()
    {
        while (currentHp > 0)
        {
            if (isActive)
            {
                if (isDie)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                float distance = Vector2.Distance(transform.position, playerTransform.position);

                if (distance <= attackRange && !isAttacking)
                {
                    isAttacking = true;
                    animator.SetBool("walk", false);
                    rb.velocity = Vector2.zero;

                    int pattern = Random.Range(0, 5);
                    switch (pattern)
                    {
                        case 0: yield return StartCoroutine(Sting()); break;
                        case 1: yield return StartCoroutine(Slash()); break;
                        case 2: yield return StartCoroutine(Kung()); break;
                        case 3: yield return StartCoroutine(Spin()); break;
                        case 4: yield return StartCoroutine(LongSpin()); break;
                    }

                    DisableAllHitBoxes();
                    isAttacking = false;
                }

                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        
       yield return null;
    }

    IEnumerator Sting()
    {
        animator.SetTrigger("sting");
        AudioManager.instance.PlaySfx(Sfx.Sting);
        yield return new WaitForSeconds(0.3f);
        stingHitBox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        stingHitBox.SetActive(false);
        yield return new WaitForSeconds(patternWaitTime);
    }

    IEnumerator Slash()
    {
        animator.SetTrigger("slash");
        AudioManager.instance.PlaySfx(Sfx.Slash);
        yield return new WaitForSeconds(0.4f);
        slashHitBox.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        slashHitBox.SetActive(false);
        yield return new WaitForSeconds(patternWaitTime);
    }

    IEnumerator Kung()
    {
        animator.SetTrigger("kung");
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlaySfx(Sfx.Stomp);
        kungParticle.Play();
        kungHitBox.SetActive(true);
        ShakeCamera.Instance.Shaking(5, 1, 0.5f);
        yield return new WaitForSeconds(0.4f);
        kungHitBox.SetActive(false);
        yield return new WaitForSeconds(patternWaitTime);
    }

    IEnumerator Spin()
    {
        
        animator.SetTrigger("spin");
        spinHitBox.SetActive(true);
        AudioManager.instance.PlaySfx(Sfx.Slash);
        AudioManager.instance.PlaySfx(Sfx.Slash);
        yield return new WaitForSeconds(1f);
        spinHitBox.SetActive(false);
        yield return new WaitForSeconds(patternWaitTime);
    }
    IEnumerator LongSpin()
    {
        float duration = 1.5f;
        float timer = 0f;
        animator.SetBool("long", true);
        spinHitBox.SetActive(true);

        while (timer < duration)
        {
            // 플레이어를 지속적으로 따라감
            
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
            AudioManager.instance.PlaySfx(Sfx.Slash);
            timer += Time.deltaTime;
            yield return null;
        }
        
        // 정지 + 초기화
        rb.velocity = Vector2.zero;
        spinHitBox.SetActive(false);
        animator.SetBool("long", false);
        yield return new WaitForSeconds(patternWaitTime);
    }

    public void SetHp(float damage)
    {
        if(!isDie)
        { 
            currentHp -= damage;
            slider.value = currentHp;
            StartCoroutine(Hit());
            if(currentHp <= 0)
            {
                slider.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.25f);
                currentHp = 0;
                isDie = true;
                animator.SetTrigger("die");
                this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0,0);
                rb.bodyType = RigidbodyType2D.Static;
                DisableAllHitBoxes();
            }

        }
    }

    IEnumerator Hit()
    {
        AudioManager.instance.PlaySfx(Sfx.hit_mino);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1,0,0,0.75f);
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1f);

    }

    //IEnumerator Rush()
    //{
    //    isRushing = true;
    //    animator.SetTrigger("rush");
    //    rushHitBox.SetActive(true);

    //    Vector2 dir = (playerTransform.position - transform.position).normalized;
    //    float speed = 8f;
    //    float duration = 0.5f;
    //    float timer = 0f;

    //    while (timer < duration && isRushing)
    //    {
    //        rb.velocity = dir * speed;
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }

    //    rb.velocity = Vector2.zero;
    //    rushHitBox.SetActive(false);
    //    isRushing = false;
    //    yield return new WaitForSeconds(0.5f);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRushing && collision.CompareTag("Player") && !isDie)
        {
            Move player = collision.GetComponent<Move>();
            if (player != null)
            {
                player.SetHp(1);
            }
            //StopRush();
        }
    }

    //public void StopRush()
    //{
    //    isRushing = false;
    //    rb.velocity = Vector2.zero;
    //    rushHitBox.SetActive(false);
    //}

    private void DisableAllHitBoxes()
    {
        stingHitBox.SetActive(false);
        slashHitBox.SetActive(false);
        kungHitBox.SetActive(false);
        spinHitBox.SetActive(false);
        rushHitBox.SetActive(false);
    }
}