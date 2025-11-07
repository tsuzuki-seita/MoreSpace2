
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
            if(!mainCamera) return;
            foreach (var ui in UIs)
            {
                ui.LookAt(mainCamera.transform);
                ui.localEulerAngles = new Vector3(0,ui.localEulerAngles.y, 0);
            }
        }
    }
}