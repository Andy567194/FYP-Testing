using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Skill1
}

public struct InputData : INetworkInput
{
    public NetworkButtons JumpButton;
    public Vector2 MoveInput;
    public Angle Pitch;
    public Angle Yaw;
    public NetworkButtons Skill1Button;
    public float ScrollInput;
}
