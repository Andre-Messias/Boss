using System;
using UnityEngine;

public class Monster : Entity
{
    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("Targeting")]
    [SerializeField] private Hunter _targetHunter;
    [SerializeField] private float _detectionRange = 10f;
    private CircleCollider2D _detectionCollider;

    [Header("Attack")]
    [SerializeField] private AnimationClip _attackAnimation;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackAngleTolerance = 3f; // Degrees
    [SerializeField] private int _attackDamage = 20;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _lastAttackTime = -Mathf.Infinity;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 3f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 100f;

    public event Action OnDeathAction;

    private void Awake()
    {
        if(_animator == null && !TryGetComponent(out _animator))
        {
            Debug.LogError("Animator component is missing on Monster GameObject.", this);
        }

        if (_rigidbody2D == null && !TryGetComponent(out _rigidbody2D))
        {
            Debug.LogError("Rigidbody2D component is missing on Monster GameObject.", this);
        }

        _detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        _detectionCollider.isTrigger = true;
        _detectionCollider.radius = _detectionRange;

        spawnPoint = transform.position;
        spawnRotation = transform.eulerAngles.z;
    }

    private void Start()
    {
        _targetHunter = SearchHunter();
    }

    private void Update()
    {
        if (_targetHunter == null || !active)
        {
            Stop();
            return;
        }

        Vector2 direction = (_targetHunter.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.z, angle);


        if(Vector2.Distance(transform.position, _targetHunter.transform.position) <= _attackRange && Mathf.Abs(angleDifference) <= _attackAngleTolerance)
        {
            // Face hunter
            _rigidbody2D.MoveRotation(angle);

            // Attack
            if(Time.time - _lastAttackTime >= _attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Move towards Hunter
            _rigidbody2D.linearVelocity = direction * _moveSpeed;

            // Rotate towards Hunter
            if (Mathf.Abs(angleDifference) <= _rotationSpeed * Time.fixedDeltaTime)
            {
                // Face hunter
                _rigidbody2D.MoveRotation(angle);               
            }
            else
            {
                float rotationDirection = Mathf.Sign(angleDifference);
                _rigidbody2D.angularVelocity = rotationDirection * _rotationSpeed;
            }
        }
    }

    private void Stop()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
    }

    private void Attack()
    {
        _animator.Play(_attackAnimation.name);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, _attackRange);
        foreach (var hit in hits)
        {
            if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent(out Hunter hunter))
            {
                hunter.TakeDamage(_attackDamage);
            }
        }
        _lastAttackTime = Time.time;
    }

    private Hunter SearchHunter()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectionRange); // OverlapCircleNonAlloc can be used for optimization (require fixed hunter size)
        Hunter closestHunter = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.TryGetComponent(out Hunter hunter))
            {
                float distanceSqr = (hunter.transform.position - transform.position).sqrMagnitude; // Using sqrMagnitude to avoid sqrt calculation

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestHunter = hunter;
                }
            }
        }
        return closestHunter;
    }

    public override void OnDeath()
    {
        OnDeathAction?.Invoke();
        gameObject.SetActive(false);
    }

    public override void OnRespawn()
    {
        gameObject.SetActive(true);
        _targetHunter = SearchHunter();
    }

    public void OnTargetDeath(Hunter target)
    {
        target.OnHunterDeath -= OnTargetDeath;
        _targetHunter = SearchHunter();
        if(_targetHunter != null)
        {
            _targetHunter.OnHunterDeath += OnTargetDeath;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_targetHunter != null || collision.attachedRigidbody == null) return;
        if(collision.attachedRigidbody.TryGetComponent(out _targetHunter))
        {
            if(_targetHunter != null)
            {
                _targetHunter.OnHunterDeath += OnTargetDeath;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null)
        {
            return;
        }

        if (collision.attachedRigidbody.TryGetComponent(out Hunter hunter) && hunter == _targetHunter)
        {
            hunter.OnHunterDeath -= OnTargetDeath;
            _targetHunter = SearchHunter();
        }
    }

    private void OnDrawGizmos()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        // Attack tolerance lines
        Gizmos.color = Color.yellow;
        Vector3 rightTolerance = Quaternion.Euler(0, 0, _attackAngleTolerance) * transform.right * _attackRange;
        Vector3 leftTolerance = Quaternion.Euler(0, 0, -_attackAngleTolerance) * transform.right * _attackRange;
        Gizmos.DrawLine(transform.position, transform.position + rightTolerance);
        Gizmos.DrawLine(transform.position, transform.position + leftTolerance);

        // Detection range
        Gizmos.color = Color.purple;
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float gizmoRadius = _detectionRange * maxScale;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
    }
}
