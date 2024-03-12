using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    //Referenz für die Objekte CharacterController, PlayerInput & Animator aus dem Projekt
    private PlayerInput playerInput;
    private CharacterController characterController;
    private Animator animator;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private Vector3 currentRunMovement;
    private bool isMovePressed;
    private bool isTurnPressed;
    private bool isRunPressed;
    private bool isDancingPressed;

    private float rotationSpeed = 2.0f;
    [SerializeField] private float runMultiplier;
    [SerializeField] private float walkSpeed;

    private float groundedGravity = -0.05f;
    private float gravity = -9.8f;

    private void Awake()
    {
        //Instanzieren eines neuen Objekts für PlayerInputs
        playerInput = new PlayerInput();

        //Zugriff auf CharacterController & Animator
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //Wenn ein Move Input vom Spieler ausgeführt wird soll die OnMove Methode ausgeführt werden -> Callback/Listener
        playerInput.CharacterControls.Move.started += OnMove;
        playerInput.CharacterControls.Move.performed += OnMove;
        playerInput.CharacterControls.Move.canceled += OnMove;

        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

        playerInput.CharacterControls.Dance.started += OnDance;
        playerInput.CharacterControls.Dance.canceled += OnDance;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleAnimation();
        HandleGravity();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        //Die Move Inputs werden
        currentMovementInput = context.ReadValue<Vector2>();
        isMovePressed = currentMovementInput.y != 0;
        isTurnPressed = currentMovementInput.x != 0;
    }

    //Überprüfung ob der RunInput ausgeführt wird und dem entsprechend aktualisiert
    private void OnRun(InputAction.CallbackContext context) 
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnDance(InputAction.CallbackContext context)
    {
        isDancingPressed = context.ReadValueAsButton();
    }

    private void HandleMovement()
    {
        if (isMovePressed)
        {
            if (currentMovementInput.y > 0)
            {
                currentMovement.x = transform.forward.x * walkSpeed;
                currentMovement.z = transform.forward.z * walkSpeed;
                currentRunMovement.x = transform.forward.x * runMultiplier;
                currentRunMovement.z = transform.forward.z * runMultiplier;
            }
            else if (currentMovementInput.y < 0)
            {
                currentMovement.x = -transform.forward.x;
                currentMovement.z = -transform.forward.z;
            }
        }
        else
        {
            currentMovement.x = 0;
            currentMovement.z = 0;
            currentRunMovement.x = 0;
            currentRunMovement.z = 0;
        }


        if (isRunPressed) 
        { 
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }
    private void HandleRotation()
    {
        if (isTurnPressed)
        {
            Vector3 targetDirection = Vector3.zero;

            if (currentMovementInput.x > 0)
            {
                targetDirection = transform.right;
            }
            else if (currentMovementInput.x < 0)
            {
                targetDirection = -1 * transform.right;
            }

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    private void HandleAnimation()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if ((isMovePressed || isTurnPressed) && !isWalking)
        {
            animator.SetBool("isWalking", true);
        }
        else if (!isMovePressed && !isTurnPressed && isWalking)
        {
            animator.SetBool("isWalking", false);
        }

        if (isMovePressed && isRunPressed && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if ((isMovePressed && !isRunPressed) && isRunning)
        {
            animator.SetBool("isRunning", false);
        }

        if (isDancingPressed)
        {
            animator.SetTrigger("Dancing");
        }
    }

    //Wenn der characterController am Boden ist wird nur eine geringe Menge an Gravitation aus geübt.
    //Sobald er in der Luft ist und eine höhere Distanz zum Boden hat wird mehr Gravitation ausgeübt (Erdbeschleunigung)
    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else
        {
            currentMovement.y += gravity * Time.deltaTime;
            currentRunMovement.y += gravity * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
