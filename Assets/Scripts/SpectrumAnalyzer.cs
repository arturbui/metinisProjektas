using UnityEngine;
using System.Collections.Generic;


public class SpectrumAnalyzer : MonoBehaviour
{
    public AnalyzerSettings settings; 

    //private
    private float[] spectrum; 
    private List<GameObject> pillars; 
    private GameObject folder;
    private bool isBuilding; 


    void Start()
    {
        isBuilding = true;
        CreatePillarsByShapes();
    }

    private void CreatePillarsByShapes()
    {
        GameObject currentPrefabType = settings.pillar.type == PillarTypes.Cylinder ? settings.Prefabs.CylPrefab : settings.Prefabs.BoxPrefab;
       
        
        pillars = MathB.ShapesOfGameObjects(currentPrefabType, settings.pillar.radius, (int) settings.pillar.amount,settings.pillar.shape);
       
        folder = new GameObject("Pillars-" + pillars.Count);
        folder.transform.SetParent(transform);

        foreach (var piller in pillars)
        {
            piller.transform.SetParent(folder.transform);
        }

        isBuilding = false;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.R)) Rebuild();
        if (isBuilding) return;

        spectrum = AudioListener.GetSpectrumData((int) settings.spectrum.sampleRate, 0, settings.spectrum.FffWindowType);


        for (int i = 0; i < pillars.Count; i++) 
        {
            float level = spectrum[i]*settings.pillar.sensitivity*Time.deltaTime*1000; 

            Vector3 previousScale = pillars[i].transform.localScale;
            previousScale.y = Mathf.Lerp(previousScale.y, level, settings.pillar.speed*Time.deltaTime);
            pillars[i].transform.localScale = previousScale;

            Vector3 pos = pillars[i].transform.position;
            pos.y = previousScale.y*.5f;
            pillars[i].transform.position = pos;
        }
    }

    public void Rebuild()
    {
        if (isBuilding) return;

        isBuilding = true;
        pillars.Clear();
        DestroyImmediate(folder);
        CreatePillarsByShapes();
    }

    private void Reset()
    {
        settings.pillar.Reset();
        settings.spectrum.Reset();
    }

    #region Dynamic floats and for UI sliders

    
    public float PillarShape
    {
        get { return (int) settings.pillar.shape; }
        set
        {
            
            int num = (int) Mathf.Clamp(value, 0, 3);
            settings.pillar.shape = (Shapes) num;
        }
    }

    public float PillarType
    {
        get { return (int) settings.pillar.type; }
        set
        {
            
            int num = (int)Mathf.Clamp(value, 0, 2); 
            settings.pillar.type = (PillarTypes) num;
        }
    }

    public float Amount
    {
        get { return settings.pillar.amount; }
        set
        {
            settings.pillar.amount = Mathf.Clamp(value, 4, 128);
            
        }
    }

    public float Radius
    {
        get { return settings.pillar.radius; }
        set { settings.pillar.radius = Mathf.Clamp(value, 2, 256); }
    }


    public float Sensitivity
    {
        get { return settings.pillar.sensitivity; }
        set { settings.pillar.sensitivity = Mathf.Clamp(value, 1, 50); }
    }

    public float PillarSpeed
    {
        get { return settings.pillar.speed; }
        set { settings.pillar.speed = Mathf.Clamp(value, 1, 30); }
    }


    public float SampleMethod
    {
        get { return (int) settings.spectrum.FffWindowType; }
        set
        {
            
            int num = (int)Mathf.Clamp(value, 0, 6); 
            settings.spectrum.FffWindowType = (FFTWindow) num;
        }
    }

    public float SampleRate
    {
        get { return (int) settings.spectrum.sampleRate; }
        set
        {
            int num = (int) Mathf.Pow(2, 7 + value);
            settings.spectrum.sampleRate = (SampleRates) num;
        }
    }

    #endregion
}