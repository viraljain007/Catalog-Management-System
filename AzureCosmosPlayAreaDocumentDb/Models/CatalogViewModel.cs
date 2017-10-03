using System.Collections.Generic;

namespace AzureCosmosPlayAreaDocumentDb.Models
{
    public class CatalogViewModel
    {
        public IList<Category> CategoryList { get; set; }

        public IList<SubCategory> SubCategoryList { get; set; }

        public IList<Product> ProductList { get; set; }
    }
}