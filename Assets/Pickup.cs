using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public GameObject pickUpSFX;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		PhysicsPlayer pp = collision.GetComponent<PhysicsPlayer>();
		if (pp != null)
		{
			pp.AddInAirJumps();
			GameObject destroyobject = Instantiate(pickUpSFX);
			Destroy(destroyobject, 3);
			Destroy(this.gameObject);
		}
	}
}
