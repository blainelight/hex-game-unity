using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSearch
{
    public static BFSResult BFSGetRange(HexGrid hexGrid, Vector3Int startPoint, int movementPoints)
    {
        Dictionary<Vector3Int, Vector3Int?> visitedNodes = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, int> costSoFar = new Dictionary<Vector3Int, int>();
        Queue<Vector3Int> nodesToVisitQueue = new Queue<Vector3Int>();
    
        nodesToVisitQueue.Enqueue(startPoint);
        costSoFar.Add(startPoint, 0);
        visitedNodes.Add(startPoint, null);

        while (nodesToVisitQueue.Count > 0) //while there are still nodes to visit
        {
            Vector3Int currentNode = nodesToVisitQueue.Dequeue();
            foreach (Vector3Int neighbourPosition in hexGrid.GetNeighboursFor(currentNode))
            {
                if(hexGrid.GetTileAt(neighbourPosition).IsObstacle())
                    continue;
                
                int nodeCost = hexGrid.GetTileAt(neighbourPosition).GetCost();
                int currentCost = costSoFar[currentNode];
                int newCost = currentCost + nodeCost;

                if (newCost <= movementPoints)
                {
                    if (!visitedNodes.ContainsKey(neighbourPosition)) //when we stuble upon a new node i.e. we haven't visited this node yet
                    {
                        visitedNodes[neighbourPosition] = currentNode; //save the parent of the neighbor that we consider
                        costSoFar[neighbourPosition] = newCost; //disposition from our startposition 
                        nodesToVisitQueue.Enqueue(neighbourPosition);
                    }
                    else if (costSoFar[neighbourPosition] > newCost)
                    {
                        costSoFar[neighbourPosition] = newCost;
                        visitedNodes[neighbourPosition] = currentNode;
                    }
                }
            }
        }
        return new BFSResult {visitedNodesDict = visitedNodes};
    }
    public static List<Vector3Int> GeneratePathBFS(Vector3Int current, Dictionary<Vector3Int, Vector3Int?> visitedNodesDict)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        path.Add(current);
        while (visitedNodesDict[current] != null) //i.e. it's not the start position
        {
            path.Add(visitedNodesDict[current].Value);
            current = visitedNodesDict[current].Value; // set current hex to be parent of hex that we checked previously. 
        }
        path.Reverse();
        return path.Skip(1).ToList();
    }
}



public struct BFSResult
{
    public Dictionary<Vector3Int,Vector3Int?> visitedNodesDict;
    public List<Vector3Int> GetPathTo(Vector3Int destination)
    {
        if(visitedNodesDict.ContainsKey(destination) == false)
            return new List <Vector3Int>(); //return empty path b/c there is no path
        return GraphSearch.GeneratePathBFS(destination, visitedNodesDict); //NEED TO MAKE GeneratePathBFS in the class, he did it with quick actions
    }

    public bool IsHexPositionInRange(Vector3Int position)
    {
        return visitedNodesDict.ContainsKey(position);
    }

    public IEnumerable<Vector3Int> GetRangePositions()
        => visitedNodesDict.Keys;
}
