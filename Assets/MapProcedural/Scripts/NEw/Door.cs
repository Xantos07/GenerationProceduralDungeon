using UnityEngine;

namespace NewGeneration
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Direction direction;
        [SerializeField] private Door exitDoor;
        [SerializeField] private bool isActivate = false;
     
        public Direction GetDirection() =>   direction;
        public bool GetIsActivate() =>   isActivate;
        
        public Door GetExitDoor() =>   exitDoor;
        public void SetExitDoor(Door _Door) => exitDoor = _Door;
        public void SetIsActivate(bool _isActivate) => isActivate = _isActivate;
        
    }
}
