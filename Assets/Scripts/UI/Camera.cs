using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float marginX = 0;
    public float marginY = 0;
    public float marginZ = 0;

    public GameObject Leader => PartyManager.Instance.GetLeader().gameObject;

    private Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    private void Update() {
        FollowLeader();
    }

    private void FollowLeader()
    {
        float posX = Leader.transform.position.x + marginX;
        float posY = Leader.transform.position.y + marginY;
        float posZ = startPos.z + marginZ;

        Vector3 posVector = new (posX, posY, posZ);
        transform.position = posVector;
    }
}
