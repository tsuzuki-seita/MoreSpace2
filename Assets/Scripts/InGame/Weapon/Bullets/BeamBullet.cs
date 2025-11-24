using System;
using ObjectPool;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;

namespace MoreSpace.InGame.Weapons.Bullets
{
    public class BeamBullet : MonoBehaviour
    {
        [SerializeField] private LineRenderer[] _renderer = new LineRenderer[2];
        [SerializeField] private int _segments = 10; 
        [SerializeField] private float _noiseAmount = 0.5f;
        [SerializeField] private float _noiseSpeed = 10f;
        [SerializeField] private Vector2 _scrollSpeed = new Vector2(-5.0f, 0.0f);

        public void Visualize(Vector3 start, Vector3 end)
        {
            VisualizeStraight(start,end,0);
            VisualizeLightning(start,end,1);
            VisualizeLightning(start,end,2);
            VisualizeLightning(start,end,3);
        }

        void VisualizeStraight(Vector3 start, Vector3 end, int index)
        {
            _renderer[index].SetPosition(0,start);
            _renderer[index].SetPosition(1,end);
            _renderer[index].material.mainTextureOffset += _scrollSpeed * Time.deltaTime;
        }

        void VisualizeLightning(Vector3 start, Vector3 end, int index)
        {
            // 頂点数を増やす
            _renderer[index].positionCount = _segments + 1;
            _renderer[index].SetPosition(0, start);
            _renderer[index].SetPosition(_segments, end);

            for (int i = 1; i < _segments; i++)
            {
                float t = (float)i / _segments;
                Vector3 basePos = Vector3.Lerp(start, end, t);

                // ランダムなノイズを加える（Time.timeを使うことでジリジリ動く）
                Vector3 noise = new Vector3(
                    Mathf.PerlinNoise(t * 10, Time.time * _noiseSpeed * (index + 1)) - 0.5f,
                    Mathf.PerlinNoise(t * 20, Time.time * _noiseSpeed * (index + 1)) - 0.5f,
                    Mathf.PerlinNoise(t * 30, Time.time * _noiseSpeed * (index + 1)) - 0.5f
                ) * _noiseAmount;

                _renderer[index].SetPosition(i, basePos + noise);
            }
        }
    }
}