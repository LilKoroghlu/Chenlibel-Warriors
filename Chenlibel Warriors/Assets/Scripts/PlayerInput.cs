using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 6f;
    private Rigidbody2D rb;
    private PlayerInputActions playerInputActions;
    private InputAction movement;
    private enum MovementState { idle, move, light_attack, heavy_attack, roll_right, roll_left }
    private MovementState state;
    private SpriteRenderer sprite;
    private Vector2 movementVector;
    private BoxCollider2D coll;
    private int jumpTime;
    private bool canMove;
    private float cannotMoveTime;
    private float lightAttackTime;
    private float heavyAttackTime;
    private float rollTime;
    [SerializeField] private LayerMask jumpableGround;
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        jumpTime = 1;
        canMove = true;
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Player_LightAttack")
            {
                lightAttackTime = clip.length;
            }
            if(clip.name == "Player_HeavyAttack")
            {
                heavyAttackTime = clip.length;
            }
            if(clip.name == "Player_RollRight"){
                rollTime = clip.length;
            }
        }
    }

    private void OnEnable()
    {
        movement = playerInputActions.Player.Movement;
        movement.Enable();
        playerInputActions.Player.Jump.performed += OnJump;
        playerInputActions.Player.Jump.Enable();
        playerInputActions.Player.LightAttack.performed += OnLightAttack;
        playerInputActions.Player.LightAttack.Enable();
        playerInputActions.Player.RollToRight.performed += OnRollRight;
        playerInputActions.Player.RollToRight.Enable();
        playerInputActions.Player.RollToLeft.performed += OnRollLeft;
        playerInputActions.Player.RollToLeft.Enable();
        playerInputActions.Player.HeavyAttack.performed += OnHeavyAttack;
        playerInputActions.Player.HeavyAttack.Enable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (jumpTime != 0)
        {
            jumpTime--;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (IsGrounded())
        {
            jumpTime = 1;
        }
    }

    private void OnLightAttack(InputAction.CallbackContext context)
    {
        if(canMove){
        state = MovementState.light_attack;
        animator.SetInteger("state", (int)state);
        cannotMoveTime = lightAttackTime;
        canMove = false;
        }
    }

    private void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if(canMove){
        state = MovementState.heavy_attack;
        animator.SetInteger("state", (int)state);
        cannotMoveTime = heavyAttackTime;
        canMove = false;
        }
    }


    private void OnRollRight(InputAction.CallbackContext context)
    {
        if(canMove)
        {
        canMove = false;
        state = MovementState.roll_right;
        rb.AddForce(Vector2.right * 5f, ForceMode2D.Impulse);
        animator.SetInteger("state", (int)state);
        cannotMoveTime = rollTime;
        sprite.flipX = false;

        }
    }

    private void OnRollLeft(InputAction.CallbackContext context)
    {
        if(canMove){
        canMove = false;
        state = MovementState.roll_left;
        rb.AddForce(Vector2.left * 5f, ForceMode2D.Impulse);
        animator.SetInteger("state", (int)state);
        cannotMoveTime = rollTime;
        sprite.flipX = true;
        }
    }

    private void OnDisable()
    {
        movement.Disable();
        playerInputActions.Player.Jump.Disable();
        playerInputActions.Player.LightAttack.Disable();
        playerInputActions.Player.HeavyAttack.Disable();
        playerInputActions.Player.RollToRight.Disable();
        playerInputActions.Player.RollToLeft.Disable();
    }

    private void Update() {
        if(cannotMoveTime <= 0){
            canMove = true;
        }
        else{
            cannotMoveTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //state = (MovementState)animator.GetInteger("state");
        movementVector = new Vector2(movement.ReadValue<Vector2>().x, rb.velocity.y);
        if (canMove)
        {
            rb.velocity = new Vector2(movementVector.x * speed, rb.velocity.y);
            if (IsGrounded())
            {
                if (rb.velocity.x != 0)
                {
                    state = MovementState.move;
                    if (rb.velocity.x > 0) sprite.flipX = false;
                    else sprite.flipX = true;
                }
                else state = MovementState.idle;
                animator.SetFloat("horizontal", rb.velocity.x);
                animator.SetFloat("vertical", 0);
            }
            else
            {

                state = MovementState.move;
                animator.SetFloat("horizontal", 0);
                if (rb.velocity.x > 0) sprite.flipX = false;
                else sprite.flipX = true;
                animator.SetFloat("vertical", rb.velocity.y);
            }
        }
        animator.SetInteger("state", (int)state);
    }


    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
