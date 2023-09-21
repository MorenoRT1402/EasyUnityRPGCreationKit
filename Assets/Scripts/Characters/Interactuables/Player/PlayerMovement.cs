using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : CharacterMovement
{

    private Vector2 input;

    private new void Awake()
    {
        base.Awake();

        moveSpeed = DM.moveSpeed;
        diagonalMove = DM.diagonalMove;
    }

    private void Update()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        //        if (!player) return;
        bool inBattle = GameManager.InBattle;
        bool inPriorityUI = GameManager.InPriorityUI();
        bool inEvent = EventManager.actualEventInteraction != null;
        bool cantMove = inBattle || inPriorityUI || inEvent;
        if (cantMove || isMoving) return;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (inBattle) input = Vector2.zero;

        if (!diagonalMove && input.x != 0) input.y = 0;

        if (input != Vector2.zero)
        {
            animator.SetFloat("XSpeed", input.x);
            animator.SetFloat("YSpeed", input.y);

            var targetPos = transform.position + (Vector3)input;

            if (IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
        }
    }

    public IEnumerator Move(Vector3 targetPos)
    {
        //        if (!isWalkable(targetPos) || isMoving) yield break;

        isMoving = true;
        animator.SetBool("IsMoving", true);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //            Debug.Log($"{targetPos} - {transform.position}.sqrMagnitude = {(targetPos - transform.position).sqrMagnitude} > {Mathf.Epsilon} ? {(targetPos - transform.position).sqrMagnitude > Mathf.Epsilon}");
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;
        animator.SetBool("IsMoving", false);

        DungeonManager.Instance.CheckForEncounters(this);
    }

    internal void Deactivate(bool full)
    {
        input = Vector2.zero;
        if (full) GetComponent<SpriteRenderer>().enabled = false;
        enabled = false;
    }
}