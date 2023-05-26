using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NewGeneration
{
    public class GridDungeonGeneration : MonoBehaviour
    {
        [Header("Generation")] [SerializeField]
        private int roomAmount = 10;

        [SerializeField] private int specialRoom = 1;
        [SerializeField] private int bossRoom = 1;
        [SerializeField] private int distanceSpecialRoom = 3;
        [SerializeField] private int distanceBossRoom = 3;
        private int indexRoom = 0;

        [SerializeField] private float offSet = 15f;

        [Header("Room")] [SerializeField] private SpawnRoom spawn;
        [SerializeField] private RoomClassic classic;
        [SerializeField] private RoomTrade trade;
        [SerializeField] private BossRoom boss;

        private List<Room> rooms = new();

        [Header("Random Seed")] [SerializeField]
        private int seed;

        [SerializeField] private bool randomSeed = true;
        private System.Random random;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (randomSeed) seed = Random.Range(-10000, 10000);

            random = new System.Random(seed);
       
            Room[] rooms = GetComponentsInChildren<Room>();
            foreach (Room _room in rooms) Destroy(_room.gameObject);
            
            indexRoom = 0;
            
            GenerateSpawnRoom();
            GenerateRoom();
            GenerateRoomView();
        }

        void GenerateRoomView()
        {
            foreach (Room _rooms in rooms)
            {
                _rooms.UpdateView();
            }
        }
        void GenerateRoom()
        {
            while (true)
            {
                int _indexRoom = indexRoom;

                //Si je n'est pas atteint mon nombre de salle attendue
                if (indexRoom < roomAmount) 
                {
                    PatternClassicRoom(false);

                    if (_indexRoom != indexRoom) continue;

                    // Pas assez de salle donc reset => a corriger pour ne pas changer de seed mais de trouver une solution
                    if (_indexRoom == indexRoom)
                    {
                        randomSeed = true;
                        Init();
                        return;
                    }
                }

                if (PatternClassicRoom(true).Count == 0 || indexRoom >= roomAmount)
                {
                    PatternClassicRoom(true);                    
                    
                    // Pas assez de salle donc reset => a corriger pour ne pas changer de seed mais de trouver une solution
                    randomSeed = true;
                    if (indexRoom < roomAmount || specialRoom > 0 ||  bossRoom > 0) Init();
                    
                    return;
                }
            }
        }

        List<Room> PatternClassicRoom(bool _isEnd)
        {
            List<Room> _roomClassicStack = new();

            foreach (Room _rooms in rooms)
            {
                for (int i = 0; i < _rooms.doors.Length; i++)
                {
                    if (!_rooms.doors[i].GetIsActivate()) continue;

                    Direction actualDirection = _rooms.doors[i].GetDirection();
                    Direction nextDirection = ConvertDirection.ConvertInverseDirectionInt(actualDirection);
                    Vector2Int nextPos = ConvertDirection.CalculateCoordinate(actualDirection);
                    Room _classicRoom = null;
                    Room nextRoom = GetRoom(_rooms.PositionRoom + nextPos);

                    if (AlreadyRoom(rooms, _rooms.PositionRoom + nextPos))
                    {
                        _rooms.AddNewPart(actualDirection);
                        if(nextRoom != null)
                        {
                            RoomDoor(_rooms, GetRoom(_rooms.PositionRoom + nextPos), i, nextDirection);
                            GetRoom(_rooms.PositionRoom + nextPos).AddNewPart(nextDirection);
                        }
                        
                        continue;
                    }
                    
                    //Doit mettre a jour la porte deja mise de la salle pour ajouter celle en face
                    if (AlreadyRoom(_roomClassicStack, _rooms.PositionRoom + nextPos))
                    {
                        _rooms.AddNewPart(actualDirection);
                        if(nextRoom != null)
                        {
                           RoomDoor(_rooms, GetRoom(_rooms.PositionRoom + nextPos), i, nextDirection);
                           GetRoom(_rooms.PositionRoom + nextPos).AddNewPart(nextDirection);
                        }
                        
                        //_rooms.ResetDoorPart(actualDirection);
                        continue;
                    }
                    
                    _classicRoom = SpawnRoom(_rooms, actualDirection);
                    RoomDoor(_rooms, _classicRoom, i, nextDirection);
                    RoomName(_rooms);

                    indexRoom++;

                    //lui envoyer sa porte aussi et non que linverse
                    _classicRoom.UpdateRoom(random, _rooms.PositionRoom + nextPos, nextDirection, false, _isEnd);

                    _roomClassicStack.Add(_classicRoom);
                }

                _rooms.IsValidate = true;
            }

            foreach (Room roomClassic in _roomClassicStack) rooms.Add(roomClassic);

            return _roomClassicStack;
        }
        
        Room SpawnRoom(Room _rooms, Direction _actualDirection)
        {         
            int pourcentageSpecialRoom = random.Next(0, 100);
            
            if (specialRoom != 0 && _rooms.PowerDistance() >= distanceSpecialRoom && pourcentageSpecialRoom <= CalculatePourcentageSpecial())
            {
                specialRoom--;
                return Instantiate(trade, _rooms.transform.position + OffSetPosition(_actualDirection),
                    Quaternion.identity, transform);
            }
            
            int pourcentageBossRoom = random.Next(0, 100);
            
            if (bossRoom != 0 &&
                _rooms.PowerDistance() >= distanceBossRoom && pourcentageBossRoom <= CalculatePourcentageSpecial())
            {
                bossRoom--;
                return Instantiate(boss, _rooms.transform.position + OffSetPosition(_actualDirection),
                    Quaternion.identity, transform);
            }
            
            return Instantiate(classic, _rooms.transform.position + OffSetPosition(_actualDirection), Quaternion.identity, transform);
        }

        void RoomDoor(Room _rooms, Room _classicRoom, int _i, Direction _nextDirection)
        {
            _rooms.doors[_i].SetExitDoor(_classicRoom.doors[(int) _nextDirection]);
            _classicRoom.doors[(int) _nextDirection].SetExitDoor(_rooms.doors[_i]);
        }

        void RoomName(Room _classicRoom)
        {
            if (roomAmount < indexRoom)
                _classicRoom.name = _classicRoom.name + " / -" + _classicRoom.PositionRoom + "- | Init";
            else _classicRoom.name = _classicRoom.name + " | DeadEnd";
        }

        bool AlreadyRoom(List<Room> _rooms, Vector2Int _newRoomPos)
        {
            foreach (Room _room in _rooms)
            {
                if (_room.PositionRoom == _newRoomPos) return true;
            }

            return false;
        }
        
        Room GetRoom(Vector2Int _newRoomPos)
        {
            foreach (Room _room in rooms)
            {
                if (_room.PositionRoom == _newRoomPos) return _room;
            }

            return null;
        }

        void GenerateSpawnRoom()
        {
            SpawnRoom _spawnRoom = Instantiate(spawn, transform.position, Quaternion.identity, transform);
            _spawnRoom.UpdateRoom(random, new Vector2Int(0, 0), Direction.north, true, false);
            rooms.Add(_spawnRoom);
        }

        float CalculatePourcentageSpecial()
        {
            int _max = roomAmount - distanceSpecialRoom;
            return (indexRoom / _max) * 100;
        }

        Vector3 OffSetPosition(Direction _direction)
        {
            Vector3 newPos = Vector3.zero;
            switch (_direction)
            {
                case Direction.north:
                    newPos.z += offSet;
                    break;
                case Direction.south:
                    newPos.z += -offSet;
                    break;
                case Direction.west:
                    newPos.x += offSet;
                    break;
                case Direction.east:
                    newPos.x += -offSet;
                    break;
            }

            return newPos;
        }
    }
}