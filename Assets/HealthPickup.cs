using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	public GameObject pickUpSFX;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Health health = collision.GetComponent<Health>();
		if (health != null)
		{
			health.Heal();
			GameObject destroyobject = Instantiate(pickUpSFX);
			Destroy(destroyobject, 3);
			Destroy(this.gameObject);
		}
	}
}
