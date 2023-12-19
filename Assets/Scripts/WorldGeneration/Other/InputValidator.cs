using UnityEngine;
using TMPro;

/// <summary>
///     This class is used to allow inly positive number as the input of an input field
/// </summary>
public class InputValidator : MonoBehaviour
{
    private TMP_InputField inputField;
    
    void Start()
    {
        inputField = gameObject.GetComponent<TMP_InputField>();
        inputField.onValidateInput += ValidateInput;
    }

    public char ValidateInput(string text, int charIndex, char addedChar)
    {
        if (addedChar != '1'
            && addedChar != '2'
            && addedChar != '3'
            && addedChar != '4'
            && addedChar != '5'
            && addedChar != '6'
            && addedChar != '7'
            && addedChar != '8'
            && addedChar != '9'
            && addedChar != '0')
        {
            //input not a number -> return a null character
            return '\0';
        }

        //return added char
        return addedChar;
    }
}
