using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This classes manages the creation of book objects and placeholder icons
/// </summary>
public class BookManager : MonoBehaviour
{
    [SerializeField] private GameObject bookSpotPrefab;
    [SerializeField] private GameObject placeholderPrefab;

    /// <summary>
    ///     This function sets up book objects for the data given
    /// </summary>
    /// <param name="bookSpots">The data needed for the books</param>
    public void Setup(List<BookSpotData> bookSpots)
    {
        ClearBookSpots();
        foreach (BookSpotData bookSpotData in bookSpots)
        {
            CreateBookSpot(bookSpotData);
        }
    }

    /// <summary>
    ///     This function sets up placeholder book objects for the data given
    /// </summary>
    /// <param name="bookSpots">The data needed for the books</param>
    public void SetupPlaceholder(List<BookSpotData> bookSpots)
    {
        ClearBookSpots();
        foreach (BookSpotData bookSpotData in bookSpots)
        {
            CreatePlaceholderBookSpot(bookSpotData);
        }
    }

    /// <summary>
    ///     This function removes all existing book objects of the given area
    /// </summary>
    private void ClearBookSpots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    ///     This function creates a book spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the book spot</param>
    private void CreateBookSpot(BookSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject bookSpot = Instantiate(bookSpotPrefab, position, Quaternion.identity, this.transform) as GameObject;

        Book book = bookSpot.GetComponent<Book>();
        if (book != null)
        {
            book.Initialize(data.GetArea(), data.GetIndex(), data.GetName());
        }
        else
        {
            Debug.LogError("Error creating book - Script not found");
        }
    }

    /// <summary>
    ///     This function creates a placeholder book spot game object and sets it up with the given data
    /// </summary>
    /// <param name="data">The data for the book spot</param>
    private void CreatePlaceholderBookSpot(BookSpotData data)
    {
        Vector3 position = new Vector3(data.GetPosition().x, data.GetPosition().y, 0);
        GameObject placeholderSpot = Instantiate(placeholderPrefab, position, Quaternion.identity, this.transform) as GameObject;
        PlaceholderObject placeholder = placeholderSpot.GetComponent<PlaceholderObject>();
        if (placeholder != null)
        {
            placeholder.Setup(PlaceholderType.BOOK, data.GetIndex());
        }
        else
        {
            Debug.LogError("Error creating placeholder book spot");
        }
    }
}
