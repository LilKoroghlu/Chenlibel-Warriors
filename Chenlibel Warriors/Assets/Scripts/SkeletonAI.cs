using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Animator animator;
    private SpriteRenderer sprite;
    private float speed = 2f;
    private float distance;
    private Vector3 oldPosition;
    private Rigidbody2D rb;
    private enum MovementState { idle, walk, attack };
    private Vector2 direction;
    private MovementState state;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        direction = player.transform.position - transform.position;
        if(distance > 3f){
        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        oldPosition = transform.position;
        }
        else{

        }

    }

    private void FixedUpdate()
    {
        
        if (direction.x != 0 && oldPosition == transform.position)
        {
            if (direction.x > 0) sprite.flipX = false;
            else sprite.flipX = true;
            state = MovementState.walk;
        }
        else
        {
            state = MovementState.idle;
        }
        animator.SetInteger("state", (int)state);
    }
}
