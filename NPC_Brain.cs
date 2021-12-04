using UnityEngine;
using System.Collections;

// Creates a Character Controller if it does not exist for the NPC...so I don't have to make one manually in Unity
[RequireComponent(typeof(CharacterController))]

public class NPC_Brain : MonoBehaviour
{
    // The speed at which the NPC walk
	public float walking_speed = 0.5f;
    // Time to complete rotation
	public float fracComplete = 1;
    // When finding a new direction the NPC can rotate at +/- 60 degrees
	public float rotationAllowance = 60;

    // Holds the vector of the new rotation direction
	Vector3 newRotation;
    // Controller object for the NPC
	CharacterController controller;

	void Awake()
	{
        // Character controller helps us to detect collision using Unity's 
		controller = GetComponent<CharacterController>();

		// Initially, the NPC moves in a random direction
        // Perform a "direction" rotation about the y axis
		transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);

        // Allows us to pause a task and continue where we left off in the next frame
		StartCoroutine(FindANewDirection());
	}

	void Update()
	{
        // Spherical interpolation from current angle "transfrom.eulerAngles" to new angle "newRotation"
        // This interpolation happens over "Time.deltaTime * fracComplete" to ensure a smooth rotation
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, newRotation, Time.deltaTime * fracComplete);
        // Vector3.forward is equivalent to < 0, 0, 1 >, which is essentially the blue axis in Unity
        // SimpleMove ignores velocity in the y direction
		controller.SimpleMove(transform.TransformDirection(Vector3.forward) * walking_speed);
	}

	IEnumerator FindANewDirection()
	{
        // Always true, so it infinitely runs
		while (true)
		{
			// Find a new direction to move towards
			// Rotate -60 or 60 from original position
			float left = transform.eulerAngles.y - rotationAllowance;
			float right = transform.eulerAngles.y + rotationAllowance;

			// Randomly choose a direction from +- 60 degrees
			// Update the the new direction
			newRotation = new Vector3(0, Random.Range(left, right), 0);
			// When using iterators, yield return will return one element at a time
			yield return new WaitForSeconds(fracComplete);
		}
	}
}
