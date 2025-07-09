using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class ChangedProperty<T>
{
    public bool Changed { get; set; }
}

public class LobbyChanges
{
    // 플레이어가 새로 들어왔는지
    public ChangedProperty<List<LobbyPlayerJoined>> PlayerJoined { get; set; }

    // 플레이어가 나갔는지
    public ChangedProperty<List<LobbyPlayerLeft>> PlayerLeft { get; set; }

    // 호스트 변경 여부
    public ChangedProperty<string> HostId { get; set; }

    // 플레이어 상태 변경 (예: 플레이어 데이터 변경)
    public ChangedProperty<List<LobbyPlayerStateChanged>> PlayerStateChanged { get; set; }

    // 로비 메타데이터 변경
    public ChangedProperty<Dictionary<string, DataObject>> LobbyDataChanged { get; set; }

    // 기타 필요한 변경사항 프로퍼티 추가 가능

    // public LobbyChanges()
    // {
    //     PlayerJoined = new ChangedProperty<List<LobbyPlayerJoined>> { Changed = false, Value = new List<LobbyPlayerJoined>() };
    //     PlayerLeft = new ChangedProperty<List<LobbyPlayerLeft>> { Changed = false, Value = new List<LobbyPlayerLeft>() };
    //     HostId = new ChangedProperty<string> { Changed = false, Value = string.Empty };
    //     PlayerStateChanged = new ChangedProperty<List<LobbyPlayerStateChanged>> { Changed = false, Value = new List<LobbyPlayerStateChanged>() };
    //     LobbyDataChanged = new ChangedProperty<Dictionary<string, DataObject>> { Changed = false, Value = new Dictionary<string, DataObject>() };
    // }
}

public class LobbyPlayerLeft
{
    public string PlayerId { get; set; }
}

public class LobbyPlayerStateChanged
{
    public Player Player { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}