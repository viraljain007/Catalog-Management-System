using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureCosmosPlayAreaDocumentDb.Models;
using AzureCosmosPlayAreaDocumentDb.Persistence;
using System.Configuration;

namespace AzureCosmosPlayAreaDocumentDb.Controllers
{
    public class CatalogueController : Controller
    {
        private static readonly string CategoryId = ConfigurationManager.AppSettings["CategoryId"];
        private static readonly string SubCategoryId = ConfigurationManager.AppSettings["SubCategoryId"];
        private static readonly string ProductId = ConfigurationManager.AppSettings["ProductId"];

        [ActionName("CatalogIndex")]
        public async Task<ActionResult> IndexAsyncL()
        {
            var category = await DocumentDbRepository<Category>.GetItemsAsync(d => d != null);
            var subcategory = await DocumentDbRepository<SubCategory>.GetDropDownAsync("SubCategory");
            var product = await DocumentDbRepository<Product>.GetDropDownAsync("Product");

            CatalogViewModel catalogViewModel = new CatalogViewModel();
            catalogViewModel.CategoryList = category;
            catalogViewModel.SubCategoryList = subcategory;
            catalogViewModel.ProductList = product;

            return View("CatalogIndex", catalogViewModel);
        }

        public ActionResult CreateCategory()
        {
            return View(new Category());
        }

        public async Task<ActionResult> CreateSubCategory()
        {
            var category = await DocumentDbRepository<Category>.GetItemsAsync(d => d != null);

            SubCategoryViewModel subCatalogViewModel = new SubCategoryViewModel();
            subCatalogViewModel.CategoryList = category;

            return View("CreateSubCategory", subCatalogViewModel);
        }

        public async Task<ActionResult> CreateProduct()
        {
            var subcategory = await DocumentDbRepository<SubCategory>.GetDropDownAsync("SubCategory");

            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.SubCategoryList = subcategory;

            return View("CreateProduct", productViewModel);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Category course)
        {
            if (ModelState.IsValid)
            {
                if (course.Id == default(Guid))
                {
                    course.Id = Guid.NewGuid();
                    await DocumentDbRepository<Category>.CreateItemAsync(course, CategoryId);
                }
                else
                {
                    await DocumentDbRepository<Category>.UpdateItemAsync(course.Id, course, CategoryId);
                }

                return RedirectToAction("CatalogIndex");
            }

            return View(course);
        }

        [HttpPost]
        [ActionName("CreateSubCategory")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubcategoryAsync(SubCategoryViewModel subCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                SubCategory subCategory = new SubCategory();
                subCategory.Id = subCategoryViewModel.Id;
                subCategory.Name = subCategoryViewModel.Name;
                subCategory.CategoryId = subCategoryViewModel.CategoryId;

                if (subCategory.Id == default(Guid))
                {
                    subCategory.Id = Guid.NewGuid();

                    await DocumentDbRepository<SubCategory>.CreateItemAsync(subCategory, SubCategoryId);
                }
                else
                {
                    await DocumentDbRepository<SubCategory>.UpdateItemAsync(subCategory.Id, subCategory, SubCategoryId);
                }

                return RedirectToAction("CatalogIndex");
            }

            return View(subCategoryViewModel);
        }

        [HttpPost]
        [ActionName("CreateProduct")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProductAsync(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product();
                product.Id = productViewModel.Id;
                product.Name = productViewModel.Name;
                product.SubCategoryId = productViewModel.SubCategoryId;

                if (product.Id == default(Guid))
                {
                    product.Id = Guid.NewGuid();
                    await DocumentDbRepository<Product>.CreateItemAsync(product, ProductId);
                }
                else
                {
                    await DocumentDbRepository<Product>.UpdateItemAsync(product.Id, product, ProductId);
                }

                return RedirectToAction("CatalogIndex");
            }

            return View(productViewModel);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync()
        {
            var category = await DocumentDbRepository<Category>.GetDropDownAsync("Category");
            var subcategory = await DocumentDbRepository<SubCategory>.GetDropDownAsync("SubCategory");
            var product = await DocumentDbRepository<Product>.GetDropDownAsync("Product");

            CatalogViewModel cvm = new CatalogViewModel();
            cvm.CategoryList = category;
            cvm.SubCategoryList = subcategory;
            cvm.ProductList = product;

            return View("CatalogManagement", cvm);
        }

        [ActionName("EditCategory")]
        public async Task<ActionResult> EditCategoryAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category category = await DocumentDbRepository<Category>.GetItemAsync(id, CategoryId);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View("CreateCategory", category);
        }

        [ActionName("EditSubCategory")]
        public async Task<ActionResult> EditSubCategoryAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SubCategory subcategory = await DocumentDbRepository<SubCategory>.GetItemAsync(id, SubCategoryId);

            if (subcategory == null)
            {
                return HttpNotFound();
            }

            var category = await DocumentDbRepository<Category>.GetItemsAsync(d => d != null);

            SubCategoryViewModel subCatalogViewModel = new SubCategoryViewModel();
            subCatalogViewModel.CategoryList = category;
            subCatalogViewModel.CategoryId = subcategory.CategoryId;
            subCatalogViewModel.Id = subcategory.Id;
            subCatalogViewModel.Name = subcategory.Name;

            return View("CreateSubCategory", subCatalogViewModel);
        }

        [ActionName("EditProduct")]
        public async Task<ActionResult> EditProductAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = await DocumentDbRepository<Product>.GetItemAsync(id, ProductId);

            if (product == null)
            {
                return HttpNotFound();
            }

            var subcategory = await DocumentDbRepository<SubCategory>.GetDropDownAsync("SubCategory");

            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.SubCategoryList = subcategory;
            productViewModel.Id = product.Id;
            productViewModel.Name = product.Name;
            productViewModel.SubCategoryId = product.SubCategoryId;

            return View("CreateProduct", productViewModel);
        }

        [ActionName("DeleteCategory")]
        public async Task<ActionResult> DeleteCategoryAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category deletecategory = await DocumentDbRepository<Category>.GetItemAsync(id, CategoryId);

            if (deletecategory == null)
            {
                return HttpNotFound();
            }

            return View(deletecategory);
        }

        [HttpPost]
        [ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCategoryConfirmedAsync([Bind(Include = "Id")] Guid id)
        {
            await DocumentDbRepository<Category>.DeleteItemAsync(id, CategoryId);

            return RedirectToAction("CatalogIndex");
        }

        [ActionName("DeleteSubCategory")]
        public async Task<ActionResult> DeleteSubCategoryAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SubCategory deleteSubCategory = await DocumentDbRepository<SubCategory>.GetItemAsync(id, SubCategoryId);

            if (deleteSubCategory == null)
            {
                return HttpNotFound();
            }

            return View(deleteSubCategory);
        }

        [HttpPost]
        [ActionName("DeleteSubCategory")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteSubCategoryConfirmedAsync([Bind(Include = "Id")] Guid id)
        {
            await DocumentDbRepository<SubCategory>.DeleteItemAsync(id, SubCategoryId);

            return RedirectToAction("CatalogIndex");
        }

        [ActionName("DeleteProduct")]
        public async Task<ActionResult> DeleteProductAsync(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product deleteProduct = await DocumentDbRepository<Product>.GetItemAsync(id, ProductId);

            if (deleteProduct == null)
            {
                return HttpNotFound();
            }

            return View(deleteProduct);
        }

        [HttpPost]
        [ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProductConfirmedAsync([Bind(Include = "Id")] Guid id)
        {
            await DocumentDbRepository<Product>.DeleteItemAsync(id, ProductId);

            return RedirectToAction("CatalogIndex");
        }

        //[ActionName("Details")]
        //public async Task<ActionResult> DetailsAsync(Guid id)
        //{
        //    Course course = await DocumentDbRepository<Course>.GetItemAsync(id);

        //    return View(course);
        //}

        [ActionName("Category")]
        public async Task<ActionResult> CategoryDetails(Guid id)
        {
            Category cat = await DocumentDbRepository<Category>.GetItemAsync(id);

            return View(cat);
        }
    }
}