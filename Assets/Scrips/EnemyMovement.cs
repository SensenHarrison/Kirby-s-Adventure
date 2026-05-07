using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Move")]
    public float minPatrolSpeed = 1.5f;
    public float maxPatrolSpeed = 3f;
    public float chaseSpeed = 4f;
    
    private float patrolSpeed;
    private float blockedByEnemyTimer = 0f;
    public float blockedTurnTime = 1f;

    [Header("Patrol")]
    public float patrolDistance = 3f;
    public float patrolWaitTime = 1f;

    [Header("Detection")]
    public Transform wallCheckPoint;
    public float wallCheckDistance = 0.2f;
    public LayerMask obstacleLayer;
    public LayerMask enemyLayer;

    [Header("Shoot")]
    public EnemyBullet[] bulletPool;
    public Transform firePoint;
    public float shootDelay = 2f;
    public float shootCooldown = 6f;
    
    [Header("Sound Effect")]
    public AudioClip shootClip;
    
    [Header("Stuck Detection")]
    public float noMoveCheckTime = 2f;
    public float moveThreshold = 0.05f;
    
    [Header("Idle")]
    public float idleDuration = 1.5f;

    private float idleTimer = 0f;

    private Vector3 lastPosition;
    private float noMoveTimer = 0f;

    private int facingDirection = 1;
    private EnemyState enemyState;

    private Rigidbody2D rb;
    private Transform player;
    private Animator anim;

    private bool playerInRange = false;
    private float stayTimer = 0f;
    private float shootCooldownTimer = 0f;

    private float patrolCenterX;
    private int patrolDirection = 1;
    private float patrolWaitTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        if (Random.value < 0.5f)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 1;
        }
        
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        transform.localScale = scale;

        patrolCenterX = transform.position.x;
        patrolDirection = facingDirection;
        patrolSpeed = Random.Range(minPatrolSpeed, maxPatrolSpeed);
        
        lastPosition = transform.position;
        
        ChangeState(EnemyState.Patrolling);
    }

    void Update()
    {
        if (shootCooldownTimer > 0f)
        {
            shootCooldownTimer -= Time.deltaTime;
        }
        
        if (playerInRange && player != null)
        {
            idleTimer = 0f;
            ChasePlayer();
        }
        else if (enemyState == EnemyState.Idle)
        {
            HandleIdle();
        }
        else
        {
            Patrol();
        }

        CheckIfStuckAndGoIdle();
    }
    
    void CheckIfStuckAndGoIdle()
    {
        float movedDistance = Vector2.Distance(transform.position, lastPosition);

        if (enemyState == EnemyState.Patrolling || enemyState == EnemyState.Chasing)
        {
            if (movedDistance <= moveThreshold)
            {
                noMoveTimer += Time.deltaTime;

                if (noMoveTimer >= noMoveCheckTime)
                {
                    rb.velocity = Vector2.zero;
                    noMoveTimer = 0f;
                    idleTimer = 0f;
                    ChangeState(EnemyState.Idle);
                }
            }
            else
            {
                noMoveTimer = 0f;
            }
        }
        else
        {
            noMoveTimer = 0f;
        }

        lastPosition = transform.position;
    }

    void ChasePlayer()
    {
        if (enemyState != EnemyState.Chasing)
        {
            ChangeState(EnemyState.Chasing);
        }

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * chaseSpeed;

        stayTimer += Time.deltaTime;

        if (stayTimer >= shootDelay && shootCooldownTimer <= 0f)
        {
            ShootAtPlayer();
            shootCooldownTimer = shootCooldown;
        }
    }

    void Patrol()
    {
        if (enemyState == EnemyState.Idle)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (enemyState != EnemyState.Patrolling)
        {
            ChangeState(EnemyState.Patrolling);
        }

        if (patrolWaitTimer > 0f)
        {
            patrolWaitTimer -= Time.deltaTime;
            rb.velocity = Vector2.zero;
            return;
        }

        float leftLimit = patrolCenterX - patrolDistance;
        float rightLimit = patrolCenterX + patrolDistance;

        bool hitWall = IsWallAhead();
        bool hitEnemy = IsEnemyAhead();

        if (patrolDirection == 1 && transform.position.x >= rightLimit)
        {
            TurnAround(-1);
            return;
        }
        else if (patrolDirection == -1 && transform.position.x <= leftLimit)
        {
            TurnAround(1);
            return;
        }
        else if (hitWall)
        {
            TurnAround(-patrolDirection);
            return;
        }
        else if (hitEnemy)
        {
            rb.velocity = Vector2.zero;
            blockedByEnemyTimer += Time.deltaTime;

            if (blockedByEnemyTimer >= blockedTurnTime)
            {
                TurnAround(-patrolDirection);
                blockedByEnemyTimer = 0f;
            }

            return;
        }

        blockedByEnemyTimer = 0f;
        rb.velocity = new Vector2(patrolDirection * patrolSpeed, rb.velocity.y);
    }
    
    void HandleIdle()
    {
        rb.velocity = Vector2.zero;
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            idleTimer = 0f;
            
            patrolCenterX = transform.position.x;
            patrolDirection = facingDirection;
            patrolWaitTimer = 0f;
            blockedByEnemyTimer = 0f;

            ChangeState(EnemyState.Patrolling);
        }
    }

    void TurnAround(int newDirection)
    {
        patrolDirection = newDirection;
        patrolWaitTimer = patrolWaitTime;
        rb.velocity = Vector2.zero;

        if (facingDirection != patrolDirection)
        {
            Flip();
        }
    }

    bool IsWallAhead()
    {
        if (wallCheckPoint == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            obstacleLayer
        );

        return hit.collider != null;
    }

    bool IsEnemyAhead()
    {
        if (wallCheckPoint == null) return false;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheckPoint.position,
            Vector2.right * facingDirection,
            wallCheckDistance,
            enemyLayer
        );

        if (hit.collider == null)
            return false;

        if (hit.collider.transform.root == transform.root)
            return false;

        return true;
    }

    void Flip()
    {
        facingDirection *= -1;

        transform.localScale = new Vector3(
            transform.localScale.x * -1f,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
            stayTimer = 0f;
            ChangeState(EnemyState.Chasing);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            stayTimer = 0f;
            rb.velocity = Vector2.zero;
            
            patrolCenterX = transform.position.x;
            patrolDirection = facingDirection;
            patrolWaitTimer = 0f;
            blockedByEnemyTimer = 0f;
            
            ChangeState(EnemyState.Patrolling);
        }
    }

    void ShootAtPlayer()
    {
        if (firePoint == null || player == null) return;

        EnemyBullet bullet = GetAvailableBullet();
        if (bullet == null) return;

        Vector2 shootDirection = (player.position - firePoint.position).normalized;
        bullet.Fire(firePoint.position, shootDirection);
        
        if (shootClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(shootClip);
        }
    }

    EnemyBullet GetAvailableBullet()
    {
        for (int i = 0; i < bulletPool.Length; i++)
        {
            if (!bulletPool[i].gameObject.activeInHierarchy)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Patrolling || enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isMoving", false);
        }

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Patrolling || enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isMoving", true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;

            Vector3 dir = Vector3.right;
            if (Application.isPlaying)
            {
                dir = Vector3.right * facingDirection;
            }

            Gizmos.DrawLine(
                wallCheckPoint.position,
                wallCheckPoint.position + dir * wallCheckDistance
            );
        }
    }
}

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
}