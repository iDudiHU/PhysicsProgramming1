using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour
{
	public DragAndShoot dragAndShoot;
	public PhysicsPlayer _pp;
	public TrailRenderer _tr;
	private float _lastShotTime;
	private Vector3 StartPosition;
	public bool isReplaying;
	public UIManager _uim;
	public Collider2D hazzards;
	public Wind wind;
	public DragAndShoot _das;
	public bool IsGameFinished;

	private List<ShotData> _shots = new List<ShotData>();

	private void Awake()
	{
		//Application.targetFrameRate = 60;
	}
	void Start()
	{
		wind = FindObjectOfType<Wind>();
		dragAndShoot.OnShoot += SaveShot;
		StartPosition = dragAndShoot.transform.position;
		_das = FindObjectOfType<DragAndShoot>();
	}

	public void SaveShot(Vector3 forceVector)
	{
		if (isReplaying)
		{
			return;
		}

		Vector3 windDirection = wind.WindDirection;

		float timeDifference = 0f;
		if (_shots.Count > 0)
		{
			timeDifference = Time.time - _lastShotTime;
		}
		_lastShotTime = Time.time;
		_shots.Add(new ShotData(_shots.Count, forceVector, timeDifference));
		Debug.Log(_shots[_shots.Count - 1].ToString());
	}

	public void StartReplay()
	{
		_das.isResplay = true;
		_pp.IsReplay = true;
		hazzards.enabled = false;
		_tr.enabled = false;
		_tr.enabled = true;
		//_uim.SwitchToReplay();
		_pp.StopAllCoroutines();
		dragAndShoot.transform.position = StartPosition;
		Time.timeScale = 2f;
		StartCoroutine(Replay());
	}

	private IEnumerator Replay()
	{
		isReplaying = true;
		foreach (ShotData shotData in _shots)
		{
			yield return new WaitForSeconds(shotData.timeDifference);
			dragAndShoot.ReplayShoot(shotData.force);
		}
		isReplaying = false;
		_das.isResplay = false;
		_pp.IsReplay = false;
		Time.timeScale = 1f;
		if (IsGameFinished)
		{
			SceneManager.LoadScene("MainMenu");
		} else
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	[System.Serializable]
	public class ShotData
	{
		public int shotNumber;
		public Vector3 force;
		public float timeDifference;

		public ShotData(int shotNumber, Vector3 force, float timeDifference)
		{
			this.shotNumber = shotNumber;
			this.force = force;
			this.timeDifference = timeDifference;
		}
		public override string ToString()
		{
			return $"{shotNumber} ShotData:\n" +
				   $"Force: {force}\n" +
				   $"Time Difference: {timeDifference:F2}s";
		}
	}
}
