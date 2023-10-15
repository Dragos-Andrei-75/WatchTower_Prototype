using UnityEngine;

public class Transporter : Linear
{
    [Header("Transporter Attributes")]
    [SerializeField] private bool reactive = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (reactive == true)
        {
            if (coroutineActive != null)
            {
                Vector3 direction = PositionTarget - InteractiveTransform.position;
                Vector3 interactiveToObject = collision.transform.position - InteractiveTransform.position;
                Vector3 objectSize = collision.transform.GetComponent<MeshRenderer>().bounds.size;
                float objectSizeMax = Mathf.Max(Mathf.Max(objectSize.x, objectSize.y), objectSize.z);

                if (Vector3.Dot(direction, interactiveToObject) > 0 && Vector3.Distance(InteractiveTransform.position, PositionTarget) <= objectSizeMax) Interact();
            }
        }

        if (collision.contacts[0].point.y > InteractiveTransform.position.y) collision.transform.SetParent(InteractiveTransform);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.parent == InteractiveTransform) collision.transform.SetParent(null);
    }
}
