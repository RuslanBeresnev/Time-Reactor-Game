using UnityEngine;

/// <summary>
/// Реализация Sci-Fi туррели, осуществляющей поиск цели. Турель вращается вверх-вниз в определённом диапазоне,
/// а затем делает небольшой поворот влево или вправо и так далее (диапазон вращения вокруг вертикальной
/// оси также задан). При поиске игрока испускается прицельный луч. Если луч пересекается с коллайдером игрока, туррель
/// начинает огонь с очень большой частотой стрельбы.
/// </summary>
public class RapidTurretRotation : MonoBehaviour
{
    // Диапазон вращения вокруг оси X в эйлеровых углах
    [SerializeField] private float minXAxisEulerAngle = 0f;
    [SerializeField] private float maxXAxisEulerAngle = 30f;
    // Диапазон вращения вокруг оси Y в эйлеровых углах
    [SerializeField] private float minYAxisEulerAngle = -60f;
    [SerializeField] private float maxYAxisEulerAngle = 60f;

    // Суммарное количество градусов по оси X (вверх-вниз), которое провращается турель за одну секунду
    [SerializeField] private float xAxisRotationSpeed = 900f;
    // Сдвиг в градусах при вращении по оси Y (влево-вправо) после каждой итерации вращения по оси X (вверх-вниз)
    [SerializeField] private float yAxisRotationShift = 5f;

    [SerializeField] private RapidTurretAttack attackComponent;
    [SerializeField] private Transform emitter;
    [SerializeField] private string targetName;

    // Время с начала работы скрипта
    private float time = 0f;
    private float nextShiftTime = 0f;
    // Накопленный сдвиг в градусах при вращении по оси Y
    private float accumulatedShiftByYAxis = 0f;
    // Время одной итерации вращения по оси X (в пределах заданного диапазона)
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
    /// Осуществлять постоянное вращение по оси X (вверх-вниз) с помощью функции Math.PingPong
    /// </summary>
    private void PerformXAxisRotation()
    {
        var currentXAngle = Mathf.PingPong(time * xAxisRotationSpeed,
            maxXAxisEulerAngle - minXAxisEulerAngle) + minXAxisEulerAngle;
        transform.rotation = Quaternion.Euler(currentXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    /// <summary>
    /// Осуществлять небольшие сдвиги по оси Y (влево или вправо) после каждой итерации вращения по оси X
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
    /// Осуществлять вращение таким образом, чтобы турель всегда была повёрнута к цели
    /// </summary>
    private void TurnTowardsTarget()
    {
        var oldYAxisEulerAngle = transform.localEulerAngles.y;

        var vectorTowardsTarget = (target.position - emitter.position).normalized;
        var newRotation = Quaternion.LookRotation(vectorTowardsTarget, Vector3.up);
        transform.rotation = newRotation;

        // Нужно для того, чтобы после потери цели из виду турель продолжила делать повороты вокруг оси Y с того же места,
        // а не с места, в котором была замечена цель (цель, будучи замеченной, могла значительно передвинуться,
        // заставив турель повернуться вокруг оси Y)
        accumulatedShiftByYAxis += transform.localEulerAngles.y - oldYAxisEulerAngle;
    }
}