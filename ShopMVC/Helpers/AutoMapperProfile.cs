using AutoMapper;
using ShopMVC.Data;
using ShopMVC.ViewModels;

namespace ShopMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<RegisterVM, KhachHang>();
            //.ForMember(kh => kh.HoTen, option => option.MapFrom(RegisterVM => RegisterVM.HoTen))
            //.ReverseMap();
            CreateMap<CustomerProfileVM, KhachHang>()
            .ReverseMap(); // Thêm reverse mapping
        }   
    }
}
