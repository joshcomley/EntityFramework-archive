﻿using Microsoft.OData.UriParser;

namespace Microsoft.AspNetCore.OData
{
    public class ODataUriResolverSettings
    {
        public bool CaseInsensitive { get; set; }

        public bool UnqualifiedNameCall { get; set; }

        public bool EnumPrefixFree { get; set; }

        public ODataUriResolver CreateResolver()
        {
            ODataUriResolver resolver;
            if (UnqualifiedNameCall && EnumPrefixFree)
            {
                resolver = new UnqualifiedCallAndEnumPrefixFreeResolver();
            }
            else if (UnqualifiedNameCall)
            {
                resolver = new UnqualifiedODataUriResolver();
            }
            else if (EnumPrefixFree)
            {
                resolver = new StringAsEnumResolver();
            }
            else
            {
                resolver = new ODataUriResolver();
            }

            resolver.EnableCaseInsensitive = CaseInsensitive;
            return resolver;
        }
    }
}