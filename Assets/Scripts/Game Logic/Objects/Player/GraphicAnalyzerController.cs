using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Реализация графического анализатора, который игрок может надевать и снимать
/// С его помощью можно видеть параметры оружия, предметов, а также
/// дополнительную статистику на панели игрока
/// </summary>
public class GraphicAnalyzerController : MonoBehaviour
{
    [SerializeField] private GameObject analyzerInfoPanel;

    // Максимальная дистанция, на которой игрок может взаимодействовать с объектами
    [SerializeField] private float interactionDistance;
    // Минимальное время между сменой режимов активировано/неактивно
    [SerializeField] private float intervalBetweenStateChanging;

    // Коэффициент от ширины/высоты панели, который умножается на ширину/высоту, и смещает панель таким образом,
    // чтобы она не загораживала объект
    [SerializeField] private float panelOffsetCoefficientInPlane;
    // Коэффициент расстояния между предметом и игроком, который определяет, насколько близко панель будет к игроку
    // Должен быть от 0f до 1f
    [SerializeField] private float panelOffsetCoefficientToPlayer;

    private static bool analyzerIsActive = false;
    private bool stateChangingIsAllowed = true;

    private ObjectWithInformation objectPlayerCurrentlyLookingAt = null;

    private AudioSource activationSound;
    private AudioSource deactivationSound;

    public void FixedUpdate()
    {
        CheckStateChanging();
        HideInformationIfPlayerStoppedLookingAtObject();
        DisplayInformationIfPlayerLookingAtObject();
    }

    /// <summary>
    /// Активен ли графический анализатор в данный момент
    /// </summary>
    public static bool AnalyzerIsActive
    {
        get { return analyzerIsActive; }
        set
        {
            analyzerIsActive = value;
            if (StateChanged != null)
            {
                StateChanged(analyzerIsActive);
            }
        }
    }

    /// <summary>
    /// Событие, которое вызывается при активации/деактивации графического анализатора
    /// </summary>
    public static Action<bool> StateChanged { get; set; }

    private void Awake()
    {
        foreach (var audioSource in GetComponents<AudioSource>())
        {
            var clipName = audioSource.clip.name;
            if (clipName == "Activation")
            {
                activationSound = audioSource;
            }
            else if (clipName == "Deactivation")
            {
                deactivationSound = audioSource;
            }
        }
    }

    /// <summary>
    /// Проверка на нажатие кнопки для смены режима
    /// </summary>
    private void CheckStateChanging()
    {
        if (Input.GetKey(KeyCode.G))
        {
            if (stateChangingIsAllowed)
            {
                if (!AnalyzerIsActive)
                {
                    activationSound.Play();
                }
                else
                {
                    deactivationSound.Play();
                }

                AnalyzerIsActive = !AnalyzerIsActive;
                analyzerInfoPanel.SetActive(AnalyzerIsActive);
                if (!AnalyzerIsActive)
                {
                    if (objectPlayerCurrentlyLookingAt != null)
                    {
                        objectPlayerCurrentlyLookingAt.HideInfoPanel();
                        objectPlayerCurrentlyLookingAt = null;
                    }
                }

                stateChangingIsAllowed = false;
                StartCoroutine(AllowStateChangingAfterIntervalPassing());
            }
        }
    }

    /// <summary>
    /// Снова разрешить изменить режим после прошествия интервала времени
    /// </summary>
    private IEnumerator AllowStateChangingAfterIntervalPassing()
    {
        yield return new WaitForSeconds(intervalBetweenStateChanging);
        stateChangingIsAllowed = true;
    }

    /// <summary>
    /// Получить ближайший объект, на который смотрит игрок в данный момент, если он находится на досягаемой дистанции,
    /// а также точку попадения луча в объект
    /// </summary>
    public (GameObject, Vector3) GetObjectPlayerIsLookingAt()
    {
        Ray rayToScreenCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.Log(rayToScreenCenter);
        RaycastHit hit;
        int defaultLayerMask = 1;

        if (Physics.Raycast(rayToScreenCenter, out hit, interactionDistance, defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            return (hit.transform.gameObject, hit.point);
        }
        return (null, Vector3.zero);
    }

    /// <summary>
    /// Задать параметры компонента Trasform у информационной панели так, чтобы она была хорошо видна игроку
    /// </summary>
    /// <param name="hitPoint">Точка, в которой луч попал в объект, для которого нужно отобразить информацию</param>
    private void SetTransformOfInfoPanel(Vector3 hitPoint)
    {
        var panelRect = objectPlayerCurrentlyLookingAt.GetInfoPanel().GetComponent<RectTransform>();
        var panelWidth = panelRect.rect.width * panelRect.localScale.x;
        var panelHeight = panelRect.rect.height * panelRect.localScale.y;

        Vector3 setterPosition = hitPoint;
        setterPosition -= Camera.main.transform.right * (panelWidth * panelOffsetCoefficientInPlane);
        setterPosition += Camera.main.transform.up * (panelHeight * panelOffsetCoefficientInPlane);
        setterPosition -= Camera.main.transform.forward * (Vector3.Distance(hitPoint, Camera.main.transform.position) * panelOffsetCoefficientToPlayer);
        objectPlayerCurrentlyLookingAt.SetInfoPanelTransform(setterPosition, Camera.main.transform.rotation, panelRect.localScale);
    }

    /// <summary>
    /// Отобразить информацию об объекте, на который в данный момент смотрит игрок, если объект
    /// является наследником ObjectWithInformation
    /// </summary>
    private void DisplayInformationIfPlayerLookingAtObject()
    {
        if (!AnalyzerIsActive)
        {
            return;
        }

        (var objectAhead, var hitPoint) = GetObjectPlayerIsLookingAt();
        if (objectAhead != null && objectAhead.GetComponent<ObjectWithInformation>() != null)
        {
            objectPlayerCurrentlyLookingAt = objectAhead.GetComponent<ObjectWithInformation>();
            objectPlayerCurrentlyLookingAt.ShowInfoPanel();
            SetTransformOfInfoPanel(hitPoint);
        }
    }

    /// <summary>
    /// Скрыть информацию об объекте, на который только что смотрел игрок, если объект
    /// является наследником ObjectWithInformation
    /// </summary>
    private void HideInformationIfPlayerStoppedLookingAtObject()
    {
        // Если игрок перевёл взгляд на другой объект
        if (objectPlayerCurrentlyLookingAt != null && GetObjectPlayerIsLookingAt().Item1 != objectPlayerCurrentlyLookingAt)
        {
            objectPlayerCurrentlyLookingAt.HideInfoPanel();
        }
    }
}