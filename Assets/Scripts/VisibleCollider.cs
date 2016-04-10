using UnityEngine;

namespace Ritualist
{
    [ExecuteInEditMode]
    public class VisibleCollider : MonoBehaviour
    {

        // The Collider itself
        private PolygonCollider2D thisCollider;
        // array of collider points
        private Vector2[] points;
        // the transform position of the collider
        private Vector3 _t;

        void Start()
        {
            thisCollider = GetComponent<PolygonCollider2D>();
            points = thisCollider.points;
            _t = thisCollider.transform.position;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            // for every point (except for the last one), draw line to the next point
            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(points[i].x + _t.x, points[i].y + _t.y), new Vector3(points[i + 1].x + _t.x, points[i + 1].y + _t.y));
            }
        }

    }
}
