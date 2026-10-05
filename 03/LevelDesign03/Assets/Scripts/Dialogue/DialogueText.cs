using UnityEngine;
using TMPro;

public class DialogueText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _speakingText;
    [SerializeField]
    private Color32 _defaultSpeakingTextColor;
    [SerializeField]
    private float _outlineWidth;
    [SerializeField]
    private TextMeshProUGUI _characterText;
    [SerializeField]
    private Color32 _defaultCharacterTextColor;
    [SerializeField]
    private TextMeshProUGUI[] _choiceText;
    [SerializeField]
    private Color32 _defaultChoiceTextColor;
    private Material _textMat;
    private Material _characterTextMat;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Code Source: https://stackoverflow.com/questions/62421243/adding-an-outline-to-a-textmeshpro-text 
        // Modified slightly for purposes of this class

        // 1. Get a copy of the material (Creates a new instance for this object)
        // If you use 'fontSharedMaterial', you will change it for EVERY text using this font!
        Material mat = _speakingText.fontSharedMaterial;
        _textMat = _speakingText.fontMaterial;
        _characterTextMat = _characterText.fontMaterial;

        // 2. Enable the Outline keyword (Required for some shaders to switch modes)
        mat.EnableKeyword("OUTLINE_ON");

        // 3. Set the Shader properties
        // Width is usually between 0.0 and 1.0
        mat.SetFloat("_OutlineWidth", _outlineWidth);
        _textMat.SetColor("_OutlineColor", _defaultSpeakingTextColor);
        _characterTextMat.SetColor("_OutlineColor", _defaultCharacterTextColor);

        // 4. IMPORTANT: Force TMP to update its mesh boundaries
        // Without this, the outline might get "clipped" or cut off at the edges
        _speakingText.UpdateMeshPadding();
        _characterText.UpdateMeshPadding();
    }

    public void SetChoiceOutlineColor()
    {
        for (int i = 0; i < _choiceText.Length; i++)
        {
            Material mat = _choiceText[i].fontMaterial;
            mat.SetColor("_OutlineColor", _defaultChoiceTextColor);
            _choiceText[i].UpdateMeshPadding();
        }
    }

    public void SetChoiceBox(string optionText, int choiceBoxNum)
    {
        if (_choiceText.Length >= choiceBoxNum)
        {
            Debug.Log("there's only support for two dialogue choices unless if u add more button in prefab");
        }
        _choiceText[choiceBoxNum].text = optionText;
    }

    public void SetSpeakingText(string text)
    {
        _speakingText.text = text;
    }

    public void SetCharacterText(string characterName)
    {
        _characterText.text = characterName;
    }
}
