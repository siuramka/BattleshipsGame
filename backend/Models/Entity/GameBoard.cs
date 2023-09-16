namespace backend.Models.Entity;

//Would be better to write a separate class for the grid
public class GameBoard
{
    public int[][] Grid { get; private set; }
    public int Size { get; private set; }
    public int ShipsRemaining { get; private set; }

    public GameBoard(int size = 10)
    {
        Size = size;
        Grid = new int[size][];
        ShipsRemaining = 0; // Initialize with zero ships, you can add ships later.
    }

    public void Initialize()
    {
        // Initialize the grid with empty cells (0)
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                Grid[row][col] = 0;
            }
        }
    }

    /// <summary>
    /// ships are just 1x1 for now
    /// </summary>
    public void AddRandomShipTest()
    {
        Grid[5][5] = 1;
        Grid[5][3] = 1;
        Grid[5][1] = 1;


        Grid[1][2] = 1;
        Grid[3][4] = 1;

        ShipsRemaining += 5;
    }

    public bool CanHit(int x, int y)
    {
        if (Grid[x][y] == 1)
        {
            return true;
        }

        return false;
    }

    // 1 = mark as checked
    // 2 = mark as ship hit
    // 0 = empty
    public bool MarkCell(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size)
        {
            // Coordinates out of bounds
            return false;
        }

        if (Grid[x][y] == 1)
        {
            Grid[x][y] = 2; // mark ship hit 1x1
            ShipsRemaining--;
            return true;
        }

        //assume cant hit on already hit targerds in frontend
        Grid[x][y] = 1;

        return true;
    }

    public bool IsGameOver()
    {
        return ShipsRemaining == 0;
    }

    public GameBoard Clone()
    {
        var clonedBoard = new GameBoard(Size);
        clonedBoard.ShipsRemaining = ShipsRemaining;

        // Copy the grid contents
        for (int row = 0; row < Size; row++)
        {
            for (int col = 0; col < Size; col++)
            {
                clonedBoard.Grid[row][col] = Grid[row][col];
            }
        }

        return clonedBoard;
    }
}