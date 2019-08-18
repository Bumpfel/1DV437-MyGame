using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class PlayerMovement : MonoBehaviour {
    
    public enum MovementControl { CameraRelativeMovement, CharacterRelativeMovement };
    
    public MovementControl m_MovementControl = MovementControl.CameraRelativeMovement;
    public float m_WalkSpeed = 5;
    public float m_RunSpeedModifier = 1.8f;

    private float m_CollisionCheckRadius;
    public Transform m_PlayerModel;
    private Vector3 m_MoveTo;
    private float m_MovementDistance;
    private Camera m_ViewCamera;
    public Canvas m_AimReticle;
    private Animator m_Animator;
    private Combatant m_Combatant;
    private LayerMask m_ObstacleMask;

    private string m_VerticalAxis;
    private string m_LookAxisX;
    private string m_LookAxisY;
    private string m_HorizontalAxis;
    private string m_SprintKey;
    
    private const float SpeedSmoothTime = .1f;
    private float m_SpeedSmoothVelocity;
    private float m_CurrentSpeed;
    private readonly Vector3 CollisionCheckPoint = Vector3.up * 1.5f;


    // method vars
    private float actualSpeed;
    private Vector3 prevPosition;
    private float SpeedMultiplier = 5.5f;
    private float moveSpeed;
    private float targetSpeed;
    private Vector3 movementInput;

    private float animationSpeedPercent;
      
    public bool IsRunning => Input.GetButton(m_SprintKey) && (Input.GetAxisRaw(m_VerticalAxis) != 0 || Input.GetAxisRaw(m_HorizontalAxis) != 0);

    private void Start() {
        m_ViewCamera = Camera.main;

        m_Animator = GetComponent<Animator>();

        m_VerticalAxis = Controls.Vertical.ToString();
        m_HorizontalAxis = Controls.Horizontal.ToString();
        m_SprintKey = Controls.Sprint.ToString();

        m_Combatant = GetComponent<Combatant>();

        m_ObstacleMask = LayerMask.GetMask("Obstacles");
        m_CollisionCheckRadius = GetComponent<CapsuleCollider>().radius * .8f;

        prevPosition = transform.position;

        // if(m_MovementControl == MovementControl.CharacterRelativeMovement) {
        //     m_AllowMovement = AllowCharacterRelativeMovement;
        // }
        // else {
        //     m_AllowMovement = AllowCameraRelativeMovement;
        // }
    }

    private void FixedUpdate() {
        // SmoothCameraRelativeMovement();
        CameraRelativeMovementActualSpeed();

        if(!transform.position.Equals(m_MoveTo)) {
            Collider[] colliders = Physics.OverlapSphere(transform.position + CollisionCheckPoint, m_CollisionCheckRadius, m_ObstacleMask);
            // Collider[] collidedWithWeapon = Physics.OverlapBox(transform.position + transform.forward * .9f + transform.right * .25f, new Vector3(.25f / 2, .4f / 2, 1.4f / 2), transform.rotation, m_ObstacleMask);
            if(colliders.Length == 0 || colliders[0].isTrigger) {// && collidedWithWeapon.Length == 0)
                transform.position += m_MoveTo * Time.fixedDeltaTime;
            }
        }
    }

    private void Update() {
        if(Time.timeScale == 0)
            return;
        Look();
        // if(m_MovementControl == MovementControl.CharacterRelativeMovement)
            // CharacterRelativeMovement();
        // else
            // SmoothCameraRelativeMovement();
            // SmoothCharacterRelativeMovement();
            // CameraRelativeMovement();
    }

   private void CameraRelativeMovementActualSpeed() { // animates the character according to actual movement speed
        actualSpeed = Vector3.Distance(prevPosition, transform.position) * SpeedMultiplier;
    
        moveSpeed = m_WalkSpeed;
        movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;
        targetSpeed = (IsRunning ? m_WalkSpeed * m_RunSpeedModifier : m_WalkSpeed) * movementInput.magnitude;
        // m_CurrentSpeed = Mathf.SmoothDamp(m_CurrentSpeed, targetSpeed, ref m_SpeedSmoothVelocity, SpeedSmoothTime);
        
        m_Animator.SetFloat(AnimatorSettings.speedPercent.ToString(), actualSpeed, SpeedSmoothTime, Time.fixedDeltaTime);

        if(Input.GetButton(m_SprintKey)) {
            moveSpeed *= m_RunSpeedModifier;
        }
		m_MoveTo = movementInput * moveSpeed;

        prevPosition = transform.position;
    }


   private void SmoothCameraRelativeMovement() { // has smooth animations
        moveSpeed = m_WalkSpeed;
        movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;
        targetSpeed = (IsRunning ? m_WalkSpeed * m_RunSpeedModifier : m_WalkSpeed) * movementInput.magnitude;
        m_CurrentSpeed = Mathf.SmoothDamp(m_CurrentSpeed, targetSpeed, ref m_SpeedSmoothVelocity, SpeedSmoothTime);
    
        animationSpeedPercent = (IsRunning ? 1 : 0.5f) * movementInput.magnitude;
        m_Animator.SetFloat(AnimatorSettings.speedPercent.ToString(), animationSpeedPercent, SpeedSmoothTime, Time.fixedDeltaTime);

        if(Input.GetButton(m_SprintKey)) {
            moveSpeed *= m_RunSpeedModifier;
        }

        transform.Translate(transform.forward * m_CurrentSpeed * Time.fixedDeltaTime);
		// m_MoveTo = movementInput * moveSpeed;
    }

       private void SmoothCharacterRelativeMovement() {
        moveSpeed = m_WalkSpeed;
        movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;
        targetSpeed = (IsRunning ? m_WalkSpeed * m_RunSpeedModifier : m_WalkSpeed) * movementInput.magnitude;
        m_CurrentSpeed = Mathf.SmoothDamp(Input.GetAxisRaw(m_VerticalAxis), targetSpeed, ref m_SpeedSmoothVelocity, SpeedSmoothTime);
        //----

        float animationSpeedPercent = (IsRunning ? 1 : 0.5f) * movementInput.magnitude;
        m_Animator.SetFloat(AnimatorSettings.speedPercent.ToString(), animationSpeedPercent, SpeedSmoothTime, Time.fixedDeltaTime);


        transform.Translate(transform.forward * m_CurrentSpeed * Time.fixedDeltaTime);
        // if(Input.GetButton(m_SprintKey)) {
        //     moveSpeed *= m_RunModifier;
        // }
		// m_MoveTo = movementInput * moveSpeed;
    }

    private void Look() {
        Vector3 mousePosInWorld = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y);
        Vector3 mousePosRelativeToCamera = m_ViewCamera.ScreenToWorldPoint(mousePosInWorld);
        
        m_PlayerModel.LookAt(mousePosRelativeToCamera + Vector3.up * transform.position.y);
        
        //placing reticle on top (5 units up), compensating for aim reticle size, so the bullet is fired at the center of the reticle
        m_AimReticle.transform.position = mousePosRelativeToCamera /* + Vector3.up * 5 */ + transform.right * .23f; // .23 for perspective, .25 for ortographic 
    }

}