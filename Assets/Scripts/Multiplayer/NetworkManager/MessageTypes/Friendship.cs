using System.Collections.Generic;

public struct Friend
{
    public string player_id;
    public string player_name;
}

public struct FriendList
{
    public List<Friend> friends;
}

public struct FriendRequest
{
    public string player_id1;
    public string player_id2;
}