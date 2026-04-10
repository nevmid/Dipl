using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.BarDTOs;
using Backend.Models.Entities;

namespace Backend.Services
{
    public class BarService : IBarService
    {
        private readonly IBarRepository _barRepository;

        public BarService(IBarRepository barRepository)
        {
            _barRepository = barRepository;
        }

        public async Task<BarItem> CreateBarItem(barItemDto dto)
        {
            try
            {
                if(string.IsNullOrEmpty(dto.Name))
                    throw new ValidationException("Название товара обязательно");

                if (await _barRepository.ItemIsExist(dto.Name))
                    throw new ValidationException("Товар с таким названием уже существует");

                var category = await _barRepository.GetCategoryById(dto.CategoryId);

                if (category == null)
                    throw new ValidationException($"Категория с id {dto.CategoryId} не найдена");

                var item = new BarItem
                {
                    Name = dto.Name,
                    CategoryId = category.Id,
                    Price = dto.Price,
                    IsAvailable = dto.IsAvailable
                };

                return await _barRepository.CreateItem(item);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarCategory> CreateCategory(string categoryName)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryName))
                    throw new ValidationException("Название категории обязательно");

                if(categoryName.Length > 100)
                    throw new ValidationException("Слишком длинное название категории");

                if (await _barRepository.CategoryIsExist(categoryName))
                    throw new ValidationException("Категория с таким названием уже существует");


                var category = new BarCategory
                {
                    Name = categoryName
                };

                return await _barRepository.CreateCategory(category);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteBarItem(int id)
        {
            try
            {
                var item = await _barRepository.GetItemById(id);

                if (item == null)
                    return false;

                _barRepository.DeleteItem(item);
                await _barRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var category = await _barRepository.GetCategoryById(id);

                if (category == null)
                    return false;

                _barRepository.DeleteCategory(category);
                await _barRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BarItem>> GetBarItems()
        {
            try
            {
                var items = await _barRepository.GetItems();

                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BarCategory>> GetGategories()
        {
            try
            {
                var categories = await _barRepository.GetCategories();

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarItem?> UpdateBarItem(int id, barItemDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    throw new ValidationException("Название товара обязательно");

                if (await _barRepository.ItemIsExist(dto.Name))
                    throw new ValidationException("Товар с таким названием уже существует");

                var category = await _barRepository.GetCategoryById(dto.CategoryId);

                if (category == null)
                    throw new ValidationException($"Категория с id {dto.CategoryId} не найдена");

                var item = await _barRepository.GetItemById(id);

                if (item == null)
                    return null;

                item.Name = dto.Name;
                item.Price = dto.Price;
                item.CategoryId = dto.CategoryId;
                item.IsAvailable = dto.IsAvailable;

                await _barRepository.SaveChangesAsync();

                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BarCategory?> UpdateCategory(int id, string categoryName)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryName))
                    throw new ValidationException("Название категории обязательно");

                if (categoryName.Length > 100)
                    throw new ValidationException("Слишком длинное название категории");

                if (await _barRepository.CategoryIsExist(categoryName))
                    throw new ValidationException("Категория с таким названием уже существует");

                var category = await _barRepository.GetCategoryById(id);

                if (category == null)
                    return null;

                category.Name = categoryName;

                await _barRepository.SaveChangesAsync();

                return category;                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
