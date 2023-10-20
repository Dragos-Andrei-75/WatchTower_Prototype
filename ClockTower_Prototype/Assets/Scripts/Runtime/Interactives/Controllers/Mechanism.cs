using UnityEngine;

public class Mechanism : MonoBehaviour
{
    [Header("Mechanism Object and Component References")]
    [SerializeField] private Transform mechanismTransform;

    [Header("Child Object and Component References")]
    [SerializeField] private GameObject[] doors;
    [SerializeField] private BoxCollider[] doorColliders;
    [SerializeField] private Door[] doorScripts;

    [Header("Mechanism Attributes")]
    [SerializeField] private Arrangements arrangement = Arrangements.Horizontal;
    [SerializeField] private bool rearranged;
    [SerializeField] private bool addedLink;
    [SerializeField] private bool addedSwitch;
    [SerializeField] private bool addedSensor;

    public enum Arrangements : ushort { Horizontal, Vertical };

    public Arrangements Arrangement
    {
        get
        {
            return arrangement;
        }
        set
        {
            arrangement = value;
            rearranged = true;

            Allign();
        }
    }

    public void SetupMechanism()
    {
        if (doors.Length != mechanismTransform.GetComponentsInChildren<Door>().Length || rearranged == true)
        {
            MobileGet();
            MobilePosition();

            if (rearranged == true) rearranged = false;
            else if (addedLink == true) Unlink();

            if (addedSwitch == true) SwitchPosition();
            if (addedSensor == true) SensorPosition(gameObject.GetComponent<SensorMobile>());
        }
    }

    public void Allign()
    {
        RaycastHit rayCastHit;
        LayerMask layerSurface;
        Vector3 positionMechanism;
        Vector3 direction;
        float offset;
        bool hit;

        if (doors.Length == 1 || arrangement == Arrangements.Horizontal) offset = doorColliders[0].size.y / 2;
        else offset = doorColliders[0].size.y;

        direction = -doors[doors.Length - 1].transform.up;
        layerSurface = LayerMask.GetMask("Surface");

        hit = Physics.Raycast(mechanismTransform.position, direction, out rayCastHit, Mathf.Infinity, layerSurface, QueryTriggerInteraction.Ignore);

        if (hit == false) hit = Physics.Raycast(mechanismTransform.position, -direction, out rayCastHit, Mathf.Infinity, layerSurface, QueryTriggerInteraction.Ignore);

        if (hit == true)
        {
            Vector3 hitPointRound;
            float hitPointRoundX;
            float hitPointRoundY;
            float hitPointRoundZ;

            hitPointRoundX = Mathf.Round(rayCastHit.point.x * 100) / 100;
            hitPointRoundY = Mathf.Round(rayCastHit.point.y * 100) / 100;
            hitPointRoundZ = Mathf.Round(rayCastHit.point.z * 100) / 100;

            hitPointRound = new Vector3(hitPointRoundX, hitPointRoundY, hitPointRoundZ);

            positionMechanism = new Vector3(hitPointRound.x, hitPointRound.y + offset, hitPointRound.z);

            mechanismTransform.position = positionMechanism;
        }
        else
        {
            Debug.Log("No surface to allign the mechanism with.");
        }
    }

    public void MobileAdd()
    {
        if (doors.Length < 2)
        {
            GameObject mobile;

            mobile = Instantiate(doors[0], mechanismTransform);
            mobile.name = doors[0].name;
        }
    }

    public void MobileRemove()
    {
        if (doors.Length > 1) DestroyImmediate(doors[doors.Length - 1]);
    }

    public void MobileGet()
    {
        doorScripts = mechanismTransform.GetComponentsInChildren<Door>();

        doorColliders = new BoxCollider[doorScripts.Length];
        doors = new GameObject[doorColliders.Length];

        for (int i = 0; i < doorScripts.Length; i++)
        {
            doors[i] = doorScripts[i].gameObject;
            doorColliders[i] = doors[i].GetComponent<BoxCollider>();
        }
    }

    public void MobilePosition()
    {
        if (doors.Length == 1)
        {
            doors[0].transform.localPosition = Vector3.zero;
        }
        else
        {
            Vector3 positionMobile;
            Vector3 increment;

            if (arrangement == Arrangements.Horizontal)
            {
                positionMobile = new Vector3(-doorColliders[0].size.x / 2, 0, 0);
                increment = new Vector3(doorColliders[0].size.x, 0, 0);
            }
            else
            {
                positionMobile = new Vector3(0, -doorColliders[0].size.y / 2, 0);
                increment = new Vector3(0, doorColliders[0].size.y, 0);
            }

            doors[0].transform.localPosition = positionMobile;
            doors[1].transform.localPosition = positionMobile + increment;

            if (doorScripts[1].DoorType == Door.DoorTypes.Rotary)
            {
                doors[1].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                doors[1].transform.localPosition -= increment;
            }
        }
    }

    public void Link()
    {
        if (addedSwitch == true || addedSensor == true) return;

        if (gameObject.GetComponent<Controller>() == null && doors.Length == 2)
        {
            gameObject.AddComponent<Controller>();

            for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Linked = true;

            addedLink = true;
        }
    }

    public void Unlink()
    {
        if ((gameObject.GetComponent<Controller>() != null && doors.Length == 2) || doors.Length == 1)
        {
            DestroyImmediate(gameObject.GetComponent<Controller>());

            for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Linked = false;

            addedLink = false;
        }
    }

    public void SwitchAdd()
    {
        if (addedLink == true || addedSensor == true) return;

        if (gameObject.GetComponentsInChildren<Switch>().Length < 2)
        {
            GameObject resource = Resources.Load<GameObject>("Interactives/Immobiles/Switches/Button");

            GameObject _switch = Instantiate(resource, mechanismTransform.position, mechanismTransform.rotation);

            _switch.transform.SetParent(mechanismTransform);
            _switch.name = resource.name;

            if (gameObject.GetComponentsInChildren<Switch>().Length == 1)
            {
                for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Controlled = true;

                addedSwitch = true;
            }

            SwitchPosition();
        }
    }

    public void SwitchRemove()
    {
        if (gameObject.GetComponentsInChildren<Switch>().Length != 0)
        {
            Switch[] switches = mechanismTransform.GetComponentsInChildren<Switch>();

            DestroyImmediate(switches[switches.Length - 1].gameObject);

            if (switches.Length == 1)
            {
                for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Controlled = false;

                addedSwitch = false;
            }
        }
    }

    public void SwitchPosition()
    {
        if (mechanismTransform.GetComponentInChildren<Switch>() != null)
        {
            Switch[] switches = mechanismTransform.GetComponentsInChildren<Switch>();
            Vector3 positionSwitch;
            float mobileSizeX = doorColliders[0].size.x;
            float mobileSizeY = doorColliders[0].size.y;
            float mobileSizeZ = doorColliders[0].size.z;
            float switchSizeZ = switches[0].transform.GetComponent<BoxCollider>().size.z;
            float offset = 1.25f;

            positionSwitch = new Vector3(-(mobileSizeX / 2 + offset), -mobileSizeY / 2 + offset, mobileSizeZ / 2 + switchSizeZ / 2);

            switches[0].transform.localPosition = doors[0].transform.localPosition + positionSwitch;

            if (switches.Length > 1)
            {
                Vector3 positionReference = switches[0].transform.localPosition;

                switches[1].transform.localPosition = new Vector3(positionReference.x, positionReference.y, -positionReference.z);
                switches[1].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }
    }

    public void SensorAdd()
    {
        if (addedLink == true || addedSwitch == true) return;

        if (gameObject.GetComponent<SensorMobile>() == null)
        {
            gameObject.AddComponent<BoxCollider>().isTrigger = true;

            SensorPosition(gameObject.AddComponent<SensorMobile>());

            for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Automatic = true;

            addedSensor = true;
        }
    }

    public void SensorRemove()
    {
        if (gameObject.GetComponent<SensorMobile>() != null)
        {
            DestroyImmediate(gameObject.GetComponent<SensorMobile>());
            DestroyImmediate(gameObject.GetComponent<BoxCollider>());

            for (int i = 0; i < doorScripts.Length; i++) doorScripts[i].Automatic = false;

            addedSensor = false;
        }
    }

    private void SensorPosition(SensorMobile sensorMobile)
    {
        sensorMobile.Arrangement = Arrangement;
        sensorMobile.SetupSensor();
    }
}
