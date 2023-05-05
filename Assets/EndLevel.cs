using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    ReplayManager _rpm;
	private void Awake()
	{
		_rpm = FindObjectOfType<ReplayManager>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		SceneManager.LoadScene("WinScreen");
	}
}
