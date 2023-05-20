using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = System.Random;

namespace NewGeneration
{
    public abstract class Room : MonoBehaviour
    {
         public Door[] doors;
        [SerializeField] private List<Direction> doorSpawnning = new List<Direction>();
        [SerializeField] private int doorAmount = 0;
        
        [SerializeField] private int powerDistance = 0;
        [SerializeField] private bool isValidate = false;
        [SerializeField] Vector2Int pos = new();
        
        [SerializeField] private GenerationRule generationRule;
        [SerializeField] RoomBuilding roomBuilding;
        private Random seed;
        public bool IsValidate { get => isValidate; set => isValidate = value; }
        public Vector2Int PositionRoom { get => pos; set => pos = value; }

         //public abstract void UpdateDoor(Random _seed, Vector2Int _pos, Direction _nextDoor,bool _isEnd);
         
         //Reractoriser cette partie
         public void UpdateView()
         {
             AddFloor();
             roomBuilding.BuildFloor(doorSpawnning);
             foreach (Direction _dir in doorSpawnning)
             {
                 doors[(int)_dir].gameObject.SetActive(true);   
             }
         }

         public void AddNewPart(Direction _nextDoor)
         {     
             if (!doorSpawnning.Contains(_nextDoor))
                 doorSpawnning.Add(_nextDoor);
         }
         public void UpdateRoom(Random _seed,Vector2Int _pos,Direction _nextDoor, bool _isSpawn, bool _isEnd)
         {
             seed = _seed;
             pos = _pos;
            powerDistance = PowerDistance();
            
            if(!_isSpawn || _isEnd)
            { 
                doors[(int)_nextDoor].SetIsActivate(true);
                //doors[(int)_nextDoor].gameObject.SetActive(true);
                doorSpawnning.Add(_nextDoor);
                doorAmount++;
                
                if(_isEnd)
                {
                   // roomBuilding.BuildFloor(doorSpawnning);
                    return;
                }
            }
            
            SetDoor();
            //roomBuilding.BuildFloor(doorSpawnning);
        }

         public void AddFloor() 
         {
             for (int x = 0; x < roomBuilding.isFloor.GetLength(0); x++)
             {
                 for (int y = 0; y < roomBuilding.isFloor.GetLength(1); y++)
                 {
                     Debug.Log("x : " + x + " / " + "y : " + y );
                     if (Pourcentage( 101, 40f))
                     {
                         roomBuilding.isFloor[x, y] = true;
                     }
                 }
             }
         }
        public void SetDoor()
        {
            int indexDoor = seed.Next(0, 4);
            
            if (doorSpawnning.Contains((Direction)indexDoor))
            {
                SetDoor();
                return;
            }

            if (!Pourcentage(101, generationRule.spawnRoomSpawn[doorAmount])) return;
            
            doors[indexDoor].SetIsActivate(true);
           // doors[indexDoor].gameObject.SetActive(true);
            doorSpawnning.Add((Direction)indexDoor);
            doorAmount++;

            if(doorAmount < doors.Length)
              SetDoor();
            
        }
        
        public bool Pourcentage(int _max,float valuePourcent)
        {   
            int _pourcentage = seed.Next(0, _max);
            Debug.Log("_pourcentage : " + _pourcentage);
            if(_pourcentage < valuePourcent)
            {
                return true;
            }

            return false;
        }

        public int PowerDistance()
        {
            return Mathf.Abs(pos.x) + Mathf.Abs(pos.y);
        }
    }
}
