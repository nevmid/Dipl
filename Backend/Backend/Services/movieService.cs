using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Backend.Interfaces;
using Backend.Models.DTOs.MovieDTOs;
using Backend.Models.Entities;

namespace Backend.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<Movie> CreateMovie(CreateMovieDto createMovieDto)
        {
            try
            {
                if (string.IsNullOrEmpty(createMovieDto.Title))
                {
                    throw new ValidationException("Название обязательно");
                }
                if (string.IsNullOrEmpty(createMovieDto.OriginalTitle))
                {
                    throw new ValidationException("Оригинальное название обязательно");
                }
                if (string.IsNullOrEmpty(createMovieDto.Description))
                {
                    throw new ValidationException("Описание обязательно");
                }

                if (string.IsNullOrEmpty(createMovieDto.TrailerUrl))
                {
                    throw new ValidationException("Трейлер обязателен");
                }

                if (!Uri.TryCreate(createMovieDto.TrailerUrl, UriKind.Absolute, out var uri))
                {
                    throw new ValidationException("Некорректный URL");
                }

                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    throw new ValidationException("URL должен использовать HTTP или HTTPS");

                if (createMovieDto.Year < 1888 || createMovieDto.Year > DateTime.Now.Year + 2)
                    throw new ValidationException($"Год должен быть между 1888 и {DateTime.Now.Year + 2}");

                if (createMovieDto.Rating < 0 || createMovieDto.Rating > 10)
                    throw new ValidationException("Рейтинг должен быть от 0 до 10");

                bool hasUrl = !string.IsNullOrEmpty(createMovieDto.PosterUrl);
                bool hasFile = createMovieDto.PosterFile != null
                    && createMovieDto.PosterFile.Length > 0;

                if(!hasUrl  && !hasFile)
                {
                    throw new ValidationException("Необходимо указать url или файл");
                }

                if (hasUrl && hasFile)
                {
                    throw new ValidationException("Необходимо указать либо url либо файл");
                }

                string savedFilePath = null;

                if (hasUrl)
                {
                    savedFilePath = await DownloadImageFromUrlAsync(createMovieDto.PosterUrl!);
                }
                else if (hasFile)
                {
                    savedFilePath = await SaveUploadedFileAsync(createMovieDto.PosterFile!);
                }

                var movie = new Movie
                {
                    Title = createMovieDto.Title.Trim().ToLower(),
                    OriginalTitle = createMovieDto.OriginalTitle.Trim().ToLower(),
                    Description = createMovieDto.Description,
                    Year = createMovieDto.Year,
                    Duration = createMovieDto.Duration,
                    PosterUrl = savedFilePath,
                    TrailerUrl = createMovieDto.TrailerUrl,
                    Rating = createMovieDto.Rating
                };

                await _movieRepository.CreateMovie(movie);

                return movie;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteMovie(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieById(id);

                if (movie == null)
                    return false;

                await _movieRepository.DeleteMovie(movie);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Movie?> GetInfoAboutMovie(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieById(id);

                if (movie == null)
                    return null;

                return movie;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Movie>> GetMovies()
        {
            try
            {
                var movies = await _movieRepository.GetAllMovies();

                return movies;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Movie?> UpdateMovie(int id, UpdateMovieDto updateMovieDto)
        {
            try
            {
                var movie = await _movieRepository.GetMovieById(id);

                if (movie == null)
                    return null;

                bool hasChanges = false;

                if (!string.IsNullOrEmpty(updateMovieDto.Title)
                    && movie.Title != updateMovieDto.Title.Trim().ToLower())
                {
                    movie.Title = updateMovieDto.Title.Trim().ToLower();
                    hasChanges = true;
                }
                if (!string.IsNullOrEmpty(updateMovieDto.OriginalTitle)
                    && movie.OriginalTitle != updateMovieDto.OriginalTitle.Trim().ToLower())
                {
                    movie.OriginalTitle = updateMovieDto.OriginalTitle.Trim().ToLower();
                    hasChanges = true;
                }
                if (!string.IsNullOrEmpty(updateMovieDto.Description)
                    && movie.Description != updateMovieDto.Description)
                {
                    movie.Description = updateMovieDto.Description;
                    hasChanges = true;
                }
                if (updateMovieDto.Year.HasValue
                    && movie.Year != updateMovieDto.Year.Value)
                {
                    movie.Year = updateMovieDto.Year.Value;
                    hasChanges = true;
                }
                if (updateMovieDto.Duration.HasValue
                    && movie.Duration != updateMovieDto.Duration.Value)
                {
                    movie.Duration = updateMovieDto.Duration.Value;
                    hasChanges = true;
                }
                if (updateMovieDto.Rating.HasValue
                    && movie.Rating != updateMovieDto.Rating.Value)
                {
                    movie.Rating = updateMovieDto.Rating.Value;
                    hasChanges = true;
                }
                if(!string.IsNullOrEmpty(updateMovieDto.TrailerUrl) 
                    && movie.TrailerUrl != updateMovieDto.TrailerUrl)
                {
                    movie.TrailerUrl = updateMovieDto.TrailerUrl;
                    hasChanges = true;
                }

                bool hasUrl = !string.IsNullOrEmpty(updateMovieDto.PosterUrl);
                bool hasFile = updateMovieDto.PosterFile != null
                    && updateMovieDto.PosterFile.Length > 0;

                DeleteImage(movie.PosterUrl);

                if (!hasUrl && !hasFile)
                {
                    throw new ValidationException("Необходимо указать url или файл");
                }

                if (hasUrl && hasFile)
                {
                    throw new ValidationException("Необходимо укаать либо url либо файл");
                }

                string savedFilePath = null;

                if (hasUrl)
                {
                    savedFilePath = await DownloadImageFromUrlAsync(updateMovieDto.PosterUrl!);
                    movie.PosterUrl = savedFilePath;
                    hasChanges = true;
                }
                else if (hasFile)
                {
                    savedFilePath = await SaveUploadedFileAsync(updateMovieDto.PosterFile!);
                    movie.PosterUrl = savedFilePath;
                    hasChanges = true;
                }

                if (hasChanges)
                    await _movieRepository.SaveChangesAsync();

                return movie;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> DownloadImageFromUrlAsync(string url)
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            try
            {

                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    throw new ValidationException("Некорректный URL");
                }

                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    throw new ValidationException("URL должен использовать HTTP или HTTPS");

                using var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new ValidationException($"Не удалось загрузить изображение. Код: {response.StatusCode}");

                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (!contentType?.StartsWith("image/") ?? true)
                    throw new ValidationException("URL должен вести на изображение");

                var fileExtension = Path.GetExtension(uri.LocalPath).ToLower();

                if (string.IsNullOrEmpty(fileExtension))
                    fileExtension = ".jpg";

                var fileName = $"poster{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{fileExtension}";

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posters");

                var filePath = Path.Combine(folder, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                if (imageBytes.Length > 10 * 1024 * 1024)
                    throw new ValidationException("Изображение слишком большое");

                await File.WriteAllBytesAsync(filePath, imageBytes);

                var fileUrl = $"/uploads/posters/{fileName}";

                return fileUrl;
            }
            catch (HttpRequestException ex)
            {
                throw new ValidationException($"Ошибка при загрузке изображения: {ex.Message}");
            }
            catch (Exception)
            {
                throw;
            } 
        }

        private async Task<string> SaveUploadedFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ValidationException("Файл не выбран");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ValidationException("Недопустимый формат файла");
                }

                if (file.Length > 10 * 1024 * 1024)
                    throw new ValidationException("Файл слишком большой");

                var fileName = $"poster{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{fileExtension}";

                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posters");

                var filePath = Path.Combine(folder, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fileUrl = $"/uploads/posters/{fileName}";

                return fileUrl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void DeleteImage(string image_url)
        {
            try
            {
                if (!string.IsNullOrEmpty(image_url) &&
                    image_url.StartsWith("/uploads/posters/"))
                {
                    var fileName = Path.GetFileName(image_url);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                        "wwwroot", "uploads", "posters", fileName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
