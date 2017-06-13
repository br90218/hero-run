using UnityEngine;
using System.Collections;

public class RFX4_ReplaceModelOnCollision : MonoBehaviour
{

	public GameObject PhysicsObjects;
	[SerializeField] private float _explosionRadius;
	[SerializeField] private float _explosionPower;
	[SerializeField] private float _upwardMod;

	private bool isCollided = false;
	Transform t;

	private void OnCollisionEnter (Collision collision)
	{
		if (!isCollided) {
			isCollided = true;
			PhysicsObjects.SetActive (true);
			var mesh = GetComponent<MeshRenderer> ();
			if (mesh != null)
				mesh.enabled = false;
			var rb = GetComponent<Rigidbody> ();
			rb.isKinematic = true;
			rb.detectCollisions = false;

			var colliders = Physics.OverlapSphere (t.position, _explosionRadius);
			foreach (Collider hitCollider in colliders) {
				//	print (hitCollider.name);
				var explodedrb = hitCollider.GetComponent<Rigidbody> ();
				if (explodedrb != null) {
					print (explodedrb.name);
					explodedrb.AddExplosionForce (_explosionPower, t.position, _explosionRadius, _upwardMod);
				}
			}
		}
	}

	void OnEnable ()
	{
		isCollided = false;
		PhysicsObjects.SetActive (false);
		var mesh = GetComponent<MeshRenderer> ();
		if (mesh != null)
			mesh.enabled = true;
		var rb = GetComponent<Rigidbody> ();
		rb.isKinematic = false;
		rb.detectCollisions = true;
	}
}
