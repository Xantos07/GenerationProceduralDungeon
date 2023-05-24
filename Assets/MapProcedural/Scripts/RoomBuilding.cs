using System.Collections.Generic;
using UnityEngine;

namespace NewGeneration
{
    public class RoomBuilding : MonoBehaviour
    {
        //Part to build
        protected const int SIZE = 3;
        [SerializeField] private float offSet = 10f;
        [SerializeField] private float offSetHeight = 2.5f;
        [SerializeField] private Partition partition;
        [SerializeField] private Floor floor;
        [SerializeField] bool[,] isFloor = new bool[3, 3];  

        List<Partition> _externalPartition = new List<Partition>();
        [SerializeField] Material material;
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
            //COnstruction de la taille de notre salle 
            Floor[,] floors = new Floor[SIZE, SIZE];
            //isFloor = new bool[SIZE, SIZE];
            
            foreach (Direction dir in doorSpawnning) CrossFloor(dir);   
            
            isFloor[1, 1] = true;
            
            // CHECK SLOT
            CheckAloneFloor();
            
            int _x = 0;
            int _y = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (isFloor[_x, _y])
                    {
                        Floor _floorInst = BuildElement<Floor>(floor.gameObject,
                            new Vector3(transform.position.x + x * offSet, 0, transform.position.z + y * offSet),
                            Quaternion.identity, transform).GetComponent<Floor>();
                        _floorInst.name = " FLOOR : " + new Vector2Int(_x, _y);
                        floors[_x, _y] = _floorInst;
                        
                        _floorInst.GetComponent<MeshRenderer>().material = material;
                        
                        CheckAdjacentFloor(new Vector2Int(_x, _y), _floorInst);
                    }
                    _y ++;
                }

                _x++;
                _y = 0;
            }
            
            BuildWallDirection(floors, partition.gameObject);
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
            Vector3 _pos = new Vector3(0, 0, 0);
            Partition _partition = null;

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
                                _pos = new Vector3(floors[x, y].transform.position.x, offSetHeight,
                                    floors[x, y].transform.position.z + offSet / 2);
                                BuildElement<Partition>(element, _pos, Quaternion.Euler(0, 0, 0), floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.south:
                                _pos = new Vector3(floors[x, y].transform.position.x, offSetHeight,
                                    floors[x, y].transform.position.z - offSet / 2);
                                BuildElement<Partition>(element, _pos, Quaternion.Euler(0, 0, 0), floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.west:
                                _pos = new Vector3(floors[x, y].transform.position.x - offSet / 2, offSetHeight,
                                    floors[x, y].transform.position.z);
                                BuildElement<Partition>(element, _pos, Quaternion.Euler(0, 90, 0), floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.east:
                                _pos = new Vector3(floors[x, y].transform.position.x + offSet / 2, offSetHeight,
                                    floors[x, y].transform.position.z);
                                BuildElement<Partition>(element, _pos, Quaternion.Euler(0, 90, 0), floors[x, y].transform).UpdateView(true);
                                break;
                        }
                    }
                }
            }
        }

        void AddPartition(Partition partition, Vector2Int pos, Direction direction)
        {
            _externalPartition.Add(this.partition);
        }

        /// <summary>
        /// Construit un objet que l'on veut exemple: un mur, sol, porte etc ... 
        /// </summary>
        T BuildElement<T>(GameObject element, Vector3 position, Quaternion rotation, Transform parent)
        {
            return Instantiate(element, position, rotation, parent).GetComponent<T>();
        }
    }
}

