using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Service
{
    public class BaseService
    {
        public IMapper _mapper { get; set; }

        public readonly IServiceProvider _serviceProvider;
        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mapper = serviceProvider.GetService<IMapper>();

        }
    }
}
