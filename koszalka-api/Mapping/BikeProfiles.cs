﻿using AutoMapper;
using koszalka_api.Persistence.DTO;
using koszalka_api.Persistence.Model;

namespace koszalka_api.Profiles
{
    public class BikeProfile : Profile
    {
        public BikeProfile()
        {
            CreateMap<Bike, BikeDTO>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(x => x.Brand))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(x => x.Description))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(x => x.Model));

            CreateMap<BikeDTO, Bike>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(x => x.Brand))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(x => x.Description))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom( x => DateTime.Now))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(x => DateTime.Now))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(x => x.Model));
        }
    }
}
