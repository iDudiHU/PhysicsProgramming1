using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour
{
	public void LoadMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
