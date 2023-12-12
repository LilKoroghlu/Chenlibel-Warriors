using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParameterOnEnter : StateMachineBehaviour
{
    [SerializeField]
    private string parameterName;
    [SerializeField] private int value;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetInteger(parameterName, value);
    }
}