using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;  // Add this for IEnumerator
using System.Collections.Generic;  // Add this for other collection typespublic class PlayerMovement : MonoBehaviour

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    Rigidbody2D rb;
    [SerializeField] float speed = 1f;
    [SerializeField] float jumpHeight = 3f;
    float direction = 0;
    bool isGrounded = false;
    bool canDash = true;
    bool isDashing;
    float dashingPower = 24f;
    float dashingTime = 0.2f;
    float dashingCooldown = 1f;
    bool isFacingRight = true;
    [SerializeField] private LayerMask groundLayer; 
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckRadius = 0.2f; 
    public CoinManager cm;

    Animator anim;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isDashing)
        {
            return;
        }
        
        Move(direction);

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        } 

        if ((isFacingRight && direction == -1) || (!isFacingRight && direction == 1))
            Flip();
    }

    void OnMove(InputValue value)
    {
        float v = value.Get<float>();
        // calling the function - making "v" a variable with float - (2d) values
        direction = v;

    }

    void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        anim.SetBool("isRunning", dir != 0);
    }
    void OnJump()
    {
        if(isGrounded)
            Jump();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }


    void OnCollisionEnter2D(Collision2D collision) 
    {
        isGrounded = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f)
            {
                isGrounded = true;
            }


        }
    }

    void OnCollisionStay2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            isGrounded = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f)
                {
                    isGrounded = true;
                }


            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        { 
            isGrounded = false; 
        }
            
        
    } 
    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dashDirection = direction != 0 ? direction : transform.localScale.x;
        rb.linearVelocity = new Vector2(dashDirection * dashingPower, 0f);
        if (trailRenderer != null)
            trailRenderer.emitting = true;
        
        yield return new WaitForSeconds(dashingTime);
        
        if (trailRenderer != null)
            trailRenderer.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.x *= -1f;
        transform.localScale = newLocalScale;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
        }
    }
    
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

    }




}
