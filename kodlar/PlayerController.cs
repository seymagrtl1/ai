using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 10f;
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 0.5f;
    public float rewindDuration = 2f; // Duration to move to the previous position
    public float rewindTime = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isJumping = false;
    private bool isDashing = false;

    private Queue<Vector2> positionHistory;
    private float recordInterval = 0.1f; // How frequently we record positions (in seconds)
    private float recordTimer;

    private bool isRewinding = false;
    private float rewindCooldown;

    private Collider2D playerCollider;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        positionHistory = new Queue<Vector2>();
    }

    void Update()
    {
        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            positionHistory.Enqueue(transform.position);
            if (positionHistory.Count > (rewindTime * (1 / recordInterval))) // Keep only the positions from the last 5 seconds
            {
                positionHistory.Dequeue();
            }
            recordTimer = 0f;
        }

        if (!isRewinding && !isDashing)
        {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = 0f; // Remove vertical input
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isJumping && !isRewinding && !isDashing)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isRewinding)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.R) && rewindCooldown <= 0f && positionHistory.Count > 0)
        {
            StartCoroutine(Rewind());
        }

        if (rewindCooldown > 0f)
        {
            rewindCooldown -= Time.deltaTime;
        }

        UpdateAnimationStates();
        FlipSprite();
    }

    void FixedUpdate()
    {
        if (!isRewinding && !isDashing)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
        else if (isDashing)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed * dashSpeedMultiplier, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop player movement while rewinding
        }
    }

    void Jump()
    {
        isJumping = true;
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("IsJumping", true);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("IsDashing", true);
        float originalSpeed = moveSpeed;
        moveSpeed *= dashSpeedMultiplier;
        yield return new WaitForSeconds(dashDuration);
        moveSpeed = originalSpeed;
        isDashing = false;
        animator.SetBool("IsDashing", false);
    }

    IEnumerator Rewind()
    {
        isRewinding = true;
        playerCollider.enabled = false; // Disable the collider

        Vector2 targetPosition = positionHistory.Peek(); // Get the position from 5 seconds ago
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < rewindDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / rewindDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isRewinding = false;
        playerCollider.enabled = true; // Enable the collider
        rewindCooldown = rewindTime; // Set cooldown to the time interval between stored positions
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }
    }

    void UpdateAnimationStates()
    {
        animator.SetBool("IsRunning", moveInput.x != 0 && !isJumping && !isDashing);
    }

    void FlipSprite()
    {
        if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
