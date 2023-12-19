using UnityEngine;

/// <summary>
///     This class describs a minimap icon
/// </summary>
public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private MinimapIconType type;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite worldSprite;
    [SerializeField] private Sprite dungeonSprite;
    [SerializeField] private Sprite npcSprite;

    private void Awake()
    {
        type = MinimapIconType.UNSET;
        UpdateSprite();
    }

    public void SetMinimapIconType(MinimapIconType type)
    {
        this.type = type;
        UpdateSprite();
    }

    public MinimapIconType GetMinimapIconType()
    {
        return type;
    }

    /// <summary>
    ///     This function sets the correct sprite, based on the type of the minimap icon
    /// </summary>
    private void UpdateSprite()
    {
        switch (type)
        {
            case MinimapIconType.UNSET:
                spriteRenderer.sprite = null;
                break;

            case MinimapIconType.WORLD:
                spriteRenderer.sprite = worldSprite;
                break;

            case MinimapIconType.DUNGEON:
                spriteRenderer.sprite = dungeonSprite;
                break;

            case MinimapIconType.NPC:
                spriteRenderer.sprite = npcSprite;
                break;
        }
    }
}
