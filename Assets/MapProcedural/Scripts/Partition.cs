using UnityEngine;
public class Partition : MonoBehaviour
{ 
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _wall;

    public void UpdateView(bool isWall)
    {
        if (isWall)
        {
            _wall.SetActive(true);
            _door.SetActive(false);
            return;
        }
        
        _wall.SetActive(false);
        _door.SetActive(true);
    }
}
