using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSchema : MonoBehaviour
{
    public TextAsset FPOXml;
    private FpoImport.FPONode FPO;
    private FpoImport.FPOSlot[] slots;
    private EngineVisualizer engineVisualizer;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()

    {
        if (FPOXml == null) return;

            FpoBuilder fpoBuilder = new FpoBuilder();
            FPO = fpoBuilder.LoadXML(FPOXml);

            slots = GetSlots();
            LoadDuzas();
    }

    private void LoadDuzas()
    {
        if (!TryGetComponent<EngineVisualizer>(out engineVisualizer)) return;
        foreach (FpoImport.FPOSlot Duza in slots)
        {
            if (IsDuza(Duza.name))
            {
                Transform slotTransform = GetSlotTransform(Duza.slotId, transform);
                if (slotTransform == null) continue;
                Debug.Log(slotTransform.name);
                engineVisualizer.AddDuza(Duza,slotTransform);
            }
        }
    }

    private Transform GetSlotTransform(int slotId,Transform searchTransform)
    {
        UnitSlotData slotData;
        if (searchTransform.gameObject.TryGetComponent<UnitSlotData>(out slotData))
        {
            if (slotId == slotData.slotId) return searchTransform;
        }

        foreach (Transform child in searchTransform)
        {
            Transform foundChild = GetSlotTransform(slotId, child);
            if (foundChild != null) return foundChild;
        }
        return null;
    }

    private bool IsDuza(uint StormId)
    {
        switch (StormId)
        {
            //case 0x0C90CFDB: // DuzaFront
            //case 0x6B09D242: // DuzaBack
            //case 0x46A1D839: // DuzaRight
            //case 0x7CA0F41D: // DuzaLeft
            //case 0x601505A9: // DuzaTop
            //case 0x1E70AAF1: // DuzaBottom
            case 0x4FA0627C: // DuzaFront lowercase
            case 0x324D1F2A: // DuzaBack lowercase
            case 0x0591759E: // DuzaRight lowercase
            case 0x25E43975: // DuzaLeft lowercase
            case 0x97E57AD5: // DuzaTop lowercase
                             //case 0x3D632B24: // DuzaBottom lowercase
            case 0x5681AC17: // DuzaBottom lowercase
                return true;
            default:
                return false;
        }
    }
    public FpoImport.FPOSlot[] GetSlots()
    {
        List<FpoImport.FPOSlot> slots = GetSlots(FPO);
        return slots.ToArray();
    }

    public List<FpoImport.FPOSlot> GetSlots(FpoImport.FPONode node) 
    {
        List<FpoImport.FPOSlot> slots = new List<FpoImport.FPOSlot>();

        if (node.slots.Length > 0)
        {
            foreach (FpoImport.FPOSlot slot in node.slots)
            {
                slots.Add(slot);
            }
        }

        foreach(FpoImport.FPONode child in node.children)
        {
            slots.AddRange(GetSlots(child));
        }
        return slots;
    }
}
