//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIImage : SLUIMaskableGraphic
    {
        private string _sprite;
        private Coroutine _spriteWorker;
        public string sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (!sprite.Contains(":"))
                {
                    sprite = $"{Environment.CurrentDirectory}//{sprite}";
                }

                if (_spriteWorker != null)
                {
                    Original.StopCoroutine(_spriteWorker);
                    _spriteWorker = null;
                }

                _spriteWorker = Original.StartCoroutine(LoadFile());
            }
        }

        public bool preserveAspect
        {
            get => Original.preserveAspect;
            set => Original.preserveAspect = value;
        }

        internal new Image Original { get; private set; }

        public SLUIImage() { }
        ~SLUIImage()
        {
            if (_spriteWorker != null && Original != null)
                Original.StopCoroutine(_spriteWorker);
        }

        /// <inheritdoc />
        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Image>();
        }

        private IEnumerator LoadFile()
        {
            var url = $"file:///{_sprite}";
            using (var r = UnityWebRequestTexture.GetTexture(url))
            {
                yield return r.SendWebRequest();
                
                if (r.isNetworkError || r.isHttpError)
                    Debug.LogError($"Failed to load {url}. {r.error}");
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(r);
                    Original.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                    yield return Original.sprite;
                }
            }

            _spriteWorker = null;
        }
    }
}
