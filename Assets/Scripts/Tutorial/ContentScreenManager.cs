using TMPro;
using UnityEngine;

public class ContentScreenManager : MonoBehaviour
{
    // singleton
    public static ContentScreenManager Instance { get; private set; }

    [Header("Content Screen")]
    [SerializeField] private TMP_Text header;
    [SerializeField] private TMP_Text content;
    [SerializeField] private TMP_Text buttonLabel;

    #region singleton
    // <summary>
    ///     This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    ///     deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public void Setup(ContentScreenData data)
    {
        header.text = data.GetHeader();
        content.text = data.GetContent();
        buttonLabel.text = data.GetButtonLabel();
    }

    public void Close()
    {
        TutorialManager.Instance.ActivateInfoScreen(false);
    }
} 