using System;

namespace Our.Umbraco.RedirectsViewer.Extensions
{
    public static class UriExtensions
    {
        public static Uri ToUri(this string value)
        {
            Uri uri;

            if (Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                return uri;
            }

            return null;
        }
    }
}