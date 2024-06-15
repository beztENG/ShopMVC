using System.Text.Json;

public static class CookieHelper
{
    public static void Set<T>(this IResponseCookies cookies, string key, T value, int? expireTime)
    {
        CookieOptions option = new CookieOptions();

        if (expireTime.HasValue)
            option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
        else
            option.Expires = DateTime.Now.AddMinutes(30);

        var serializedValue = JsonSerializer.Serialize(value);
        cookies.Append(key, serializedValue, option);
    }

    public static T? Get<T>(this IRequestCookieCollection cookies, string key)
    {
        var value = cookies[key];
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}
