using UnityEngine;

public static class Constants {

    // Player controller

    public const float PlDefaultDrag = 0.12f;
    public const float PlDefaultAngularDrag = 3f;
    public const float PlTurnAngularDrag = 0.05f;
    public const float PlBrakeDrag = 0.01f;
    public const float PlTurnDrag = 0.0015f;
    public const float PlaneLiftForce = 0.9f;
    public const int SendHeightFramerule = 2;
    public const int SendCoordsFramerule = 1;
    public const int SendSpeedFramerule = 5;
    public const int SpinPropellerFramerule = 7;
    public const float MinHeightAllowed = 0;
    public const float MaxHeightAllowed = 1200;
    public const float HeightDrag = 0.5f;
    public const float HeightDragTurn = 1f;
    public const string PlayerTagName = "Player";
    public const string EnemiesTagName = "Enemy";
    public const float PitchDragAngle = 21;
    public const float HighSpeedDrag = 0.0002f;
    public const float HighPitchDrag = 0.001f;
    public const float MaxIdlePropellerSpeed = 50;
    public const float FuelCapacity = 10000;
    public const float EngineRunningFuelWaste = 0.01f;
    public const float PlaneMaxSpeed = 210;
    public const float WeightPlaneNoLoad = 2000;
    public const float MaxWeightPlaneFullyLoaded = 3000;
    public static Vector3 CameraTrailingDistanceDefault = new Vector3(5.75f,1.29f,0);
    public static Vector3 CameraTrailingDistanceWater = new Vector3(6f, 2.3f, 0);
    public static Vector3 CameraCrashDistance = new Vector3(30, 15, 0);
    public const float CameraTransitionSpeed = 0.05f;
    public const float CameraTimeLimit = 3;


    // Fire Missions

    public const float WaterCapacity = 1000;
    public const float WaterWasteRate = 1.5f;
    public const float WaterScoopRate = 30;
    public const float WaterQuantityToWeightRatio = 1;
    public const float PourWaterBankAngleMinMax = 15;
    public const float ExtinguishedComboTime = 2.5f;
    public const int SingleExtinguishScore = 100;
    public const int WaterUnitScoopScore = 10;
    public const string WaterSurfaceTagName = "Surface";
    public const string WaterDepthsTagName = "Depths";
    public const float WaterFloatForce = 375;

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
    public const float AttitudeMeterMaxPitchShown = 45;
    public const float AlertHeightUI = 200;
    public static Color32 HeightAboveAlertColour = new Color32(80, 255, 0, 255);
    public static Color32 HeightBelowAlertColour = new Color32(255, 141, 0, 255);
    public static Color32 SpeedColourInactive = new Color32(130, 130, 130, 190);
    public static Color32 SpeedColourIndicatorOn = new Color32(255, 130, 0, 255);
    public static Color32 AutoSpeedColourOn = new Color32(53, 255, 0, 255);
    public static Color32 WeaponEmptyIconColour = new Color32(78, 78, 78, 178);
    public const float TargetIconMovementMult = 2;
    public const float BarResetBorder = 300;
    public static Color32 FuelGaugeColor = new Color32(255, 0, 0, 1);
    public const float FuelGaugeNormalAlpha = 1;
    public const float FuelGaugeAlertAlpha = 160;
    public static Color32 FuelGaugeEmptyColour = new Color32(87, 87, 87, 200);
    public static Color32 FadePanelDefaultColour = new Color32(0, 0, 0, 0);
    public static Color32 ExtinguishSignColour = new Color32(69, 237, 0, 0);
    public static Color32 ExtinguishSignColourAll = new Color32(250, 255, 65, 0);
    public const byte UISignMaxAlpha = 200;
    public const string ExtinguishSignDefaultText = "FIRE EXTINGUISHED";
    public const string ExtinguishSignAllExtinguishedText= "ALL FIRES EXTINGUISHED";
    public const int UISignFadeSpeed = 10;
    public const float UISignDefaultFontSize = 60;
    public const float UISignMaxFontSize = 80;
    public static Color32 ScoreToAddSignColour = new Color32(255, 255, 0, 0);
    public static float FadeScreenSpeed = 5;
    public static Color32 CrashSignColourGround = new Color32(237, 83, 0, 0);
    public static Color32 CrashSignColourWater = new Color32(0, 228, 227, 0);
    public static Color32 CrashSignColourFuel = new Color32(237, 184, 0, 0);
    public static Color32 ClearSignColour = new Color32(240, 255, 55, 0);
    public const string CrashSignTextCrash = "CRASH";
    public const string CrashSignTextEmpty = "FUEL DEPLETED";
    public const string StageClearSign = "STAGE CLEAR";
    public const string TutorialClearSign = "TUTORIAL CLEAR";
    public static Vector3 DefaultLocatorIconPosition = new Vector3(0, 0, 0);
    public const string GoalSphereTag = "Goal";

    // Terrain interaction

    public const string TerrainTagName = "Terrain";
    public const string StageBoundsTagName = "StageBounds";
    public const string LandingZoneTagName = "LandingZone";
    public const float LandingPitchMin = -6;
    public const float LandingPitchMax = 3;
    public const float LandingBankMin = -5;
    public const float LandingBankMax = 5;
    public const float ScoopPitchMin = -15;
    public const float ScoopPitchMax = 3;
    public const float ScoopBankMin = -5;
    public const float ScoopBankMax = 5;
    public const float LandingTimer = 1.5f;

    // Gameplay
    public const float FireDefaultIntensity = 60;
    public const float FireParticleDamage = 5;
    public static Vector3 FireScaleReduction = new Vector3(0, 0, 0.04f);
    public const int ClearSphereScore = 200;
    public const int LandingScore = 1000;

}