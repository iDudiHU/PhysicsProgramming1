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
    [SerializeField] private float _gravityStrength = -9.8f;
    Vector3 _LaunchVectorDirection = Vector3.zero;
    private float playerHalfWidth;
    public float magicNumber;
    private float _elapsedTime;
    private bool _isLaunched = false;
    private bool _IsGrounded = false;
    private bool _collidedOnSide, _collidedOnTop = false;
    public bool IsReplay = false;
    public AudioSource Audio;
    public ParticleSystem Particle;
    private int _InAirJumpsMax = 1;
    private int _InAirJumpsRemaining = 1;
    public TextMeshProUGUI jumps;
    // Store the launch position
    [SerializeField]
    Wind wind;
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
        wind = FindObjectOfType<Wind>();
        Audio = GetComponent<AudioSource>();
        playerHalfWidth = GetComponent<Collider2D>().bounds.size.x / 2;
        Particle = GetComponent<ParticleSystem>();
    }

    void MouseReleased(Vector3 forceVector)
    {
        // If we're already in the air, adjust the launchVector based on the current position and velocity
        if (_InAirJumpsRemaining > 0)
        {
            Audio.Stop();
            Audio.Play();
            Particle.Play();
            _currentVelocity = forceVector * magicNumber;
            _isLaunched = true;
            _collidedOnSide = false;
            _collidedOnTop = false;
            _InAirJumpsRemaining--;
            jumps.text = $"In air jumps: {_InAirJumpsRemaining}";
        }
        if (!IsReplay)
		{
            //StartCoroutine(SlowDown());
		}
    }
    private void FixedUpdate()
	{
        if (_isLaunched)
        {
            Vector3 gravityVelocityChange = new Vector3(0f, _gravityStrength * Time.fixedDeltaTime, 0f);
            // Calculate the velocity change due to wind
            Vector3 windVelocityChange = wind.WindDirection * Time.fixedDeltaTime;

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
            newPos = transform.position;
            _collidedOnSide = true;
        }
        if (hitLeft.collider != null && _currentVelocity.x < 0)
        {
            newPos = transform.position;
            _collidedOnSide = true;
        }
        if (hitUp.collider != null && _currentVelocity.y > 0)
        {
            _collidedOnTop = true;
        }

        if (_collidedOnSide)
        {
            float playerHalfWidth = GetComponent<Collider2D>().bounds.size.x / 2;
            transform.position = newPos;
            _currentVelocity.x = -_currentVelocity.x * .7f;
            _collidedOnSide = false;
        }
        if (_collidedOnTop)
        {
            // Set the upward component of the current velocity to zero
            _currentVelocity.y = -_currentVelocity.y * .7f;
            _collidedOnTop = false;
        }

        // Check to see if we are falling that we land on top of a floor tile
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.1f + 0.25f, _tilemapLayerMask);
        if (_currentVelocity.y < 0)
        {
            if (hitDown.collider != null)
            {
                _isLaunched = false;

                // Calculate the distance between the player and the hit point
                float playerHalfHeight = GetComponent<Collider2D>().bounds.size.y / 2;

                // Set the new position of the player directly above the ground
                newPos = new Vector3(transform.position.x, hitDown.point.y + playerHalfHeight, transform.position.z);
                transform.position = newPos;

                // Set the downward component of the current velocity to zero
                _currentVelocity.y = 0;

                //Reset Jumps
                _InAirJumpsRemaining = _InAirJumpsMax;
                jumps.text = $"In air jumps: {_InAirJumpsRemaining}";
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

    public void AddInAirJumps()
	{
        _InAirJumpsMax++;
        ResetAirJumps();
    }
    public void ResetAirJumps()
    {
        _InAirJumpsRemaining = _InAirJumpsMax;
    }
}
