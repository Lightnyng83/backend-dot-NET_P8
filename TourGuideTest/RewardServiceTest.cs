using System.Diagnostics;
using GpsUtil.Location;
using TourGuide.Users;
using Xunit;
using Xunit.Abstractions;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;
    private readonly ITestOutputHelper _output;
    public RewardServiceTest(DependencyFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attractionList = await _fixture.GpsUtil.GetAttractions();
        var attraction = attractionList.First();
        await user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        await _fixture.TourGuideService.TrackUserLocation(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    [Fact]
    public async Task IsWithinAttractionProximity()
    {
        var attractionList = await _fixture.GpsUtil.GetAttractions();
        var attraction = attractionList.First();
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

    [Fact]
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        var user = _fixture.TourGuideService.GetAllUsers().First();
        await _fixture.RewardsService.CalculateRewards(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);
        _fixture.TourGuideService.Tracker.StopTracking();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var attractionCount = await _fixture.GpsUtil.GetAttractions();
        stopwatch.Stop();
        _output.WriteLine($"{stopwatch.Elapsed} sec for GetAttraction ");
        Assert.Equal(attractionCount.Count, userRewards.Count);
    }

}
