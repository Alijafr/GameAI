﻿using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
public class BreadthFirstPathSearch : PathSearchProvider
{
    private static PathSearchProvider instance;
    public static PathSearchProvider Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BreadthFirstPathSearch();
            }
            return instance;
        }
    }
    override public PathSearchResultType FindPathIncremental(
        //List<Vector2> nodes,
        //List<List<int>> edges,
        GetNodeCount getNodeCount,
        GetNode getNode,
        GetNodeAdjacencies adjacencies,
        bool useManhattan, 
        int startNodeIndex, int goalNodeIndex, int maxNumNodesToExplore, bool doInitialization, ref int currentNodeIndex, ref Dictionary<int, PathSearchNodeRecord> searchNodeRecords, ref SimplePriorityQueue<int, float> openNodes, ref HashSet<int> closedNodes, ref List<int> returnPath)
    {

        return BasicPathSearchImpl.FindPathIncremental(
            //nodes,
            //edges,
            getNodeCount,
            getNode,
            adjacencies,
            startNodeIndex, goalNodeIndex, true, false, maxNumNodesToExplore, doInitialization,
            ref currentNodeIndex, ref searchNodeRecords, ref openNodes, ref closedNodes, ref returnPath);

    }
}
