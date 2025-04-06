using UnityEngine;


public enum GameState
{
    Menu,
    Options,
    Playing,
    Pause,
    GameOver,
    Confirmation,
    Transition
}

public enum GameOverType
{
    GroundCrash,
    WaterCrash,
    FuelDepleted,
    TutorialFail
}

public enum PlayMode
{
    FireMission,
    Tutorial,
    Generated
}

public enum MenuType
{
    None,
    Main,
    Options,
    Missions,
    Tutorials,
    Gameplay,
    Controls
}

public struct PlayerCheckpointData
{
    public Vector3 Position { get; set; }
    public Vector3 ForwardVector { get; set; }
    public float PlayerEulerRotationY { get; set; }
    public float FuelRemaining { get; set; }
    public float WaterRemaining { get; set; }
}

public struct FireData
{
    public string GroupName { get; }
    public string Name { get; }
    public Vector3 Position { get; }

    public FireData(string groupName, string name, Vector3 position)
    {
        GroupName = groupName;
        Name = name;
        Position = position;
    }
}

public struct FireCheckpointData
{
    public Vector3 PlayerPosition { get; set; }
    public float FuelRemaining { get; set; }
    public float WaterRemaining { get; set; }
}