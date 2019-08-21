using System;

namespace Skybrud.Umbraco.Redirects.Extensions
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