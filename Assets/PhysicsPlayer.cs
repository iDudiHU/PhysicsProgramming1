using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhysicsPlayer : MonoBehaviour
{
    /**
        Kinematic equations:
        1. s = ((u + v) * t) / 2
        2. v = u + a * t
        3. s = u * t + (a * t * t) / 2
        4. s = v * t - (a * t * t) / 2
        5. v * v = u * u + 2 * a * s
 
        v = final velocity
        u = initial velocity
        a = acceleration (gravity)
        s = displacement
        t = duration time
     */
    DragAndShoot DNS;
    [SerializeField] private float _gravityStrength = 9.8f;
    Vector3 _LaunchVectorDirection = Vector3.zero;
    public float magicNumber;
    private float _elapsedTime;
    private bool _isLaunched = false;
    private bool _IsGrounded = false;
    private bool _collidedOnSide, _collidedOnTop = false;
    public bool IsReplay = false;
    // Store the launch position
    [SerializeField]
    private Vector3 _windDirection;
    private Vector3 _currentVelocity;   

    // For collision detection
    private RaycastHit2D _raycastHit;
    [SerializeField] private LayerMask _tilemapLayerMask;
    [SerializeField] private float _collisionCheckDistance = 0.1f;
    [SerializeField] private float _groundedCheckDistance = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        DNS = GetComponent<DragAndShoot>();
        DNS.OnShoot += MouseReleased;
        _windDirection = new Vector3(Random.Range(-5,5), .0f, .0f);
    }

    void MouseReleased(Vector3 forceVector)
    {
        // If we're already in the air, adjust the launchVector based on the current position and velocity
        if (_isLaunched)
        {
            //Vector2 currentPosition = transform.position;
            //Vector2 currentVelocity = (_LaunchVectorDirection.y - _gravityStrength * _elapsedTime) * Vector2.up
            //    + _LaunchVectorDirection.x * Vector2.right;
            //_LaunchVectorDirection = forceVector + currentVelocity;
            _LaunchVectorDirection = forceVector;
            _elapsedTime = 0f;
        }
        else
        {
            _LaunchVectorDirection = forceVector;
            _elapsedTime = 0f;
        }
        _currentVelocity = _LaunchVectorDirection;
        _isLaunched = true;
        _collidedOnSide = false;
        _collidedOnTop = false;
        if (!IsReplay)
		{
            //StartCoroutine(SlowDown());
		}
    }
    private void FixedUpdate()
	{
        if (_isLaunched)
        {
            Vector3 gravityVelocityChange = new Vector3(0f, -_gravityStrength * Time.fixedDeltaTime, 0f);
            // Calculate the velocity change due to wind
            Vector3 windVelocityChange = _windDirection * Time.fixedDeltaTime;

            // Update the current velocity with gravity and wind effects
            _currentVelocity += gravityVelocityChange + windVelocityChange;

            // Calculate the position change based on the current velocity
            Vector3 positionDiff = _currentVelocity * Time.fixedDeltaTime;

            // Apply the total position change to the current position
            Vector3 newPos = transform.position + positionDiff;

            HandleCollisionDetection(ref newPos);

            transform.position = newPos;
            //transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        }
    }
    private IEnumerator SlowDown()
    {
        Time.timeScale = 1.0f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = .3f;
    }

    void HandleCollisionDetection(ref Vector3 newPos)
	{
        // Perform a raycast right and left to stop calculating x position
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.1f + 0.25f, _tilemapLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.1f + 0.25f, _tilemapLayerMask);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 0.1f + 0.25f, _tilemapLayerMask);
        if (hitRight.collider != null && _currentVelocity.x > 0)
        {
            _collidedOnSide = true;
        }
        if (hitLeft.collider != null && _currentVelocity.x < 0)
        {
            _collidedOnSide = true;
        }
        if (hitUp.collider != null && _currentVelocity.y > 0)
        {
            _collidedOnTop = true;
        }

        if (_collidedOnSide)
		{
            _currentVelocity.x = -_currentVelocity.x;
            _collidedOnSide = false;
		}
        if (_collidedOnTop)
		{
            // Set the upward component of the current velocity to zero
            _currentVelocity.y = -_currentVelocity.y;
            _collidedOnTop = false;
		}

        // Check to see if we are falling that we land on top of a floor tile
        _IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f + 0.25f, _tilemapLayerMask);
        if (_currentVelocity.y < 0)
        {
            if (_IsGrounded)
            {
                _isLaunched = false;
            }
        }
    }
	private void OnCollisionEnter2D(Collision2D collision)
	{
        IDamaging damagingObject = collision.gameObject.GetComponent<IDamaging>();

        if (damagingObject != null)
        {
            damagingObject.DamagePlayer(gameObject);
        }
    }
    public void HandleDamageCollision(Vector3 vectorToAdd)
	{
        _currentVelocity = Vector3.zero;
        AddVector3(vectorToAdd);
	}

    public void AddVector3(Vector3 vectorToAdd)
	{
		_currentVelocity += vectorToAdd;
	}
}
