
using UnityEngine;

namespace Muco
{
    [RequireComponent(typeof(TMPro.TextMeshPro))]
    public class MultiLangTextBox : MonoBehaviour
    {

        [TextArea]
        public string Dansk;
        [TextArea]
        public string English;

        [TextArea]
        public string Deutsch;

        [Tooltip("Optional per-language text file. When assigned, it takes precedence over the inline string.")]
        public TextAsset danskFile;
        [Tooltip("Optional per-language text file. When assigned, it takes precedence over the inline string.")]
        public TextAsset englishFile;
        [Tooltip("Optional per-language text file. When assigned, it takes precedence over the inline string.")]
        public TextAsset deutschFile;

        public void SelectLanguage(Language language)
        {
            var textComponent = GetComponent<TMPro.TextMeshPro>();

            string text;
            switch (language)
            {
                case Language.English:
                    text = englishFile != null ? englishFile.text : English;
                    break;
                case Language.Dansk:
                    text = danskFile != null ? danskFile.text : Dansk;
                    break;
                case Language.Deutsch:
                    text = deutschFile != null ? deutschFile.text : Deutsch;
                    break;
                default:
                    Debug.Log("Unsupported language: " + language);
                    return;
            }

            textComponent.text = text.Replace('\n', '\u000a').Replace('\t', '\t');
        }

    }
}
