using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBool : StateMachineBehaviour
{
    public string boolName;
    public bool status;
    public bool resetOnExit = true;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, !status);
        animator.GetComponentInParent<SA.StateManager>().rigid.velocity = Vector3.zero;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(resetOnExit)
        animator.SetBool(boolName, status);
       
    }
}
