using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private LayerMask customerLayer;

    private void Start()
    {
        mainCamera = Camera.main;
        customerLayer = LayerMask.GetMask("Customer");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }
    }

    private void HandleTap(Vector3 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Customer"))
            {
                Customer customer = hit.collider.GetComponent<Customer>();
                if (customer != null)
                {
                    GameManager.Instance.Customers.TapCustomer(customer);
                    GameManager.Instance.Audio.PlaySound("tap");
                    return;
                }
            }
        }
    }
}
