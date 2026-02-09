using R3;

namespace Novel
{
    public sealed class NovelUiManager
    {
        private NovelView _view;

        private TalkText _talk;

        private string[] _texts;

        private int _skipCount;

        // –{—ˆ‚Å‚ ‚ê‚ÎScriptableObject
        public NovelUiManager(NovelView view)
        {
            _view = view;
            _talk = new TalkText(_view);
            _texts = view.Texts;
            _skipCount = 0;
        }

        public void InitByPresenter(NovelPresenter presenter)
        {
            presenter.OnClicked.Subscribe(_ => OnClicked());
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
            if (_skipCount == _texts.Length - 1) return;
            _skipCount++;
            _talk.StartTextAnim(_texts[_skipCount]);
        }
    }

}


