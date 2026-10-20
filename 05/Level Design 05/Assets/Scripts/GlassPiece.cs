using UnityEngine;

public class GlassPiece : MonoBehaviour
{
    [SerializeField]
    public GameObject glass;

    public GameObject GetGlass() {
        return glass;
    }
}
