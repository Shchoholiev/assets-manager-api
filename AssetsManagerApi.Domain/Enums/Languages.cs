namespace AssetsManagerApi.Domain.Enums;

public enum Languages
{
    csharp = 0,
    python = 1,
    javascript = 2,
    xml
}

public static class LanguagesExtensions
{
    public static string LanguageToString(this Languages language)
    {
        return language switch
        {
            Languages.csharp => "Csharp",
            Languages.javascript => "Javascript",
            Languages.python => "Python",
            Languages.xml => "XML",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };
    }

    public static Languages StringToLanguage(this string language)
    {
        return language switch
        {
            "Csharp" => Languages.csharp,
            "Javascript" => Languages.javascript,
            "Python" => Languages.python,
            "XML" => Languages.xml,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };
    }
}
