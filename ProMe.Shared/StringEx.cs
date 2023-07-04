namespace ProMe.Shared;

public static class StringEx
{
    public static string FromBase64Url(this string input)
    {
        return input.Replace('-', '+').Replace("_", "/");
    }
}
