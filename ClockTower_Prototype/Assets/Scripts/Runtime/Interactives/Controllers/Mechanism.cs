using UnityEngine;

public class Mechanism : MonoBehaviour
{
    [Header("Mechanism Object and Component References")]
    [SerializeField] private Transform mechanismTransform;

    [Header("Child Object and Component References")]
    [SerializeField] private GameObject[] mobileGameObjects;
    [SerializeField] private BoxCollider[] mobileColliders;
    [SerializeField] private Mobile[] mobileScripts;

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
        if (mobileGameObjects.Length != mechanismTransform.GetComponentsInChildren<Mobile>().Length || rearranged == true)
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

        if (mobileGameObjects.Length == 1 || arrangement == Arrangements.Horizontal) offset = mobileColliders[0].size.y / 2;
        else offset = mobileColliders[0].size.y;

        direction = -mobileGameObjects[mobileGameObjects.Length - 1].transform.up;
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
        if (mobileGameObjects.Length < 2)
        {
            GameObject mobile;

            mobile = Instantiate(mobileGameObjects[0], mechanismTransform);
            mobile.name = mobileGameObjects[0].name;
        }
    }

    public void MobileRemove()
    {
        if (mobileGameObjects.Length > 1) DestroyImmediate(mobileGameObjects[mobileGameObjects.Length - 1]);
    }

    public void MobileGet()
    {
        mobileScripts = mechanismTransform.GetComponentsInChildren<Mobile>();

        mobileColliders = new BoxCollider[mobileScripts.Length];
        mobileGameObjects = new GameObject[mobileColliders.Length];

        for (int i = 0; i < mobileScripts.Length; i++)
        {
            mobileGameObjects[i] = mobileScripts[i].gameObject;
            mobileColliders[i] = mobileGameObjects[i].GetComponent<BoxCollider>();
        }
    }

    public void MobilePosition()
    {
        if (mobileGameObjects.Length == 1)
        {
            mobileGameObjects[0].transform.localPosition = Vector3.zero;
        }
        else
        {
            Vector3 positionMobile;
            Vector3 increment;

            if (arrangement == Arrangements.Horizontal)
            {
                positionMobile = new Vector3(-mobileColliders[0].size.x / 2, 0, 0);
                increment = new Vector3(mobileColliders[0].size.x, 0, 0);
            }
            else
            {
                positionMobile = new Vector3(0, -mobileColliders[0].size.y / 2, 0);
                increment = new Vector3(0, mobileColliders[0].size.y, 0);
            }

            mobileGameObjects[0].transform.localPosition = positionMobile;
            mobileGameObjects[1].transform.localPosition = positionMobile + increment;

            if (mobileScripts[1].MotionType == Mobile.MotionTypes.Rotary)
            {
                mobileGameObjects[1].transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                mobileGameObjects[1].transform.localPosition -= increment;
            }
        }
    }

    public void Link()
    {
        if (addedSwitch == true || addedSensor == true) return;

        if (gameObject.GetComponent<Controller>() == null && mobileGameObjects.Length == 2)
        {
            gameObject.AddComponent<Controller>();

            for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Linked = true;

            addedLink = true;
        }
    }

    public void Unlink()
    {
        if ((gameObject.GetComponent<Controller>() != null && mobileGameObjects.Length == 2) || mobileGameObjects.Length == 1)
        {
            DestroyImmediate(gameObject.GetComponent<Controller>());

            for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Linked = false;

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
                for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Controlled = true;

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
                for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Controlled = false;

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
            float mobileSizeX = mobileColliders[0].size.x;
            float mobileSizeY = mobileColliders[0].size.y;
            float mobileSizeZ = mobileColliders[0].size.z;
            float switchSizeZ = switches[0].transform.GetComponent<BoxCollider>().size.z;
            float offset = 1.25f;

            positionSwitch = new Vector3(-(mobileSizeX / 2 + offset), -mobileSizeY / 2 + offset, mobileSizeZ / 2 + switchSizeZ / 2);

            switches[0].transform.localPosition = mobileGameObjects[0].transform.localPosition + positionSwitch;

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

            for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Automatic = true;

            addedSensor = true;
        }
    }

    public void SensorRemove()
    {
        if (gameObject.GetComponent<SensorMobile>() != null)
        {
            DestroyImmediate(gameObject.GetComponent<SensorMobile>());
            DestroyImmediate(gameObject.GetComponent<BoxCollider>());

            for (int i = 0; i < mobileScripts.Length; i++) mobileScripts[i].Automatic = false;

            addedSensor = false;
        }
    }

    private void SensorPosition(SensorMobile sensorMobile)
    {
        sensorMobile.Arrangement = Arrangement;
        sensorMobile.SetupSensor();
    }
}
