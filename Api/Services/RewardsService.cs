﻿using GpsUtil.Location;
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
        _rewardsCentral = rewardCentral;
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

                if (!alreadyRewarded && NearAttraction(visitedLocation, attraction))
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

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        double distance =  GetDistance(attraction, visitedLocation.Location);
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
            Console.WriteLine($"Erreur de récupération : {ex}");
            return 0;
        }
    }


    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                 + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }


}
