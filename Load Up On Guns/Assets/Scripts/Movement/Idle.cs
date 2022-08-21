using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rb;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }
    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }
    private void OnDisable()
    {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }
    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();   
    }

    private void MoveRigidBody()
    {
        rb.velocity = Vector3.zero;
    }
}
