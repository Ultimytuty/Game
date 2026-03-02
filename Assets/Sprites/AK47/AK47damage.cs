using UnityEngine;

public class AK47damage : MonoBehaviour
{
    public float damageAmount = 20f; // Amount of damage the bullet will deal

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            HealthInternational enemyHealth = collision.GetComponent<HealthInternational>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Mathf.RoundToInt(damageAmount), enemyHealth.isDefending); // Pass the defending state to calculate damage
                Destroy(gameObject); // Destroy the bullet after hitting an enemy
            }
        }
    }
}
