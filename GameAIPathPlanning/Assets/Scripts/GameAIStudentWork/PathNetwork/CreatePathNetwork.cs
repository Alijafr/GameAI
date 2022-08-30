// compile_check
// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse
{

    public class CreatePathNetwork
    {

        public const string StudentAuthorName = "Ali Alrasheed";




        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        public static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        public static int Convert(float v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2Int point, Vector2Int lineStart, Vector2Int lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }

        // Helper method provided to help you implement this file. Leave as is.
        //Get the shortest distance from a point to a line
        //Line is defined by the lineStart and lineEnd points
        public static float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            return CG.DistanceToLineSegment(point, lineStart, lineEnd);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Determines if a point is inside/on a CCW polygon and if so returns true. False otherwise.
        public static bool IsPointInPolygon(Vector2Int[] polyPts, Vector2Int point)
        {
            return CG.PointPolygonIntersectionType.Outside != CG.InPoly1(polyPts, point);
        }




        //Student code to build the path network from the given pathNodes and Obstacles
        //Obstacles - List of obstacles on the plane
        //agentRadius - the radius of the traversing agent
        //pathEdges - out parameter that will contain the edges you build.
        //  Edges cannot intersect with obstacles or boundaries. Edges must be at least agentRadius distance
        //  from all obstacle/boundary line segments

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight,
            List<Polygon> obstacles, float agentRadius, List<Vector2> pathNodes, out List<List<int>> pathEdges)
        {

            //STUDENT CODE HERE
            int agentRadiusInt = Convert(agentRadius);
            pathEdges = new List<List<int>>(pathNodes.Count);

            for (int i = 0; i < pathNodes.Count; ++i)
            {
                //create an empty pathedge
                List<int> pathEdge = new List<int>();

                bool validnode = true;
                foreach (Polygon obstacle in obstacles)
                {
                    Vector2Int[] ptsInt = obstacle.getIntegerPoints();
                    //check if this node is inside of a obstacle
                    if (IsPointInPolygon(ptsInt, Convert(pathNodes[i])))
                    {
                        validnode = false;
                        break; //ignore the node
                        
                    }
                    //make sure that the obstacle is not too close from the node
                    for (int n = 0; n < ptsInt.Length; n++)
                    {
                        if (n == ptsInt.Length - 1)
                        {
                            if (DistanceToLineSegment(Convert(pathNodes[i]), ptsInt[0], ptsInt[n])<Convert(2 * agentRadius))
                            {
                                validnode = false;
                                break;
                            }
                            //else if { }

                        }
                        else
                        {
                            if (DistanceToLineSegment(Convert(pathNodes[i]), ptsInt[0], ptsInt[n]) < Convert(2 * agentRadius))
                            {
                                validnode = false;
                                break;
                            }
                        }

                    }


                }
                //make sure the node is not outside of the grid
                Vector2Int minBoundsInt = Convert(canvasOrigin);
                Vector2Int offset = new Vector2Int(Convert(canvasWidth), Convert(canvasHeight));
                Vector2Int maxBoundsInt = Convert(canvasOrigin) + offset;
                //check if th point is inside the grid

                //get the 4 verteces of the
                Vector2Int topEdgeInt_i = Convert(pathNodes[i]) + new Vector2Int(0, agentRadiusInt);
                Vector2Int rightEdgeInt_i = Convert(pathNodes[i]) + new Vector2Int(agentRadiusInt, 0);
                Vector2Int leftEdgeInt_i = Convert(pathNodes[i]) + new Vector2Int(-agentRadiusInt, 0);
                Vector2Int bottomEdgeInt_i = Convert(pathNodes[i]) + new Vector2Int(0, -agentRadiusInt);
                if (leftEdgeInt_i.x < minBoundsInt.x || rightEdgeInt_i.x > maxBoundsInt.x || bottomEdgeInt_i.y < minBoundsInt.y || topEdgeInt_i.y > maxBoundsInt.y)
                {
                    //not valid node, continue
                    validnode = false;
                }

                if (!validnode)
                {
                    //not valid node, continue
                    pathEdges.Add(pathEdge);
                    continue;
                }
                

                for (int j = 0; j < pathNodes.Count; j++)
                {
                    bool validedge = true;
                    //nodes should not connnect to itself
                    if (i == j)
                    {
                        continue; 
                    }
                    //get the 4 verteces of the
                    Vector2Int topEdgeInt = Convert(pathNodes[j]) + new Vector2Int(0, agentRadiusInt);
                    Vector2Int rightEdgeInt = Convert(pathNodes[j]) + new Vector2Int(agentRadiusInt,0);
                    Vector2Int leftEdgeInt = Convert(pathNodes[j]) + new Vector2Int(-agentRadiusInt, 0);
                    Vector2Int bottomEdgeInt = Convert(pathNodes[j]) + new Vector2Int(0, -agentRadiusInt);
                    if (leftEdgeInt.x < minBoundsInt.x || rightEdgeInt.x > maxBoundsInt.x || bottomEdgeInt.y < minBoundsInt.y || topEdgeInt.y > maxBoundsInt.y)
                    {
                        //not valid node, continue
                        continue;
                    }

                    foreach (Polygon obstacle in obstacles)
                    {
                        Vector2Int[] ptsInt = obstacle.getIntegerPoints();
                        //check if this pairing node is inside an obstacle 
                        if (IsPointInPolygon(ptsInt, Convert(pathNodes[j])))
                        {
                            validedge = false;
                            continue;
                        }
                        else
                        {
                            //check intercetion between these nodes and the the obstacle 
                            for (int n = 0; n < ptsInt.Length; n++)
                            {
                                if (n == ptsInt.Length - 1)
                                {
                                    if (Intersects(Convert(pathNodes[i]), Convert(pathNodes[j]), ptsInt[0], ptsInt[n]))
                                    {
                                        validedge = false;
                                        break;
                                    }
                                    else if (DistanceToLineSegment(Convert(pathNodes[j]), ptsInt[0], ptsInt[n]) < Convert(agentRadius))
                                    {
                                        validedge = false;
                                        break;
                                    }
                                    else if (DistanceToLineSegment(ptsInt[n],Convert(pathNodes[j]),Convert(pathNodes[i])) < Convert(agentRadius))
                                    {
                                        validedge = false;
                                        break;
                                    }

                                }
                                else
                                {
                                    if (Intersects(Convert(pathNodes[i]), Convert(pathNodes[j]), ptsInt[n], ptsInt[n + 1]))
                                    {
                                        validedge = false;
                                        break;
                                    }
                                    else if (DistanceToLineSegment(Convert(pathNodes[j]), ptsInt[0], ptsInt[n]) < Convert(2 * agentRadius))
                                    {
                                        validedge = false;
                                        break;
                                    }
                                    else if (DistanceToLineSegment(ptsInt[n], Convert(pathNodes[j]), Convert(pathNodes[i])) < Convert(agentRadius))
                                    {
                                        validedge = false;
                                        break;
                                    }
                                }

                            }
                        }
                    }

                    if (validedge)
                    {
                        pathEdge.Add(j);
                    }

                }

                pathEdges.Add(pathEdge);

                
            }


            // END STUDENT CODE

        }


    }

}