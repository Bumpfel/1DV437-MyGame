using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {
    
    public int playerNumber = 1; // TODO temp
    public float m_Speed = 5;
    public float m_StrafeSpeed = 4;
    public int m_RotationSpeed = 150;
    private float m_RunModifier = 2f;
    // public Text debugText;
    public GameObject m_FocusObject;

    // public Camera camera;

    // private float rotation;
    // private float directionY;
    private Rigidbody m_Body;
    private Animator m_Animator;


    //TODO tanke - hämta tiden på animationen istället för booleans
    private bool m_IsRunning = false;
    private bool m_IsWalking = false;
    private bool m_IsStrafingLeft = false;
    private bool m_IsStrafingRight = false;
    private readonly float m_TURN_WAIT = 0.2f;
    private float m_TurnTimestamp;

    private string m_MoveAxis;
    private string m_TurnAxis;
    private string m_StrafeAxis;
    private string m_SprintKey;
    
    // private float prevPrint;
    // private float printInterval = 2;

    void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_Shoot");
        m_Body = GetComponent<Rigidbody>();


        m_MoveAxis = "Move_Player" + playerNumber; //TODO temp if only 1 player
        m_TurnAxis = "Turn_Player" + playerNumber;
        m_StrafeAxis = "Strafe_Player" + playerNumber;
        m_SprintKey = "Sprint_Player" + playerNumber;
    }

    //FixedUpdate is called in fixed intervals (by default every 0.02 secs - 50 times/second)
    void FixedUpdate() {
        MoveAndStrafe();
        // Move();
        KBTurn();
        // SharpTurn();
    }


    // Update is called once per frame
    void Update() {
        
    }

    private void Move() {
        float movementInput = Input.GetAxis(m_MoveAxis);
        // Vector3 movementDirection = new Vector3(directionY * move, 0, directionY * move);

        if(movementInput != 0) {
            // GetComponent<Rigidbody>().velocity = movementDirection * speed * Time.deltaTime;
            
            Vector3 movement;
            if(Input.GetAxis(m_SprintKey) > 0) {
                movement = transform.forward * movementInput * m_Speed * m_RunModifier * Time.deltaTime;

                if(!m_IsRunning && movementInput > 0) { // TODO harcoded run modifier
                    m_Animator.Play("Run_Guard");
                    m_IsRunning = true;
                }
            }
            else {
                movement = transform.forward * movementInput * m_Speed * Time.deltaTime;

                if(!m_IsWalking) {
                    m_Animator.Play("WalkForward_Shoot");
                    m_IsWalking = true;
                }
            }
           
            m_Body.MovePosition(m_Body.position + movement);
                
        }
        else if(m_IsRunning || m_IsWalking) {
            m_Animator.Play("Idle_Shoot");
            m_IsRunning = false;            
            m_IsWalking = false;
        }
    }

    private void KBTurn() {
        float turn = Input.GetAxis(m_TurnAxis);

        if(turn != 0) {
            Quaternion turnRotation = Quaternion.Euler(0, turn * m_RotationSpeed * Time.deltaTime, 0);
            m_Body.MoveRotation(m_Body.rotation * turnRotation);
        }
    }




    //unused right now

    private void MoveAndStrafe() {
        Move();

        float strafe = Input.GetAxis(m_StrafeAxis);

        if(strafe != 0) {
            Vector3 movement = transform.right * strafe * m_StrafeSpeed * Time.deltaTime;

            // Rigidbody body = GetComponent<Rigidbody>();
            m_Body.MovePosition(m_Body.position + movement);

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


   







    private void MouseTurn() {
        // focusObject.GetComponent<Rigidbody>().MovePosition(Input.mousePosition);
        // focusObject.transform.position = Input.mousePosition;
        
        float scale = 10;

        Vector3 v = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        m_FocusObject.transform.position = new Vector3(v.x * scale, 0, v.y * scale);

        // debugText.text = "Monster transform.position:" + transform.position;
        // debugText.text += "\nMouse position:" + Input.mousePosition;
        // debugText.text += "\nCamera position:" + v;
        // debugText.text += "\nBall position:" + focusObject.transform.position;

        transform.LookAt(m_FocusObject.transform);
    }


}
