using System.Collections.Generic;
using UnityEngine;

public class RoomBuilding : MonoBehaviour
{
    //Part to build
    protected const int SIZE = 3;
    [SerializeField] private float _offSet = 10f;
    [SerializeField] private float _offSetHeight = 2.5f;
    [SerializeField] private Partition _partition;
    [SerializeField] private Floor _floor;
    bool[,] isFloor = new bool[3, 3];  

    List<Partition> _externalPartition = new List<Partition>();
    [SerializeField] Material _material;
    public bool[,] GetIsFloor() => isFloor;
    public void SetIsFloor(Vector2Int isFloorCoord, bool floor) => isFloor[isFloorCoord.x, isFloorCoord.y] = floor;
    
    void CrossFloor(Direction doorSpawnning)
    {
        switch (doorSpawnning)
        {
            case Direction.north:
                isFloor[1, 2] = true;
                break;
            case Direction.south :
                isFloor[1, 0] = true;
                break;
            case Direction.east :
                isFloor[0, 1] = true;
                break;
            case Direction.west :
                isFloor[2, 1] = true;
                break;
        }
    }
    
    void CheckAloneFloor()
    {
        if (isFloor[0, 0])
            if (!isFloor[1, 0] && !isFloor[0, 1]) isFloor[0, 1] = true;
            
        
        if (isFloor[2, 0])
            if (!isFloor[2, 1] && !isFloor[1, 0]) isFloor[1, 0] = true;
            
        
        if (isFloor[2, 2])
            if (!isFloor[2, 1] && !isFloor[1, 2]) isFloor[1, 2] = true;

        if (isFloor[0, 2])
            if (!isFloor[0, 1] && !isFloor[1, 2]) isFloor[1, 2] = true;
    }
    
    public void BuildFloor(List<Direction> doorSpawnning)
    {
        //Construction de la taille de notre salle 
        Floor[,] floors = new Floor[SIZE, SIZE];
        
        foreach (Direction dir in doorSpawnning) CrossFloor(dir);   
        
        isFloor[1, 1] = true;
        
        // CHECK SLOT
        CheckAloneFloor();
        
        int x = 0;
        int y = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (isFloor[x, y])
                {
                    Floor _floorInst = BuildElement<Floor>(_floor.gameObject,
                        new Vector3(transform.position.x + i * _offSet, 0, transform.position.z + j * _offSet),
                        Quaternion.identity, transform).GetComponent<Floor>();
                    _floorInst.name = " FLOOR : " + new Vector2Int(x, y);
                    floors[x, y] = _floorInst;
                    
                    _floorInst.GetComponent<MeshRenderer>().material = _material;
                    
                    CheckAdjacentFloor(new Vector2Int(x, y), _floorInst);
                }
                y ++;
            }

            x++;
            y = 0;
        }
        
        BuildWallDirection(floors, _partition.gameObject);
    }
    
    void CheckAdjacentFloor(Vector2Int coord, Floor floor)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

                //SI nous somms à l'exterieur de notre tableau && que nous avons un floor dans une certaine direction
                if (IsInSide(new Vector2Int(coord.x + x, coord.y + y)) && isFloor[coord.x + x, coord.y + y])
                {
                    floor.AddDirection(ConvertDirection.CalculateDirection(new Vector2Int(x, y)));
                    continue;
                }

                floor.NoAdjacent(ConvertDirection.CalculateDirection(new Vector2Int(x, y)));
            }
        }
    }

    bool IsInSide(Vector2Int coord)
    {
        if (coord.x < SIZE && coord.x >= 0 && coord.y < SIZE && coord.y >= 0)
        {
            return true;
        }
        
        return false;
    }

    void BuildWallDirection(Floor[,] floors, GameObject element)
    {
        Vector3 pos = new Vector3(0, 0, 0);
        Partition partition = null;

        for (int x = 0; x < floors.GetLength(0); x++)
        {
            for (int y = 0; y < floors.GetLength(1); y++)
            {
                if (floors[x, y] == null) continue;

                for (int i = 0; i < floors[x, y].GetNoAdjacentFloor().Count; i++)
                {
                    switch (floors[x, y].GetNoAdjacentFloor()[i])
                    {
                        case Direction.north:
                            pos = new Vector3(floors[x, y].transform.position.x, _offSetHeight,
                                floors[x, y].transform.position.z + _offSet / 2);
                            BuildElement<Partition>(element, pos, Quaternion.Euler(0, 0, 0), floors[x, y].transform).UpdateView(true);
                            break;
                        case Direction.south:
                            pos = new Vector3(floors[x, y].transform.position.x, _offSetHeight,
                                floors[x, y].transform.position.z - _offSet / 2);
                            BuildElement<Partition>(element, pos, Quaternion.Euler(0, 0, 0), floors[x, y].transform).UpdateView(true);
                            break;
                        case Direction.west:
                            pos = new Vector3(floors[x, y].transform.position.x - _offSet / 2, _offSetHeight,
                                floors[x, y].transform.position.z);
                            BuildElement<Partition>(element, pos, Quaternion.Euler(0, 90, 0), floors[x, y].transform).UpdateView(true);
                            break;
                        case Direction.east:
                            pos = new Vector3(floors[x, y].transform.position.x + _offSet / 2, _offSetHeight,
                                floors[x, y].transform.position.z);
                            BuildElement<Partition>(element, pos, Quaternion.Euler(0, 90, 0), floors[x, y].transform).UpdateView(true);
                            break;
                    }
                }
            }
        }
    }

    void AddPartition(Partition partition, Vector2Int pos, Direction direction)
    {
        _externalPartition.Add(this._partition);
    }

    /// <summary>
    /// Construit un objet que l'on veut exemple: un mur, sol, porte etc ... 
    /// </summary>
    T BuildElement<T>(GameObject element, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(element, position, rotation, parent).GetComponent<T>();
    }
}


