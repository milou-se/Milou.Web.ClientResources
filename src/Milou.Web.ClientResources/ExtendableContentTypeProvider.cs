using System.Collections.Generic;
using Microsoft.Owin.StaticFiles.ContentTypes;

namespace Milou.Web.ClientResources
{
    public class ExtendableContentTypeProvider : FileExtensionContentTypeProvider
    {
        public ExtendableContentTypeProvider(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            foreach (KeyValuePair<string, string> keyValuePair in pairs)
            {
                if (Mappings.ContainsKey(keyValuePair.Key))
                {
                    Mappings[keyValuePair.Key] = keyValuePair.Value;
                }
                else
                {
                    Mappings.Add(keyValuePair);
                }
            }
        }
    }
}