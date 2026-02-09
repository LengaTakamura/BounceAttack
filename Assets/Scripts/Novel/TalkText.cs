

using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Novel
{
    public class TalkText : NovelUiBase
    {
        private Text _text;

        private string[] _texts;
        public TalkText(NovelView view)
        {
            _text = view.Text;
            _texts = view.Texts;
        }

        public override void Init()
        {
            _text.text = _texts[0];
        }

        public void StartTextAnim(string text)
        {
            _text.text = text;
        }

        public override void OnUpdate()
        {

        }

        private async UniTaskVoid TextAnimationAsync(string text)
        {

        }

       
    }
}


