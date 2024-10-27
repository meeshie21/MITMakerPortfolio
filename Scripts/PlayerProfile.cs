using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfile
{
    private string username;

    public PlayerProfile(string username)
    {
        this.username = username;
    }

    public string GetUsername() { return this.username; }
}
