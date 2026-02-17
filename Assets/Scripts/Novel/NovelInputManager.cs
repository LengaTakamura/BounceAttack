using System;
using R3;
using UnityEngine;

namespace Novel
{
    public sealed class NovelInputManager:IDisposable
    {
        private readonly Subject<Unit> _onClicked = new();
        public Observable<Unit> OnClicked => _onClicked;
        public NovelInputManager()
        {
        }

        public void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _onClicked?.OnNext(Unit.Default);
            }
        }
        public void Dispose()
        {
           _onClicked?.Dispose();
        }
    }
}
