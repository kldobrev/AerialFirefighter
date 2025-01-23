using UnityEngine;

public static class Constants {

    // Player controller

    public const float AirbourneThresholdHeight = 1;
    public const float PlDefaultDrag = 0.12f;
    public const float PlDefaultAngularDrag = 3f;
    public const float PlTurnAngularDrag = 0.05f;
    public const float PlBrakeDrag = 0.01f;
    public const float PlTurnDrag = 0.0012f;
    public const float PlaneLiftForce = 0.9f;
    public const float LiftModifier = 1.5f;
    public const float DragModifier = 0.003f;
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
    public const float WingChordLengthAtRoot = 0.86f;
    public const float WingChordLengthAtTip = 0.492f;
    public const float DryAirDensitySummer = 1.1839f;
    public const float DragCoefficientStreamlined = 0.03f;
    public const float KmPhToKnotsRatio = 0.54f;
    public const float StallLeanAngleMax = 80;
    public const float StallLeanVelocityTreshold = -15;
    public const float TurnStartBankAngleMin = -2f;



    // Camera

    public const int CameraModesCount = 4;
    public const float CameraSpecialModeHeightThreshold = 5;
    public static Vector3 CameraTrailingDistanceBehindAbove = new(0, 3.06f, -17);
    public static Vector3 CameraTrailingDistanceBehindCentered = new(0, 1.26f, -16.9f);
    public static Vector3 CameraTrailingDistancePropeller = new(0, 0.08f, 0.12f);
    public static Vector3 CameraTrailingDistanceFirstPerson = new(0, 0, 0.42f);
    public static Vector3 CameraTrailingDistanceSurface = new(0, 3.06f, -23);
    public static Vector3 CameraTrailingDistanceImpact = new(0, 15, 18);
    public static Vector3 CameraModeTransitionPoint = new(0, 0.49f, -4.43f);
    public static Vector3 CameraCrashDistance = new(30, 15, 0);
    public const float CameraTransitionSpeedDefault = 5;
    public const float CameraTransitionSpeedFrontBack = 30;
    public const float CameraTransitionSpeedAirToSurface = 8;
    public const float CameraTransitionSpeedSurfaceToAir = 8;
    public const float CameraTransitionSpeedGameOver = 3;
    public const float CameraTransitionSpeedImpact = 200;
    public const float CameraRotationSpeedAirToSurface = 10;
    public const float CameraRotationSpeedSurfaceToAir = 10;
    public const float CameraRotationSpeedDefault = 80;
    public const float CameraRotationSpeedImpact = 500;
    public const float CameraImpactViewAngleX = 100;
    public const float CameraTimeLimit = 3;
    public const string CameraTagName = "MainCamera";
    public const uint CameraSurfaceCheckInterval = 10;
    public const float CameraImpactAngleThreshold = -20;
    public const float CameraSurfaceToAirHeightTrigger = 3;
    public const float CameraUnderwaterEffectThreshold = 1;

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
    public static Color32 OutOfRangeColour = new(140, 140, 140, 255);
    public static Color32 EnemyColour = new(255, 66, 0, 255);
    public static Color32 AllyColour = new(0, 255, 7, 255);
    public const float MissileBarUpdBoundLow = 0.3f;
    public const float MissileBarUpdBoundHigh = 0.7f;

    // UI

    public const string DefaultSpeedValueUI = "Speed: ";
    public const string DefaultAutoSpeedValueUI = "Auto speed: ";
    public const float AttitudeMeterMaxPitchShown = 45;
    public const float AlertHeightUI = 200;
    public static Color32 HeightAboveAlertColour = new(80, 255, 0, 255);
    public static Color32 HeightBelowAlertColour = new(255, 141, 0, 255);
    public static Color32 SpeedColourInactive = new(130, 130, 130, 190);
    public static Color32 SpeedColourIndicatorOn = new(255, 130, 0, 255);
    public static Color32 AutoSpeedColourOn = new(53, 255, 0, 255);
    public static Color32 WeaponEmptyIconColour = new(78, 78, 78, 178);
    public const float TargetIconMovementMult = 2;
    public const float BarResetBorder = 300;
    public static Color32 FuelGaugeColor = new(255, 0, 0, 1);
    public const float FuelGaugeNormalAlpha = 1;
    public const float FuelGaugeAlertAlpha = 160;
    public static Color32 FuelGaugeEmptyColour = new(87, 87, 87, 200);
    public static Color32 EffectsPanelColourDefault = new(0, 0, 0, 0);
    public static Color32 EffectsPanelColourWater = new(0, 157, 255, 137);
    public static Color32 ExtinguishSignColour = new(69, 237, 0, 0);
    public static Color32 ExtinguishSignColourAll = new(250, 255, 65, 0);
    public const byte UISignMaxAlpha = 200;
    public const string ExtinguishSignDefaultText = "FIRE EXTINGUISHED";
    public const string ExtinguishSignAllExtinguishedText= "ALL FIRES EXTINGUISHED";
    public const int UISignFadeSpeed = 10;
    public const float UISignDefaultFontSize = 60;
    public const float UISignMaxFontSize = 80;
    public static Color32 ScoreToAddSignColour = new(255, 255, 0, 0);
    public static byte FadeScreenAlphaMin = 0;
    public static byte FadeScreenAlphaMax = 255;
    public static float FadeScreenSpeed = 5;
    public static Color32 CrashSignColourGround = new(237, 83, 0, 0);
    public static Color32 CrashSignColourWater = new(0, 228, 227, 0);
    public static Color32 CrashSignColourFuel = new(237, 184, 0, 0);
    public static Color32 ClearSignColour = new(240, 255, 55, 0);
    public const string CrashSignTextCrash = "CRASH";
    public const string CrashSignTextEmpty = "FUEL DEPLETED";
    public const string StageClearSign = "STAGE CLEAR";
    public const string TutorialClearSign = "TUTORIAL CLEAR";
    public static Vector3 DefaultLocatorIconPosition = new(0, 0, 0);
    public const string GoalSphereTag = "Goal";
    public const float ScreenFadePauseSpeed = 20;
    public const float ScreenFadeQuitSpeed = 10;
    public const float WaterGaugeAlphaDefault = 160;
    public const float WaterGaugeAlphaPouring = 50;
    public const float WaterGaugeAlphaChangeSpeed = 10;


    // Menus

    public const byte PauseMenuAlphaDefault = 0;
    public const byte PauseMenuAlphaVisible = 153;
    public const byte PauseMenuItemBakcgroundAlphaDefault = 0;
    public const byte PauseMenuItemBakcgroundAlphaVisible = 100;
    public const byte FadeScreenAlphaPause = 180;
    public const float FadeAlphaSpeedPause = 20;
    public const float PauseMenuHeight = 600;
    public const string RestartSignStage = "RESTART STAGE";
    public const string RestartSignTutorial = "RESTART TUTORIAL";
    public const string ContinueSignRegular = "CONTINUE";
    public const string ContinueSignCheckpoint = "CONTINUE FROM CHECKPOINT";
    public const string InGameMenuStateSignPause = "PAUSE";
    public const string InGameMenuStateSignGameOver = "GAME OVER";
    public static Color32 InGameMenuStateColorPause = new(116, 255, 62, 0);
    public const float MenuTextAlphaMin = 0;
    public const float MenuTextAlphaMax = 255;
    public const float MenuCursorAlphaMin = 0;
    public const float MenuCursorAlphaMax = 92;
    public const float MenuCursorFadeSpeedIn = 20;
    public const float MenuCursorFadeSpeedOut = 50;
    public const float InGameMenuBkgAlphaMin = 0;
    public const float InGameMenuBkgAlphaMax = 600;
    public const float InGameMenuBkgSizeChangeSpeed = 60;
    public const float InGameMenuTextFadeSpeedPauseIn = 30;
    public const float InGameMenuTextFadeSpeedPauseOut = 100;
    public const float InGameMenuTextFadeSpeedGameOver = 20;
    public const float InGameMenuTextTrigger = 440;
    public const float InGameMenuSizeTrigger = 24;
    public static Color32 MenuInactiveOptionColour = new(143, 143, 143, 96);
    public static Color32 InGameMenuOptionColourDefault = new(255, 255, 255, 0);
    public const string InGameUICanvasTagName = "InGameUI";
    public const string InGameMenuTagNam = "InGameMenu";
    public const string ConfirmPromptMenuTagName = "Confirm";
    public const string LeaveStagePromptText = "IF YOU EXIT YOUR CURRENT PROGRESS ON THIS STAGE WILL BE LOST. WILL YOU PROCEED?";
    public const float ConfirmPromptTextTrigger = 100;
    public const float ConfirmPromptBkgAlphaMax = 350;

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
    public const string GroundLayerName = "Ground";
    public const string BuildingLayerName = "Building";
    public const string WaterLayerName = "Water";

    // Gameplay

    public const float FireDefaultIntensity = 60;
    public const float FireParticleDamage = 5;
    public static Vector3 FireScaleReduction = new(0, 0, 0.04f);
    public const int ClearSphereScore = 200;
    public const int LandingScore = 1000;
    public const float WaterFloatForceUp = 0.12f;
    public const float WaterFloatForceDown = 1;
    public const string Stage1SceneName = "Stage1";

}