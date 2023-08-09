using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sequence.Demo
{
    public class SequenceUI : MonoBehaviour
    {
        private UIConnect connect;
        
        private void Awake()
        {
            connect = FindObjectOfType<UIConnect>();
        }

        private void Start()
        {
            connect.Open();
        }
    }
}