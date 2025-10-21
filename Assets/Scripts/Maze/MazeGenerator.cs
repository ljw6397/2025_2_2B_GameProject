using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public static MazeGenerator Instance;

    [Header("mirror")]
    public int width = 10;
    public int height = 10;
    public GameObject cellPrefab;
    public float cellSize = 2f;

    [Header("sigakwha")]
    public bool visualizeGeneration = false;   //process
    public float viauliztionSpeed = 0.05f;     //speed
    public Color visitedColor = Color.cyan;     //visited color
    public Color currentColor = Color.yellow;    //now color
    public Color backtrackColor = Color.magenta;   //back color

    private MazeCell[,] maze;
    private Stack<MazeCell> cellStack;     //Dfs stack
    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMaze()
    {
        maze = new MazeCell[width, height];
        cellStack = new Stack<MazeCell>();

        CreateCells();     //all cell 

        if(visualizeGeneration)
        {
            StartCoroutine(GenerateWithDFSVisualized());
        }
        else
        {
            GenerateWithDFS();
        }
    }
    void GenerateWithDFS()              //dfs algorithm
    {
        MazeCell current = maze[0, 0];
        current.visited = true;
        cellStack.Push(current);                   //now first put stack

        while(cellStack.Count > 0)
        {
            current = cellStack.Peek();

            //no visited neighbors find
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current);     //find neighbors

            if (unvisitedNeighbors.Count > 0)
            {     //random
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  //random neighbors sellect
                RemoveWallBetween(current, next);        //delete wall
                next.visited = true;
                cellStack.Push(next);
            }
            else
            {
                cellStack.Pop();   //back tracking
            }
        }
    }

    void CreateCells()     //cell function
    {
        if(cellPrefab == null)
        {
            Debug.LogError("No cellprefab");
            return; 
        }
        
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject cellobj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellobj.name = $"Cell_{x}_{z}";

                MazeCell cell = cellobj.GetComponent<MazeCell>();
                if (cell == null)
                {
                    Debug.LogError("MazeCell script dont have");
                    return;

                }
                cell.Initialize(x, z);
                maze[x, z] = cell;
            }
        }
    }

    List<MazeCell> GetUnvisitedNeighbors(MazeCell cell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();

        //updown~ check
        if (cell.x > 0 && !maze[cell.x - 1, cell.z].visited)
            neighbors.Add(maze[cell.x - 1, cell.z]);
        if (cell.x < width - 1 && !maze[cell.x + 1, cell.z].visited)
            neighbors.Add(maze[cell.x + 1, cell.z]);
        if (cell.z > 0 && !maze[cell.x, cell.z - 1].visited)
            neighbors.Add(maze[cell.x, cell.z - 1]);
        if (cell.z < width - 1 && !maze[cell.x, cell.z + 1].visited)
            neighbors.Add(maze[cell.x, cell.z + 1]);

        return neighbors;
    }

    void RemoveWallBetween(MazeCell current, MazeCell next)    //between two cells delete
    {
        if(current.x < next.x)   //right
        {
            current.RemoveWall("right");
            next.RemoveWall("left");
        }
        else if (current.x > next.x)   //;left
        {
            current.RemoveWall("left");
            next.RemoveWall("right");
        }
        else if(current.z < next.z)  //up
        {
            current.RemoveWall("top");
            next.RemoveWall("bottom");
        }
        else if(current.z > next.z)   //down
        {
            current.RemoveWall("bottom");
            next.RemoveWall("top");
        }
    }

    //position take
    public  MazeCell GetCell(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
            return maze[x, z];

        return null;
    }

    IEnumerator GenerateWithDFSVisualized()     //visual 
    {
        MazeCell current = maze[0, 0];
        current.visited = true;
        
        current.SetColor(currentColor);    //visual coding
        cellStack.Clear();    //visual coding

        cellStack.Push(current);

        yield return new WaitForSeconds(viauliztionSpeed);    //+

        int totalCells = width * height;              //+
        int visitedCount = 1;                    //+

        while (cellStack.Count > 0)
        {
            current = cellStack.Peek();
            
            current.SetColor(currentColor);      //+(not gangzo)
            yield return new WaitForSeconds(viauliztionSpeed); //+

            //no visited neighbors find
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current);     //find neighbors

            if (unvisitedNeighbors.Count > 0)
            {     //random
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  //random neighbors sellect
                RemoveWallBetween(current, next);        //delete wall

                current.SetColor(visitedColor);  //+
                next.visited = true;
                visitedCount++;    //+
                cellStack.Push(next);

                next.SetColor(currentColor);//+
                yield return new WaitForSeconds(viauliztionSpeed);  //+
            }
            else
            {
                current.SetColor(backtrackColor);
                yield return new WaitForSeconds(viauliztionSpeed);   //+

                current.SetColor(visitedColor);
                cellStack.Pop();
            }

            yield return new WaitForSeconds(viauliztionSpeed);   //+
            ResetAllColors();
            Debug.Log($"Make Mirror success! ({visitedCount} / {totalCells} )");
           
        }

        void ResetAllColors()          //reset all color
        {
            for (int x = 0; x < width; x++)
            {
                for(int z = 0; z< height; z++)
                {
                    maze[x, z].SetColor(Color.white);
                }
            }
        }
    }
}
