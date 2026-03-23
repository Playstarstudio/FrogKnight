using UnityEngine;
using System.Collections;
  
public static class TransformExtensions { //Original code taken from lovely unity forum user capz-nst

	//This creates transform and vector3 versions of Unity's built in LookAt that work for 2D objects
	public static void LookAt2D(this Transform t, Vector3 worldPosition) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI);
	}
	public static void LookAt2D(this Transform t, Transform target) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI);
	}

    //This creates transform and vector3 versions of Unity's built in LookAt that work for 2D objects but rotate AWAY from desired obj
    public static void LookAwayFrom2D(this Transform t, Vector3 worldPosition) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI) - 180f);
	}
	public static void LookAwayFrom2D(this Transform t, Transform target) {
		t.rotation = Quaternion.identity;
		t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI) - 180f);
	}
}