using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NameSetter : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public int StartNum;

    //public void Awake()
    //{
    //    // ���� ���� ��� �� ���ϸ�
    //    foreach (GameObject go in gameObjects)
    //    {
    //        string oldPath = AssetDatabase.GetAssetPath(go);
    //        string oldFileName = Path.GetFileNameWithoutExtension(oldPath);

    //        // ���ο� ���ϸ�
    //        string newFileName = $"{StartNum++}"; // ���ϴ� ���ο� ���ϸ����� ����

    //        // ������ ����
    //        string newPath = oldPath.Replace(oldFileName, newFileName);
    //        AssetDatabase.CopyAsset(oldPath, newPath);

    //        // ���� ������ ����
    //        AssetDatabase.DeleteAsset(oldPath);
    //    }

    //}
}
