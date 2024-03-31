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
    private bool isJumpPressed;
    private bool isDancing;

    private float rotationSpeed = 2.0f;
    [SerializeField] private float runMultiplier;
    private float walkSpeed = 1.5f;

    private float initialJumpVelocity;
    private float maxJumpHeight = 1.0f;
    private float maxJumpTime = 0.5f;
    private bool isJumping = false;

    private float groundedGravity = -0.05f;
    private float gravity = -9.8f;

    private void Awake()
    {
        //Instanzieren eines neuen Objekts für PlayerInput
        playerInput = new PlayerInput();

        //Zugriff auf CharacterController & Animator
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        InitJumpVariables();

        //Wenn ein Move Input vom Spieler ausgeführt wird soll die OnMove Methode ausgeführt werden -> Callback/Listener
        playerInput.CharacterControls.Move.started += OnMove;
        playerInput.CharacterControls.Move.performed += OnMove;
        playerInput.CharacterControls.Move.canceled += OnMove;

        playerInput.CharacterControls.Run.started += OnRun;
        playerInput.CharacterControls.Run.canceled += OnRun;

       
        //playerInput.CharacterControls.Dance.started += OnJump;
        //playerInput.CharacterControls.Dance.canceled += OnJump;
        playerInput.CharacterControls.Jump.started += OnJump; //Korrektur
        playerInput.CharacterControls.Jump.canceled += OnJump; //Korrektur

        playerInput.CharacterControls.Dance.started += OnDance;
        playerInput.CharacterControls.Dance.canceled += OnDance;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleAnimation();
        HandleGravity();
        HandleJump();
    }

    //Überprüfung ob die PlayerInputs ausgeführt werden und dem entsprechend aktualisiert werden
    public void OnMove(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovePressed = currentMovementInput.y != 0;
        isTurnPressed = currentMovementInput.x != 0;
    }

    
    private void OnRun(InputAction.CallbackContext context) 
    {
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    private void OnDance(InputAction.CallbackContext context)
    {
        isDancing = context.ReadValueAsButton();    
    }

    //Immer wenn der Move Input soll der Charakter sich bewegen
    //Wenn der input größer als null ausfällt wird current(Run)Movement an eine neue position übermittelt
    //dies wird mit dem walkSpeed oder dem RunMultiplier zusammen multipliziert
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
        //Default wert liegt bei  0 dementsprechend steht der charakter
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

        //Der Jump ist abhängig von der Sprunghöhe und der Sprungdauer
        //Hierzu muss eine Formel genutzt werden die auf beide Faktoren eingehen

        private void InitJumpVariables()
        {
            float timeToAppex = 0.5f * maxJumpTime;
            gravity = (-2 * maxJumpHeight)/ Mathf.Pow(timeToAppex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToAppex;
        }

        private void HandleJump()
        {
            if (!isJumping && characterController.isGrounded && isJumpPressed)
            {
                isJumping = true;
                currentMovement.y = initialJumpVelocity;
                currentRunMovement.y = initialJumpVelocity;
                animator.SetTrigger("Jumping"); //Korrektur
            }
            else if (!isJumpPressed && isJumping && characterController.isGrounded)
            {
                isJumping = false;
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

        if (isDancing)
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

                /*float previousYVelocity = currentMovement.y;
                float newYVelocity = currentMovement.y + gravity * Time.deltaTime;
                float nextYVelocity = previousYVelocity + newYVelocity * .5f;
                currentMovement.y += nextYVelocity;
                currentRunMovement.y += nextYVelocity;*/
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
