using UnityEngine;
using UnityEngine.EventSystems;

public class MouseLookPlayer : MonoBehaviour
{
    [Header("=== MOUSE LOOK ===")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float verticalClampMax = 80f;
    [SerializeField] private float verticalClampMin = -80f;
    [SerializeField] private float smoothTime = 0.1f; // Suavizado del movimiento de cámara

    [Header("=== MOVIMIENTO ===")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private bool moveWithClicks = true;
    [SerializeField] private float clickMoveSpeed = 5f;
    [SerializeField] private float moveAcceleration = 10f; // Aceleración gradual

    [Header("=== ZOOM (RUEDA DEL MOUSE) ===")]
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 70f;
    [SerializeField] private float zoomSmoothTime = 0.2f; // Suavizado del zoom

    [Header("=== ROTACIÓN CON CLICK DERECHO ===")]
    [SerializeField] private bool rotateWithRightClick = true;
    [SerializeField] private float orbitSpeed = 100f;
    [SerializeField] private float orbitSmoothTime = 0.15f;

    [Header("=== PANEO CON CLICK IZQUIERDO ===")]
    [SerializeField] private bool enablePanWithLeftClick = true;
    [SerializeField] private float panSpeed = 0.5f;
    [SerializeField] private float panSmoothTime = 0.1f;

    [Header("=== MOVIMIENTO WASD ===")]
    [SerializeField] private bool enableWASDMovement = true;
    [SerializeField] private float wasdSpeed = 6f;

    [Header("=== INTERACCIÓN ===")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask interactableLayer;

    [Header("=== CURSOR ===")]
    [SerializeField] private bool lockCursorOnStart = false;
    [SerializeField] private KeyCode toggleCursorKey = KeyCode.Escape;

    // Variables privadas para suavizado
    private float xRotation = 0f;
    private float targetXRotation = 0f;
    private float currentXRotationVelocity = 0f;
    
    private float yRotation = 0f;
    private float targetYRotation = 0f;
    private float currentYRotationVelocity = 0f;

    private Transform cameraTransform;
    private CharacterController characterController;
    private Camera mainCamera;
    
    private Vector3 clickTargetPosition;
    private bool isMovingToClick = false;
    
    private float currentZoom = 60f;
    private float targetZoom = 60f;
    private float zoomVelocity = 0f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 moveVelocity = Vector3.zero;

    private bool isRightClickHeld = false;
    private bool isLeftClickHeld = false;
    private Vector3 lastMousePosition;

    void Start()
    {
        // Obtener referencias
        mainCamera = GetComponentInChildren<Camera>();
        if (mainCamera != null)
        {
            cameraTransform = mainCamera.transform;
            currentZoom = mainCamera.fieldOfView;
            targetZoom = currentZoom;
        }
        else
        {
            Debug.LogError("No se encontró una cámara en los hijos del jugador!");
            return;
        }

        characterController = GetComponent<CharacterController>();

        // Inicializar rotaciones
        Vector3 currentRotation = transform.eulerAngles;
        yRotation = currentRotation.y;
        targetYRotation = yRotation;
        
        if (cameraTransform != null)
        {
            xRotation = cameraTransform.localEulerAngles.x;
            if (xRotation > 180f) xRotation -= 360f;
            targetXRotation = xRotation;
        }

        // Bloquear cursor si está configurado
        if (lockCursorOnStart)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }

    void Update()
    {
        // Detectar si se mantiene presionado el click derecho
        if (Input.GetMouseButtonDown(1))
        {
            isRightClickHeld = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightClickHeld = false;
        }

        // Detectar si se mantiene presionado el click izquierdo
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            isLeftClickHeld = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isLeftClickHeld = false;
        }

        // Controlar paneo con click izquierdo
        HandlePan();

        // Controlar rotación de cámara
        HandleMouseLook();

        // Controlar movimiento
        HandleMovement();

        // Controlar zoom
        HandleZoom();

        // Alternar cursor
        HandleCursorToggle();
    }

    void HandlePan()
    {
        if (!enablePanWithLeftClick || !isLeftClickHeld) return;

        // Calcular diferencia del mouse
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        // Convertir movimiento del mouse a movimiento en el mundo
        Vector3 panMovement = Vector3.zero;
        
        // Movimiento horizontal (izquierda/derecha)
        panMovement -= transform.right * mouseDelta.x * panSpeed * Time.deltaTime;
        
        // Movimiento vertical (adelante/atrás basado en movimiento vertical del mouse)
        panMovement -= transform.forward * mouseDelta.y * panSpeed * Time.deltaTime;

        // Aplicar movimiento suave
        if (characterController != null && characterController.enabled)
        {
            characterController.Move(panMovement);
        }
        else
        {
            transform.position += panMovement;
        }

        // Cancelar movimiento automático por click
        isMovingToClick = false;
    }

    void HandleMouseLook()
    {
        // Solo rotar cuando se mantiene presionado el botón derecho del mouse
        if (rotateWithRightClick && isRightClickHeld)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Actualizar rotaciones objetivo
            targetXRotation -= mouseY;
            targetXRotation = Mathf.Clamp(targetXRotation, verticalClampMin, verticalClampMax);
            targetYRotation += mouseX;
        }

        // Suavizar rotación vertical (cámara)
        xRotation = Mathf.SmoothDamp(xRotation, targetXRotation, ref currentXRotationVelocity, smoothTime);
        
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Suavizar rotación horizontal (cuerpo)
        yRotation = Mathf.SmoothDampAngle(yRotation, targetYRotation, ref currentYRotationVelocity, orbitSmoothTime);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void HandleMovement()
    {
        Vector3 targetMove = Vector3.zero;

        // Movimiento con WASD
        if (enableWASDMovement)
        {
            float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
            float vertical = Input.GetAxisRaw("Vertical");     // W/S

            if (horizontal != 0 || vertical != 0)
            {
                Vector3 direction = (transform.right * horizontal + transform.forward * vertical).normalized;
                
                float currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : wasdSpeed;
                targetMove = direction * currentSpeed;
            }
        }

        // Suavizar el movimiento con aceleración
        moveVelocity = Vector3.Lerp(moveVelocity, targetMove, moveAcceleration * Time.deltaTime);

        // Aplicar movimiento
        if (characterController != null && characterController.enabled)
        {
            // Agregar gravedad si usas CharacterController
            Vector3 finalMove = moveVelocity;
            if (!characterController.isGrounded)
            {
                finalMove.y -= 9.81f * Time.deltaTime;
            }
            characterController.Move(finalMove * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveVelocity * Time.deltaTime, Space.World);
        }
    }

    void HandleZoom()
    {
        if (!enableZoom) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Suavizar el zoom
        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSmoothTime);

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = currentZoom;
        }
    }

    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(toggleCursorKey))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ===== MÉTODOS PÚBLICOS =====

    /// <summary>
    /// Cambia la sensibilidad del mouse en tiempo real
    /// </summary>
    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = Mathf.Max(0, newSensitivity);
    }

    /// <summary>
    /// Cambia la velocidad de movimiento en tiempo real
    /// </summary>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0, newSpeed);
    }

    /// <summary>
    /// Establece el tiempo de suavizado de la cámara
    /// </summary>
    public void SetSmoothTime(float newSmoothTime)
    {
        smoothTime = Mathf.Max(0.01f, newSmoothTime);
    }

    /// <summary>
    /// Habilita o deshabilita la rotación con click derecho
    /// </summary>
    public void SetRotateWithRightClick(bool enabled)
    {
        rotateWithRightClick = enabled;
    }

    /// <summary>
    /// Habilita o deshabilita el paneo con click izquierdo
    /// </summary>
    public void SetPanWithLeftClick(bool enabled)
    {
        enablePanWithLeftClick = enabled;
    }

    /// <summary>
    /// Ajusta la velocidad del paneo
    /// </summary>
    public void SetPanSpeed(float newSpeed)
    {
        panSpeed = Mathf.Max(0.1f, newSpeed);
    }
}