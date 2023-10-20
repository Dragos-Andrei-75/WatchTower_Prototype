using UnityEngine;
using System;
using System.Collections;

public class Transporter : Mobile
{
    [Header("Transporter Attributes")]
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform pointCurrent;
    [SerializeField] private Transform pointTarget;
    [SerializeField] private int pointIndex = 0;
    [SerializeField] private int increment = 1;

    public override void Setup()
    {
        base.Setup();

        if (MobileTransform.parent.childCount == 1)
        {
            pointIndex = 0;

            for (int i = 0; i < 2; i++)
            {
                PointAdd();
                points[i] = MobileTransform.parent.GetChild(i + 1);
            }
        }
        else
        {
            points = MobileTransform.parent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < points.Length - 2; i++)
            {
                points[i + 1] = points[i + 2];
                points[i] = points[i + 1];
            }

            Array.Resize(ref points, points.Length - 2);
        }

        pointCurrent = points[pointIndex];

        MobileTransform.position = pointCurrent.position;
    }

    public override void Interact()
    {
        base.Interact();
        MoveMobile(MoveTransporter());
    }

    private IEnumerator MoveTransporter()
    {
        pointIndex += increment;

        pointTarget = points[pointIndex];

        while (MobileTransform.position != pointTarget.position)
        {
            MobileRigidbody.MovePosition(Vector3.Lerp(pointCurrent.position, pointTarget.position, TimePassed / TimeToMove));
            TimePassed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (pointIndex == 0 || pointIndex == points.Length - 1) increment = -increment;

        MobileTransform.position = pointTarget.position;

        pointCurrent = pointTarget;

        yield break;
    }

    public void PointAdd()
    {
        GameObject pointNew = new GameObject("Position (" + MobileTransform.parent.childCount + ")");
        Transform pointTransform = MobileTransform.parent.childCount == 1 ? MobileTransform : points[points.Length - 1];

        pointNew.transform.position = pointTransform.position;
        pointNew.transform.rotation = pointTransform.rotation;

        pointNew.transform.SetParent(MobileTransform.parent);
    }

    public void PointRemove() => DestroyImmediate(points[points.Length - 1].gameObject);
}
