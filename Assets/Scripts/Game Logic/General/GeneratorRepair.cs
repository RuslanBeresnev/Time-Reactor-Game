using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// ���������� ��������� ������� ���������� �������� � ������� ������� �� ��������, � ������� �����
/// ������ �������� � �������� ������� E
/// </summary>
public class GeneratorRepair : MonoBehaviour
{
    private static int repairedGeneratorsCount = 0;

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
    /// ����� ���������� �����������, ������� ��������� ��������
    /// </summary>
    public static int NeedToRepairGeneratorsCount { get; } = 6;

    /// <summary>
    /// ���������� ���������� ������� �����������
    /// </summary>
    public static int RepairedGeneratorsCount
    {
        get => repairedGeneratorsCount;
        set
        {
            repairedGeneratorsCount = value;
            if (RepairedGeneratorsCount == NeedToRepairGeneratorsCount)
            {
                OnAllGeneratorsHaveBeenRepaired();
            }
        }
    }

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
    /// ��������� ������� ��� ���������� ������� ����������
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
            OnGeneratorRepairComplition();
        }
    }

    /// <summary>
    /// �������� ����� ������ ������� ����������
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
    /// ������������ ����� ������������ ����� � ���������� ��������� � �����
    /// </summary>
    private void RedrawCircleUI()
    {
        whiteCircle.fillAmount = degreeOfRepair;
        percentage.text = ((int)(degreeOfRepair * 100)).ToString() + "%";
    }

    /// <summary>
    /// ������� ��������� ������� ����� �� ���������� ���������� ������ ����� ���������� �������
    /// </summary>
    private IEnumerator DisableSideTextAfterFewSeconds()
    {
        yield return new WaitForSeconds(2f);
        sideText.gameObject.SetActive(false);
    }

    /// <summary>
    /// �������� ��� ���������� �������� ������� ����������
    /// </summary>
    private void OnGeneratorRepairComplition()
    {
        repairCompleted = true;
        canPerformRepair = false;
        RepairedGeneratorsCount += 1;

        repairedScreen.SetActive(true);
        whiteCircle.color = Color.green;
        percentage.color = Color.green;
        sideText.text = "Repair completed";
        StartCoroutine(DisableSideTextAfterFewSeconds());
    }

    /// <summary>
    /// �������� ����� ������� ���� ����������� ��������
    /// </summary>
    private static void OnAllGeneratorsHaveBeenRepaired()
    {
        SceneManager.LoadScene("Win Menu");
        Cursor.lockState = CursorLockMode.Confined;

        GameProperties.ResetStatistics();
    }
}