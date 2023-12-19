/// <summary>
///     This class is used by the <c>DrunkardsWalkGenerator</c> for shifting the layout and contains the distances of the layout structure to each border
/// </summary>
public class LayoutDistance
{
    public int left;
    public int up;
    public int right;
    public int down;

    public LayoutDistance(int left, int up, int right, int down)
    {
        this.left = left;
        this.up = up;
        this.right = right;
        this.down = down;
    }
}
