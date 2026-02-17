
using System;
using R3;

namespace Novel
{
    public sealed class NovelPresenter : IDisposable
    {
        private NovelInputManager _novelInputManager;
        private NovelUiManager _novelUiManager;
        public Observable<Unit> OnClicked;
        public NovelPresenter(NovelInputManager novelInputManager, NovelUiManager novelUiManager)
        {
            _novelInputManager = novelInputManager;
            _novelUiManager = novelUiManager;
        }

        public void Init()
        {
            OnClicked = _novelInputManager.OnClicked;
            _novelUiManager.InitByPresenter(this);
        }

        public void OnUpdate()
        {
            if (_novelInputManager != null && _novelUiManager != null)
            {
                _novelInputManager.OnUpdate();
                _novelUiManager.OnUpdate();
            }
        }

        public void Dispose()
        {
            _novelInputManager.Dispose();
            _novelUiManager.Dispose();
            OnClicked = null;
        }

    }
}


