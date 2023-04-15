using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicsPlayer : MonoBehaviour
{
    DragAndShoot DNS;
    [SerializeField] private float _gravityStrength = 10f;
    Vector2 _launchVector = Vector2.zero;
    private float _elapsedTime;
    private bool _isLaunched = false;

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
        _launchVector = forceVector;
        _elapsedTime = 0f;
        _isLaunched = true;
        StartCoroutine(SlowDown());

        // Update the power and angle display
        _powerText.text = $"Power: {forceVector.magnitude:F2}";
        _angleText.text = $"Angle: {Vector2.SignedAngle(Vector2.right, forceVector):F2}°";
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLaunched)
        {
            // Update the elapsed time
            _elapsedTime += Time.deltaTime;

            // Calculate the new position based on the kinematic equations
            Vector3 newPos = new Vector3(
                _launchVector.x * _elapsedTime,
                _launchVector.y * _elapsedTime - 0.5f * _gravityStrength * Mathf.Pow(_elapsedTime, 2),
                0f
            );

            // Perform a raycast to check for Tilemap collisions
            _raycastHit = Physics2D.Raycast(transform.position, (newPos - transform.position).normalized, _collisionCheckDistance, _tilemapLayerMask);

            if (_raycastHit.collider != null)
            {
                // If there's a collision, handle it based on the direction
                float angle = Vector2.Angle(_raycastHit.normal, Vector2.up);
                if (angle <= 45)
                {
                    // Top collision, zero y velocity
                    _launchVector.y = 0;
                    _elapsedTime = 0f;
                }
                else
                {
                    // Side collision, zero x velocity
                    _launchVector.x = 0;
                    _elapsedTime = 0f;
                }
                // Set newPos to the current position to avoid the bounce
                newPos = transform.position;
            }

            // Check if the player is grounded
            bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, _groundedCheckDistance, _tilemapLayerMask);

            // Apply gravity if the player is not grounded
            if (!isGrounded)
            {
                newPos = new Vector3(
                    _launchVector.x * _elapsedTime,
                    _launchVector.y * _elapsedTime - 0.5f * _gravityStrength * Mathf.Pow(_elapsedTime, 2),
                    0f
                );
            }
            else
            {
                _isLaunched = false;
            }

            // Smoothly interpolate between the current position and the new position
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * 10f);
        }
    }

    private IEnumerator SlowDown()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        //Time.timeScale = 0.3f;
    }
}
