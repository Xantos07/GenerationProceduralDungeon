using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [field:SerializeField] public List<Direction> _adjacentFloor  { get; set; } = new();
    [field:SerializeField] public List<Direction> _noAdjacentFloor  { get; set; } = new();
    [field:SerializeField] public Vector2Int _pos { get; set; } = new Vector2Int();
    [field:SerializeField] public DecorativeEnvironment _decorativeEnvironment { get; set; }
    [field:SerializeField] public MeshRenderer[] Slots { get; set; }
}
