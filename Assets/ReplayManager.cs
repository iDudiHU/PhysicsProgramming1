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

	private List<ShotData> _shots = new List<ShotData>();

	private void Awake()
	{
		//Application.targetFrameRate = 60;
	}
	void Start()
	{
		dragAndShoot.OnShoot += SaveShot;
		StartPosition = dragAndShoot.transform.position;
	}

	public void SaveShot(Vector3 forceVector)
	{
		if (isReplaying)
		{
			return;
		}

		float timeDifference = 0f;
		if (_shots.Count > 0)
		{
			timeDifference = Time.time - _lastShotTime;
		}
		_lastShotTime = Time.time;
		_shots.Add(new ShotData(forceVector, timeDifference));
	}

	public void StartReplay()
	{
		_pp.IsReplay = true;
		hazzards.enabled = false;
		_tr.enabled = false;
		_tr.enabled = true;
		_uim.SwitchToReplay();
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
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	[System.Serializable]
	public class ShotData
	{
		public Vector3 force;
		public float timeDifference;

		public ShotData(Vector3 force, float timeDifference)
		{
			this.force = force;
			this.timeDifference = timeDifference;
		}
	}
}
