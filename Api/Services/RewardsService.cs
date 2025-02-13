using GpsUtil.Location;
using System.Collections.Concurrent;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral =rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    public async Task CalculateRewards(User user)
    {
        List<VisitedLocation> userLocations = user.VisitedLocations;
        List<Attraction> attractions = await _gpsUtil.GetAttractions();

        ConcurrentBag<UserReward> rewards = new ConcurrentBag<UserReward>();

        await Parallel.ForEachAsync(userLocations, async (visitedLocation, cancellationToken) =>
        {
            foreach (var attraction in attractions)
            {
                bool alreadyRewarded = rewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName)
                                       || user.UserRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName);

                if (!alreadyRewarded && await NearAttraction(visitedLocation, attraction))
                {
                    var reward = new UserReward(visitedLocation, attraction, GetRewardPoints(attraction, user));
                    rewards.Add(reward);
                }
            }
        });
        foreach (var reward in rewards)
        {
            user.AddUserReward(reward);
        }
    }

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return true;
    }

    private async Task<bool> NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        double distance = await GetDistance(attraction, visitedLocation.Location);
        return distance <= _proximityBuffer;
    }

    public int GetRewardPoints(Attraction attraction, User user)
    {
        try
        {
            return _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de récupération : {ex}" );
            return 0;
        }
    }


    public async Task<double> GetDistance(Locations loc1, Locations loc2)
    {
        const double R = 3958.8;

        // Conversion des latitudes et longitudes en radians
        double lat1 = loc1.Latitude * Math.PI / 180.0;
        double lat2 = loc2.Latitude * Math.PI / 180.0;
        double dLat = (loc2.Latitude - loc1.Latitude) * Math.PI / 180.0;
        double dLon = (loc1.Longitude - loc2.Longitude) * Math.PI / 180.0;

        // Formule de Haversine
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return await Task.FromResult(R * c);
    }

    
}
