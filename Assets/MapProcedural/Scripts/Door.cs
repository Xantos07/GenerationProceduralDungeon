using UnityEngine;
public class Door : MonoBehaviour
{
     [SerializeField] private Direction direction;
     private Door exitDoor;
     
     public Door GetExitDoor()
     {
         return exitDoor;
     }
     
     public void SetExitDoor(Door _Door)
     {
          exitDoor = _Door;
     }
}


