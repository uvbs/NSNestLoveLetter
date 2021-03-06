﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NSNetwork;

namespace NSNetwork
{
    public class Receiver
    {
        public static void OnReceive(Packet packet)
        {
            byte[] data = packet.GetData();
            eGCP type = (eGCP)BitConverter.ToInt16(data, 0);

            switch (type)
            {
                case eGCP.GCP_LoginRet:         Receiver.OnLogin(new GCPLoginRet(data));                    break;
                case eGCP.GCP_AliveAck:         Receiver.OnAliveAck(); break;
                case eGCP.GCP_RegisterUserRet:  Receiver.OnRegisterUserRet(new GCPRegisterUserRet(data));  break;
                case eGCP.GCP_RoomListRet:      Receiver.OnRoomListRet(new GCPRoomListRet(data));           break;
                case eGCP.GCP_CreateRoomRet:    Receiver.OnCreateRoomRet(new GCPCreateRoomRet(data));       break;
                case eGCP.GCP_EnterRoomRet:     Receiver.OnEnterRoomRet(new GCPEnterRoomRet(data));         break;
                case eGCP.GCP_LeaveRoomRet:     Receiver.OnLeaveRoomRet(new GCPLeaveRoomRet(data));         break;
                case eGCP.GCP_RoomState:        Receiver.OnRoomState(new GCPRoomState(data));               break;
                case eGCP.GCP_GameStartRet:     Receiver.OnGameStartRet(new GCPGameStartRet(data));         break;
                case eGCP.GCP_GameLoveLetter:   Receiver.OnReceiveGameLoveLetter(packet);                   break;
            }
        }

        static void OnReceiveGameLoveLetter(Packet packet)
        {
            byte[] data = packet.GetData();
            eGCP type = (eGCP)BitConverter.ToInt16(data, 0);
            if (type != eGCP.GCP_GameLoveLetter)
                return;

            eGCP_LoveLetter typeLL = (eGCP_LoveLetter)BitConverter.ToInt16(data, sizeof(short));

            switch (typeLL)
            {
                case eGCP_LoveLetter.GCP_LL_InitStatus:     Receiver.OnLLInitStatus(new GCPLLInitStatus(data)); break;
                case eGCP_LoveLetter.GCP_LL_Status:         Receiver.OnLLStatus(new GCPLLStatus(data));             break;
                case eGCP_LoveLetter.GCP_LL_ActionRet:      Receiver.OnLLActionRet(new GCPLLActionRet(data));       break;
                case eGCP_LoveLetter.GCP_LL_RoundResult:    Receiver.OnLLRoundResult(new GCPLLRoundResult(data));   break;
                case eGCP_LoveLetter.GCP_LL_FinalResult:    Receiver.OnLLFinalResult(new GCPLLFinalResult(data));   break;
                case eGCP_LoveLetter.GCP_LL_Aborted:        Receiver.OnLLAborted(new GCPLLAborted(data)); break;
                case eGCP_LoveLetter.GCP_LL_Emotion:        Receiver.OnLLEmotion(new GCPLLEmotion(data)); break;
            }
        }

        public delegate void LoginRetDelegate(GCPLoginRet loginRet);
        static LoginRetDelegate m_onLoginRetCallback = null;
        public static event LoginRetDelegate OnLoginRetCallback
        {
            add { m_onLoginRetCallback += value; }
            remove { m_onLoginRetCallback -= value; }
        }

        static void OnLogin(GCPLoginRet packet)
        {
            Debug.Log("### OnLogin ###");
            if (packet.result == GCPLoginRet.eResult.Success)
            {
                Debug.Log("nickname >> " + packet.nickName);
                GlobalData.Instance.userNickname = packet.nickName;
                GlobalData.Instance.userSN = packet.sn;
            }

            if (m_onLoginRetCallback != null)
                m_onLoginRetCallback(packet);
        }

        public delegate void RegisterUserRetDelegate(GCPRegisterUserRet registerUserRet);
        static RegisterUserRetDelegate m_onRegisterUserRetCallback = null;
        public static event RegisterUserRetDelegate OnRegisterUserRetCallback
        {
            add { m_onRegisterUserRetCallback += value; }
            remove { m_onRegisterUserRetCallback -= value; }
        }

        static void OnRegisterUserRet(GCPRegisterUserRet packet)
        {
            Debug.Log("### OnRegisterUserRest ###");

            if (m_onRegisterUserRetCallback != null)
                m_onRegisterUserRetCallback(packet);
        }

        static void OnAliveAck()
        {
            Sender.AliveAck();
        }

        public delegate void RoomListRetDelegate(GCPRoomListRet roomListRet);
        static RoomListRetDelegate m_onRoomListRetCallback;
        public static event RoomListRetDelegate OnRoomListRetCallback
        {
            add { m_onRoomListRetCallback += value; }
            remove { m_onRoomListRetCallback -= value; }
        }

        static void OnRoomListRet(GCPRoomListRet packet)
        {
            Debug.Log("### OnRoomListRet ###");

            if (m_onRoomListRetCallback != null)
                m_onRoomListRetCallback(packet);
        }

        public delegate void CreateRoomRetDelegate(GCPCreateRoomRet createRoomRet);
        static CreateRoomRetDelegate m_onCreateRoomRetCallback;
        public static event CreateRoomRetDelegate OnCreateRoomRetCallback
        {
            add { m_onCreateRoomRetCallback += value; }
            remove { m_onCreateRoomRetCallback -= value; }
        }

        static void OnCreateRoomRet(GCPCreateRoomRet packet)
        {
            Debug.Log("### OnCreateRoomRet ###");
            if (packet.result == GCPCreateRoomRet.eReuslt.Success)
            {
                GlobalData.Instance.roomSN = packet.sn;
            }

            if (m_onCreateRoomRetCallback != null)
                m_onCreateRoomRetCallback(packet);
        }

        public delegate void EnterRoomRetDelegate(GCPEnterRoomRet enterRoomRet);
        static EnterRoomRetDelegate m_onEnterRoomRetCallback;
        public static event EnterRoomRetDelegate OnEnterRoomRetCallback
        {
            add { m_onEnterRoomRetCallback += value; }
            remove { m_onEnterRoomRetCallback -= value; }
        }

        static void OnEnterRoomRet(GCPEnterRoomRet packet)
        {
            Debug.Log("### OnEnterRoomRet ###");
            if (packet.result == GCPEnterRoomRet.eResult.Success)
            {
                GlobalData.Instance.roomSN = packet.sn;
            }

            if (m_onEnterRoomRetCallback != null)
                m_onEnterRoomRetCallback(packet);
        }

        public delegate void LeaveRoomRetDelegate(GCPLeaveRoomRet leaveRoomRet);
        static LeaveRoomRetDelegate m_onLeaveRoomRetCallback;
        public static event LeaveRoomRetDelegate OnLeaveRoomRetCallback
        {
            add { m_onLeaveRoomRetCallback += value; }
            remove { m_onLeaveRoomRetCallback -= value; }
        }

        static void OnLeaveRoomRet(GCPLeaveRoomRet packet)
        {
            Debug.Log("### LeaveRoomRet ###");
            GlobalData.Instance.roomSN = 0;
            GlobalData.Instance.roomMasterSN = 0;

            if (m_onLeaveRoomRetCallback != null)
                m_onLeaveRoomRetCallback(packet);
        }

        public delegate void RoomStateDelegate(GCPRoomState roomState);
        static RoomStateDelegate m_onRoomStateCallback;
        public static event RoomStateDelegate OnRoomStateCallback
        {
            add { m_onRoomStateCallback += value; }
            remove { m_onRoomStateCallback -= value; }
        }

        static void OnRoomState(GCPRoomState packet)
        {
            Debug.Log("### OnRoomState ###");
            if ((packet.flag & (int)GCPRoomState.FLAG_WITHOUT_ROOM_MASTER) != 0)
                GlobalData.Instance.roomUsers = packet.listUsers;
            if ((packet.flag & (int)GCPRoomState.FLAG_ROOM_MASTER) != 0)
                GlobalData.Instance.roomMasterSN = packet.masterSN;            

            if (m_onRoomStateCallback != null)
                m_onRoomStateCallback(packet);
        }

        public delegate void GameStartRestDelegate(GCPGameStartRet gameStartRet);
        static GameStartRestDelegate m_onGameStartRetCallback;
        public static event GameStartRestDelegate OnGameStartRetCallback
        {
            add { m_onGameStartRetCallback += value; }
            remove { m_onGameStartRetCallback -= value; }
        }

        static void OnGameStartRet(GCPGameStartRet packet)
        {
            Debug.Log("### OnGameStartRet ###");
            if (m_onGameStartRetCallback != null)
                m_onGameStartRetCallback(packet);
        }


        // Game Love Letter
        public delegate void LLInitStatusDelegate(GCPLLInitStatus llStatus);
        static LLInitStatusDelegate m_onLLInitStatusCallback;
        public static event LLInitStatusDelegate OnLLInitStatusCallback
        {
            add { m_onLLInitStatusCallback += value; }
            remove { m_onLLInitStatusCallback -= value; }
        }

        static void OnLLInitStatus(GCPLLInitStatus packet)
        {
            Debug.Log("### OnLLInitStatus ###");
            if (m_onLLInitStatusCallback != null)
                m_onLLInitStatusCallback(packet);
        }

        public delegate void LLStatusDelegate(GCPLLStatus llStatus);
        static LLStatusDelegate m_onLLStatusCallback;
        public static event LLStatusDelegate OnLLStatusCallback
        {
            add { m_onLLStatusCallback += value; }
            remove { m_onLLStatusCallback -= value; }
        }

        static void OnLLStatus(GCPLLStatus packet)
        {
            Debug.Log("### OnLLStatus ###");
            if (m_onLLStatusCallback != null)
                m_onLLStatusCallback(packet);
        }

        public delegate void LLActionRetDelegate(GCPLLActionRet llActionRet);
        static LLActionRetDelegate m_onLLActionRetCallback;
        public static event LLActionRetDelegate OnLLActionRetCallback
        {
            add { m_onLLActionRetCallback += value; }
            remove { m_onLLActionRetCallback -= value; }
        }

        static void OnLLActionRet(GCPLLActionRet packet)
        {
            Debug.Log("### OnLLActionRet ###");
            if (m_onLLActionRetCallback != null)
                m_onLLActionRetCallback(packet);
        }

        public delegate void LLRoundResultDelegate(GCPLLRoundResult llRoundResult);
        public static LLRoundResultDelegate m_onLLRoundResultCallback;
        public static event LLRoundResultDelegate OnLLRoundResultCallback
        {
            add { m_onLLRoundResultCallback += value; }
            remove { m_onLLRoundResultCallback -= value; }
        }

        static void OnLLRoundResult(GCPLLRoundResult packet)
        {
            Debug.Log("### OnLLRoundResult ###");
            if (m_onLLRoundResultCallback != null)
                m_onLLRoundResultCallback(packet);
        }

        public delegate void LLFinalResultDelagate(GCPLLFinalResult llFinalResult);
        static LLFinalResultDelagate m_onLLFinalResultCallback;
        public static event LLFinalResultDelagate OnLLFinalResultCallback
        {
            add { m_onLLFinalResultCallback += value; }
            remove { m_onLLFinalResultCallback -= value; }
        }

        static void OnLLFinalResult(GCPLLFinalResult packet)
        {
            Debug.Log("### OnLLFinalResult ###");
            if (m_onLLFinalResultCallback != null)
                m_onLLFinalResultCallback(packet);
        }

        public delegate void LLAbortedDelagate(GCPLLAborted llFinalResult);
        static LLAbortedDelagate m_onLLAbortedCallback;
        public static event LLAbortedDelagate OnLLAbortedCallback
        {
            add { m_onLLAbortedCallback += value; }
            remove { m_onLLAbortedCallback -= value; }
        }

        static void OnLLAborted(GCPLLAborted packet)
        {
            Debug.Log("### OnLLAborted ###");
            if (m_onLLAbortedCallback != null)
                m_onLLAbortedCallback(packet);
        }

        public delegate void LLEmotionDelagate(GCPLLEmotion packet);
        static LLEmotionDelagate m_onLLEmotionCallback;
        public static event LLEmotionDelagate OnLLEmotionCallback
        {
            add { m_onLLEmotionCallback += value; }
            remove { m_onLLEmotionCallback -= value; }
        }
        static void OnLLEmotion(GCPLLEmotion packet)
        {
            Debug.Log("### OnLLEmotion ###");
            if (m_onLLEmotionCallback != null)
                m_onLLEmotionCallback(packet);
        }

        static public void ClearEmotionEvent()
        {
            m_onLLEmotionCallback = null;
        }

        static public void ClearLobbyEvent()
        {
            m_onRoomListRetCallback = null;
            m_onEnterRoomRetCallback = null;
            m_onCreateRoomRetCallback = null;
            m_onRoomStateCallback = null;            
        }
    }
}