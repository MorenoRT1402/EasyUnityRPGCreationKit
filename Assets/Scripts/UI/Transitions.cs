using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEditor.Animations;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    internal void StartTransition(AnimatorController controller)
    {
        animator.runtimeAnimatorController = controller;
        animator.SetTrigger("StartTransition");
    }
}
