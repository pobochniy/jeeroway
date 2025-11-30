namespace JeerowayWiki.Client
{
    public class Environments
    {
        public const string imgBaseHref = "https://img.jeeroway.ru";

        public static string ImgHref(string path)
        {
            return imgBaseHref + path;
        }
    }
}
