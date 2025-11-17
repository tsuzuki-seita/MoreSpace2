using MoreSpace.Domain;
using MoreSpace.Presentation; // IngameSceneManager を使うため
using UnityEngine;

namespace MoreSpace.Presentation
{
    /// <summary>
    /// Ingameシーン開始時に、スキル選択シーンからのデータを
    /// 受け取り、プレイヤーにスキルをセットアップするクラス。
    /// Ingameシーンの適当なGameObjectにアタッチします。
    /// </summary>
    public sealed class IngamePlayerSetup : MonoBehaviour
    {
        // IngameSceneManager から受け取ったスキルセット
        private SkillSet _selectedSkillSet;
        private GameObject _playerObject;

        private void Start()
        {
            _playerObject = this.gameObject;
            // 1. スキル選択シーンからのデータを取得
            if (IngameSceneManager.Instance != null && 
                IngameSceneManager.Instance.TryConsume<StartIngameArgs>(out var args))
            {
                // SceneArgsBus から StartIngameArgs を取り出すことに成功
                _selectedSkillSet = args.SelectedSkills;
                
                // 2. スキルのInitializeを実行
                SetupSkills();
            }
            else
            {
                // スキルデータが渡されなかった場合 (デバッグ実行など)
                Debug.LogWarning("スキル選択データ (StartIngameArgs) が見つかりませんでした。");
                // (ここでデフォルトスキルをロードするなどのフォールバック処理も可能)
            }
        }

        private void SetupSkills()
        {
            if (_selectedSkillSet == null) return;

            // 3つのスキルを取得
            Skill s1 = _selectedSkillSet.Level1Skill;
            Skill s2 = _selectedSkillSet.Level2Skill;
            Skill s3 = _selectedSkillSet.Level3Skill;

            // 各スキルのInitializeを実行し、プレイヤーオブジェクトを渡す
            if (s1 != null)
            {
                s1.Initialize(_playerObject);
                Debug.Log($"Level 1 Skill [{s1.Type}] Initialized.");
            }
            
            if (s2 != null)
            {
                s2.Initialize(_playerObject);
                Debug.Log($"Level 2 Skill [{s2.Type}] Initialized.");
            }

            if (s3 != null)
            {
                s3.Initialize(_playerObject);
                Debug.Log($"Level 3 Skill [{s3.Type}] Initialized.");
            }

            // 4. (参考) Actメソッドの登録
            // ここでプレイヤーの入力やボタンとActメソッドを紐付けます。
            // (これはあくまで一例です。実際の入力システムに合わせてください)
            
            // PlayerInputController (仮) のようなコンポーネントを取得し、
            // Action (C# Event) に Act デリゲートを登録するイメージ
            
            // var inputController = _playerObject.GetComponent<PlayerInputController>();
            // if (inputController != null)
            // {
            //     if (s1 is ActiveSkill as1) inputController.OnSkill1Pressed += () => as1.Act(_playerObject);
            //     if (s2 is ActiveSkill as2) inputController.OnSkill2Pressed += () => as2.Act(_playerObject);
            //     if (s3 is ActiveSkill as3) inputController.OnSkill3Pressed += () => as3.Act(_playerObject);
            // }
        }
    }
}
