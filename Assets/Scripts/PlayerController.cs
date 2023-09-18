using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimationController))]

public class PlayerController : MonoBehaviour
{

    AnimationController animationController;
    Rigidbody2D rb;
    Vector2 moveInput;

    public bool IsMoving { get; private set; }
    public bool IsRising { get; private set; }
    public bool IsFalling { get; private set; }

    public int AttackState { get; private set; }

    public bool IsHurt { get; private set; }

    [SerializeField] private float speed = 5.0f;

    private Coroutine resetLastAttackCoroutine;

    private enum Attack
    {
        Null,
        One,
        Two,
        Three
    }

    private Attack NextAttack(Attack attack)
    {
        if (attack == Attack.Three)
            return Attack.One;
        else
            return (Attack)((int)attack + 1);
    }

    private Attack lastAttack = Attack.Null;
    private Attack currentAttack = Attack.Null;
    private bool isAttacking = false;
    private bool attackInBuffer = false;

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixDirection()
    {
        if ((moveInput.x > 0 && transform.localScale.x < 0) ||
                (moveInput.x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }

    }
    private void FixedUpdate()
    {
		if(!isAttacking){// don't use this here, add some animation events to let player move in certain frames off attack like firts two to make the movement smoother.
			rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
			FixDirection();
		}

        Debug.Log("current: " + currentAttack);
        Debug.Log("lastattack: " + lastAttack);
    }

    private void Update()
    {
        AttackState = (int)currentAttack;
    }

    public void OnMove(InputAction.CallbackContext cb)
    {
        moveInput = cb.ReadValue<Vector2>();
        IsMoving = moveInput.x != 0;
    }

    private void OnAttackHandler()
    {
        if (isAttacking == true)
            attackInBuffer = true;

        isAttacking = true;

        currentAttack = NextAttack(lastAttack);

        if (resetLastAttackCoroutine != null)
        {
            StopCoroutine(resetLastAttackCoroutine);
        }

        resetLastAttackCoroutine = StartCoroutine(ResetLastAttackAfterDelay(0.7f));
    }

    public void OnAttack(InputAction.CallbackContext cb)
    {
        if (cb.started)
        {
            // Debug.Log("befre:");
            // Debug.Log("AttackState: " + AttackState);
            // Debug.Log("currentAttack: " + currentAttack);
            // Debug.Log("nextAttack: " + nextAttack);

			OnAttackHandler();

            // Debug.Log("after:");
            // Debug.Log("AttackState: " + AttackState);
            // Debug.Log("currentAttack: " + currentAttack);
            // Debug.Log("nextAttack: " + nextAttack);
        }
    }

    private IEnumerator ResetLastAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lastAttack = Attack.Null;
    }

    public void AttackComplete()
    {
        isAttacking = false;
        lastAttack = currentAttack;
        currentAttack = Attack.Null;

        if (attackInBuffer)
        {
            attackInBuffer = false;
			if(!IsInvoking("OnAttackHandler"))
				Invoke("OnAttackHandler", 0.1f); //I have no idea but this fixed some bugs.

        }

    }
}

