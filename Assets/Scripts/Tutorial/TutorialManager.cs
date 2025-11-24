using UnityEngine;

namespace MoreSpace.Tutorial
{
    public enum TutorialStepType
    {
        MoveAround,     // 動き回る
        ShootCrystal,   // クリスタルを撃つ・壊す
        AttackEnemy,    // 敵にダメージを与える（撃破までは不要）
        FinalTest       // 最後の実戦訓練（任意で使用）
    }

    /// <summary>
    /// チュートリアル全体を管理し、
    /// トリガーから呼ばれたときにポップアップ表示＆一時停止を行う。
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TutorialPopupUI popupUI;
        [SerializeField] private MonoBehaviour playerInputRoot;
        // ↑ プレイヤー操作を司るコンポーネント（例: PlayerController）をインスペクタからアサイン

        private bool _isPopupOpen = false;

        private void Start()
        {
            ResumeGame();  // 念のため初期状態は動作状態に
        }

        /// <summary>
        /// トリガーエリアから呼ばれる入口
        /// </summary>
        public void OnStepTriggerEnter(TutorialStepType stepType)
        {
            if (_isPopupOpen) return; // 既にポップアップが出ていたら無視

            string message = GetMessageForStep(stepType);

            _isPopupOpen = true;
            PauseGame();

            popupUI.Show(message, () =>
            {
                // 閉じるボタンが押されたとき
                _isPopupOpen = false;
                ResumeGame();

                // ステップ開始時の処理があればここに
                StartStepLogic(stepType);
            });
        }

        /// <summary>
        /// 各ステップで表示する司令文言
        /// </summary>
        private string GetMessageForStep(TutorialStepType stepType)
        {
            switch (stepType)
            {
                case TutorialStepType.MoveAround:
                    return
                        "【司令】機体制御訓練を開始せよ\n\n" +
                        "こちら司令部。新任パイロット、状況は聞こえているか？\n" +
                        "まずはこの惑星上での機体制御に慣れてもらう。\n" +
                        "移動操作で周囲を走り回り、カメラを動かして地形と感覚をつかめ。\n" +
                        "今は偵察フェーズだ。好きに動き回って、機体のクセを体に叩き込め。";

                case TutorialStepType.ShootCrystal:
                    return
                        "【司令】クリスタルの性質を把握せよ\n\n" +
                        "パイロット、視界にある発光体は〈クリスタル〉だ。\n" +
                        "撃てば銃声と光で“場所がバレる”が、破壊に成功すれば新たなスキルを獲得できる。\n" +
                        "これはこの戦場での重要な資源──リスクとリターンを見極めるのが腕の見せどころだ。\n" +
                        "まずはクリスタルに数発撃ち込み、1つ破壊してスキル入手を確認せよ。";

                case TutorialStepType.AttackEnemy:
                    return
                        "【司令】敵機への攻撃テストを実施せよ\n\n" +
                        "次の任務だ。演習用の敵機を視認し、攻撃を命中させろ。\n" +
                        "HPバーが減る様子をよく観察し、自分の武装がどれだけダメージを与えるか把握せよ。\n" +
                        "これは訓練だ。撃ち落とす必要はない──ある程度HPを削れれば任務達成とする。\n" +
                        "落ち着いて狙い、確実に命中させろ。";

                case TutorialStepType.FinalTest:
                    // 「最後に実践に挑戦しよう」ポジションの司令
                    return
                        "【司令】訓練は十分だ、パイロット。\n\n" +
                        "これより最終テストだ。実戦さながらの状況に飛び込め。\n" +
                        "ここまで学んだすべてを使って戦い、勝ち筋を自分の手で掴め。";

                default:
                    return string.Empty;
            }
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;

            if (playerInputRoot != null)
            {
                playerInputRoot.enabled = false;
            }

            // 必要ならカーソル制御など
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible = true;
        }

        private void ResumeGame()
        {
            Time.timeScale = 1f;

            if (playerInputRoot != null)
            {
                playerInputRoot.enabled = true;
            }

            // ゲーム中のカーソル設定に戻すならここ
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }

        /// <summary>
        /// 各ステップ開始時に一度だけ行いたい処理があればここに書く
        /// （誘導マーカー表示、ログ送信など）
        /// </summary>
        private void StartStepLogic(TutorialStepType stepType)
        {
            switch (stepType)
            {
                case TutorialStepType.MoveAround:
                    // 例：一定時間移動したら次のトリガーを有効化…など
                    break;

                case TutorialStepType.ShootCrystal:
                    // 例：チュートリアル用クリスタル破壊を監視…など
                    break;

                case TutorialStepType.AttackEnemy:
                    // 例：敵へのダメージが入ったら次のステップ解放…など
                    break;

                case TutorialStepType.FinalTest:
                    // 例：本番用1v1に遷移する or 実戦ルールを有効化…など
                    break;
            }
        }

        #region 外部からの完了通知（任意）

        // クリスタルが壊れたときに呼ぶ想定
        public void OnCrystalDestroyedForTutorial()
        {
            // ここで「次のエリアへの道を開ける」「UIにチェックマーク表示」など
        }

        // 敵にダメージが入ったときに呼ぶ想定
        public void OnEnemyDamagedForTutorial()
        {
            // 同上
        }

        #endregion
    }
}
