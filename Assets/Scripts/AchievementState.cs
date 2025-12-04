using System;
using System.Collections.Generic;

public enum Achievement {
    PickupEarth,
    PickupFire,
    PickupWater,
    PickupAir,
    PickupLightning,
    
    UseEarthEarth,
    UseFireEarth,
    UseWaterEarth,
    UseAirEarth,
    UseLightningEarth,
    UseFireFire,
    UseWaterFire,
    UseAirFire,
    UseLightningFire,
    UseWaterWater,
    UseAirWater,
    UseLightningWater,
    UseAirAir,
    UseLightningAir,
    UseLightningLightning,

}

public class AchievementState {
    public static HashSet<Achievement> achievements = new HashSet<Achievement>();
    
    public static void GiveAchievement(Achievement achievement) {
        if (!achievements.Add(achievement)) {
            return;
        }

        AchievementPopup.instance.ShowAchievement("Achievement Get: " + TextForAchievement(achievement) + "!");
    }

    private static string TextForAchievement(Achievement achievement) {
        return achievement switch {
            Achievement.PickupEarth => "Pickup Earth",
            Achievement.PickupFire => "Pickup Fire",
            Achievement.PickupWater => "Pickup Water",
            Achievement.PickupAir => "Pickup Air",
            Achievement.PickupLightning => "Pickup Lightning",
            Achievement.UseEarthEarth => "Use Earth Earth",
            Achievement.UseFireEarth => "Use Fire Earth",
            Achievement.UseWaterEarth => "Use Water Earth",
            Achievement.UseAirEarth => "Use Air Earth",
            Achievement.UseLightningEarth => "Use Lightning Earth",
            Achievement.UseFireFire => "Use Fire Fire",
            Achievement.UseWaterFire => "Use Water Fire",
            Achievement.UseAirFire => "Use Air Fire",
            Achievement.UseLightningFire => "Use Lightning Fire",
            Achievement.UseWaterWater => "Use Water Water",
            Achievement.UseAirWater => "Use Air Water",
            Achievement.UseLightningWater => "Use Lightning Water",
            Achievement.UseAirAir => "Use Air Air",
            Achievement.UseLightningAir => "Use Lightning Air",
            Achievement.UseLightningLightning => "Use Lightning Lightning",
            _ => throw new ArgumentOutOfRangeException(nameof(achievement), achievement, null)
        };
    }
}
