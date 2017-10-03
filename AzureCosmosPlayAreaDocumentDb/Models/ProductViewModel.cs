using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzureCosmosPlayAreaDocumentDb.Models
{
    public class ProductViewModel
    {
        public IList<SubCategory> SubCategoryList { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        [Required(ErrorMessage = "Product Name is Required")]
        public string Name { get; set; }

        [JsonProperty("subcategoryId")]
        [Required(ErrorMessage = "Please select a Sub Category")]
        public Guid SubCategoryId { get; set; }
    }
}