using UnityEngine;

[ExecuteInEditMode]
public class IsometricSpriteRenderer : MonoBehaviour 
{
	void Update ()
	{
		GetComponent<Renderer>().sortingOrder = (int)(transform.position.y * -10);
	}
}
