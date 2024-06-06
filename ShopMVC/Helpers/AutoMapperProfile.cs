using AutoMapper;
using ShopMVC.Data2;
using ShopMVC.Data2;
using ShopMVC.ViewModels;

namespace ShopMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<RegisterViewModel, Customer>();
            //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
            //.ReverseMap();
            CreateMap<RegisterViewModel, Customer>()
            .ReverseMap(); // Thêm reverse mapping
            CreateMap<CustomerProfileVM, Customer>()
            .ReverseMap();
        }   
    }
}
