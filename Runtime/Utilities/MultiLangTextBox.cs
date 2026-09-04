using UnityEngine;

namespace Muco
{
[RequireComponent(typeof(TMPro.TextMeshPro))]
public class MultiLangTextBox : MonoBehaviour
{

[Header("Manual input")]
[TextArea]
public string Dansk;
[TextArea]
public string English;

[TextArea]
public string Deutsch;

[Header("Text file override (takes precedence over manual input)")]
public TextAsset danskFile;
public TextAsset englishFile;
public TextAsset deutschFile;
// Might want to do this dynamically based on Language

public void SelectLanguage(Language language)
{
var textComponent = GetComponent<TMPro.TextMeshPro>();

switch (language)
{
case Language.English:
textComponent.text = Resolve(englishFile, English);
break;
case Language.Dansk:
textComponent.text = Resolve(danskFile, Dansk);
break;
case Language.Deutsch:
textComponent.text = Resolve(deutschFile, Deutsch);
break;
default:
Debug.Log("Unsupported language: " + language);
break;
}
}

private static string Resolve(TextAsset file, string manualText)
{
return file != null ? file.text : manualText;
}

}
}