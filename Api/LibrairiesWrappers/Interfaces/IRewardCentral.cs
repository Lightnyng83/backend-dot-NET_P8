using GpsUtil.Location;
using TourGuide.Users;

namespace TourGuide.LibrairiesWrappers.Interfaces
{
    public interface IRewardCentral
    {
        int GetAttractionRewardPoints(Guid attractionId, Guid userId);
        int GetRewardPoints(Attraction attraction, User user);
    }
}
