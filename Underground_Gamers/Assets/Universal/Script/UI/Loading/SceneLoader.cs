using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingUI; // �ε� UI(���α׷��� �� ��)�� ��� �ִ� ���� ������Ʈ
    //public Slider progressBar; // ���α׷��� ��
    public Image progressBar1;
    public Image progressBar2;
    public string sceneToLoad; // �ε��� ���� �̸�
    private AsyncOperation asyncLoad; // �񵿱� �� �ε带 ���� AsyncOperation ����
    private float minLoadingTime = 3.0f; // �ּҷ� �ε�â�� ������ �ð�

    private float loadingTimer = 0.0f; // �ε� ȭ���� ������ �ð��� �����ϴ� Ÿ�̸�
    private bool isSceneLoad = false;

    void Start()
    {
    }

    public void SceneLoad(string sceneName)
    {
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
