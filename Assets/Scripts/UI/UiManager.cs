using System;
using System.Collections.Generic;

namespace UI
{
    public class UiManager : IBeatSyncListener,IDisposable
    {
        private List<UiBase> _uiObjects = new();

        public void InGameInitByPresenter(Presenter presenter, UiView uiView)
        {
            BeatSyncDispatcher.Instance.RegisterBeatSync(this);

            _uiObjects = uiView.UiObjects;
            foreach (var ui in _uiObjects)
            {
                ui.Init(presenter);
            }

        }

        public void OnBeat(BeatInfo info)
        {
            foreach (var uiObject in _uiObjects)
            {
                uiObject.UIOnBeat(info);
            }
        }

        public void Dispose()
        {
            BeatSyncDispatcher.Instance.UnregisterBeatSync(this);
            _uiObjects?.Clear();
        }

    }

    public enum UiType
    {
        Score, InputAction, Player, Time, Enemy, None
    }
}
