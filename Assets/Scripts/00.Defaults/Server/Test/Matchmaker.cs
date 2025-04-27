using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class Matchmaker : MonoBehaviour
{
    private string _ticketId = string.Empty;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Start Client"))
        {
            StartClient().Forget();
        }
    }

    public async UniTask StartClient()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        var players = new List<Unity.Services.Matchmaker.Models.Player>
        {
            new(AuthenticationService.Instance.PlayerId)
        };

        var option = new CreateTicketOptions("PublicQueue");

        var response = await MatchmakerService.Instance.CreateTicketAsync(players, option);
        _ticketId = response.Id;

        LoopCheckMatch().Forget();
    }

    private async UniTask LoopCheckMatch()
    {
        var gotMatch = false;

        while (gotMatch == false)
        {
            await UniTask.Delay(1000);
            var status = await MatchmakerService.Instance.GetTicketAsync(_ticketId);
            var assignment = status.Value as MultiplayAssignment;

            switch (assignment.Status)
            {
                case MultiplayAssignment.StatusOptions.InProgress:
                    Debug.Log("Waiting for Match...");
                    continue;
                case MultiplayAssignment.StatusOptions.Found:
                    InstanceFinder.ClientManager.StartConnection(assignment.Ip, (ushort)assignment.Port);
                    break;
                default:
                    var msg = $"Fail: {assignment.Status}, {assignment.Message}";
                    throw new Exception(msg);
            }

            gotMatch = true;
        }
    }
}
