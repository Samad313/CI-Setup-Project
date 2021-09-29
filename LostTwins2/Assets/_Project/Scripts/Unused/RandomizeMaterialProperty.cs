using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeMaterialProperty : MonoBehaviour
{
	[SerializeField]
	private string materialPropertyName = "_Color";

	[SerializeField]
	private Vector2 minMaxValue = new Vector2(0, 1);

    // Start is called before the first frame update
    void Awake()
    {

		MaterialPropertyBlock matpropertyBlock = new MaterialPropertyBlock();
		float value = Random.Range(minMaxValue.x, minMaxValue.y);
		matpropertyBlock.SetFloat(materialPropertyName, value);
		GetComponent<MeshRenderer>().SetPropertyBlock(matpropertyBlock);
	}
}
