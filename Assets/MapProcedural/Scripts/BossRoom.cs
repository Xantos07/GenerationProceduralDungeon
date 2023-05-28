using UnityEngine;

public class BossRoom : Room
{ 
    public override void AddFloor()
    {
        for (int x = 0; x < _roomBuilding.GetIsFloor().GetLength(0); x++)
        {
            for (int y = 0; y < _roomBuilding.GetIsFloor().GetLength(1); y++)
            {
                _roomBuilding.SetIsFloor(new Vector2Int(x,y), true);
            }
        }
    }
}
