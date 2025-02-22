using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    private const string multiplayerBackendPath = "/multiplayer/api/v1";

    public async void StartMultiplayer()
    {
        bool successful = await ServerHandshake();

        if (successful)
        {
            MultiplayerManager.Instance.Initialize();
            UpdateButtonText();
        }
        else
        {
            Debug.LogError("Unable to connect to server!");
        }
    }

    private async UniTask<bool> ServerHandshake()
    {
        ConnectionDTO connectionDTO = new(GameManager.Instance.GetUserId(), GameManager.Instance.GetCourseId());
        string basePath = multiplayerBackendPath + "/join";
        string json = JsonUtility.ToJson(connectionDTO, true);

        Optional<ResponseDTO> data = await RestRequest.PostRequest<ResponseDTO>(basePath, json);

        if (data.IsPresent())
        {
            MultiplayerManager.Instance.SetPlayerId(data.Value().playerId);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateButtonText()
    {
        if (MultiplayerManager.Instance.IsConnected())
        {
            buttonText.text = "STOP";
        }
        else
        {
            buttonText.text = "START";
        }
    }
}
