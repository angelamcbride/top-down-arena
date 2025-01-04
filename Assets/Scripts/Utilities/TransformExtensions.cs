using UnityEngine;

public static class TransformExtensions
{
    public static Transform ChildWithTag(this Transform parent, string tag)
    {
        // Check direct children
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }

            // Recursively search in the child's children
            Transform found = child.ChildWithTag(tag);
            if (found != null)
            {
                return found;
            }
        }

        // If no child with the tag is found
        return null;
    }

public static Transform SiblingWithTag(this Transform transform, string tag)
    {
        Transform parent = transform.parent;
        if (parent == null) return null;

        foreach (Transform sibling in parent)
        {
            if (sibling != transform && sibling.CompareTag(tag))
            {
                return sibling;
            }
        }
        return null;
    }
}
