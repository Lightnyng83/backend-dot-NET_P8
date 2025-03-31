using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers;

public class GpsUtilWrapper : IGpsUtil
{
    private readonly GpsUtil.GpsUtil _gpsUtil;
    private List<Attraction> _allAttractionsCache = null;
    private DateTime _allAttractionsCacheLastUpdated = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(20);

    public GpsUtilWrapper()
    {
        _gpsUtil = new();
    }

    public VisitedLocation GetUserLocation(Guid userId)
    {
        return _gpsUtil.GetUserLocation(userId);
    }

    public async Task<List<Attraction>> GetAttractions()
    {
        if (_allAttractionsCache == null || DateTime.UtcNow - _allAttractionsCacheLastUpdated > _cacheDuration)
        {
            _allAttractionsCache = await _gpsUtil.GetAttractions();
            _allAttractionsCacheLastUpdated = DateTime.UtcNow;
        }
        return _allAttractionsCache;
    }



}
