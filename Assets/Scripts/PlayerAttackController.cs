using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : MonoBehaviour
{
    public enum Attack
    {
        Null,
        One,
        Two,
        Three
    }

    AnimationController animationController;
	PlayerController playerController;

    ParticleSystem particleComboSpecialIndicator;
    ParticleSystem particleSpecialAttackIndicator;

    public bool IsSpecialAttacking { get; private set; }
    public int AttackState { get; private set; } // 1 first, 2 second, 3 combo special, null not attacking.

    [SerializeField] private const float SPECIAL_ATTACK_SPEED_REDUCE = 0.5f;
    [SerializeField] private const float SPECIAL_ATTACK_DELAY = 10.0f;

    private Attack lastAttack = Attack.Null;
    private Attack currentAttack = Attack.Null;

    private bool isAttacking = false;
    private bool attackInBuffer = false;

    private Coroutine resetLastAttackCoroutine;
    private Coroutine resetCanSpecialAttackCoroutine;

    private bool canSpecialAttack = false;

    private void Awake(){
        animationController = GetComponent<AnimationController>();
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

        Debug.Log("current: " + currentAttack);
        Debug.Log("lastattack: " + lastAttack);
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

    private void OnAttackHandler(){
        if (isAttacking == true && currentAttack != Attack.Three)
            attackInBuffer = true;

        isAttacking = true;

        currentAttack = NextAttack(lastAttack);

        if (resetLastAttackCoroutine != null){
            StopCoroutine(resetLastAttackCoroutine);
        }

        resetLastAttackCoroutine = StartCoroutine(ResetLastAttackAfterDelay(1.0f));

        if (currentAttack == Attack.Three){
            ComboSpecialNotReady();
        }
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

    private void OnSpecialAttackHandler(){
        if (!canSpecialAttack)
            return;

        IsSpecialAttacking = true;
		playerController.multiplySpeed(SPECIAL_ATTACK_SPEED_REDUCE); 

        canSpecialAttack = false;
        SpecialAttackNotReady();
    }

    public void OnSpecialAttack(InputAction.CallbackContext cb){
        if (cb.started)
            OnSpecialAttackHandler();
    }

    public void OnResetAttack(InputAction.CallbackContext cb){
        if (cb.started){
            if (lastAttack == Attack.Two) //allow reset attack for special 3th attack only
                ResetAttack();
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
        lastAttack = Attack.Null;
        ComboSpecialNotReady();
    }

    public void AttackComplete(){
        isAttacking = false;
        lastAttack = currentAttack;
        currentAttack = Attack.Null;

        if (attackInBuffer){
            attackInBuffer = false;
            if (!IsInvoking("OnAttackHandler"))
                Invoke("OnAttackHandler", 0.1f); //I have no idea but this fixed some bugs.

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

}
