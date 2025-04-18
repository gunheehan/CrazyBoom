using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClientLobby : MonoBehaviour
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Transform roomListParent;
    [SerializeField] private GameObject roomListItemPrefab;

    private List<string> roomList = new List<string>();

    private ClientToServerRoomProxy roomProxy;

    private void Start()
    {
        roomProxy = new ClientToServerRoomProxy();

        // 새로 고침 버튼 클릭 시 룸 목록 요청
        refreshButton.onClick.AddListener(RequestRoomList);

        // 방 생성 버튼 클릭 시 룸 생성 요청
        createRoomButton.onClick.AddListener(CreateRoom);

        // 초기 룸 목록 요청
        RequestRoomList();
    }

    private void OnEnable()
    {
        // 룸 목록 업데이트 이벤트 구독
        RoomListEventDispatcher.OnRoomListUpdated += UpdateRoomList;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        RoomListEventDispatcher.OnRoomListUpdated -= UpdateRoomList;
    }

    private void RequestRoomList()
    {
        // 서버에서 룸 목록 요청
        roomProxy.RequestRoomList();
    }

    private void CreateRoom()
    {
        string newRoomId = "Room_" + Random.Range(1000, 9999); // 임의로 룸 ID 생성
        roomProxy.RequestCreateRoom(newRoomId); // 서버에 룸 생성 요청
    }

    private void JoinRoom(string roomId)
    {
        roomProxy.RequestJoinRoom(roomId); // 서버에 방 참가 요청
    }

    // 서버에서 받은 룸 목록을 UI에 갱신
    public void UpdateRoomList(List<string> roomList)
    {
        this.roomList = roomList;

        // 기존의 리스트 아이템들을 삭제
        foreach (Transform child in roomListParent)
        {
            Destroy(child.gameObject);
        }

        // 룸 목록을 UI에 업데이트
        foreach (var room in roomList)
        {
            var roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.GetComponentInChildren<Text>().text = room;
            roomItem.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room));
        }
    }
}
