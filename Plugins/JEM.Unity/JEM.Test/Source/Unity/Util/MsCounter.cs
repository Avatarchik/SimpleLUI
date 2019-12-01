using System.Collections.Generic;
using UnityEngine;

namespace JEM.Test.Unity.Util
{
    public class MsCounter : MonoBehaviour
    {
        public int HistorySize = 64;

        private float _ms;
        private float _fps;
        private Color _condition;
        private string _str;
        private string _avgStr;
        private float _msTimeDelta;

        private int _msHistoryIndex = 0;
        private readonly List<float> _msHistory = new List<float>();

        private void Start()
        {
            for (int index = 0; index < HistorySize; index++)
            {
                _msHistory.Add(0f);
            }
        }

        private void Update()
        {
            var fpsDelta = Time.deltaTime;
            _ms = fpsDelta * 1000.0f;
            _fps = 1.0f / fpsDelta;

            if (_fps < 30)
                _condition = Color.red;
            else if (_fps < 50)
                _condition = Color.yellow;
            else
            {
                _condition = Color.white;
            }

            _str = $"{_ms:0.000} ms/frame ({_fps:0} FPS)";

            _msHistoryIndex++;
            if (_msHistoryIndex >= _msHistory.Count)
                _msHistoryIndex = 0;

            _msHistory[_msHistoryIndex] = _ms;

            var avgMs = 0f;
            for (var index = 0; index < _msHistory.Count; index++)
            {
                var m = _msHistory[index];
                avgMs += m;
            }
            avgMs /= _msHistory.Count;
            _avgStr = $"{avgMs:0.000} avgMs";
        }

        private void OnDisable()
        {
            Debug.Log($"Avg: " + _avgStr);
        }

        private void OnGUI()
        {
            GUI.color = _condition;
            GUILayout.Box(_str);
            GUILayout.Box(_avgStr);
        }
    }
}
