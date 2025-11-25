using MoreSpace.InGame;
using MoreSpace.InGame.Master;
using MoreSpace.InGame.Player;
using MoreSpace.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace MoreSpace.Tutorial
{
    /// <summary>
    /// チュートリアル全体を管理し、
    /// トリガーから呼ばれたときにポップアップ表示＆一時停止を行う。
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TutorialPopupUI popupUI;
        [SerializeField] private Text stepText;
        [SerializeField] private GameObject dummyCrystal;
        [SerializeField] private HealthBase crystalHealth;
        [SerializeField] private HealthBase enemyHealth;

        private bool _isPopupOpen = false;

        private void Start()
        {
            //初期化
            stepText.text = "";
            crystalHealth.gameObject.SetActive(false);
            enemyHealth.gameObject.SetActive(false);
            ResumeGame();
            ChangeStep(TutorialStepType.MoveAround);
        }

        public void ChangeStep(TutorialStepType stepType)
        {
            if (_isPopupOpen) return; // 既にポップアップが出ていたら無視

            (string,string) message = GetMessageForPopup(stepType);

            _isPopupOpen = true;
            PauseGame();

            popupUI.Show(message.Item1,message.Item2, () =>
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
        private (string,string) GetMessageForPopup(TutorialStepType stepType)
        {
            switch (stepType)
            {
                case TutorialStepType.MoveAround:
                    return
                        ("機体制御訓練を開始せよ" ,
                        "こちら司令部。新任パイロット、聞こえているか？\n" +
                        "まずはこの機体の制御に慣れてもらおう。\n" +
                        "移動操作で動き回り、カメラを動かして感覚をつかめ。\n" +
                        "今は偵察フェーズだ。惑星のクリスタルに接近してくれ。");

                case TutorialStepType.ShootCrystal:
                    return
                        ("クリスタルの性質を把握せよ" ,
                        "パイロット、視界にある発光体が〈クリスタル〉だ。\n" +
                        "撃てば銃声と光で“場所がバレる”が、破壊に成功すれば\n" +
                        "新たなスキルを獲得できる。\n" +
                        "リスクとリターンを見極めるのが腕の見せどころだ。\n" +
                        "まずはクリスタルを1つ破壊してスキル入手を確認せよ。");

                case TutorialStepType.AttackEnemy:
                    return
                        ("敵機への攻撃テストを実施せよ" ,
                        "連射弾が解放された。マウスホイールで切り替えられる。\n" +
                        "次の任務だ。演習用の敵機を視認し、攻撃を命中させろ。\n" +
                        "敵機が損傷を受ける様子を把握するのだ。\n" +
                        "ある程度HPを削れれば任務達成とする。\n" +
                        "落ち着いて狙い、確実に命中させろ。");

                case TutorialStepType.FinalTest:
                    // 「最後に実践に挑戦しよう」ポジションの司令
                    return
                        ("訓練は十分だ、パイロット" ,
                        "これで訓練は終了とする。次からは本当の戦場だ。\n" +
                        "クリスタルを4つ破壊、もしくは敵機を撃墜させることで\n" +
                        "その戦場の勝者となれる。\n" +
                        "ここで学んだ全てを使って戦い、勝利を自分の手で掴め。");

                default:
                    return (string.Empty,string.Empty);
            }
        }

        private void PauseGame()
        {
            Time.timeScale = 0f;
            StartGameWithCountDown.ForceSetGamePlayableState(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void ResumeGame()
        {
            Time.timeScale = 1f;
            StartGameWithCountDown.ForceSetGamePlayableState(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// 各ステップ開始時に一度だけ行いたい処理があればここに書く
        /// （誘導マーカー表示、ログ送信など）
        /// </summary>
        private void StartStepLogic(TutorialStepType stepType)
        {
            stepText.text = GetMessageForStep(stepType);
            switch (stepType)
            {
                case TutorialStepType.MoveAround:
                    // 例：一定時間移動したら次のトリガーを有効化…など
                    break;

                case TutorialStepType.ShootCrystal:
                    dummyCrystal.SetActive(false);
                    crystalHealth.gameObject.SetActive(true);
                    crystalHealth.OnHpZero += () =>
                    {
                        ChangeStep(TutorialStepType.AttackEnemy);
                    };
                    break;
                case TutorialStepType.AttackEnemy:
                    enemyHealth.gameObject.SetActive(true);
                    enemyHealth.OnDamage += (hp, maxHp) =>
                    {
                        if(hp < 30)
                            ChangeStep(TutorialStepType.FinalTest);
                    };
                    break;

                case TutorialStepType.FinalTest:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    IngameSceneManager.Instance.ChangeScene(InGameState.Result, new ResultArgs(ResultPattern.Finish));
                    break;
            }
        }
        
        private string GetMessageForStep(TutorialStepType stepType)
        {
            switch (stepType)
            {
                case TutorialStepType.MoveAround:
                    return "動きを確認しながらクリスタルに近づいてください。\n" +
                           "マウスで方向指定、ADで旋回することができます。";

                case TutorialStepType.ShootCrystal:
                    return "左クリックで目の前のクリスタルを撃ってみましょう。\n" +
                           "撃つと場所がバレますが、壊すとスキルを獲得できます。";

                case TutorialStepType.AttackEnemy:
                    return "周囲を見渡してハイライトされた敵機を探しましょう。\n" +
                           "敵機に攻撃を当ててHPを減らしてみましょう。";

                default:
                    return "";
            }
        }
    }
    public enum TutorialStepType
    {
        MoveAround,     // 動き回る
        ShootCrystal,   // クリスタルを撃つ・壊す
        AttackEnemy,    // 敵にダメージを与える（撃破までは不要）
        FinalTest       // 最後の実戦訓練（任意で使用）
    }
}
