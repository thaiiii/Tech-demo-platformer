using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    [Header("Piston Settings")]
    private Transform pistonBody; // Phần động của piston
    private Transform pistonMaxPosition; // Vị trí tối đa
    private Transform pistonMinPosition; // Vị trí tối thiểu
    public float returnTime; // Thời gian piston quay lại Max
    private float returnSpeed; // Vận tốc di chuyển về Max

    [SerializeField] private bool isReturning = false; // Đang quay về Max

    private enum FunctionType { SingleShow, MultipleShow };
    [SerializeField] private FunctionType functionType;

    [Header("Single Object Settings")]
    public GameObject objectToShow; // Object sẽ hiển thị

    [Header("Multiple Objects Settings")]
    public List<ObjectDisplay> objectDisplays; // Danh sách object
    private Coroutine displayCoroutine;

    [System.Serializable]
    public class ObjectDisplay
    {
        public GameObject obj;
        public bool originStatus; // Trạng thái ban đầu (true: hiện, false: ẩn)
        public float delayToShow = 0f; // Delay xuất hiện
        public float activeDuration = 2f; // Thời gian hiện
        public float inactiveDuration = 2f; // Thời gian ẩn
    }

    private List<Coroutine> objectCoroutines = new List<Coroutine>();

    private void Start()
    {
        pistonBody = GetComponent<Transform>();
        pistonMaxPosition = transform.parent.Find("MaxPosition").GetComponent<Transform>();
        pistonMinPosition = transform.parent.Find("MinPosition").GetComponent<Transform>();

        // Tính vận tốc dựa trên khoảng cách giữa Min và Max
        float distance = Vector3.Distance(pistonMinPosition.position, pistonMaxPosition.position);
        returnSpeed = distance / returnTime;
    }

    private void Update()
    {
        if (Vector3.Distance(pistonBody.position, pistonMinPosition.position) < 0.01f)
        {
            ActivateFunction();
        }

        // Nếu piston đang quay về Max, di chuyển nó với vận tốc đã tính
        if (isReturning)
        {
            pistonBody.position = Vector3.MoveTowards(pistonBody.position, pistonMaxPosition.position, returnSpeed * Time.deltaTime);

            // Khi piston chạm Max, dừng di chuyển và reset object
            if (Vector3.Distance(pistonBody.position, pistonMaxPosition.position) < 0.01f)
            {
                isReturning = false;
                ResetObjects();
            }
        }
        else
        {
            if (Vector3.Distance(pistonBody.position, pistonMaxPosition.position) >= 0.01f)
                isReturning = true;
        }
    }

    private void ActivateFunction()
    {
        isReturning = true;
        if (functionType == FunctionType.SingleShow && objectToShow != null)
            objectToShow.SetActive(true);


        if (functionType == FunctionType.MultipleShow)
        {
            if (objectCoroutines.Count != 0)
                StopAllCoroutines();
            foreach (ObjectDisplay obj in objectDisplays)
                objectCoroutines.Add(StartCoroutine(HandleObjectDisplay(obj)));
        }
    }

    private IEnumerator HandleObjectDisplay(ObjectDisplay obj)
    {
        yield return new WaitForSeconds(obj.delayToShow);
        obj.obj.SetActive(true);
        yield return new WaitForSeconds(obj.activeDuration);

        while (true)
        {
            obj.obj.SetActive(false);
            yield return new WaitForSeconds(obj.inactiveDuration);
            obj.obj.SetActive(true);
            yield return new WaitForSeconds(obj.activeDuration);
        }
    }

    private void ResetObjects()
    {
        if (functionType == FunctionType.SingleShow && objectToShow != null)
        {
            objectToShow.SetActive(false);
        }

        if (functionType == FunctionType.MultipleShow)
        {
            StopAllCoroutines();
            foreach (var obj in objectDisplays)
            {
                obj.obj.SetActive(obj.originStatus);
            }
            
        }
    }
}
