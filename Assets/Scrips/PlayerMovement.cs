using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    public float normalSpeed = 5f;
    public float sprintSpeed = 10f;

    [Header("Sprint")]
    public float sprintDuration = 2.5f;
    public float sprintCooldown = 5f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator anim;
    public Text apText;

    public int facingDirection = 1;

    private float currentSpeed;
    private float horizontal;
    private float vertical;

    private bool isSprinting = false;
    private bool isCoolingDown = false;

    private float sprintTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        currentSpeed = normalSpeed;
        UpdateAPText();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if ((horizontal > 0 && transform.localScale.x < 0) || (horizontal < 0 && transform.localScale.x > 0))
        {
            Flip();
        }

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && !isCoolingDown)
        {
            StartSprint();
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0f)
            {
                EndSprint();
            }
        }

        if (isCoolingDown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= sprintCooldown)
            {
                cooldownTimer = sprintCooldown;
                isCoolingDown = false;
            }

            UpdateAPText();
        }
        else if (!isSprinting)
        {
            UpdateAPText();
        }
    }

    void FixedUpdate()
    {
        Vector2 move = new Vector2(horizontal, vertical);

        if (move.magnitude > 1f)
        {
            move = move.normalized;
        }

        rb.velocity = move * currentSpeed;
    }

    void StartSprint()
    {
        isSprinting = true;
        sprintTimer = sprintDuration;
        currentSpeed = sprintSpeed;

        UpdateAPText();
    }

    void EndSprint()
    {
        isSprinting = false;
        currentSpeed = normalSpeed;

        isCoolingDown = true;
        cooldownTimer = 0f;

        UpdateAPText();
    }

    void UpdateAPText()
    {
        if (apText == null) return;

        int maxAP = Mathf.RoundToInt(sprintCooldown);

        if (isSprinting)
        {
            apText.text = $"AP:  0/{maxAP}";
            return;
        }

        if (isCoolingDown)
        {
            int currentAP = Mathf.FloorToInt(cooldownTimer);

            if (currentAP > maxAP)
            {
                currentAP = maxAP;
            }

            apText.text = $"AP:  {currentAP}/{maxAP}";
        }
        else
        {
            apText.text = $"AP:  {maxAP}/{maxAP}";
        }
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y,
            transform.localScale.z
        );
    }
}