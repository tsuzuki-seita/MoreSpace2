
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreSpace.InGame.Player
{
    public class LookUiToCamera : MonoBehaviour
    {
        [SerializeField] private List<Transform> UIs = new List<Transform>();
        [SerializeField] private Transform mainCamera;

        public void AssertCamera(Transform camera)
        {
            mainCamera = camera;
        }

        public void AssertUI(Transform ui)
        {
            UIs.Add(ui);
        }

        private void Update()
        {
            if(mainCamera == null) return;
            foreach(var ui in UIs)
                ui.LookAt(mainCamera.transform);
        }
    }
}