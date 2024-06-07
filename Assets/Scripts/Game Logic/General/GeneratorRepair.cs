using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Реализация механизма починки генератора реактора с помощью области из триггера, в которую игрок
/// должен смотреть и зажимать клавишу E
/// </summary>
public class GeneratorRepair : MonoBehaviour
{
    [SerializeField] private string analyzerObjectName;
    [SerializeField] private GameObject repairAreaTrigger;
    [SerializeField] private GameObject repairedScreen;

    [SerializeField] private float timeForRepair = 3f;

    [SerializeField] private Image whiteCircle;
    [SerializeField] private Image whiteCircleBackground;
    [SerializeField] private TextMeshProUGUI percentage;
    [SerializeField] private TextMeshProUGUI sideText;
    [SerializeField] private Image exclamationMark;

    private bool canPerformRepair = false;
    private bool repairWasStarted = false;
    private bool repairCompleted = false;

    private GraphicAnalyzerController analyzer;
    private float degreeOfRepair = 0f;

    /// <summary>
    /// Событие окончания починки генератора
    /// </summary>
    public UnityEvent RepairCompleted = new();

    private void Start()
    {
        analyzer = GameObject.Find(analyzerObjectName).GetComponent<GraphicAnalyzerController>();
    }

    private void Update()
    {
        if (!repairCompleted)
        {
            DetectRepairProcess();
        }
    }

    /// <summary>
    /// Проверить условия для выполнения починки генератора
    /// </summary>
    private void DetectRepairProcess()
    {
        (var objectPlayerIsLookingAt, var _) = analyzer.GetObjectPlayerIsLookingAt();
        canPerformRepair = objectPlayerIsLookingAt == repairAreaTrigger ? true : false;

        if (canPerformRepair)
        {
            sideText.gameObject.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                if (!repairWasStarted)
                {
                    PerformActionsAfterRepairStarting();
                }

                degreeOfRepair += (1 / timeForRepair) * Time.deltaTime;
                degreeOfRepair = Mathf.Clamp(degreeOfRepair, 0f, 1f);
                RedrawCircleUI();
            }
        }
        else
        {
            sideText.gameObject.SetActive(false);
        }

        if (degreeOfRepair == 1f)
        {
            RepairCompleted.Invoke();
        }
    }

    /// <summary>
    /// Действия после начала ремонта генератора
    /// </summary>
    private void PerformActionsAfterRepairStarting()
    {
        repairWasStarted = true;
        exclamationMark.gameObject.SetActive(false);
        whiteCircle.gameObject.SetActive(true);
        whiteCircleBackground.gameObject.SetActive(true);
        percentage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Перерисовать часть заполненного круга и количество процентов в круге
    /// </summary>
    private void RedrawCircleUI()
    {
        whiteCircle.fillAmount = degreeOfRepair;
        percentage.text = ((int)(degreeOfRepair * 100)).ToString() + "%";
    }

    /// <summary>
    /// Сделать невидимым боковой текст по прошествии нескольких секунд после завершения починки
    /// </summary>
    private IEnumerator DisableSideTextAfterFewSeconds()
    {
        yield return new WaitForSeconds(2f);
        sideText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Действия при завершении процесса починки генератора
    /// </summary>
    public void OnRepairComplition()
    {
        repairCompleted = true;
        canPerformRepair = false;
        repairedScreen.SetActive(true);
        whiteCircle.color = Color.green;
        percentage.color = Color.green;
        sideText.text = "Repair completed";
        StartCoroutine(DisableSideTextAfterFewSeconds());
    }
}