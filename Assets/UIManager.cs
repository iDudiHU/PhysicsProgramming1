using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // For displaying power and angle
    [SerializeField] private TextMeshProUGUI _powerTMP;
    [SerializeField] private TextMeshProUGUI _angleTMP;
    [SerializeField] private TextMeshProUGUI _movesTotalTMP;
    [SerializeField] private TextMeshProUGUI _windPowerTMP;
    [SerializeField] private TextMeshProUGUI _WindAngleTMP;
    [SerializeField] private TextMeshProUGUI _HealthTMP;

    //Player Reference
    [SerializeField] private GameObject _player;
    [SerializeField] private PhysicsPlayer _pp;
    [SerializeField] private DragAndShoot _das;
    [SerializeField] private Health _h;

    //GameplayState
    [SerializeField] private GameObject _gameplay;
    [SerializeField] private GameObject _replay;

    //Moves Count
    [SerializeField] private int _moves;

    //Wind Info
    [SerializeField] private Wind _wind;
    [SerializeField] private Vector3 _oldWindInfo;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

	private void Start()
	{
        _wind = FindObjectOfType<Wind>();
        _das.OnShoot += DisplayShootInfo;
        DisplayWindInfo();
	}

    void DisplayShootInfo(Vector3 forceVector)
	{
        // Update the power and angle display
        _powerTMP.text = $"Power: {forceVector.magnitude:F2}";
        // Calculate the angle between the force vector and the x-axis
        float angle = Mathf.Atan2(forceVector.y, forceVector.x) * Mathf.Rad2Deg;

        _angleTMP.text = $"Angle: {angle:F2}°";
        _movesTotalTMP.text = $"Moves: {++_moves}";
        DisplayWindInfo();
    }

    void DisplayWindInfo()
	{
        // Update the power and angle display
        _windPowerTMP.text = $"Wind Power: {_wind.WindDirection.magnitude:F2}";
        // Calculate the angle between the force vector and the x-axis
        float angle = Mathf.Atan2(_wind.WindDirection.y, _wind.WindDirection.x) * Mathf.Rad2Deg;

        _WindAngleTMP.text = $"Wind Angle: {angle:F2}°";
        SetArrowActive(_wind.WindDirection);
    }

    public void SwitchToReplay()
	{
        _gameplay.SetActive(false);
        _replay.SetActive(true);
	}

    void SetArrowActive(Vector3 windDirection)
	{
        if (windDirection.x == 0)
		{
            _leftArrow.SetActive(false);
            _rightArrow.SetActive(false);
        } else if (windDirection.x < 0)
		{
            _leftArrow.SetActive(true);
            _rightArrow.SetActive(false);
        } else
		{
            _leftArrow.SetActive(false);
            _rightArrow.SetActive(true);
        }
	}

}
