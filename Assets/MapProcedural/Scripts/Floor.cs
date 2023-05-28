using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private List<Direction> _adjacentFloor = new();
    [SerializeField] private List<Direction> _noAdjacentFloor = new();
    [SerializeField] private Vector2Int _pos = new Vector2Int();

    /// <summary>
    /// AJout d'une direction
    /// </summary>
    public void AddDirection(Direction _dir) => _adjacentFloor.Add(_dir);
    public void NoAdjacent(Direction _dir) => _noAdjacentFloor.Add(_dir);
    /// <summary>
    /// Enelever une direction
    /// </summary>
    public void RemoveDirection(Direction _dir) => _adjacentFloor.Remove(_dir);

    public List<Direction> GetDirection( ) => _adjacentFloor;
    public List<Direction> GetNoAdjacentFloor( ) => _noAdjacentFloor;
    
    public Vector2Int GetPosition( ) => _pos;
    public void SetPosition(Vector2Int _pos) => this._pos = _pos;
}
