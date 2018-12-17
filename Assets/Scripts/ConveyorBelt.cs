using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConveyorBelt : MonoBehaviour {

    public float speed;
    public ObstacleType beltType;
    public int beltDirection;

    public SpriteRenderer spriteRenderer;
    BoxCollider2D box;
    Vector2 offset;
    Vector2 spriteDir;
	
	// Update is called once per frame
	void Update () {
        offset += new Vector2(0, 0.1f) * Time.deltaTime;
        spriteRenderer.material.SetTextureOffset("_MainTex", offset);

        if(transform.childCount > 0)
        {
            List<Transform> children =GetComponentsInChildren<Transform>().ToList();
            children.Remove(transform);
            children.Remove(spriteRenderer.transform);

            foreach(Transform child in children)
            {
                // Don't effect nested objects
                if(child.parent == transform)
                {
                    if (beltType == ObstacleType.Horiztonal)
                    {
                        child.localPosition += transform.right * (beltDirection * Time.deltaTime * speed);
                    }
                    else
                    {
                       child.localPosition += transform.up * (beltDirection * Time.deltaTime * speed);
                    }
                }
            }
        }
    }

    
}
