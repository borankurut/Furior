using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(PlayerAttackController))]

public class PlayerController : MonoBehaviour
{
    AnimationController animationController;
    PlayerAttackController attackController;
    Rigidbody2D rb;
    Vector2 moveInput;

    public bool IsMoving { get; private set; }
    public bool IsRising { get; private set; }
    public bool IsFalling { get; private set; }

    public bool IsHurt { get; private set; }

    [SerializeField] private float ORIGINAL_SPEED = 5.0f;

    private float speed;

    private bool canMove = true;

    private void Awake(){
        speed = ORIGINAL_SPEED;
        animationController = GetComponent<AnimationController>();
        attackController = GetComponent<PlayerAttackController>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixDirection(){
        if ((moveInput.x > 0 && transform.localScale.x < 0) ||
                (moveInput.x < 0 && transform.localScale.x > 0)){
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

    }
    private void FixedUpdate(){
        if (canMove){
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
            if(!attackController.isAttacking())
                FixDirection();
        }

    }

    public void OnMove(InputAction.CallbackContext cb){
        moveInput = cb.ReadValue<Vector2>();
        IsMoving = moveInput.x != 0;
    }

    public void setCanMoveTrue(){
        canMove = true;
    }

    public void setCanMoveFalse(){
        canMove = false;
    }

	public void multiplySpeed(float value){
		speed *= value; 
	}

	public void resetSpeed(){
		speed = ORIGINAL_SPEED;
	}
}

