using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MoreSpace.InGame.Master
{
    public class VisualizeJudge : MonoBehaviour
    {
        [SerializeField] private Text[] breakCrystalCount = new Text[2];
        [SerializeField] private Image barImage;
        [SerializeField] private float[] barImagePosition = new float[4];
        private static ReactiveProperty<int[]> NowCount = new ReactiveProperty<int[]>(new int[2]);

        private void Start()
        {
            NowCount.Subscribe(val => ResetView());
        }

        void ResetView()
        {
            breakCrystalCount[0].text = NowCount.Value[0].ToString();
            breakCrystalCount[1].text = NowCount.Value[1].ToString();

            var compare = NowCount.Value[0] - NowCount.Value[1];
            barImage.transform.localPosition = new Vector2(Mathf.Sign(compare) * barImagePosition[Mathf.Abs(compare)],barImage.transform.localPosition.y);
        }

        public static void AddBreakCount(int playerIndex)
        {
            NowCount.Value[playerIndex]++;
            NowCount.OnNext(NowCount.Value);
        }
    }
}