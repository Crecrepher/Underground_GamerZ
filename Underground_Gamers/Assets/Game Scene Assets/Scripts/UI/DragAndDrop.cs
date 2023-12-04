using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Vector3 prevPos;
    private CommandManager commandManager;
    public bool isDragging;

    private void Awake()
    {
        commandManager = GameObject.FindGameObjectWithTag("CommandManager").GetComponent<CommandManager>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        prevPos = transform.position;
        transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //transform.position = prevPos;
        // ��ӵ� ��ġ�� UI ��� ���� ��������
        GameObject droppedObject = eventData.pointerDrag;
        GameObject droppedOnObject = eventData.pointerCurrentRaycast.gameObject;

        // ��ӵ� ��ġ�� �ִ� UI ��Ұ� �г����� Ȯ��
        if (droppedOnObject != null && droppedOnObject.GetComponent<RectTransform>() != null)
        {
            // �ش� UI ��Ұ� �г��̶�� ó���� ���� �߰�
            Debug.Log($"Drop : {droppedObject.name}");
            Debug.Log($"Drop On : {droppedOnObject.name}");
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // �гο� ���� �߰� ���� ����
        }
        else
        {
            Debug.Log("��ӵ� ��ġ�� �г��� �ƴմϴ�.");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        transform.position = prevPos;

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        //prevPos = transform.position;
        //transform.position = eventData.position;
    }
}
