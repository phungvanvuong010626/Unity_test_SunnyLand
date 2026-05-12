using System.Collections;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D coll;
    [SerializeField] private Transform pointA, pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private SpriteRenderer enemySprite;
    private bool movingToB = true;
    [SerializeField] private float timeDelay = 4f;
    public bool IsDead { get; private set; }


    private void Start()
    {
        Init();
    }
    private void Init()
    {
        startPosition = transform.position;
    }

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (coll == null)
        {
            coll = GetComponent<Collider2D>();
        }
        if(enemyCollider == null)
        {
            enemyCollider = GetComponent<Collider2D>();
        }
        if(enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }
    }
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (pointA == null || pointB == null) return;
        Transform target = movingToB ? pointB : pointA;
        if (target == null) return;
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (target.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            movingToB = !movingToB;
        }
    }

    private const string playerTag = "Player";
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(playerTag) && !IsDead)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    // kiểu trả về bắt buộc để sử dụng Coroutine, cho phép bạn tạm dừng thực thi code trong một khoảng thời gian mà không làm treo game.
    IEnumerator RespawnRoutine()
    {
        DeSpawn();
        yield return new WaitForSeconds(timeDelay);
        Spawn();
    }

    private void Spawn()
    {
        transform.position = startPosition;
        enemyCollider.enabled = true;
        enemySprite.enabled = true;
        if (rb != null) rb.simulated = true;
        IsDead = false;
    }

    private void DeSpawn()
    {
        IsDead = true;
        enemyCollider.enabled = false;
        enemySprite.enabled = false;
        if (rb != null) rb.simulated = false;
    }


    public void TakeDamage()
    {
        if (IsDead) return; // Nếu quái đang chết/đang đợi hồi sinh thì không làm gì cả

        StartCoroutine(RespawnRoutine()); // Gọi Coroutine ẩn quái và đợi hồi sinh
    }
}
