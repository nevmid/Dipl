using System.ComponentModel.DataAnnotations;
using Backend.Interfaces;
using Backend.Models.DTOs.HallDTOs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;
        private readonly ISeatRepository _seatRepository;

        public HallService(
            IHallRepository hallRepository,
            ISeatRepository seatRepository)
        {
            _hallRepository = hallRepository;
            _seatRepository = seatRepository;
        }
        public async Task<Hall> CreateHall(RequestHallDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    throw new ValidationException("Название зала обязательно");

                var hallExist = await _hallRepository.HallIsExist(dto.Name.Trim().ToLower());

                if (hallExist)
                    throw new ValidationException("Зал с таким названием уже существует");

                var hall = new Hall
                {
                    Name = dto.Name.Trim().ToLower(),
                    Description = dto.Description ?? string.Empty
                };

                var createdHall = await _hallRepository.CreateHall(hall);

                for(int i = 1; i <= dto.RowNum; i++)
                {
                    for (int j = 1; j <= dto.ColNum; j++)
                    {
                        var seat = new Seat
                        {
                            HallId = createdHall.Id,
                            RowNum = i,
                            ColNum = j,
                            Type = "standard"
                        };

                        await _seatRepository.CreateSeat(seat);
                    }
                }

                await _hallRepository.SaveChangesAsync();

                return createdHall;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteHall(int id)
        {
            try
            {
                var hall = await _hallRepository.GetHallById(id);

                if (hall == null)
                    return false;

                _hallRepository.DeleteHall(hall);
                await _hallRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Hall?> GetHallById(int id)
        {
            try
            {
                var hall =  await _hallRepository.GetHallById(id);

                return hall; 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Hall>> GetHalls()
        {
            try
            {
                var halls = await _hallRepository.GetHalls();

                return halls;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Hall?> UpdateHall(int id, RequestHallDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                    throw new ValidationException("Название зала обязательно");



                var hallExist = await _hallRepository.HallIsExist(dto.Name.Trim().ToLower());

                if (hallExist)
                    throw new ValidationException("Зал с таким названием уже существует");

                var hall = await _hallRepository.GetHallById(id);

                if(hall == null)
                    return null;

                hall.Name = dto.Name;
                hall.Description = dto.Description ?? string.Empty;

                await _seatRepository.DeleteSeats(id);

                for (int i = 1; i <= dto.RowNum; i++)
                {
                    for (int j = 1; j <= dto.ColNum; j++)
                    {
                        var seat = new Seat
                        {
                            HallId = hall.Id,
                            RowNum = i,
                            ColNum = j,
                            Type = "standard"
                        };

                        await _seatRepository.CreateSeat(seat);
                    }
                }

                await _hallRepository.SaveChangesAsync();

                return hall;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
