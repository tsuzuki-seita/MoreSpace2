using UnityEngine;

public class RotateToMovementDirectionSmooth : MonoBehaviour
{
    // 最大の回転角速度[deg/s]
    [SerializeField] private float _maxAngularSpeed = Mathf.Infinity;
    
    // 進行方向に向くのにかかるおおよその時間[s]
    [SerializeField] private float _smoothTime = 0.1f;

    private Transform _transform;

    // 前フレームのワールド位置
    private Vector3 _prevPosition;

    private float _currentAngularVelocity;

    private void Start()
    {
        _transform = transform;

        _prevPosition = _transform.position;
    }

    private void Update()
    {
        var position = _transform.position;
        var delta = position - _prevPosition;
        _prevPosition = position;

        if (delta == Vector3.zero)
            return;

        // 1. 進行方向（移動量ベクトル）に向くようなクォータニオンを取得
        var targetRot = Quaternion.LookRotation(delta, Vector3.up);

        // 2. [変更] 進行方向に対して、ローカルX軸で+90度のオフセットを加えた回転を「最終目標」とする
        var offsetTargetRot = targetRot * Quaternion.Euler(90, 0, 0);

        // 3. [変更] 現在の向きと「最終目標の向き(オフセット後)」との角度差を計算
        //    ( Quaternion.Angle は2つの回転間の最短角度を返します )
        var diffAngle = Quaternion.Angle(_transform.rotation, offsetTargetRot);
        
        // 4. 現在フレームで回転する角度の計算
        //    ( 角度差(diffAngle) が 0 になるように )
        var rotAngle = Mathf.SmoothDampAngle(
            diffAngle, // 現在の角度差
            0,         // 目標の角度差
            ref _currentAngularVelocity,
            _smoothTime,
            _maxAngularSpeed
        );

        // 5. [変更] 現在フレームにおける回転を計算
        //    ( 現在の向きから「最終目標の向き(offsetTargetRot)」に向かって回転 )
        //    ( スムージング計算の結果、現在の角度差(diffAngle)から (diffAngle - rotAngle) 分だけ回転する )
        var nextRot = Quaternion.RotateTowards(
            _transform.rotation,
            offsetTargetRot,
            diffAngle - rotAngle // SmoothDampAngleは残りの角度を返すので、進むべき角度は(全体 - 残り)
        );

        // 6. オブジェクトの回転に反映
        _transform.rotation = nextRot;
    }
}
