using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureCosmosPlayAreaDocumentDb.Models
{
    public class Category
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        [Required(ErrorMessage = "Catagory Name is Required")]
        public string Name { get; set; }
    }
}