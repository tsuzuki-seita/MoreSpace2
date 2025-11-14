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

        var targetRot = Quaternion.LookRotation(delta, Vector3.up);
        var offsetTargetRot = targetRot * Quaternion.Euler(90, 0, 0);
        _transform.rotation = offsetTargetRot;
    }
}
