using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridDungeonGeneration : MonoBehaviour
{
    [Header("Generation")] [SerializeField]
    private int _roomAmount = 10;

    [SerializeField] private int _specialRoom = 1;
    [SerializeField] private int _bossRoom = 1;
    [SerializeField] private int _distanceSpecialRoom = 3;
    [SerializeField] private int _distanceBossRoom = 3;
    private int _indexRoom = 0;

    [SerializeField] private float _offSet = 15f;

    [Header("Room")] [SerializeField] private SpawnRoom _spawn;
    [SerializeField] private RoomClassic _classic;
    [SerializeField] private RoomTrade _trade;
    [SerializeField] private BossRoom _boss;

    private List<Room> _rooms = new();

    [Header("Random Seed")] [SerializeField]
    private int _seed;

    [SerializeField] private bool _randomSeed = true;
    private System.Random _random;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_randomSeed) _seed = Random.Range(-10000, 10000);

        _random = new System.Random(_seed);
   
        Room[] rooms = GetComponentsInChildren<Room>();
        foreach (Room room in rooms) Destroy(room.gameObject);
        
        _indexRoom = 0;
        
        GenerateSpawnRoom();
        GenerateRoom();
        GenerateRoomView();
    }

    void GenerateRoomView()
    {
        foreach (Room rooms in _rooms)
        {
            rooms.UpdateView();
        }
    }
    void GenerateRoom()
    {
        while (true)
        {
            int indexRoom = _indexRoom;

            //Si je n'est pas atteint mon nombre de salle attendue
            if (_indexRoom < _roomAmount) 
            {
                PatternClassicRoom(false);

                if (indexRoom != _indexRoom) continue;

                // Pas assez de salle donc reset => a corriger pour ne pas changer de seed mais de trouver une solution
                if (indexRoom == _indexRoom)
                {
                    _randomSeed = true;
                    Init();
                    return;
                }
            }

            if (PatternClassicRoom(true).Count == 0 || _indexRoom >= _roomAmount)
            {
                PatternClassicRoom(true);                    
                
                // Pas assez de salle donc reset => a corriger pour ne pas changer de seed mais de trouver une solution
                _randomSeed = true;
                if (_indexRoom < _roomAmount || _specialRoom > 0 ||  _bossRoom > 0) Init();
                
                return;
            }
        }
    }

    List<Room> PatternClassicRoom(bool isEnd)
    {
        List<Room> roomClassicStack = new();

        foreach (Room rooms in _rooms)
        {
            for (int i = 0; i < rooms.Doors.Length; i++)
            {
                if (!rooms.Doors[i].GetIsActivate()) continue;

                Direction actualDirection = rooms.Doors[i].GetDirection();
                Direction nextDirection = ConvertDirection.ConvertInverseDirectionInt(actualDirection);
                Vector2Int nextPos = ConvertDirection.CalculateCoordinate(actualDirection);
                Room classicRoom = null;
                Room nextRoom = GetRoom(rooms.PositionRoom + nextPos);

                if (AlreadyRoom(_rooms, rooms.PositionRoom + nextPos))
                {
                    rooms.AddNewPart(actualDirection);
                    if(nextRoom != null)
                    {
                        RoomDoor(rooms, GetRoom(rooms.PositionRoom + nextPos), i, nextDirection);
                        GetRoom(rooms.PositionRoom + nextPos).AddNewPart(nextDirection);
                    }
                    
                    continue;
                }
                
                //Doit mettre a jour la porte deja mise de la salle pour ajouter celle en face
                if (AlreadyRoom(roomClassicStack, rooms.PositionRoom + nextPos))
                {
                    rooms.AddNewPart(actualDirection);
                    if(nextRoom != null)
                    {
                       RoomDoor(rooms, GetRoom(rooms.PositionRoom + nextPos), i, nextDirection);
                       GetRoom(rooms.PositionRoom + nextPos).AddNewPart(nextDirection);
                    }
                    
                    //_rooms.ResetDoorPart(actualDirection);
                    continue;
                }
                
                classicRoom = SpawnRoom(rooms, actualDirection);
                RoomDoor(rooms, classicRoom, i, nextDirection);
                RoomName(rooms);

                _indexRoom++;

                //lui envoyer sa porte aussi et non que linverse
                classicRoom.UpdateRoom(_random, rooms.PositionRoom + nextPos, nextDirection, false, isEnd);

                roomClassicStack.Add(classicRoom);
            }

            rooms.IsValidate = true;
        }

        foreach (Room roomClassic in roomClassicStack) _rooms.Add(roomClassic);

        return roomClassicStack;
    }
    
    Room SpawnRoom(Room _rooms, Direction _actualDirection)
    {         
        int pourcentageSpecialRoom = _random.Next(0, 100);
        
        if (_specialRoom != 0 && _rooms.PowerDistance() >= _distanceSpecialRoom && pourcentageSpecialRoom <= CalculatePourcentageSpecial())
        {
            _specialRoom--;
            return Instantiate(_trade, _rooms.transform.position + OffSetPosition(_actualDirection),
                Quaternion.identity, transform);
        }
        
        int pourcentageBossRoom = _random.Next(0, 100);
        
        if (_bossRoom != 0 &&
            _rooms.PowerDistance() >= _distanceBossRoom && pourcentageBossRoom <= CalculatePourcentageSpecial())
        {
            _bossRoom--;
            return Instantiate(_boss, _rooms.transform.position + OffSetPosition(_actualDirection),
                Quaternion.identity, transform);
        }
        
        return Instantiate(_classic, _rooms.transform.position + OffSetPosition(_actualDirection), Quaternion.identity, transform);
    }

    void RoomDoor(Room rooms, Room classicRoom, int i, Direction nextDirection)
    {
        rooms.Doors[i].SetExitDoor(classicRoom.Doors[(int) nextDirection]);
        classicRoom.Doors[(int) nextDirection].SetExitDoor(rooms.Doors[i]);
    }

    void RoomName(Room classicRoom)
    {
        if (_roomAmount < _indexRoom)
            classicRoom.name = classicRoom.name + " / -" + classicRoom.PositionRoom + "- | Init";
        else classicRoom.name = classicRoom.name + " | DeadEnd";
    }

    bool AlreadyRoom(List<Room> rooms, Vector2Int newRoomPos)
    {
        foreach (Room room in rooms)
        {
            if (room.PositionRoom == newRoomPos) return true;
        }

        return false;
    }
    
    Room GetRoom(Vector2Int newRoomPos)
    {
        foreach (Room room in _rooms)
        {
            if (room.PositionRoom == newRoomPos) return room;
        }

        return null;
    }

    void GenerateSpawnRoom()
    {
        SpawnRoom spawnRoom = Instantiate(_spawn, transform.position, Quaternion.identity, transform);
        spawnRoom.UpdateRoom(_random, new Vector2Int(0, 0), Direction.north, true, false);
        _rooms.Add(spawnRoom);
    }

    float CalculatePourcentageSpecial()
    {
        int max = _roomAmount - _distanceSpecialRoom;
        return (_indexRoom / max) * 100;
    }

    Vector3 OffSetPosition(Direction direction)
    {
        Vector3 newPos = Vector3.zero;
        switch (direction)
        {
            case Direction.north:
                newPos.z += _offSet;
                break;
            case Direction.south:
                newPos.z += -_offSet;
                break;
            case Direction.west:
                newPos.x += _offSet;
                break;
            case Direction.east:
                newPos.x += -_offSet;
                break;
        }

        return newPos;
    }
}
