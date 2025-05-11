using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

public class Multiplay : MonoBehaviour
{
   [SerializeField] private ushort maxPlayers = 10;
   private string _backFillTicketId = string.Empty;
   private ushort _serverPort;
   private IServerQueryHandler _serverQuery;

   private void Start()
   {
      if (Application.platform != RuntimePlatform.LinuxServer)
      {
         return;
      }

      Init();
      StartMultiplay().Forget();
   }

   private void Init()
   {
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = 60;
      if (Camera.main)
      {
         Camera.main.enabled = false;
      }

      /*var args = Environment.GetCommandLineArgs();

      for (var i = 1; i < args.Length; i++)
      {
         switch (args[i])
         {
            case "-port":
               if (args.Length > i + 1)
               {
                  _serverPort = ushort.Parse(args[i + 1]);
               }

               break;
         }
      }*/
   }

   private async UniTaskVoid StartMultiplay()
   {
      await UnityServices.InitializeAsync();

      var serverCallback = new MultiplayEventCallbacks();
      serverCallback.Allocate += OnAllocate;


      await MultiplayService.Instance.SubscribeToServerEventsAsync(serverCallback);

      _serverQuery =
         await MultiplayService.Instance.StartServerQueryHandlerAsync(maxPlayers, "MiniStadium", "n/a", "n/a", "n/a");

      LoopSyncServerQuery().Forget();
   }

   private async UniTask LoopSyncServerQuery()
   {
      while (true)
      {
         await UniTask.Yield();
         if (_serverQuery == null)
         {
            continue;
         }

         _serverQuery.CurrentPlayers = (ushort)InstanceFinder.ServerManager.Clients.Count;
         _serverQuery.UpdateServerCheck();
      }
   }

   private async void OnAllocate(MultiplayAllocation obj)
   {
      var serverConfig = MultiplayService.Instance.ServerConfig;

      InstanceFinder.ServerManager.StartConnection(serverConfig.Port);
      var matchResult = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
      _backFillTicketId = matchResult.BackfillTicketId;
      LoopBackfillTicket().Forget();
   }

   private async UniTaskVoid LoopBackfillTicket()
   {
      while (true)
      {
         await UniTask.Delay(1000);

         var localBackFillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync((_backFillTicketId));
         _backFillTicketId = localBackFillTicket.Id;
      }
   }
}