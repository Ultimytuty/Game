using System;
using UnityEngine;
using random = UnityEngine.Random; 

public class bulletScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    //MIGHT DELETE LATER
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.orange;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * AK47main.bulletSpeed);
        if (transform.position.x > 70f || transform.position.x < -70f || transform.position.y > 50f || transform.position.y < -50f)
        {
            Destroy(gameObject);
        }
        
    }
}
