// Presentation/SkillSelectionPresenter.cs
using System.Collections.Generic;
using MoreSpace.Application;
using MoreSpace.Domain;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使う場合
using VContainer;
using System.Text; // DIコンテナ

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
        [SerializeField] private Text _level1Name;
        [SerializeField] private Button _level1DetailButton;

        [Header("UI References (Level 2)")]
        [SerializeField] private Button _level2UpButton;
        [SerializeField] private Button _level2DownButton;
        [SerializeField] private Image _level2Icon;
        [SerializeField] private Text _level2Name;
        [SerializeField] private Button _level2DetailButton;

        [Header("UI References (Level 3)")]
        [SerializeField] private Button _level3UpButton;
        [SerializeField] private Button _level3DownButton;
        [SerializeField] private Image _level3Icon;
        [SerializeField] private Text _level3Name;
        [SerializeField] private Button _level3DetailButton;
        
        [Header("Common Buttons")]
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _backButton;

        [Header("UI References (Skill Detail Panel)")]
        [SerializeField] private GameObject _detailPanel; // 詳細パネルのルートGameObject
        [SerializeField] private Image _detailIcon;
        [SerializeField] private Text _detailLevel;
        [SerializeField] private Text _detailName;
        [SerializeField] private Text _detailDescription;
        [SerializeField] private Text _detailStats; // スキル固有ステータス (Damage, Valueなど)
        [SerializeField] private Button _detailCloseButton;

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

            _level1DetailButton.onClick.AddListener(() => OpenDetailPanel(DeckLevel.Level1));
            _level2DetailButton.onClick.AddListener(() => OpenDetailPanel(DeckLevel.Level2));
            _level3DetailButton.onClick.AddListener(() => OpenDetailPanel(DeckLevel.Level3));

            _detailCloseButton.onClick.AddListener(CloseDetailPanel);

            _detailPanel.SetActive(false);
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
            SoundManager.Instance.PlaySE(SoundManager.SEData.SETYPE.SelectMove);
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

        /// <summary>
        /// 詳細パネルを開き、指定されたレベルのスキル情報を表示する
        /// </summary>
        private void OpenDetailPanel(DeckLevel level)
        {
            // 1. 現在選択中のスキルを取得
            Skill currentSkill = null;
            switch (level)
            {
                case DeckLevel.Level1:
                    currentSkill = _level1Skills[_level1Index];
                    break;
                case DeckLevel.Level2:
                    currentSkill = _level2Skills[_level2Index];
                    break;
                case DeckLevel.Level3:
                    currentSkill = _level3Skills[_level3Index];
                    break;
            }

            if (currentSkill == null) return;

            // 2. パネルのUIコンポーネントに情報を設定
            _detailIcon.sprite = currentSkill.Icon;
            _detailName.text = currentSkill.SkillName;
            _detailDescription.text = currentSkill.Description;
            _detailLevel.text = currentSkill.Level.ToString();
            
            // 3. スキルの種類に応じて固有のステータスをフォーマット
            _detailStats.text = FormatSkillStats(currentSkill);

            // 4. パネルを表示
            _detailPanel.SetActive(true);
        }

        /// <summary>
        /// スキル詳細パネルを閉じる
        /// </summary>
        private void CloseDetailPanel()
        {
            _detailPanel.SetActive(false);
        }

        /// <summary>
        /// スキル(SO)を受け取り、その種類に応じたステータス文字列を生成する
        /// </summary>
        private string FormatSkillStats(Skill skill)
        {
            // StringBuilderを使って効率的に文字列を結合
            StringBuilder statsBuilder = new StringBuilder();

            // ScriptableObjectの型をチェックして分岐
            if (skill is PassiveSkill passive)
            {
                statsBuilder.AppendLine($"効果値: {passive.Value}");
            }
            else if (skill is WeaponSkill weapon)
            {
                statsBuilder.AppendLine($"ダメージ: {weapon.Damage}");
                statsBuilder.AppendLine($"射程: {weapon.Distance}");
                statsBuilder.AppendLine($"リキャスト: {weapon.RecastTime}秒");
            }
            else if (skill is ActiveSkill buff)
            {
                statsBuilder.AppendLine($"効果時間: {buff.Duration}秒");
                statsBuilder.AppendLine($"リキャスト: {buff.RecastTime}秒");
            }
            // (もし ActiveSkill を直接継承するクラスがあれば、ここに追加)
            else if (skill is ActiveSkill active) // 派生クラス(Weapon, Buff)に一致しなかった場合
            {
                statsBuilder.AppendLine($"リキャスト: {active.RecastTime}秒");
            }

            if (statsBuilder.Length == 0)
            {
                return "固有ステータスなし";
            }

            return statsBuilder.ToString();
        }
    }
}