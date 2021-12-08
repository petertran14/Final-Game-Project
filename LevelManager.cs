using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    // Creates a grid of intersections
    public Intersection[,] cityGrid;
    public int row = 9;
    public int col = 9;

    // On the four corners of the grid, the lowest real coordinate the x and z decend to is -400
    // Conversely, the max of the grid bound is positive 400
    public const float MIN_GRID_BOUND = -400;

    // The actual y coordinate of the map
    public float y_level = 33.8f; 


    // Instantiates a lot of stuff
    void Awake() {

        // Creates a 9 x 9 array consisting of a total of 81 intersections
        cityGrid = new Intersection[row, col];

        // Instantiate each Intersection
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {

                //Intersection testNode;
                //testNode = gameObject.AddComponent<Intersection>();
                //Debug.Log(testNode == null);

                cityGrid[i, j] = gameObject.AddComponent<Intersection>();
                cityGrid[i, j].setXPosGrid(i);
                cityGrid[i, j].setYPosGrid(j);
            }
        }

        // Probably the culprit here...

        // Loop through each Intersection
        for (int i = 0; i < row; i++) {
            for (int j = 0; j < col; j++) {

                // Current Intersection
                Intersection curInter = cityGrid[i, j];

                //Checks if there is a viable intersection in the NORTH direction
                if (j + 1 != col) {
                    curInter.north = cityGrid[i, j + 1]; 
                }

                //Check DOWN
                if (j != 0) {
                    curInter.south = cityGrid[i, j - 1];
                }

                //Checks UP
                if (i + 1 != row) {
                    curInter.east = cityGrid[i + 1, j]; 
                }

                //Checks DOWN
                if (i != 0) {
                    curInter.west = cityGrid[i - 1, j]; 
                }

            }
        }

        this.assignRealPositions();

       
    }

    /// <summary>
    /// Assigns each intersection "node" with the real x, y, and z coordinates from the game world.
    /// </summary>
    public void assignRealPositions() {

        for (int i = 0; i < row; i++) {
            // -400 + (0 * 100) = -400
            // -400 + (1 * 100) = -300
            // -400 + (2 * 100) = -200 ...
            // -400 + (8 * 100) = 400
            float curXAssigning = MIN_GRID_BOUND + (i * 100f); 
            for (int j = 0; j < col; j++) {
                float curZAssigning = MIN_GRID_BOUND + (j * 100f);

                cityGrid[i, j].mapPosition = new Vector3(curXAssigning, y_level, curZAssigning);

                // cityrGrid[0, 0] = { x: -400, z: -400 }
                // cityrGrid[0, 1] = { x: -400, z: -300 }  // Moving north increases z, and increases y index
            }
        }
    }

    public void printNodePositions()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Debug.Log("CityGrid[" + cityGrid[i, j].getXPosGrid() + "," + cityGrid[i, j].getYPosGrid() + "] = " + cityGrid[i, j].mapPosition.x + " " + cityGrid[i, j].mapPosition.y + " " + cityGrid[i, j].mapPosition.z);
            }
        }
    }
}
