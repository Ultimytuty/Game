using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AK47main : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float fireRate = 0.1f;
    public static float bulletSpeed = 25f;
    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;
    Animator animator;
    public bool aimAtMouse = true;
    public int maxAmmo = 30;           // maximum ammo capacity
    public int ammoCount = 30;         // current ammo in magazine
    public int totalAmmo = 90;         // reserve ammo carried
    public float reloadTime = 2f;      // seconds to reload
    private bool isReloading = false;
    public float knockbackForce = 5f;  // knockback strength when shooting
    public float knockbackHorizontalScale = 0.4f;  // reduces horizontal knockback (0-1, lower = less horizontal)
    private AudioSource audioSource;
    public AudioClip ak47ShootClip;    // assign in inspector
    public CameraShaker cameraShaker;

    // UI
    public TextMeshProUGUI ammoUIText;           // assign in inspector to display counts

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("AK47 main script has started.");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (aimAtMouse)
        {
            AimAtMouse();
        }
        else
        {
            // Flip sprite based on movement direction (if not aiming at mouse)
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = transform.localScale.x < 0;
            }
        }
        
        bool isHeld = false;
        // handle reload input
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCoroutine(Reload());
        }
#else
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
#endif
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Mouse.current != null)
        {
            isHeld = Mouse.current.leftButton.isPressed;
        }
#else
        isHeld = Input.GetMouseButton(0); // Left mouse button held
#endif

        if (isHeld && Time.time >= nextFireTime && ammoCount > 0 && !isReloading)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    private void AimAtMouse()
    {
        // Get mouse position in world space using new Input System
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 10f; // Distance from camera
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        
        // Calculate direction from gun to mouse
        Vector2 direction = (worldMousePos - transform.position).normalized;
        
        // Calculate angle to face mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Flip sprite based on mouse side
        if (spriteRenderer != null)
        {
            spriteRenderer.flipY = direction.x < 0;
        }
    }
    
    void Shoot()
    {
        if (ammoCount <= 0)
        {
            Debug.Log("No ammo in magazine!");
            return;
        }

        ammoCount--;
        Debug.Log($"AK47 fired! Magazine: {ammoCount}/{maxAmmo}, Reserve: {totalAmmo}");
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        animator.SetTrigger("Shoot");
        if (audioSource != null && ak47ShootClip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // random pitch variation (±5%)
            audioSource.PlayOneShot(ak47ShootClip);
        }
        ApplyShake();
        ApplyKnockback();
        UpdateAmmoUI();

        if (ammoCount <= 0 && totalAmmo > 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        if (isReloading || ammoCount == maxAmmo || totalAmmo <= 0)
            yield break;

        Debug.Log("Reloading...");
        isReloading = true;
        animator.SetTrigger("Reload"); // assumes you have a reload animation
        yield return new WaitForSeconds(reloadTime);

        int needed = maxAmmo - ammoCount;
        int transfer = Mathf.Min(needed, totalAmmo);
        ammoCount += transfer;
        totalAmmo -= transfer;

        isReloading = false;
        Debug.Log($"Reload complete. Magazine: {ammoCount}/{maxAmmo}, Reserve: {totalAmmo}");
        UpdateAmmoUI();
    }

    void ApplyKnockback()
    {
        // Get camera and rigidbody
        Camera cam = Camera.main;
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        
        if (cam == null || rb == null)
            return;

        // Get mouse position in screen space
        Vector3 mouseScreen;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Mouse.current != null)
            mouseScreen = Mouse.current.position.ReadValue();
        else
            mouseScreen = Input.mousePosition;
#else
        mouseScreen = Input.mousePosition;
#endif
        
        // Convert to world space using character's actual Z position
        Vector3 worldMousePos = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, rb.transform.position.z));
        
        // Calculate direction from character to mouse
        Vector2 fireDirection = (new Vector2(worldMousePos.x, worldMousePos.y) - rb.position).normalized;
        
        // Scale horizontal and vertical independently
        Vector2 scaledForce = new Vector2(-fireDirection.x * knockbackHorizontalScale, -fireDirection.y);
        
        // Debug: log the calculation
        Debug.Log($"Mouse world pos: {worldMousePos}, Character pos: {rb.position}, Direction: {fireDirection}");
        
        // Apply force (vertical full, horizontal scaled)
        rb.AddForce(scaledForce * knockbackForce, ForceMode2D.Impulse);
    }

    void UpdateAmmoUI()
    {
        if (ammoUIText != null)
        {
            ammoUIText.text = $"Ammo: {ammoCount}/{maxAmmo} | {totalAmmo}";
        }
    }
    
    void ApplyShake()
    {
        if (cameraShaker != null)
        {
            cameraShaker.StartShake(0.3f, 0.2f); // Shake for 0.5s with magnitude 0.4
        }
    }
}
