using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;
    public InputAction MoveAction;

    public InputAction BoostAction;
    public float boostMultiplier = 2f;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;

    public bool hasShield = false;
    public GameObject shieldVisual;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

   
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        BoostAction.Enable();
        m_Animator = GetComponent<Animator>();  
    }

    void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        bool isBoosting = BoostAction.IsPressed();

        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * (isBoosting ? walkSpeed * boostMultiplier : walkSpeed) * Time.deltaTime);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
    public void ActivateShield()
    {
        hasShield = true;
        if (shieldVisual != null)
            shieldVisual.SetActive(true);
    }
    public void DeactivateShield()
    {
        hasShield = false;
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ghost"))
        {
            if (hasShield)
            {
                Destroy(collision.gameObject);
                DeactivateShield();             
            }
            else
            {
                Debug.Log("Player touched by ghost (no shield)!");
            }
        }
    }
}
