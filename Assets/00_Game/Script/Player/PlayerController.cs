//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    [SerializeField] private Rigidbody2D rb;
//    [SerializeField] private Collider2D coll;
//    [SerializeField] private Animator animator;
//    [SerializeField] private float speed = 8f;
//    [SerializeField] private float jumpforce = 10f;
//    [SerializeField] private LayerMask ground;
//    [SerializeField] private Transform groundCheck;
//    [SerializeField] private float checkRadius = 0.2f;
//    [SerializeField] private Vector3 startPosition;

//    private bool isGrounded;
//    private bool isJumping = false;


//    public float Speed => speed;
//    public float SpeedValue() => speed;

//    public void SpeedUp(float speed)
//    {
//        this.speed += speed;
//    }

//    private void Start()
//    {
//        Init();
//    }

//    private void Init()
//    {
//        startPosition = transform.position;
//    }
//    void Awake()
//    {
//        if(rb == null)
//        {
//            rb = GetComponent<Rigidbody2D>();
//        }    
//        if(coll == null)
//        {
//            coll = GetComponent<Collider2D>();
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        CheckGrounded();
//        GetInput();
//        Movement();
//        UpdateAnimParams();
//    }

//    private void CheckGrounded()
//    {
//        isGrounded = IsGrounded();
//    }
//    float hDirection;
//    private void GetInput()
//    {
//        hDirection = Input.GetAxisRaw("Horizontal");

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            Jump();
//        }
//    }
//    private void Movement()
//    {
//        if(rb == null)
//        {
//            Debug.LogError("Rigidbody is null");
//            return;
//        }
//        if(hDirection < 0)
//        {
//            rb.velocity = new Vector2(-speed, rb.velocity.y);
//            transform.localScale = new Vector3(-1, 1, 1);
//            PlayAnimRun();
//        }
//        else if(hDirection > 0)
//        {
//            rb.velocity = new Vector2(speed, rb.velocity.y);
//            transform.localScale = new Vector3(1, 1, 1);
//            PlayAnimRun();
//        }
//        else
//        {
//            rb.velocity = new Vector2(0, rb.velocity.y);
//            PlayAnimIdle();
//        }
//    }

//    private void UpdateAnimParams()
//    {
//        if(animator == null)
//        {
//            Debug.LogError("");
//            return;
//        }
//        animator.SetBool("IsGrounded", isGrounded);
//        animator.SetFloat("YVelocity", rb.velocity.y); 
//    }
//    private bool IsGrounded() => Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
//    private void Jump()
//    {
//        if (!isGrounded) return;
//        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
//        animator.SetTrigger("Jump");
//    }
//    private void PlayAnimRun()
//    {
//        animator.SetBool("IsRun", true);
//    }
//    private void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
//    }

//    private void PlayAnimIdle()
//    {
//        animator.SetBool("IsRun", false);
//    }


//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Enemy"))
//        {
//            transform.position = startPosition;
//        }
//    }


//    //Dictionary<Collider2D, EnemyControl> enemyDic = new Dictionary<Collider2D, EnemyControl>();
//    //private void OnTriggerEnter2D(Collider2D collision)
//    //{
//    //    if (collision != null)
//    //    {
//    //        if (enemyDic.TryGetValue(collision, out var value))
//    //        {
//    //            //value.TakeDamage();
//    //        }
//    //        else
//    //        {
//    //            var enemy = collision.GetComponent<EnemyControl>();
//    //            //enemy.TakeDamage();
//    //            enemyDic[collision] = enemy;
//    //        }

//    //    }
//    //}

//}


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

    [Header("Audio Settings")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip itemSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip jumpSound;

    private Health _health; //Tham chiếu đến script Health cùng đối tượng
    private bool _isGrounded;
    private float _hDirection;
    private bool _isHurt = false;
    private Vector3 _startPosition;



    void Awake()
    {
        SetupComponents();
    }

    void Start()
    {
        InitializePosition();
    }

    void Update()
    {
        HandlePhysicsDetection();
        HandleInput();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        HandleMovementExecution();
    }


    private void SetupComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<Collider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        _health = GetComponent<Health>();
    }

    private void InitializePosition()
    {
        _startPosition = transform.position;
    }


    private void HandlePhysicsDetection()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
    }


    private void HandleInput()
    {
        _hDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space)) ExecuteJump();
        if (Input.GetMouseButtonDown(0)) PlaySFX(clickSound);
    }


    private void HandleMovementExecution()
    {
        if (_isHurt) return; //Nếu đang bị đẩy lùi thì không cho điều khiển di chuyển

        rb.velocity = new Vector2(_hDirection * speed, rb.velocity.y);
        ApplyFlip();
    }

    //Xử lý xoay hình ảnh nhân vật
    private void ApplyFlip()
    {
        if (_hDirection != 0)
        {
            transform.localScale = new Vector3(_hDirection > 0 ? 1 : -1, 1, 1);
        }
    }

    private void ExecuteJump()
    {
        if (!_isGrounded) return;

        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        animator.SetTrigger("Jump");
        PlaySFX(jumpSound);
    }

    //Cập nhật các tham số cho Animator
    private void HandleAnimations()
    {
        if (animator == null) return;

        animator.SetBool("IsRun", _hDirection != 0);
        animator.SetBool("IsGrounded", _isGrounded);
        animator.SetFloat("YVelocity", rb.velocity.y);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruit")) HandleFruitPickup(collision.gameObject);
        if (collision.CompareTag("Enemy")) HandleEnemyStomp(collision);
    }

    private void HandleEnemyCollision(GameObject enemyObj)
    {
        EnemyControl enemy = enemyObj.GetComponent<EnemyControl>();
        if (enemy != null && !enemy.IsDead && !_isHurt)
        {
            _health.TakeDamage(20f); //Gọi sang script Health để trừ máu
            ApplyKnockback();
            PlaySFX(hitSound);
        }
    }

    private void HandleFruitPickup(GameObject fruit)
    {
        _health.TakeDamage(-20f);// Dùng TakeDamage với số âm để hồi máu hoặc viết hàm Heal trong Health
        PlaySFX(itemSound);
        Destroy(fruit);
    }

    private void HandleEnemyStomp(Collider2D enemyCollider)
    {
        if (rb.velocity.y < -0.1f && !_isHurt)
        {
            EnemyControl enemy = enemyCollider.GetComponent<EnemyControl>();
            if (enemy != null && !enemy.IsDead)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpforce * 0.8f);
                enemy.TakeDamage();
            }
        }
    }

    // --- CÁC HÀM HỖ TRỢ (HELPERS) ---

    private void ApplyKnockback()
    {
        _isHurt = true;
        animator.SetTrigger("Hurt");
        float knockDir = transform.localScale.x > 0 ? -1 : 1;
        rb.velocity = new Vector2(knockDir * 5f, 5f);
        Invoke(nameof(EndHurtState), 0.5f);
    }

    private void EndHurtState() => _isHurt = false;

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}