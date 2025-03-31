using GpsUtil.Location;
using System.Collections.Concurrent;
using TripPricer;

namespace TourGuide.Users
{
    public class User
    {
        public Guid UserId { get; }
        public string UserName { get; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public DateTime LatestLocationTimestamp { get; set; }

        // Utilisation de ConcurrentBag pour stocker les visites et les récompenses
        private readonly ConcurrentBag<VisitedLocation> _visitedLocations = new ConcurrentBag<VisitedLocation>();
        private readonly ConcurrentBag<UserReward> _userRewards = new ConcurrentBag<UserReward>();

        public UserPreferences UserPreferences { get; set; } = new UserPreferences();
        public List<Provider> TripDeals { get; set; } = new List<Provider>();

        public User(Guid userId, string userName, string phoneNumber, string emailAddress)
        {
            UserId = userId;
            UserName = userName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }

        // Ajoute une visite de manière thread-safe
        public Task AddToVisitedLocations(VisitedLocation visitedLocation)
        {
            _visitedLocations.Add(visitedLocation);
            return Task.CompletedTask;
        }

        // Retourne une copie de la collection des visites
        public List<VisitedLocation> GetVisitedLocationsSnapshot()
        {
            return _visitedLocations.ToList();
        }

        // Vide la collection des visites en retirant tous les éléments
        public void ClearVisitedLocations()
        {
            while (!_visitedLocations.IsEmpty)
            {
                _visitedLocations.TryTake(out _);
            }
        }

        // Ajoute une récompense si elle n'existe pas déjà (vérification sur le nom de l'attraction)
        public void AddUserReward(UserReward userReward)
        {
            if (!_userRewards.Any(r => r.Attraction.AttractionName == userReward.Attraction.AttractionName))
            {
                _userRewards.Add(userReward);
            }
        }

        // Retourne la dernière visite en se basant sur le temps de visite.
        // Remarque : ConcurrentBag n'impose pas d'ordre, il est donc nécessaire de trier par TimeVisited.
        public VisitedLocation GetLastVisitedLocation()
        {
            return _visitedLocations.OrderBy(v => v.TimeVisited).LastOrDefault();
        }

        // Retourne une copie de la collection des récompenses
        public List<UserReward> GetUserRewardsSnapshot()
        {
            return _userRewards.ToList();
        }

        public List<UserReward> UserRewards => GetUserRewardsSnapshot();

        public List<VisitedLocation> VisitedLocations => GetVisitedLocationsSnapshot();
    }
}
