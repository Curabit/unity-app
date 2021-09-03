using System;

[Serializable]
public class SetPairingCode
{
    public string req = "set_pairing_code";
    public string code;
}

[Serializable]
public class GetID
{
    public string req = "get_id";
    public string code;
}

[Serializable]
public class GetUserSession
{
    public string req = "get_user_session";
    public string id;
}

public class SessionDetails
{
    public string status { get; set; }
}

public class Link
{
    public string resp { get; set; }
    public SessionDetails session_details { get; set; }
}

public class Link2
{
    public string resp { get; set; }
}

public class RequestID
{
    public SetPairingCode setPairingCode;
    public GetID getID;
    public GetUserSession getUserSession;
}