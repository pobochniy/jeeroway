namespace JeerowayWiki.Client
{
    public class Environments
    {
        public const string imgBaseHref = "http://img-jeeroway.1gb.ru";

        public static string ImgHref(string path)
        {
            return imgBaseHref + path;
        }
    }
}
