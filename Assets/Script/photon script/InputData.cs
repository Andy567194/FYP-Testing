using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Skill1,

    PickUp,
    Skill2,
    Skill3,
    VoiceChat,
}

public struct InputData : INetworkInput
{
    public NetworkButtons JumpButton;
    public NetworkButtons PickUpButton;
    public Vector2 MoveInput;
    public Angle Pitch;
    public Angle Yaw;
    public NetworkButtons Skill1Button;
    public float ScrollInput;
    public NetworkButtons Skill2Button;
    public NetworkButtons Skill3Button;
    public NetworkButtons VoiceChatButton;
}
