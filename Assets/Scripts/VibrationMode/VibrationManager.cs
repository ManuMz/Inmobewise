using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class VibrationManager : MonoBehaviour
{
    [System.Serializable]
    public class AdvancedVibrations
    {        
        public TextAsset AHAPFile;
        public MMNVAndroidWaveFormAsset WaveFormAsset;
    }
    public List<AdvancedVibrations> vibrations;

    private bool EnableVibration = true;

    public void PlayVibration(string vib)
    {
        //print("Entra aquí");
        switch(vib)
        {
            case "warning":
                MMVibrationManager.Haptic(HapticTypes.Warning);
                //print("Play warning");
                break;
            case "success":
                MMVibrationManager.Haptic(HapticTypes.Success);
                //print("Play success");
                break;                
            case "selection":
                MMVibrationManager.Haptic(HapticTypes.Selection);
                //print("Play selection");
                break;
            case "dice":
                MMVibrationManager.AdvancedHapticPattern(vibrations[0].AHAPFile.text, vibrations[0].WaveFormAsset.WaveForm.Pattern, vibrations[0].WaveFormAsset.WaveForm.Amplitudes, -1, null, null, null, -1, HapticTypes.LightImpact);
                //print("Play dice");
                break;
            default:
                break;
        }
        //
    }
}
