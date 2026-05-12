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
using UnityEngine.UI; // Thư viện để làm việc với các thành phần giao diện như Slider

public class PlayerController : MonoBehaviour
{
    // --- CẤU HÌNH DI CHUYỂN (MOVEMENT SETTINGS) ---
    [SerializeField] private Rigidbody2D rb;          // Tham chiếu đến thành phần vật lý 2D để điều khiển lực và vận tốc
    [SerializeField] private Collider2D coll;        // Tham chiếu đến bộ va chạm của nhân vật
    [SerializeField] private Animator animator;      // Tham chiếu đến bộ điều khiển hoạt ảnh (animation)
    [SerializeField] private float speed = 8f;       // Tốc độ di chuyển sang trái/phải
    [SerializeField] private float jumpforce = 10f;  // Lực nhảy lên
    [SerializeField] private LayerMask ground;       // Lớp (Layer) được coi là mặt đất để kiểm tra nhảy
    [SerializeField] private Transform groundCheck;  // Vị trí kiểm tra dưới chân nhân vật xem có chạm đất không
    [SerializeField] private float checkRadius = 0.2f; // Bán kính của vòng tròn kiểm tra mặt đất

    // --- HỆ THỐNG MÁU (HEALTH SYSTEM) ---
    [SerializeField] private Slider healthSlider;    // Kéo thanh Slider UI vào đây để hiển thị máu trực quan
    [SerializeField] private float maxHealth = 100f; // Chỉ số máu tối đa (mặc định là 100)
    private float currentHealth;                     // Biến lưu trữ lượng máu thực tế hiện tại của nhân vật
    private Vector3 startPosition;                   // Lưu lại vị trí tọa độ lúc vừa bắt đầu game để hồi sinh
    private bool isHurt = false; // Biến kiểm tra xem có đang trong trạng thái bị thương không

    // --- CẤU HÌNH ÂM THANH (AUDIO SETTINGS) ---
    [SerializeField] private AudioSource sfxSource;  // Thành phần phát âm thanh hiệu ứng (SFX)
    [SerializeField] private AudioClip hitSound;     // File âm thanh khi bị quái đâm trúng
    [SerializeField] private AudioClip itemSound;    // File âm thanh khi ăn được hoa quả
    [SerializeField] private AudioClip clickSound;   // File âm thanh khi nhấn chuột
    [SerializeField] private AudioClip jumpSound;    // File âm thanh khi nhấn nút nhảy

    private bool isGrounded;                         // Biến logic: đúng nếu đang đứng trên đất, sai nếu đang trên không
    private float hDirection;                        // Giá trị hướng di chuyển ngang (trái: -1, phải: 1, đứng yên: 0)

    // --- KHỞI TẠO (INITIALIZATION) ---

    void Awake()
    {
        // Gán các linh kiện bằng code để tránh việc quên kéo thả trong Inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<Collider2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        InitPlayer(); // Gọi hàm thiết lập trạng thái ban đầu cho nhân vật
    }

    private void InitPlayer()
    {
        startPosition = transform.position; // Ghi nhớ vị trí đứng lúc Start game
        currentHealth = maxHealth;           // Đặt máu hiện tại bằng máu tối đa
        UpdateHealthUI();                   // Cập nhật giá trị máu lên thanh Slider UI
    }

    // --- VÒNG LẶP CHÍNH (CORE LOOP) ---

    void Update()
    {
        CheckGrounded();     // Luôn kiểm tra xem nhân vật có đang chạm đất không
        HandleInput();       // Luôn lắng nghe các phím bấm từ người chơi
        HandleMovement();    // Xử lý di chuyển và lật mặt nhân vật
        UpdateAnimParams();  // Cập nhật các thông số cho bộ máy Animation (Run, Jump, Fall)
    }

    // --- CÁC HÀM XỬ LÝ LOGIC (LOGIC FUNCTIONS) ---

    private void CheckGrounded()
    {
        // Tạo một vòng tròn ảo tại groundCheck để xem có chạm vào Layer mặt đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, ground);
    }

    private void HandleInput()
    {
        // Lấy dữ liệu phím mũi tên hoặc A/D (Trả về -1, 0, hoặc 1)
        hDirection = Input.GetAxisRaw("Horizontal");

        // Nếu nhấn Space thì gọi hàm nhảy
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Nếu nhấn chuột trái (0) thì phát âm thanh click
        if (Input.GetMouseButtonDown(0))
        {
            PlaySFX(clickSound);
        }
    }

    private void HandleMovement()
    {
        // Gán vận tốc cho Rigidbody: X là hướng di chuyển * tốc độ, Y giữ nguyên vận tốc rơi hiện tại
        rb.velocity = new Vector2(hDirection * speed, rb.velocity.y);

        // Lật mặt nhân vật (Flip) dựa trên hướng đi
        if (hDirection != 0)
        {
            // Nếu đi phải (h > 0) thì scale X = 1, đi trái (h < 0) thì scale X = -1
            transform.localScale = new Vector3(hDirection > 0 ? 1 : -1, 1, 1);
            animator.SetBool("IsRun", true); // Bật animation chạy
        }
        else
        {
            animator.SetBool("IsRun", false); // Tắt animation chạy, quay về đứng yên
        }
    }

    private void Jump()
    {
        // Chỉ cho phép nhảy nếu đang đứng trên mặt đất
        if (!isGrounded) return;

        // Gán lực nhảy cho trục Y của Rigidbody
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        animator.SetTrigger("Jump"); // Kích hoạt animation nhảy
        PlaySFX(jumpSound);          // Phát tiếng nhảy
    }

    // --- QUẢN LÝ MÁU VÀ CHIẾN ĐẤU (HEALTH & COMBAT) ---

    private void TakeDamage(float damage)
    {
        if (isHurt) return; // Nếu đang đau thì thoát luôn, không trừ máu tiếp

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            RestartGame();
            return;
        }

        // Bắt đầu trạng thái bị thương
        isHurt = true;
        if (animator != null) animator.SetTrigger("Hurt");

        ApplyKnockback();

        // Gọi hàm tắt trạng thái bị thương sau 1 giây (hoặc thời gian bạn muốn)
        Invoke("EndHurt", 1f);
    }

    // Hàm bổ trợ để kết thúc trạng thái đau
    private void EndHurt()
    {
        isHurt = false;
    }

    private void Heal(float amount)
    {
        // Tăng máu nhưng dùng Mathf.Min để đảm bảo không vượt quá máu tối đa (maxHealth)
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI(); // Cập nhật lại thanh máu UI
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            // Gán giá trị máu hiện tại vào thanh Slider để thanh tụt/tăng
            healthSlider.value = currentHealth;
        }
    }

    private void ApplyKnockback()
    {
        // Xác định hướng đẩy lùi: nếu đang nhìn phải (X=1) thì đẩy sang trái (-1) và ngược lại
        float knockDir = transform.localScale.x > 0 ? -1 : 1;
        // Gán lực đẩy: bay ngược về sau và hơi nảy lên trời một chút (Y=5)
        rb.velocity = new Vector2(knockDir * 5f, 5f);
    }

    private void RestartGame()
    {
        transform.position = startPosition; // Đưa nhân vật về tọa độ ban đầu
        currentHealth = maxHealth;           // Hồi đầy máu
        rb.velocity = Vector2.zero;          // Đứng yên tại chỗ, không cho bị trôi do vận tốc cũ
        UpdateHealthUI();                   // Vẽ lại thanh máu đầy
        Debug.Log("Nhân vật đã hết máu và quay lại vạch xuất phát!");
    }

    // --- XỬ LÝ VA CHẠM (COLLISION & TRIGGER) ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu va chạm với vật thể có Tag là "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Lấy script EnemyControl từ quái vật để kiểm tra trạng thái
            EnemyControl enemy = collision.gameObject.GetComponent<EnemyControl>();

            // Nếu quái đang sống (không trong trạng thái ẩn để hồi sinh)
            if (enemy != null && !enemy.IsDead)
            {
                TakeDamage(20f);  // Trừ 20 máu
                PlaySFX(hitSound); // Phát tiếng bị thương
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fruit"))
        {
            Heal(20f);
            PlaySFX(itemSound);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            // LOGIC QUAN TRỌNG: 
            // 1. Con cáo phải ĐANG RƠI XUỐNG (Y velocity âm)
            // 2. Con cáo KHÔNG ĐANG BỊ THƯƠNG (!isHurt)
            if (rb.velocity.y < -0.1f && !isHurt)
            {
                EnemyControl enemy = collision.GetComponent<EnemyControl>();
                if (enemy != null && !enemy.IsDead)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpforce * 0.8f);
                    enemy.TakeDamage();
                }
            }
        }
    }

    // --- HÀM HỖ TRỢ (HELPERS) ---

    private void PlaySFX(AudioClip clip)
    {
        // Phát một đoạn âm thanh một lần duy nhất mà không làm dừng các âm thanh khác
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private void UpdateAnimParams()
    {
        if (animator != null)
        {
            // Cập nhật trạng thái đứng trên đất cho Animator
            animator.SetBool("IsGrounded", isGrounded);
            // Cập nhật vận tốc trục Y để Animator biết khi nào đang rơi (Y < 0)
            animator.SetFloat("YVelocity", rb.velocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ vòng tròn đỏ trong cửa sổ Scene (không hiện trong game) để bạn dễ căn chỉnh groundCheck
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}