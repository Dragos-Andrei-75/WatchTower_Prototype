using UnityEngine;
using System.Collections;

public class PickUpWeapon : MonoBehaviour
{
    [Header("PickUp Object and Component References")]
    [SerializeField] private GameObject pickUp;
    [SerializeField] private Transform pickUpTransform;
    [SerializeField] private SphereCollider pickUpCollider;
    [SerializeField] private MeshRenderer pickUpMesh;

    [Header("Player Object and Component References")]
    [SerializeField] private LoadOut loadOut;

    [Header("PickUp Attributes")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Vector3 holdPosition;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private float speedRotation = 25.0f;
    [SerializeField] private float timeRespawn = 2.5f;
    [SerializeField] private bool canRespawn = false;

    public delegate void WeaponEquip(int weaponIndex);
    public static event WeaponEquip OnWeaponEquip;

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
            loadOut = other.gameObject.GetComponentInChildren<LoadOut>();

            pickUpCollider.enabled = false;
            pickUpMesh.enabled = false;

            StopCoroutine(Move());
            StartCoroutine(Equip());

            if (canRespawn == true) StartCoroutine(Respawn());
            else Destroy(pickUp);
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

    private IEnumerator Equip()
    {
        WeaponData weaponData = weapon.transform.GetComponent<Weapon>().WeaponData;

        if (loadOut.Weapons[weaponData.index] == null)
        {
            Instantiate(weapon, loadOut.transform);

            weapon.transform.localPosition = weaponData.position;
            loadOut.Weapons[weaponData.index] = weapon.transform;

            if (OnWeaponEquip != null) OnWeaponEquip(weaponData.index);
        }
        else
        {
            weaponData.ammunition = weaponData.ammunitionCapacity;
        }

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
