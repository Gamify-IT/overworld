using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        char output = addedChar;

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
            //return a null character
            output = '\0';
        }

        return output;
    }
}
