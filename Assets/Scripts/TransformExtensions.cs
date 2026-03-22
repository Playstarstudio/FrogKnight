using UnityEngine;
using System.Collections;
  
public static class TransformExtensions {
	public static void LookAwayFrom2D(this Transform t, Vector3 worldPosition) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI) - 180f);
	}
	public static void LookAwayFrom2D(this Transform t, Transform target) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI) - 180f);
	}
	public static void LookAt2D(this Transform t, Vector3 worldPosition) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI);
	}
	public static void LookAt2D(this Transform t, Transform target) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI);
	}
}