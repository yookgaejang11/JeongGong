using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("Move Settings")]
    public float groundMoveForce = 1500f;      // ���� �̵���
    public float airMoveForce = 250f;          // ���� �̵���
    public float maxSpeedX = 5f;               // �ִ� ���� �ӵ�
    public float maxSpeedY = 10f;              // �ִ� ���� �ӵ�
    public float groundFriction = 0.99f;       // ���� ���� ���

    [Header("Jump & Shoot Settings")]
    public GameObject bulletPrefab;            // �Ѿ� ������
    public Transform muzzle;                   // �߻� ��ġ(�ѱ�)
    public GameObject shotDirection;           // �ݵ� ���� ���� ������Ʈ
    public float shootBackPower = 5f;          // �߻� �ݵ� ��
    public int maxAirShots = 2;                // ���� �߻� �ִ� Ƚ��

    [Header("HP Settings")]
    public int maxHp = 3;                      // �ִ� HP
    public List<GameObject> hpIcons;           // HP ������ ����Ʈ

    [Header("Ground Check")]
    public float groundCheckDistance = 0.1f;   // ���� üũ ���� ����
    public LayerMask groundLayer;              // ���� ���̾� ����ũ

    [Header("UI")]
    public TextMeshProUGUI bulletText;         // ź�� ǥ�� UI

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
        // 1) �Է� ó��
        moveInput = Input.GetAxis("Horizontal");

        // 2) �ִϸ��̼� & ��������Ʈ ����
        bool isWalking = Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool("isMove", isWalking);
        sprite.flipX = moveInput < 0f;

        // 3) ���콺 ���� ȸ��
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
            mouseWorld.y - muzzle.position.y,
            mouseWorld.x - muzzle.position.x
        ) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        gunSprite.flipY = angle > 90f || angle < -90f;
        shotDirection.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        // 4) ��� �Է�
        if (Input.GetMouseButtonDown(0) && airShotsRemaining > 0)
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        // 1) ���� üũ (����ĳ��Ʈ)
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isGrounded = hit.collider != null;

        // 2) �̵��� ����
        float force = isGrounded ? groundMoveForce : airMoveForce;
        Vector2 input = new Vector2(moveInput, 0f);
        if (input.sqrMagnitude > 1f) input.Normalize();  // �밢�� �ӵ� ����
        rb.AddForce(input * force * Time.fixedDeltaTime, ForceMode2D.Force);

        // 3) ���� ����
        if (isGrounded && Mathf.Abs(moveInput) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x * groundFriction, rb.velocity.y);
        }

        // 4) �ӵ� ����
        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -maxSpeedX, maxSpeedX),
            Mathf.Clamp(rb.velocity.y, -maxSpeedY, maxSpeedY)
        );
    }

    private void Shoot()
    {
        // 1) �Ѿ� ����
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);

        // 2) �ݵ� ���� (Impulse)
        Vector2 backDir = -shotDirection.transform.up;
        rb.AddForce(backDir * shootBackPower, ForceMode2D.Impulse);

        // 3) ���� ��� Ƚ�� ����
        if (!isGrounded)
        {
            airShotsRemaining--;
            UpdateBulletUI();
        }

        // 4) ī�޶� ����ũ & �ִϸ��̼�
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
        // ���ӿ��� ó��
        UIManager.Instance.fullText = "Game\nOver";
        UIManager.Instance.ShowGameOver();
        GameManager.Instance.StageOver = true;
        // �߰� �ִϸ��̼ǡ����� ó�� ����
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
        // ���鿡 ������ ź�� ����
        if (isGrounded)
            airShotsRemaining = maxAirShots;

        bulletText.text = "x" + airShotsRemaining;
    }
}
