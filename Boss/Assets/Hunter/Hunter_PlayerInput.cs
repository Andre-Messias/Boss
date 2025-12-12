using UnityEngine;
using UnityEngine.InputSystem;

public class Hunter_PlayerInput : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Hunter _hunter;

    private void Awake()
    {
        if (_hunter == null && !TryGetComponent(out _hunter))
        {
            Debug.LogError("Hunter component is missing on Hunter_PlayerInput GameObject.", this);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        _hunter.MoveDir(context.ReadValue<Vector2>());
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _hunter.RotateDir(1);
        }
        else if (context.canceled)
        {
            _hunter.RotateDir(0);
        }
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _hunter.RotateDir(-1);
        }
        else if (context.canceled)
        {
            _hunter.RotateDir(0);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _hunter.Attack();
        }
    }
}
