using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    public InputActionReference MoveAction;
    public InputActionReference DashAction;

    public float walkSpeed = 3.0f;
    public float turnSpeed = 8f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    [Header("Dash Settings")]
    public float maxChargeTime = 1.0f;     
    public float minDashDistance = 2.0f;  
    public float maxDashDistance = 3.0f;   
    public float dashCooldown = 2.0f;       

    [Header("Dash UI")]
    public Slider dashCooldownBar;

    bool isCharging;
    bool isOnCooldown;
    float chargeStartTime;
    float cooldownTimer;

    Vector3 pendingDashOffset;
    bool hasPendingDash;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (MoveAction != null)
        {
            MoveAction.action.Enable();
        }

        if (DashAction != null)
        {
            DashAction.action.Enable();
            DashAction.action.started += OnDashStarted;
            DashAction.action.canceled += OnDashCanceled;
        }

        if (dashCooldownBar != null)
        {
            dashCooldownBar.minValue = 0f;
            dashCooldownBar.maxValue = 1f;
            dashCooldownBar.value = 0f;
            dashCooldownBar.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (DashAction != null)
        {
            DashAction.action.started -= OnDashStarted;
            DashAction.action.canceled -= OnDashCanceled;
            DashAction.action.Disable();
        }

        if (MoveAction != null)
        {
            MoveAction.action.Disable();
        }
    }

    void Update()
    {
        HandleCooldown();
    }

    void FixedUpdate()
    {
        Vector2 moveInput = Vector2.zero;

        if (MoveAction != null)
        {
            moveInput = MoveAction.action.ReadValue<Vector2>();
        }

         if (moveInput != Vector2.zero)
        {
    Debug.Log("Move input: " + moveInput);
        }

        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        if (m_Animator != null)
        {
            m_Animator.SetBool("IsWalking", isWalking);
        }

       
        if (m_Movement.sqrMagnitude > 0.001f)
        {
            Vector3 desiredForward = Vector3.RotateTowards(
                transform.forward,
                m_Movement,
                turnSpeed * Time.deltaTime,
                0f
            );

            m_Rotation = Quaternion.LookRotation(desiredForward);
        }

       
        Vector3 newPosition = m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime;

        
        if (hasPendingDash)
        {
            newPosition += pendingDashOffset;
            hasPendingDash = false;
            pendingDashOffset = Vector3.zero;
        }

        m_Rigidbody.MovePosition(newPosition);

        if (m_Movement.sqrMagnitude > 0.001f)
        {
            m_Rigidbody.MoveRotation(m_Rotation);
        }
    }

    void OnDashStarted(InputAction.CallbackContext ctx)
    {
        if (isOnCooldown)
            return;

        isCharging = true;
        chargeStartTime = Time.time;
    }

    void OnDashCanceled(InputAction.CallbackContext ctx)
    {
        if (!isCharging || isOnCooldown)
            return;

        isCharging = false;

        float chargeTime = Mathf.Clamp(Time.time - chargeStartTime, 0f, maxChargeTime);
        float chargePercent = maxChargeTime > 0f ? chargeTime / maxChargeTime : 1f;
        chargePercent = Mathf.Clamp01(chargePercent);

        PerformDash(chargePercent);
    }

    void PerformDash(float chargePercent)
    {
        float dashDistance = Mathf.Lerp(minDashDistance, maxDashDistance, chargePercent);

        Vector3 dashDirection = m_Movement.sqrMagnitude > 0.01f ? m_Movement : transform.forward;
        dashDirection.y = 0f;
        dashDirection.Normalize();

        pendingDashOffset = dashDirection * dashDistance;
        hasPendingDash = true;

        isOnCooldown = true;
        cooldownTimer = 0f;

        if (dashCooldownBar != null)
        {
            dashCooldownBar.value = 0f;
            dashCooldownBar.gameObject.SetActive(true);
        }
    }

    void HandleCooldown()
    {
        if (!isOnCooldown)
            return;

        cooldownTimer += Time.deltaTime;

        float t = dashCooldown > 0f ? cooldownTimer / dashCooldown : 1f;
        t = Mathf.Clamp01(t);

        if (dashCooldownBar != null)
        {
            dashCooldownBar.value = t;
        }

        if (cooldownTimer >= dashCooldown)
        {
            isOnCooldown = false;
            cooldownTimer = 0f;

            if (dashCooldownBar != null)
            {
                dashCooldownBar.gameObject.SetActive(false);
                dashCooldownBar.value = 0f;
            }
        }
    }
}
