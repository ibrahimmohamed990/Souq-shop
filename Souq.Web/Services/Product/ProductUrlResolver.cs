using AutoMapper;
using Souq.Entities.ViewModels;

namespace Souq.Web.Services.Product
{
    public class ProductUrlResolver : IValueResolver<Souq.Entities.Models.Product, Souq.Entities.Models.Product, string>
    {
        private readonly IConfiguration configuration;
        public ProductUrlResolver(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public string Resolve(Souq.Entities.Models.Product source, Souq.Entities.Models.Product destination, string destMember, ResolutionContext context)
        {
            if (source.ImageUrl != null && !string.IsNullOrEmpty(source.ImageUrl))
                return $"{configuration["BaseUrl"]}{source.ImageUrl}";
            return null;
        }
    }
}
