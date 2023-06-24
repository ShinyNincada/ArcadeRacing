    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarLite : MonoBehaviour
{
    int gridSizeX = 60;
    int gridSizeY = 50;

    float cellSize = 2;

    AStarNode[,] aStarNodes;    
    AStarNode startNode;

    List<AStarNode> nodesToCheck = new List<AStarNode>();
    List<AStarNode> nodesChecked = new List<AStarNode>();
    List<Vector2> aiPath = new List<Vector2>();

    //Debug
    Vector3 startPositionDebug = new Vector3(1000, 0, 0);
    Vector3 destinationPositionDebug = new Vector3(1000, 0 ,0);
    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();

        PathFinding(new Vector2(1, 7));
    }

    void CreateGrid(){
        //Allocate space in the array for the nodes
        aStarNodes = new AStarNode[gridSizeX, gridSizeY];
        
        //Create the grid of nodes
        for(int x = 0; x < gridSizeX; x++){
            for(int y = 0; y < gridSizeY; y++){
                aStarNodes[x, y] = new AStarNode(new Vector2Int(x, y));

                Vector3 worldPosition = ConvertGridPositionToWorldPosition(aStarNodes[x, y]);

                //Check if the nodes is an obstacle
                Collider2D hitCollider2D = Physics2D.OverlapCircle(worldPosition, cellSize/2.0f);

                if(hitCollider2D != null){
                    //Ignore AI cars, they not obstacle
                    if(hitCollider2D.transform.root.CompareTag("AI")){
                        continue;
                    }
                   
                    //Ignore AI cars, they not obstacle
                    if(hitCollider2D.transform.root.CompareTag("Player")){
                        continue;
                    }

                    if(hitCollider2D.isTrigger){
                        continue;
                    }
                    
                    //Mark as obstacle
                    aStarNodes[x, y].isObstacle = true;
                  

                }
            }
        }

        // Loop through the grid again and populate 
        for(int x = 0; x < gridSizeX; x++)
            for(int y = 0; y < gridSizeY; y++){
                // Check the  to North, if we are on the edge so don't add it
                if(y - 1 >= 0){
                    if(!aStarNodes[x,y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x, y-1]);
                }
                
                // Check the  to South, if we are on the edge so don't add it
                if(y + 1 <=  gridSizeY - 1){
                    if(!aStarNodes[x,y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x, y+1]);
                }

                // Check the to West, if we are on the edge so don't add it
                if(x - 1 >= 0){
                    if(!aStarNodes[x,y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x - 1, y]);
                }
                
                // Check the to East, if we are on the edge so don't add it
                if(x + 1 <=  gridSizeX - 1){
                    if(!aStarNodes[x,y].isObstacle)
                        aStarNodes[x, y].neighbours.Add(aStarNodes[x + 1, y]);
                }
            }
    }

    public List<Vector2> PathFinding(Vector2 destination){
        if(aStarNodes == null)
            return null;

        //Convert the destination from world to grid position
        Vector2Int destinationGridPoint = ConvertWorldToGridPoint(destination);
        Vector2Int currentPositionGridPoint = ConvertWorldToGridPoint(transform.position);

        //Set a debug position to show while developing
        destinationPositionDebug = destination;

        //Start the algorithm by calculating the costs for first node
        startNode = GetNodeFromPoint(currentPositionGridPoint);

        //Store the start grid for debug
        startPositionDebug = ConvertGridPositionToWorldPosition(startNode);

        // Set the current node to start node
        AStarNode currentNode = startNode;
        
        bool isDonePathFinding = false;
        int pickedOrder = 1;

        //Loop while we are not done with the path
        while(!isDonePathFinding){
            //Remove the current node from the list of nodes that should be checked.
            nodesToCheck.Remove(currentNode);

            //Set the pick order
            currentNode.pickedOrder = pickedOrder;

            pickedOrder++;

            //Add the current node to the checked list
            nodesChecked.Add(currentNode);  

            // Found it! Here come the destination
            if(currentNode.gridPosition == destinationGridPoint){
                isDonePathFinding = true;
                break;
            }

            //Calculate cost for all nodes
            CalculateCostsForNodeAndNeighbours(currentNode, currentPositionGridPoint, destinationGridPoint);

            //Check if the neighbour nodes should be considered
            foreach(AStarNode neighbourNode in currentNode.neighbours){
                //Skip any node that already checked
                if(nodesChecked.Contains(neighbourNode)){
                    continue;
                }

                //Skip any node that already on the list
                if(nodesToCheck.Contains(neighbourNode)){
                    continue;
                }

                //Add the node to the list that we should check
                nodesToCheck.Add(neighbourNode);
            }

            //Sort the list so that the items with the lowest Total cost (f cost and if they have the same value then lets pick the one with lowest cost to reach the goal)
            nodesToCheck = nodesToCheck.OrderBy(x => x.fCostTotal).ThenBy(x => x.hCostDistanceFromGoal).ToList();

            //Pick the node with lowest cost to be the next
            if(nodesToCheck.Count == 0){
                Debug.Log($"No  node left to check, we have no solution");
                return null;
            }
            else{
                currentNode = nodesToCheck[0];
            }
        }

        aiPath = CreatePathForAI(currentPositionGridPoint);

        return null;
    }

    List<Vector2> CreatePathForAI(Vector2Int currentPositionGridPoint){
        List<Vector2> resultAIPath = new List<Vector2>();
        List<AStarNode> aiPath = new List<AStarNode>();

        //Reverse the nodes to check as the last added node will be the AI destination
        nodesChecked.Reverse();

        bool isPathCreated = false;

        AStarNode currentNode = nodesChecked[0];

        aiPath.Add(currentNode);

        int attempts = 0;

        while (!isPathCreated)
        {
            //Go backwards with the lowest creation order
            currentNode.neighbours = currentNode.neighbours.OrderBy(x => x.pickedOrder).ToList();

            //Pick the neighbour with the lowest cost if it is not already in the list
            foreach (AStarNode aStarNode in currentNode.neighbours)
            {
                if (!aiPath.Contains(aStarNode) && nodesChecked.Contains(aStarNode))
                {
                    aiPath.Add(aStarNode);
                    currentNode = aStarNode;

                    break;
                }
            }

            if (currentNode == startNode)
                isPathCreated = true;

            if (attempts > 1000)
            {
                Debug.LogWarning("CreatePathForAI failed after too many attempts");
                break;
            }

            attempts++;
        }

        foreach (AStarNode aStarNode in aiPath)
        {
            resultAIPath.Add(ConvertGridPositionToWorldPosition(aStarNode));
        }

        //Flip the result
        resultAIPath.Reverse();

        return resultAIPath;
    }
    void CalculateCostsForNodeAndNeighbours(AStarNode aStarNode, Vector2Int aiPosition, Vector2Int aiDestination){
        aStarNode.CalculatedCostsForNode(aiPosition, aiDestination);

        foreach(AStarNode neighbourNode in aStarNode.neighbours){
            neighbourNode.CalculatedCostsForNode(aiPosition, aiDestination);
        }
    }
    AStarNode GetNodeFromPoint(Vector2Int gridPoint){
        if(gridPoint.x < 0)
            return null;
        
        if(gridPoint.x > gridSizeX - 1)
            return null;
        
        if(gridPoint.y < 0)
            return null;
        
        if(gridPoint.y > gridSizeY - 1)
            return null;

        return aStarNodes[gridPoint.x, gridPoint.y];
        
    }
    Vector2Int ConvertWorldToGridPoint(Vector2 position){
        //Calculate grid point
        Vector2Int gridpoint = new Vector2Int(Mathf.RoundToInt(position.x / cellSize + gridSizeX / 2.0f), 
                                Mathf.RoundToInt(position.y / cellSize + gridSizeY / 2.0f));
        
        return gridpoint;
    }

    Vector3 ConvertGridPositionToWorldPosition(AStarNode aStarNode){
        return new Vector3(aStarNode.gridPosition.x * cellSize - (gridSizeX * cellSize)/2.0f, aStarNode.gridPosition.y * cellSize - (gridSizeY * cellSize)/2.0f, 0);
    }
    private void OnDrawGizmos() {
        if(aStarNodes == null)
            return;

         //Create the grid of nodes
        for(int x = 0; x < gridSizeX; x++){
            for(int y = 0; y < gridSizeY; y++){
                
                if(!aStarNodes[x, y].isObstacle){
                    Gizmos.color = Color.green;
                }
                else{
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawWireCube(ConvertGridPositionToWorldPosition(aStarNodes[x, y]), new Vector3(cellSize, cellSize, cellSize));
            }
        }
        
        //Draw the all the checked nodes 
        foreach(AStarNode checkedNode in nodesChecked){
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(checkedNode), 1.0f);
        }
        
        //Draw the all the next node to check
        foreach(AStarNode toCheckNode in nodesToCheck){
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(toCheckNode), 1.0f);
        }

       //Draw the nodes that we should check
        foreach (AStarNode toCheckNode in nodesToCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ConvertGridPositionToWorldPosition(toCheckNode), 1.0f);
        }

        Vector3 lastAIPoint = Vector3.zero;
        bool isFirstStep = true;

        Gizmos.color = Color.black;

        foreach (Vector2 point in aiPath)
        {
            if (!isFirstStep)
                Gizmos.DrawLine(lastAIPoint, point);

            lastAIPoint = point;

            isFirstStep = false;

        }

        //Draw the start position
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(startPositionDebug, 1f);

        //Draw the end position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(destinationPositionDebug, 1f);
    }
}
