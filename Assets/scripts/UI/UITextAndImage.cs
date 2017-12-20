using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextAndImage : MonoBehaviour {

    [SerializeField] protected Text m_text;
    [SerializeField] protected Image m_image;

    public void SetText(string _text)
    {
        m_text.text = _text;
    }

    public void SetImage(Sprite _sprite)
    {
        m_image.sprite = _sprite;
    }
}
