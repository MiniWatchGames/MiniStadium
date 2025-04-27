using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakingManager : MonoBehaviour
{
    public enum Team
    {
        Blue,
        Red
    }
    public bool isMatched;
    public bool isMatchedCanceled;

    public Team playerTeam;
    // Start is called before the first frame update
    public Team SetPlayerTeam()
    {
        var team = Random.Range(0, 1);
        return (Team)team;
    }

    public void OnMatchMaking()
    {
        
    }
    public void OnClickedButton()
    {
        
    }
}
