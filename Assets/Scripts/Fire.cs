using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] GameObject On;
    [SerializeField] GameObject Off;

    // Start is called before the first frame update
    void Start()
    {
        On.SetActive(false);
        Off.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        On.SetActive(true);
        Off.SetActive(false);
    }
}
