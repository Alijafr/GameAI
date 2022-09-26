
// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;


namespace GameAICourse
{


    public class AStarPathSearchImpl
    {

        // Please change this string to your name
        public const string StudentAuthorName = "Ali Alrasheed";
      

        // Null Heuristic for Dijkstra
        public static float HeuristicNull(Vector2 nodeA, Vector2 nodeB)
        {
            return 0f;
        }

        // Null Cost for Greedy Best First
        public static float CostNull(Vector2 nodeA, Vector2 nodeB)
        {
            return 0f;
        }



        // Heuristic distance fuction implemented with manhattan distance
        public static float HeuristicManhattan(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return Mathf.Abs(nodeA.x-nodeB.x)+Mathf.Abs(nodeA.y-nodeB.y);

            //END CODE 
        }

        // Heuristic distance function implemented with Euclidean distance
        public static float HeuristicEuclidean(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return (float)(Mathf.Sqrt(Mathf.Pow(nodeA.x-nodeB.x,2)+Mathf.Pow(nodeA.y-nodeB.y,2)));

            //END CODE 
        }


        // Cost is only ever called on adjacent nodes. So we will always use Euclidean distance.
        // We could use Manhattan dist for 4-way connected grids and avoid sqrroot and mults.
        // But we will avoid that for simplicity.
        public static float Cost(Vector2 nodeA, Vector2 nodeB)
        {
            //STUDENT CODE HERE

            // The following code is just a placeholder so that the method has a valid return
            // You will replace it with the correct implementation
            return HeuristicEuclidean(nodeA,nodeB);

            //END STUDENT CODE
        }



        public static PathSearchResultType FindPathIncremental(
            GetNodeCount getNodeCount,
            GetNode getNode,
            GetNodeAdjacencies getAdjacencies,
            CostCallback G,
            CostCallback H,
            int startNodeIndex, int goalNodeIndex,
            int maxNumNodesToExplore, bool doInitialization,
            ref int currentNodeIndex,
            ref Dictionary<int, PathSearchNodeRecord> searchNodeRecords,
            ref SimplePriorityQueue<int, float> openNodes, ref HashSet<int> closedNodes, ref List<int> returnPath)
        {
            PathSearchResultType pathResult = PathSearchResultType.InProgress;

            var nodeCount = getNodeCount();

            if (startNodeIndex >= nodeCount || goalNodeIndex >= nodeCount ||
                startNodeIndex < 0 || goalNodeIndex < 0 ||
                maxNumNodesToExplore <= 0 ||
                (!doInitialization &&
                 (openNodes == null || closedNodes == null || currentNodeIndex < 0 ||
                  currentNodeIndex >= nodeCount)))
            {
                searchNodeRecords = new Dictionary<int, PathSearchNodeRecord>();
                openNodes = new SimplePriorityQueue<int, float>();
                closedNodes = new HashSet<int>();
                return PathSearchResultType.InitializationError;
            }

     

            //float max_dfs_priority = Mathf.Pow(2f, 20f);

            if (doInitialization)
            {
                currentNodeIndex = startNodeIndex;
                searchNodeRecords = new Dictionary<int, PathSearchNodeRecord>();
                openNodes = new SimplePriorityQueue<int, float>();
                closedNodes = new HashSet<int>();
                var firstNodeRecord = new PathSearchNodeRecord(currentNodeIndex);
                searchNodeRecords.Add(firstNodeRecord.NodeIndex, firstNodeRecord);
                //float startingPriority = IsBFS ? 0f : max_dfs_priority;
                float startingPriority = 0f;
                openNodes.Enqueue(firstNodeRecord.NodeIndex, startingPriority);
                returnPath = new List<int>();
            }


            int nodesProcessed = 0;
            while (nodesProcessed < maxNumNodesToExplore && openNodes.Count > 0)
            {
                //Find the smallest element in the open list using the estimated total cost
                var currentNodeRecord = searchNodeRecords[openNodes.First];
                currentNodeIndex = currentNodeRecord.NodeIndex;


                ++nodesProcessed;

                // goal check should be in edge expansion for better time performance!
                // However, doing so means goal found sooner. But if we are using DFS
                // we are probably intentionally looking for long paths...
                if (currentNodeIndex == goalNodeIndex)
                    break;


                


                PathSearchNodeRecord edgeNodeRecord = null;

                //var currEdges = edges[currentNodeIndex];
                var currEdges = getAdjacencies(currentNodeIndex);


                foreach (var edgeNodeIndex in currEdges)
                {
                    float heuristic = 0.0f;
                    //get the cost estimate for this node
                    var costToEdgeNode = currentNodeRecord.CostSoFar +
                        G(getNode(currentNodeIndex), getNode(edgeNodeIndex));

                    //check if it is in the closed set, see if the current cost is better than when it was closed
                    if (closedNodes.Contains(edgeNodeIndex))
                    {
                        edgeNodeRecord = searchNodeRecords[edgeNodeIndex];
                        //check if the registered cost is worse than before
                        if (edgeNodeRecord.CostSoFar <= costToEdgeNode)
                        {
                            //no benefit in processing this edge
                            continue;
                        }
                        //otherwise, remove it from the closed list
                        closedNodes.Remove(edgeNodeIndex);
                        //estimate the heuristic for this node (if heuristic is expensive to recalculate)
                        //heuristic = H(getNode(edgeNodeIndex), getNode(edgeNodeIndex));
                        heuristic = edgeNodeRecord.EstimatedTotalCost - edgeNodeRecord.CostSoFar;
                    }
                    else if (openNodes.Contains(edgeNodeIndex))
                    {
                        //find the record of the current node
                        edgeNodeRecord = searchNodeRecords[edgeNodeIndex];
                        //if this path is not better skip 
                        if (edgeNodeRecord.CostSoFar <= costToEdgeNode)
                        { 
                            continue;
                        }
                        //estimate the heuristic for this node (if heuristic is expensive to recalculate)
                        //heuristic = H(getNode(edgeNodeIndex), getNode(edgeNodeIndex));
                        heuristic = edgeNodeRecord.EstimatedTotalCost - edgeNodeRecord.CostSoFar;

                    }
                    else
                    {
                        //never explored(unvisited node)
                        //create a new record 
                        edgeNodeRecord = new PathSearchNodeRecord(edgeNodeIndex);
                        //estimate the heurisics 
                        heuristic = H(getNode(edgeNodeIndex), getNode(edgeNodeIndex));
                    }

                    // update the from node index.
                    edgeNodeRecord.FromNodeIndex = currentNodeIndex;
                    // update the CostSoFar.
                    edgeNodeRecord.CostSoFar = costToEdgeNode;
                    // update the estimated total cost.
                    edgeNodeRecord.EstimatedTotalCost = costToEdgeNode + heuristic;
                    // add the node to searchNodeRecords.
                    searchNodeRecords[edgeNodeIndex] = edgeNodeRecord;


                    if (!openNodes.Contains(edgeNodeIndex))
                    {
                        //add the node to the open queue with its  priority (esitmated total cost G+H)
                        openNodes.Enqueue(edgeNodeIndex, edgeNodeRecord.EstimatedTotalCost);
                    }
                    else
                    {
                        //update the the priority of this node to it esitmated total cost (G+H)
                        openNodes.UpdatePriority(edgeNodeIndex, edgeNodeRecord.EstimatedTotalCost);
                        

                    }

                } //foreach() edge processing of current node

                //make this current node closed --> will be revisited in case a better cost was found for this node
                openNodes.Remove(currentNodeIndex);
                closedNodes.Add(currentNodeIndex);
            } //while
            if (openNodes.Count <= 0 && currentNodeIndex != goalNodeIndex)
            {
                pathResult = PathSearchResultType.Partial;
                //find the closest node we looked at and use for partial path
                int closest = -1;
                float closestDist = float.MaxValue;
                foreach (var n in closedNodes)
                {
                    var nrec = searchNodeRecords[n];

                    // Hmmm. I bet if we were using this partial path code with an A* implementation 
                    // we could avoid calculating Euclidean distance again using cached metadata.
                    // (But we would need to deduce whether said code was running in Dijkstra
                    // mode with a zero constant Heuristic() func)
                    // Otherwise, we must calculate the distance
                    var d = Vector2.Distance(getNode(nrec.NodeIndex), getNode(goalNodeIndex));
                    if (d < closestDist)
                    {
                        closest = n;
                        closestDist = d;
                    }
                }
                if (closest >= 0)
                {
                    currentNodeIndex = closest;
                }
            }
            else if (currentNodeIndex == goalNodeIndex)
            {
                pathResult = PathSearchResultType.Complete;
            }


            if (pathResult != PathSearchResultType.InProgress)
            {
                // processing complete, a path (possibly partial) can be generated returned
                returnPath = new List<int>();
                while (currentNodeIndex != startNodeIndex)
                {
                    returnPath.Add(currentNodeIndex);
                    currentNodeIndex = searchNodeRecords[currentNodeIndex].FromNodeIndex;
                }
                returnPath.Add(startNodeIndex);
                returnPath.Reverse();
            }

            return pathResult;

    }

    }

}