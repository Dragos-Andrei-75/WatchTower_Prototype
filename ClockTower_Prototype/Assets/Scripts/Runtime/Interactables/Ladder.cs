using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Object and Components References")]
    [SerializeField] private Transform ladderTransform;
    [SerializeField] private BoxCollider ladderCollider;

    [Header("Object and Component References")]
    [SerializeField] private GameObject[] rungs;
    [SerializeField] private GameObject rung;
    [SerializeField] private GameObject sideRailLeft;
    [SerializeField] private GameObject sideRailRight;

    [Header("Ladder Attributes")]
    [SerializeField] private float ladderHeight = 1.0f;
    [SerializeField] private float ladderWidthX = 1.0f;
    [SerializeField] private float ladderWidthZ = 1.0f;
    [SerializeField] private float rungOffset = 0.5f;
    [SerializeField] private float rungOffsetMin = 0.05f;
    [SerializeField] private float rungOffsetMax = 0.95f;
    [SerializeField] private float ladderColliderOffset = 0.25f;

    public float LadderHeight
    {
        get { return ladderHeight; }
        set { if (value >= 1) ladderHeight = value; }
    }

    public float RungOffset
    {
        get { return rungOffset; }
        set { rungOffset = Mathf.Clamp(value, rungOffsetMin, rungOffsetMax); }
    }

    public float RungOffsetMin
    {
        get { return rungOffsetMin; }
    }

    public float RungOffsetMax
    {
        get { return rungOffsetMax; }
    }

    public void LadderSetUp()
    {
        ladderTransform = gameObject.transform;
        ladderCollider = ladderTransform.GetComponent<BoxCollider>();

        sideRailLeft = ladderTransform.GetChild(0).gameObject;
        sideRailRight = ladderTransform.GetChild(1).gameObject;
        rung = ladderTransform.GetChild(2).gameObject;
    }

    public void LadderCreate()
    {
        gameObject.isStatic = sideRailLeft.isStatic = sideRailRight.isStatic = rung.isStatic = true;

        if (rungs.Length == 0)
        {
            rungs = new GameObject[1];
            rungs[0] = rung;
        }

        if (rungs.Length != Mathf.RoundToInt(ladderHeight))
        {
            GameObject[] rungsOld = new GameObject[rungs.Length];

            for (int i = 0; i < rungsOld.Length; i++) rungsOld[i] = rungs[i];

            rungs = new GameObject[Mathf.RoundToInt(ladderHeight)];

            if (rungs.Length > rungsOld.Length) AddRungs(rungsOld);
            else if (rungs.Length < rungsOld.Length) RemoveRungs(rungsOld);
        }

        PositionRungs();

        rungOffsetMin = rung.GetComponent<MeshFilter>().sharedMesh.bounds.size.y / 2;
        rungOffsetMax = ladderHeight / rungs.Length - rungOffsetMin;

        sideRailLeft.transform.localScale = sideRailRight.transform.localScale = new Vector3(ladderWidthX, ladderHeight, ladderWidthZ);

        ladderCollider.size = new Vector3(ladderWidthX, ladderHeight, ladderWidthZ);
        ladderCollider.center = new Vector3(0, -ladderColliderOffset, 0);
    }

    public void LadderReset()
    {
        ladderHeight = 1;
        rungOffset = ladderHeight / 2;

        if (ladderTransform.childCount > 3) for (int i = 3; i <= ladderTransform.childCount - 1; i++) DestroyImmediate(ladderTransform.GetChild(i).gameObject);
    }

    private void AddRungs(GameObject[] rungsOld)
    {
        for (int i = 0; i < rungs.Length; i++)
        {
            if (i < rungsOld.Length) rungs[i] = rungsOld[i];
            else rungs[i] = Instantiate(rung, ladderTransform);
        }
    }

    private void RemoveRungs(GameObject[] rungsOld)
    {
        for (int i = 0; i < rungsOld.Length; i++)
        {
            if (i < rungs.Length) rungs[i] = rungsOld[i];
            else DestroyImmediate(rungsOld[i]);
        }
    }

    private void PositionRungs()
    {
        Vector3 position = new Vector3(0, -ladderHeight / 2 + rungOffset, 0);
        Vector3 increment = new Vector3(0, ladderHeight / rungs.Length, 0);

        for (int i = 0; i < rungs.Length; i++)
        {
            rungs[i].transform.localPosition = position;
            rungs[i].isStatic = true;
            position += increment;
        }
    }
}
