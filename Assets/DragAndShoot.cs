using System;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
	private Vector3 _mousePressDownPosition;
	private Vector3 _mouseReleasePosition;
	Vector3 startPosition, mouseWorld, mouseWorldStart;
	public LineRenderer _lineRenderer;
	private Vector3 _launchDirection;
	public float arrowheadSize = 0.2f;
	public float maxLength = 2f;
	[SerializeField] private int _MovesCount = 0;
	public int MovesCount => _MovesCount;
	Gradient gradient;
	[SerializeField]
	private float _forceMultiplier = 3;

	private Rigidbody rb;

	public event Action<Vector3> OnShoot;
	public bool isResplay;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		gradient = _lineRenderer.colorGradient;
	}

	private void Update()
	{
		if (!isResplay)
		{
			if (Input.GetMouseButtonDown(0))
			{ // Check if left mouse button is clicked
				_mousePressDownPosition = Input.mousePosition;
				mouseWorldStart = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
				
				Time.timeScale = 0.1f;
				// Increase FixedUpdate frequency
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
			else if (Input.GetMouseButton(0))
			{
				_lineRenderer.enabled = true;
				DrawArrow();
			}
			else if (Input.GetMouseButtonUp(0))
			{ // Check if left mouse button is released
				_lineRenderer.enabled = false;
				Shoot(-_launchDirection*1.6f);
				Debug.Log(_launchDirection);
				Time.timeScale = 1f;
				Time.fixedDeltaTime = 0.02f;
			} 
		}
	}

	private void DrawArrow()
	{
		startPosition = transform.position;
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		_launchDirection = mouseWorld - startPosition;
		_launchDirection = Vector3.ClampMagnitude(_launchDirection, maxLength);

		Vector3 endPoint = startPosition + -_launchDirection*1.6f;

		float percentSize = arrowheadSize / _launchDirection.magnitude;
		_lineRenderer.SetPosition(0, startPosition);
		_lineRenderer.SetPosition(1, Vector3.Lerp(startPosition, endPoint, 0.999f - percentSize));
		_lineRenderer.SetPosition(2, Vector3.Lerp(startPosition, endPoint, 1 - percentSize));
		_lineRenderer.SetPosition(3, endPoint);
		_lineRenderer.widthCurve = new AnimationCurve(
			new Keyframe(0, 0.4f),
			new Keyframe(0.999f - percentSize, 0.4f),
			new Keyframe(1 - percentSize, 1f),
			new Keyframe(1 - percentSize, 1f),
			new Keyframe(1, 0f));

		// Calculate the ratio of the launch vector magnitude to the max length
		float ratio = Vector3.Magnitude(new Vector3(_launchDirection.x, _launchDirection.y, 0f)*1.6f) / maxLength;

		// Create a custom gradient
		Gradient customGradient = new Gradient();
		customGradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.green, 1) },
			new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
		);

		// Sample the gradient based on the ratio and set the color of the line renderer
		Color sampledColor = customGradient.Evaluate(ratio);
		_lineRenderer.startColor = sampledColor;
		_lineRenderer.endColor = sampledColor;
	}


	public void Shoot(Vector3 forceVector)
	{
		Vector3 force = new Vector3(forceVector.x, forceVector.y, .0f) * _forceMultiplier;
		OnShoot?.Invoke(force);
		_MovesCount++;
	}

	public void HazzardShoot(Vector3 forceVector)
	{
		Vector3 force = new Vector3(forceVector.x, forceVector.y, .0f) * _forceMultiplier;
		GetComponent<PhysicsPlayer>().ResetAirJumps();
		OnShoot?.Invoke(forceVector);
	}

	public void ReplayShoot(Vector3 forceVector)
	{
		Vector3 force = new Vector3(forceVector.x, forceVector.y, .0f) * _forceMultiplier;
		OnShoot?.Invoke(forceVector);
		Wind wind = FindObjectOfType<Wind>();
		Debug.Log($"Shoot Vector: {forceVector}");
		Debug.Log($"Wind Durection: {wind.WindDirection.x}");
	}

}