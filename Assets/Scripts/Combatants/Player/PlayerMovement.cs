using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    
    public enum MovementControl { CameraRelativeMovement, CharacterRelativeMovement };
    
    public MovementControl m_MovementControl = MovementControl.CameraRelativeMovement;
    public int m_PlayerNumber = 1; // TODO temp
    public float m_WalkSpeed = 5;
    public float m_StrafeSpeed = 4;
    public int m_RotationSpeed = 200;
    public float m_RunModifier = 2f;

    private Vector3 m_MoveTo;
    
    private Camera m_ViewCamera;

    public Canvas m_AimReticle;
    // private Rigidbody m_Body;
    private Animator m_Animator;
    private bool m_IsRunning = false;
    private bool m_IsWalking = false;
    private bool m_IsStrafingLeft = false;
    private bool m_IsStrafingRight = false;
    // private readonly float m_TURN_WAIT = 0.2f;
    private float m_TurnTimestamp;

    private string m_VerticalAxis;
    // private string m_TurnAxis;
    private string m_LookAxisX;
    private string m_LookAxisY;
    private string m_HorizontalAxis;
    private string m_SprintKey;
    
    private Combatant m_Combatant;

    [HideInInspector]
    public bool m_GamePaused = false;


    private float speedSmoothTime = .1f;
    private float speedSmoothVelocity;
    private float currentSpeed;

    void Start() {
        m_ViewCamera = Camera.main;

        m_Animator = GetComponent<Animator>();

        m_VerticalAxis = "Vertical_Player" + m_PlayerNumber;
        m_HorizontalAxis = "Horizontal_Player" + m_PlayerNumber;
        m_SprintKey = "Sprint_Player" + m_PlayerNumber;

        Cursor.visible = false;

        m_Combatant = GetComponent<Combatant>();

        // print(m_AimReticle.GetComponentInChildren<Image>().transform.localScale);

        // if(m_MovementControl == MovementControl.CharacterRelativeMovement) {
        //     m_AllowMovement = AllowCharacterRelativeMovement;
        // }
        // else {
        //     m_AllowMovement = AllowCameraRelativeMovement;
        // }
    }

    void FixedUpdate() {
        if(!m_Combatant.IsDead() && !m_GamePaused) {
            transform.position = transform.position + m_MoveTo * Time.fixedDeltaTime;
        }
    }


    void Update() {
        if(!m_Combatant.IsDead() && !m_GamePaused) {
            Look();

            if(m_MovementControl == MovementControl.CharacterRelativeMovement)
                CharacterRelativeMovement();
            else
                SmoothCameraRelativeMovement();
                // SmoothCharacterRelativeMovement();
                // CameraRelativeMovement();
        }
    }

    private void CameraRelativeMovement() { // not currently used
        Vector3 mousePos = m_ViewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
        float moveSpeed = m_WalkSpeed;

        Vector3 movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;
        float speedPercent = (IsRunning() ? 1 : 0.5f) * movementInput.magnitude;
        m_Animator.SetFloat("speedPercent", speedPercent);
 
        if(Input.GetButton(m_SprintKey)) {
            moveSpeed *= m_RunModifier;
        }
		m_MoveTo = movementInput * moveSpeed;
    }

   private void SmoothCameraRelativeMovement() { // has smooth animations
        Vector3 mousePos = m_ViewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
        float moveSpeed = m_WalkSpeed;

        Vector3 movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;

        float targetSpeed = (IsRunning() ? m_WalkSpeed * m_RunModifier : m_WalkSpeed) * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        // transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime);
        
        float animationSpeedPercent = (IsRunning() ? 1 : 0.5f) * movementInput.magnitude;
        m_Animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.fixedDeltaTime);

        if(Input.GetButton(m_SprintKey)) {
            moveSpeed *= m_RunModifier;
        }
		m_MoveTo = movementInput * moveSpeed;
    }

       private void SmoothCharacterRelativeMovement() {
        Vector3 mousePos = m_ViewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
        // float moveSpeed = m_WalkSpeed;

        Vector3 movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;

        float targetSpeed = (IsRunning() ? m_WalkSpeed * m_RunModifier : m_WalkSpeed) * movementInput.magnitude;
        currentSpeed = Mathf.SmoothDamp(Input.GetAxisRaw(m_VerticalAxis), targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        //----

        float animationSpeedPercent = (IsRunning() ? 1 : 0.5f) * movementInput.magnitude;
        m_Animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.fixedDeltaTime);


        transform.Translate(transform.forward * currentSpeed * Time.fixedDeltaTime);
        // if(Input.GetButton(m_SprintKey)) {
        //     moveSpeed *= m_RunModifier;
        // }
		// m_MoveTo = movementInput * moveSpeed;
    }

    private void Look() {
        Vector3 mousePosInWorld = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y);
        Vector3 mousePosRelativeToCamera = m_ViewCamera.ScreenToWorldPoint(mousePosInWorld);
        transform.LookAt(mousePosRelativeToCamera + Vector3.up * transform.position.y);
        
        //placing reticle on top (5 units up), compensating for aim reticle size, so the bullet is fire at the center of the reticle
        m_AimReticle.transform.position = mousePosRelativeToCamera + Vector3.up * 5 + transform.right * .23f;//.25f; // .23 for perspective, .25 for ortographic 
    }


    public bool IsRunning() {
        return Input.GetButton(m_SprintKey) && (Input.GetAxisRaw(m_VerticalAxis) != 0 || Input.GetAxisRaw(m_HorizontalAxis) != 0);
    }










    private void CharacterRelativeMovement() {
        float movementInput = Input.GetAxisRaw(m_VerticalAxis);

        Vector3 forwardMovement = transform.forward * movementInput * m_WalkSpeed;
        if(Input.GetAxisRaw(m_SprintKey) > 0)
            forwardMovement *= m_RunModifier;


        if(movementInput != 0) {           
            if(Input.GetAxisRaw(m_SprintKey) > 0) {
                if(!m_IsRunning && movementInput > 0) {
                    m_Animator.Play("Run_Guard");
                    m_IsRunning = true;
                }
            }
            else {
                forwardMovement = transform.forward * movementInput * m_WalkSpeed;

                if(!m_IsWalking) {
                    m_Animator.Play("WalkForward_Shoot");
                    m_IsWalking = true;
                }
            }                
        }
        else if(m_IsRunning || m_IsWalking) {
            m_Animator.Play("Idle_Shoot");
            m_IsRunning = false;            
            m_IsWalking = false;
        }


        //strafe
        float strafe = Input.GetAxisRaw(m_HorizontalAxis);

        Vector3 strafeMovement = transform.right * strafe * m_StrafeSpeed;
        if(strafe != 0) {
            // m_Body.MovePosition(m_Body.position + strafeMovement);

            if(strafe < 0 && !m_IsStrafingLeft) {
                m_Animator.Play("WalkLeft_Shoot");
                m_IsStrafingLeft = true;
            }
            else if(strafe > 0 && !m_IsStrafingRight) {
                m_Animator.Play("WalkRight_Shoot");
                m_IsStrafingRight = true;
            }
        }
        else if(m_IsStrafingLeft || m_IsStrafingRight) {
            m_Animator.Play("Idle_Shoot");
            m_IsStrafingLeft = false;
            m_IsStrafingRight = false;
        }
        m_MoveTo = forwardMovement + strafeMovement;
    }



    //unused right now
    // private void KBTurn() {
    //     float turn = Input.GetAxisRaw(m_TurnAxis);

    //     if(turn != 0) {
    //         Quaternion turnRotation = Quaternion.Euler(0, turn * m_RotationSpeed * Time.deltaTime, 0);
    //         m_Body.MoveRotation(m_Body.rotation * turnRotation);
    //     }
    // }

}
