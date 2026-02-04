#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 飞剑控制器
/// </summary>
public class FlyingSword : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float trackingDelay = 2f;
    [SerializeField] private float attackDuration = 3f;
    [SerializeField] private float damage = 10f;

    [Header("视觉效果")]
    [SerializeField] private GameObject warningEffect = null!;
    [SerializeField] private GameObject attackEffect = null!;
    [SerializeField] private TrailRenderer trailRenderer = null!;

    private Transform playerTransform = null!;
    private Vector3 targetPosition;
    private FlyingSwordState currentState;
    private IObjectPool<FlyingSword> pool = null!;
    private float stateTimer;
    private bool hasAttacked;

    private enum FlyingSwordState
    {
        MovingToBossArea,
        TrackingPlayer,
        Attacking,
        Returning
    }

    public void Initialize(IObjectPool<FlyingSword> swordPool, Transform player)
    {
        pool = swordPool;
        playerTransform = player;
        hasAttacked = false;
        stateTimer = 0f;
        currentState = FlyingSwordState.MovingToBossArea;
        trailRenderer.enabled = false;

        if (warningEffect != null)
            warningEffect.SetActive(true);
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case FlyingSwordState.MovingToBossArea:
                MoveToBossArea();
                break;

            case FlyingSwordState.TrackingPlayer:
                TrackPlayer();
                break;

            case FlyingSwordState.Attacking:
                AttackPlayer();
                break;

            case FlyingSwordState.Returning:
                ReturnToPool();
                break;
        }
    }

    private void MoveToBossArea()
    {
        // 移动到目标位置
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 到达目标区域后开始跟踪玩家
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentState = FlyingSwordState.TrackingPlayer;
            stateTimer = 0f;

            if (warningEffect != null)
                warningEffect.SetActive(false);
        }
    }

    private void TrackPlayer()
    {
        if (stateTimer < trackingDelay)
        {
            // 延迟期间旋转对准玩家
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, directionToPlayer);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // 延迟结束，开始攻击
            currentState = FlyingSwordState.Attacking;
            stateTimer = 0f;
            trailRenderer.enabled = true;

            if (attackEffect != null)
                attackEffect.SetActive(true);
        }
    }

    private void AttackPlayer()
    {
        // 向玩家位置移动
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * (moveSpeed * 1.5f) * Time.deltaTime;

        // 检查是否攻击到玩家
        if (!hasAttacked && Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            hasAttacked = true;
            // 这里可以触发玩家受伤逻辑
            Debug.Log($"飞剑对玩家造成{damage}点伤害");
        }

        // 攻击持续时间结束或飞出屏幕外
        if (stateTimer > attackDuration ||
            !IsInScreenBounds(transform.position))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            trailRenderer.enabled = false;

            if (warningEffect != null)
                warningEffect.SetActive(false);

            if (attackEffect != null)
                attackEffect.SetActive(false);

            pool.Release(this);
        }
    }

    private bool IsInScreenBounds(Vector3 position)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(position);
        return viewportPos.x > -0.1f && viewportPos.x < 1.1f &&
               viewportPos.y > -0.1f && viewportPos.y < 1.1f;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == FlyingSwordState.Attacking)
        {
            // 攻击到玩家
            ReturnToPool();
        }
        else if (other.CompareTag("PlayerAttack"))
        {
            // 被玩家攻击摧毁
            ReturnToPool();
        }
    }
}

/// <summary>
/// 飞剑对象池管理器
/// </summary>
public class SwordPoolManager : MonoBehaviour
{
    [SerializeField] private FlyingSword swordPrefab = null!;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxPoolSize = 50;

    private IObjectPool<FlyingSword> swordPool = null!;
    private Transform playerTransform = null!;

    private void Awake()
    {
        swordPool = new ObjectPool<FlyingSword>(
            CreateSword,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxPoolSize
        );
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public FlyingSword GetSword()
    {
        return swordPool.Get();
    }

    private FlyingSword CreateSword()
    {
        var sword = Instantiate(swordPrefab, transform);
        sword.gameObject.SetActive(false);
        return sword;
    }

    private void OnTakeFromPool(FlyingSword sword)
    {
        sword.gameObject.SetActive(true);
        sword.Initialize(swordPool, playerTransform);
    }

    private void OnReturnedToPool(FlyingSword sword)
    {
        sword.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(FlyingSword sword)
    {
        Destroy(sword.gameObject);
    }
}