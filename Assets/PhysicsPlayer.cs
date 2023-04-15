using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicsPlayer : MonoBehaviour
{
    DragAndShoot DNS;
    [SerializeField] private float _gravityStrength = 10f;
    Vector2 _LaunchVectorDirection = Vector2.zero;
    private float _elapsedTime;
    private bool _isLaunched = false;
    private bool _IsGrounded = false;
    private bool _collidedOnSide, _collidedOnTop = false;
    // Store the launch position
    private Vector3 _launchPosition;

    // For displaying power and angle
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _angleText;

    // For collision detection
    private RaycastHit2D _raycastHit;
    [SerializeField] private LayerMask _tilemapLayerMask;
    [SerializeField] private float _collisionCheckDistance = 0.1f;
    [SerializeField] private float _groundedCheckDistance = 0.2f;

    private Vector2 _prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        DNS = GetComponent<DragAndShoot>();
        DNS.OnShoot += MouseReleased;
    }

    void MouseReleased(Vector2 forceVector)
    {
        // If we're already in the air, adjust the launchVector based on the current position and velocity
        if (_isLaunched)
        {
            Vector2 currentPosition = transform.position;
            Vector2 currentVelocity = (_LaunchVectorDirection.y - _gravityStrength * _elapsedTime) * Vector2.up
                + _LaunchVectorDirection.x * Vector2.right;
            _LaunchVectorDirection = forceVector + currentVelocity;
            _elapsedTime = 0f;
        }
        else
        {
            _LaunchVectorDirection = forceVector;
            _elapsedTime = 0f;
        }
        _launchPosition = transform.position;
        _isLaunched = true;
        _collidedOnSide = false;
        _collidedOnTop = false;
        StartCoroutine(SlowDown());

        // Update the power and angle display
        _powerText.text = $"Power: {_LaunchVectorDirection.magnitude:F2}";
        _angleText.text = $"Angle: {Vector2.SignedAngle(Vector2.right, _LaunchVectorDirection):F2}°";
    }


    private void FixedUpdate()
	{
        if (_isLaunched)
        {
            // Update the elapsed time
            _elapsedTime += Time.fixedDeltaTime;

            // Calculate the new position based on the kinematic equations
            Vector3 positionDiff = new Vector3(
                _LaunchVectorDirection.x * _elapsedTime,
                _LaunchVectorDirection.y * _elapsedTime - 0.5f * _gravityStrength * Mathf.Pow(_elapsedTime, 2),
                0f
            );
            // Calculate the new position by adding the position difference to the launch position
            Vector3 newPos = _launchPosition + positionDiff;

            // Perform a raycast right and left to stop calculating x position
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.1f + 0.25f, _tilemapLayerMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.1f + 0.25f, _tilemapLayerMask);
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 0.1f + 0.25f, _tilemapLayerMask);
            if (hitRight.collider != null && _LaunchVectorDirection.x > 0)
			{
                _collidedOnSide = true;
            }
            if (hitLeft.collider != null && _LaunchVectorDirection.x < 0)
            {
                _collidedOnSide = true;
            }
            if (hitUp.collider != null && _LaunchVectorDirection.y > 0)
            {
                _collidedOnTop = true;
            }

            if (_collidedOnSide)
                newPos.x = transform.position.x;
            if (_collidedOnTop)
                newPos.y = Mathf.Min(newPos.y, transform.position.y);

            // Check if the player is grounded
            _IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f + 0.25f, _tilemapLayerMask);

            // If we are on the ground don't fall through
            if (_IsGrounded && (_LaunchVectorDirection.y - _gravityStrength * _elapsedTime) < 0)
            {
                _isLaunched = false;
                return;
            }

            // Smoothly interpolate between the current position and the new position
            transform.position = newPos;
            //transform.position = Vector3.Lerp(transform.position, newPos, Time.fixedDeltaTime * 10f);
        }
    }

	private IEnumerator SlowDown()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixedDeltaTime based on the current timeScale
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust fixedDeltaTime based on the current timeScale
    }
}
