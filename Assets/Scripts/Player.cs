using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float Speed = 10f;
    [SerializeField] float jumpPower = 10;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject collectPrefab;

    SpriteRenderer sr;
    Rigidbody2D rb;
    BoxCollider2D bc;
    Animator animator;

    bool isDead;

    enum LastWallJumpSide
    {
        None,
        Left,
        Right,
    }

    // variable to keep track of which side we last jumped
    LastWallJumpSide lastWallJumpSide = LastWallJumpSide.None;

    void Start()
    {
        // for performance, keep references to various components
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
            return;

        float direction = Input.GetAxisRaw("Horizontal");

        if (direction == 0)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Run", true);
            // keep 5 cm distance to make sure we cannot "stick" to walls
            if (direction < 0 && !GroundCheck(Vector2.left, 0.05f))
            {
                transform.Translate(direction * Speed * Time.deltaTime, 0, 0);
                rb.velocity = new Vector2(0, rb.velocity.y);
                sr.flipX = true;
                animator.SetBool("Run", true);
            }
            else if (direction > 0 && !GroundCheck(Vector2.right, 0.05f))
            {
                transform.Translate(direction * Speed * Time.deltaTime, 0, 0);
                rb.velocity = new Vector2(0, rb.velocity.y);
                sr.flipX = false;
                animator.SetBool("Run", true);
            }
        }

        bool jump = Input.GetButtonDown("Jump");

        // if just jumping from ground, to disable jumping in mid-air)
        if (jump && GroundCheck(Vector2.down))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
            lastWallJumpSide = LastWallJumpSide.None;
        }

        // if we have a wall on left side, check that our last walljump wasn't on same side
        else if (jump && GroundCheck(Vector2.left) && lastWallJumpSide != LastWallJumpSide.Left)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
            lastWallJumpSide = LastWallJumpSide.Left;
        }

        // same check for wall being on right side
        else if (jump && GroundCheck(Vector2.right) && lastWallJumpSide != LastWallJumpSide.Right)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
            lastWallJumpSide = LastWallJumpSide.Right;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.name == "Terrain")
        {
            print("not jumping");
            animator.SetBool("Jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            // show the "collected animation" when picking up a fruit
            var collect = Instantiate(
                collectPrefab,
                collision.gameObject.transform.position,
                Quaternion.identity);
            Destroy(collect, 0.5f);

            Destroy(collision.gameObject);
        }
    }

    bool GroundCheck(Vector2 vector, float distance = 0.5f)
    {
        return Physics2D.CapsuleCast(
            bc.bounds.center, bc.bounds.size,
            0f, 0f,
            vector, distance, groundLayer);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fan"))
        {
            float distX = collision.gameObject.transform.position.x - gameObject.transform.position.x;
            if (Mathf.Abs(distX) < 0.5f)
                distX = Mathf.Sign(distX) * 0.5f;

            float distY = Mathf.Abs(collision.gameObject.transform.position.y
                - gameObject.transform.position.y);
            float forceY = jumpPower / (7 * distY * distY);
            float forceX = -jumpPower / (4 * distX);

            if ((distY < 3) && (rb.velocity.y < 0))
                forceY *= 3;

            if (Mathf.Abs(forceY) > 450)
                forceY = Mathf.Sign(forceY) * 450;

            rb.AddForce(new Vector2(forceX, forceY));
        }
    }
}
