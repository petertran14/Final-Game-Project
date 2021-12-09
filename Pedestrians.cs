using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : MonoBehaviour {

    public Intersection currentIntersection;
    public Intersection targetIntersection;

    public Vector3 targetPosition;

    public Pedestrian(Intersection starting) {
        currentIntersection = starting;
        updateTarget(starting); 
    }
    

    // Update is called once per frame
    void Update() {

        Vector3 currentPosition = transform.position;
        float step = 0.04f;
        if (currentPosition != targetPosition) {
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, step);
        } else {
            currentIntersection = targetIntersection;
            updateTarget(currentIntersection); 
        }


        
    }

    //Given a current intersection we will randomly generate the next intersection
    //That we will visit that node 
    public void updateTarget(Intersection cur) {
       

        List<Intersection> possibleNext = new List<Intersection>();
        List<Intersection> actualNext = new List<Intersection>(); 

        possibleNext.Add(cur.getNorth());
        possibleNext.Add(cur.getSouth());
        possibleNext.Add(cur.getEast());
        possibleNext.Add(cur.getWest());

        for (int i = 0; i < possibleNext.Count; i++) {
            if (possibleNext[i] != null) {
                actualNext.Add(possibleNext[i]); 
            }
        }



        
        Intersection startingInter = actualNext[Random.Range(0, actualNext.Count)];
        targetIntersection = startingInter; 

        Vector3[] possibleTargetPositions = startingInter.getIntersectionCorners(); 
        Vector3 currentVector = possibleTargetPositions[0]; 
        float smallestTotalDistance = 0;
        int smallestDistance = 0;   
        smallestTotalDistance += Mathf.Abs(transform.position.x - currentVector.x);
        smallestTotalDistance += Mathf.Abs(transform.position.z - currentVector.z);

        for (int i = 1; i < possibleTargetPositions.Length; i++) {
            float newTotalDistance = 0;
            currentVector = possibleTargetPositions[i]; 
            newTotalDistance += Mathf.Abs(transform.position.x - currentVector.x);
            newTotalDistance += Mathf.Abs(transform.position.z - currentVector.z);

            if (newTotalDistance < smallestTotalDistance) {
                smallestDistance = i;
                smallestTotalDistance = newTotalDistance; 
            }

        }



        targetPosition = possibleTargetPositions[smallestDistance]; 
    }

    
}
