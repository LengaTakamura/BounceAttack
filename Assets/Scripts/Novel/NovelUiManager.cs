using System;
using R3;

namespace Novel
{
    public sealed class NovelUiManager:IDisposable
    {
        private NovelView _view;
        private TalkText _talk;
        private string[] _texts;
        private int _skipCount;
        public Action OnSceneChanged;
        private readonly CompositeDisposable _disposables = new();

        // �{���ł����ScriptableObject
        public NovelUiManager(NovelView view)
        {
            _view = view;
            _talk = new TalkText(_view);
            _texts = view.Texts;
            _skipCount = 0;
        }

        public void InitByPresenter(NovelPresenter presenter)
        {
            presenter.OnClicked?.Subscribe(_ => OnClicked()).AddTo(_disposables);
            Init();
        }

        private void Init()
        {
            _talk.Init();
        }

        public void OnUpdate()
        {

        }


        private void OnClicked()
        {
            if (_skipCount == _texts.Length - 1)
            {
                OnSceneChanged?.Invoke();
                return;
            }
            _skipCount++;
            _talk.StartTextAnim(_texts[_skipCount]);
        }

        public void Dispose()
        {
            OnSceneChanged = null ;
            _disposables?.Dispose();
        }
    }

}


