﻿using koszalka_api.DTO;
using koszalka_api.Model;
using Microsoft.AspNetCore.Mvc;

namespace koszalka_api.Service
{
    public interface IShoesService
    {
        Task<ActionResult<IEnumerable<ShoesDTO>>> GetShoes();
        Task<ShoesDTO> GetShoe(long id);
        Task<bool> PutShoe(long id, Shoes shoes);
        Task<int> PostShoe(Shoes shoes);
        Task<int> DeleteShoe(long id);
    }
}