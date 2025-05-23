using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    private Rigidbody2D _rb;
    private Vector2 _move;

    public void Initialize(Rigidbody2D rigidbody)
    {
        _rb = rigidbody;
    }
 
    public void Move(Vector2 input)
    {
        _move = input;
        if(input.x < 0)
        {
            _move = new Vector2(-1f, input.y);
        } else if (input.x > 0) 
        { 
            _move = new Vector2(1f, input.y);
        }
        else
        {
            _move = Vector2.zero;
        }

        if (_move != Vector2.zero)
        {
            Vector2 targetVelocity = new Vector2(_move.x, 0f) * moveSpeed;
            if (_rb.velocity.y != 0) 
            {
                _rb.velocity = new Vector2(targetVelocity.x, _rb.velocity.y);
            }
            else
            {
                _rb.velocity = new Vector2(targetVelocity.x, _rb.velocity.y);
            }
        }
        else
        {
                _rb.velocity = new Vector2(0f, _rb.velocity.y);   
        }
    }
}
