using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    /*[HideInInspector]*/
    public string id;

    protected Animator animator;

    protected bool isMoving = false;
    protected float moveSpeed = 2;
    protected bool diagonalMove;
    protected List<LayerMask> solidObjectsLayer;
    protected List<LayerMask> interactuableObjectsLayer;
    protected float solidCollisionRadio = 0.4f; //0.4f
    protected float interactuableCollisionRadio = 0.1f;

    protected DungeonManager DM;

    public bool IsMoving => isMoving;


    protected virtual void Awake()
    {
        DM = DungeonManager.Instance;
        animator = GetComponent<Animator>();

//        id = $"CM{cont++}-{gameObject.name}";
        id = $"CM-{gameObject.name}";


        solidObjectsLayer = DM.solidObjectsLayer;
        interactuableObjectsLayer = DM.interactuableObjectsLayer;
        solidCollisionRadio = DM.solidCollisionRadio;
        interactuableCollisionRadio = DM.interactuableCollisionRadio;
    }

    protected bool IsWalkable(Vector3 targetPos)
    {
        Debug.DrawLine(targetPos + new Vector3(-solidCollisionRadio, 0f, 0f), targetPos + new Vector3(solidCollisionRadio, 0f, 0f), Color.red, 1f); // Línea horizontal
        Debug.DrawLine(targetPos + new Vector3(0f, -solidCollisionRadio, 0f), targetPos + new Vector3(0f, solidCollisionRadio, 0f), Color.red, 1f); // Línea vertical

        Collider2D[] colliders = GetColliders(targetPos);

//        Debug.Log($"Is Walkable {gameObject} {colliders.Length == 0}");

        return colliders.Length == 0;
    }

    protected Collider2D[] GetColliders(Vector3 targetPos)
    {
        List<Collider2D> colliders = new();
        Vector2 solidRadio = new(solidCollisionRadio, solidCollisionRadio);
        Vector2 interactuableRadio = new(interactuableCollisionRadio, interactuableCollisionRadio);

        colliders.AddRange(GetColliders(targetPos, solidRadio, solidObjectsLayer));
        colliders.AddRange(GetColliders(targetPos, interactuableRadio, interactuableObjectsLayer));

        return colliders.ToArray();
    }

    protected Collider2D[] GetColliders(Vector3 targetPos, Vector2 halfSize, List<LayerMask> layers)
    {
        List<Collider2D> colliders = new();

        for (int i = 0; i < layers.Count; i++)
            colliders.AddRange(Physics2D.OverlapBoxAll(targetPos, halfSize, 0f, layers[i]));

        return colliders.ToArray();
    }

}
