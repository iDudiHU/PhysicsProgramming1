using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	[SerializeField] private ReplayManager _rm;
	[SerializeField]
	private int _health = 1;
	private bool _IsDead;

	public void TakeDamage()
	{
		if (!_IsDead)
		{
			_health--;
			DieCheck();
		}
	}

	void DieCheck()
	{
		if (_health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		_rm.StartReplay();
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}
