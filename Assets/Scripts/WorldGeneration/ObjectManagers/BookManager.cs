using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField] private GameObject bookSpotPrefab;

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
    ///     This function removes all existing book objects
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
            book.SetWorldIndex(data.GetArea().GetWorldIndex());
            if (data.GetArea().IsDungeon())
            {
                book.SetDungeonIndex(data.GetArea().GetDungeonIndex());                
            }
            else
            {
                book.SetDungeonIndex(0);
            }
            book.SetIndex(data.GetIndex());
            book.SetName(data.GetName());
        }
        else
        {
            Debug.LogError("Error creating book - Script not found");
        }
    }
}
