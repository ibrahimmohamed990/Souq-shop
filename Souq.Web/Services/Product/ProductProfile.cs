using AutoMapper;
using Souq.Entities.ViewModels;

namespace Souq.Web.Services.Product
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Souq.Entities.Models.Product, Souq.Entities.Models.Product>()
            .ForMember(dest => dest.ImageUrl, options => options.MapFrom<ProductUrlResolver>())
            .ReverseMap();

            //CreateMap<ProductVM, Souq.Entities.Models.Product>()
            //.ForMember(dest => dest.ImageUrl, options => options.MapFrom<ProductUrlResolver, string>(src => src.Product.ImageUrl))
            //.ReverseMap();

        }

    }
}
