using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

	private void Start()
	{
        _das.OnShoot += DisplayShootInfo;
	}

    void DisplayShootInfo(Vector3 forceVector)
	{
        // Update the power and angle display
        _powerTMP.text = $"Power: {forceVector.magnitude:F2}";
        // Calculate the angle between the force vector and the x-axis
        float angle = Mathf.Atan2(forceVector.y, forceVector.x) * Mathf.Rad2Deg;

        _angleTMP.text = $"Angle: {angle:F2}°";
        _movesTotalTMP.text = $"Moves: {++_moves}";
    }

    public void SwitchToReplay()
	{
        _gameplay.SetActive(false);
        _replay.SetActive(true);
	}

}
