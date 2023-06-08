using UnityEngine;

public class DecorativeEnvironment : MonoBehaviour
{
    [SerializeField] private EnvironmentalObject[] environmentalObjects;

    public void BuildDecorativeObject(Vector2Int pos)
    {
        Instantiate(environmentalObjects[0], new Vector3(pos.x,0,pos.y), Quaternion.identity, transform);
    }
}
