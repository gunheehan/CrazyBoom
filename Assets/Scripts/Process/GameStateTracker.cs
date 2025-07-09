using Unity.Services.Lobbies.Models;

public class GameStateTracker
{
    private Lobby current;

    public GameStateTracker(Lobby initialLobby)
    {
        current = initialLobby;
    }

    public Lobby Apply(LobbyChanges changes)
    {
        // 이곳은 실제로 변경된 로비 데이터를 current에 적용해야 합니다.
        // 단순화된 버전에서는 최신 상태를 SDK에서 다시 받아와서 갱신할 수 있음.
        // 여기선 그냥 그대로 반환한다고 가정.
        return current;
    }

    public Lobby GetCurrent() => current;
}