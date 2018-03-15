using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {

	[SerializeField]
	private Sphere _sphere = null;
	[SerializeField]
	private Rigidbody _rigidbody = null;


	public void SphereFreeze(){
		_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void SphereResume(){
		_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
	}

	void OnCollisionEnter(Collision target){
		if (target.transform.tag == "obstacle")
			Manager.Instance.GameStop ();
		else
			_rigidbody.AddForce (target.transform.up * 1000);

	}
}
		