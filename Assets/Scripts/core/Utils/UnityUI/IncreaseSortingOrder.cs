using UnityEngine;

public class IncreaseSortingOrder : MonoBehaviour
{
	static int renderOrderOffset = 0;

	void Start ()
	{
		renderOrderOffset += 1000;

		foreach (var renderer in GetComponentsInChildren<Renderer>())
			renderer.sortingOrder += renderOrderOffset;
	}
}
