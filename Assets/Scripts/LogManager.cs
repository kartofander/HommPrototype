using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LogManager : MonoBehaviour
    {
        [SerializeField] private Text textObject;

        public static LogManager instance;

        void Awake()
        {
            instance = this;
        }

        public void Log(string message)
        {
            var time = DateTime.Now.ToShortTimeString();
            var text = $"[{time}]: {message}\n";
            textObject.text += text;
        }
    }
}
