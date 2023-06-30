using AutoMapper;
using FFMpegUI.Models;

namespace FFMpegUI.Persistence.Mapping
{
    /// <summary>
    /// For internal models
    /// </summary>
    public class FFMpegMapperProfile : Profile
    {
        public FFMpegMapperProfile()
        {
            CreateMap<FFMpegProcessItem, FFMpegProcessItemStatusNotification>()
                .ForMember(dest => dest.ProcessItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProgressMessage, opt => opt.MapFrom(src => src.StatusMessage));

            CreateMap<FFMpegUpdateProcessCommand, FFMpegProcessStatusNotification>()
                .ForMember(dest => dest.AllFilesTotalSize, opt => opt.Ignore())
                .ForMember(dest => dest.ProgressMessage, opt => opt.MapFrom(src => src.StatusMessage));
        }
    }
}
