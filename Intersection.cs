using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class defines the location of an intersection on the map, as well as pointing to it's neighbors.
/// </summary>
public class Intersection {
    // Intersections to the north, south, east, and west of the current intersection
    public Intersection north;
    public Intersection south;
    public Intersection east;
    public Intersection west;

    // Current index position of the intersection in the 9 x 9 grid
    public int xPos;
    public int yPos;

    // Game coordinate of the intersection
    public Vector3 mapPosition; 

    // Constructor 
    public Intersection(int i, int j) {
        north = null;
        south = null;
        east = null;
        west = null;
        xPos = i;
        yPos = j;
        mapPosition = new Vector3(); 
    }

    public Intersection getNorth()
    {
        return this.north;
    }

    public Intersection getSouth()
    {
        return this.south;
    }

    public Intersection getEast()
    {
        return this.east;
    }

    public Intersection getWest()
    {
        return this.west;
    }

    public int getXPosGrid()
    {
        return xPos;
    }

    public int getYPosGrid()
    {
        return yPos;
    }

    public Vector3 getMapPosition()
    {
        return mapPosition;
    }

    public void setNorth(Intersection node)
    {
        this.north = node;
    }

    public void setSouth(Intersection node)
    {
        this.south = node;
    }

    public void setEast(Intersection node)
    {
        this.east = node;
    }

    public void setWest(Intersection node)
    {
        this.west = node;
    }

    public void setXPosGrid(int x)
    {
        this.xPos = x;
    }

    public void setYPosGrid(int y)
    {
        this.yPos = y;
    }

    public void setMapPosition(Vector3 mapPosition)
    {
        this.mapPosition = mapPosition;
    }

    //We take the center of the intersection
    //From there we generate all four of the corners of that intersection
    //Function is used to spawn pedestrians so that they are not in the street. 
    public Vector3[] getIntersectionCorners() {

        Vector3[] fourCorners = new Vector3[4];

        fourCorners[0] = new Vector3(mapPosition.x - 8, mapPosition.y, mapPosition.z + 8);
        fourCorners[1] = new Vector3(mapPosition.x + 8, mapPosition.y, mapPosition.z + 8);
        fourCorners[2] = new Vector3(mapPosition.x - 8, mapPosition.y, mapPosition.z - 8);
        fourCorners[3] = new Vector3(mapPosition.x + 8, mapPosition.y, mapPosition.z - 8);

        return fourCorners;
    }
}
