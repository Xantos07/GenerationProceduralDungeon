using UnityEngine;

public class Wall : Partition
{
    [SerializeField] private Direction _direction;
    [SerializeField] private bool _externWall = false;
}
