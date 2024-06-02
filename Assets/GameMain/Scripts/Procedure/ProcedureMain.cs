//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace StarForce
{
    public class ProcedureMain : ProcedureBase
    {
        private bool m_GotoMenu = false;
        private TileMapLogic m_TileMapLogic;
        private TileMapController m_TileMapController;
        private TileMapForm m_TileMapForm;
        private float m_DelayInputTimer;
        private Direction m_LastInput;
        private int m_TileMapFormSerialId;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public void GotoMenu()
        {
            m_GotoMenu = true;
        }

        public void Restart()
        {
            m_TileMapController.Restart();
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_GotoMenu = false;
            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccess);
            GameEntry.Event.Subscribe(OpenUIFormFailureEventArgs.EventId, OpenUIFormFailure);
            m_TileMapFormSerialId = (int)GameEntry.UI.OpenUIForm(UIFormId.TileMapForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.UI.CloseUIForm(m_TileMapForm);
            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OpenUIFormSuccess);
            GameEntry.Event.Unsubscribe(OpenUIFormFailureEventArgs.EventId, OpenUIFormFailure);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            m_DelayInputTimer += elapseSeconds;
            if (m_DelayInputTimer > 0.5f && m_LastInput != Direction.None)
            {
                m_TileMapLogic.SetInput(m_LastInput);
                m_TileMapLogic.Update();

                m_DelayInputTimer = 0f;
                m_LastInput = Direction.None;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameEntry.UI.OpenUIForm(UIFormId.PauseMenuForm, this);
            }

            if (m_GotoMenu)
            {
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt("Scene.Menu"));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OpenUIFormSuccess(object sender, BaseEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            if (ne.UIForm.SerialId != m_TileMapFormSerialId)
            {
                return;
            }

            m_TileMapForm = ne.UIForm.Logic as TileMapForm;
            m_TileMapLogic = new TileMapLogic(5, 5);
            m_TileMapController = new TileMapController(m_TileMapForm, m_TileMapLogic);
            m_TileMapForm.SetController(m_TileMapController);
            m_TileMapLogic.Start();
        }

        private void OpenUIFormFailure(object sender, BaseEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            Log.Error(ne.ErrorMessage);
        }
    }
}
