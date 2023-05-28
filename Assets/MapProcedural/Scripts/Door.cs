using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Direction _direction;
    [SerializeField] private Door _exitDoor;
    [SerializeField] private bool _isActivate = false;
 
    public Direction GetDirection() => _direction;
    public bool GetIsActivate() => _isActivate;
    
    public Door GetExitDoor() => _exitDoor;
    public void SetExitDoor(Door door) => _exitDoor = door;
    public void SetIsActivate(bool isActivate) => _isActivate = isActivate;
    
}

