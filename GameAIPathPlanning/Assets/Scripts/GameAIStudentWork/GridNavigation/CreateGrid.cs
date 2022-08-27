// compile_check
// Remove the line above if you are subitting to GradeScope for a grade. But leave it if you only want to check
// that your code compiles and the autograder can access your public methods.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAICourse {

    public class CreateGrid
    {

        // Please change this string to your name
        public const string StudentAuthorName = "Ali Alrasheed";


        // Helper method provided to help you implement this file. Leave as is.
        // Returns true if point p is inside (or on edge) the polygon defined by pts (CCW winding). False, otherwise
        public static bool IsPointInsidePolygon(Vector2Int[] pts, Vector2Int p)
        {
            return CG.InPoly1(pts, p) != CG.PointPolygonIntersectionType.Outside;
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns float converted to int according to default scaling factor (1000)
        public static int Convert(float v)
        {
            return CG.Convert(v);
        }


        // Helper method provided to help you implement this file. Leave as is.
        // Returns Vector2 converted to Vector2Int according to default scaling factor (1000)
        public static Vector2Int Convert(Vector2 v)
        {
            return CG.Convert(v);
        }

        // Helper method provided to help you implement this file. Leave as is.
        // Returns true is segment AB intersects CD properly or improperly
        static public bool Intersects(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
        {
            return CG.Intersect(a, b, c, d);
        }


        // PointInsideBoundingBox(): Determines whether a point (Vector2Int:p) is On/Inside a bounding box (such as a grid cell) defined by
        // minCellBounds and maxCellBounds (both Vector2Int's).
        // Returns true if the point is ON/INSIDE the cell and false otherwise
        // This method should return true if the point p is on one of the edges of the cell.
        // This is more efficient than PointInsidePolygon() for an equivalent dimension poly
        public static bool PointInsideBoundingBox(Vector2Int minCellBounds, Vector2Int maxCellBounds, Vector2Int p)
        {
            //TODO IMPLEMENT
            //add the -1 
            if (minCellBounds.x <= p.x & maxCellBounds.x >= p.x & minCellBounds.y <= p.y  & maxCellBounds.y >= p.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        // Istraversable(): returns true if the grid is traversable from grid[x,y] in the direction dir, false otherwise.
        // The grid boundaries are not traversable. If the grid position x,y is itself not traversable but the grid cell in direction
        // dir is traversable, the function will return false.
        // returns false if the grid is null, or any dimension of grid is zero length
        // returns false if x,y is out of range
        public static bool IsTraversable(bool[,] grid, int x, int y, TraverseDirection dir)
        {
            // TODO IMPLEMENT
            //check if the grid is null or the x and y is out of bound or the size of the grid is 0 
            if (grid == null | grid.GetLength(0) == 0 | grid.GetLength(1) == 0 | grid.GetLength(0) <= x | grid.GetLength(1) <= y)
            {
                return false;
            }
            else if (grid[x, y] == false)
            {
                return false;
            }
            else
            {
                //check the 8 directions
                if (dir == TraverseDirection.Up) { y += 1; }
                else if (dir == TraverseDirection.Down) { y -= 1; }
                else if (dir == TraverseDirection.Right) { x += 1; }
                else if (dir == TraverseDirection.Left) { x -= 1; }
                else if (dir == TraverseDirection.UpLeft) { y += 1; x -= 1; }
                else if (dir == TraverseDirection.UpRight) { y += 1; x += 1; }
                else if (dir == TraverseDirection.DownLeft) { y -= 1; x -= 1; }
                else if (dir == TraverseDirection.DownRight) { y -= 1; x += 1; }

                //check if the new x and y is out of bound now
                if (grid.GetLength(0) <= x | grid.GetLength(1) <= y)
                {
                    return false;
                }
                else
                {
                    return grid[x, y];
                }

            }
        }

        // Create(): Creates a grid lattice discretized space for navigation.
        // canvasOrigin: bottom left corner of navigable region in world coordinates
        // canvasWidth: width of navigable region in world dimensions
        // canvasHeight: height of navigable region in world dimensions
        // cellWidth: target cell width (of a grid cell) in world dimensions
        // obstacles: a list of collider obstacles
        // grid: an array of bools. row major. a cell is true if navigable, false otherwise

        public static void Create(Vector2 canvasOrigin, float canvasWidth, float canvasHeight, float cellWidth,
            List<Polygon> obstacles,
            out bool[,] grid
            )
        {
            // ignoring the obstacles for this limited demo; 
            // Marks cells of the grid untraversable if geometry intersects interior!
            // Carefully consider all possible geometry interactions

            // also ignoring the world boundary defined by canvasOrigin and canvasWidth and canvasHeight

            Debug.Log(canvasOrigin);
            int grid_size_x = Mathf.FloorToInt(canvasWidth / cellWidth);
            int grid_size_y = Mathf.FloorToInt(canvasHeight / cellWidth);
            
            grid = new bool[grid_size_x, grid_size_y];

            for (int i =0; i <grid.GetLength(0); i++)
            {
                for (int j=0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = true;
                    foreach (Polygon obstacle in obstacles)
                    {
                        Vector2 minBounds = new Vector2();
                        Vector2 maxBounds = new Vector2();
                        minBounds.x = canvasOrigin.x + i * cellWidth;
                        minBounds.y = canvasOrigin.y + i * cellWidth;
                        maxBounds.x = minBounds.x + cellWidth;
                        maxBounds.y = minBounds.y + cellWidth;

                        Vector2Int minBoundsInt = Convert(minBounds);
                        Vector2Int maxBoundsInt = Convert(maxBounds);
                        Vector2[] pts = obstacle.getPoints();
                        //Debug.Log("minBoundsInt: " + minBoundsInt);
                        //Debug.Log("maxBoundsInt: " + maxBoundsInt);
                        foreach (Vector2 pt in pts)
                        {
                            Vector2Int pt_int = Convert(pt);
                            // Debug.Log("pt: " + pt_int);
                            bool navigable = PointInsideBoundingBox(minBoundsInt, maxBoundsInt, pt_int);
                            if (navigable == true)
                            {
                                Debug.Log("obstacle inside ");
                                grid[i, j] = false;
                                break;
                            }
                        }
                        
                        
                    }


                }
            }


        }

    }

}