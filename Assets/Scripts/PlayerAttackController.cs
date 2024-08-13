using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Animator))]

public class PlayerAttackController : MonoBehaviour
{
    public enum Attack
    {
        Null,
        One,
        Two,
        Three,
		Canceled
    }
    Animator animator;
    AnimationController animationController;
	PlayerController playerController;

    ParticleSystem particleComboSpecialIndicator;
    ParticleSystem particleSpecialAttackIndicator;

    public bool IsSpecialAttacking { get; private set; }
    public int AttackState { get; private set; } // 1 first, 2 second, 3 combo special, null not attacking.

    [SerializeField] private const float SPECIAL_ATTACK_SPEED_REDUCE = 0.5f;
    [SerializeField] private const float ATTACK_SPEED_REDUCE = 0.75f;
    [SerializeField] private const float SPECIAL_ATTACK_DELAY = 10.0f;

    private Vector2 _knockBackVector = new Vector2(0f, 0f);
    private Attack lastAttack = Attack.Null;
    private Attack currentAttack = Attack.Null;

    private bool isNormalAttacking = false;
    private bool attackInBuffer = false;

    private Coroutine resetLastAttackCoroutine;
    private Coroutine resetCanSpecialAttackCoroutine;

    private bool canSpecialAttack = false;

    public bool isAttacking(){
        return IsSpecialAttacking || isNormalAttacking;
    }

    private void Awake(){
        animationController = GetComponent<AnimationController>();
        animator = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();
        particleComboSpecialIndicator = GameObject.Find("AttackReadyIndicator").GetComponent<ParticleSystem>();
        particleSpecialAttackIndicator = GameObject.Find("SpecialAttackReadyIndicator").GetComponent<ParticleSystem>();
    }

    private void Start(){
        resetCanSpecialAttackCoroutine = StartCoroutine(ResetCanSpecialAttackAfterDelay(SPECIAL_ATTACK_DELAY));
    }

    // Update is called once per frame
    private void Update(){
        AttackState = (int)currentAttack;
        _knockBackVector.x = animator.GetFloat("KnockBackX");
        _knockBackVector.y = animator.GetFloat("KnockBackY");
        // Debug.Log("current: " + currentAttack);
        // Debug.Log("lastattack: " + lastAttack);
    }

    private Attack NextAttack(Attack attack){
        if (attack == Attack.Three)
            return Attack.One;
        else
            return (Attack)((int)attack + 1);
    }

    private void ComboSpecialReady(){
        particleComboSpecialIndicator.Play();
    }

    private void ComboSpecialNotReady(){
        particleComboSpecialIndicator.Stop();
    }

    private void SpecialAttackReady(){
        particleSpecialAttackIndicator.Play();
    }

    private void SpecialAttackNotReady(){
        particleSpecialAttackIndicator.Stop();
    }

    public void OnAttack(InputAction.CallbackContext cb){
        if (cb.started){
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

    public void OnSpecialAttack(InputAction.CallbackContext cb){
        if (cb.started)
            OnSpecialAttackHandler();
    }

    public void OnCancelCombo(InputAction.CallbackContext cb){
        if (cb.started){
            CancelComboHandler();
		}
    }


	// HANDLERS
    private void OnAttackHandler(){
        if (isNormalAttacking == true && currentAttack != Attack.Three)
            attackInBuffer = true;

        isNormalAttacking = true;

		playerController.multiplySpeed(ATTACK_SPEED_REDUCE); 

		if(lastAttack == Attack.Canceled)
			currentAttack = Attack.One;

		else
	        currentAttack = NextAttack(lastAttack);

        if (resetLastAttackCoroutine != null){
            StopCoroutine(resetLastAttackCoroutine);
        }

        resetLastAttackCoroutine = StartCoroutine(ResetLastAttackAfterDelay(1.0f));

        if (currentAttack == Attack.Three){
            ComboSpecialNotReady();
        }
    }

    private void OnSpecialAttackHandler(){
        if (!canSpecialAttack)
            return;

        IsSpecialAttacking = true;
		playerController.multiplySpeed(SPECIAL_ATTACK_SPEED_REDUCE); 

        canSpecialAttack = false;
        SpecialAttackNotReady();
    }

    private void CancelComboHandler(){
			//allow reset attack for special 3th attack only
            if (lastAttack == Attack.Two || currentAttack == Attack.Two){
				ResetAttack();
		        lastAttack = Attack.Canceled;
			}
    }

    private IEnumerator ResetLastAttackAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        ResetAttack();
    }

    private IEnumerator ResetCanSpecialAttackAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        canSpecialAttack = true;
        SpecialAttackReady();
    }

    private void ResetAttack(){
		attackInBuffer = false;
        ComboSpecialNotReady();
    }

    public void AttackComplete(){
        isNormalAttacking = false;
		playerController.resetSpeed();

		if(!(lastAttack == Attack.Canceled && currentAttack == Attack.Two))
	        lastAttack = currentAttack;

        currentAttack = Attack.Null;

        if (attackInBuffer){
            attackInBuffer = false;
            if (!IsInvoking("OnAttackHandler"))
                Invoke("OnAttackHandler", 0.001f); //I have no idea but this fixed some bugs.

        }

        if (lastAttack == Attack.Two){
            ComboSpecialReady();
        }
    }

    public void SpecialAttackComplete(){
        IsSpecialAttacking = false;
		playerController.resetSpeed();

        resetCanSpecialAttackCoroutine = StartCoroutine(ResetCanSpecialAttackAfterDelay(SPECIAL_ATTACK_DELAY));
    }

    public void SetKnockBackVector(Vector2 kbVec){
        _knockBackVector = kbVec;
    }

    public Vector2 GetKnockBackVector(){
        return _knockBackVector;
    }
}
