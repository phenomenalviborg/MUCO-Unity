using System;
using System.Collections.Generic;
using UnityEngine;

namespace Muco
{
    [Serializable]
    public class LocalizedText
    {
        [Tooltip("BCP47 tag, for example en-GB or da-DK.")]
        public string languageTag;

        [TextArea]
        public string text;
    }

    [RequireComponent(typeof(TMPro.TextMeshPro))]
    public class MultiLangTextBox : MonoBehaviour
    {
        [Tooltip("Translations used by this text box. Empty tags are ignored.")]
        public List<LocalizedText> translations = new List<LocalizedText>();

        [Tooltip("Used when the requested language is unavailable.")]
        public string fallbackLanguageTag = "en-GB";

        public IEnumerable<string> LanguageTags
        {
            get
            {
                foreach (var translation in translations)
                {
                    var tag = NormalizeTag(translation.languageTag);
                    if (!string.IsNullOrEmpty(tag))
                        yield return tag;
                }
            }
        }

        public void SelectLanguage(Language language)
        {
            SelectLanguage(language.ToBcp47());
        }

        public void SelectLanguage(string languageTag)
        {
            var text = GetText(languageTag);
            if (text == null)
            {
                Debug.LogWarning($"No translation found for '{languageTag}' on {name}.", this);
                return;
            }

            GetComponent<TMPro.TextMeshPro>().text = text.Replace("\\n", "\n").Replace("\\t", "\t");
        }

        public string GetText(string languageTag)
        {
            var requested = NormalizeTag(languageTag);
            var fallback = NormalizeTag(fallbackLanguageTag);

            var text = FindText(requested);
            if (text != null)
                return text;

            var requestedLanguage = GetLanguagePart(requested);
            text = FindText(requestedLanguage);
            if (text != null)
                return text;

            text = FindText(fallback);
            if (text != null)
                return text;

            return FindText(GetLanguagePart(fallback));
        }

        public static string NormalizeTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return string.Empty;

            tag = tag.Trim().Replace('_', '-');
            var parts = tag.Split('-');
            if (parts.Length == 0 || parts[0].Length < 2 || parts[0].Length > 8)
                return string.Empty;

            for (var i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i]))
                    return string.Empty;
                parts[i] = i == 0 ? parts[i].ToLowerInvariant() : parts[i].Length == 2 || parts[i].Length == 3 ? parts[i].ToUpperInvariant() : parts[i];
            }

            return string.Join("-", parts);
        }

        private string FindText(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            foreach (var translation in translations)
            {
                if (NormalizeTag(translation.languageTag) == tag)
                    return translation.text;
            }

            return null;
        }

        private static string GetLanguagePart(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return string.Empty;

            var separator = tag.IndexOf('-');
            return separator < 0 ? tag : tag.Substring(0, separator);
        }
    }
}
