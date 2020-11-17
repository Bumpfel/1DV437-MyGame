using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public Transform m_PlayerModel;
    public Canvas m_AimReticle;
    
    private enum MovementControl { CameraRelativeMovement, CharacterRelativeMovement };
    private MovementControl m_MovementControl = MovementControl.CameraRelativeMovement;
    private Vector3 m_MoveTo;
    private Animator m_Animator;
    private Combatant m_Combatant;
    private LayerMask m_ObstacleMask;

    private readonly Vector3 CollisionCheckPoint = Vector3.up * 1.5f;
    private const float SpeedSmoothTime = .1f;
    private const float WalkSpeed = 5;
    private const float RunSpeed = WalkSpeed * 1.8f;
    private float m_CollisionCheckRadius;
    private string m_VerticalAxis;
    private string m_LookAxisX;
    private string m_LookAxisY;
    private string m_HorizontalAxis;
    private string m_SprintKey;
    

    // Movement method vars
    private float animationSpeed;
    private Vector3 prevPosition;
    private float currentMoveSpeed;
    private float targetSpeed;
    private Vector3 movementInput;


    // Look vars
    private Vector3 mousePosRelativeToCamera;
    private Vector3 mousePosInWorld;
    private const float ScreenClampPercentage = .98f;

      
    public bool IsRunning => Input.GetButton(m_SprintKey) && (Input.GetAxisRaw(m_VerticalAxis) != 0 || Input.GetAxisRaw(m_HorizontalAxis) != 0);

    private void Start() {
        m_Animator = GetComponent<Animator>();

        m_VerticalAxis = Controls.Vertical.ToString();
        m_HorizontalAxis = Controls.Horizontal.ToString();
        m_SprintKey = Controls.Sprint.ToString();

        m_Combatant = GetComponent<Combatant>();

        m_ObstacleMask = LayerMask.GetMask("Obstacles");
        m_CollisionCheckRadius = GetComponent<CapsuleCollider>().radius * .8f;

        prevPosition = transform.position;

        Enum.TryParse(PlayerPrefs.GetString(Settings.MovemenControl.ToString(), MovementControl.CameraRelativeMovement.ToString()), out m_MovementControl);
    }

    private void FixedUpdate() {

        //calculates movement based on input
        if(m_MovementControl == MovementControl.CharacterRelativeMovement)
            CharacterRelativeMovement();
        else
            CameraRelativeMovement();
        
        // confirms or rejects movement
        if(!transform.position.Equals(m_MoveTo)) {
            Collider[] colliders = Physics.OverlapSphere(transform.position + CollisionCheckPoint, m_CollisionCheckRadius, m_ObstacleMask);
            // Collider[] collidedWithWeapon = Physics.OverlapBox(transform.position + transform.forward * .9f + transform.right * .25f, new Vector3(.25f / 2, .4f / 2, 1.4f / 2), transform.rotation, m_ObstacleMask);
            if(colliders.Length == 0 || colliders[0].isTrigger) {// && collidedWithWeapon.Length == 0)
                transform.Translate(m_MoveTo * Time.fixedDeltaTime);
            }
        }
    }

    private void Update() {
        if(Time.timeScale == 0)
            return;
        Look();

        if(Input.GetKeyDown(KeyCode.M)) {
            m_MovementControl = (m_MovementControl == MovementControl.CameraRelativeMovement) ? MovementControl.CharacterRelativeMovement : MovementControl.CameraRelativeMovement;
            ScreenUI.DisplayMessage("Switched to " + m_MovementControl.ToString());
            PlayerPrefs.SetString(Settings.MovemenControl.ToString(), m_MovementControl.ToString());
            PlayerPrefs.Save();
        }

    }

   private void CameraRelativeMovement() {
        animationSpeed = m_MoveTo.magnitude / RunSpeed;
        
        currentMoveSpeed = WalkSpeed;
        movementInput = new Vector3(Input.GetAxisRaw(m_HorizontalAxis), 0, Input.GetAxisRaw(m_VerticalAxis)).normalized;
        targetSpeed = (IsRunning ? RunSpeed : WalkSpeed) * movementInput.magnitude;
        
        m_Animator.SetFloat(AnimatorSettings.speedPercent.ToString(), animationSpeed, SpeedSmoothTime, Time.fixedDeltaTime);

        if(Input.GetButton(m_SprintKey)) {
            currentMoveSpeed = RunSpeed;
        }
		m_MoveTo = movementInput * currentMoveSpeed;

        prevPosition = transform.position;
    }

    private void CharacterRelativeMovement() {
        animationSpeed = m_MoveTo.magnitude / RunSpeed;

        currentMoveSpeed = WalkSpeed;
        targetSpeed = (IsRunning ? RunSpeed : WalkSpeed) * Input.GetAxisRaw(m_VerticalAxis);

        m_Animator.SetFloat(AnimatorSettings.speedPercent.ToString(), animationSpeed, SpeedSmoothTime, Time.fixedDeltaTime);

        if(Input.GetButton(m_SprintKey)) {
            currentMoveSpeed = RunSpeed;
        }
		m_MoveTo = m_PlayerModel.forward * Input.GetAxisRaw(m_VerticalAxis) * currentMoveSpeed + m_PlayerModel.right * Input.GetAxis(m_HorizontalAxis) * currentMoveSpeed;
        m_MoveTo = Vector3.ClampMagnitude(m_MoveTo, RunSpeed); // stops overspeeding when running diagonally
        prevPosition = transform.position;
    }

    private void Look() {
        mousePosInWorld.Set(Mathf.Clamp(Input.mousePosition.x, Screen.width * (1 - ScreenClampPercentage), Screen.width * ScreenClampPercentage), Mathf.Clamp(Input.mousePosition.y, Screen.height * (1 - ScreenClampPercentage), Screen.height * ScreenClampPercentage), Camera.main.transform.position.y);
        mousePosRelativeToCamera = Camera.main.ScreenToWorldPoint(mousePosInWorld);

        m_PlayerModel.LookAt(mousePosRelativeToCamera + Vector3.up * transform.position.y);

        //placing reticle on top, compensating for aim reticle size, so the bullet is fired at the center of the reticle
        m_AimReticle.transform.position = mousePosRelativeToCamera + m_PlayerModel.right * .23f; // .23 for perspective, .25 for ortographic
    }

}