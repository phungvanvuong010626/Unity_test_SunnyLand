using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D coll;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private Vector3 startPosition;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip itemSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip jumpSound;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float hDirection;
    [SerializeField] private bool isHurt = false; // Khóa di chuyển khi bị đẩy lùi
    [SerializeField] private Health _health;      // Tham chiếu sang script quản lý máu


    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<Collider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        _health = GetComponent<Health>();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        CheckGrounded();
        GetInput();
        Movement();
        UpdateAnimParams();
    }

    private void CheckGrounded()
    {
        isGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
    }

    private void GetInput()
    {
        // Nếu đang bị đau thì không nhận Input di chuyển hay nhảy
        if (isHurt) return;

        hDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Nhấp chuột trái phát âm thanh click
        if (Input.GetMouseButtonDown(0))
        {
            PlaySFX(clickSound);
        }
    }

    private void Movement()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null");
            return;
        }

        // Nếu đang bị quái đẩy lùi (Hurt), giữ nguyên lực vật lý bounce, không can thiệp vận tốc di chuyển
        if (isHurt) return;

        // Xử lý di chuyển mượt mà bám theo Update cũ của bạn
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
            PlayAnimRun();
        }
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
            PlayAnimRun();
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            PlayAnimIdle();
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        animator.SetTrigger("Jump");
        PlaySFX(jumpSound); 
    }

    private void UpdateAnimParams()
    {
        if (animator == null) return;

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", rb.velocity.y);
    }

    private void PlayAnimRun()
    {
        animator.SetBool("IsRun", true);
    }

    private void PlayAnimIdle()
    {
        animator.SetBool("IsRun", false);
    }

    // --- TÍCH HỢP CÁC CHỨC NĂNG VA CHẠM, SÁT THƯƠNG VÀ ĐỒ VẬT ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi đâm vào quái vật
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyControl enemy = collision.gameObject.GetComponent<EnemyControl>();
            // Nếu quái chưa chết và người chơi không trong trạng thái bị thương trước đó
            if (enemy != null && !enemy.IsDead && !isHurt)
            {
                if (_health != null) _health.TakeDamage(20f); // Trừ 20 máu
                ApplyKnockback();
                PlaySFX(hitSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi ăn trái cây hồi máu
        if (collision.CompareTag("Fruit"))
        {
            if (_health != null) _health.TakeDamage(-20f); // Truyền số âm để hồi máu
            PlaySFX(itemSound);
            Destroy(collision.gameObject);
        }

        // Khi giẫm lên đầu quái vật (Trigger nằm ở đầu quái)
        if (collision.CompareTag("Enemy"))
        {
            // Chỉ tính là giẫm khi nhân vật đang rơi xuống (YVelocity < -0.1f)
            if (rb.velocity.y < -0.1f && !isHurt)
            {
                EnemyControl enemy = collision.GetComponent<EnemyControl>();
                if (enemy != null && !enemy.IsDead)
                {
                    // Nảy nhẹ lên khi giẫm chết quái
                    rb.velocity = new Vector2(rb.velocity.x, jumpforce * 0.8f);
                    enemy.TakeDamage(); // Kích hoạt hàm chết của quái
                }
            }
        }
    }


    // Xử lý hiệu ứng đẩy lùi khi trúng đòn
    private void ApplyKnockback()
    {
        isHurt = true;
        hDirection = 0; // Reset input di chuyển tạm thời
        animator.SetTrigger("Hurt");

        // Đẩy lùi về hướng ngược lại hướng đang nhìn
        float knockDir = transform.localScale.x > 0 ? -1 : 1;
        rb.velocity = new Vector2(knockDir * 5f, 5f);

        // Chờ 0.5 giây rồi trả lại quyền điều khiển
        Invoke(nameof(EndHurtState), 0.5f);
    }

    private void EndHurtState()
    {
        isHurt = false;
    }

    // Hàm phát âm thanh ngắn độc lập
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}