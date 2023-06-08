using UnityEngine;

public class DecorativeEnvironment : MonoBehaviour
{
    [SerializeField] private GameObject[] environmentalObjectsPattern;

    public void BuildDecorativeObject(Vector3 pos)
    {
        int randomRange = Random.Range(0, environmentalObjectsPattern.Length);
        Instantiate(environmentalObjectsPattern[randomRange], pos, Quaternion.identity);
    }
}
