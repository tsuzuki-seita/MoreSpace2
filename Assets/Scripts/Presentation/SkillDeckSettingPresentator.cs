// Presentation/SkillSelectionPresenter.cs
using System.Collections.Generic;
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合
using VContainer; // DIコンテナ

namespace MoreSpace.Presentation
{
    public sealed class SkillSelectionPresenter : MonoBehaviour
    {
        // Application Service (DIで注入)
        private SkillSelectionService _service;

        [Header("UI References (Level 1)")]
        [SerializeField] private Button _level1UpButton;
        [SerializeField] private Button _level1DownButton;
        [SerializeField] private Image _level1Icon;
        [SerializeField] private TextMeshProUGUI _level1Name;

        [Header("UI References (Level 2)")]
        [SerializeField] private Button _level2UpButton;
        [SerializeField] private Button _level2DownButton;
        [SerializeField] private Image _level2Icon;
        [SerializeField] private TextMeshProUGUI _level2Name;

        [Header("UI References (Level 2)")]
        [SerializeField] private Button _level3UpButton;
        [SerializeField] private Button _level3DownButton;
        [SerializeField] private Image _level3Icon;
        [SerializeField] private TextMeshProUGUI _level3Name;
        
        [Header("Common Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _backButton;

        // スキルリストのキャッシュ
        private List<Skill> _level1Skills;
        private List<Skill> _level2Skills;
        private List<Skill> _level3Skills;

        // 現在選択中のインデックス
        private int _level1Index = 0;
        private int _level2Index = 0;
        private int _level3Index = 0;

        [Inject]
        public void Construct(SkillSelectionService service)
        {
            _service = service;
        }

        private void Start()
        {
            // 1. スキルを読み込む
            _level1Skills = _service.GetSkillsForLevel(DeckLevel.Level1);
            _level2Skills = _service.GetSkillsForLevel(DeckLevel.Level2);
            _level3Skills = _service.GetSkillsForLevel(DeckLevel.Level3);
            
            // (もしスキルが1つもなければエラー処理)
            if (_level1Skills.Count == 0 || _level2Skills.Count == 0 || _level3Skills.Count == 0)
            {
                Debug.LogError("Skills not found in Resources. Make sure SOs exist.");
                return;
            }

            // 2. UIの初期表示
            UpdateSkillUI(DeckLevel.Level1);
            UpdateSkillUI(DeckLevel.Level2);
            UpdateSkillUI(DeckLevel.Level3);

            // 3. ボタンのリスナーを登録
            _level1UpButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level1, 1));
            _level1DownButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level1, -1));
            // (Lvl 2, Lvl 3 も同様に)
            _level2UpButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level2, 1));
            _level2DownButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level2, -1));
            _level3UpButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level3, 1));
            _level3DownButton.onClick.AddListener(() => ChangeSkill(DeckLevel.Level3, -1));

            _confirmButton.onClick.AddListener(OnConfirm);
            _backButton.onClick.AddListener(OnBack);
        }

        // スロットのスキルを切り替える
        private void ChangeSkill(DeckLevel level, int direction)
        {
            switch (level)
            {
                case DeckLevel.Level1:
                    _level1Index = GetWrappedIndex(_level1Index + direction, _level1Skills.Count);
                    break;
                case DeckLevel.Level2:
                    _level2Index = GetWrappedIndex(_level2Index + direction, _level2Skills.Count);
                    break;
                case DeckLevel.Level3:
                    _level3Index = GetWrappedIndex(_level3Index + direction, _level3Skills.Count);
                    break;
            }
            UpdateSkillUI(level);
        }

        // スロットのUIを更新する
        private void UpdateSkillUI(DeckLevel level)
        {
            switch (level)
            {
                case DeckLevel.Level1:
                    Skill s1 = _level1Skills[_level1Index];
                    _level1Icon.sprite = s1.Icon;
                    _level1Name.text = s1.SkillName;
                    break;
                // (Lvl 2, Lvl 3 も同様に)
                case DeckLevel.Level2:
                    Skill s2 = _level2Skills[_level2Index];
                    _level2Icon.sprite = s2.Icon;
                    _level2Name.text = s2.SkillName;
                    break;
                case DeckLevel.Level3:
                    Skill s3 = _level3Skills[_level3Index];
                    _level3Icon.sprite = s3.Icon;
                    _level3Name.text = s3.SkillName;
                    break;
            }
        }

        // 決定ボタン処理
        private void OnConfirm()
        {
            // 現在選択中のスキルを取得
            Skill s1 = _level1Skills[_level1Index];
            Skill s2 = _level2Skills[_level2Index];
            Skill s3 = _level3Skills[_level3Index];
            
            // Application Serviceを呼び出す
            _service.ConfirmSelection(s1, s2, s3);
        }

        // 戻るボタン処理
        private void OnBack()
        {
            _service.GoBackToTitle();
        }

        // インデックスがリストの範囲をループするように
        private int GetWrappedIndex(int index, int count)
        {
            if (count == 0) return 0;
            if (index < 0) return count - 1;
            if (index >= count) return 0;
            return index;
        }
    }
}