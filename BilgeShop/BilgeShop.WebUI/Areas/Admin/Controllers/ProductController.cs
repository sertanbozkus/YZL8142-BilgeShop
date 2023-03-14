using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment; // wwwroot yolunu yakalamak için kullanılan metotları içerecek.
        public ProductController(ICategoryService categoryService, IProductService productService, IWebHostEnvironment environment)
        {
            _categoryService = categoryService;
            _productService = productService;
            _environment = environment;
        }
        public IActionResult List()
        {
          var productDtos = _productService.GetProducts();

            var viewModel = productDtos.Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CategoryName = x.CategoryName,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                ImagePath = x.ImagePath
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult New()
        {
           

            ViewBag.Categories = _categoryService.GetCategories();
            
            return View("Form" , new ProductFormViewModel());
        }

        [HttpPost]
        public IActionResult Save(ProductFormViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryService.GetCategories();
                return View("Form", formData);
            }

            var newFileName = "";

            if(formData.File != null)
            {
                var allowedFileContentTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" }; // izin vereceğim dosya tipleri.

                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" }; // izin vereceğim dosya uzantıları.

                var fileContentType = formData.File.ContentType; // Dosyanın içeriğini bul
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(formData.File.FileName); // Dosyanın uzantısı hariç ismini bul

                var fileExtension = Path.GetExtension(formData.File.FileName); // Dosyanın uzantısını bul

                if (!allowedFileContentTypes.Contains(fileContentType) || !allowedFileExtensions.Contains(fileExtension))
                { 
                    // Dosya uzantısı veya içeriği istediğim gibi değilse, ilgili form sayfasına geri gönderiyorum.

                    ViewBag.FileError = "Dosya formatı veya içeriği hatalı.";

                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);
                }

                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;


                var folderPath = Path.Combine("images", "products");
                // images/products

                var wwwRootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                // ...wwwroot/images/products

                var wwwRootFilePath = Path.Combine(wwwRootFolderPath, newFileName);

                // ...wwwroot/images/products/gora-31231wawdaw321.jpg

                Directory.CreateDirectory(wwwRootFolderPath); // images/products yoksa bile sen oluştur.

        using(var fileStream = new FileStream(wwwRootFilePath, FileMode.Create))
                {
                    formData.File.CopyTo(fileStream);
                } 
        // asıl dosya yüklemenin yapıldığı kısım.
         // using içerisinde new'lenen FileStream nesnesi , scope boyunca yaşar, scope bitiminde silinir.

            }


            if(formData.Id == 0) // Ekleme
            {

                var addProductDto = new AddProductDto()
                {
                    Name = formData.Name.Trim(),
                    Description = formData.Description,
                    UnitPrice = formData.UnitPrice,
                    UnitInStock = formData.UnitInStock,
                    CategoryId = formData.CategoryId,
                    ImagePath = newFileName
                };

               var response = _productService.AddProduct(addProductDto);

                if (response.IsSucceed)
                {
                    return RedirectToAction("List");
                    
                }
                else
                {
                    ViewBag.ErrorMessage = response.Message;
                    ViewBag.Categories = _categoryService.GetCategories();
                    return View("Form", formData);

                }

            }
            else // Güncelleme
            {

                var editProductDto = new EditProductDto()
                {
                    Id = formData.Id,
                    Name = formData.Name,
                    Description = formData.Description,
                    UnitInStock = formData.UnitInStock,
                    UnitPrice = formData.UnitPrice,
                    CategoryId = formData.CategoryId
                };

                if(formData.File != null) // is not null
                    editProductDto.ImagePath = newFileName;


                _productService.UpdateProduct(editProductDto);



            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var editProductDto = _productService.GetProductById(id);

            var viewModel = new ProductFormViewModel()
            {
                Id = editProductDto.Id,
                Name = editProductDto.Name,
                Description = editProductDto.Description,
                UnitInStock = editProductDto.UnitInStock,
                UnitPrice = editProductDto.UnitPrice,
                CategoryId = editProductDto.CategoryId
            };

            ViewBag.Categories = _categoryService.GetCategories();
            return View("Form", viewModel);
        }

        
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);

            return RedirectToAction("List");
        }
    }
}
