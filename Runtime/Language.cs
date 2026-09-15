namespace Muco {
    public enum Language
    {
        English,
        Dansk,
        Deutsch,
        Nederlands,
        Français
    }

    public static class LanguageExtensions
    {
        public static string ToBcp47(this Language lang) => lang switch
        {
            Language.English    => "en-GB",
            Language.Dansk      => "da-DK",
            Language.Deutsch    => "de-DE",
            Language.Nederlands => "nl-NL",
            Language.Français   => "fr-FR",
            _                   => "en-GB",
        };

        public static Language FromBcp47(string tag) => tag switch
        {
            "en-GB" => Language.English,
            "da-DK" => Language.Dansk,
            "de-DE" => Language.Deutsch,
            "nl-NL" => Language.Nederlands,
            "fr-FR" => Language.Français,
            _       => Language.English,
        };
    }
}
