using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Controls the player: jump, dash, lives, footsteps and animations
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 14f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;
    public float jumpBufferTime = 0.15f;   // how long a jump press is remembered

    [Header("Dash")]
    public float dashSpeedMultiplier = 2f;
    public float dashDuration = 0.4f;
    public float dashCooldown = 1.5f;
    public TrailRenderer dashTrail;   // optional

    [Header("Health")]
    public int maxLives = 3;
    public float invincibleTime = 1.2f;
    public Color hitColor = new Color(1f, 0.3f, 0.3f);

    [Header("Footsteps")]
    public float stepInterval = 0.3f;   // time between two steps

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    InputAction jumpAction;
    InputAction dashAction;

    int lives;
    bool isGrounded;
    bool isDashing;
    bool isHitInvincible;
    bool isDead;
    float nextDashTime;
    float jumpBufferCounter;   // time left on the remembered jump press
    float lastJumpTime;        // prevents double jumps right after jumping
    float stepTimer;

    // The player takes no damage while dashing or right after being hit.
    // To take damage while dashing, change this line to: bool IsInvincible => isHitInvincible;
    bool IsInvincible => isDashing || isHitInvincible;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // The player only moves up and down, the world is what moves
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Define inputs with the new Input System
        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        jumpAction.performed += ctx => Jump();

        dashAction = new InputAction("Dash", InputActionType.Button, "<Keyboard>/leftShift");
        dashAction.AddBinding("<Keyboard>/rightShift");
        dashAction.performed += ctx => Dash();
    }

    void OnEnable()
    {
        jumpAction.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        jumpAction.Disable();
        dashAction.Disable();
    }

    void Start()
    {
        lives = maxLives;
        UIManager.Instance.UpdateHealth(lives, maxLives);
        if (dashTrail != null) dashTrail.emitting = false;
    }

    void Update()
    {
        // Check if the player is standing on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        anim.SetBool("IsGrounded", isGrounded);

        // The player stays in Idle in the main menu and starts running when the game starts
        anim.SetBool("GameStarted", GameManager.Instance.IsPlaying);

        // Count down the remembered jump press
        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;

        // Jump as soon as we are on the ground and a jump press is remembered
        if (jumpBufferCounter > 0f && isGrounded && CanAct() && Time.time > lastJumpTime + 0.1f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            AudioManager.Instance.PlayJump();
            lastJumpTime = Time.time;
            jumpBufferCounter = 0f;
        }

        HandleFootsteps();
    }

    // Input is ignored when dead, paused, in the menu or after game over
    bool CanAct()
    {
        GameManager gm = GameManager.Instance;
        return !isDead && gm.IsPlaying && !gm.IsPaused;
    }

    // Only remembers the press, Update does the actual jump
    void Jump()
    {
        if (!CanAct()) return;
        jumpBufferCounter = jumpBufferTime;
    }

    void Dash()
    {
        if (!CanAct() || isDashing || Time.time < nextDashTime) return;
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        AudioManager.Instance.PlayDash();
        anim.SetBool("IsDashing", true);
        nextDashTime = Time.time + dashCooldown;
        GameManager.Instance.SpeedMultiplier = dashSpeedMultiplier;
        if (dashTrail != null) dashTrail.emitting = true;

        yield return new WaitForSeconds(dashDuration);

        GameManager.Instance.SpeedMultiplier = 1f;
        if (dashTrail != null) dashTrail.emitting = false;
        isDashing = false;
        anim.SetBool("IsDashing", false);
    }

    // Plays a step sound at a regular interval while running on the ground
    void HandleFootsteps()
    {
        GameManager gm = GameManager.Instance;
        bool running = isGrounded && !isDead && gm.IsPlaying && !gm.IsPaused;

        if (!running)
        {
            stepTimer = 0f;   // the first step plays right when we land
            return;
        }

        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0f)
        {
            AudioManager.Instance.PlayStep();
            stepTimer = stepInterval / gm.SpeedMultiplier;   // steps are faster while dashing
        }
    }

    // Obstacles call this method when they touch the player
    public void TakeDamage(int amount)
    {
        if (isDead || IsInvincible) return;

        lives = Mathf.Max(0, lives - amount);
        UIManager.Instance.UpdateHealth(lives, maxLives);

        if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, 0.2f);

        if (lives <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
            AudioManager.Instance.PlayHit();
            StartCoroutine(HitRoutine());
        }
    }

    // Flash red and stay invincible for a short time after being hit
    IEnumerator HitRoutine()
    {
        isHitInvincible = true;
        float t = 0f;
        while (t < invincibleTime)
        {
            sr.color = hitColor;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            t += 0.2f;
        }
        sr.color = Color.white;
        isHitInvincible = false;
    }

    void Die()
    {
        isDead = true;
        AudioManager.Instance.PlayDeath();
        StopAllCoroutines();
        sr.color = Color.white;
        if (dashTrail != null) dashTrail.emitting = false;
        anim.SetBool("IsDashing", false);
        anim.SetTrigger("Death");
        GameManager.Instance.GameOver();
    }

    // Draws the ground check circle in the Scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}