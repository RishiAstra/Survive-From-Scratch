﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLinks : MonoBehaviour
{
    private const float MaxDirectionFactor = 2f;
	private const float minDot = -0.2f;

    public Transform[] linkPoints;
    public Vector3 b;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// use the right vectors of link points to match up the best location for this BuildingLinks
    /// </summary>
    /// <param name="other"></param>
    /// <param name="myPosition">the predicted position of this building</param>
    /// <returns></returns>
    public Vector3 GetBestLinkedPosition(BuildingLinks other, Vector3 myPosition, Vector3 normal)
    {
        if (linkPoints.Length < 1 || other.linkPoints.Length < 1)
        {
            Debug.LogError("No link points");
            return myPosition;
        }

        List<LinkPointMatch> matches = new List<LinkPointMatch>();

        for (int i = 0; i < linkPoints.Length; i++)
        {
            for (int j = 0; j < other.linkPoints.Length; j++)
            {
                Vector3 s1 = GetSize() / 2f;
                Vector3 s2 = other.GetSize() / 2f;
                Vector3 p1 = other.linkPoints[j].position - linkPoints[i].position + transform.position;
                Vector3 p2 = other.transform.position;
                if (AABB_Check(p1, p2, s1, s2)) continue;//don't allow things that would result in overlap
														 //Vector3 directionFromPointHitToPotentialBuild = p1 - Player.main.cam.position;// p1 - myPosition;
														 //if (Vector3.Dot(directionFromPointHitToPotentialBuild.normalized, Player.main.cam.forward) < minDot)
														 //{
														 //	print("bad dot");
														 //	continue;
														 //}

				if (Physics.CheckBox(p1, s1, Quaternion.identity, BuildControl.main.avoidOverlap)) continue;

				//bool ok = false;
				//foreach (Transform t in linkPoints)
				//{
				//	if (!Physics.Linecast(Player.main.cam.position, other.linkPoints[j].position - t.position + transform.position, BuildControl.main.blockSight))
				//	{
				//		ok = true;
				//		break;
				//	}
				//}

				//if (!ok) continue;

				float dot = Vector3.Dot(linkPoints[i].right, other.linkPoints[j].right);
                dot = Mathf.Abs(dot);//if it's opposite, they still lie on the same line

                //if both vectors almost lie on the same line
                if (dot > 0.9f)
                {
                    matches.Add(new LinkPointMatch(linkPoints[i].position - transform.position + myPosition, other.linkPoints[j].position, linkPoints[i].position - transform.position));
                }
            }
        }

        if (matches.Count < 1) return myPosition;

        matches.Sort();

        LinkPointMatch m = matches[0];
        return m.theirLinkPoint - m.relativePos;
    }

    public bool AABB_Check(Vector3 p1, Vector3 p2, Vector3 s1, Vector3 s2)
	{
        if (Mathf.Abs(p1.x - p2.x) > (s1.x + s2.x)) return false;
        if (Mathf.Abs(p1.y - p2.y) > (s1.y + s2.y)) return false;
        if (Mathf.Abs(p1.z - p2.z) > (s1.z + s2.z)) return false;

        return true;
    }

    class LinkPointMatch : System.IComparable
	{
        public Vector3 myLinkPoint;
        public Vector3 theirLinkPoint;
        public Vector3 relativePos;

        public LinkPointMatch(Vector3 myLinkPoint, Vector3 theirLinkPoint, Vector3 relativePos)
		{
            this.myLinkPoint = myLinkPoint;
            this.theirLinkPoint = theirLinkPoint;
            this.relativePos = relativePos;
		}

		public int CompareTo(object obj)
		{
            if (obj == null) return 1;
            LinkPointMatch other = obj as LinkPointMatch;
            if (other != null)
            {
                float diff = this.GetDistanceScore() - other.GetDistanceScore();
                if (diff > 0.01f)
                {
                    return 1;
                }
                else if (diff < -0.01f)
                {
                    return -1;
                }
                else return 0;
            }
            else
            {
                throw new System.ArgumentException("Object is not a LinkPointMatch");
            }
		}

        public float GetDistanceScore()
		{
            //TODO: don't depend on the camera pivot being the parent of the camera
            //get the dot between camera's forward and direction to where this option will place
            //float dot = Vector3.Dot(Player.main.cam.forward, ((theirLinkPoint - relativePos) - Player.main.cam.parent.position).normalized);
            //Vector3 diff = 
            ////1 if parallel, more if perpendicular (capped at perpendicular, further than 90 deg apart doesn't do more)
            //float mult = 1 + Mathf.Clamp01(1-dot) * MaxDirectionFactor;

            return Vector3.Distance(myLinkPoint, theirLinkPoint);// * mult;
		}
	}

    public Vector3 GetSize()
    {
        Vector3 size = transform.TransformVector(b);
        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        size.z = Mathf.Abs(size.z);
        return size;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.Scale(GetSize(), transform.lossyScale));
    }
}