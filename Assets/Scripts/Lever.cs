using UnityEngine;

public class Lever : MonoBehaviour
{
    public Sprite SpriteOn;
    // public 

    void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<SpriteRenderer>().sprite = SpriteOn;
    }
}
