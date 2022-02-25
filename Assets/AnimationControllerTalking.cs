using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerTalking : StateMachineBehaviour
{
    readonly float talkMinTime = 1;
    readonly float talkMaxTime = 3;

    float talkTimer = 0;

    string[] talkTriggers = { "Thinking", "Talking 2", "Talking 3" };

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (talkTimer <= 0)
        {
            TalkRandom(animator);
            talkTimer = Random.Range(talkMinTime, talkMaxTime);
        }
        else {
            talkTimer -= Time.deltaTime;
        }
    }

    void TalkRandom (Animator animator)
    {
        System.Random rnd = new System.Random();
        int talkPosition = rnd.Next(talkTriggers.Length);
        string talkTrigger = talkTriggers[talkPosition];
        animator.SetTrigger(talkTrigger);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
