using UnityEngine;

public class SelfDestroyScript : MonoBehaviour 
{
	[SerializeField] private float destroyDelay = 3.0f;
	private void Start() 
	{
		Destroy(gameObject, destroyDelay);
	}
}
