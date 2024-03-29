﻿using AutoMapper;
using FFMpegUI.Models;
using FFMpegUI.Persistence.Entities;

namespace FFMpegUI.Persistence.Mapping
{
    public class FFMpegPersistenceMapperProfile : Profile
    {
        public FFMpegPersistenceMapperProfile()
        {
            CreateMap<FFMpegProcessItem, FFMpegPersistedProcessItem>()
                .ReverseMap();

            CreateMap<FFMpegProcess, FFMpegPersistedProcess>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProcessId));

            CreateMap<FFMpegProcess, FFMpegPersistedProcessFeatures>();

            CreateMap<(FFMpegPersistedProcess, FFMpegPersistedProcessFeatures), FFMpegProcess>()
               .ConstructUsing((src, ctx) => new FFMpegProcess
               {
                   ProcessId = src.Item1.Id,
                   StartDate = src.Item1.StartDate,
                   EndDate = src.Item1.EndDate,
                   Items = ctx.Mapper.Map<IEnumerable<FFMpegProcessItem>>(src.Item1.Items),
                   OverallConversionQuality = src.Item2.OverallConversionQuality,
                   AudioCodec = src.Item2.AudioCodec,
                   VideoCodec = src.Item2.VideoCodec,
                   RescaleHorizontalWidth = src.Item2.RescaleHorizontalWidth,
                   ConvertedFilesTotalSize = src.Item1.ConvertedFilesTotalSize,
                   SourceFilesTotalSize = src.Item1.SourceFilesTotalSize
               });

            // used just for the status notification creation
            CreateMap<FFMpegPersistedProcess, FFMpegProcess>()
                .ForMember(dest => dest.ProcessId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.SourceFilesTotalSize, opt => opt.MapFrom(src => src.SourceFilesTotalSize))
                .ForMember(dest => dest.ConvertedFilesTotalSize, opt => opt.MapFrom(src => src.ConvertedFilesTotalSize))
                .ForMember(dest => dest.Successfull, opt => opt.MapFrom(src => src.Successfull))
                .ForMember(dest => dest.StatusMessage, opt => opt.MapFrom(src => src.StatusMessage));
        }
    }
}
