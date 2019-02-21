﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    enum State { InFloor, Jumping, Immune }

    [Header("Horizontal speed")]
    [SerializeField] float linearSpeed;
    [Header("Jump force")]
    [SerializeField] float jumpForce;
    [SerializeField] float health;
    [Header("Impulse force")]
    [SerializeField] float xForce;
    [SerializeField] float yForce;

    private State state = State.InFloor;
    private Rigidbody2D rb2d;
    private SpriteRenderer sr;
    private float x, y;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }
    private void FixedUpdate()
    {
        Walk();        
    }
    private void Jump()
    {
        if (state == State.Jumping) {
            return;
        }
        rb2d.velocity = new Vector2(x * linearSpeed, jumpForce);
        state = State.Jumping;
    }
    private void Walk()
    {
        if (state == State.Jumping) {
            return;
        }
        if (Mathf.Abs(x) > 0) {
            rb2d.velocity = new Vector2(x * linearSpeed, rb2d.velocity.y);
            /* Version 'tradicional'
            if (x<0) {
                sr.flipX = true;
            } else {
                sr.flipX = false;
            }
            */
            /* Ternaria
            sr.flipX = x < 0 ? true : false;
            */
            sr.flipX = (x < 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.ITEM)) {
            collision.gameObject.GetComponent<Item>().DoAction();
            //collision.gameObject.GetComponent("Item");
        }
    }
    public void ReceiveDamage(int damage)
    {
        health = health - damage;
    }
    public void SetImpulse(float force)
    {
        int multiplier = sr.flipX ? 1 : -1;
        rb2d.AddForce((new Vector2(xForce * multiplier, yForce)) * force);
        state = State.Jumping;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == State.Jumping) {
            state = State.InFloor;
        }
    }
}
