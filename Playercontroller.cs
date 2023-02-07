using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Takes and handels input and movement for a player character
public class Playercontroller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;
    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            // If movement input is not 0, try to move
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput);
                animator.SetBool("isMoving", true);

                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));

                    if (!success)
                    {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
            // Set direction of sprite to movement direction
            if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

    }

    private bool TryMove(Vector2 direction)
    {
        // Check for potential collisions
        int count = rb.Cast(
            direction, // X and Y values betweeen -1 and 1 that represent the direction from the body to look for collisions
            movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
            castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amount to cast equal to the movement plus an offset

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();

        if (movementInput != Vector2.zero)
        {
            animator.SetFloat("XInput", movementInput.x);
            animator.SetFloat("YInput", movementInput.y);
        }
    }

    void OnFire()
    {
        animator.SetTrigger("swordAttack");
    }

    public void SwordAttack()
    {
        LockMovement();
        if (spriteRenderer.flipX == true)
        {
            swordAttack.AttackLeft();
        }
        else if (spriteRenderer.flipX == false)
        {
            swordAttack.AttackRight();
        }
        else if (movementInput.y > 0)
        {
            swordAttack.AttackUp();
        }
        else
        {
            swordAttack.AttackDown();
        }
    }

    public void EndSwordAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}
