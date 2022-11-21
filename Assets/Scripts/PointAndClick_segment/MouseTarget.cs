using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    [field: SerializeField] public InteractElement Target { get; private set; }
    [field: SerializeField] public InteractElement InventoryPicked { get; set; }
    [field: SerializeField] public bool IsInteractionDone { get; private set; }
    [field: SerializeField] public bool IsInteractionSucess { get; private set; }

    Ray ray;
    RaycastHit hit;

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            GetTarget();
        }

        if (IsInteractionSucess)
        {
            IsInteractionSucess = false;
        }
    }

    private void GetTarget()
    {
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider != null)
            {
                hit.collider.gameObject.TryGetComponent(out InteractElement interactElement);
                if (interactElement == null) { return; }

                Target = interactElement;
                Target.Use();
            }
        }
    }
}
