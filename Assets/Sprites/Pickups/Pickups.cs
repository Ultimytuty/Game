using UnityEngine;

public class Pickups : MonoBehaviour
{
    public int playerLayer;
    private AK47main ak47;

    void Start()
    {
        ak47 = FindObjectOfType<AK47main>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            ak47.totalAmmo += 30;
            Destroy(gameObject);
        }
    }
}