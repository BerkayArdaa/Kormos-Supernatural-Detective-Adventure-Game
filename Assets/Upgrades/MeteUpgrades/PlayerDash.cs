using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private float InitialDashDistance;
    public float dashDistance = 1.5f;
    public float dashSpeed = 10f;
    public float dashCooldown = 3f;
    public Transform cameraTransform; 

    private CharacterController characterController;
    private Vector3 dashDirection;
    private float lastDashTime; 
    private bool isDashing;

    private void Start()
    {
        InitialDashDistance = dashDistance;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleDashInput();

        if (isDashing)
        {
            Dash();
        }
    }

    private void HandleDashInput()
    {
        if (Time.time < lastDashTime + dashCooldown) return; 

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 inputDirection = GetInputDirection();
            if (inputDirection != Vector3.zero)
            {
                StartDash(inputDirection);
            }
        }
    }

    private Vector3 GetInputDirection()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDirection += cameraForward;
        if (Input.GetKey(KeyCode.S)) inputDirection -= cameraForward;
        if (Input.GetKey(KeyCode.A)) inputDirection -= cameraRight;
        if (Input.GetKey(KeyCode.D)) inputDirection += cameraRight;

        return inputDirection.normalized;
    }

    private void StartDash(Vector3 direction)
    {
        dashDirection = direction;
        isDashing = true;
        lastDashTime = Time.time;
    }

    private void Dash()
    {
        float dashStep = dashSpeed * Time.deltaTime;
        characterController.Move(dashDirection * dashStep);
        dashDistance -= dashStep;

        if (dashDistance <= 0)
        {
            isDashing = false;
            dashDistance = InitialDashDistance;
        }
    }
}
