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
        public bool[,] isFloor = new bool[3, 3];

        List<Partition> externalPartition = new List<Partition>();
        [SerializeField] Material material;
        void SetFloor(Direction _doorSpawnning)
        {
            switch (_doorSpawnning)
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
        
        public void BuildFloor(List<Direction> _doorSpawnning)
        {
            //COnstruction de la taille de notre salle 
            Floor[,] _floors = new Floor[SIZE, SIZE];
            //isFloor = new bool[SIZE, SIZE];
            
            foreach (Direction _dir in _doorSpawnning) SetFloor(_dir);   
            
            isFloor[1, 1] = true;
            
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
                        _floors[_x, _y] = _floorInst;
                        
                        _floorInst.GetComponent<MeshRenderer>().material = material;
                        
                        CheckAdjacentFloor(new Vector2Int(_x, _y), _floorInst);
                    }
                    _y ++;
                }

                _x++;
                _y = 0;
            }
            
            // CHECK SLOT
            CheckAloneFloor();
            BuildWallDirection(_floors, partition.gameObject);
        }
        
        public void CheckAdjacentFloor(Vector2Int _coord, Floor _floor)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Mathf.Abs(x) == Mathf.Abs(y)) continue;

                    //SI nous somms à l'exterieur de notre tableau && que nous avons un floor dans une certaine direction
                    if (IsInSide(new Vector2Int(_coord.x + x, _coord.y + y)) && isFloor[_coord.x + x, _coord.y + y])
                    {
                        _floor.AddDirection(ConvertDirection.CalculateDirection(new Vector2Int(x, y)));
                        continue;
                    }

                    _floor.NoAdjacent(ConvertDirection.CalculateDirection(new Vector2Int(x, y)));
                }
            }
        }

        public bool IsInSide(Vector2Int _coord)
        {
            if (_coord.x < SIZE && _coord.x >= 0 && _coord.y < SIZE && _coord.y >= 0)
            {
                return true;
            }
            
            return false;
        }

        public void BuildWallDirection(Floor[,] _floors, GameObject _element)
        {
            Vector3 _pos = new Vector3(0, 0, 0);
            Partition _partition = null;

            for (int x = 0; x < _floors.GetLength(0); x++)
            {
                for (int y = 0; y < _floors.GetLength(1); y++)
                {
                    if (_floors[x, y] == null) continue;

                    for (int i = 0; i < _floors[x, y].GetNoAdjacentFloor().Count; i++)
                    {
                        switch (_floors[x, y].GetNoAdjacentFloor()[i])
                        {
                            case Direction.north:
                                _pos = new Vector3(_floors[x, y].transform.position.x, offSetHeight,
                                    _floors[x, y].transform.position.z + offSet / 2);
                                BuildElement<Partition>(_element, _pos, Quaternion.Euler(0, 0, 0), _floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.south:
                                _pos = new Vector3(_floors[x, y].transform.position.x, offSetHeight,
                                    _floors[x, y].transform.position.z - offSet / 2);
                                BuildElement<Partition>(_element, _pos, Quaternion.Euler(0, 0, 0), _floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.west:
                                _pos = new Vector3(_floors[x, y].transform.position.x - offSet / 2, offSetHeight,
                                    _floors[x, y].transform.position.z);
                                BuildElement<Partition>(_element, _pos, Quaternion.Euler(0, 90, 0), _floors[x, y].transform).UpdateView(true);
                                break;
                            case Direction.east:
                                _pos = new Vector3(_floors[x, y].transform.position.x + offSet / 2, offSetHeight,
                                    _floors[x, y].transform.position.z);
                                BuildElement<Partition>(_element, _pos, Quaternion.Euler(0, 90, 0), _floors[x, y].transform).UpdateView(true);
                                break;
                        }
                    }
                }
            }
        }

        void AddPartition(Partition _partition, Vector2Int _pos, Direction _direction)
        {
            externalPartition.Add(partition);
        }

        /// <summary>
        /// Construit un objet que l'on veut exemple: un mur, sol, porte etc ... 
        /// </summary>
        public T BuildElement<T>(GameObject _element, Vector3 _position, Quaternion _rotation, Transform _parent)
        {
            return Instantiate(_element, _position, _rotation, _parent).GetComponent<T>();
        }
    }
}

