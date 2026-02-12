using R3;
using UnityEngine;

namespace Novel
{
    public sealed class NovelInputManager
    {
        private readonly Subject<Unit> _onClicked = new();
        public Observable<Unit> OnClicked => _onClicked;

        private readonly CompositeDisposable _disposables = new();
        public NovelInputManager()
        {
        }

        public void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _onClicked.OnNext(Unit.Default);
            }
        }

        public void OnEnd()
        {
            _disposables.Dispose();
        }

        
    }
}
