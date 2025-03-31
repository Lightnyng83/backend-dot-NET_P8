using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{

    //Ajout de _rewardsService et _gpsUtil dans le constructeur du controller
    private readonly ITourGuideService _tourGuideService;
    private readonly IRewardsService _rewardsService;
    private readonly IGpsUtil _gpsUtil;
    public TourGuideController(ITourGuideService tourGuideService, IGpsUtil gpsUtil, IRewardsService rewardsService)
    {
        _tourGuideService = tourGuideService;
        _gpsUtil = gpsUtil;
        _rewardsService = rewardsService;
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    // TODO: Change this method to no longer return a List of Attractions.
    // Instead: Get the closest five tourist attractions to the user - no matter how far away they are.
    // Return a new JSON object that contains:
    // Name of Tourist attraction, 
    // Tourist attractions lat/long, 
    // The user's location lat/long, 
    // The distance in miles between the user's location and each of the attractions.
    // The reward points for visiting each Attraction.
    //    Note: Attraction reward points can be gathered from RewardsCentral
    [HttpGet("getNearbyAttractions")]
    public async Task<ActionResult<List<NearbyAttractionDto>>> GetNearbyAttractions([FromQuery] string userName)
    {
        // Récupère l'utilisateur via une méthode existante (GetUser)
        var user = GetUser(userName);
        // Récupère la dernière localisation connue de l'utilisateur
        var visitedLocation = await _tourGuideService.GetUserLocation(user);

        var allAttractions = _gpsUtil.GetAttractions();

        // Pour chaque attraction, on calcule la distance entre la localisation de l'utilisateur et l'attraction.
        // Puis on trie par distance croissante et on prend les cinq premières.
        var closestAttractions = await _tourGuideService.GetNearByAttractions(visitedLocation);

        // Pour chaque attraction sélectionnée, on crée un DTO incluant :
        // - le nom et les coordonnées de l'attraction
        // - les coordonnées de l'utilisateur
        // - la distance calculée entre la localisation de l'utilisateur et l'attraction
        // - les points de récompense pour cette attraction et cet utilisateur
        var result = closestAttractions.Select(attraction => new NearbyAttractionDto
        {
            AttractionName = attraction.AttractionName,
            AttractionLatitude = attraction.Latitude,
            AttractionLongitude = attraction.Longitude,
            UserLatitude = visitedLocation.Location.Latitude,
            UserLongitude = visitedLocation.Location.Longitude,
            Distance = _rewardsService.GetDistance(visitedLocation.Location, attraction),
            RewardPoints = _rewardsService.GetRewardPoints(attraction, user)
        }).ToList();

        return Ok(result);
    }


    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User? GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
