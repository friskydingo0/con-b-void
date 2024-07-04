
public interface IGameStateListener
{
    // When deciding to expand, the additional functionality for the listeners could be added here.

    public void OnGameStateChanged(GameState fromState, GameState toState);
}
