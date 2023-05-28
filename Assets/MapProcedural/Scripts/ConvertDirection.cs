using UnityEngine;

public static class ConvertDirection 
{
    public static Direction CalculateDirection(Vector2Int coord)
    {
        Direction newDirection = Direction.north;
        
        if (coord == new Vector2Int(0,-1))
            newDirection = Direction.south;
        else if (coord == new Vector2Int(0,1)) 
            newDirection = Direction.north;
        else if (coord == new Vector2Int(-1,0))
            newDirection = Direction.west;
        else if (coord == new Vector2Int(1,0))
            newDirection = Direction.east;

        return newDirection;
    }    
    
    public static Direction ConvertInverseDirectionInt(Direction direction)
    {
        Direction newDirection = Direction.north;

        switch (direction)
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
    
   public static Vector2Int CalculateCoordinate(Direction dir)
    {
        Vector2Int newDirection = new ();
        
        switch (dir)
        {
            case  Direction.north:
                newDirection.y += 1;
                break;
            case Direction.south :
                newDirection.y += -1;
                break;
            case Direction.west :
                newDirection.x += -1;
                break;
            case Direction.east :
                newDirection.x += 1;
                break;
        }

        return newDirection;
    }
}
