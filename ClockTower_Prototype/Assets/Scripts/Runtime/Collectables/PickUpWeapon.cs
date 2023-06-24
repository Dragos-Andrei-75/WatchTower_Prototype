using UnityEngine;
using System.Collections;

public class PickUpWeapon : MonoBehaviour
{
    [Header("Character Object and Component References")]
    [SerializeField] private CharacterLoadOut loadOut;

    [Header("PickUp Object and Component References")]
    [SerializeField] private GameObject pickUp;
    [SerializeField] private Transform pickUpTransform;
    [SerializeField] private SphereCollider pickUpCollider;
    [SerializeField] private MeshRenderer pickUpMesh;

    [Header("PickUp Attributes")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private float speedRotation = 25.0f;
    [SerializeField] private float timeRespawn = 2.5f;
    [SerializeField] private bool canRespawn = false;

    [Header("Weapon Attributes")]
    [SerializeField] private WeaponData[] weaponData;
    [SerializeField] private Vector3 position;
    [SerializeField] private int index;

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

        weaponData = new WeaponData[weapon.transform.GetComponents<Weapon>().Length];

        for (int i = 0; i < weaponData.Length; i++) weaponData[i] = weapon.transform.GetComponents<Weapon>()[i].WeaponData;

        StartCoroutine(Move());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            loadOut = other.gameObject.GetComponentInChildren<CharacterLoadOut>();
            loadOut.enabled = true;

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
        if (loadOut.Weapons[index] == null)
        {
            Instantiate(weapon, loadOut.transform);

            weapon.transform.localPosition = position;
            loadOut.Weapons[index] = weapon.transform;

            if (OnWeaponEquip != null) OnWeaponEquip(index);
        }
        else
        {
            for (int i = 0; i < weaponData.Length; i++) weaponData[i].Ammunition = weaponData[i].AmmunitionCapacity;
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
