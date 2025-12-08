using UnityEngine;
using UnityEngine.InputSystem;

public class Hunter : MonoBehaviour
{
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

    [Header("Animation")]
    [SerializeField] private Animator _animator;

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
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocity = _moveInput * _moveSpeed;
        _rigidbody2D.angularVelocity = _rotationInput * _rotationSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rotationInput = 1f;
        }
        else if (context.canceled)
        {
            _rotationInput = 0f; 
        }
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rotationInput = -1f;
        }
        else if (context.canceled)
        {
            _rotationInput = 0f;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Attack();
        }
    }

    public void Attack()
    {
        _animator.Play(_attackAnimation.name);
    }
}