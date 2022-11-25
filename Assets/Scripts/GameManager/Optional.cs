/// <summary>
///     This struct specifies an Optional of given type <c>T</c>.
/// </summary>
/// <typeparam name="T">The type of the Optional</typeparam>
[System.Serializable]
public struct Optional<T>
{
    private bool present;
    private T value;

    public Optional(T initialValue)
    {
        present = true;
        value = initialValue;
    }

    public bool IsPresent()
    {
        return present;
    }

    public T Value()
    {
        return value;
    }

    public void Clear()
    {
        present = false;
    }

    public void SetValue(T value)
    {
        present = true;
        this.value = value;
    }
}
