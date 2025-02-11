using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Users;

namespace TourGuide.LibrairiesWrappers
{
    public class RewardCentralWrapper : IRewardCentral
    {
        private readonly RewardCentral.RewardCentral _rewardCentral;

        public RewardCentralWrapper()
        {
            _rewardCentral = new();
        }

        public int GetAttractionRewardPoints(Guid attractionId, Guid userId)
        {
            return _rewardCentral.GetAttractionRewardPoints(attractionId, userId);
        }

        public int GetRewardPoints(Attraction attraction, User user)
        {
            // Assuming the correct method is GetAttractionRewardPoints
            return _rewardCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
        }
    }
}
