using UnityEngine;
public class Partition : MonoBehaviour
{ 
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject wall;

    public void UpdateView(bool _isWall)
    {
        if (_isWall)
        {
            wall.SetActive(true);
            door.SetActive(false);
            return;
        }
        
        wall.SetActive(false);
        door.SetActive(true);
    }
}
