using UnityEngine;

[System.Serializable]
public class PlayerStateFactory
{
    public PlayerMoveState _moveState;
    public PlayerJumpState _jumpState;
    public PlayerIdleState _idleState;
    public PlayerFallState _fallState;
    public PlayerSwitchGravityState _switchGravityState;


    public PlayerStateFactory(PlayerController m_Controller)
    {
        _moveState = new PlayerMoveState(m_Controller);
        _jumpState = new PlayerJumpState(m_Controller);
        _idleState = new PlayerIdleState(m_Controller);
        _fallState = new PlayerFallState(m_Controller);
        _switchGravityState = new PlayerSwitchGravityState(m_Controller);
    }
}
