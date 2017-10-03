using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureCosmosPlayAreaDocumentDb.Models
{
    public class SubCategoryViewModel
    {
        public IList<Category> CategoryList { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        [Required(ErrorMessage = "SubCategory Name is Required")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        [Required(ErrorMessage = "Please select a catagory")]
        public Guid CategoryId { get; set; }

        
    }
}