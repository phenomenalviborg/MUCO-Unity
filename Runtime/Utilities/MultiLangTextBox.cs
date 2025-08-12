
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
        // Might want to do this dynamically based on Language

        public void SelectLanguage(Language language)
        {
            var textComponent = GetComponent<TMPro.TextMeshPro>();

            switch (language)
            {
                case Language.English:
                    textComponent.text = English.Replace('\n', '\u000a').Replace('\t', '\t');
                    break;
                case Language.Dansk:
                    textComponent.text = Dansk.Replace('\n', '\u000a').Replace('\t', '\t');
                    break;
                case Language.Deutsch:
                    textComponent.text = Deutsch.Replace('\n', '\u000a').Replace('\t', '\t');
                    break;
                default:
                    Debug.Log("Unsupported language: " + language);
                    break;
            }
        }
        
    }
}
