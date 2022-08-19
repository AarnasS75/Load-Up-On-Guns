using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
#endregion

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetailsSO;
    [HideInInspector] public Health health;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    public void Initialize(PlayerDetailsSO playerDetailsSO)
    {
        this.playerDetailsSO = playerDetailsSO;

        SetPlayerHealth();
    }

    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetailsSO.playerHealth);
    }
}