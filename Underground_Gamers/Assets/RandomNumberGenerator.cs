using UnityEngine;

public class RandomNumberGenerator : MonoBehaviour
{
    // ����� 0�̰� ǥ�� ������ 1�� ���� ������ ������ ������ ����
    float damage;
    float acc;
    float GenerateRandomNumber()
    {
        float u1 = 1f - Random.Range(0f, 1f);
        float u2 = 1f - Random.Range(0f, 1f);

        float randStdNormal = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        // ����� 0, ǥ�� ������ 1�� ���� ������ ��ȯ
        return randStdNormal;
    }

    void Start()
    {
        damage = 100f;
        acc = 40f;
        

        // ���� ������ ������ ����
        for (int i = 0; i < 20; ++i)
        {
            float randomValue = GenerateRandomNumber();
            damage *= (1.1f - Mathf.Abs((randomValue * 50.0f / acc)) * 0.2f);
            damage = 100f;
        }
    }
}