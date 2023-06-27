using AutoMapper;
using FFMpegUI.Messages;
using FFMpegUI.Models;

namespace FFMpegUI.Infrastructure.Mapping
{
    public  class FFMpegMessagesMapperProfile : Profile
    {
        public FFMpegMessagesMapperProfile()
        {
            CreateMap<FFMpegProcessItemMessage, FFMpegUpdateProcessItemCommand>();
        }
    }
}
