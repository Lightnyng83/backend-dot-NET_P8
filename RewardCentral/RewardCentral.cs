﻿using GpsUtil.Location;


namespace RewardCentral;

public class RewardCentral
{
    public int GetAttractionRewardPoints(Guid attractionId, Guid userId)
    {
        //int randomDelay = new Random().Next(1, 1000);
        //Thread.Sleep(randomDelay);

        int randomInt = new Random().Next(1, 1000);
        return randomInt;
    }

    public int GetAttractionRewardPoints(Attraction attractionId, int userId)
    {
        //int randomDelay = new Random().Next(1, 1000);
        //Thread.Sleep(randomDelay);

        int randomInt = new Random().Next(1, 1000);
        return randomInt;
    }
}
