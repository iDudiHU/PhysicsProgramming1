using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	[SerializeField] private ReplayManager _rm;
	[SerializeField] private TextMeshProUGUI healthTMP;
	[SerializeField]
	private int _health = 1;
	private bool _IsDead;

	public void TakeDamage()
	{
		if (!_IsDead)
		{
			_health--;
			healthTMP.text = $"Health: { _health}";
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

	public void Heal()
	{
		_health++;
		healthTMP.text = $"Health: { _health}";
	}

	void Die()
	{
		//_rm.StartReplay();
		SceneManager.LoadScene("LoseScreen");
	}

}
