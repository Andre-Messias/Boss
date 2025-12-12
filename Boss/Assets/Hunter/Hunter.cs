using UnityEngine;
using UnityEngine.InputSystem;

public class Hunter : Entity
{
    const string MONSTER_TAG = "Monster";

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    private Vector2 _moveInput;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 200f;
    private float _rotationInput;

    [Header("Attack")]
    [SerializeField] private AnimationClip _attackAnimation;
    [SerializeField] private int _attackDamage = 25;
    [SerializeField] private float _attackRange = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;

    public event System.Action<Hunter> OnHunterDeath;
    public event System.Action<Hunter> OnHunterHit;
    public event System.Action OnHitMonster;

    private void Awake()
    {
        if (_rigidbody2D == null && !TryGetComponent(out _rigidbody2D))
        {
            Debug.LogError("Rigidbody2D component is missing on Hunter GameObject.", this);
        }

        if (_animator == null && !TryGetComponent(out _animator))
        {
            Debug.LogError("Animator component is missing on Hunter GameObject.", this);
        }

        spawnPoint = transform.position;
        spawnRotation = transform.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveInput * _moveSpeed;
        _rigidbody2D.angularVelocity = _rotationInput * _rotationSpeed;
    }
    public void MoveDir(Vector2 dir)
    {
        _moveInput = dir;
    }

    public void RotateDir(float dir)
    {
        _rotationInput = dir;
    }

    public void Attack()
    {
        _animator.Play(_attackAnimation.name);
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(transform.position, transform.right, _attackRange);
        foreach (var hit in raycastHit2Ds)
        {
            if (hit.collider.tag == MONSTER_TAG && hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.TryGetComponent(out Monster monster))
            {
                monster.TakeDamage(_attackDamage);
                OnHitMonster?.Invoke();
            }
        }
    }

    public override void OnDeath()
    {
        OnHunterDeath?.Invoke(this);
        gameObject.SetActive(false);
    }

    public override void OnTakeDamage(int damage)
    {
        OnHunterHit?.Invoke(this);
    }

    public override void OnRespawn()
    {
        gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * _attackRange);
    }
}