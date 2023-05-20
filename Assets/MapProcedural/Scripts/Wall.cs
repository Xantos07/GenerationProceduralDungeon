using UnityEngine;

public class Wall : Partition
{
    [SerializeField] private Direction direction;
    [SerializeField] private bool ExternWall = false;
}
