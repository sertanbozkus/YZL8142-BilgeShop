using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Business.Types;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Manager
{
    public class ProductManager : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly ICategoryService _categoryService;

        public ProductManager(IRepository<ProductEntity> productRepository, ICategoryService categoryService)
        {
            _productRepository = productRepository;
            _categoryService = categoryService;
        }

        public ServiceMessage AddProduct(AddProductDto addProductDto)
        {
            var hasProduct = _productRepository.GetAll(x => x.Name.ToLower() == addProductDto.Name.ToLower()).ToList();

            if (hasProduct.Any())
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu isimde bir ürün zaten mevcut."
                };
            }

            var productEntity = new ProductEntity()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                UnitInStock = addProductDto.UnitInStock,
                UnitPrice = addProductDto.UnitPrice,
                CategoryId = addProductDto.CategoryId,
                ImagePath = addProductDto.ImagePath
            };

            _productRepository.Add(productEntity);

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }

        public EditProductDto GetProductById(int id)
        {
            var productEntity = _productRepository.GetById(id);

            var editProductDto = new EditProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitInStock = productEntity.UnitInStock,
                UnitPrice = productEntity.UnitPrice,
                CategoryId = productEntity.CategoryId
            };

            return editProductDto;
        }

        public DetailProductDto GetProductDetail(int id)
        {
            //var productEntites = _productRepository.GetAll(x => x.Id == id);

            //var dtos = productEntites.Select(x => new DetailProductDto
            //{
            //    Name=x.Name,
            //    CategoryName = x.Category.Name
            //}).ToList();

            //return dtos[0];

            // Bu şekilde bir kullanımla , X.Category.Name ulaşabilirdik, çünkü ToList() diyene kadar bir sorgudayım (query) ve EntityFramework otomatik olarak tablo bağlantılarını yapıyor.





            var productEntity = _productRepository.GetById(id);

            var productDto = new DetailProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitPrice = productEntity.UnitPrice,
                ImagePath = productEntity.ImagePath,
                CategoryId = productEntity.CategoryId,
                UnitInStock = productEntity.UnitInStock
            };

            productDto.CategoryName = _categoryService.GetCategoryName(productDto.CategoryId);

            return productDto;


        }

        public List<ProductDto> GetProducts()
        {
            var productEntites = _productRepository.GetAll().OrderBy(x => x.Category.Name).ThenBy(x => x.Name); // Önce kategori adına, sonra ürün adına göre sıralıyorum.



            var productDtoList = productEntites.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UnitInStock = x.UnitInStock,
                UnitPrice = x.UnitPrice,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                ImagePath = x.ImagePath
            }).ToList();

            return productDtoList;
        }

        public List<ProductDto> GetProductsByCategoryId(int? categoryId = null)
        {

            if (categoryId.HasValue) // is not null -- != null
            {
                var productEntites = _productRepository.GetAll(x => x.CategoryId == categoryId).OrderBy(x => x.Name);
                // gönderdiğim id değeri ile, categoryId verisi eşleşen bütün ürünleri isimlerine göre sıralayarak getir.


                var productDtos = productEntites.Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitInStock = x.UnitInStock,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    ImagePath = x.ImagePath
                }).ToList();

                return productDtos;

            }
            else
            {
                return GetProducts();
            }
        }

        public void UpdateProduct(EditProductDto editProductDto)
        {
            var productEntity = _productRepository.GetById(editProductDto.Id);

            productEntity.Name = editProductDto.Name;
            productEntity.Description = editProductDto.Description;
            productEntity.UnitInStock = editProductDto.UnitInStock;
            productEntity.UnitPrice = editProductDto.UnitPrice;
            productEntity.CategoryId = editProductDto.CategoryId;

            if (editProductDto.ImagePath is not null)
                productEntity.ImagePath = editProductDto.ImagePath;

            _productRepository.Update(productEntity);

        }
    }
}




//int Topla(int a , int b = 10)
//{
//    return a + b;
//}


//Topla(3,5) -> 8

//Topla(3) -> 13