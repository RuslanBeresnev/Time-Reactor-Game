using UnityEngine;

/// <summary>
/// ���������� Sci-Fi �������, �������������� ����� ����. ������ ��������� �����-���� � ����������� ���������,
/// � ����� ������ ��������� ������� ����� ��� ������ � ��� ����� (�������� �������� ������ ������������
/// ��� ����� �����). ��� ������ ������ ����������� ���������� ���. ���� ��� ������������ � ����������� ������, �������
/// �������� ����� � ����� ������� �������� ��������.
/// </summary>
public class RapidTurretRotation : MonoBehaviour
{
    // �������� �������� ������ ��� X � ��������� �����
    [SerializeField] private float minXAxisEulerAngle = 0f;
    [SerializeField] private float maxXAxisEulerAngle = 30f;
    // �������� �������� ������ ��� Y � ��������� �����
    [SerializeField] private float minYAxisEulerAngle = -60f;
    [SerializeField] private float maxYAxisEulerAngle = 60f;

    // ��������� ���������� �������� �� ��� X (�����-����), ������� ������������ ������ �� ���� �������
    [SerializeField] private float xAxisRotationSpeed = 900f;
    // ����� � �������� ��� �������� �� ��� Y (�����-������) ����� ������ �������� �������� �� ��� X (�����-����)
    [SerializeField] private float yAxisRotationShift = 5f;

    [SerializeField] private RapidTurretAttack attackComponent;
    [SerializeField] private Transform emitter;
    [SerializeField] private string targetName;

    // ����� � ������ ������ �������
    private float time = 0f;
    private float nextShiftTime = 0f;
    // ����������� ����� � �������� ��� �������� �� ��� Y
    private float accumulatedShiftByYAxis = 0f;
    // ����� ����� �������� �������� �� ��� X (� �������� ��������� ���������)
    private float xAxisFullTurnTime;

    private Transform target;

    private void Start()
    {
        target = GameObject.Find(targetName).transform;
        xAxisFullTurnTime = 2 * (maxXAxisEulerAngle - minXAxisEulerAngle) / xAxisRotationSpeed / TimeScale.SharedInstance.Scale;
    }

    private void Update()
    {
        if (attackComponent.TargetDetected)
        {
            TurnTowardsTarget();
            return;
        }

        PerformXAxisRotation();
        PerformYAxisRotation();
        time += Time.deltaTime * TimeScale.SharedInstance.Scale;
    }

    /// <summary>
    /// ������������ ���������� �������� �� ��� X (�����-����) � ������� ������� Math.PingPong
    /// </summary>
    private void PerformXAxisRotation()
    {
        var currentXAngle = Mathf.PingPong(time * xAxisRotationSpeed,
            maxXAxisEulerAngle - minXAxisEulerAngle) + minXAxisEulerAngle;
        transform.rotation = Quaternion.Euler(currentXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// ������������ ��������� ������ �� ��� Y (����� ��� ������) ����� ������ �������� �������� �� ��� X
    /// </summary>
    private void PerformYAxisRotation()
    {
        if (time > nextShiftTime)
        {
            nextShiftTime += xAxisFullTurnTime;
            accumulatedShiftByYAxis += yAxisRotationShift;
            var currentYAngle = Mathf.PingPong(accumulatedShiftByYAxis, maxYAxisEulerAngle - minYAxisEulerAngle) + minYAxisEulerAngle;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentYAngle, transform.localEulerAngles.z);
        }
    }

    /// <summary>
    /// ������������ �������� ����� �������, ����� ������ ������ ���� �������� � ����
    /// </summary>
    private void TurnTowardsTarget()
    {
        var oldYAxisEulerAngle = transform.localEulerAngles.y;

        var vectorTowardsTarget = (target.position - emitter.position).normalized;
        var newRotation = Quaternion.LookRotation(vectorTowardsTarget, Vector3.up);
        transform.rotation = newRotation;

        // ����� ��� ����, ����� ����� ������ ���� �� ���� ������ ���������� ������ �������� ������ ��� Y � ���� �� �����,
        // � �� � �����, � ������� ���� �������� ���� (����, ������ ����������, ����� ����������� �������������,
        // �������� ������ ����������� ������ ��� Y)
        accumulatedShiftByYAxis += transform.localEulerAngles.y - oldYAxisEulerAngle;
    }
}