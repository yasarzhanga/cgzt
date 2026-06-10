using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace jxzt
{
    [RequireComponent(typeof(RawImage))]
    public class StudentLayerManager : LayerManager
    {

        private void Start()
        {
            LayerSize = new Size(MainManager.instance.width, MainManager.instance.height);
            SetActive();
        }
    }
}

