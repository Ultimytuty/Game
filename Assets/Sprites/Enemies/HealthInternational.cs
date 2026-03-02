using UnityEngine;
using System.Collections;
using System;

public class HealthInternational : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public float defancePrecentage = 0.5f; // 50% damage reduction when defending
    public Color colorWhenDamaged = Color.red; // Color to flash when damaged
    private SpriteRenderer spriteRenderer;
    public bool isDefending = true; // Flag to indicate if the character is currently defending
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        col.enabled = true;
    }

    public void TakeDamage(int damage, bool isDefending)
    {
        int finalDamage = isDefending ? Mathf.RoundToInt(damage * (1 - defancePrecentage)) : damage;
        currentHealth -= finalDamage;
        spriteRenderer.color = colorWhenDamaged; // Flash color when damaged
        Invoke("ResetColor", 0.1f); // Reset color after a short delay
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FreezeAllConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void ResetColor()
    {
        spriteRenderer.color = Color.white; // Reset to default color
    }

    public void Die()
    {
        if (isDead) return; // Prevent multiple death triggers
        isDead = true;
        animator.SetTrigger("Die");
        FreezeAllConstraints();
        Debug.Log(gameObject.name + " has died.");
        Invoke("DeactivateObject", 2f); // Wait for animation, then disable
    }

    private void DeactivateObject()
    {
    gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
