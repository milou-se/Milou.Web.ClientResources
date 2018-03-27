using System;
using System.Collections.Generic;
using System.Linq;

namespace Milou.Web.ClientResources
{
    //Note: Licensed under The MIT License (MIT), originally taken from https://gist.github.com/niklaslundberg/5d03c22b9226fcb8f3c9
    internal sealed class ContentType : IEquatable<ContentType>
    {
        public static readonly ContentType Csv = new ContentType("CSV", "text/csv", ".csv");

        public static readonly ContentType Html = new ContentType("HTML", "text/html", ".html");

        public static readonly ContentType Jpeg = new ContentType("JPEG", "image/jpeg", ".jpg");

        public static readonly ContentType Css = new ContentType("CSS", "text/css", ".css");

        public static readonly ContentType Json = new ContentType("JSON", "application/json", ".json");

        public static readonly ContentType JavaScript = new ContentType("JavaScript", "application/javascript", ".js");

        public static readonly ContentType Pdf = new ContentType("PDF", "application/pdf", ".pdf");

        public static readonly ContentType PlainText = new ContentType("Plaintext", "text/plain", ".txt");

        public static readonly ContentType FormUrlEncoded = new ContentType(
            "Plaintext",
            "application/x-www-form-urlencoded",
            "");

        private ContentType(string displayName, string contentType, string fileExtension)
        {
            DisplayName = displayName;
            RegisteredContentType = contentType;
            FileExtension = fileExtension;
        }

        public string DisplayName { get; }

        public string RegisteredContentType { get; }

        public string FileExtension { get; }

        public static bool operator ==(ContentType left, ContentType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ContentType left, ContentType right)
        {
            return !Equals(left, right);
        }

        public static IReadOnlyCollection<ContentType> All { get; } =
            typeof(ContentType).GetFields()
                .Where(field => field.IsPublic && field.IsStatic && field.FieldType == typeof(ContentType))
                .Select(field => (ContentType)field.GetValue(null))
                .SafeToReadOnlyCollection();

        public bool Equals(ContentType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(RegisteredContentType, other.RegisteredContentType);
        }

        public override string ToString()
        {
            return $"{DisplayName} '{RegisteredContentType}' '{FileExtension}'";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is ContentType && Equals((ContentType)obj);
        }

        public override int GetHashCode()
        {
            return RegisteredContentType?.GetHashCode() ?? 0;
        }
    }
}