using System;
using Sequence.Utils;
using TMPro;
using UnityEngine;

namespace Sequence.Demo
{
    public class InfoPopupPanel : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _infoText;
        
        public override void Open(params object[] args)
        {
            _gameObject.SetActive(true);
            _animator.AnimateIn( _openAnimationDurationInSeconds);
            string info = args.GetObjectOfTypeIfExists<string>();
            if (string.IsNullOrEmpty(info))
            {
                throw new SystemException(
                    $"Invalid use. {GetType().Name} must be opened with a {typeof(string)} as an argument");
            }
            _infoText.text = info;
        }
    }
}