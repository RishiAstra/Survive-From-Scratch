using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLinks : MonoBehaviour
{
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
    /// <param name="myPosition>the predicted position of this building</param>
    /// <returns></returns>
    public Vector3 GetBestLinkedPosition(BuildingLinks other, Vector3 myPosition)
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
                float diff = this.GetDistance() - other.GetDistance();
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
                throw new System.ArgumentException("Object is not a Temperature");
            }
		}

        public float GetDistance()
		{
            return Vector3.Distance(myLinkPoint, theirLinkPoint);
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
        Gizmos.DrawWireCube(transform.position, Vector3.Scale(b, transform.lossyScale));
    }
}
