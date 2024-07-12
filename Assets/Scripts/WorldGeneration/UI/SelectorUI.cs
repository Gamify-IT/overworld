using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SelectorUI : MonoBehaviour
{
    /// <summary>
    ///     Import of the overworld close methode
    /// </summary>
    [DllImport("__Internal")]
    private static extern void CloseOverworld();

    public async UniTask ContinueButton()
    {
        await GameSettings.FetchValues();
        await SceneManager.LoadSceneAsync("AreaGeneratorManager");
    }
}
