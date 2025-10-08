using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("Move Settings")]
    public float groundMoveForce = 1500f;      // 지면 이동력
    public float airMoveForce = 250f;          // 공중 이동력
    public float maxSpeedX = 5f;               // 최대 수평 속도
    public float maxSpeedY = 10f;              // 최대 수직 속도
    public float groundFriction = 0.99f;       // 지면 마찰 계수

    [Header("Jump & Shoot Settings")]
    public GameObject bulletPrefab;            // 총알 프리팹
    public Transform muzzle;                   // 발사 위치(총구)
    public GameObject shotDirection;           // 반동 방향 기준 오브젝트
    public float shootBackPower = 5f;          // 발사 반동 힘
    public int maxAirShots = 2;                // 공중 발사 최대 횟수

    [Header("HP Settings")]
    public int maxHp = 3;                      // 최대 HP
    public List<GameObject> hpIcons;           // HP 아이콘 리스트

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;   // 지면 체크 레이 길이
    public LayerMask groundLayer;              // 지면 레이어 마스크

    [Header("UI")]
    public TextMeshProUGUI bulletText;         // 탄약 표시 UI

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;
    private SpriteRenderer gunSprite;

    private float moveInput;
    private bool isGrounded;
    private int currentHp;
    private int airShotsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        gunSprite = muzzle.GetComponent<SpriteRenderer>();

        currentHp = maxHp;
        airShotsRemaining = maxAirShots;
        UpdateHpUI();
        UpdateBulletUI();
    }

    private void Update()
    {
        // 1) 입력 처리
        moveInput = Input.GetAxis("Horizontal");

        // 2) 애니메이션 & 스프라이트 방향
        bool isWalking = Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool("isMove", isWalking);
        sprite.flipX = moveInput < 0f;

        // 3) 마우스 방향 회전
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
            mouseWorld.y - muzzle.position.y,
            mouseWorld.x - muzzle.position.x
        ) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        gunSprite.flipY = angle > 90f || angle < -90f;
        shotDirection.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 4) 사격 입력
        if (Input.GetMouseButtonDown(0) && airShotsRemaining > 0)
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        // 1) 지면 체크 (레이캐스트)
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isGrounded = hit.collider != null;

        // 2) 이동력 적용
        float force = isGrounded ? groundMoveForce : airMoveForce;
        Vector2 input = new Vector2(moveInput, 0f);
        if (input.sqrMagnitude > 1f) input.Normalize();  // 대각선 속도 보정
        rb.AddForce(input * force * Time.fixedDeltaTime, ForceMode2D.Force);

        // 3) 지면 마찰
        if (isGrounded && Mathf.Abs(moveInput) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x * groundFriction, rb.velocity.y);
        }

        // 4) 속도 제한
        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -maxSpeedX, maxSpeedX),
            Mathf.Clamp(rb.velocity.y, -maxSpeedY, maxSpeedY)
        );
    }

    private void Shoot()
    {
        // 1) 총알 생성
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        // 2) 반동 적용 (Impulse)
        Vector2 backDir = -shotDirection.transform.up;
        rb.AddForce(backDir * shootBackPower, ForceMode2D.Impulse);

        // 3) 공중 사격 횟수 차감
        if (!isGrounded)
        {
            airShotsRemaining--;
            UpdateBulletUI();
        }

        // 4) 카메라 쉐이크 & 애니메이션
        StartCoroutine(ShakeCamera(0.4f, 0.3f));
        animator.SetTrigger("isAttack");
    }

    public void TakeDamage(int dmg)
    {
        currentHp = Mathf.Max(0, currentHp - dmg);
        UpdateHpUI();

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Blink(6, 0.1f));
        }
    }

    private void Die()
    {
        // 게임오버 처리
        UIManager.Instance.fullText = "Game\nOver";
        UIManager.Instance.ShowGameOver();
        GameManager.Instance.StageOver = true;
        // 추가 애니메이션·사운드 처리 가능
    }

    private IEnumerator Blink(int repeats, float interval)
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        for (int i = 0; i < repeats; i++)
        {
            sprite.color = new Color(1f, 1f, 1f, i % 2 == 0 ? 0.5f : 1f);
            yield return new WaitForSeconds(interval);
        }
        sprite.color = Color.white;
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    private IEnumerator ShakeCamera(float duration, float power)
    {
        Transform cam = Camera.main.transform;
        Vector3 origin = cam.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * power;
            float y = Random.Range(-1f, 1f) * power;
            cam.localPosition = origin + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.localPosition = origin;
    }

    private void UpdateHpUI()
    {
        for (int i = 0; i < hpIcons.Count; i++)
            hpIcons[i].SetActive(i < currentHp);
    }

    private void UpdateBulletUI()
    {
        // 지면에 닿으면 탄약 리셋
        if (isGrounded)
            airShotsRemaining = maxAirShots;

        bulletText.text = "x" + airShotsRemaining;
    }
}
