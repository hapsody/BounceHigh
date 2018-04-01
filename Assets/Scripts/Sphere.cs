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
			var temp = _rigidbody.velocity;
			_rigidbody.velocity = new Vector3 (temp.x, (35 + Manager.Instance.Level * 0.5f) * Mathf.Abs( target.transform.up.y) , temp.z);

			temp = target.transform.up;
			if (temp.y > 0) {
				temp.y = 0;
				_rigidbody.AddForce (temp * 250);
			} else {
				temp.y = 0;
				_rigidbody.AddForce (temp * -250);
			}
		
		} else {
			_rigidbody.AddForce (target.transform.up * 300);
		}

	}


}
		