using UnityEngine;

public static class ConvertDirection 
{
    public static Direction CalculateDirection(Vector2Int _coord)
    {
        Direction newDirection = Direction.north;
        
        if (_coord == new Vector2Int(0,-1))
            newDirection = Direction.south;
        else if (_coord == new Vector2Int(0,1)) 
            newDirection = Direction.north;
        else if (_coord == new Vector2Int(-1,0))
            newDirection = Direction.west;
        else if (_coord == new Vector2Int(1,0))
            newDirection = Direction.east;

        return newDirection;
    }    
    
    public static Direction ConvertInverseDirectionInt(Direction _direction)
    {
        Direction newDirection = Direction.north;

        switch (_direction)
        {
            case Direction.north:
                newDirection = Direction.south;
                break;
            case Direction.south :
                newDirection = Direction.north;
                break;
            case Direction.east :
                newDirection = Direction.west;
                break;
            case Direction.west :
                newDirection = Direction.east;
                break;
        }

        return newDirection;
    }
    
   public static Vector2Int CalculateCoordinate(Direction _dir)
    {
        Vector2Int dir = new ();
        
        switch (_dir)
        {
            case  Direction.north:
                dir.y += 1;
                break;
            case Direction.south :
                dir.y += -1;
                break;
            case Direction.west :
                dir.x += -1;
                break;
            case Direction.east :
                dir.x += 1;
                break;
        }

        return dir;
    }
}
