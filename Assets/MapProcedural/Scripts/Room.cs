using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public abstract class Room : MonoBehaviour
{
     public Door[] Doors;
    [field:SerializeField] public List<Direction> _doorSpawnning { get; } = new List<Direction>();
    [SerializeField] private int _doorAmount = 0;
    
    [SerializeField] private int _powerDistance = 0;
    public bool IsValidate { get; set; } = false;
    public bool IsFull { get; set; } = false;
    
    [SerializeField] Vector2Int _pos = new();
    
    [SerializeField] private GenerationRule _generationRule;
    [SerializeField] protected RoomBuilding _roomBuilding;
    private Random _seed;
    public Vector2Int PositionRoom { get => _pos; set => _pos = value; }

     //public abstract void UpdateDoor(Random _seed, Vector2Int _pos, Direction _nextDoor,bool _isEnd);
     
     //Reractoriser cette partie
     public void UpdateView()
     {
         AddFloor();
         _roomBuilding.BuildFloor(_doorSpawnning);
         
         //normalement nous pouvons le supp
         foreach (Direction dir in _doorSpawnning)
         {
             Doors[(int)dir].gameObject.SetActive(true);   
         }

         if (_doorSpawnning.Count == 4) IsFull = true;
     }

     public void AddNewPart(Direction nextDoor)
     {     
         if (!_doorSpawnning.Contains(nextDoor))
             _doorSpawnning.Add(nextDoor);
     }
     public void UpdateRoom(Random seed,Vector2Int pos,Direction nextDoor, bool isSpawn, bool isEnd)
     {
         _seed = seed;
         _pos = pos;
         _powerDistance = PowerDistance();
        
        if(!isSpawn || isEnd)
        { 
            Doors[(int)nextDoor].SetIsActivate(true);
            _doorSpawnning.Add(nextDoor);
            _doorAmount++;
            
            if(isEnd) return;
        }
        
        SetDoor();
     }

     public  abstract void AddFloor();
     void SetDoor()
    {
        int indexDoor = _seed.Next(0, 4);
        
        if (_doorSpawnning.Contains((Direction)indexDoor))
        {
            SetDoor();
            return;
        }

        if (!Pourcentage(101, _generationRule.SpawnRoomSpawn[_doorAmount])) return;
        
        Doors[indexDoor].SetIsActivate(true);
       // doors[indexDoor].gameObject.SetActive(true);
        _doorSpawnning.Add((Direction)indexDoor);
        _doorAmount++;

        if(_doorAmount < Doors.Length)
          SetDoor();
        
    }
    
    protected bool Pourcentage(int max,float valuePourcent)
    {   
        int _pourcentage = _seed.Next(0, max);

        if(_pourcentage < valuePourcent)
        {
            return true;
        }

        return false;
    }

    public int PowerDistance()
    {
        return Mathf.Abs(_pos.x) + Mathf.Abs(_pos.y);
    }
}

