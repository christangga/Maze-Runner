using UnityEngine;
using System.Collections;

public class CameraControllers : MonoBehaviour {

	public GameObject character;
	private Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position=character.transform.position + offset;
	}
}
