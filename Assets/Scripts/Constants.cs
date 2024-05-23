using UnityEngine;

public static class Constants {

    // Weapons

    public static WeaponData BulletCannon = new WeaponData("Bullet Cannon", 5, 2000, 1000, 0, 0, "WeaponIcons/BulletCannonIcon", "CannonEffectPrefabs/FireBulletEffect", "");
    public static WeaponData HeatseekerMissile = new WeaponData("Heatseeker Missile", 80, 6000, 30, 7, 0.05f, "WeaponIcons/HeatseekerMissileIcon", "", "MissilePrefabs/HeatseekerMissile");
    public static WeaponData RadarMissile = new WeaponData("Radar Missile", 100, 8000, 20, 12, 0.008f, "WeaponIcons/RadarMissileIcon", "", "MissilePrefabs/RadarMissile");

    // Weapon controllers

    public const string EnemyLayerName = "Enemy";
    public const string PlayerLayerName = "Player";
    public const int MaxNumWeapons = 2; // Change when adding missiles
    public const int FireMissileWaitSeconds = 3;
    public const float MissileSpeed = 500;
    public const string HeatSignatureTag = "HeatSource";
    public const float MaxMissileSpeed = 2000;
    public const float MissileSpeedPush = 200;

    // Player controller

    public const string RetractGearsAnimParamName = "RetractLandingGears";
    public const float PlDefaultDrag = 0.05f;
    public const float PlBrakeDrag = 0.01f;
    public const float PlTurnDrag = 0.003f;
    public const float PlMinSpeedAir = 100f;
    public const int SendHeightFramerule = 2;
    public const int SendCoordsFramerule = 1;
    public const int SendSpeedFramerule = 5;
    public const float HeightTreshold = 4100;
    public const float HeightDrag = 200f;
    public const float HeightDragTurn = 1f;
    public const string GearsRetractTriggerTag = "GearsRetractor";
    public const string PlayerTagName = "Player";
    public const string EnemiesTagName = "Enemy";

    // Tracker controller

    public const string EnemyPointTagName = "RadarPointEnemy";
    public const string RadarPointLayerName = "RadarTracked";
    public const float CoveredDistance = 30000;
    public static Color32 OutOfRangeColour = new Color32(140, 140, 140, 255);
    public static Color32 EnemyColour = new Color32(255, 66, 0, 255);
    public static Color32 AllyColour = new Color32(0, 255, 7, 255);
    public const float MissileBarUpdBoundLow = 0.3f;
    public const float MissileBarUpdBoundHigh = 0.7f;

    // UI

    public const string DefaultSpeedValueUI = "Speed: 0000 km/h";
    public const string DefaultAutoSpeedValueUI = "Auto speed: 0000 km/h";
    public const float HeightMeterValueMinUI = 0f;
    public const float HeightMeterValueAlertUI = 1000f;
    public const float HeightMeterValueMaxUI = 15230f;
    public static Color32 HeightAboveAlertColour = new Color32(80, 255, 0, 255);
    public static Color32 HeightBelowAlertColour = new Color32(255, 141, 0, 255);
    public static Color32 AutoSpeedColourOff = new Color32(130, 130, 130, 190);
    public static Color32 AutoSpeedColourOn = new Color32(53, 255, 0, 255);
    public static Color32 WeaponEmptyIconColour = new Color32(78, 78, 78, 178);

}