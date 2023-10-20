using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstantsFurior;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    PlayerController playerController;
	PlayerAttackController attackController;
    private string currentState;

    private void Start() {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        attackController = GetComponent<PlayerAttackController>();
    }


    private void Update() {
        if(playerController.IsHurt){
            ChangeState(AnimationConstants.HURT);
        }        

		if(attackController.IsSpecialAttacking)
			ChangeState(AnimationConstants.SPECIAL_ATTACK);

        else if(attackController.AttackState != 0){
			int state = attackController.AttackState;

			if(state == 1){
				ChangeState(AnimationConstants.ATTACK_1);
			}

			else if(state == 2){
				ChangeState(AnimationConstants.ATTACK_2);
			}

			else if(state == 3){
				ChangeState(AnimationConstants.ATTACK_3);
			}
            
        }
		
        else if(playerController.IsMoving){
            ChangeState(AnimationConstants.RUN);
        }
        else{
            ChangeState(AnimationConstants.IDLE);
        }
    }

    private void ChangeState(string state){
        if(currentState == state)
            return;
        
        animator.Play(state);
        currentState = state;   
    }

}

