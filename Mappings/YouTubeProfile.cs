using AutoMapper;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;

namespace Youtube_Entertainment_Project.Mappings
{
    public class YouTubeProfile : Profile
    {
        public YouTubeProfile()
        {
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UserDto, AppUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Channel, ChannelDto>()
                .ForMember(dest => dest.ChannelId, opt => opt.MapFrom(src => src.ChannelId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ChannelName ?? src.Name)) 
                .ForMember(dest => dest.OwnerUserId, opt => opt.MapFrom(src => src.OwnerUserId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

                .ForMember(dest => dest.OwnerProfileImagePath, opt => opt.MapFrom(src =>
                    (src.Owner != null && !string.IsNullOrEmpty(src.Owner.ProfileImagePath))
                    ? src.Owner.ProfileImagePath
                    : "/images/default-profile.png"))
                .ForMember(dest => dest.Subscribers, opt => opt.MapFrom(src => src.Subscribers))
                .ForMember(dest => dest.Videos, opt => opt.MapFrom(src => src.Videos));

            CreateMap<ChannelDto, Channel>()
                .ForMember(dest => dest.ChannelId, opt => opt.MapFrom(src => src.ChannelId))
                .ForMember(dest => dest.ChannelName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.OwnerUserId, opt => opt.MapFrom(src => src.OwnerUserId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Videos, opt => opt.Ignore())
                .ForMember(dest => dest.Subscribers, opt => opt.Ignore());



            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.UserName : "Unknown User"))
                .ForMember(dest => dest.Replies, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());


            // VIDEO → VIDEODTO
            CreateMap<Video, VideoDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.UploadTime))
                .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility ?? "public"))
                .ForMember(dest => dest.ChannelName, opt => opt.MapFrom(src => src.Channel != null ? src.Channel.ChannelName : null))
                .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src =>
                    src.VideoTags != null ? src.VideoTags.Select(vt => vt.TagId).ToList() : new List<Guid>()))
                .ForMember(dest => dest.ChannelOwnerProfileImage, opt => opt.MapFrom(src =>
                    src.Channel != null && src.Channel.Owner != null
                        ? src.Channel.Owner.ProfileImagePath
                            : "/images/default-profile.png"))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    src.Thumbnails != null && src.Thumbnails.Any(t => t.IsDefault)
                    ? src.Thumbnails.First(t => t.IsDefault).Url
                    : "/images/default-thumbnail.jpg"))
                .ReverseMap()
                .ForMember(dest => dest.Visibility, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Visibility)))
                .ForMember(dest => dest.VideoTags, opt => opt.Ignore())
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.Thumbnails, opt => opt.Ignore())
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            // CREATEVIDEO → VIDEO
            CreateMap<CreateVideoDto, Video>()
                .ForMember(dest => dest.VideoId, opt => opt.Ignore())
                .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility ?? "public"))
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.VideoTags, opt => opt.Ignore())
                .ForMember(dest => dest.Thumbnails, opt => opt.Ignore())
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath));

            // UPDATEVIDEO → VIDEO
            CreateMap<UpdateVideoDto, Video>()
                .ForMember(dest => dest.VideoId, opt => opt.Ignore())
                .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.Visibility ?? "public"))
                .ForMember(dest => dest.ChannelId, opt => opt.Ignore())
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.FilePath, opt => opt.Ignore())
                .ForMember(dest => dest.UploadTime, opt => opt.Ignore())
                .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
                .ForMember(dest => dest.LikeCount, opt => opt.Ignore())
                .ForMember(dest => dest.DislikeCount, opt => opt.Ignore())
                .ForMember(dest => dest.CommentCount, opt => opt.Ignore())
                .ForMember(dest => dest.VideoTags, opt => opt.Ignore())
                .ForMember(dest => dest.Thumbnails, opt => opt.Ignore())
                .ForMember(dest => dest.Likes, opt => opt.Ignore());

            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.Videos, opt => opt.MapFrom(src => src.Videos))
                .ReverseMap()
                .ForMember(dest => dest.Videos, opt => opt.Ignore());


            CreateMap<PlaylistVideo, PlaylistVideoDto>()
                .ForMember(dest => dest.VideoId, opt => opt.MapFrom(src => src.VideoId))
                .ForMember(dest => dest.VideoTitle, opt => opt.MapFrom(src => src.Video != null ? src.Video.Title : "Unknown"))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.Video != null ? src.Video.FilePath : ""))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    (src.Video != null && src.Video.Thumbnails != null && src.Video.Thumbnails.Any())
                        ? (src.Video.Thumbnails.FirstOrDefault(t => t.IsDefault) ?? src.Video.Thumbnails.First()).Url
                            : "/images/default-thumbnail.jpg"));

            CreateMap<Thumbnail, ThumbnailDto>().ReverseMap();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.UserName : "Anonymous"))
                .ForMember(dest => dest.UserProfilePictureUrl, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.ProfileImagePath : null)) 
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

            CreateMap<CommentDto, Comment>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Video, opt => opt.Ignore());

            CreateMap<Subscription, SubscriptionDto>()
                .ForMember(dest => dest.ChannelName, opt => opt.Ignore())
                .ForMember(dest => dest.ProfilePicture, opt => opt.Ignore())
                .ReverseMap() 
                .ForMember(dest => dest.Subscriber, opt => opt.Ignore()); 

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<Notification, NotificationDto>().ReverseMap();
        }
    }
}
