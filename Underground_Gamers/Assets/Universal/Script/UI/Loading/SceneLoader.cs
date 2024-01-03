using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingUI; // �ε� UI(���α׷��� �� ��)�� ��� �ִ� ���� ������Ʈ
    public Image progressBar1;
    public Image progressBar2;
    public string sceneToLoad; // �ε��� ���� �̸�
    private AsyncOperation asyncLoad; // �񵿱� �� �ε带 ���� AsyncOperation ����
    private float minLoadingTime = 3.0f; // �ּҷ� �ε�â�� ������ �ð�

    private float loadingTimer = 0.0f; // �ε� ȭ���� ������ �ð��� �����ϴ� Ÿ�̸�
    private bool isSceneLoad = false;

    public Image background;
    public TextMeshProUGUI tipText;

    public Sprite[] backgrounds = new Sprite[5];
    private string[] tips = new string[10];

    private void Awake()
    {
        tips[0] = "tip1";
        tips[1] = "tip2";
        tips[2] = "tip3";
        tips[3] = "tip4";
        tips[4] = "tip5";
        tips[5] = "tip6";
        tips[6] = "tip7";
        tips[7] = "tip8";
        tips[8] = "tip9";
        tips[9] = "tip10";
    }


    public void SceneLoad(string sceneName)
    {
        int randBackground = Random.Range(0, backgrounds.Length);
        int randTip = Random.Range(0, tips.Length);

        background.sprite = backgrounds[randBackground];
        tipText.text = DataTableManager.instance.Get<StringTable>(DataType.String).Get(tips[randTip]);
        // �ε� UI�� ��Ȱ��ȭ
        loadingUI.SetActive(false);

        // �񵿱������� �� �ε� ����
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // �� �ڵ����� Ȱ��ȭ���� �ʵ��� ����
        isSceneLoad = true;
    }

    void Update()
    {
        if(isSceneLoad)
        {
            // �ε� UI�� Ȱ��ȭ�Ͽ� �ε� ȭ�� ǥ��
            loadingUI.SetActive(true);

            float progress = Mathf.Clamp01(loadingTimer / minLoadingTime);

            progressBar1.fillAmount = progress;
            progressBar2.fillAmount = progress;

            loadingTimer += Time.unscaledDeltaTime;


            if ((loadingTimer >= minLoadingTime && asyncLoad.progress >= 0.9f) || asyncLoad.allowSceneActivation)
            {
                asyncLoad.allowSceneActivation = true; // �� Ȱ��ȭ ���
            }
        }
    }
}
