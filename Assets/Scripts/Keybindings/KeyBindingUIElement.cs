using UnityEngine;
using TMPro;

public class KeyBindingUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text binding;

    private Keybinding keybinding;

    public void Setup(Keybinding keybinding)
    {
        this.keybinding = keybinding;
        title.text = keybinding.GetBinding().ToString();
        binding.text = keybinding.GetKey().ToString();
    }
}
