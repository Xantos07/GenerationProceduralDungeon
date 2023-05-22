using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private List<Direction> adjacentFloor = new();
    [SerializeField] private List<Direction> noAdjacentFloor = new();
    [SerializeField] private Vector2Int pos = new Vector2Int();

    /// <summary>
    /// AJout d'une direction
    /// </summary>
    public void AddDirection(Direction _dir) => adjacentFloor.Add(_dir);
    public void NoAdjacent(Direction _dir) => noAdjacentFloor.Add(_dir);
    /// <summary>
    /// Enelever une direction
    /// </summary>
    public void RemoveDirection(Direction _dir) => adjacentFloor.Remove(_dir);

    public List<Direction> GetDirection( ) => adjacentFloor;
    public List<Direction> GetNoAdjacentFloor( ) => noAdjacentFloor;
    
    public Vector2Int GetPosition( ) => pos;
    public void SetPosition(Vector2Int _pos) => pos = _pos;
}
