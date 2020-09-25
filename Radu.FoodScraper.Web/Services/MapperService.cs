using Radu.FoodScraper.Models;
using Radu.FoodScraper.Scrapers.Dto;

namespace Radu.FoodScraper.Web.Services
{
    public class MapperService
    {
        protected AutoMapper.IMapper AutoMapper
        {
            get;
            private set;
        }

        public MapperService()
        {
            var config = new AutoMapper.MapperConfiguration(GetMapperConfiguration());
            AutoMapper = config.CreateMapper();
        }

        protected virtual AutoMapper.Configuration.MapperConfigurationExpression GetMapperConfiguration()
        {
            var cfg = new AutoMapper.Configuration.MapperConfigurationExpression();
            cfg.CreateMap<DishDto, Dish>();
            return cfg;
        }

        public T Map<T>(object source)
        {
            return AutoMapper.Map<T>(source);
        }
    }
}
