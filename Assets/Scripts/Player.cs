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
    bool isDead;

    enum LastWallJumpSide
    {
        None,
        Left,
        Right,
    }
    LastWallJumpSide lastWallJumpSide = LastWallJumpSide.None;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isDead)
            return;

        float dir = Input.GetAxisRaw("Horizontal");

        if (dir < 0 && !GroundCheck(Vector2.left, 0.05f))
        {
            transform.Translate(dir * Speed * Time.deltaTime, 0, 0);
            sr.flipX = true;
        }
        else if (dir > 0 && !GroundCheck(Vector2.right, 0.05f))
        {
            transform.Translate(dir * Speed * Time.deltaTime, 0, 0);
            sr.flipX = false;
        }

        bool jump = Input.GetButtonDown("Jump");
        if (jump && GroundCheck(Vector2.down))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            lastWallJumpSide = LastWallJumpSide.None;
        } 
        else if (jump && GroundCheck(Vector2.left) && lastWallJumpSide != LastWallJumpSide.Left)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            lastWallJumpSide = LastWallJumpSide.Left;
        }
        else if (jump && GroundCheck(Vector2.right) && lastWallJumpSide != LastWallJumpSide.Right)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
            lastWallJumpSide = LastWallJumpSide.Right;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fruit"))
        {
            var collect = Instantiate(
                collectPrefab,
                collision.gameObject.transform.position,
                Quaternion.identity);
            Destroy(collect, 0.5f);

            Destroy(collision.gameObject);
        }
    }

    bool GroundCheck(Vector2 vector, float distance = 0.5f) // <--
    {
        return Physics2D.CapsuleCast(
            bc.bounds.center, bc.bounds.size,
            0f, 0f,
            vector, distance, groundLayer); // <--
    }
}
