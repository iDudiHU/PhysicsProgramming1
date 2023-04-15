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

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		gradient = _lineRenderer.colorGradient;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{ // Check if left mouse button is clicked
			_mousePressDownPosition = Input.mousePosition;
			mouseWorldStart = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		}
		else if (Input.GetMouseButton(0))
		{
			_lineRenderer.enabled = true;
			DrawArrow();
		}
		else if (Input.GetMouseButtonUp(0))
		{ // Check if left mouse button is released
			_mouseReleasePosition = Input.mousePosition;
			Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
			_lineRenderer.enabled = false;

			// Calculate the normalized difference between mouse press and release positions
			Vector2 mouseDiffNormalized = (_mouseReleasePosition - _mousePressDownPosition).normalized;
			// Calculate the launch direction by multiplying the normalized difference by the length of the arrow
			_launchDirection = -mouseDiffNormalized * _launchDirection.magnitude;

			Shoot(_launchDirection);
		}
	}

	private void DrawArrow()
	{
		startPosition = transform.position;
		Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
		_launchDirection = mouseWorld - mouseWorldStart;
		_launchDirection = Vector3.ClampMagnitude(_launchDirection, maxLength);

		Vector3 endPoint = startPosition + -_launchDirection;

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
		GradientColorKey[] colorKeys = gradient.colorKeys;
		colorKeys[1].time = Mathf.Clamp01(1 - _launchDirection.magnitude / maxLength);
		gradient.SetKeys(colorKeys, gradient.alphaKeys);
		_lineRenderer.colorGradient = gradient;
	}

	public void Shoot(Vector3 forceVector)
	{
		Vector3 force = new Vector3(forceVector.x, forceVector.y, .0f) * _forceMultiplier;
		OnShoot?.Invoke(force);
		_MovesCount++;
	}

	public void ReplayShoot(Vector3 forceVector)
	{
		OnShoot?.Invoke(forceVector);
	}

}