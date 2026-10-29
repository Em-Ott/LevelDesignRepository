using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    // Assign your Skybox Materials in the Unity Inspector
    public Material daySkybox;
    public Material nightSkybox;

    public void SetDaySkybox()
    {
        if (daySkybox != null)
        {
            // 1. Assign the material
            RenderSettings.skybox = daySkybox;
            
            // 2. Force Unity to update the scene's ambient lighting
            DynamicGI.UpdateEnvironment();
        }
    }

    public void SetNightSkybox()
    {
        if (nightSkybox != null)
        {
            RenderSettings.skybox = nightSkybox;
            DynamicGI.UpdateEnvironment(); 
        }
    }
}
