using UnityEngine;

public class GlassPiece : MonoBehaviour
{
    [SerializeField]
    public GameObject glass;

    [SerializeField]
    public GameObject glassBack;

    public GameObject GetGlass() {
        return glass;
    }

    public GameObject GetGlassBack() {
        return glassBack;
    }
}
