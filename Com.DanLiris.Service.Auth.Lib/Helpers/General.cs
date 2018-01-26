using System;
using System.Collections.Generic;

namespace Com.DanLiris.Service.Auth.Lib.Helpers
{
    public static class General
    {
        public const string ASCENDING = "asc";
        public const string DESCENDING = "desc";
        public const string ASIA_DATE_FORMAT = "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(SE Asia Standard Time)'";

        public static string TransformOrderBy(string Order)
        {
            List<string> Transform = new List<string> {
                "_id", "_deleted", "_active",
                "_createdDate", "_createdBy", "_createAgent",
                "_updatedDate", "_updatedBy", "_updateAgent"
            };

            if (Transform.FindIndex(t => string.Equals(Order, t, StringComparison.OrdinalIgnoreCase)) > -1)
            {
                /* Exists in the list */

                string TransformOrder = "";

                switch (Order)
                {
                    case "_deleted": TransformOrder = "_IsDeleted"; break;
                    case "_active": TransformOrder = "Active"; break;
                    case "_createdDate": TransformOrder = "_CreatedUtc"; break;
                    case "_createdBy": TransformOrder = "_CreatedBy"; break;
                    case "_createdAgent": TransformOrder = "_CreatedAgent"; break;
                    case "_updatedDate": TransformOrder = "_LastModifiedUtc"; break;
                    case "_updatedBy": TransformOrder = "_LastModifiedBy"; break;
                    case "_updateAgent": TransformOrder = "_LastModifiedAgent"; break;
                }

                return TransformOrder;
            }
            else
            {
                return Order;
            }
        }

        public static string BuildSearch(List<string> SearchAttributes)
        {
            string SearchQuery = String.Empty;
            foreach (string Attribute in SearchAttributes)
            {
                SearchQuery = string.Concat(SearchQuery, Attribute, ".Contains(@0) OR ");
            }

            SearchQuery = SearchQuery.Remove(SearchQuery.Length - 4);

            return SearchQuery;
        }
    }
}