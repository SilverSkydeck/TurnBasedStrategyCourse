using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public static Pathfinding Instance 
    { 
        get; 
        private set; 
    }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 15;
    
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are more than one Pathfinding" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize,
                              (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        //GetNode(2, 3).SetIsWalkable(false);
        //GetNode(3, 1).SetIsWalkable(false);
        //GetNode(3, 2).SetIsWalkable(false);
        //GetNode(3, 3).SetIsWalkable(false);

        for (int x = 0; x< width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

                float raycastOffsetDistance = 5f;
                if( Physics.Raycast(
                    worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up,
                    raycastOffsetDistance * 2,
                    obstaclesLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);

                }   
                    
                    

            }
        }


    }


    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition , out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetPreviousPathNode();

            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode) //reached final node
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);

            }
               

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode)) continue;

                if (!neighborNode.GetIsWalkable())
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighborNode.GetGridPosition());

                if (tentativeGCost < neighborNode.GetGCost())
                {
                    neighborNode.SetPreviousPathNode(currentNode);
                    neighborNode.SetGCost(tentativeGCost);
                    neighborNode.SetHCost(CalculateDistance(neighborNode.GetGridPosition(), endGridPosition));
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode)) openList.Add(neighborNode);
                }

            }

        }
        pathLength = 0;
        return null; //no path found
    }

    public int CalculateDistance(GridPosition A, GridPosition B) // why calc distance not calc h cost?
    {
        GridPosition distance = A - B ;
        //int absDistance = Mathf.Abs(distance.x) + Mathf.Abs(distance.z);

        int horiDistance = Mathf.Abs(distance.x);
        int vertDistance = Mathf.Abs(distance.z);

        int straightMoveCount = Mathf.Abs(horiDistance - vertDistance);
        int diagonalMoveCount = Mathf.Min(horiDistance, vertDistance);

        return straightMoveCount * MOVE_STRAIGHT_COST + diagonalMoveCount * MOVE_DIAGONAL_COST;
        //return absDistance * MOVE_STRAIGHT_COST;



    }


    private PathNode GetLowestFCostPathNode (List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];

        for (int i = 0; i < pathNodeList.Count ; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;


    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetGridObject( new GridPosition(x, z) );
    }


    private List<PathNode> GetNeighborList(PathNode currentNode)
    {

        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));//left
            if(gridPosition.z + 1 < gridSystem.GetHeight())
            { 
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));//left up
            }
            if(gridPosition.z - 1 >=0)
            { 
                neighborList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));//left down
            }
        }
        if(gridPosition.x + 1 < gridSystem.GetWidth())
        { 
            neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));//right
            if (gridPosition.z + 1 < gridSystem.GetHeight())
            {
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));//right up
            }
            if (gridPosition.z - 1 >= 0)
            {
                neighborList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));//right down
            }
        }
        if (gridPosition.z + 1 < gridSystem.GetHeight())
        {
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));//up
        }
        if (gridPosition.z - 1 >= 0)
        {
            neighborList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));//down
        }

        return neighborList;


    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);

        PathNode currentPathNode = endNode;

        while(currentPathNode.GetPreviousPathNode() != null)
        {
            pathNodeList.Add(currentPathNode.GetPreviousPathNode());
            currentPathNode = currentPathNode.GetPreviousPathNode();
        }

        pathNodeList.Reverse();
        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathNode pathNode in pathNodeList) gridPositionList.Add(pathNode.GetGridPosition());

        return gridPositionList;
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).GetIsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }



}
