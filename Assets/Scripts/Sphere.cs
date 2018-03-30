//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {

	[SerializeField]
	private Rigidbody _rigidbody = null;


	public void SphereFreeze(){
		_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}

	public void SphereResume(){
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

	}

	void OnCollisionEnter(Collision target){
		if (target.transform.tag != "obstacle") {
			_rigidbody.AddForce (target.transform.up * (1000 + 20 * Manager.Instance.Level));
		} else {
			_rigidbody.AddForce (target.transform.up * 300);
		}

	}


}
		