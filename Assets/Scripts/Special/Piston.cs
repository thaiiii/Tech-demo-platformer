using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    [Header("Piston Settings")]
    public List<string> applicableTags; // Các tag được phép đẩy
    private Transform pistonBody; // Pít tông di chuyển
    public Transform pistonMaxPosition; // Vị trí tối đa
    public Transform pistonMinPosition; // Vị trí tối thiểu
    public float extendSpeed = 0.5f; // Tốc độ giãn ra

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onMaxExtend; // Sự kiện khi giãn tối đa
    public UnityEngine.Events.UnityEvent onMinRetract; // Sự kiện khi bị ép tối đa

    public bool isExtending = true; // Trạng thái đang giãn
    public bool isMax = false; //Đã giãn hết mức
    public bool isMin = false;   //Đã thu hết mức

    private void Awake()
    {
        pistonBody = GetComponent<Transform>();
    }

    private void Update()
    {
        //transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);

        // Di chuyển pít tông tùy thuộc vào trạng thái
        if (isExtending)
        {
            RestractPiston(pistonMaxPosition.position, extendSpeed);
        }

    }

    private void RestractPiston(Vector3 targetPosition, float extendSpeed)
    {
        // Di chuyển pít tông
        pistonBody.position = Vector3.MoveTowards(pistonBody.position, targetPosition, extendSpeed * Time.deltaTime);

        // Kiểm tra trạng thái
        if (Vector3.Distance(pistonBody.position, targetPosition) < 0.01f)
        {
            if (!isMax)
                onMaxExtend?.Invoke();

            isMax = true;
            isMin = false;
        }
        else if (Vector3.Distance(pistonBody.position, pistonMinPosition.position) < 0.01f)
        {
            if (!isMin)
                onMinRetract?.Invoke();
            isMin = true;
            isMax = false;
        }
        else
        {
            isMin = false;
            isMax = false;
        }

    }

    #region Only choosen tags con collide with piston
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!applicableTags.Contains(collision.collider.tag))
        {
            // Vô hiệu hóa va chạm với các tag không hợp lệ
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
            return;
        }
    }
    #endregion


}
