using UnityEngine;
using System.Collections;

public class WeaponPickUp : MonoBehaviour
{
    [Header("PickUp Object and Component References")]
    [SerializeField] private GameObject pickUp;
    [SerializeField] private Transform pickUpTransform;
    [SerializeField] private SphereCollider pickUpCollider;
    [SerializeField] private MeshRenderer pickUpMesh;

    [Header("PickUp Attributes")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private float speedRotation = 25.0f;
    [SerializeField] private float timeRespawn = 2.5f;
    [SerializeField] private bool canRespawn = false;

    private void Start()
    {
        pickUp = gameObject;
        pickUpTransform = pickUp.transform;
        pickUpCollider = pickUp.GetComponent<SphereCollider>();
        pickUpMesh = pickUp.GetComponent<MeshRenderer>();

        startPosition = pickUpTransform.position;
        startRotation = pickUpTransform.rotation;

        StartCoroutine(Move());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LoadOut loadout = other.gameObject.GetComponentInChildren<LoadOut>();

            pickUpCollider.enabled = false;
            pickUpMesh.enabled = false;

            StartCoroutine(Equip(loadout));
            StopCoroutine(Move());

            if (canRespawn == true)
            {
                StartCoroutine(Respawn());
            }
            else
            {
                Destroy(pickUp);
            }
        }
    }

    private IEnumerator Move()
    {
        while (pickUpMesh.enabled == true)
        {
            pickUpTransform.Rotate(pickUpTransform.up * speedRotation * Time.deltaTime);
            yield return null;
        }

        yield break;
    }

    private IEnumerator Equip(LoadOut loadOut)
    {
        //Instantiate(weapon, loadOut.transform);
        yield break;
    }

    private IEnumerator Respawn()
    {
        pickUpTransform.position = startPosition;
        pickUpTransform.rotation = startRotation;

        yield return new WaitForSeconds(timeRespawn);

        pickUpCollider.enabled = true;
        pickUpMesh.enabled = true;

        StartCoroutine(Move());

        yield break;
    }
}
