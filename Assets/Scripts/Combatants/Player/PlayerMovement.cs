using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    
    public enum MovementControl { CameraRelativeMovement, CharacterRelativeMovement };
    
    public MovementControl m_MovementControl = MovementControl.CameraRelativeMovement;
    public int m_PlayerNumber = 1; // TODO temp
    public float m_MovementSpeed = 5;
    public float m_StrafeSpeed = 4;
    public int m_RotationSpeed = 200;
    public float m_RunModifier = 2f;

    private Vector3 m_MoveTo;
    
    private Camera m_ViewCamera;

    private Rigidbody m_Body;
    private Animator m_Animator;

    //TODO tanke - hämta tiden på animationen istället för booleans
    private bool m_IsRunning = false;
    private bool m_IsWalking = false;
    private bool m_IsStrafingLeft = false;
    private bool m_IsStrafingRight = false;
    private readonly float m_TURN_WAIT = 0.2f;
    private float m_TurnTimestamp;

    private string m_VerticalAxis;
    private string m_TurnAxis;
    private string m_HorizontalAxis;
    private string m_SprintKey;
    
    void Start() {
        m_ViewCamera = Camera.main;

        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_Shoot");
        m_Body = GetComponent<Rigidbody>();


        m_VerticalAxis = "Vertical_Player" + m_PlayerNumber; //TODO temp if only 1 player
        // m_TurnAxis = "Turn_Player" + playerNumber;
        m_HorizontalAxis = "Horizontal_Player" + m_PlayerNumber;
        m_SprintKey = "Sprint_Player" + m_PlayerNumber;
    }

    //FixedUpdate is called in fixed intervals (by default every 0.02 secs - 50 times/second)
    void FixedUpdate() {
        m_Body.MovePosition(m_Body.position + m_MoveTo * Time.fixedDeltaTime);
        // Move();
        // KBTurn();
        // SharpTurn();
    }


    // Update is called once per frame
    void Update() {
        MouseTurn();

        if(m_MovementControl == MovementControl.CharacterRelativeMovement)
            CharacterRelativeMovement();
        else
            CameraRelativeMovement();

    }

    private void CharacterRelativeMovement() {
        float movementInput = Input.GetAxisRaw(m_VerticalAxis);

        Vector3 forwardMovement = transform.forward * movementInput * m_MovementSpeed;
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
                forwardMovement = transform.forward * movementInput * m_MovementSpeed;

                if(!m_IsWalking) {
                    m_Animator.Play("WalkForward_Shoot");
                    m_IsWalking = true;
                }
            }
           
        //    m_MoveTo = m_Body.position + movement;
            // m_Body.MovePosition(m_Body.position + movement);
                
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


    private void CameraRelativeMovement() {
        Vector3 mousePos = m_ViewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
		m_MoveTo = new Vector3 (Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized * m_MovementSpeed;
    }


    private void MouseTurn() {
        Vector3 mousePos = m_ViewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ViewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);
    }





    //unused right now
    private void KBTurn() {
        float turn = Input.GetAxisRaw(m_TurnAxis);

        if(turn != 0) {
            Quaternion turnRotation = Quaternion.Euler(0, turn * m_RotationSpeed * Time.deltaTime, 0);
            m_Body.MoveRotation(m_Body.rotation * turnRotation);
        }
    }

    private void SharpTurn() {
        if(Time.time > m_TURN_WAIT + m_TurnTimestamp) {
            Quaternion turnRotation;
            if(Input.GetKey(KeyCode.LeftArrow)) {
                turnRotation = Quaternion.Euler(0, -90, 0);
                m_Body.MoveRotation(m_Body.rotation  * turnRotation);
            }
            else if(Input.GetKey(KeyCode.RightArrow)) {
                turnRotation = Quaternion.Euler(0, 90, 0);
                m_Body.MoveRotation(m_Body.rotation  * turnRotation);
            }
            m_TurnTimestamp = Time.time;
        }
    } 


}
