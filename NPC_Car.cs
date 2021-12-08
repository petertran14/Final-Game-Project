using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a Character Controller if it does not exist for the NPC...so I don't have to make one manually in Unity
[RequireComponent(typeof(CharacterController))]

// Class is for regular cars only, COP cars behave differently

/// <summary>
/// Prerequisites:
///     1. Car is initially placed on the exact coordinate of a particular node
/// </summary>
public class NPC_Car : MonoBehaviour
{
    private LevelManager traversableNodes;
    public float drivingSpeed;

    private Intersection currNode;
    private Intersection nextNode;

    // Initially the direction will be set manually, subsequently the direction will be set automatically
    private string direction;
    private bool isFirstTime;

    // Everytime the car is at a node, isShifted is set to false so we can adjust it's position a singular time. Then while the car is driving isShfited is true so we won't adjust with each update call
    private bool isShifted;

    // Car must satisfy the constraints above
    private bool isValid;

    // Controller object for the NPC
    CharacterController controller;

    private void Start()
    {
       
        // Creating the grid and all the intersection information as detailed in LevelManager
        traversableNodes = gameObject.AddComponent<LevelManager>();
        isFirstTime = true;

        // In the beginning, the car is not yet shifted
        this.isShifted = false;

        // traversableNodes.printNodePositions();

        // Find the current node
        currNode = findCurrentNode();
        Debug.Log("Currrent node: " + currNode.getMapPosition().x + " " + currNode.getMapPosition().y + " " + currNode.getMapPosition().z);
        isValid = isValidCarPlacement();

        // Character controller helps us to detect collision using Unity's 
        controller = GetComponent<CharacterController>();

        nextNode = chooseNextNode();
        Debug.Log("Next node: " + nextNode.getMapPosition().x + " " + nextNode.getMapPosition().y + " " + nextNode.getMapPosition().z);

        // This should never happen.
        if (nextNode == null)
            Debug.Log("There are adjacent nodes. Yikes.");
    }

    private void Update()
    {
        if (isValid)
        {
            moveTowardsNextIntersection();
        }

    }

    /**
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     *    * * * * * * * * *
     */

    private void moveTowardsNextIntersection()
    {
        // Debug.Log("Car's Position: " + transform.position.x + " " + transform.position.y + " " + transform.position.z);
        // In inspector change the radius and height to 0.

        if (direction.Equals("north")) {
            AdjustCarPosition("north");
        }
        
        else if (direction.Equals("south")) {
            AdjustCarPosition("south");
        }
        
        else if (direction.Equals("east")) {
            AdjustCarPosition("east");
        }
        
        else if (direction.Equals("west")) {
            AdjustCarPosition("west");
        }

        // Start function chose next node to move to, so that means at this point the car has a direction, and will always have a direction
        else
            Debug.Log("The car has no direction. Something bad has happened.");

        controller.SimpleMove(transform.TransformDirection(Vector3.forward) * drivingSpeed);
        // Need to know when I have approaced an intersection, make curr_node = next_node
        // So I just need to check if the next frame passes the node's coordinate, then adjust the car position so that it aligns with the turn angle.
        if (ArrivedAtIntersection())
        {
            Debug.Log("Current Node = Next Node");            
            currNode = nextNode;
            this.isShifted = false;
        }

        // If the currNode == nextNode then I need to advance the next node
        if (currNode.Equals(nextNode))
        {
            Debug.Log("Choosing next node.");
            nextNode = chooseNextNode();

            // This should never happen.
            if (nextNode == null)
                Debug.Log("There are adjacent nodes. Yikes.");
        }
    }

    /// <summary>
    /// Adjusts the car's position so that it stays on it's own lane, with each subsequent turn. Also adjust car orientation to match direction.
    /// </summary>
    public void AdjustCarPosition(string dir)
    {
        if (dir.Equals("north"))
        {
            // North is 0 degrees rotation about y axis
            transform.rotation = Quaternion.Euler(0, 0, 0);
            // A car moving north is +2.5 in the x direction from the current node
            if (!isShifted)
            {
                // The character controller has it's own instance of position, so we need to temporarily deactivate it's default position so we can start with a modified position
                controller.enabled = false;
                controller.transform.position = new Vector3(this.currNode.getMapPosition().x + 2.5f, this.currNode.getMapPosition().y, this.currNode.getMapPosition().z); ;
                controller.enabled = true;
            }

            isShifted = true;
        }

        else if (dir.Equals("south"))
        {
            // South is 180 degrees rotation about y axis
            transform.rotation = Quaternion.Euler(0, 180, 0);
            // A car moving south is -2.5 in the x direction from the current node
            if (!isShifted)
            {

                controller.enabled = false;
                controller.transform.position = new Vector3(this.currNode.getMapPosition().x - 2.5f, this.currNode.getMapPosition().y, this.currNode.getMapPosition().z); ;
                controller.enabled = true;
            }

            isShifted = true;
        }

        else if (dir.Equals("east"))
        {
            // East is 90 degrees rotation about y axis
            transform.rotation = Quaternion.Euler(0, 90, 0);
            // A car moving east is -2.5 in the z direction from the current node
            if (!isShifted)
            {
                controller.enabled = false;
                controller.transform.position = new Vector3(this.currNode.getMapPosition().x, this.currNode.getMapPosition().y, this.currNode.getMapPosition().z - 2.5f); ;
                controller.enabled = true;
            }

            isShifted = true;
        }

        else if (dir.Equals("west"))
        {
            // West is 270 degrees rotation about y axis
            transform.rotation = Quaternion.Euler(0, 270, 0);
            // A car moving west is +2.5 in the z direction from the current node
            if (!isShifted)
            {
                controller.enabled = false;
                controller.transform.position = new Vector3(this.currNode.getMapPosition().x, this.currNode.getMapPosition().y, this.currNode.getMapPosition().z + 2.5f); ;
                controller.enabled = true;
            }

            isShifted = true;
        }

        else
        {
            Debug.Log("The car has no direction. Something bad has happened.");
        }
    }

    /// <summary>
    /// Returns true or false whether or not the car has arrived at the approximate location of the intersection.
    ///
    /// The problem with comparing using == is that the update frames do not get data continually, but every frame. So it misses the exact moment where the car's position is exactly equal to the node position.
    /// Using the Distance() function for Vector3's will let me know when the car has approximately arrived at the interesection, since it is impossible to be 100% accurate.
    /// </summary>
    /// <returns></returns>
    public bool ArrivedAtIntersection() 
    {
        if (Vector3.Distance(nextNode.getMapPosition(), transform.position) < 4.5f) {
            return true;
        }
    
        return false;
    }

    /// <summary>
    /// Returns a randomly choosen valid node to traverse to.
    /// </summary>
    /// <returns></returns>
    private Intersection chooseNextNode()
    {
        // The problem is that the car is moving "north" when z is increasing. South an z is decreasing. East when x is increasing, west when x is decreasing.



        // Need to see the availability of the current node with regards to direction it can expand to
        List<string> available_directions = new List<string>();

        // The first time this is called, we haven't been given a direction so the car is free to choose any 4 direction to begin
        if (isFirstTime)
        {
            if (currNode.getNorth() != null)
                available_directions.Add("north");
            if (currNode.getSouth() != null)
                available_directions.Add("south");
            if (currNode.getEast() != null)
                available_directions.Add("east");
            if (currNode.getWest() != null)
                available_directions.Add("west");
            this.isFirstTime = false;
        }

        else
        {
            // If there is an intersection north, and the car is not moving south, then add it. The reason is that if it is moving south we don't want it to make a U-turn
            if (currNode.getNorth() != null && !direction.Equals("south"))
                available_directions.Add("north");
            if (currNode.getSouth() != null && !direction.Equals("north"))
                available_directions.Add("south");
            if (currNode.getEast() != null && !direction.Equals("west"))
                available_directions.Add("east");
            if (currNode.getWest() != null && !direction.Equals("east"))
                available_directions.Add("west");
        }

        // Randomly pick a direction from the available direction
        var random = new System.Random();
        int index = random.Next(available_directions.Count);
        // Assign direction
        this.direction = available_directions[index];

        if (direction.Equals("north"))
            // In the north direction, the Y index of the current node increases by 1
            return traversableNodes.cityGrid[currNode.getXPosGrid(), currNode.getYPosGrid() + 1];
        if (direction.Equals("south"))
            // In the south direction, the Y index of the current node decreases by 1
            return traversableNodes.cityGrid[currNode.getXPosGrid(), currNode.getYPosGrid() - 1];
        if (direction.Equals("east"))
            // In the east direction, the X index of the current node increases by 1
            return traversableNodes.cityGrid[currNode.getXPosGrid() + 1, currNode.getYPosGrid()];
        if (direction.Equals("west"))
            // In the west direction, the X index of the current node decreases by 1
            return traversableNodes.cityGrid[currNode.getXPosGrid() - 1, currNode.getYPosGrid()];
        // If this function returns null (should never happen because there are no isolated nodes), then that means something is bad
        return null;

    }

    /// <summary>
    /// Returns the node that the car is placed on, using the premade grid initialization from the LevelManager.cs script.
    /// </summary>
    /// <returns></returns>
    private Intersection findCurrentNode()
    {
        // Loop through all the intersection points
        for (int i = 0; i < traversableNodes.row; i++)
        {
            for (int j = 0; j < traversableNodes.col; j++)
            {
                // If the intersection point aligns with the car position that means that car is directly on top of the node
                if (traversableNodes.cityGrid[i, j].getMapPosition().Equals(transform.position))
                {
                    return traversableNodes.cityGrid[i, j];
                }
            }
        }

        // The car is not placed on top of a node and thus is not valid
        return null;
    }

    private bool isValidCarPlacement()
    {
        if (currNode == null)
        {
            Debug.Log("Invalid Car Placement.");
            return false;
        }
        Debug.Log("Valid Car Placement");
        return true;
    }
}

