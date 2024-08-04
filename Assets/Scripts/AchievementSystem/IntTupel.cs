[System.Serializable]
public class IntTupel
{
    public int First { get; set; }
    public int Second { get; set; }
    public int Third { get; set; }

    public IntTupel(int first, int second, int third)
    {
        First = first;
        Second = second;
        Third = third;
    }
}
